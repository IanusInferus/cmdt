'==========================================================================
'
'  File:        PicD3D.vb
'  Location:    NDGrapher.GraphD3D <Visual Basic .Net>
'  Description: PicD3D
'  Created:     2007.08.30.22:01(GMT+08:00)
'  Version:     2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Imports SlimDX
Imports SlimDX.Direct3D9

Imports GraphSystem

Imports Color = System.Drawing.Color

''' <summary>图片对象Direct3D实现</summary>
''' <remarks>
''' Direct3D使用X轴向右，Y轴向上，Z轴垂直纸面向内的左手坐标系
''' 接口要求使用X轴向右，Y轴向下，Z轴垂直纸面向内的右手坐标系
''' </remarks>
Public NotInheritable Class PicD3D
    Implements IPic

    'Direct3D设备相关声明
    Private D3D As New Direct3D
    Private Device As Device
    Private DeviceLost As Boolean = True
    Private NumLine As Integer
    Private NumRegionVertex As Integer
    Private NumRegionTriangle As Integer
    Private LineVertexBuffer As VertexBuffer
    Private RegionVertexBuffer As VertexBuffer
    Private RegionIndexBuffer As IndexBuffer

    Public Sub InitializeGraphics()
        DisposeGraphics()

        Dim Mode As DisplayMode = D3D.GetAdapterDisplayMode(0)
        Dim Capabilities As Capabilities = D3D.GetDeviceCaps(0, DeviceType.Hardware)

        '初始化设备参数
        Dim PresentParams As PresentParameters
        PresentParams = New PresentParameters
        PresentParams.Windowed = True  '干脆不支持全屏
        PresentParams.SwapEffect = SwapEffect.Discard
        PresentParams.EnableAutoDepthStencil = True

        If D3D.CheckDepthStencilMatch(0, DeviceType.Hardware, Mode.Format, Format.A8R8G8B8, Format.D24X8) Then
            PresentParams.AutoDepthStencilFormat = Format.D24X8
        ElseIf D3D.CheckDepthStencilMatch(0, DeviceType.Hardware, Mode.Format, Format.A8R8G8B8, Format.D16) Then
            PresentParams.AutoDepthStencilFormat = Format.D16
        Else
            Throw New InvalidOperationException
        End If

        '初始化设备
        If (Capabilities.DeviceCaps And DeviceCaps.HWTransformAndLight) <> 0 Then
            Device = New Device(D3D, 0, DeviceType.Hardware, Grapher.Handle, CreateFlags.HardwareVertexProcessing, PresentParams)
        Else
            Device = New Device(D3D, 0, DeviceType.Hardware, Grapher.Handle, CreateFlags.SoftwareVertexProcessing, PresentParams)
        End If

        '设置透视变换
        SetupMatrices()

        Device.SetRenderState(RenderState.CullMode, Cull.Clockwise)
        Device.SetRenderState(RenderState.Lighting, False)
        Device.SetRenderState(RenderState.PointSpriteEnable, True)
        Device.SetRenderState(RenderState.ColorVertex, True)
        Device.SetRenderState(RenderState.PointSize, 2)

        If (Capabilities.LineCaps And LineCaps.Antialias) <> 0 Then
            Device.SetRenderState(RenderState.AntialiasedLineEnable, True)
        End If

        DeviceLost = False
        EnableGenerate = True
    End Sub
    Public Sub DisposeGraphics()
        If Device IsNot Nothing Then
            Device.Dispose()
            Device = Nothing
        End If

        '清空3D绘图缓存对象
        NumLine = 0
        If LineVertexBuffer IsNot Nothing Then LineVertexBuffer.Dispose()
        LineVertexBuffer = Nothing
        NumRegionVertex = 0
        If RegionVertexBuffer IsNot Nothing Then RegionVertexBuffer.Dispose()
        RegionVertexBuffer = Nothing
        NumRegionTriangle = 0
        If RegionIndexBuffer IsNot Nothing Then RegionIndexBuffer.Dispose()
        RegionIndexBuffer = Nothing

        '清空2D绘图缓存对象
        If Draw2DLine_Pen IsNot Nothing Then
            Draw2DLine_Pen.Dispose()
            Draw2DLine_Pen = Nothing
        End If
        If Draw2DString_Font IsNot Nothing Then
            Draw2DString_Font.Dispose()
            Draw2DString_Font = Nothing
        End If

        DeviceLost = True
    End Sub

    Public Sub Clear(ByVal Color As ColorInt32) Implements IPic.Clear
        RefreshDeviceLost()
        If DeviceLost Then InitializeGraphics()
        Device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, Color.ARGB, 1.0F, 0)
    End Sub

    Private Sub GenerateBuffer()
        Grapher.NDWorld.GraphObject(Me)

        Dim Lines As New List(Of Line)
        Dim Regions As New List(Of Region)
        Dim ImageQuadrangles As New List(Of ImageQuadrangle)
        For n As Integer = 0 To Final.Count - 1
            Dim o As IPicObj = Final(n)
            If TypeOf o Is Line Then
                Lines.Add(o)
            ElseIf TypeOf o Is Region Then
                Dim r As Region = o
                If r.PosArray Is Nothing OrElse r.PosArray.GetLength(0) < 3 Then Continue For
                Regions.Add(r)
            ElseIf TypeOf o Is ImageQuadrangle Then
                ImageQuadrangles.Add(o)
            Else
                Stop
            End If
        Next

        If Lines.Count <> 0 Then
            NumLine = Lines.Count
            LineVertexBuffer = New VertexBuffer(Device, 16 * Lines.Count * 2, Usage.WriteOnly, VertexFormat.Position Or VertexFormat.Diffuse, Pool.Default)
            Dim LineVertexStream = LineVertexBuffer.Lock(0, 16 * Lines.Count * 2, LockFlags.None)
            Dim bw = New System.IO.BinaryWriter(LineVertexStream)
            Try
                For i As Integer = 0 To Lines.Count - 1
                    Dim Line As Line = Lines(i)
                    If Double.IsNegativeInfinity(Line.LowerBound) OrElse Double.IsPositiveInfinity(Line.UpperBound) Then
                        Dim PosA = Line.Pos + Max(-10000, Line.LowerBound) * Line.Dir
                        Dim PosB = Line.Pos + Min(10000, Line.UpperBound) * Line.Dir
                        bw.Write(CSng(PosA(0)))
                        bw.Write(CSng(PosA(1)))
                        bw.Write(CSng(PosA(2)))
                        bw.Write(Line.ColorInt.ARGB)
                        bw.Write(CSng(PosB(0)))
                        bw.Write(CSng(PosB(1)))
                        bw.Write(CSng(PosB(2)))
                        bw.Write(Line.ColorInt.ARGB)
                    Else
                        bw.Write(CSng(Line.PosA(0)))
                        bw.Write(CSng(Line.PosA(1)))
                        bw.Write(CSng(Line.PosA(2)))
                        bw.Write(Line.ColorInt.ARGB)
                        bw.Write(CSng(Line.PosB(0)))
                        bw.Write(CSng(Line.PosB(1)))
                        bw.Write(CSng(Line.PosB(2)))
                        bw.Write(Line.ColorInt.ARGB)
                    End If
                Next
            Finally
                bw.Dispose()
                LineVertexStream.Dispose()
                LineVertexBuffer.Unlock()
            End Try
        Else
            NumLine = 0
            If LineVertexBuffer IsNot Nothing Then LineVertexBuffer.Dispose()
            LineVertexBuffer = Nothing
        End If

        '对Region的每一个进行三角剖分
        NumRegionVertex = 0
        NumRegionTriangle = 0
        If Regions.Count <> 0 Then
            For Each r As Region In Regions
                Dim n As Integer = r.PosArray.GetLength(0)
                NumRegionVertex += n
                NumRegionTriangle += n - 2
            Next
            RegionVertexBuffer = New VertexBuffer(Device, 16 * NumRegionVertex, Usage.WriteOnly, VertexFormat.Position Or VertexFormat.Diffuse, Pool.Default)
            RegionIndexBuffer = New IndexBuffer(Device, 2 * NumRegionTriangle * 3, Usage.WriteOnly, Pool.Default, True)
            Dim RegionVertexStream = RegionVertexBuffer.Lock(0, 16 * NumRegionVertex, LockFlags.None)
            Dim bwVertex = New System.IO.BinaryWriter(RegionVertexStream)
            Dim RegionIndexStream = RegionIndexBuffer.Lock(0, 2 * NumRegionTriangle * 3, LockFlags.None)
            Dim bwIndex = New System.IO.BinaryWriter(RegionIndexStream)
            Try
                Dim RegionVertexArrayIndex As Integer = 0
                For Each r As Region In Regions
                    For Each p As Vector In r.PosArray
                        bwVertex.Write(CSng(p(0)))
                        bwVertex.Write(CSng(p(1)))
                        bwVertex.Write(CSng(p(2)))
                        bwVertex.Write(r.ColorInt.ARGB)
                    Next
                    For n As Integer = 0 To r.PosArray.GetLength(0) - 3
                        bwIndex.Write(CShort(RegionVertexArrayIndex))
                        bwIndex.Write(CShort(RegionVertexArrayIndex + n + 1))
                        bwIndex.Write(CShort(RegionVertexArrayIndex + n + 2))
                    Next
                    RegionVertexArrayIndex += r.PosArray.Length
                Next
            Finally
                bwVertex.Dispose()
                bwIndex.Dispose()
                RegionVertexStream.Dispose()
                RegionIndexStream.Dispose()
                RegionVertexBuffer.Unlock()
                RegionIndexBuffer.Unlock()
            End Try
        Else
            If RegionVertexBuffer IsNot Nothing Then RegionVertexBuffer.Dispose()
            RegionVertexBuffer = Nothing
            If RegionIndexBuffer IsNot Nothing Then RegionIndexBuffer.Dispose()
            RegionIndexBuffer = Nothing
        End If

        EnableGenerate = False
    End Sub

    Private EnableGenerate As Boolean = False
    Public Sub NotifyModelChanged()
        EnableGenerate = True
    End Sub

    Private Sub RefreshDeviceLost()
        If Device Is Nothing Then
            DeviceLost = True
            Return
        End If
        DeviceLost = (Device.TestCooperativeLevel() = ResultCode.DeviceLost)
    End Sub

    Public Sub Graph() Implements IPic.Graph
        Dim NDWorld As NDWorld = Grapher.NDWorld
        NDWorld.GraphClear(Me) '清屏

        '如果自身支持世界变换，则不使用NDWorld的世界变换
        Dim BackupHomotheticTransformation As HomotheticTransformation = Nothing
        If TrySetWorldTransformation(NDWorld.HomotheticTransformation) Then
            BackupHomotheticTransformation = NDWorld.HomotheticTransformation
            NDWorld.HomotheticTransformation = Nothing
        End If

        RefreshDeviceLost()
        If DeviceLost Then InitializeGraphics()
        If EnableGenerate Then GenerateBuffer()

        Device.BeginScene()

        If NumRegionVertex <> 0 Then
            'Z-Fight  Hidden Lines Removal
            'The bias is not applied to any line and point primitive. However, this bias needs to be applied to triangles drawn in wireframe mode.
            Device.SetRenderState(RenderState.SlopeScaleDepthBias, 1.0F)

            Device.SetStreamSource(0, RegionVertexBuffer, 0, 16)
            Device.Indices = RegionIndexBuffer
            Device.VertexFormat = VertexFormat.Position Or VertexFormat.Diffuse
            Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, NumRegionVertex, 0, NumRegionTriangle)
            Device.SetRenderState(RenderState.SlopeScaleDepthBias, 0)
        End If

        If NumLine <> 0 Then
            Device.SetStreamSource(0, LineVertexBuffer, 0, 16)
            Device.VertexFormat = VertexFormat.Position Or VertexFormat.Diffuse
            Device.DrawPrimitives(PrimitiveType.LineList, 0, NumLine)
        End If

        Final.Clear()


        '还原NDWorld的世界变换
        If BackupHomotheticTransformation IsNot Nothing Then NDWorld.HomotheticTransformation = BackupHomotheticTransformation

        NDWorld.GraphText(Me)

        Device.EndScene()

        Try
            Device.Present()
        Catch ex As Direct3D9Exception When ex.ResultCode = ResultCode.DeviceLost
        End Try

        '清空2D绘图缓存对象，不跨帧缓存
        If Draw2DLine_Pen IsNot Nothing Then
            Draw2DLine_Pen.Dispose()
            Draw2DLine_Pen = Nothing
        End If
        If Draw2DString_Font IsNot Nothing Then
            Draw2DString_Font.Dispose()
            Draw2DString_Font = Nothing
        End If
    End Sub


    '常规声明
    Public Grapher As Grapher
    Private ImageDistanceValue As Double = 2 '相距 眼睛到焦点(原点)的距离
    Private FociValue As Double = 0.3 '焦距
    Private Final As New List(Of IPicObj)
    Private SemiWidth As Integer
    Private SemiHeight As Integer

#Region " 暴露的一般性接口实现 "
    Public Sub New(ByVal ImageDistance As Double)
        ImageDistanceValue = ImageDistance
    End Sub
    Public ReadOnly Property Width() As Integer Implements IPic.Width
        Get
            Return SemiWidth * 2
        End Get
    End Property
    Public ReadOnly Property Height() As Integer Implements IPic.Height
        Get
            Return SemiHeight * 2
        End Get
    End Property
    Public Sub AttachToGrapher(ByVal Grapher As GraphSystem.Grapher, ByRef UseImageBuffer As Boolean) Implements IPic.AttachToGrapher
        If Grapher Is Nothing Then Throw New ArgumentNullException
        Me.Grapher = Grapher
        SemiWidth = Grapher.Width \ 2
        SemiHeight = Grapher.Height \ 2
        UseImageBuffer = False
        DisposeGraphics()
    End Sub
    Public Function TrySetWorldTransformation(ByVal WorldTransformation As HomotheticTransformation) As Boolean
        If WorldTransformation.Dimension > 3 Then Return False '如果是超过3维的世界坐标变换，D3D无法实现

        '注意: Direct3D中的变换矩阵是数学书上的齐次坐标矩阵的转置
        Dim m As New SlimDX.Matrix()
        Dim s As Single = 2 ^ WorldTransformation.Scaler
        With WorldTransformation
            m.M11 = .Matrix(0, 0) * s
            m.M21 = .Matrix(0, 1) * s
            m.M31 = .Matrix(0, 2) * s
            m.M12 = .Matrix(1, 0) * s
            m.M22 = .Matrix(1, 1) * s
            m.M32 = .Matrix(1, 2) * s
            m.M13 = .Matrix(2, 0) * s
            m.M23 = .Matrix(2, 1) * s
            m.M33 = .Matrix(2, 2) * s
            m.M41 = .RefPos(0)
            m.M42 = .RefPos(1)
            m.M43 = .RefPos(2)
            m.M14 = 0
            m.M24 = 0
            m.M34 = 0
            m.M44 = 1
        End With
        Dim World As SlimDX.Matrix = SlimDX.Matrix.Identity
        World.M11 = -1

        '注意: 因为上面的原因，结合也需要反方向相乘
        Device.SetTransform(TransformState.World, m * World)
        Return True
    End Function

    Public Sub Draw(ByVal Obj As IPicObj) Implements IPic.Draw
        Final.Add(Obj)
    End Sub
    Public Property ImageDistance() As Double Implements IPic.ImageDistance
        Get
            Return ImageDistanceValue
        End Get
        Set(ByVal Value As Double)
            If Value <= 0 Then Return
            ImageDistanceValue = Value
            SetupMatrices()
        End Set
    End Property
    Private Sub SetupMatrices()
        If Device Is Nothing Then Return
        Dim World As SlimDX.Matrix = SlimDX.Matrix.Identity
        World.M11 = -1
        Device.SetTransform(TransformState.World, World)
        Device.SetTransform(TransformState.View, SlimDX.Matrix.LookAtLH(New Vector3(0, 0, 0), New Vector3(0, 0, ImageDistance), New Vector3(0, -1, 0)))
        Dim FOV As Double = Atan(SemiHeight / ImageDistanceValue) * 2
        Device.SetTransform(TransformState.Projection, SlimDX.Matrix.PerspectiveFovLH(FOV, SemiWidth / SemiHeight, 1, 100000))
    End Sub

    Private Draw2DLine_Pen As Direct3D9.Line
    Public Sub Draw2DLine(ByVal Color As ColorInt32, ByVal PenWidth As Single, ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single) Implements IPic.Draw2DLine
        Static LastPenWidth As Single
        If Draw2DLine_Pen Is Nothing Then
            LastPenWidth = PenWidth
            Draw2DLine_Pen = New Direct3D9.Line(Device)
            Draw2DLine_Pen.Width = PenWidth
        ElseIf PenWidth <> LastPenWidth Then
            LastPenWidth = PenWidth
            Draw2DLine_Pen.Dispose()
            Draw2DLine_Pen = New Direct3D9.Line(Device)
            Draw2DLine_Pen.Width = PenWidth
        End If
        Draw2DLine_Pen.Draw(New Vector2() {New Vector2(x1 + SemiWidth, y1 + SemiHeight), New Vector2(x2 + SemiWidth, y2 + SemiHeight)}, Color.ARGB)
    End Sub

    Public Sub Draw2DRegion(ByVal Color As ColorInt32, ByVal Points As Vector()) Implements IPic.Draw2DRegion
        Throw New Exception("此函数Draw2DRegion尚未实现")
    End Sub

    Private Draw2DString_Font As Direct3D9.Font
    Public Sub Draw2DString(ByVal s As String, ByVal Font As System.Drawing.Font, ByVal Color As ColorInt32, ByVal x As Single, ByVal y As Single) Implements IPic.Draw2DString
        If s = "" Then Return

        Static LastFont As Drawing.Font
        If Draw2DString_Font Is Nothing Then
            LastFont = Font
            Draw2DString_Font = New Direct3D9.Font(Device, Font)
        ElseIf Font IsNot LastFont Then
            LastFont = Font
            Draw2DString_Font.Dispose()
            Draw2DString_Font = New Direct3D9.Font(Device, Font)
        End If
        Dim yl As Single = y + 0.3 '+0.3是对比GDI+得到的偏移量
        Dim h As Single = Font.GetHeight()
        For Each l As String In s.Replace(Environment.NewLine, ChrW(10)).Split(ChrW(10))
            Draw2DString_Font.DrawString(Nothing, l, CInt(x + 2), CInt(yl), Color.ARGB) '+2是对比GDI+得到的偏移量
            yl += h
        Next
    End Sub

    ' IDisposable
    Private disposedValue As Boolean = False ' 检测冗余的调用
    Private Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '显式调用时释放托管资源
            End If

            '释放共享的非托管资源
            DisposeGraphics()
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
