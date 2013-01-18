'==========================================================================
'
'  File:        Picker.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: 拾取器
'  Version:     2013.01.18.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports Color = System.Drawing.Color
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports Firefly
Imports FileSystem
Imports GraphSystem

Public Class Picker
    Inherits Grapher

    Public SECFile As SEC_Simple.FileInfo
    Public SecState As SecState

    Public Changed As Boolean

    Public WithEvents PropertyGrid As PropertyGrid
    Public SelectionDisplayer As ToolStripStatusLabel
    Public ControlModeDisplayer As ToolStripStatusLabel

    Public Sub ResetView()
        With NDWorld
            Dim m As Decimal(,) = {{0, 1, 0}, {0, 0, -1}, {-1, 0, 0}}
            .HomotheticTransformation = New HomotheticTransformation(New Matrix(m), New Vector(0, 0, 3000))
            '.HomotheticTransformation.Rotate(1, 2, PI / 2)
            .HomotheticTransformation.Rotate(2, 0, CRad(20))
            .HomotheticTransformation.Rotate(1, 2, CRad(30))
        End With
    End Sub

    Public Shadows Sub Initialize(ByVal ProgramName As String, Optional ByVal PropertyGrid As PropertyGrid = Nothing, Optional ByVal SelectionDisplayer As ToolStripStatusLabel = Nothing, Optional ByVal ControlModeDisplayer As ToolStripStatusLabel = Nothing)
        NDWorld = New NDWorld(New World) '声明坐标系统
        With NDWorld
            .ProgramName = ProgramName
            .Clear()
            .ColorAxis = Drawing.Color.Black.ToArgb
            .ColorBack = Drawing.Color.LightGray.ToArgb

            ResetView()

            .ShowAxis = True
            .ShowText = False
            .FontName = "宋体"
            .FontSize = 9
            .Add(New Point(New Vector(500, 0, 0), Color.Red))
            .Add(New Point(New Vector(0, 500, 0), Color.Green))
            .Add(New Point(New Vector(0, 0, 500), Color.Blue))
        End With

        MyBase.Initialize(NDWorld, Nothing)

        MoveSpeed = 150
        ControlMode = Grapher.ControlModeEnum.FirstLand

        Me.PropertyGrid = PropertyGrid
        Me.SelectionDisplayer = SelectionDisplayer
        If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = "None"
        Me.ControlModeDisplayer = ControlModeDisplayer
        If ControlModeDisplayer IsNot Nothing Then ControlModeDisplayer.Text = "ControlMode " & Me.ControlMode.ToString
    End Sub

    Public Sub UseGDIP()
        'ImageDistance = Resolution * RealImageDistance
        'RealImageDistance = 0.5 m
        'Resolution = 337.9 mm / 1024 pixel ~= 0.33mm / pixel
        'ImageDistance ~= 1500 pixel
        Dim ImageDistance As Double = 1500
        If Picture IsNot Nothing Then
            ImageDistance = Picture.ImageDistance
            Picture.Dispose()
        End If
        Picture = New PicGDIPlusPer(ImageDistance)
        CType(Picture, PicGDIPlusPer).EnableSort = True
        MyBase.Initialize(NDWorld, Picture)
        ReGraph()
    End Sub
    Public Sub UseD3D()
        Dim ImageDistance As Double = 1500
        If Picture IsNot Nothing Then
            ImageDistance = Picture.ImageDistance
            Picture.Dispose()
        End If
        Picture = New PicD3D(ImageDistance)
        MyBase.Initialize(NDWorld, Picture)
        ReGraph()
    End Sub

    Public Sub ReGraph()
        If TypeOf Picture Is PicGDIPlusPer Then
            Graph()
        ElseIf TypeOf Picture Is PicD3D Then
            CheckSize()
            CType(Picture, PicD3D).NotifyModelChanged()
            Picture.Graph()
        End If
    End Sub

    Protected Selected As New List(Of NDObject)

    Public Sub PickPoint(ByVal Point As SecPoint, ByVal Add As Boolean)
        If SECFile Is Nothing Then Return

        For Each o In Selected
            Dim p = CType(o, IPickShowable)
            p.ShowAsUnpicked()
        Next
        Selected.Clear()
        If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = "None"
        If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing

        If Point Is Nothing Then Return

        Selected.Add(Point)
        Point.ShowAsPicked()
        If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = String.Format("P{0}({1})", Point.Index.ToInvariantString(), String.Join(" ", SecState.PointToDistrict(Point.Index).Select(Function(dIndex) "D" & dIndex.ToInvariantString()).ToArray()))
        If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = SECFile.Points(Point.Index)
    End Sub
    Public Sub PickDistrict(ByVal District As SecDistrict, ByVal Add As Boolean)
        If SECFile Is Nothing Then Return

        If Not Add Then
            For Each o In Selected
                Dim p = CType(o, IPickShowable)
                p.ShowAsUnpicked()
            Next
            Selected.Clear()
            If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = "None"
            If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing

            If District Is Nothing Then Return
        End If

        If Selected.Contains(District) Then
            Selected.Remove(District)
            District.ShowAsUnpicked()
            If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = "None"
            If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing
        Else
            Selected.Add(District)
            District.ShowAsPicked()
            If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = String.Format("D{0}({1})", District.Index.ToInvariantString(), String.Join(" ", SecState.DistrictToPoint(District.Index).Select(Function(pIndex) "P" & pIndex.ToInvariantString()).ToArray()))
            If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = SECFile.Districts(District.Index)
        End If
    End Sub

    Public Sub Pick(ByVal x As Integer, ByVal y As Integer, ByVal Add As Boolean)
        Dim Picked = NDWorld.GetBestFitPicking(x - Picture.Width \ 2, y - Picture.Height \ 2, Picture.ImageDistance)

        If TypeOf Picked Is SecPoint Then
            PickPoint(DirectCast(Picked, SecPoint), Add)
        ElseIf TypeOf Picked Is SecDistrict Then
            PickDistrict(DirectCast(Picked, SecDistrict), Add)
        End If
    End Sub

    Private Function GetPoints(ByVal o As Object) As Vector()
        If TypeOf o Is SecPoint Then
            Dim Line = DirectCast(o, SecPoint).GetLine()
            Return New Vector() {Line.PosB}
        ElseIf TypeOf o Is SecDistrict Then
            Dim PosArray = DirectCast(o, SecDistrict).GetTopRegion().PosArray
            Return PosArray
        Else
            Throw New InvalidOperationException
        End If
    End Function

    Public Sub FocusOn()
        If Selected.Count = 0 Then Return

        Dim Points = Selected.SelectMany(Function(o) GetPoints(o)).ToArray()

        Dim Center = Points.Aggregate(Function(a, b) a + b) * (1 / Points.Count)

        Dim Distance As Integer = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2)

        FocusPoint = Center

        If NDWorld.HomotheticTransformation Is Nothing Then NDWorld.HomotheticTransformation = New HomotheticTransformation
        NDWorld.HomotheticTransformation.RefPos = -(NDWorld.HomotheticTransformation * FocusPoint) + New Vector(0, 0, Distance)
    End Sub

    Public Sub FocusIn()
        If Selected.Count = 0 Then Return

        Dim Points = Selected.SelectMany(Function(o) GetPoints(o)).ToArray()
        If Points.Length <= 1 Then Return

        Dim Center = Points.Aggregate(Function(a, b) a + b) * (1 / Points.Count)

        Dim RadiusSqr As Double = 0
        For Each Pos In Points
            Dim Sqr As Double = (Pos - Center).Sqr
            If Sqr > RadiusSqr Then RadiusSqr = Sqr
        Next

        FocusPoint = Center

        If NDWorld.HomotheticTransformation Is Nothing Then NDWorld.HomotheticTransformation = New HomotheticTransformation
        NDWorld.HomotheticTransformation.RefPos = -(NDWorld.HomotheticTransformation * FocusPoint) + New Vector(0, 0, Sqrt(RadiusSqr) * 10)
    End Sub


    Public Sub AttachToSEC(ByVal SECFile As SEC_Simple.FileInfo)
        NDWorld.Clear()
        FocusPoint = Nothing
        If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing
        If SelectionDisplayer IsNot Nothing Then SelectionDisplayer.Text = "None"
        Me.SECFile = SECFile
        If SECFile Is Nothing Then
            ReGraph()
            Return
        End If

        SecState = New SecState With {.Sec = SECFile}

        Dim pIndex = 0
        For Each p In SECFile.Points
            Dim Point As New SecPoint(SecState, pIndex, False)
            SecState.SecPoints.Add(Point)
            NDWorld.Add(Point)
            pIndex += 1
        Next

        Dim Index = 0
        For Each d In SECFile.Districts
            If d.UnknownData.Length = 0 Then
                If SECFile.VersionSign = SEC.Version.Comm2 Then
                    d.UnknownData = New Byte(24 - 1) {}
                Else
                    d.UnknownData = New Byte(16 - 1) {}
                End If
            End If

            For Each b In d.Borders
                Dim spi = b.StartPointIndex
                Dim epi = b.EndPointIndex
                Dim h As HashSet(Of Integer)
                If Not SecState.DistrictToPoint.ContainsKey(Index) Then
                    h = New HashSet(Of Integer)
                    SecState.DistrictToPoint.Add(Index, h)
                Else
                    h = SecState.DistrictToPoint(Index)
                End If
                If Not h.Contains(spi) Then h.Add(spi)
                If Not h.Contains(epi) Then h.Add(epi)

                Dim sh As HashSet(Of Integer)
                If Not SecState.PointToDistrict.ContainsKey(spi) Then
                    sh = New HashSet(Of Integer)
                    SecState.PointToDistrict.Add(spi, sh)
                Else
                    sh = SecState.PointToDistrict(spi)
                End If
                If Not sh.Contains(Index) Then sh.Add(Index)

                Dim eh As HashSet(Of Integer)
                If Not SecState.PointToDistrict.ContainsKey(epi) Then
                    eh = New HashSet(Of Integer)
                    SecState.PointToDistrict.Add(epi, eh)
                Else
                    eh = SecState.PointToDistrict(epi)
                End If
                If Not eh.Contains(Index) Then eh.Add(Index)
            Next

            Dim District As New SecDistrict(SecState, Index, False)
            SecState.SecDistricts.Add(District)
            NDWorld.Add(District)
            Index += 1
        Next

        'Dim O As New Vector(SECFile.MinX, SECFile.MinY)
        'Dim PA As Vector = O + New Vector(64, 64)
        'Dim PB As Vector = O + New Vector(0, 64)
        'Dim PC As Vector = O + New Vector(0, 0)
        'Dim PD As Vector = O + New Vector(64, 0)
        'For y As Integer = 0 To SECFile.NumberMeshY - 1
        '    For x As Integer = 0 To SECFile.NumberMeshX - 1
        '        Dim P As New Vector(x * 64, y * 64)
        '        Dim R As SEC.Mesh = SECFile.MeshDS(x, y)
        '        If R.NumDistrict > 0 AndAlso Array.IndexOf(R.DistrictIndex, 109) >= 0 Then
        '            NDWorld.Add(New Polygon(New Vector() {P + PA, P + PB, P + PC, P + PD}, Color.Yellow, True, Color.Yellow))
        '        Else
        '            NDWorld.Add(New Polygon(New Vector() {P + PA, P + PB, P + PC, P + PD}, Color.Purple, False, Nothing))
        '        End If
        '    Next
        'Next

        ReGraph()
    End Sub

    Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
        If Picture IsNot Nothing Then ReGraph()
        MyBase.OnSizeChanged(e)
    End Sub

    Protected Overrides Sub DoKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        Static LastTime As Double '上次处理的时间 用来处理过量按键等问题
        If DateAndTime.Timer - LastTime < 0.4 Then Exit Sub
        LastTime = DateAndTime.Timer
        e.Handled = True
        Select Case e.KeyData
            Case Keys.Tab
                e.Handled = False
                MyBase.DoKeyUp(e)
                e.Handled = True
                If ControlModeDisplayer IsNot Nothing Then ControlModeDisplayer.Text = "ControlMode " & Me.ControlMode.ToString
            Case Keys.F
                Dim FocusPoint1 As Vector = FocusPoint
                FocusOn()
                Dim FocusPoint2 As Vector = FocusPoint
                If Equal(FocusPoint1, FocusPoint2) Then FocusIn()
                e.Handled = True
            Case Else
                e.Handled = False
        End Select
        If e.Handled Then ReGraph()

        MyBase.DoKeyUp(e)
    End Sub

    Protected Overrides Sub DoMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.DoMouseDown(e)
    End Sub
    Protected Overrides Sub DoMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        Select Case e.Clicks
            Case 1
                Select Case e.Button
                    Case Windows.Forms.MouseButtons.Left
                        [Select]()
                        Pick(e.X, e.Y, (Control.ModifierKeys And Keys.Control) <> 0)
                        ReGraph()
                        Return
                End Select
            Case 2
                Select Case e.Button
                    Case Windows.Forms.MouseButtons.Left
                        Pick(e.X, e.Y, (Control.ModifierKeys And Keys.Control) <> 0)
                        FocusIn()
                        ReGraph()
                        Return
                    Case Windows.Forms.MouseButtons.Right
                        ResetView()
                End Select
                Graph()
        End Select

        MyBase.DoMouseUp(e)
    End Sub

    Private Sub PropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid.PropertyValueChanged
        ReGraph()

        Changed = True
    End Sub
End Class
