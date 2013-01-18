'==========================================================================
'
'  File:        SecDistrict.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: SEC区域
'  Version:     2013.01.18.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Color = System.Drawing.Color
Imports GraphSystem
Imports FileSystem

Public Class SecState
    Public Sec As SEC_Simple.FileInfo
    Public PointToDistrict As New Dictionary(Of Integer, HashSet(Of Integer))
    Public DistrictToPoint As New Dictionary(Of Integer, HashSet(Of Integer))
    Public SecPoints As New List(Of SecPoint)
    Public SecDistricts As New List(Of SecDistrict)
End Class

Public Interface IPickShowable
    Sub ShowAsPicked()
    Sub ShowAsUnpicked()
End Interface

Public Class SecPoint
    Inherits NDObject
    Implements IPickShowable

    Public ReadOnly SecState As SecState
    Public ReadOnly Index As Integer
    Public Picked As Boolean

    Public Sub New()
    End Sub
    Public Sub New(ByVal SecState As SecState, ByVal Index As Integer, ByVal Picked As Boolean)
        Me.SecState = SecState
        Me.Index = Index
        Me.Picked = Picked
        If Picked Then ShowAsPicked()
    End Sub

    Public Overrides Function Copy() As NDObject
        Return BaseCopy()
    End Function
    Public Function GetLine() As Line
        Dim p = SecState.Sec.Points(Index)

        Dim lz As New List(Of Double)

        For Each dIndex In SecState.PointToDistrict(Index)
            Dim d = SecState.Sec.Districts(dIndex)
            Dim z = d.GetZ(p.x, p.y)
            lz.Add(z)
        Next

        Dim zLower = 0
        Dim zUpper = lz.Max
        If lz.Count >= 2 Then zLower = lz.Min

        Dim Line As Line
        If Picked Then
            Line = New Line(New Vector(p.x, p.y, zLower), New Vector(p.x, p.y, zUpper), Color.Blue)
        Else
            Line = New Line(New Vector(p.x, p.y, zLower), New Vector(p.x, p.y, zUpper), Color.Red)
        End If
        Return Line
    End Function
    Public Overrides Function Complete() As IPicObj()
        Dim Line = GetLine()
        Line.HomotheticTransformation = HomotheticTransformation

        Return Line.Complete()
    End Function

    Public Sub ShowAsPicked() Implements IPickShowable.ShowAsPicked
        Picked = True
    End Sub
    Public Sub ShowAsUnpicked() Implements IPickShowable.ShowAsUnpicked
        Picked = False
    End Sub
End Class

Public Class SecDistrict
    Inherits NDObject
    Implements IPickableGroup
    Implements IPickShowable

    Public ReadOnly SecState As SecState
    Public ReadOnly Index As Integer
    Public Picked As Boolean

    Public Sub New()
    End Sub
    Public Sub New(ByVal SecState As SecState, ByVal Index As Integer, ByVal Picked As Boolean)
        Me.SecState = SecState
        Me.Index = Index
        Me.Picked = Picked
        If Picked Then ShowAsPicked()
    End Sub

    Public Overrides Function Copy() As NDObject
        Return BaseCopy()
    End Function
    Public Function GetTopRegion() As Region
        Dim d = SecState.Sec.Districts(Index)
        '绘制上顶面
        Dim Points As New List(Of Vector)
        For Each b In d.Borders
            Dim p = SecState.Sec.Points(b.StartPointIndex)
            Points.Add(New Vector(p.x, p.y, d.GetZ(p.x, p.y)))
        Next
        If Picked Then
            Return New Region(Points.ToArray(), CSelectionColor(GetColor(d.Terrain)))
        Else
            Return New Region(Points.ToArray(), GetColor(d.Terrain))
        End If
    End Function
    Public Function GetSideRegions() As Region()
        Dim d = SecState.Sec.Districts(Index)
        Dim l As New List(Of Region)()
        '绘制侧面
        Dim BorderOfDistrict As Integer = 0
        For Each b In d.Borders
            If b.NeighborDistrictIndex = -1 Then
                BorderOfDistrict += 1
                l.Add(Nothing)
                Continue For
            End If

            Dim StartPoint = SecState.Sec.Points(b.StartPointIndex)
            Dim EndPoint = SecState.Sec.Points(b.EndPointIndex)
            Dim Az As Double = d.GetZ(StartPoint.x, StartPoint.y)
            Dim Bz As Double
            Dim Cz As Double
            Dim Dz As Double = d.GetZ(EndPoint.x, EndPoint.y)

            If b.NeighborDistrictIndex = -1 Then
                Bz = 0
                Cz = 0
            Else
                Dim nd = SecState.Sec.Districts(b.NeighborDistrictIndex)
                Bz = nd.GetZ(StartPoint.x, StartPoint.y)
                Cz = nd.GetZ(EndPoint.x, EndPoint.y)
            End If

            Const epsilon As Double = 0.25 '误差范围，不能过小，否则会顶点数过多，超出Short范围
            If Az - Bz < epsilon OrElse Dz - Cz < epsilon Then
                BorderOfDistrict += 1
                l.Add(Nothing)
                Continue For '两邻边重合或者当前区域比相邻区域低，无需绘制
            End If

            Dim OA As New Vector(StartPoint.x, StartPoint.y, Az)
            Dim OB As New Vector(StartPoint.x, StartPoint.y, Bz)
            Dim OC As New Vector(EndPoint.x, EndPoint.y, Cz)
            Dim OD As New Vector(EndPoint.x, EndPoint.y, Dz)

            Dim SidePolygon As Region
            If Picked Then
                SidePolygon = New Region(New Vector() {OA, OB, OC, OD}, CSelectionColor(Color.White.ToArgb()))
            Else
                SidePolygon = New Region(New Vector() {OA, OB, OC, OD}, Color.White.ToArgb())
            End If
            l.Add(SidePolygon)
        Next
        Return l.ToArray()
    End Function
    Public Overrides Function Complete() As IPicObj()
        Dim l As New List(Of IPicObj)

        Dim TopRegion = GetTopRegion()
        Dim Points = TopRegion.PosArray
        For k = 0 To Points.Count - 1
            Dim i = k
            Dim j = (k + 1) Mod Points.Count
            Dim Line As New Line(Points(i), Points(j), Color.Red)
            Line.HomotheticTransformation = HomotheticTransformation
            l.AddRange(Line.Complete())
        Next
        TopRegion.HomotheticTransformation = HomotheticTransformation
        l.AddRange(TopRegion.Complete())

        Dim SideRegions = GetSideRegions()
        For Each r In SideRegions
            If r Is Nothing Then Continue For
            r.HomotheticTransformation = HomotheticTransformation
            l.AddRange(r.Complete())
        Next

        Return l.ToArray()
    End Function

    Public Shared Function GetColor(ByVal Terrain As SEC.TerrainInfo) As ColorInt32
        Dim c As ColorInt32

        '根据主类型
        Select Case Terrain.MajorType
            Case 0  '陆地
                c = Color.DarkOrange
            Case 1 '雪地
                c = Color.White
            Case 2 '深水
                c = Color.DarkBlue
            Case 3 '浅水
                c = Color.RoyalBlue
            Case 4 '水下
                c = Color.CadetBlue
            Case Else
                Throw New IO.InvalidDataException
        End Select

        '根据子类型
        Select Case Terrain.MinorType
            Case 0 '柔软地面(土壤、沙)

            Case 1 '草地
                c = Color.LimeGreen
            Case 2 '硬质地面(路、甲板)
                c = Color.Gainsboro
            Case 3 '弱硬质地面
                c = Color.Gainsboro
            Case 4 '木条地面
                c = Color.Goldenrod
            Case 5 '木质地面
                c = Color.Goldenrod
            Case 6 '半柔软地面
                c = Color.Khaki
            Case 7 '雪地

            Case 8 '冰面
                c = Color.White
            Case 9 '半硬质地面
                c = Color.Linen
            Case 10 '草丛
                c = Color.Green
            Case 11 '金属网格地面(铁栏杆、铁楼梯)
                c = Color.Silver
            Case 12 '金属地面
                c = Color.Silver
            Case 13 '浅水滩

            Case 14 '水

            Case 15 '石子地
                c = Color.Silver
            Case Else
                Throw New IO.InvalidDataException
        End Select

        '根据是否可进入
        If Not Terrain.IsEnterable Then
            c.R \= 4
            c.G \= 4
            c.B \= 4
        End If

        '根据是否是阴影
        If Terrain.IsShadow Then
            c.R \= 2
            c.G \= 2
            c.B \= 2
        End If

        '根据是否有未知的属性
        If Terrain.UnknownAttributes <> "None" Then
            c = &HFFFF00FF
        End If

        Return c
    End Function
    Public Shared Function CSelectionColor(ByVal c As ColorInt32) As ColorInt32
        Dim R As Byte = (CInt(Not c.R) + 191) \ 2
        Dim G As Byte = (CInt(Not c.G) + 191) \ 2
        Dim B As Byte = (CInt(Not c.B) + 191) \ 2
        Return New ColorInt32(c.A, R, G, B)
    End Function

    Public Sub ShowAsPicked() Implements IPickShowable.ShowAsPicked
        Picked = True
    End Sub
    Public Sub ShowAsUnpicked() Implements IPickShowable.ShowAsUnpicked
        Picked = False
    End Sub

    Public Function GetT(ByVal PickingLine As Line, ByRef SubPicking As NDObject) As Double Implements IPickableGroup.GetT
        Dim ht As HomotheticTransformation
        If HomotheticTransformation Is Nothing Then
            ht = New HomotheticTransformation
        Else
            ht = HomotheticTransformation.Invert
        End If
        PickingLine = New Line(ht.CPos(PickingLine.Pos), ht * PickingLine.Dir, PickingLine.LowerBound, PickingLine.UpperBound, Drawing.Color.Transparent)

        Dim TopRegion = GetTopRegion()
        Dim SideRegions = GetSideRegions()
        Dim Regions = (New Region() {TopRegion}).Concat(SideRegions).ToArray()

        Dim MinT As Double = Double.PositiveInfinity
        Dim Region As Region = Nothing

        For k = 0 To Regions.Length - 1
            Dim r = Regions(k)
            If r Is Nothing Then Continue For
            Dim t = GetTOfRegion(r.PosArray, PickingLine)
            If t > 0 AndAlso t < MinT Then
                MinT = t
                Region = r
            End If
        Next
        If Region Is Nothing Then
            SubPicking = Nothing
            Return Double.PositiveInfinity
        End If

        Dim Center = Region.PosArray.Aggregate(Function(a, b) a + b) * (1 / Region.PosArray.Length)
        Dim Radius As Double = Region.PosArray.Select(Function(v) (v - Center).Norm).Min() * (1 / 8)

        If Region Is TopRegion Then
            Dim MinD = Double.PositiveInfinity
            Dim MinPIndex = -1
            Dim MinTP = Double.PositiveInfinity
            Dim pIndex = 0
            For Each v In Region.PosArray
                Dim d = Double.PositiveInfinity
                Dim t = GetTOfPoint(v, PickingLine, d)
                If d < Radius AndAlso t < MinTP AndAlso t > 0 AndAlso Not Double.IsPositiveInfinity(t) Then
                    MinD = d
                    MinPIndex = pIndex
                    MinTP = t
                End If
                pIndex += 1
            Next
            If MinD < Radius Then
                Dim b = SecState.Sec.Districts(Index).Borders(MinPIndex)
                SubPicking = SecState.SecPoints(b.StartPointIndex)
                Return Min(MinTP, MinT)
            End If
        Else
            Dim BA = New Line(Region.PosArray(1), Region.PosArray(0), Color.Red)
            Dim CD = New Line(Region.PosArray(2), Region.PosArray(3), Color.Red)

            Dim MinD = Double.PositiveInfinity
            Dim MinLineIndex = -1
            Dim MinTP = Double.PositiveInfinity
            Dim LineIndex = 0
            For Each Line In New Line() {BA, CD}
                Dim d = Double.PositiveInfinity
                Dim t = GetTOfLine(Line, PickingLine, d)
                If d < Radius AndAlso t < MinTP AndAlso t > 0 AndAlso Not Double.IsPositiveInfinity(t) Then
                    MinD = d
                    MinLineIndex = LineIndex
                    MinTP = t
                End If
                LineIndex += 1
            Next
            If MinD < Radius Then
                Dim pIndex = Array.FindIndex(Regions, Function(r) r Is Region) - 1
                Dim b = SecState.Sec.Districts(Index).Borders(pIndex)
                If MinLineIndex = 0 Then
                    SubPicking = SecState.SecPoints(b.StartPointIndex)
                Else
                    SubPicking = SecState.SecPoints(b.EndPointIndex)
                End If
                Return Min(MinTP, MinT)
            End If
        End If

        SubPicking = Me
        Return MinT
    End Function

    ''' <summary>
    ''' 得到平面与选取直线交点在直线的参数式中的t。
    ''' 直线方程 N = Pos + t * Dir, t (- [1, +inf)
    ''' 平面方程 N * Normal + D = 0
    ''' 仅对顶点在正面成逆时针顺序的凸多边形适用。
    ''' 没有交点返回+inf。
    ''' 法线和选取直线正方向相同则返回+inf。
    ''' </summary>
    Public Function GetTOfRegion(ByVal PosArray As Vector(), ByVal PickingLine As Line) As Double
        If PosArray Is Nothing OrElse PosArray.Length < 3 Then Return Double.PositiveInfinity
        Dim Normal As Vector = Vector.VecPro(True, PosArray(1) - PosArray(0), PosArray(2) - PosArray(0))
        Dim D As Double = -Normal * PosArray(0)
        '直线方程 N = Pos + t * Dir, t <- [1, +inf)
        '平面方程 N * Normal + D = 0
        '合并，可得 t = - (Normal * Pos + D) / (Normal * Dir)
        Dim Divisor As Double = Normal * PickingLine.Dir

        '法线和选取直线正方向相同则返回+inf。
        If Divisor >= 0 Then Return Double.PositiveInfinity

        Dim t As Double = -(Normal * PickingLine.Pos + D) / Divisor
        Dim P As Vector = PickingLine.Pos + t * PickingLine.Dir

        '如果不在每条边的箭头方向在正面的左侧，则返回+inf。
        For n As Integer = 0 To PosArray.Length - 2
            If Vector.VecPro(True, PosArray(n + 1) - PosArray(n), P - PosArray(n)) * Normal < 0 Then
                Return Double.PositiveInfinity
            End If
        Next
        If Vector.VecPro(True, PosArray(0) - PosArray(PosArray.Length - 1), P - PosArray(PosArray.Length - 1)) * Normal < 0 Then
            Return Double.PositiveInfinity
        End If

        Return t
    End Function

    ''' <summary>
    ''' 得到直线到选取直线垂点在直线的参数式中的t。
    ''' 直线方程 N = Pos + t * Dir, t (- [1, +inf)
    ''' </summary>
    Private Overloads Function GetTOfLine(ByVal Line As Line, ByVal PickingLine As Line, ByRef Distance As Double) As Double
        'PickingLine AB
        'Line CD
        'A = PickingLine.Pos
        'C = Line.Pos
        'B、D为垂点
        'n = DirAB x DirCD
        Dim n = Vector.VecPro(True, PickingLine.Dir, Line.Dir)
        If n.Norm = 0 Then
            '平行线，若Line两端点均不在直线方程的t (- [1, +inf)范围内，则认为不靠近
            Dim TempDistance1 = 0
            Dim TempDistance2 = 0
            Dim Ts = New List(Of Double)
            Ts.Add(GetTOfPoint(Line.PosA, PickingLine, TempDistance1))
            Ts.Add(GetTOfPoint(Line.PosB, PickingLine, TempDistance2))
            Dim TsValid = Ts.Where(Function(tt) tt > 1).ToArray()
            Dim t = Ts.Min
            If TsValid.Length > 0 Then
                t = TsValid.Min
            End If
            If (Line.PosA - PickingLine.PosA) * PickingLine.Dir < 0 AndAlso (Line.PosB - PickingLine.PosA) * PickingLine.Dir < 0 Then
                Distance = Min((Line.PosA - PickingLine.PosA).Norm, (Line.PosB - PickingLine.PosA).Norm)
            Else
                Distance = Min(TempDistance1, TempDistance2)
            End If
            Return t
        End If
        Dim nDir = n.Dir
        Dim AC = Line.Pos - PickingLine.Pos
        Dim BD = (AC * nDir) * nDir
        Distance = BD.Norm

        'B = A + t1 DirAB
        'D = C + t2 DirCD
        'BD = AC + t2 DirCD - t1 DirAB
        '-t1 DirAB + t2 DirCD = BD - AC
        '-t1 n = (BD - AC) x DirCD
        't1 = (DirCD x (BD - AC)) . n / n.Sqr
        't2 n = DirAB x (BD - AC)
        't2 = (DirAB x (BD - AC)) . n / n.Sqr
        'BD - AC = CD - AB

        Dim ndnSqr = n * (1 / n.Sqr)
        Dim t1 = Vector.VecPro(True, Line.Dir, BD - AC) * ndnSqr
        Dim t2 = Vector.VecPro(True, PickingLine.Dir, BD - AC) * ndnSqr

        If (t1 - Line.LowerBound) * (Line.UpperBound - t1) >= 0 AndAlso (t2 - PickingLine.LowerBound) * (PickingLine.UpperBound - t2) >= 0 Then
            Return t2
        End If

        With Nothing
            Dim Ts As New List(Of Double)
            Ts.Add(t2)
            Dim TempDistance1 = 0
            Dim TempDistance2 = 0
            Ts.Add(GetTOfPoint(Line.PosA, PickingLine, TempDistance1))
            Ts.Add(GetTOfPoint(Line.PosB, PickingLine, TempDistance2))
            Distance = Min(Distance, TempDistance1, TempDistance2)
            Dim TsValid = Ts.Where(Function(tt) tt > 1).ToArray()
            Dim t = Ts.Min
            If TsValid.Length > 0 Then
                t = TsValid.Min
            End If
            Return t
        End With
    End Function

    ''' <summary>
    ''' 得到点到选取直线垂点在直线的参数式中的t。
    ''' 直线方程 N = Pos + t * Dir, t (- [1, +inf)
    ''' </summary>
    Private Overloads Function GetTOfPoint(ByVal Pos As Vector, ByVal PickingLine As Line, ByRef Distance As Double) As Double
        Dim AP = Pos - PickingLine.Pos
        Dim nScale = PickingLine.Dir.Norm
        If nScale <= 0 Then
            Distance = Double.PositiveInfinity
            Return Double.PositiveInfinity
        End If
        Dim n = PickingLine.Dir.Dir
        Dim t = AP * n / nScale
        Dim d = (AP - (AP * n) * n).Norm
        Distance = d
        Return t
    End Function
End Class
