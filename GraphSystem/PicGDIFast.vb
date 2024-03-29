'==========================================================================
'
'  File:        PicGDIFast.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: PicGDIFast实现
'  Created:     2007.08.04.02:09(GMT+8:00)
'  Version:     0.5 2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports Microsoft.VisualBasic

#Region " GDI+实现类(透视作图) "
''' <summary>GDI+实现类(透视作图)</summary>
Public Class PicGDIPlusPer
    Implements IPic

    Private NDWorld As NDWorld
    Private g As Graphics
    Private ImageDistanceValue As Double = 2 '相距 眼睛到焦点(原点)的距离
    Private FociValue As Double = 0.3 '焦距
    Private Cut As Double = 1.0E+20
    Public EnableSort As Boolean = False '是否排序

    Private Final As New List(Of IPicObj)
    Private LineIndex As New List(Of Int32)
    Private RegionIndex As New List(Of Int32)
    Private ImageQuadrangleIndex As New List(Of Int32)

    Private SemiWidth As Integer
    Private SemiHeight As Integer

#Region " 暴露的接口实现 "
    Public Sub New(ByVal ImageDistanceValue As Double)
        ImageDistance = ImageDistanceValue
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
        If Grapher.Image Is Nothing Then Grapher.Image = New Bitmap(Grapher.Width, Grapher.Height)
        NDWorld = Grapher.NDWorld
        SemiWidth = Grapher.Width \ 2
        SemiHeight = Grapher.Height \ 2
        UseImageBuffer = True
        If g IsNot Nothing Then g.Dispose()
        g = Graphics.FromImage(Grapher.Image)
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
    End Sub
    Public Sub Clear(ByVal Color As ColorInt32) Implements IPic.Clear
        g.Clear(Color.ToColor)
    End Sub
    Public Sub Draw(ByVal Obj As IPicObj) Implements IPic.Draw
        Final.Add(Obj)
    End Sub
    Public Property ImageDistance() As Double Implements IPic.ImageDistance
        Get
            Return ImageDistanceValue
        End Get
        Set(ByVal Value As Double)
            If Value > 0 Then ImageDistanceValue = Value
        End Set
    End Property
#End Region

    Public Sub Graph() Implements IPic.Graph
        NDWorld.GraphClear(Me) '清屏
        NDWorld.GraphObject(Me)

        '透视投影及空间剪裁
        PerspectiveProjectAndClip()

        '排序
        If EnableSort Then Sort() 'Final.Sort(AddressOf Compare)

        '为每个类型的物体生成索引
        GenerateIndex()

        For n As Integer = 0 To Final.Count - 1
            Dim o As IPicObj = Final(n)
            If TypeOf o Is Line Then
                DrawLine(o)
            ElseIf TypeOf o Is Region Then
                DrawRegion(o)
            ElseIf TypeOf o Is ImageQuadrangle Then
                DrawImageQuadrangle(o)
            End If
        Next

        Final.Clear()
        LineIndex.Clear()
        RegionIndex.Clear()
        ImageQuadrangleIndex.Clear()

        NDWorld.GraphText(Me)
    End Sub

    Private Function CPer(ByVal Pos As Vector) As Vector
        Return New Vector(Pos(0) * ImageDistanceValue / Pos(2), Pos(1) * ImageDistanceValue / Pos(2), Pos(2))
    End Function
    Private Function PerClipLine(ByRef Line As Line) As Line
        With Line
            Dim Px As Double = .Pos(0)
            Dim Py As Double = .Pos(1)
            Dim Pz As Double = .Pos(2)
            Dim Dx As Double = .Dir(0)
            Dim Dy As Double = .Dir(1)
            Dim Dz As Double = .Dir(2)
            Dim l As Double = .LowerBound
            Dim u As Double = .UpperBound

            '对Z轴裁边，去掉<0的部分和>Cut的部分
            If l > u Then Exchange(l, u)
            If Dz = 0 Then
                If Not (Pz > 0) Then Return Nothing
                If Not (Pz < Cut) Then Return Nothing
            Else
                Dim zl As Double = -Pz / Dz
                Dim zu As Double = (Cut - Pz) / Dz
                If zl > zu Then Exchange(zl, zu)
                If u < zl Then Return Nothing
                If l < zl Then l = zl
                If l > zu Then Return Nothing
                If u > zu Then u = zu
            End If

            '进行透视计算
            Dim k As Double = ImageDistanceValue / Pz
            Px = Px * k
            Py = Py * k
            Dx = Dx * ImageDistanceValue - Dz * Px
            Dy = Dy * ImageDistanceValue - Dz * Py
            If Double.IsNegativeInfinity(l) Then l = Double.MinValue
            If Double.IsPositiveInfinity(l) Then l = Double.MaxValue
            If Double.IsNegativeInfinity(u) Then u = Double.MinValue
            If Double.IsPositiveInfinity(u) Then u = Double.MaxValue
            Dim Dzq As Double = Dz / Pz * (Pz + l * Dz) * (Pz + u * Dz)
            Dim Pzq As Double = Pz - u * l * Dz ^ 2 / Pz
            Dim lq As Double = l / (Pz + l * Dz)
            Dim uq As Double = u / (Pz + u * Dz)

            '屏幕裁边
            If lq > uq Then Exchange(lq, uq)
            Dim xl, xu, yl, yu As Double
            If Dx = 0 Then
                If Px < -SemiWidth Then Return Nothing
                If Px > SemiWidth Then Return Nothing
            Else
                xl = DIV(-SemiWidth - Px, Dx)
                xu = DIV(SemiWidth - Px, Dx)
                If xl > xu Then Exchange(xl, xu)
                lq = Max(lq, xl)
                uq = Min(uq, xu)
            End If
            If Dy = 0 Then
                If Py < -SemiHeight Then Return Nothing
                If Py > SemiHeight Then Return Nothing
            Else
                yl = DIV(-SemiHeight - Py, Dy)
                yu = DIV(SemiHeight - Py, Dy)
                If yl > yu Then Exchange(yl, yu)
                lq = Max(lq, yl)
                uq = Min(uq, yu)
            End If
            If lq > uq Then Return Nothing
            .LowerBound = lq
            .UpperBound = uq

            .Pos = New Vector(Px, Py, Pzq)
            .Dir = New Vector(Dx, Dy, Dzq)
            Return Line
        End With
    End Function
    Private Function PerClipRegion(ByRef Region As Region) As Region
        Dim TempPos As New Collections.Generic.List(Of Vector)
        If Region.PosArray Is Nothing OrElse Region.PosArray.GetLength(0) = 0 Then Return Nothing
        Dim LastPos As Vector = Region.PosArray(Region.PosArray.GetUpperBound(0)).Part(3)
        For Each Pos As Vector In Region.PosArray
            Dim p As Vector = Pos.Part(3)
            If LastPos(2) > 0 AndAlso p(2) > 0 Then
                TempPos.Add(p)
                LastPos = p
                Continue For
            Else
                Dim Dir As Vector = p - LastPos
                Dim CurrentPos As Vector
                Dim LowerBound As Double = 0
                Dim UpperBound As Double = Dir.Norm
                Dir = Dir.Dir

                '对Z轴裁边，去掉<0的部分
                If Dir(2) = 0 Then
                    If Not (p(2) > 0) Then
                        LastPos = p
                        Continue For
                    End If
                Else
                    Dim zl As Double = -LastPos(2) / Dir(2)
                    If Dir(2) > 0 Then
                        If UpperBound < zl Then
                            LastPos = p
                            Continue For
                        End If
                        If LowerBound < zl Then
                            LowerBound = zl
                            CurrentPos = LastPos + LowerBound * Dir
                            TempPos.Add(CurrentPos)
                            TempPos.Add(p)
                            LastPos = p
                            Continue For
                        End If
                    ElseIf Dir(2) < 0 Then
                        If LowerBound > zl Then
                            LastPos = p
                            Continue For
                        End If
                        If UpperBound > zl Then
                            UpperBound = zl
                            CurrentPos = LastPos + UpperBound * Dir
                            TempPos.Add(CurrentPos)
                            LastPos = p
                            Continue For
                        End If
                    End If
                    LastPos = p
                    Continue For
                End If
            End If
        Next

        If TempPos Is Nothing Then Return Nothing
        Dim Num As Integer = TempPos.Count
        If Num - 1 < 2 Then Return Nothing

        Dim PosArray As Vector() = New Vector(Num - 1) {}
        For n As Integer = 0 To Num - 1
            PosArray(n) = CPer(TempPos(n))
        Next
        Try
            If AreaTriangle2D(PosArray(0)(0), PosArray(0)(1), PosArray(Num \ 4 + 1)(0), PosArray(Num \ 4 + 1)(1), PosArray(Num \ 2 + 1)(0), PosArray(Num \ 2 + 1)(1)) > 0 Then Return Nothing '去掉顺时针的Region
        Catch
            Return Nothing
        End Try
        Region.PosArray = PosArray
        Return Region
    End Function
    Private Sub PerspectiveProjectAndClip()
        '透视投影及空间剪裁

        '点的透视投影: x'=x/z, y'=y/z, z'=z
        '直线的透视投影: Px'=Px/Pz*i, Py'=Py/Pz*i, Dx'=Dx*i-Dz*Px/Pz*i, Dy'=Dy*i-Dz*Py/Pz*i, l'=l/(Pz+l*Dz), u'=u/(Pz+u*Dz)
        '                Dz'=Dz/Pz*(Pz+l*Dz)*(Pz+u*Dz), Pz'=Pz-l*u*Dz^2/Pz

        '空间剪裁共有5个条件
        '能够保留的物体需要至少有一个点在这五个面所围成的空间里
        'z > 0
        'x / z * ImageDistanceValue > -SemiWidth
        'x / z * ImageDistanceValue < SemiWidth
        'y / z * ImageDistanceValue > -SemiHeight
        'y / z * ImageDistanceValue < SemiHeight

        Dim TempFinal As New List(Of IPicObj)
        For n As Integer = 0 To Final.Count - 1
            Dim o As IPicObj = Final(n)
            If TypeOf o Is Line Then
                Dim t As Line = PerClipLine(o)
                If t IsNot Nothing Then TempFinal.Add(t)
            ElseIf TypeOf o Is Region Then
                Dim t As Region = PerClipRegion(o)
                If t IsNot Nothing Then TempFinal.Add(t)
            ElseIf TypeOf o Is ImageQuadrangle Then
                Dim t As ImageQuadrangle = o
                If Not (t.PosA(2) <= 0 OrElse Abs(t.PosA(0) * ImageDistanceValue) >= SemiWidth * t.PosA(2) OrElse Abs(t.PosA(1) * ImageDistanceValue) >= SemiHeight * t.PosA(2)) Then
                    TempFinal.Add(t)
                ElseIf Not (t.PosB(2) <= 0 OrElse Abs(t.PosB(0) * ImageDistanceValue) >= SemiWidth * t.PosB(2) OrElse Abs(t.PosB(1) * ImageDistanceValue) >= SemiHeight * t.PosB(2)) Then
                    TempFinal.Add(t)
                ElseIf Not (t.PosC(2) <= 0 OrElse Abs(t.PosC(0) * ImageDistanceValue) >= SemiWidth * t.PosC(2) OrElse Abs(t.PosC(1) * ImageDistanceValue) >= SemiHeight * t.PosC(2)) Then
                    TempFinal.Add(t)
                ElseIf Not (t.PosD(2) <= 0 OrElse Abs(t.PosD(0) * ImageDistanceValue) >= SemiWidth * t.PosD(2) OrElse Abs(t.PosD(1) * ImageDistanceValue) >= SemiHeight * t.PosD(2)) Then
                    TempFinal.Add(t)
                End If
            End If
        Next
        Final.Clear()
        Final = TempFinal
    End Sub

#Region " 排序 "
    'TODO 排序 这个有很大问题
    ''' <summary>
    ''' 比较
    ''' 记a与b公垂线向量(从a上指向b上)在z轴上投影为正为a大于b，同理定义a等于b，a小于b，分别返回1，0，-1
    ''' a大于b时，a遮蔽b
    ''' </summary>
    Private Function CompareLine(ByVal a As Line, ByVal b As Line) As Double
        Return b.Pos(2) - a.Pos(2)
        '下面的方法可以准确判断两条直线的位置关系，但是不具有良序性
        'Dim r As Vector = Vector.VecPro(True, a.Dir, b.Dir).Dir
        'r = ((b.Pos - a.Pos) * r) * r
        'If Equal(r.Sqr, 0, -2) Then Return b.Pos(2) - a.Pos(2)
        ''If r is from a to b Then (b.Pos - a.Pos - r) * r = 0, thus (b.Pos - a.Pos) * r = r ^ 2 > 0
        ''If r is from b to a Then (b.Pos - a.Pos + r) * r = 0, thus (b.Pos - a.Pos) * r = -r ^ 2 < 0
        'Return ((b.Pos - a.Pos) * r) * r(2)
    End Function

    ''' <summary>
    ''' 比较
    ''' 记P在ABC正方向为l大于ABC，同理定义P等于ABC，P小于ABC，分别返回1，0，-1
    ''' ABC正方向指 ABxAC 方向，即三顶点逆时针的方向时的上方
    ''' P大于ABC时，l遮蔽ABC
    ''' </summary>
    Private Function ComparePointTriangle(ByVal P As Vector, ByVal A As Vector, ByVal B As Vector, ByVal C As Vector) As Double
        Dim ret As Double = (P - A) * Vector.VecPro(True, B - A, C - A)

        Return ret
    End Function

    Private Function Compare(ByVal a As IPicObj, ByVal b As IPicObj) As Integer
        Dim ret As Double
        Dim l As Line
        Dim r As Region
        Dim i As ImageQuadrangle
        Dim k As Integer
        If TypeOf a Is Line Then
            If TypeOf b Is Line Then
                ret = CompareLine(a, b)
            ElseIf TypeOf b Is Region Then
                l = a
                r = b
                k = r.PosArray.GetLength(0)
                ret = ComparePointTriangle(l.Pos, r.PosArray(0), r.PosArray(k \ 4 + 1), r.PosArray(k \ 2 + 1))
                If Equal(ret, 0, -2) Then Return 1
            ElseIf TypeOf b Is ImageQuadrangle Then
                l = a
                i = b
                ret = ComparePointTriangle(l.Pos, i.PosA, i.PosB, i.PosC)
            End If
        ElseIf TypeOf a Is Region Then
            If TypeOf b Is Line Then
                l = b
                r = a
                k = r.PosArray.GetLength(0)
                ret = -ComparePointTriangle(l.Pos, r.PosArray(0), r.PosArray(k \ 4 + 1), r.PosArray(k \ 2 + 1))
                If Equal(ret, 0, -2) Then Return -1
            ElseIf TypeOf b Is Region Then
                r = b
                k = r.PosArray.GetLength(0)
                ret = ComparePointTriangle(CType(a, Region).PosArray(0), r.PosArray(0), r.PosArray(k \ 4 + 1), r.PosArray(k \ 2 + 1))
            ElseIf TypeOf b Is ImageQuadrangle Then
                i = b
                ret = ComparePointTriangle(CType(a, Region).PosArray(0), i.PosA, i.PosB, i.PosC)
            End If
        ElseIf TypeOf a Is ImageQuadrangle Then
            If TypeOf b Is Line Then
                l = b
                i = a
                ret = -ComparePointTriangle(l.Pos, i.PosA, i.PosB, i.PosC)
            ElseIf TypeOf b Is Region Then
                i = a
                ret = -ComparePointTriangle(CType(b, Region).PosArray(0), i.PosA, i.PosB, i.PosC)
            ElseIf TypeOf b Is ImageQuadrangle Then
                i = b
                ret = ComparePointTriangle(CType(a, ImageQuadrangle).PosA, i.PosA, i.PosB, i.PosC)
            End If
        End If
        Return sgn(ret)
    End Function

    Private Sub Sort()
        'TODO 排序 这个有很大问题 主要是面直接作为XY平面上一点了
        Dim TempFinal As IPicObj() = Final.ToArray
        Final.Clear()
        Dim SumZ As Double() = New Double(TempFinal.GetUpperBound(0)) {}
        For n As Integer = 0 To TempFinal.GetUpperBound(0)
            Dim o As IPicObj = TempFinal(n)
            If TypeOf o Is Line Then
                Dim Temp As Line = o
                Dim DepthBias = 0.1
                If Temp.Dir.Norm = 0 Then
                    SumZ(n) = -Temp.Pos(2) + DepthBias * 2
                ElseIf Double.IsNegativeInfinity(Temp.LowerBound) OrElse Double.IsPositiveInfinity(Temp.UpperBound) Then
                    SumZ(n) = -Temp.Pos(2) + DepthBias
                Else
                    SumZ(n) = -Min(Temp.PosA(2), Temp.PosB(2)) + DepthBias
                End If
            ElseIf TypeOf o Is Region Then
                Dim Temp As Region = o
                Try
                    SumZ(n) = -Temp.PosArray.Min(Function(p) p(2))
                Catch
                    SumZ(n) = -Double.NaN
                End Try
            ElseIf TypeOf o Is ImageQuadrangle Then
                Dim Temp As ImageQuadrangle = o
                SumZ(n) = -Min(Temp.PosA(2), Temp.PosB(2), Temp.PosC(2), Temp.PosD(2))
            End If
        Next
        Array.Sort(SumZ, TempFinal)
        Final.AddRange(TempFinal)
        TempFinal = Nothing
    End Sub
#End Region

    Private Sub GenerateIndex()
        For n As Integer = 0 To Final.Count - 1
            Dim o As IPicObj = Final(n)
            If TypeOf o Is Line Then
                LineIndex.Add(n)
            ElseIf TypeOf o Is Region Then
                RegionIndex.Add(n)
            ElseIf TypeOf o Is ImageQuadrangle Then
                ImageQuadrangleIndex.Add(n)
            End If
        Next
    End Sub

    Public Sub DrawLine(ByVal Line As Line)
        If Line Is Nothing Then Return
        With Line
            Dim x1 As Double = .PosA(0)
            Dim y1 As Double = .PosA(1)
            Dim x2 As Double = .PosB(0)
            Dim y2 As Double = .PosB(1)
            Try
                If Equal(x1, x2) AndAlso Equal(y1, y2) Then
                    Draw2DPie(Line.ColorInt, (x1 + x2) \ 2, (y1 + y2) \ 2, 2)
                Else
                    Draw2DLine(Line.ColorInt, 1, .PosA(0), .PosA(1), .PosB(0), .PosB(1))
                End If
            Catch
            End Try
        End With
    End Sub
    Public Sub DrawRegion(ByVal Region As Region)
        Draw2DRegion(Region.ColorInt, Region.PosArray)
    End Sub
    Public Sub DrawImageQuadrangle(ByVal ImageQuadrangle As ImageQuadrangle)
        With ImageQuadrangle
            If .Bitmap Is Nothing Then Return
            If Not (.Bitmap.Width > 0) Then Return
            If Not (.Bitmap.Height > 0) Then Return

            Dim PosA, PosD, PosC, PosB As Vector
            PosA = CPer(.PosA).Part(2)
            PosB = CPer(.PosB).Part(2)
            PosC = CPer(.PosC).Part(2)
            PosD = CPer(.PosD).Part(2)

            If AreaTriangle2D(PosA(0), PosA(1), PosB(0), PosB(1), PosC(0), PosC(1)) > 0 Then Return '去掉顺时针的Region
            If AreaTriangle2D(PosC(0), PosC(1), PosD(0), PosD(1), PosA(0), PosA(1)) > 0 Then Return '去掉交叉的Region

            Dim w, h As Integer
            w = .Bitmap.Width
            h = .Bitmap.Height

            Dim xBoundL, xBoundR, yBoundT, yBoundB As Double
            xBoundL = Min(PosA(0), PosD(0), PosC(0), PosB(0))
            xBoundR = Max(PosA(0), PosD(0), PosC(0), PosB(0))
            If xBoundL < -SemiWidth Then xBoundL = -SemiWidth
            If xBoundR > SemiWidth Then xBoundR = SemiWidth

            Dim tBase0, tBase1, tBase2, tBase3 As Double
            tBase0 = 1 / (PosD(0) - PosA(0))
            tBase1 = 1 / (PosC(0) - PosD(0))
            tBase2 = 1 / (PosB(0) - PosC(0))
            tBase3 = 1 / (PosA(0) - PosB(0))

            Dim U0, U1, D0, D1, x, y As Double
            ' l = Q0U / Q0Q1, m = Q0D / Q0Q2, n = UX / UD
            ' U = (1 - l) * Q0 + l * Q1, D = (1 - m) * Q0 + m * Q2

            Dim t0, t1, t2, t3 As Double
            Dim xInt, yInt As Integer
            Dim ColorInt As ColorInt32

            For xApo As Double = xBoundL To xBoundR
                t0 = (xApo - PosA(0)) * tBase0
                t1 = (xApo - PosD(0)) * tBase1
                t2 = (xApo - PosC(0)) * tBase2
                t3 = (xApo - PosB(0)) * tBase3

                If t0 >= 0 AndAlso t0 < 1 Then
                    yBoundT = (1 - t0) * PosA(1) + t0 * PosD(1)
                    'U = (1 - t0) * New Vector(0, 0) + t0 * New Vector(w, 0)
                    U0 = t0 * w
                    U1 = 0
                    If t1 >= 0 AndAlso t1 <= 1 Then
                        yBoundB = (1 - t1) * PosD(1) + t1 * PosC(1)
                        'D = (1 - t1) * New Vector(w, 0) + t1 * New Vector(w, h)
                        D0 = w
                        D1 = t1 * h
                    ElseIf t2 >= 0 AndAlso t2 <= 1 Then
                        yBoundB = (1 - t2) * PosC(1) + t2 * PosB(1)
                        'D = (1 - t2) * New Vector(w, h) + t2 * New Vector(0, h)
                        D0 = (1 - t2) * w
                        D1 = h
                    ElseIf t3 >= 0 AndAlso t3 <= 1 Then
                        yBoundB = (1 - t3) * PosB(1) + t3 * PosA(1)
                        'D = (1 - t3) * New Vector(0, h) + t3 * New Vector(0, 0)
                        D0 = 0
                        D1 = (1 - t3) * h
                    Else
                        Continue For
                    End If
                ElseIf t1 >= 0 AndAlso t1 <= 1 Then
                    yBoundT = (1 - t1) * PosD(1) + t1 * PosC(1)
                    U0 = w
                    U1 = t1 * h
                    If t2 >= 0 AndAlso t2 <= 1 Then
                        yBoundB = (1 - t2) * PosC(1) + t2 * PosB(1)
                        D0 = (1 - t2) * w
                        D1 = h
                    ElseIf t3 >= 0 AndAlso t3 <= 1 Then
                        yBoundB = (1 - t3) * PosB(1) + t3 * PosA(1)
                        D0 = 0
                        D1 = (1 - t3) * h
                    Else
                        Continue For
                    End If
                ElseIf t2 >= 0 AndAlso t2 <= 1 Then
                    yBoundT = (1 - t2) * PosC(1) + t2 * PosB(1)
                    U0 = (1 - t2) * w
                    U1 = h
                    If t3 >= 0 AndAlso t3 <= 1 Then
                        yBoundB = (1 - t3) * PosB(1) + t3 * PosA(1)
                        D0 = 0
                        D1 = (1 - t3) * h
                    Else
                        Continue For
                    End If
                Else
                    Continue For
                End If

                If yBoundT > yBoundB Then
                    Exchange(yBoundT, yBoundB)
                    Exchange(U0, D0)
                    Exchange(U1, D1)
                End If

                'n = UX / UD = (X1-U1)/(D1-U1) = (yApo-yBoundT)/(yBoundB-yBoundT)
                For n As Double = 0 To 1 Step 1 / (yBoundB - yBoundT)
                    x = (1 - n) * U0 + n * D0
                    y = (1 - n) * U1 + n * D1
                    If Not (x > 0 AndAlso x < w - 1) OrElse Not (y > 0 AndAlso y < h - 1) Then Continue For
                    xInt = Floor(x)
                    yInt = Floor(y)
                    x -= xInt
                    y -= yInt
                    ColorInt.A = (1 - y) * ((1 - x) * .Bitmap.GetPixel(xInt, yInt).A + x * .Bitmap.GetPixel(xInt + 1, yInt).A) + y * ((1 - x) * .Bitmap.GetPixel(xInt, yInt + 1).A + x * .Bitmap.GetPixel(xInt + 1, yInt + 1).A)
                    ColorInt.R = (1 - y) * ((1 - x) * .Bitmap.GetPixel(xInt, yInt).R + x * .Bitmap.GetPixel(xInt + 1, yInt).R) + y * ((1 - x) * .Bitmap.GetPixel(xInt, yInt + 1).R + x * .Bitmap.GetPixel(xInt + 1, yInt + 1).R)
                    ColorInt.G = (1 - y) * ((1 - x) * .Bitmap.GetPixel(xInt, yInt).G + x * .Bitmap.GetPixel(xInt + 1, yInt).G) + y * ((1 - x) * .Bitmap.GetPixel(xInt, yInt + 1).G + x * .Bitmap.GetPixel(xInt + 1, yInt + 1).G)
                    ColorInt.B = (1 - y) * ((1 - x) * .Bitmap.GetPixel(xInt, yInt).B + x * .Bitmap.GetPixel(xInt + 1, yInt).B) + y * ((1 - x) * .Bitmap.GetPixel(xInt, yInt + 1).B + x * .Bitmap.GetPixel(xInt + 1, yInt + 1).B)
                    Draw2DLinePrecise(ColorInt, 1, xApo, (1 - n) * yBoundT + n * yBoundB, xApo, (1 - n) * yBoundT + n * yBoundB)
                Next
            Next
        End With
    End Sub

    Public Sub Draw2DLine(ByVal Color As ColorInt32, ByVal PenWidth As Integer, ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
        Dim Pen As New Pen(Color.ToColor, PenWidth)
        Try
            g.DrawLine(Pen, x1 + SemiWidth, y1 + SemiHeight, x2 + SemiWidth, y2 + SemiHeight)
        Catch
        End Try
        Pen.Dispose()
    End Sub
    Public Sub Draw2DRegion(ByVal Color As ColorInt32, ByVal Points As Vector())
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            Dim Center = Points.Aggregate(Function(a, b) a + b) * (1 / Points.Length)
            Dim TempPoints(Points.GetUpperBound(0)) As Drawing.Point
            For n As Integer = 0 To Points.GetUpperBound(0)
                Dim p = Points(n) + (Center - Points(n)).Dir() '使得边缘1像素不显示
                If Double.IsNaN(p(0)) Then Return
                If Double.IsNaN(p(1)) Then Return
                If p(0) < Integer.MinValue OrElse p(0) > Integer.MaxValue Then Return
                If p(1) < Integer.MinValue OrElse p(1) > Integer.MaxValue Then Return
                TempPoints(n).X = CInt(p(0)) + SemiWidth
                TempPoints(n).Y = CInt(p(1)) + SemiHeight
            Next
            g.FillPolygon(Brush, TempPoints)
        Catch
        End Try
        Brush.Dispose()
    End Sub
    Public Sub Draw2DPie(ByVal Color As ColorInt32, ByVal x As Integer, ByVal y As Integer, ByVal r As Integer)
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            g.FillPie(Brush, x - r + SemiWidth, y - r + SemiHeight, 2 * r, 2 * r, 0, 360)
        Catch
        End Try
        Brush.Dispose()
    End Sub

    Public Sub Draw2DLinePrecise(ByVal Color As ColorInt32, ByVal PenWidth As Single, ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single) Implements IPic.Draw2DLine
        Dim Pen As New Pen(Color.ToColor, PenWidth)
        Try
            If (x1 = x2 AndAlso y1 = y2) Then
                g.DrawPie(Pen, CSng(x1 + Width / 2 - Pen.Width / 2), CSng(y1 + Height / 2 - Pen.Width / 2), Pen.Width, Pen.Width, 0, 360)
            Else
                g.DrawLine(Pen, CSng(x1 + Width / 2), CSng(y1 + Height / 2), CSng(x2 + Width / 2), CSng(y2 + Height / 2))
            End If
        Catch
        End Try
        Pen.Dispose()
    End Sub
    Public Sub Draw2DRegionPrecise(ByVal Color As ColorInt32, ByVal Points As Vector()) Implements IPic.Draw2DRegion
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            Dim TempPoints(Points.GetUpperBound(0)) As Drawing.PointF
            For n As Integer = 0 To Points.GetUpperBound(0)
                TempPoints(n).X = Points(n)(0) + Width / 2
                TempPoints(n).Y = Points(n)(1) + Height / 2
            Next
            g.FillPolygon(Brush, TempPoints)
        Catch
        End Try
        Brush.Dispose()
    End Sub
    Public Sub Draw2DStringPrecise(ByVal s As String, ByVal Font As System.Drawing.Font, ByVal Color As ColorInt32, ByVal x As Single, ByVal y As Single) Implements IPic.Draw2DString
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            g.DrawString(s, Font, Brush, x, y)
        Catch
        End Try
        Brush.Dispose()
    End Sub


    ' IDisposable
    Private disposedValue As Boolean = False        ' 检测冗余的调用
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '显式调用时释放托管资源
            End If

            '释放共享的非托管资源
            If g IsNot Nothing Then g.Dispose()
        End If
        Me.disposedValue = True
    End Sub
    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
#End Region
