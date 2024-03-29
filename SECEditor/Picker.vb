'==========================================================================
'
'  File:        Picker.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: 拾取器
'  Version:     2023.01.30.
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

    Public SecState As SecState

    Public Event PointSelected(ByVal Point As SecPoint)
    Public Event DistrictSelected(ByVal District As SecDistrict)
    Public Event PointUnselected(ByVal Point As SecPoint)
    Public Event DistrictUnselected(ByVal District As SecDistrict)
    Public Event ControlModeChanged(ByVal ControlMode As ControlModeEnum)

    Public Sub ResetView()
        With NDWorld
            Dim m As Double(,) = {{0, 1, 0}, {0, 0, -1}, {-1, 0, 0}}
            .HomotheticTransformation = New HomotheticTransformation(New Matrix(m), New Vector(0, 0, 3000))
            '.HomotheticTransformation.Rotate(1, 2, PI / 2)
            .HomotheticTransformation.Rotate(2, 0, CRad(20))
            .HomotheticTransformation.Rotate(1, 2, CRad(30))
        End With
    End Sub

    Public Shadows Sub Initialize(ByVal ProgramName As String)
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

        RaiseEvent ControlModeChanged(Me.ControlMode)
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
        Graph()
    End Sub
    Public Sub UseD3D()
        Dim ImageDistance As Double = 1500
        If Picture IsNot Nothing Then
            ImageDistance = Picture.ImageDistance
            Picture.Dispose()
        End If
        Picture = New PicD3D(ImageDistance)
        MyBase.Initialize(NDWorld, Picture)
        Graph()
    End Sub

    Public Overrides Sub Graph()
        If TypeOf Picture Is PicGDIPlusPer Then
            MyBase.Graph()
        ElseIf TypeOf Picture Is PicD3D Then
            CheckSize()
            CType(Picture, PicD3D).NotifyModelChanged()
            Picture.Graph()
        End If
    End Sub

    Protected Selected As New List(Of NDObject)

    Public Sub SelectAllDistricts()
        UnselectAll()

        Selected.Clear()
        For Each o In NDWorld.Obj()
            If TypeOf o IsNot SecDistrict Then Continue For
            Dim p = CType(o, SecDistrict)
            Selected.Add(p)
            p.ShowAsPicked()
            RaiseEvent DistrictSelected(DirectCast(p, SecDistrict))
        Next
    End Sub
    Public Sub UnselectAll()
        For Each o In Selected
            Dim p = CType(o, IPickShowable)
            p.ShowAsUnpicked()
            If TypeOf p Is SecPoint Then
                RaiseEvent PointUnselected(DirectCast(p, SecPoint))
            ElseIf TypeOf p Is SecDistrict Then
                RaiseEvent DistrictUnselected(DirectCast(p, SecDistrict))
            End If
        Next
        Selected.Clear()
    End Sub

    Public Sub SelectPoint(ByVal Point As SecPoint, ByVal Add As Boolean)
        If SecState Is Nothing Then Return

        If Not Add Then
            UnselectAll()
        End If

        If Point Is Nothing Then Return

        If Selected.Contains(Point) Then
            Selected.Remove(Point)
            Point.ShowAsUnpicked()
            RaiseEvent PointUnselected(Point)
        Else
            Selected.Add(Point)
            Point.ShowAsPicked()
            RaiseEvent PointSelected(Point)
        End If
    End Sub
    Public Sub SelectDistrict(ByVal District As SecDistrict, ByVal Add As Boolean)
        If SecState Is Nothing Then Return

        If Not Add Then
            UnselectAll()
        End If

        If District Is Nothing Then Return

        If Selected.Contains(District) Then
            Selected.Remove(District)
            District.ShowAsUnpicked()
            RaiseEvent DistrictUnselected(District)
        Else
            Selected.Add(District)
            District.ShowAsPicked()
            RaiseEvent DistrictSelected(District)
        End If
    End Sub

    Public Sub SelectByPick(ByVal x As Integer, ByVal y As Integer, ByVal Add As Boolean)
        If Picture Is Nothing Then Return

        Dim Picked = NDWorld.GetBestFitPicking(x - Picture.Width \ 2, y - Picture.Height \ 2, Picture.ImageDistance)

        If TypeOf Picked Is SecPoint Then
            SelectPoint(DirectCast(Picked, SecPoint), Add)
        ElseIf TypeOf Picked Is SecDistrict Then
            SelectDistrict(DirectCast(Picked, SecDistrict), Add)
        ElseIf Not Add Then
            UnselectAll()
        End If
    End Sub

    Public Function CreatePoint(ByVal PosArray As Single()) As Integer
        If PosArray.Length <> 2 Then Throw New InvalidOperationException
        Dim pIndex = SecState.Sec.Points.Count
        Dim p As New SEC_Simple.PointInfo With {.x = PosArray(0), .y = PosArray(1)}
        SecState.Sec.Points.Add(p)
        NotifyPointAdded(pIndex)
        Return pIndex
    End Function

    Public Function CreateDistrict(ByVal PointIndices As Integer()) As Integer
        Return CreateDistrict(PointIndices, SEC.TerrainInfo.FromString("0000000005000000"), 0, 0, 0)
    End Function
    Public Function CreateDistrict(ByVal PointIndices As Integer(), ByVal Terrain As SEC.TerrainInfo, ByVal kx As Single, ByVal ky As Single, ByVal bz As Single) As Integer
        If PointIndices.Length < 3 Then Throw New InvalidOperationException
        If PointIndices.Any(Function(i) i < 0 OrElse i >= SecState.Sec.Points.Count) Then Throw New InvalidOperationException
        Dim dIndex = SecState.Sec.Districts.Count
        Dim d As New SEC_Simple.DistrictInfo With
        {
            .Terrain = Terrain,
            .kx = kx,
            .ky = ky,
            .bz = bz,
            .Unknown1 = 0,
            .ZoneFlags = 0,
            .Unknown2 = 0,
            .Borders = New List(Of SEC_Simple.BorderInfo)()
        }
        SecState.Sec.Districts.Add(d)
        For k = 0 To PointIndices.Length - 1
            Dim StartIndex = PointIndices(k)
            Dim EndIndex = PointIndices((k + 1) Mod PointIndices.Length)
            Dim sh = SecState.PointToDistrict(StartIndex)
            Dim eh = SecState.PointToDistrict(EndIndex)
            Dim IntersectedDistrictIndices = sh.Intersect(eh)
            Dim NeighborDistrictIndex = -1
            For Each idi In IntersectedDistrictIndices
                Dim id = SecState.Sec.Districts(idi)
                For Each b In id.Borders
                    If (b.StartPointIndex = StartIndex AndAlso b.EndPointIndex = EndIndex) OrElse (b.StartPointIndex = EndIndex AndAlso b.EndPointIndex = StartIndex) Then
                        If b.NeighborDistrictIndex = -1 Then
                            b.NeighborDistrictIndex = dIndex
                            NeighborDistrictIndex = idi
                        End If
                    End If
                Next
            Next
            d.Borders.Add(New SEC_Simple.BorderInfo With {.StartPointIndex = StartIndex, .EndPointIndex = EndIndex, .NeighborDistrictIndex = NeighborDistrictIndex, .Adjacency = 0})
        Next
        NotifyDistrictAdded(dIndex)
        Return dIndex
    End Function

    Public Sub MergeSelectedDistricts()
        Dim DistrictsToBeMerged As New HashSet(Of Integer)
        For Each s In Selected
            If TypeOf s Is SecDistrict Then
                Dim dIndex = DirectCast(s, SecDistrict).Index
                DistrictsToBeMerged.Add(dIndex)
            End If
        Next

        Dim DeletedDistrictIndices As New List(Of Integer)
        While DistrictsToBeMerged.Count > 1
            Dim dIndexFirst = DistrictsToBeMerged.First()
            Dim dFirst = SecState.Sec.Districts(dIndexFirst)
            Dim dIndexSecond = -1
            Dim dBorderIndexFirst = -1
            Dim dBorderIndexSecond = -1
            For k = 0 To dFirst.Borders.Count - 1
                Dim b = dFirst.Borders(k)
                If b.NeighborDistrictIndex = -1 Then Continue For
                If DistrictsToBeMerged.Contains(b.NeighborDistrictIndex) Then
                    If b.NeighborDistrictIndex = dIndexFirst Then Continue For
                    Dim d = SecState.Sec.Districts(b.NeighborDistrictIndex)
                    If d.Terrain.ValueInt <> dFirst.Terrain.ValueInt Then Continue For
                    For j = 0 To d.Borders.Count - 1
                        Dim b2 = d.Borders(j)
                        If b2.NeighborDistrictIndex = -1 Then Continue For
                        If b2.NeighborDistrictIndex = dIndexFirst Then
                            If b.StartPointIndex = b2.EndPointIndex AndAlso b.EndPointIndex = b2.StartPointIndex Then
                                dIndexSecond = b.NeighborDistrictIndex
                                dBorderIndexFirst = k
                                dBorderIndexSecond = j
                                Exit For
                            End If
                        End If
                    Next
                    If dIndexSecond <> -1 Then Exit For
                End If
            Next
            If dIndexSecond = -1 Then
                DistrictsToBeMerged.Remove(dIndexFirst)
                Continue While
            End If

            Dim dSecond = SecState.Sec.Districts(dIndexSecond)
            dFirst.Borders(dBorderIndexFirst).NeighborDistrictIndex = -1
            dSecond.Borders(dBorderIndexSecond).NeighborDistrictIndex = -1
            dFirst.Borders = dFirst.Borders.Take(dBorderIndexFirst).Concat(dSecond.Borders.Skip(dBorderIndexSecond + 1)).Concat(dSecond.Borders.Take(dBorderIndexSecond)).Concat(dFirst.Borders.Skip(dBorderIndexFirst + 1)).ToList()
            SecState.DistrictToPoint(dIndexFirst).Clear()
            NotifyDistrictUpdated(dIndexFirst)

            dSecond.Borders.Clear()
            Dim h = SecState.DistrictToPoint(dIndexSecond)
            For Each pIndex In h
                Dim ph = SecState.PointToDistrict(pIndex)
                If ph.Contains(dIndexSecond) Then ph.Remove(dIndexSecond)
            Next
            h.Clear()
            NotifyDistrictUpdated(dIndexSecond)

            DistrictsToBeMerged.Remove(dIndexSecond)
        End While
        UnselectAll()
    End Sub

    Public Sub SplitDistrictByPoints()
        Dim PointsToSplit As New HashSet(Of Integer)
        For Each s In Selected
            If TypeOf s Is SecPoint Then
                Dim pIndex = DirectCast(s, SecPoint).Index
                PointsToSplit.Add(pIndex)
            End If
        Next

        If PointsToSplit.Count <> 2 Then Return

        Dim pIndexFirst = PointsToSplit(0)
        Dim pIndexSecond = PointsToSplit(1)

        Dim dIndices = SecState.PointToDistrict(pIndexFirst).Intersect(SecState.PointToDistrict(pIndexSecond)).ToList()
        If dIndices.Count <> 1 Then Return
        Dim dIndex = dIndices.Single()
        Dim d = SecState.Sec.Districts(dIndex)
        Dim StartBorderIndex = d.Borders.FindIndex(Function(b) b.StartPointIndex = pIndexFirst)
        Dim EndBorderIndex = d.Borders.FindIndex(Function(b) b.EndPointIndex = pIndexSecond)
        Dim BordersFirst As List(Of SEC_Simple.BorderInfo)
        Dim BordersSecond As List(Of SEC_Simple.BorderInfo)
        If StartBorderIndex <= EndBorderIndex Then
            BordersFirst = d.Borders.Skip(StartBorderIndex).Take(EndBorderIndex + 1 - StartBorderIndex).ToList()
            BordersSecond = d.Borders.Skip(EndBorderIndex + 1).Concat(d.Borders.Take(StartBorderIndex)).ToList()
        Else
            BordersFirst = d.Borders.Skip(StartBorderIndex).Concat(d.Borders.Take(EndBorderIndex + 1)).ToList()
            BordersSecond = d.Borders.Skip(EndBorderIndex + 1).Take(StartBorderIndex - EndBorderIndex - 1).ToList()
        End If
        If BordersFirst.Count < 2 OrElse BordersSecond.Count < 2 Then Return
        Dim NewBorderFirst As New SEC_Simple.BorderInfo With {.StartPointIndex = pIndexSecond, .EndPointIndex = pIndexFirst, .NeighborDistrictIndex = -1, .Adjacency = 0}
        BordersFirst.Add(NewBorderFirst)
        BordersSecond.Add(New SEC_Simple.BorderInfo With {.StartPointIndex = pIndexFirst, .EndPointIndex = pIndexSecond, .NeighborDistrictIndex = dIndex, .Adjacency = 0})

        d.Borders = BordersFirst
        SecState.DistrictToPoint(dIndex) = New HashSet(Of Integer)(BordersFirst.Select(Function(b) b.StartPointIndex))

        Dim dIndexNew = CreateDistrict(BordersSecond.Select(Function(b) b.StartPointIndex).ToArray(), d.Terrain, d.kx, d.ky, d.bz)
        NewBorderFirst.NeighborDistrictIndex = dIndexNew

        SecState.Sec.Districts(dIndexNew).Borders = BordersSecond
        NotifyDistrictUpdated(dIndex)

        For Each b In BordersSecond
            If b.NeighborDistrictIndex <> -1 Then
                Dim d2 = SecState.Sec.Districts(b.NeighborDistrictIndex)
                For Each b2 In d2.Borders
                    If b2.NeighborDistrictIndex = dIndex AndAlso b2.StartPointIndex = b.EndPointIndex AndAlso b2.EndPointIndex = b.StartPointIndex Then
                        b2.NeighborDistrictIndex = dIndexNew
                    End If
                Next
            End If
        Next

        UnselectAll()
    End Sub

    Public Sub DeleteSelected()
        Dim DeletedDistrictIndices As New List(Of Integer)
        For Each s In Selected
            If TypeOf s Is SecPoint Then
                Dim pIndex = DirectCast(s, SecPoint).Index
                Dim h = SecState.PointToDistrict(pIndex)
                For Each dIndex In h
                    Dim d = SecState.Sec.Districts(dIndex)
                    Dim dh = SecState.DistrictToPoint(dIndex)
                    If dh.Contains(pIndex) Then dh.Remove(pIndex)
                    Dim k = 0
                    While k < d.n
                        Dim NextBorderIndex = (k + 1) Mod d.n
                        If d.Borders(k).EndPointIndex = pIndex AndAlso d.Borders(NextBorderIndex).StartPointIndex = pIndex Then
                            d.Borders(k).EndPointIndex = d.Borders(NextBorderIndex).StartPointIndex
                            d.Borders.RemoveAt(NextBorderIndex)
                        ElseIf d.Borders(k).EndPointIndex = pIndex Then
                            d.Borders(k).EndPointIndex = 0
                        ElseIf d.Borders(NextBorderIndex).StartPointIndex = pIndex Then
                            d.Borders(NextBorderIndex).StartPointIndex = 0
                        End If
                        k += 1
                    End While
                    If d.n <= 2 Then d.Borders.Clear()
                    NotifyDistrictUpdated(dIndex)
                Next
                h.Clear()
            ElseIf TypeOf s Is SecDistrict Then
                Dim dIndex = DirectCast(s, SecDistrict).Index
                Dim h = SecState.DistrictToPoint(dIndex)
                For Each pIndex In h
                    Dim ph = SecState.PointToDistrict(pIndex)
                    If ph.Contains(dIndex) Then ph.Remove(dIndex)
                Next
                Dim d = SecState.Sec.Districts(dIndex)
                d.Borders.Clear()
                NotifyDistrictUpdated(dIndex)
                DeletedDistrictIndices.Add(dIndex)
            End If
        Next
        If DeletedDistrictIndices.Count > 0 Then
            For Each d In SecState.Sec.Districts
                For Each b In d.Borders
                    If DeletedDistrictIndices.Contains(b.NeighborDistrictIndex) Then
                        b.NeighborDistrictIndex = -1
                    End If
                Next
            Next
        End If
        UnselectAll()
    End Sub

    Public Sub Compact()
        Dim SecFile = SecState.Sec
        AttachToSEC(Nothing)

        Dim UsedPointIndices As New HashSet(Of Integer)
        Dim UsedDistrictIndices As New HashSet(Of Integer)
        For dIndex = 0 To SecFile.Districts.Count - 1
            Dim d = SecFile.Districts(dIndex)
            If d.n > 0 Then UsedDistrictIndices.Add(dIndex)
            For Each b In d.Borders
                If Not UsedPointIndices.Contains(b.StartPointIndex) Then UsedPointIndices.Add(b.StartPointIndex)
                If Not UsedPointIndices.Contains(b.EndPointIndex) Then UsedPointIndices.Add(b.EndPointIndex)
            Next
        Next
        Dim PointIndexToNewPointIndex As New Dictionary(Of Integer, Integer)
        Dim DistrictIndexToNewDistrictIndex As New Dictionary(Of Integer, Integer)
        Dim NewPoints As New List(Of SEC_Simple.PointInfo)
        Dim NewDistricts As New List(Of SEC_Simple.DistrictInfo)
        Dim pNewIndex = 0
        For pIndex = 0 To SecFile.Points.Count - 1
            If UsedPointIndices.Contains(pIndex) Then
                NewPoints.Add(SecFile.Points(pIndex))
                PointIndexToNewPointIndex.Add(pIndex, pNewIndex)
                pNewIndex += 1
            End If
        Next
        Dim dNewIndex = 0
        For dIndex = 0 To SecFile.Districts.Count - 1
            If UsedDistrictIndices.Contains(dIndex) Then
                NewDistricts.Add(SecFile.Districts(dIndex))
                DistrictIndexToNewDistrictIndex.Add(dIndex, dNewIndex)
                dNewIndex += 1
            End If
        Next
        For dIndex = 0 To SecFile.Districts.Count - 1
            If UsedDistrictIndices.Contains(dIndex) Then
                Dim d = SecFile.Districts(dIndex)
                For Each b In d.Borders
                    If PointIndexToNewPointIndex.ContainsKey(b.StartPointIndex) Then
                        b.StartPointIndex = PointIndexToNewPointIndex(b.StartPointIndex)
                    End If
                    If PointIndexToNewPointIndex.ContainsKey(b.EndPointIndex) Then
                        b.EndPointIndex = PointIndexToNewPointIndex(b.EndPointIndex)
                    End If
                    If DistrictIndexToNewDistrictIndex.ContainsKey(b.NeighborDistrictIndex) Then
                        b.NeighborDistrictIndex = DistrictIndexToNewDistrictIndex(b.NeighborDistrictIndex)
                    End If
                Next
            End If
        Next
        SecFile.Points = NewPoints
        SecFile.Districts = NewDistricts

        AttachToSEC(SecFile)
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
        If Points.Count = 0 Then Return

        Dim Center = Points.Aggregate(Function(a, b) a + b) * (1 / Points.Count)

        Dim Distance As Integer = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2)

        FocusPoint = Center

        If NDWorld.HomotheticTransformation Is Nothing Then NDWorld.HomotheticTransformation = New HomotheticTransformation
        NDWorld.HomotheticTransformation.RefPos = -(NDWorld.HomotheticTransformation * FocusPoint) + New Vector(0, 0, Distance)
    End Sub

    Public Sub FocusIn()
        If Selected.Count = 0 Then Return

        Dim Points = Selected.SelectMany(Function(o) GetPoints(o)).ToArray()
        If Points.Length <= 1 Then
            FocusOn()
            Return
        End If

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
        UnselectAll()
        If SECFile Is Nothing Then
            SecState = Nothing
            Graph()
            Return
        End If

        SecState = New SecState With {.Sec = SECFile}

        For pIndex = 0 To SECFile.Points.Count - 1
            NotifyPointAdded(pIndex)
        Next

        For dIndex = 0 To SECFile.Districts.Count - 1
            NotifyDistrictAdded(dIndex)
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

        Graph()
    End Sub

    Public Sub NotifyPointAdded(ByVal pIndex As Integer)
        Dim Point As New SecPoint(SecState, pIndex, False)
        SecState.SecPoints.Add(Point)
        SecState.PointToDistrict.Add(pIndex, New HashSet(Of Integer))
        NDWorld.Add(Point)
    End Sub

    Public Sub NotifyDistrictAdded(ByVal dIndex As Integer)
        Dim d = SecState.Sec.Districts(dIndex)
        SecState.DistrictToPoint.Add(dIndex, New HashSet(Of Integer))

        NotifyDistrictUpdated(dIndex)

        Dim District As New SecDistrict(SecState, dIndex, False)
        SecState.SecDistricts.Add(District)
        NDWorld.Add(District)
    End Sub

    Public Sub NotifyDistrictUpdated(ByVal dIndex As Integer)
        Dim d = SecState.Sec.Districts(dIndex)
        Dim h = SecState.DistrictToPoint(dIndex)
        For Each pIndex In h
            If Not SecState.PointToDistrict.ContainsKey(pIndex) Then Continue For
            Dim ph = SecState.PointToDistrict(pIndex)
            If ph.Contains(dIndex) Then ph.Remove(dIndex)
        Next
        h.Clear()
        For Each b In d.Borders
            Dim spi = b.StartPointIndex
            Dim epi = b.EndPointIndex
            If Not h.Contains(spi) Then h.Add(spi)
            If Not h.Contains(epi) Then h.Add(epi)

            If SecState.PointToDistrict.ContainsKey(spi) Then
                Dim sh = SecState.PointToDistrict(spi)
                If Not sh.Contains(dIndex) Then sh.Add(dIndex)
            End If

            If SecState.PointToDistrict.ContainsKey(epi) Then
                Dim eh = SecState.PointToDistrict(epi)
                If Not eh.Contains(dIndex) Then eh.Add(dIndex)
            End If
        Next
    End Sub

    Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
        If Picture IsNot Nothing Then Graph()
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
                RaiseEvent ControlModeChanged(Me.ControlMode)
            Case Keys.F
                Dim FocusPoint1 As Vector = FocusPoint
                FocusOn()
                Dim FocusPoint2 As Vector = FocusPoint
                If Equal(FocusPoint1, FocusPoint2) Then FocusIn()
                e.Handled = True
            Case Keys.Control Or Keys.A
                SelectAllDistricts()
                e.Handled = True
            Case Else
                e.Handled = False
        End Select
        If e.Handled Then Graph()

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
                        SelectByPick(e.X, e.Y, (Control.ModifierKeys And Keys.Control) <> 0)
                        Graph()
                        Return
                End Select
            Case 2
                Select Case e.Button
                    Case Windows.Forms.MouseButtons.Left
                        SelectByPick(e.X, e.Y, (Control.ModifierKeys And Keys.Control) <> 0)
                        FocusIn()
                        Graph()
                        Return
                    Case Windows.Forms.MouseButtons.Right
                        ResetView()
                        Graph()
                        Return
                End Select
        End Select

        MyBase.DoMouseUp(e)
    End Sub
End Class
