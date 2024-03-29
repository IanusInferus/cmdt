'==========================================================================
'
'  File:        PicGDIPrecise.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: PicGDIPrecise实现
'  Created:     2007.08.04.02:08(GMT+8:00)
'  Version:     0.5 2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing

#Region " GDI+实现类(平行作图) "
''' <summary>GDI+实现类(平行作图)</summary>
''' <remarks>
''' 不实现绘图片四边形
''' </remarks>
Public Class PicGDIPlusPara
    Implements IPic
    Private NDWorld As NDWorld
    Private g As Graphics

    Private SemiWidth As Integer
    Private SemiHeight As Integer

    Public Sub New()
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
    Public Sub Draw2DLine(ByVal Color As ColorInt32, ByVal PenWidth As Single, ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single) Implements IPic.Draw2DLine
        Try
            Dim Pen As New Pen(Color.ToColor, PenWidth)
            If (x1 = x2 AndAlso y1 = y2) Then
                g.DrawRectangle(Pen, CSng(x1 + SemiWidth - Pen.Width / 2), CSng(y1 + SemiHeight - Pen.Width / 2), Pen.Width, Pen.Width)
            Else
                g.DrawLine(Pen, CSng(x1 + SemiWidth), CSng(y1 + SemiHeight), CSng(x2 + SemiWidth), CSng(y2 + SemiHeight))
            End If
        Catch
            Return
        End Try
    End Sub
    Public Sub Draw2DRegion(ByVal Color As ColorInt32, ByVal Points As Vector()) Implements IPic.Draw2DRegion
        Try
            Dim TempPoints(Points.GetUpperBound(0)) As Drawing.Point
            For n As Integer = 0 To Points.GetUpperBound(0)
                TempPoints(n).X = Points(n)(0) + SemiWidth
                TempPoints(n).Y = Points(n)(1) + SemiHeight
            Next
            g.FillPolygon(New SolidBrush(Color.ToColor), TempPoints)
        Catch
            Return
        End Try
    End Sub
    Public Sub Draw2DString(ByVal s As String, ByVal Font As System.Drawing.Font, ByVal Color As ColorInt32, ByVal x As Single, ByVal y As Single) Implements IPic.Draw2DString
        Try
            g.DrawString(s, Font, New SolidBrush(Color.ToColor), x, y)
        Catch
            Return
        End Try
    End Sub
    Public Sub Draw(ByVal Obj As IPicObj) Implements IPic.Draw
        If TypeOf Obj Is Line Then
            DrawLine(Obj)
        ElseIf TypeOf Obj Is Region Then
            DrawRegion(Obj)
            'ElseIf TypeOf Obj Is ImageQuadrangle Then
        End If
    End Sub
    Public Sub Graph() Implements IPic.Graph
        NDWorld.GraphClear(Me) '清屏
        NDWorld.GraphObject(Me)
        NDWorld.GraphText(Me)
    End Sub
    Public Property ImageDistance() As Double Implements IPic.ImageDistance
        Get
            Return Double.MaxValue
        End Get
        Set(ByVal Value As Double)
        End Set
    End Property
    Public ReadOnly Property Left() As Double
        Get
            Return -SemiWidth
        End Get
    End Property
    Public ReadOnly Property Right() As Double
        Get
            Return SemiWidth
        End Get
    End Property
    Public ReadOnly Property Top() As Double
        Get
            Return -SemiHeight
        End Get
    End Property
    Public ReadOnly Property Bottom() As Double
        Get
            Return SemiHeight
        End Get
    End Property
    Public Sub DrawLine(ByVal Line As Line)
        If Line Is Nothing Then Return
        Dim Pos, Dir As Vector, LowerBound, UpperBound As Double, Color As ColorInt32
        Pos = +Line.Pos
        Dir = +Line.Dir
        LowerBound = Line.LowerBound
        UpperBound = Line.UpperBound
        Color = Line.ColorInt

        '对Z轴裁边，去掉<0的部分
        If Dir(2) = 0 Then
            If Not (Pos(2) > ImageDistance) Then Return
        Else
            Dim zl As Double = (ImageDistance - Pos(2)) / Dir(2)
            If Dir(2) > 0 Then
                If UpperBound < zl Then Return
                If LowerBound < zl Then LowerBound = zl
            ElseIf Dir(2) < 0 Then
                If LowerBound > zl Then Return
                If UpperBound > zl Then UpperBound = zl
            Else
                Return
            End If
        End If

        '进行绘图计算，先期绘出重合为一点的
        Dim r As Double
        r = Max(Abs(Dir(0)), Abs(Dir(1)))
        If r = 0 Then
            Dim TempPos As Vector
            If Line.Dir(2) > 0 Then
                TempPos = Pos + LowerBound * Dir
            Else
                TempPos = Pos + UpperBound * Dir
            End If
            Draw2DLine(Color, 2, TempPos(0), TempPos(1), TempPos(0), TempPos(1))
            Return
        End If
        Dir *= 1 / r
        LowerBound *= r
        UpperBound *= r
        If LowerBound > UpperBound Then Exchange(LowerBound, UpperBound)

        '屏幕裁边
        Dim xl, xu, yl, yu As Double
        If Dir(0) = 0 Then
            If Pos(0) < Left Then Return
            If Pos(0) > Right Then Return
        Else
            xl = DIV(Left - Pos(0), Dir(0))
            xu = DIV(Right - Pos(0), Dir(0))
            If xl > xu Then Exchange(xl, xu)
            LowerBound = Max(LowerBound, xl)
            UpperBound = Min(UpperBound, xu)
        End If
        If Dir(1) = 0 Then
            If Pos(1) < Top Then Return
            If Pos(1) > Bottom Then Return
        Else
            yl = DIV(Top - Pos(1), Dir(1))
            yu = DIV(Bottom - Pos(1), Dir(1))
            If yl > yu Then Exchange(yl, yu)
            LowerBound = Max(LowerBound, yl)
            UpperBound = Min(UpperBound, yu)
        End If
        If LowerBound > UpperBound Then Return
        Dim PosA() As Single = New Single() {Pos(0) + LowerBound * Dir(0), Pos(1) + LowerBound * Dir(1), Pos(2) + LowerBound * Dir(2)}
        Dim PosB() As Single = New Single() {Pos(0) + UpperBound * Dir(0), Pos(1) + UpperBound * Dir(1), Pos(2) + UpperBound * Dir(2)}
        Draw2DLine(Line.ColorInt, 1, PosA(0), PosA(1), PosB(0), PosB(1))
    End Sub
    Public Sub DrawRegion(ByVal Region As Region)
        Dim TempPos, TempPos2 As New Collections.Generic.List(Of Vector)

        For n As Integer = 0 To Region.PosArray.GetUpperBound(0)
            TempPos.Add(Region.PosArray(n))
        Next

        Dim LastPos As Vector = TempPos(TempPos.Count - 1)
        Dim Pos As Vector
        For n As Integer = 0 To TempPos.Count - 1
            Pos = TempPos(n)
            If LastPos(2) > ImageDistance AndAlso Pos(2) > ImageDistance Then
                TempPos2.Add(Pos)
                LastPos = Pos
                Continue For
            Else
                Dim Dir As Vector = Pos - LastPos
                Dim CurrentPos As Vector
                Dim LowerBound As Double = 0
                Dim UpperBound As Double = Dir.Norm
                Dir = Dir.Dir

                '对Z轴裁边，去掉<0的部分
                If Dir(2) = 0 Then
                    If Not (Pos(2) > ImageDistance) Then
                        LastPos = Pos
                        Continue For
                    End If
                Else
                    Dim zl As Double = (ImageDistance - LastPos(2)) / Dir(2)
                    If Dir(2) > 0 Then
                        If UpperBound < zl Then
                            LastPos = Pos
                            Continue For
                        End If
                        If LowerBound < zl Then
                            LowerBound = zl
                            CurrentPos = LastPos + LowerBound * Dir
                            TempPos2.Add(CurrentPos)
                            TempPos2.Add(Pos)
                            LastPos = Pos
                            Continue For
                        End If
                    ElseIf Dir(2) < 0 Then
                        If LowerBound > zl Then
                            LastPos = Pos
                            Continue For
                        End If
                        If UpperBound > zl Then
                            UpperBound = zl
                            CurrentPos = LastPos + UpperBound * Dir
                            TempPos2.Add(CurrentPos)
                            LastPos = Pos
                            Continue For
                        End If
                    End If
                    LastPos = Pos
                    Continue For
                End If
            End If
        Next

        Dim TempPos3 As Vector() = New Vector(TempPos2.Count - 1) {}

        For n As Integer = 0 To TempPos2.Count - 1
            TempPos3(n) = TempPos2(n)
        Next

        If TempPos3 Is Nothing Then Return
        If TempPos3.GetUpperBound(0) < 2 Then Return
        If AreaTriangle2D(TempPos3(0)(0), TempPos3(0)(1), TempPos3(1)(0), TempPos3(1)(1), TempPos3(2)(0), TempPos3(2)(1)) > 0 Then Return '去掉顺时针的Region
        Draw2DRegion(Region.ColorInt, TempPos3)
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

#Region " GDI+实现类(透视作图，精确) "
''' <summary>GDI+实现类(透视作图，精确)</summary>
Public Class PicGDIPlusPerPrecise
    Implements IPic
    Private NDWorld As NDWorld
    Private g As Graphics
    Private ImageDistanceValue As Double = 2 '相距 眼睛到焦点(原点)的距离
    Private FociValue As Double = 0.3 '焦距
    Public EnableSort As Boolean = False '是否排序

    Private Final As New List(Of IPicObj)

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
    Public ReadOnly Property Left() As Double
        Get
            Return -SemiWidth
        End Get
    End Property
    Public ReadOnly Property Right() As Double
        Get
            Return SemiWidth
        End Get
    End Property
    Public ReadOnly Property Top() As Double
        Get
            Return -SemiHeight
        End Get
    End Property
    Public ReadOnly Property Bottom() As Double
        Get
            Return SemiHeight
        End Get
    End Property
#End Region

    Public Sub Graph() Implements IPic.Graph
        NDWorld.GraphClear(Me) '清屏
        NDWorld.GraphObject(Me)

        '排序
        If EnableSort Then Sort()

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

        NDWorld.GraphText(Me)
    End Sub

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
                If Double.IsNegativeInfinity(Temp.LowerBound) OrElse Double.IsPositiveInfinity(Temp.UpperBound) Then
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

    Public Sub DrawLine(ByVal Line As Line)
        If Line Is Nothing Then Return
        Dim Pos, Dir As Vector, LowerBound, UpperBound, Color As Double
        Pos = +Line.Pos
        Dir = +Line.Dir
        LowerBound = Line.LowerBound
        UpperBound = Line.UpperBound
        Color = Line.ColorInt

        '对Z轴裁边，去掉<0的部分
        If Dir(2) = 0 Then
            If Not (Pos(2) > 0) Then Return
        Else
            Dim zl As Double = -Pos(2) / Dir(2)
            If Dir(2) > 0 Then
                If UpperBound < zl Then Return
                If LowerBound < zl Then LowerBound = zl
            ElseIf Dir(2) < 0 Then
                If LowerBound > zl Then Return
                If UpperBound > zl Then UpperBound = zl
            Else
                Return
            End If
        End If

        '进行透视计算，先期绘出重合为一点的
        Dim r As Double
        Pos = CPer(Pos)
        Dir = New Vector(Dir(0) * ImageDistanceValue - Dir(2) * Pos(0), Dir(1) * ImageDistanceValue - Dir(2) * Pos(1), Dir(2))
        r = Max(Abs(Dir(0)), Abs(Dir(1)))
        If r = 0 Then
            Dim TempPos As Vector
            If Line.Dir(2) > 0 Then
                TempPos = Pos + LowerBound * Dir
            Else
                TempPos = Pos + UpperBound * Dir
            End If
            DrawLine2(TempPos, Nothing, 0, 0, Color)
            Return
        End If
        Dir(2) = -Dir(2) * Pos(0)
        Dir *= 1 / r
        If Double.IsNegativeInfinity(LowerBound) Then LowerBound = Double.MinValue
        If Double.IsPositiveInfinity(LowerBound) Then LowerBound = Double.MaxValue
        If Double.IsNegativeInfinity(UpperBound) Then UpperBound = Double.MinValue
        If Double.IsPositiveInfinity(UpperBound) Then UpperBound = Double.MaxValue
        LowerBound = (LowerBound * r) / (Line.Pos(2) + LowerBound * Line.Dir(2))
        UpperBound = (UpperBound * r) / (Line.Pos(2) + UpperBound * Line.Dir(2))
        If LowerBound > UpperBound Then Exchange(LowerBound, UpperBound)

        '屏幕裁边
        Dim xl, xu, yl, yu As Double
        If Dir(0) = 0 Then
            If Pos(0) < Left Then Return
            If Pos(0) > Right Then Return
        Else
            xl = DIV(Left - Pos(0), Dir(0))
            xu = DIV(Right - Pos(0), Dir(0))
            If xl > xu Then Exchange(xl, xu)
            LowerBound = Max(LowerBound, xl)
            UpperBound = Min(UpperBound, xu)
        End If
        If Dir(1) = 0 Then
            If Pos(1) < Top Then Return
            If Pos(1) > Bottom Then Return
        Else
            yl = DIV(Top - Pos(1), Dir(1))
            yu = DIV(Bottom - Pos(1), Dir(1))
            If yl > yu Then Exchange(yl, yu)
            LowerBound = Max(LowerBound, yl)
            UpperBound = Min(UpperBound, yu)
        End If
        If LowerBound > UpperBound Then Return
        DrawLine2(Pos, Dir, LowerBound, UpperBound, Line.ColorInt)
    End Sub
    Private Sub DrawLine2(ByVal Pos As Vector, ByVal Dir As Vector, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal Color As ColorInt32)
        'Dim SumZ As Double = Pos(2) ^ 2 * 1024
        'If SumZ = Double.NaN Then Return
        'If SumZ < 0 Then Return
        'If SumZ > 3 * 255 Then SumZ = 3 * 255
        '三基色的大小决定彩色光的亮度，混合色的亮度等于各基色分量亮度之和。
        '三基色的比例决定混合色的色调，当三基色混合比例相同时，色调相同。
        Dim ColorValue As Color = Color.ToColor
        'ColorValue= Drawing.Color.FromArgb(SumZ, Color.ToColor)
        'Dim LightDegree As Double = SumZ / (CInt(ColorValue.R) + CInt(ColorValue.G) + CInt(ColorValue.B))
        'If Not (LightDegree < 1 AndAlso LightDegree > 0) Then LightDegree = 1
        'ColorValue = Drawing.Color.FromArgb(ColorValue.R * LightDegree, ColorValue.G * LightDegree, ColorValue.B * LightDegree)


        'Dim Pen As Pen = New Pen(Drawing.Color.FromArgb(CInt(2 ^ (8 - SumZ)), Color), 1)
        If Dir.Sqr = 0 Then
            'TODO ToEdit
            Draw2DLine(ColorValue.ToArgb, 2, Pos(0), Pos(1), Pos(0), Pos(1))
        Else

            'Dim Magniscale As Double
            'Dim r As Double = Max(Abs(Dir(0)), Abs(Dir(1)))
            'Dim TempPos As Vector
            'Dim TempColor As Color = ColorValue
            'Dim Alpha As Double
            'For t As Double = LowerBound To UpperBound Step r
            '    TempPos = Pos + t * Dir
            '    Magniscale = ImageDistance * (1 / TempPos(2) - 1 / Foci)
            '    Alpha = (1 / Magniscale) ^ 2 * 1024
            '    If Alpha = Double.NaN Then Continue For
            '    If Alpha > 255 Then Alpha = 255
            '    TempColor = Drawing.Color.FromArgb(Alpha, TempColor)
            '    TempPos(0) -= Abs(Magniscale / 4) / 2
            '    TempPos(1) -= Abs(Magniscale / 4) / 2
            '    Draw2DPie(TempColor.ToArgb, TempPos, Abs(Magniscale / 4))
            'Next

            Dim PosA() As Single = New Single() {Pos(0) + LowerBound * Dir(0), Pos(1) + LowerBound * Dir(1), Pos(2) + LowerBound * Dir(2)}
            Dim PosB() As Single = New Single() {Pos(0) + UpperBound * Dir(0), Pos(1) + UpperBound * Dir(1), Pos(2) + UpperBound * Dir(2)}
            Draw2DLine(ColorValue.ToArgb, 1, PosA(0), PosA(1), PosB(0), PosB(1))
            'If PosA(2) > PosB(2) Then 'TODO ToEdit
            '    Draw2DLine(Drawing.Color.Red.ToArgb, 2, PosA(0), PosA(1), PosA(0), PosA(1))
            'Else
            '    Draw2DLine(Drawing.Color.Red.ToArgb, 2, PosB(0), PosB(1), PosB(0), PosB(1))
            'End If
            If Equal(PosA(2), 0) Then 'TODO ToEdit
                Draw2DLine(Drawing.Color.Green.ToArgb, 1, PosA(0), PosA(1), PosA(0), PosA(1))
            End If
            If Equal(PosB(2), 0) Then
                Draw2DLine(Drawing.Color.Green.ToArgb, 1, PosB(0), PosB(1), PosB(0), PosB(1))
            End If
        End If
    End Sub
    Public Sub DrawRegion(ByVal Region As Region)
        Dim TempPos, TempPos2 As New Collections.Generic.List(Of Vector)

        For n As Integer = 0 To Region.PosArray.GetUpperBound(0)
            TempPos.Add(Region.PosArray(n))
        Next

        If TempPos.Count = 0 Then Return
        Dim LastPos As Vector = TempPos(TempPos.Count - 1)
        Dim Pos As Vector
        For n As Integer = 0 To TempPos.Count - 1
            Pos = TempPos(n)
            If LastPos(2) > 0 AndAlso Pos(2) > 0 Then
                TempPos2.Add(Pos)
                LastPos = Pos
                Continue For
            Else
                Dim Dir As Vector = Pos - LastPos
                Dim CurrentPos As Vector
                Dim LowerBound As Double = 0
                Dim UpperBound As Double = Dir.Norm
                Dir = Dir.Dir

                '对Z轴裁边，去掉<0的部分
                If Dir(2) = 0 Then
                    If Not (Pos(2) > 0) Then
                        LastPos = Pos
                        Continue For
                    End If
                Else
                    Dim zl As Double = -LastPos(2) / Dir(2)
                    If Dir(2) > 0 Then
                        If UpperBound < zl Then
                            LastPos = Pos
                            Continue For
                        End If
                        If LowerBound < zl Then
                            LowerBound = zl
                            CurrentPos = LastPos + LowerBound * Dir
                            TempPos2.Add(CurrentPos)
                            TempPos2.Add(Pos)
                            LastPos = Pos
                            Continue For
                        End If
                    ElseIf Dir(2) < 0 Then
                        If LowerBound > zl Then
                            LastPos = Pos
                            Continue For
                        End If
                        If UpperBound > zl Then
                            UpperBound = zl
                            CurrentPos = LastPos + UpperBound * Dir
                            TempPos2.Add(CurrentPos)
                            LastPos = Pos
                            Continue For
                        End If
                    End If
                    LastPos = Pos
                    Continue For
                End If
            End If
        Next

        Dim TempPos3 As Vector() = New Vector(TempPos2.Count - 1) {}

        For n As Integer = 0 To TempPos2.Count - 1
            TempPos3(n) = CPer(TempPos2(n))
        Next
        Dim SumZ As Double
        Try
            For n As Integer = 0 To Region.PosArray.GetUpperBound(0)
                SumZ += Region.PosArray(n)(2)
            Next
            SumZ /= Region.PosArray.GetLength(0)
        Catch
            SumZ = Double.NaN
        End Try
        SumZ = SumZ ^ 2 * 1024
        If SumZ = Double.NaN Then Return
        If SumZ < 0 Then Return
        If SumZ > 3 * 255 Then SumZ = 3 * 255

        Dim ColorValue As Color = Region.ColorInt.ToColor
        Dim LightDegree As Double = SumZ / (CInt(ColorValue.R) + CInt(ColorValue.G) + CInt(ColorValue.B))
        If Not (LightDegree < 1 AndAlso LightDegree > 0) Then LightDegree = 1
        ColorValue = Drawing.Color.FromArgb(ColorValue.R * LightDegree, ColorValue.G * LightDegree, ColorValue.B * LightDegree)

        If TempPos3 Is Nothing Then Return
        If TempPos3.GetUpperBound(0) < 2 Then Return
        If AreaTriangle2D(TempPos3(0)(0), TempPos3(0)(1), TempPos3(1)(0), TempPos3(1)(1), TempPos3(2)(0), TempPos3(2)(1)) > 0 Then Return '去掉顺时针的Region
        Draw2DRegion(ColorValue.ToArgb, TempPos3)
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
            If xBoundL < Left Then xBoundL = Left
            If xBoundR > Right Then xBoundR = Right

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
                    Draw2DLine(ColorInt, 1, xApo, (1 - n) * yBoundT + n * yBoundB, xApo, (1 - n) * yBoundT + n * yBoundB)
                Next
            Next
        End With
    End Sub

    Public Sub Draw2DLine(ByVal Color As ColorInt32, ByVal PenWidth As Single, ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single) Implements IPic.Draw2DLine
        Dim Pen As New Pen(Color.ToColor, PenWidth)
        Try
            If (x1 = x2 AndAlso y1 = y2) Then
                g.DrawPie(Pen, CSng(x1 + SemiWidth - Pen.Width / 2), CSng(y1 + SemiHeight - Pen.Width / 2), Pen.Width, Pen.Width, 0, 360)
            Else
                g.DrawLine(Pen, CSng(x1 + SemiWidth), CSng(y1 + SemiHeight), CSng(x2 + SemiWidth), CSng(y2 + SemiHeight))
            End If
        Catch
        End Try
        Pen.Dispose()
    End Sub
    Public Sub Draw2DRegion(ByVal Color As ColorInt32, ByVal Points As Vector()) Implements IPic.Draw2DRegion
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            Dim TempPoints(Points.GetUpperBound(0)) As Drawing.PointF
            For n As Integer = 0 To Points.GetUpperBound(0)
                TempPoints(n).X = Points(n)(0) + SemiWidth
                TempPoints(n).Y = Points(n)(1) + SemiHeight
            Next
            g.FillPolygon(Brush, TempPoints)
        Catch
        End Try
        Brush.Dispose()
    End Sub
    Public Sub Draw2DPie(ByVal Color As ColorInt32, ByVal Pos As Vector, ByVal r As Double)
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            g.FillPie(Brush, CSng(Pos(0) - r + SemiWidth), CSng(Pos(1) - r + SemiHeight), CSng(2 * r), CSng(2 * r), 0, 360)
        Catch
        End Try
        Brush.Dispose()
    End Sub
    Public Sub Draw2DString(ByVal s As String, ByVal Font As System.Drawing.Font, ByVal Color As ColorInt32, ByVal x As Single, ByVal y As Single) Implements IPic.Draw2DString
        Dim Brush As New SolidBrush(Color.ToColor)
        Try
            g.DrawString(s, Font, Brush, x, y)
        Catch
        End Try
        Brush.Dispose()
    End Sub

    Public Function CPer(ByVal Pos As Vector) As Vector
        '将多维坐标(x,y,z,...)转换为三维坐标(x*i/z,y*i/z,1/z) 其中i为ImageDistance
        'X轴正方向水平向右 Y轴正方向竖直向下 Z轴正方向向屏内

        '以下公式备用
        '入射光介质折射率/物距+出射光介质折射率/像距=入射光介质折射率/物方焦距=出射光介质折射率/像方焦距
        ' n1 / u  +  n2 / v = n1 / f1 = n2 / f2

        Dim x As Double = Pos(0)
        Dim y As Double = Pos(1)
        Dim z As Double = Pos(2)
        '像大/物大=像距/物距
        x *= ImageDistanceValue / z
        y *= ImageDistanceValue / z
        Return New Vector(x, y, z)
    End Function
    Public Function UnCPer(ByVal Pos As Vector) As Vector
        '将三维坐标转换为三维坐标(CPer的逆运算)

        Dim x As Double = Pos(0)
        Dim y As Double = Pos(1)
        Dim z As Double = Pos(2)
        '像大/物大=像距/物距
        x /= ImageDistanceValue / z
        y /= ImageDistanceValue / z
        Return New Vector(x, y, z)
    End Function

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
