'==========================================================================
'
'  File:        Grapher.vb
'  Location:    NDGrapher <Visual Basic .Net>
'  Description: NDGrapher Grapher控件源文件
'  Created:     2005.07.12.00:44:19(GMT+8:00)
'  Version:     0.5 2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports GraphSystem

#Region " 绘图器 "
''' <summary>绘图器</summary>
''' <remarks>
''' 包括对PictureBox的封装和对快捷键的封装
''' 使用时务必使TabStop = True
''' 即保证代码中(主要是机器生成的代码中)没有 [Grapher].TabStop = False
''' 实现键盘和鼠标事件的处理顺序为最外层->里层->再里层->……，如果无需改变，请勿覆盖键盘和鼠标世间的On'Event'，优先重载Do'Event'，再尝试使用'Event'的事件处理器
''' </remarks>
Public Class Grapher
    Inherits Control

    Public TextBox As TextBox

    Public NDWorld As NDWorld '声明坐标系统
    Public Picture As IPic '图片对象，用于缓冲技术和便于整理程序结构

    Public UseImageBuffer As Boolean = True
    Public Image As Image

    Public RotateDirectionI As Byte 'Z、C键旋转的I轴
    Public RotateDirectionJ As Byte 'Z、C键旋转的J轴
    Public RotateDirectionState As Boolean
    Public ControlMode As ControlModeEnum
    Public Const ControlModeCount As Integer = 3
    Public Enum ControlModeEnum As Byte
        Third '第三人称 Alpha方向键表示世界旋转 Beta方向键表示按屏幕方向移动
        FirstLand '地面上的第一人称 Alpha方向键表示按世界坐标移动 Beta方向键表示视角旋转
        First '第一人称  Alpha方向键表示按屏幕坐标移动 Beta方向键表示视角旋转
    End Enum
    Public RotateSpeed As Double
    Public MoveSpeed As Double
    Public MoveSpeedExp As Double
    Public ZoomExp As Double
    Private KeyPressTimeLimitTickValue As Integer = 300
    Public Property KeyPressTimeLimit() As Double
        Get
            Return KeyPressTimeLimitTickValue / 1000
        End Get
        Set(ByVal Value As Double)
            KeyPressTimeLimitTickValue = Value * 1000
        End Set
    End Property
    Protected MultiClickTimeLimitTickValue As Integer = 100
    Public Property MultiClickTimeLimit() As Double
        Get
            Return MultiClickTimeLimitTickValue / 1000
        End Get
        Set(ByVal Value As Double)
            MultiClickTimeLimitTickValue = Value * 1000
        End Set
    End Property

    Public CamModeSwitch As Keys = Keys.Tab
    Public RotationDirection0 As Keys = Keys.D0
    Public RotationDirection1 As Keys = Keys.D1
    Public RotationDirection2 As Keys = Keys.D2
    Public RotationDirection3 As Keys = Keys.D3
    Public RotationDirection4 As Keys = Keys.D4
    Public RotationDirection5 As Keys = Keys.D5
    Public RotationDirection6 As Keys = Keys.D6
    Public RotationDirection7 As Keys = Keys.D7
    Public RotationDirection8 As Keys = Keys.D8
    Public RotationDirection9 As Keys = Keys.D9
    Public CamAlphaLeft As Keys = Keys.A
    Public CamAlphaRight As Keys = Keys.D
    Public CamAlphaUp As Keys = Keys.W
    Public CamAlphaDown As Keys = Keys.S
    Public CamAnticlockwise As Keys = Keys.Q
    Public CamClockwise As Keys = Keys.E
    Public CamDirectionRotateNegative As Keys = Keys.Z
    Public CamDirectionRotatePositive As Keys = Keys.C
    Public CamBetaLeft As Keys = Keys.Left
    Public CamBetaRight As Keys = Keys.Right
    Public CamBetaUp As Keys = Keys.Up
    Public CamBetaDown As Keys = Keys.Down
    Public CamElevateUp As Keys = Keys.PageUp
    Public CamElevateDown As Keys = Keys.PageDown
    Public CamOrbitLeft As Keys = Keys.Alt Or Keys.Left
    Public CamOrbitRight As Keys = Keys.Alt Or Keys.Right
    Public CamZoomIn As Keys = Keys.Alt Or Keys.Up
    Public CamZoomeOut As Keys = Keys.Alt Or Keys.Down
    Public CamMoveSpeedDown As Keys = Keys.OemMinus '-
    Public CamMoveSpeedUp As Keys = Keys.Oemplus '=

    Public DisableRotationDirectionControl As Boolean
    Public DisableCamControl As Boolean

    Protected ZoomerValue As Double
    Protected FocusPoint As New Vector '定位点

    Protected NormalKeyLastTick As Integer '上次处理的时间 用来处理过量按键等问题
    Protected ModifierKeyLastTick As Integer '上次处理的时间 用来处理过量按键等问题

    Public Sub New()
        MyBase.New()
        MyBase.SetStyle(ControlStyles.Selectable, True) '使得控件可选
        MyBase.SetStyle(ControlStyles.ResizeRedraw, True) '使得控件大小改变时重绘
        MyBase.SetStyle(ControlStyles.Opaque, True) '使得控件不绘制背景
        MyBase.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True) '禁止控件自行绘图
        MyBase.TabStop = True
        MyBase.Visible = True
        MyBase.Enabled = True

        ReturnToDefaultState()
    End Sub

    Public Sub Initialize(ByVal NDWorld As NDWorld, ByVal Picture As IPic)
        Me.NDWorld = NDWorld
        Me.Picture = Picture
        If Picture IsNot Nothing Then Picture.AttachToGrapher(Me, UseImageBuffer)
    End Sub

    '实现键盘和鼠标事件的处理顺序为最外层->里层->再里层->……，如果无需改变，请勿覆盖键盘和鼠标世间的On'Event'
    ''' <summary>鼠标按下时调用，多击时会调用多次，继承时请覆盖DoMouseDown</summary>
    Public Shadows Event MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent MouseDown(Me, e)
        DoMouseDown(e)
    End Sub

    Private MouseTranslateStart As Boolean = False
    Private MouseTranslateDone As Boolean
    Private XTranslate As Integer
    Private YTranslate As Integer
    Private TickTranslate As Integer
    Private Sub StartMouseTranslate(ByVal x As Double, ByVal y As Double)
        MouseTranslateStart = True
        MouseTranslateDone = False
        XTranslate = x
        YTranslate = y
        TickTranslate = Environment.TickCount
    End Sub
    Private Sub DoMouseTranslate(ByVal x As Double, ByVal y As Double)
        If Not MouseTranslateStart Then Return
        If Environment.TickCount - TickTranslate < KeyPressTimeLimitTickValue Then Return
        Dim Pos As Drawing.Point = PointToClient(MousePosition)
        If Pos.X <> XTranslate OrElse Pos.Y <> YTranslate Then
            MouseTranslateDone = True
            Dim d As Double = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2) / 1500
            Dim v As Vector = -New Vector(Pos.X - XTranslate, Pos.Y - YTranslate) * d
            Select Case ControlMode
                Case ControlModeEnum.Third
                    NDWorld.CamMove(v)
                Case ControlModeEnum.FirstLand
                    NDWorld.CamMoveOnLand(v)
                Case ControlModeEnum.First
                    NDWorld.CamMove(v)
            End Select
            XTranslate = Pos.X
            YTranslate = Pos.Y
            Graph()
        End If
    End Sub
    Private Sub EndMouseTranslate(ByVal x As Double, ByVal y As Double)
        DoMouseTranslate(x, y)
        MouseTranslateStart = False
    End Sub
    Private MouseRotateStart As Boolean = False
    Private MouseRotateDone As Boolean
    Private XRotate As Integer
    Private YRotate As Integer
    Private TickRotate As Integer
    Private HiddenRotate As Boolean
    Private Sub StartMouseRotate(ByVal x As Double, ByVal y As Double)
        MouseRotateStart = True
        MouseRotateDone = False
        XRotate = x
        YRotate = y
        TickRotate = Environment.TickCount
        HiddenRotate = False
    End Sub
    Private Sub DoMouseRotate(ByVal x As Double, ByVal y As Double)
        If Not MouseRotateStart Then Return
        If Environment.TickCount - TickRotate < KeyPressTimeLimitTickValue Then Return
        Dim Pos As Drawing.Point = PointToClient(MousePosition)
        If Pos.X <> XRotate OrElse Pos.Y <> YRotate Then
            MouseRotateDone = True
            MouseRotate(Pos.X - XRotate, Pos.Y - YRotate)
            If Not HiddenRotate Then
                Cursor.Hide()
                HiddenRotate = True
            End If
            Cursor.Position = PointToScreen(New Drawing.Point(XRotate, YRotate))
            Graph()
        End If
    End Sub
    Private Sub EndMouseRotate(ByVal x As Double, ByVal y As Double)
        DoMouseRotate(x, y)
        MouseRotateStart = False
        Cursor.Position = PointToScreen(New Drawing.Point(XRotate, YRotate))
        If HiddenRotate Then Cursor.Show()
    End Sub
    ''' <summary>鼠标按下时调用，多击时会调用多次</summary>
    Protected Overridable Sub DoMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Select Case e.Button
            Case Windows.Forms.MouseButtons.Left
                If MouseRotateStart Then EndMouseRotate(e.X, e.Y)
                If Not MouseTranslateStart Then
                    StartMouseTranslate(e.X, e.Y)
                    Return
                End If
                DoMouseTranslate(e.X, e.Y)
            Case Windows.Forms.MouseButtons.Right
                If MouseTranslateStart Then EndMouseTranslate(e.X, e.Y)
                If Not MouseRotateStart Then
                    StartMouseRotate(e.X, e.Y)
                    Return
                End If
                DoMouseRotate(e.X, e.Y)
        End Select
    End Sub

    Private MouseClickStart As Boolean = False
    Private MouseClickDone As Boolean
    Private XClick As Integer
    Private YClick As Integer
    Private CurrentButtonClick As MouseButtons
    Private NumClick As Integer
    Private TimerClick As Threading.Timer
    Private Sub StartMouseClick(ByVal x As Double, ByVal y As Double, ByVal b As MouseButtons)
        MouseClickStart = True
        MouseClickDone = False
        XClick = x
        YClick = y
        CurrentButtonClick = b
        NumClick = 0
        TimerClick = New Threading.Timer(AddressOf EndMouseClickFromOtherThread, Nothing, MultiClickTimeLimitTickValue, Threading.Timeout.Infinite)
    End Sub
    Private Sub DoMouseClick()
        NumClick += 1
    End Sub
    Private Sub EndMouseClick()
        If MouseClickStart Then
            TimerClick.Dispose()
            TimerClick = Nothing
            MouseClickStart = False
            RaiseEvent MouseUp(Me, New MouseEventArgs(CurrentButtonClick, NumClick, XClick, YClick, 0))
            DoMouseUp(New MouseEventArgs(CurrentButtonClick, NumClick, XClick, YClick, 0))
        End If
    End Sub
    Private Sub EndMouseClickFromOtherThread(ByVal o As Object)
        Me.BeginInvoke(Sub() EndMouseClick())
    End Sub
    ''' <summary>鼠标释放时调用，覆盖以处理鼠标释放，多击时仅调用一次，点击次数请调用e.Clicks得到，继承时请覆盖DoMouseUp</summary>
    Public Shadows Event MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        '如果正在拖动则忽略MouseUp
        Dim Ignored = False
        If MouseTranslateStart Then
            Ignored = MouseTranslateDone
            EndMouseTranslate(e.X, e.Y)
            If Ignored Then Return
        End If
        If MouseRotateStart Then
            Ignored = MouseRotateDone
            EndMouseRotate(e.X, e.Y)
            If Ignored Then Return
        End If
        If Not MouseClickStart Then
            StartMouseClick(e.X, e.Y, e.Button)
        End If
        If e.Button = CurrentButtonClick Then
            DoMouseClick()
        Else
            EndMouseClick()
        End If
    End Sub
    ''' <summary>鼠标释放时调用，覆盖以处理鼠标释放，多击时仅调用一次，点击次数请调用e.Clicks得到</summary>
    Protected Overridable Sub DoMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Clicks = 1 AndAlso e.Button = Windows.Forms.MouseButtons.Left Then Me.Select()
    End Sub

    ''' <summary>鼠标移动时调用，继承时请覆盖DoMouseMove</summary>
    Public Shadows Event MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent MouseMove(Me, e)
        DoMouseMove(e)
    End Sub
    ''' <summary>鼠标移动时调用</summary>
    Protected Overridable Sub DoMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        DoMouseTranslate(e.X, e.Y)
        DoMouseRotate(e.X, e.Y)
    End Sub

    ''' <summary>鼠标滚轮转动时调用，继承时请覆盖DoMouseWheel</summary>
    Public Shadows Event MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    Protected Overrides Sub OnMouseWheel(ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent MouseWheel(Me, e)
        DoMouseWheel(e)
    End Sub
    ''' <summary>鼠标滚轮转动时调用</summary>
    Protected Overridable Sub DoMouseWheel(ByVal e As System.Windows.Forms.MouseEventArgs)
        ZoomIn(ZoomExp * e.Delta / 120)
        Graph()
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnKeyDown(e)
        DoKeyDown(e)
    End Sub
    Protected Overridable Sub DoKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.Handled Then Return
        If Environment.TickCount - NormalKeyLastTick < KeyPressTimeLimitTickValue Then Return
        NormalKeyLastTick = Environment.TickCount
        If NDWorld Is Nothing Then Return
        If e.Modifiers <> Keys.None Then
            ModifierKeyLastTick = Environment.TickCount
        End If
        If Not DisableCamControl Then
            Select Case ControlMode
                Case ControlModeEnum.Third
                    Select Case e.KeyData
                        Case CamAlphaLeft
                            NDWorld.CamRotateLeft(RotateSpeed)
                        Case CamAlphaRight
                            NDWorld.CamRotateRight(RotateSpeed)
                        Case CamAlphaUp
                            NDWorld.CamRotateUp(RotateSpeed)
                        Case CamAlphaDown
                            NDWorld.CamRotateDown(RotateSpeed)
                        Case CamAnticlockwise
                            NDWorld.CamRotateAnticlockwise(RotateSpeed)
                        Case CamClockwise
                            NDWorld.CamRotateClockwise(RotateSpeed)
                        Case CamDirectionRotateNegative
                            NDWorld.CamRotate(RotateDirectionJ, RotateDirectionI, RotateSpeed)
                        Case CamDirectionRotatePositive
                            NDWorld.CamRotate(RotateDirectionI, RotateDirectionJ, RotateSpeed)
                        Case CamBetaLeft
                            NDWorld.CamMove(New Vector(-MoveSpeed, 0))
                        Case CamBetaRight
                            NDWorld.CamMove(New Vector(MoveSpeed, 0))
                        Case CamBetaUp
                            NDWorld.CamMove(New Vector(0, 0, MoveSpeed))
                        Case CamBetaDown
                            NDWorld.CamMove(New Vector(0, 0, -MoveSpeed))
                        Case CamElevateUp
                            NDWorld.CamMove(New Vector(0, -MoveSpeed))
                        Case CamElevateDown
                            NDWorld.CamMove(New Vector(0, MoveSpeed))
                        Case CamOrbitLeft
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(0, 1, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamOrbitRight
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(1, 0, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamZoomIn
                            ZoomIn(ZoomExp)
                        Case CamZoomeOut
                            ZoomOut(ZoomExp)
                        Case CamMoveSpeedDown
                            MoveSpeed /= 2 ^ MoveSpeedExp
                        Case CamMoveSpeedUp
                            MoveSpeed *= 2 ^ MoveSpeedExp
                        Case Else
                            Return
                    End Select
                Case ControlModeEnum.FirstLand
                    Select Case e.KeyData
                        Case CamAlphaLeft
                            NDWorld.CamMoveOnLand(New Vector(-MoveSpeed, 0))
                        Case CamAlphaRight
                            NDWorld.CamMoveOnLand(New Vector(MoveSpeed, 0))
                        Case CamAlphaUp
                            NDWorld.CamMoveOnLand(New Vector(0, 0, MoveSpeed))
                        Case CamAlphaDown
                            NDWorld.CamMoveOnLand(New Vector(0, 0, -MoveSpeed))
                        Case CamAnticlockwise
                            NDWorld.CamTurnAnticlockwise(RotateSpeed)
                        Case CamClockwise
                            NDWorld.CamTurnClockwise(RotateSpeed)
                        Case CamDirectionRotateNegative
                            NDWorld.CamTurn(RotateDirectionJ, RotateDirectionI, RotateSpeed)
                        Case CamDirectionRotatePositive
                            NDWorld.CamTurn(RotateDirectionI, RotateDirectionJ, RotateSpeed)
                        Case CamBetaLeft
                            NDWorld.CamTurnAgainstLand(1, 0, RotateSpeed)
                        Case CamBetaRight
                            NDWorld.CamTurnAgainstLand(0, 1, RotateSpeed)
                        Case CamBetaUp
                            NDWorld.CamTurnUp(RotateSpeed)
                        Case CamBetaDown
                            NDWorld.CamTurnDown(RotateSpeed)
                        Case CamElevateUp
                            NDWorld.CamElevateAgainstLand(MoveSpeed)
                        Case CamElevateDown
                            NDWorld.CamElevateAgainstLand(-MoveSpeed)
                        Case CamOrbitLeft
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(0, 1, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamOrbitRight
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(1, 0, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamZoomIn
                            ZoomIn(ZoomExp)
                        Case CamZoomeOut
                            ZoomOut(ZoomExp)
                        Case CamMoveSpeedDown
                            MoveSpeed /= 2 ^ MoveSpeedExp
                        Case CamMoveSpeedUp
                            MoveSpeed *= 2 ^ MoveSpeedExp
                        Case Else
                            Return
                    End Select
                Case ControlModeEnum.First
                    Select Case e.KeyData
                        Case CamAlphaLeft
                            NDWorld.CamMove(New Vector(-MoveSpeed, 0))
                        Case CamAlphaRight
                            NDWorld.CamMove(New Vector(MoveSpeed, 0))
                        Case CamAlphaUp
                            NDWorld.CamMove(New Vector(0, 0, MoveSpeed))
                        Case CamAlphaDown
                            NDWorld.CamMove(New Vector(0, 0, -MoveSpeed))
                        Case CamAnticlockwise
                            NDWorld.CamTurnAnticlockwise(RotateSpeed)
                        Case CamClockwise
                            NDWorld.CamTurnClockwise(RotateSpeed)
                        Case CamDirectionRotateNegative
                            NDWorld.CamTurn(RotateDirectionJ, RotateDirectionI, RotateSpeed)
                        Case CamDirectionRotatePositive
                            NDWorld.CamTurn(RotateDirectionI, RotateDirectionJ, RotateSpeed)
                        Case CamBetaLeft
                            NDWorld.CamTurnLeft(RotateSpeed)
                        Case CamBetaRight
                            NDWorld.CamTurnRight(RotateSpeed)
                        Case CamBetaUp
                            NDWorld.CamTurnUp(RotateSpeed)
                        Case CamBetaDown
                            NDWorld.CamTurnDown(RotateSpeed)
                        Case CamElevateUp
                            NDWorld.CamMove(New Vector(0, -MoveSpeed))
                        Case CamElevateDown
                            NDWorld.CamMove(New Vector(0, MoveSpeed))
                        Case CamOrbitLeft
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(0, 1, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamOrbitRight
                            Dim Focus As Vector = NDWorld.HomotheticTransformation.CPos(FocusPoint)
                            NDWorld.CamOrbit(1, 0, RotateSpeed)
                            NDWorld.HomotheticTransformation.RefPos = Focus - NDWorld.HomotheticTransformation * FocusPoint
                        Case CamZoomIn
                            ZoomIn(ZoomExp)
                        Case CamZoomeOut
                            ZoomOut(ZoomExp)
                        Case CamMoveSpeedDown
                            MoveSpeed /= 2 ^ MoveSpeedExp
                        Case CamMoveSpeedUp
                            MoveSpeed *= 2 ^ MoveSpeedExp
                        Case Else
                            Return
                    End Select
            End Select
            Graph()
        End If
    End Sub

    Protected Overrides Sub OnKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnKeyUp(e)
        DoKeyUp(e)
    End Sub
    Protected Overridable Sub DoKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.Handled Then Return
        Static s As Integer = Environment.TickCount - ModifierKeyLastTick
        If Environment.TickCount - ModifierKeyLastTick < KeyPressTimeLimitTickValue * 5 Then Return
        If Not DisableRotationDirectionControl Then
            Select Case e.KeyData
                Case CamModeSwitch '切换第一人称旋转和第三人称旋转
                    ControlMode = (ControlMode + 1) Mod ControlModeCount
                Case RotationDirection0
                    If RotateDirectionState Then
                        RotateDirectionJ = 0
                    Else
                        RotateDirectionI = 0
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection1
                    If RotateDirectionState Then
                        RotateDirectionJ = 1
                    Else
                        RotateDirectionI = 1
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection2
                    If RotateDirectionState Then
                        RotateDirectionJ = 2
                    Else
                        RotateDirectionI = 2
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection3
                    If RotateDirectionState Then
                        RotateDirectionJ = 3
                    Else
                        RotateDirectionI = 3
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection4
                    If RotateDirectionState Then
                        RotateDirectionJ = 4
                    Else
                        RotateDirectionI = 4
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection5
                    If RotateDirectionState Then
                        RotateDirectionJ = 5
                    Else
                        RotateDirectionI = 5
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection6
                    If RotateDirectionState Then
                        RotateDirectionJ = 6
                    Else
                        RotateDirectionI = 6
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection7
                    If RotateDirectionState Then
                        RotateDirectionJ = 7
                    Else
                        RotateDirectionI = 7
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection8
                    If RotateDirectionState Then
                        RotateDirectionJ = 8
                    Else
                        RotateDirectionI = 8
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
                Case RotationDirection9
                    If RotateDirectionState Then
                        RotateDirectionJ = 9
                    Else
                        RotateDirectionI = 9
                    End If
                    If RotateDirectionI = RotateDirectionJ Then
                        RotateDirectionJ = (RotateDirectionJ + 1) Mod NDWorld.HomotheticTransformation.Dimension
                    End If
                    RotateDirectionState = Not RotateDirectionState
            End Select
        End If
    End Sub

    ''' <summary>是否输入键 用于禁止输入预处理</summary>
    Protected Overrides Function IsInputKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Return True
    End Function

    Public Sub ReturnToDefaultState()
        RotateSpeed = CRad(10)
        MoveSpeed = 0.25
        MoveSpeedExp = 0.25
        ZoomExp = 0.25
    End Sub
    Public Sub ZoomIn(ByVal ZoomExp As Double)
        If NDWorld Is Nothing Then Return
        Dim d As Double = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2)
        If d <= 0 Then Return
        If d < 2 AndAlso ZoomExp > 0 Then Return
        d = d * (1 - 2 ^ (-ZoomExp))
        NDWorld.CamMove(New Vector(0, 0, d))
    End Sub
    Public Sub ZoomOut(ByVal ZoomExp As Double)
        If NDWorld Is Nothing Then Return
        Dim d As Double = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2)
        If d <= 0 Then Return
        If d < 2 AndAlso ZoomExp < 0 Then Return
        d = d * (1 - 2 ^ ZoomExp)
        NDWorld.CamMove(New Vector(0, 0, d))
    End Sub
    Public Sub MouseRotate(ByVal dx As Integer, ByVal dy As Integer)
        '转轴 过FocusPoint点平行于屏幕，垂直于(dx, dy)的直线

        '获得旋转时所在坐标系的x, y, z轴方向
        Dim rx As New Vector(dx, dy)
        Dim theta As Double = rx.Norm
        If Equal(theta, 0, 1) Then Return
        rx = rx * (1 / theta)
        Dim ry As New Vector(0, 0, 1)
        Dim rz As Vector = Vector.VecPro(True, rx, ry).Dir

        '获得旋转时所在坐标系到屏幕空间的变换
        Dim M As New Matrix(3)
        M.Column(0) = rx
        M.Column(1) = ry
        M.Column(2) = rz
        Dim T As New HomotheticTransformation(M, NDWorld.HomotheticTransformation.CPos(FocusPoint))

        'NDWorld.HomotheticTransformation = T * C
        'NDWorld.HomotheticTransformation(FocusPoint) = (T * C)(FocusPoint)
        'NDWorld.HomotheticTransformation(FocusPoint) = (T * K * C)(FocusPoint)
        Dim C As HomotheticTransformation = T.Invert * NDWorld.HomotheticTransformation
        Dim K As New HomotheticTransformation
        K.Rotate(0, 1, CRad(theta / 8))
        NDWorld.HomotheticTransformation = T * K * C
    End Sub

    Public Sub CheckSize()
        '确保大小正确
        Static LastWidth As Integer = 0
        Static LastHeight As Integer = 0

        If Width <> LastWidth OrElse Height <> LastHeight Then
            If Image IsNot Nothing Then
                Image.Dispose()
                Image = Nothing
            End If
            LastWidth = Width
            LastHeight = Height
            If Width <= 0 OrElse Height <= 0 Then Return
            Image = New Bitmap(Width, Height)
            Picture.AttachToGrapher(Me, UseImageBuffer)
        End If
    End Sub

    Protected Fps As Double
    Protected Overridable Sub DoGraph()
        If NDWorld Is Nothing Then Return
        If Picture Is Nothing Then Return
        NDWorld.DebugInfo = "ImageDistance " & GetRoundString(Picture.ImageDistance, Vector.Decimaldigits) & Environment.NewLine
        NDWorld.DebugInfo &= "GraphicsInterface " & CType(Picture, Object).GetType.Name & Environment.NewLine
        NDWorld.DebugInfo &= "ControlMode " & ControlMode.ToString & Environment.NewLine
        NDWorld.DebugInfo &= "MoveSpeed " & GetRoundString(MoveSpeed, Vector.Decimaldigits) & Environment.NewLine
        NDWorld.DebugInfo &= "RotateSpeed " & GetRoundString(RotateSpeed, Vector.Decimaldigits) & "Rad" & Environment.NewLine
        If Fps <> 0 Then NDWorld.DebugInfo &= "FPS " & (Fps).ToString("#0.0") & "Hz"

        CheckSize()

        Picture.Graph()
    End Sub
    Protected GraphInvalidated As Boolean = False
    Protected NumFlame As Double
    Protected TickUsed As Integer
    Protected Overridable Sub GraphToScreen()
        Dim StartTick As Double = Environment.TickCount

        Dim g As Graphics = Me.CreateGraphics
        If UseImageBuffer Then
            If GraphInvalidated Then
                DoGraph()
            End If
            If Image Is Nothing Then
                g.Clear(BackColor)
                If BackgroundImage IsNot Nothing Then g.DrawImageUnscaled(BackgroundImage, 0, 0)
            Else
                g.DrawImageUnscaled(Image, 0, 0)
            End If
        Else
            DoGraph()
        End If
        g.Dispose()

        If TickUsed > 300 Then
            Fps = NumFlame / TickUsed * 1000
            TickUsed = 0
            NumFlame = 0
        End If

        TickUsed += Environment.TickCount - StartTick
        NumFlame += 1
    End Sub
    Public Overridable Sub Graph()
        GraphInvalidated = True
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(pe)
        If Picture Is Nothing Then
            pe.Graphics.Clear(BackColor)
        Else
            GraphToScreen()
        End If
    End Sub
End Class
#End Region
