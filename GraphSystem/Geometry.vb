'==========================================================================
'
'  File:        Geometry.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: NDSystem基础设施
'  Created:     2004.07.05.00:23:34(GMT+8:00)
'  Version:     0.5 2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Text
Imports System.Diagnostics
Imports Microsoft.VisualBasic

#Region " 向量 "
''' <summary>向量</summary>
''' <remarks>坐标有关维数无关向量 用于向量计算</remarks>
<DebuggerStepThrough(), DebuggerDisplay("Vector{ToString()}")> Public Structure Vector
    ''' <summary>坐标</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Private Pos As Double()

    ''' <summary>转换成字符串时的精度</summary>
    Public Shared Decimaldigits As Byte = 4

    Public Sub New(ByVal Dimension As Byte)
        ReDim Pos(Dimension - 1)
    End Sub
    Public Sub New(ByVal PosDouble As Double())
        If PosDouble Is Nothing Then Return
        Pos = PosDouble.Clone
    End Sub
    Public Sub New(ByVal x As Double, ByVal y As Double, ByVal ParamArray PosDouble As Double())
        ReDim Pos(PosDouble.GetUpperBound(0) + 2)
        Pos(0) = x
        Pos(1) = y
        For n As Integer = 0 To PosDouble.GetUpperBound(0)
            Pos(n + 2) = PosDouble(n)
        Next
    End Sub
    ''' <summary>向量的坐标</summary>
    Default Public Property Value(ByVal i As Byte) As Double
        <DebuggerHidden()> Get
            If i > UpperBound Then Return 0
            Return Pos(i)
        End Get
        Set(ByVal PosDouble As Double)
            If i > UpperBound Then ReDim Preserve Pos(i)
            Pos(i) = PosDouble
        End Set
    End Property
    ''' <summary>向量的前i个坐标</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public ReadOnly Property Part(ByVal Length As Byte) As Vector
        Get
            Dim Vector As New Vector(Length)
            For n As Integer = 0 To Length - 1
                Vector(n) = Value(n)
            Next
            Return Vector
        End Get
    End Property
    ''' <summary>向量的下标上限</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public ReadOnly Property UpperBound() As Integer
        Get
            If Pos Is Nothing Then Return -1
            Return Pos.GetUpperBound(0)
        End Get
    End Property
    ''' <summary>向量的维数</summary>
    Public Property Dimension() As Byte
        Get
            If Pos Is Nothing Then Return 0
            Return Pos.GetLength(0)
        End Get
        Set(ByVal Value As Byte)
            ReDim Preserve Pos(Value - 1)
        End Set
    End Property

    '向量运算函数

    ''' <summary>向量的坐标转换成数组</summary>
    Public Shared Widening Operator CType(ByVal Vector As Vector) As Double()
        If Vector.Pos Is Nothing Then Return Nothing
        Return Vector.Pos.Clone
    End Operator
    ''' <summary>压缩向量的坐标</summary>
    ''' <remark>去掉后面多余的0</remark>
    Public Sub Optimize()
        Dim CurrentUpper As Integer = Pos.GetUpperBound(0)
        While CurrentUpper > -1
            If Pos(CurrentUpper) = 0 Then
                CurrentUpper -= 1
            Else
                Exit While
            End If
        End While
        ReDim Preserve Pos(CurrentUpper)
    End Sub
    ''' <summary>判断是否为空</summary>
    Public Function IsNothing() As Boolean
        Return Pos Is Nothing
    End Function
    ''' <summary>近似相等</summary>
    ''' <param name="Vector">预与其比较的向量</param><param name="Exp">精确的位的指数</param>
    ''' <returns>是否近似相等</returns>
    Public Function EqualsTo(ByVal Vector As Vector, Optional ByVal Exp As Double = -5) As Boolean
        For n As Integer = 0 To Max(UpperBound, Vector.UpperBound)
            If Math.Abs(Me(n) - Vector(n)) > 10 ^ Exp Then Return False
        Next
        Return True
    End Function
    ''' <summary>向量加法</summary>
    Public Shared Operator +(ByVal Left As Vector, ByVal Right As Vector) As Vector
        Dim Pos As Vector
        If Left.Dimension > Right.Dimension Then
            Pos = +Left
        Else
            Pos = +Right
        End If
        For n As Integer = 0 To Min(Left.UpperBound, Right.UpperBound)
            Pos(n) = Left(n) + Right(n)
        Next
        Return Pos
    End Operator
    ''' <summary>向量减法</summary>
    Public Shared Operator -(ByVal Left As Vector, ByVal Right As Vector) As Vector
        Dim Pos As Vector
        If Left.Dimension > Right.Dimension Then
            Pos = +Left
        Else
            Pos = -Right
        End If
        For n As Integer = 0 To Min(Left.UpperBound, Right.UpperBound)
            Pos(n) = Left(n) - Right(n)
        Next
        Return Pos
    End Operator
    ''' <summary>向量取正</summary>
    ''' <remarks>可用于复制一份向量</remarks> 
    Public Shared Operator +(ByVal Right As Vector) As Vector
        Return New Vector(Right.Pos)
    End Operator
    ''' <summary>向量取负</summary>
    Public Shared Operator -(ByVal Right As Vector) As Vector
        Dim Pos As Vector = +Right
        For n As Integer = 0 To Pos.UpperBound
            Pos(n) = -Pos(n)
        Next
        Return Pos
    End Operator
    ''' <summary>向量数乘</summary>
    Public Shared Operator *(ByVal Left As Double, ByVal Right As Vector) As Vector
        Dim Pos As New Vector(Right.Dimension)
        For n As Integer = 0 To Right.UpperBound
            Pos(n) = Left * Right(n)
        Next
        Return Pos
    End Operator
    ''' <summary>向量数乘</summary>
    Public Shared Operator *(ByVal Left As Vector, ByVal Right As Double) As Vector
        Dim Pos As New Vector(Left.Dimension)
        For n As Integer = 0 To Left.UpperBound
            Pos(n) = Left(n) * Right
        Next
        Return Pos
    End Operator
    ''' <summary>向量数量积</summary>
    Public Shared Operator *(ByVal Left As Vector, ByVal Right As Vector) As Double
        Dim Value As Double
        For n As Integer = 0 To Min(Left.UpperBound, Right.UpperBound)
            Value += Left(n) * Right(n)
        Next
        Return Value
    End Operator
    ''' <summary>向量向量积</summary>
    Public Shared Function VecPro(ByVal IsRightHanded As Boolean, ByVal ParamArray Vectors As Vector()) As Vector
        If Vectors Is Nothing Then Return Nothing
        Dim D As Integer = Vectors.GetLength(0) + 1
        Dim Det As Double(,) = New Double(D - 2, D - 2) {}
        Dim TempVector As New Vector()
        For n As Integer = 0 To D - 1
            For m As Integer = 0 To D - 2
                For l As Integer = 0 To n - 1
                    Det(m, l) = Vectors(m)(l)
                Next
                For l As Integer = n + 1 To D - 1
                    Det(m, l - 1) = Vectors(m)(l)
                Next
            Next
            If n Mod 2 = 0 Then
                TempVector(n) += (New Matrix(Det)).Determinant
            Else
                TempVector(n) -= (New Matrix(Det)).Determinant
            End If
        Next
        If IsRightHanded Then
            Return TempVector
        Else
            Return -TempVector
        End If
    End Function
    ''' <summary>向量模(Euclidean范数)</summary>
    Public Function Norm() As Double
        Dim Value As Double
        For n As Integer = 0 To UpperBound
            Value += Me(n) ^ 2
        Next
        Return Value ^ 0.5
    End Function
    ''' <summary>向量平方和</summary>
    Public Function Sqr() As Double
        Dim Value As Double
        For n As Integer = 0 To UpperBound
            Value += Me(n) ^ 2
        Next
        Return Value
    End Function
    ''' <summary>向量的方向向量</summary>
    Public Function Dir() As Vector
        Dim AbsValue As Double = Norm()
        If AbsValue = 0 Then Return New Vector(New Double() {1})
        If Double.IsInfinity(AbsValue) Then Return New Vector(New Double() {1})
        Dim Direction As New Vector(Dimension)
        For n As Integer = 0 To UpperBound
            Direction(n) = Me(n) / AbsValue
        Next
        Return Direction
    End Function
    ''' <summary>格式化字符串</summary>
    Public Overrides Function ToString() As String
        Dim FormatString As New StringBuilder
        For n As Integer = 0 To Decimaldigits - 1
            FormatString.Append("0")
        Next
        Return ToString(String.Format(" 0.{0};-0.{0}", FormatString.ToString)) 'Default:" 0.0000;-0.0000"
    End Function
    ''' <summary>格式化字符串</summary>
    ''' <param name="Spliter">分隔号</param>
    ''' <param name="Format">格式字符串</param>
    Public Overloads Function ToString(ByVal Format As String, Optional ByVal Spliter As Char = " ") As String
        If UpperBound = -1 Then Return "()"
        Dim ThisString As New StringBuilder
        ThisString.Append("(")
        For n As Integer = 0 To UpperBound - 1
            ThisString.Append(Value(n).ToString(Format))
            ThisString.Append(Spliter)
        Next
        ThisString.Append(Value(UpperBound).ToString(Format))
        ThisString.Append(")")
        Return ThisString.ToString
    End Function
    ''' <summary>字符串初始化</summary>
    ''' <param name="Spliters">分隔号</param>
    Public Shared Function FromString(ByVal VectorString As String, Optional ByVal Spliters As String = " ,") As Vector
        If VectorString Is Nothing Then Return Nothing
        Dim VectorValue As String = VectorString
        Dim VectorStart, VectorEnd As Integer
        VectorStart = VectorValue.IndexOf("(")
        If VectorStart >= 0 Then
            VectorStart += 1
            VectorEnd = VectorValue.IndexOf(")", VectorStart) - 1
        Else
            VectorStart = 0
            VectorEnd = VectorValue.Length - 1
        End If
        VectorValue = VectorValue.Substring(VectorStart, VectorEnd - VectorStart + 1).Trim
        Dim OldLength As Integer = VectorValue.Length
        Do
            VectorValue = VectorValue.Replace("  ", " ")
            If OldLength = VectorValue.Length Then Exit Do
            OldLength = VectorValue.Length
        Loop
        Do
            VectorValue = VectorValue.Replace(", ", ",")
            If OldLength = VectorValue.Length Then Exit Do
            OldLength = VectorValue.Length
        Loop
        If VectorValue = "" Then Return Nothing
        Dim Value() As String = VectorValue.Split(Spliters.ToCharArray)
        Dim ReturnValue As Vector = New Vector(Value.GetLength(0))
        For n As Integer = 0 To Value.GetUpperBound(0)
            ReturnValue(n) = CDbl(Value(n))
        Next
        Return ReturnValue
    End Function
    ''' <summary>转化为字符串</summary>
    Public Shared Widening Operator CType(ByVal ThisVector As Vector) As String
        Return ThisVector.ToString
    End Operator
    ''' <summary>从字符串转化</summary>
    Public Shared Widening Operator CType(ByVal ThisString As String) As Vector
        Return FromString(ThisString)
    End Operator
End Structure
#End Region

#Region " 矩阵 "
''' <summary>矩阵 Matrix</summary>
''' <remarks>维数无关方阵 维度0为纵向改变 维度1为横向改变</remarks> 
<DebuggerStepThrough(), DebuggerDisplay("{ToString()}")> Public Class Matrix
    ''' <summary>矩阵元素</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Protected ElementValue(,) As Double

#Region " 属性 "
    ''' <summary>阶数</summary>
    Public ReadOnly Property Dimension() As Byte
        Get
            If ElementValue Is Nothing Then Return 0
            Return ElementValue.GetLength(0)
        End Get
    End Property
    ''' <summary>秩</summary>
    Public Function Rank() As Byte
        Dim Length As Integer = Me.Dimension
        Dim r As Integer = Length
        Dim Sign As Boolean
        If ElementValue Is Nothing Then Return Nothing
        Dim Elements As Double(,) = ElementValue.Clone
        For j As Integer = 0 To Length - 1
            Sign = False
            For i As Integer = j To Length - 1
                If Not Equal(Elements(i, j), 0, -6) Then
                    If i <> j Then
                        For n As Integer = 0 To Length - 1
                            Exchange(Elements(i, n), Elements(j, n))
                        Next
                    End If
                    Dim Temp As Double
                    For k As Integer = j + 1 To Length - 1
                        Temp = -Elements(k, j) / Elements(j, j)
                        For n As Integer = 0 To Length - 1
                            Elements(k, n) += Temp * Elements(j, n)
                        Next
                    Next
                    Sign = True
                    Exit For
                End If
            Next
            If Not Sign Then r -= 1
        Next
        Return r
    End Function
    ''' <summary>元素</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public ReadOnly Property Element() As Double(,)
        Get
            Return ElementValue.Clone
        End Get
    End Property
    ''' <summary>元素</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Element(ByVal i As Byte, ByVal j As Byte) As Double
        Get
            Return ElementValue(i, j)
        End Get
        Set(ByVal Value As Double)
            ElementValue(i, j) = Value
        End Set
    End Property
    ''' <summary>行向量</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Row(ByVal Index As Byte) As Vector
        Get
            If Index > Dimension() - 1 Then Return Nothing
            Dim ReturnVector As New Vector(Dimension)
            For n As Integer = 0 To Dimension() - 1
                ReturnVector(n) = Element(Index, n)
            Next
            Return ReturnVector
        End Get
        Set(ByVal Value As Vector)
            If Index > Dimension() - 1 Then Throw New IndexOutOfRangeException
            If Value.Dimension > Dimension() Then Throw New IndexOutOfRangeException
            For n As Integer = 0 To Dimension() - 1
                ElementValue(Index, n) = Value(n)
            Next
        End Set
    End Property
    ''' <summary>列向量</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Column(ByVal Index As Byte) As Vector
        Get
            If Index > Dimension() - 1 Then Return Nothing
            Dim ReturnVector As New Vector(Dimension)
            For n As Integer = 0 To Dimension() - 1
                ReturnVector(n) = Element(n, Index)
            Next
            Return ReturnVector
        End Get
        Set(ByVal Value As Vector)
            If Index > Dimension() - 1 Then Throw New IndexOutOfRangeException
            If Value.Dimension > Dimension() Then Throw New IndexOutOfRangeException
            For n As Integer = 0 To Dimension() - 1
                ElementValue(n, Index) = Value(n)
            Next
        End Set
    End Property
    ''' <summary>行列式</summary>
    Public Function Determinant() As Double
        If ElementValue Is Nothing Then Return 0
        Dim Elements As Double(,) = ElementValue.Clone
        Dim Length As Integer = Me.Dimension
        If Length <= 0 Then Return 0
        If Length = 1 Then Return Elements(0, 0)
        If Length = 2 Then Return Elements(0, 0) * Elements(1, 1) - Elements(0, 1) * Elements(1, 0)
        Dim Value As Double = 1
        Dim Sign As Boolean
        For j As Integer = 0 To Length - 1
            Sign = False
            For i As Integer = j To Length - 1
                If Elements(i, j) <> 0 Then
                    If i <> j Then
                        For n As Integer = 0 To Length - 1
                            Exchange(Elements(i, n), Elements(j, n))
                        Next
                    End If
                    Dim Temp As Double
                    Value *= sgn(Elements(j, j))
                    For k As Integer = j + 1 To Length - 1
                        Temp = -Elements(k, j) / Elements(j, j)
                        For n As Integer = 0 To Length - 1
                            Elements(k, n) += Temp * Elements(j, n)
                        Next
                    Next
                    Sign = True
                    Exit For
                End If
            Next
            If Not Sign Then Return 0
        Next
        For i As Integer = 0 To Length - 1
            Value *= Elements(i, i)
        Next
        Return Value
    End Function
    ''' <summary>转置矩阵</summary>
    Public Function Transpose() As Matrix
        Dim Dimension As Integer = Me.Dimension
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For i As Integer = 0 To Dimension - 1
            For j As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = ElementValue(j, i)
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Function
    ''' <summary>逆矩阵</summary>
    ''' <remarks>若为奇异矩阵将抛出ArithmeticException</remarks>
    Public Function Invert() As Matrix
        Dim Length As Integer = Me.Dimension
        Dim Sign As Boolean
        If ElementValue Is Nothing Then Return Nothing
        Dim ElementOld As Double(,) = ElementValue.Clone
        Dim ElementTemp As Double(,) = New Double(Length - 1, Length - 1) {}
        For n As Integer = 0 To Length - 1
            ElementTemp(n, n) = 1
        Next
        For j As Integer = 0 To Length - 1
            Sign = False
            For i As Integer = j To Length - 1
                If ElementOld(i, j) <> 0 Then
                    If i <> j Then
                        For n As Integer = 0 To Length - 1
                            Exchange(ElementTemp(i, n), ElementTemp(j, n))
                            Exchange(ElementOld(i, n), ElementOld(j, n))
                        Next
                    End If
                    Dim Temp As Double = ElementOld(j, j)
                    For n As Integer = 0 To Length - 1
                        ElementTemp(j, n) /= Temp
                        ElementOld(j, n) /= Temp
                    Next
                    For k As Integer = 0 To j - 1
                        Temp = -ElementOld(k, j)
                        For n As Integer = 0 To Length - 1
                            ElementTemp(k, n) += Temp * ElementTemp(j, n)
                            ElementOld(k, n) += Temp * ElementOld(j, n)
                        Next
                    Next
                    For k As Integer = j + 1 To Length - 1
                        Temp = -ElementOld(k, j)
                        For n As Integer = 0 To Length - 1
                            ElementTemp(k, n) += Temp * ElementTemp(j, n)
                            ElementOld(k, n) += Temp * ElementOld(j, n)
                        Next
                    Next
                    Sign = True
                    Exit For
                End If
            Next
            If Not Sign Then Throw New ArithmeticException
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Function
#End Region

#Region " 方法 "
    ''' <summary>初始化函数</summary>
    Private Sub New()
    End Sub
    ''' <summary>初始化函数</summary>
    Public Sub New(ByVal Dimension As Byte)
        ReDim ElementValue(Dimension - 1, Dimension - 1)
    End Sub
    ''' <summary>初始化函数</summary>
    Public Sub New(ByVal Element As Double(,))
        If Not Element Is Nothing AndAlso Element.GetUpperBound(0) <> Element.GetUpperBound(1) Then Throw New ArithmeticException
        ElementValue = Element
    End Sub
    ''' <summary>矩阵加法</summary>
    Public Shared Operator +(ByVal Left As Matrix, ByVal Right As Matrix) As Matrix
        If Left Is Nothing Then Return +Right
        If Right Is Nothing Then Return +Left
        Dim Dimension As Integer = Max(Left.Dimension, Right.Dimension)
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For i As Integer = 0 To Dimension - 1
            For j As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = Left.Element(i, j) + Right.Element(i, j)
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Operator
    ''' <summary>矩阵减法</summary>
    Public Shared Operator -(ByVal Left As Matrix, ByVal Right As Matrix) As Matrix
        If Left Is Nothing Then Return -Right
        If Right Is Nothing Then Return +Left
        Dim Dimension As Integer = Max(Left.Dimension, Right.Dimension)
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For i As Integer = 0 To Dimension - 1
            For j As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = Left.Element(i, j) - Right.Element(i, j)
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Operator
    ''' <summary>矩阵数乘</summary>
    Public Shared Operator *(ByVal Left As Double, ByVal Right As Matrix) As Matrix
        If Right Is Nothing Then Return Nothing
        Dim Dimension As Integer = Right.Dimension
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For i As Integer = 0 To Dimension - 1
            For j As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = Left * Right.Element(i, j)
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Operator
    ''' <summary>矩阵数乘</summary>
    Public Shared Operator *(ByVal Left As Matrix, ByVal Right As Double) As Matrix
        If Left Is Nothing Then Return Nothing
        Dim Dimension As Integer = Left.Dimension
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For i As Integer = 0 To Dimension - 1
            For j As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = Left.Element(i, j) * Right
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Operator
    ''' <summary>矩阵取正</summary>
    ''' <remarks>可用于复制一份矩阵</remarks> 
    Public Shared Operator +(ByVal Right As Matrix) As Matrix
        If Right Is Nothing Then Return Nothing
        Return Right.MemberwiseClone
    End Operator
    ''' <summary>矩阵取负</summary>
    Public Shared Operator -(ByVal Right As Matrix) As Matrix
        If Right Is Nothing Then Return Nothing
        Return Right * (-1)
    End Operator
    ''' <summary>向量与矩阵的乘法</summary>
    ''' <remarks>向量看作行向量</remarks>
    Public Shared Operator *(ByVal Left As Vector, ByVal Right As Matrix) As Vector
        If Right Is Nothing Then Return +Left
        Dim PosNew As New Vector(Left)
        For j As Integer = 0 To Right.Dimension - 1
            PosNew(j) = 0
            For i As Integer = 0 To Right.Dimension - 1
                PosNew(j) += Left(i) * Right.ElementValue(i, j)
            Next
        Next
        Return PosNew
    End Operator
    ''' <summary>矩阵与向量的乘法</summary>
    ''' <remarks>向量看作列向量</remarks>
    Public Shared Operator *(ByVal Left As Matrix, ByVal Right As Vector) As Vector
        If Left Is Nothing Then Return +Right
        Dim PosNew As New Vector(Right)
        For i As Integer = 0 To Left.Dimension - 1
            PosNew(i) = 0
            For j As Integer = 0 To Left.Dimension - 1
                PosNew(i) += Left.ElementValue(i, j) * Right(j)
            Next
        Next
        Return PosNew
    End Operator
    ''' <summary>矩阵乘法</summary>
    Public Shared Operator *(ByVal Left As Matrix, ByVal Right As Matrix) As Matrix
        If Left Is Nothing Then Return +Right
        If Right Is Nothing Then Return +Left
        Dim Length As Integer = Max(Left.Dimension, Right.Dimension)
        Dim ElementTemp As Double(,) = New Double(Length - 1, Length - 1) {}
        For j As Integer = 0 To Length - 1
            For i As Integer = 0 To Length - 1
                ElementTemp(i, j) = 0
                For n As Integer = 0 To Length - 1
                    ElementTemp(i, j) += Left.Element(i, n) * Right.Element(n, j)
                Next
            Next
        Next
        Dim MatTemp As New Matrix
        MatTemp.ElementValue = ElementTemp
        Return MatTemp
    End Operator
    ''' <summary>横行取反</summary>
    Public Sub NegRow(ByVal i As Byte)
        If i > Dimension() - 1 Then Throw New IndexOutOfRangeException
        For n As Integer = 0 To Dimension() - 1
            ElementValue(i, n) = -ElementValue(i, n)
        Next
    End Sub
    ''' <summary>纵列取反</summary>
    Public Sub NegColumn(ByVal j As Byte)
        If j > Dimension() - 1 Then Throw New IndexOutOfRangeException
        For n As Integer = 0 To Dimension() - 1
            ElementValue(n, j) = -ElementValue(n, j)
        Next
    End Sub
    ''' <summary>格式化字符串</summary>
    Public Overrides Function ToString() As String
        Dim FormatString As New StringBuilder
        For n As Integer = 0 To Vector.Decimaldigits - 1
            FormatString.Append("0")
        Next
        Return ToString(String.Format(" 0.{0};-0.{0}", FormatString.ToString)) 'Default:" 0.0000;-0.0000"
    End Function
    ''' <summary>格式化字符串</summary>
    ''' <param name="Format">格式字符串</param>
    Public Overloads Function ToString(ByVal Format As String, Optional ByVal Spliter As String = " ") As String
        Dim ThisString As New StringBuilder
        If Dimension() = 0 Then Return "Matrix(" & Environment.NewLine & ")"

        For i As Integer = 0 To Dimension() - 2
            For j As Integer = 0 To Dimension() - 2
                ThisString.Append(Element(i, j).ToString(Format) & Spliter)
            Next
            ThisString.Append(Element(i, Dimension() - 1).ToString(Format) & Environment.NewLine)
        Next
        For j As Integer = 0 To Dimension() - 2
            ThisString.Append(Element(Dimension() - 1, j).ToString(Format) & Spliter)
        Next
        ThisString.Append(Element(Dimension() - 1, Dimension() - 1).ToString(Format))

        Return "Matrix(" & Environment.NewLine & AddSpaceForEachLine(ThisString.ToString) & Environment.NewLine & ")"
    End Function
    ''' <summary>字符串初始化</summary>
    ''' <remarks>
    ''' 格式样例为:
    ''' Matrix (
    '''      1  0
    '''      0  1
    ''' )
    ''' Matrix里的回车可用";"代替
    ''' 分隔符可混合使用" "或"," 或自定义Spliters
    ''' 空格可恰当使用以增强可读性
    ''' </remarks>
    Public Shared Function FromString(ByVal MatrixValue As String) As Matrix
        If MatrixValue Is Nothing Then Return Nothing
        Dim Mat As String() = GetMultiLineFieldFromString(MatrixValue, "Matrix", False, True).Replace(Environment.NewLine, ";").Split(";")
        Dim MatList As New List(Of String)
        For Each MatRow As String In Mat
            If MatRow = "" Then
            Else
                MatList.Add(MatRow)
            End If
        Next
        Mat = MatList.ToArray
        Dim Matrix As New Matrix(Mat.GetLength(0))
        For n As Integer = 0 To Mat.GetUpperBound(0)
            Matrix.Row(n) = Vector.FromString(Mat(n))
        Next
        Return Matrix
    End Function
    ''' <summary>转化为字符串</summary>
    Public Shared Widening Operator CType(ByVal ThisMat As Matrix) As String
        If ThisMat Is Nothing Then Return Nothing
        Return ThisMat.ToString
    End Operator
    ''' <summary>从字符串转化</summary>
    Public Shared Widening Operator CType(ByVal ThisString As String) As Matrix
        Return FromString(ThisString)
    End Operator
#End Region

End Class
#End Region

#Region " 变换 "
''' <summary>变换</summary>
Public Interface ITransformation(Of Domain, Range)
    Function CPos(ByVal Pos As Domain) As Range
End Interface

''' <summary>位似变换</summary>
''' <remarks>维数无关坐标变换 所有角数据单位为弧度(Radian)</remarks>
<DebuggerStepThrough(), DebuggerDisplay("{ToString()}")> Public Class HomotheticTransformation
    Implements ITransformation(Of Vector, Vector)

    ''' <summary>正交矩阵元素</summary>
    Protected MatrixValue(,) As Double

    ''' <summary>基准点 坐标原点</summary>
    ''' <remarks>Reference Position</remarks>
    Public RefPos As Vector

    ''' <summary>放大倍数的以2为底的对数</summary>
    Public Scaler As Double

#Region " 属性 "
    ''' <summary>维数</summary>
    Public Property Dimension() As Byte
        Get
            If MatrixValue Is Nothing Then Return 0
            Return MatrixValue.GetLength(0)
        End Get
        Set(ByVal DimensionOut As Byte)
            Dim Dimension As Byte = Me.Dimension
            If DimensionOut = Dimension Then Return
            If DimensionOut > Dimension AndAlso Dimension >= 1 Then
                Dim ElementTemp(DimensionOut - 1, DimensionOut - 1) As Double
                For i As Integer = 0 To Dimension - 1
                    For j As Integer = 0 To Dimension - 1
                        ElementTemp(i, j) = MatrixValue(i, j)
                    Next
                Next
                For n As Integer = Dimension To DimensionOut - 1
                    ElementTemp(n, n) = 1
                Next
                MatrixValue = ElementTemp
            ElseIf DimensionOut >= 0 Then
                ReDim MatrixValue(DimensionOut - 1, DimensionOut - 1)
                If Not (DimensionOut = 0) Then
                    For n As Integer = 0 To DimensionOut - 1
                        MatrixValue(n, n) = 1
                    Next
                End If
            End If
        End Set
    End Property
    ''' <summary>坐标轴数据矩阵</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public ReadOnly Property Matrix() As Matrix
        Get
            Return New Matrix(Me.MatrixValue)
        End Get
    End Property
    ''' <summary>坐标轴数据</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Matrix(ByVal i As Byte, ByVal j As Byte) As Double
        Get
            If Not MatrixValue Is Nothing Then
                If i < MatrixValue.GetLength(0) AndAlso j < MatrixValue.GetLength(0) Then
                    Return MatrixValue(i, j)
                End If
            End If
            If i = j Then Return 1
            Return 0
        End Get
        Set(ByVal Value As Double)
            If i > Dimension - 1 OrElse j > Dimension - 1 Then Me.Dimension = Max(i, j) + 1
            MatrixValue(i, j) = Value
        End Set
    End Property
    ''' <summary>本坐标系相对于基坐标系的手性改变</summary>
    Public ReadOnly Property IsChiralityChanged() As Boolean
        Get
            If MatrixValue Is Nothing Then Return False
            If Matrix.Determinant >= 0 Then Return False
            Return True
        End Get
    End Property
    ''' <summary>行向量</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Row(ByVal Index As Byte) As Vector
        Get
            If Index > Dimension - 1 Then Return Nothing
            Dim ReturnVector As New Vector(Dimension)
            For n As Integer = 0 To Dimension - 1
                ReturnVector(n) = Matrix(Index, n)
            Next
            Return ReturnVector
        End Get
        Set(ByVal Value As Vector)
            If Index > Dimension - 1 Then Dimension = Index + 1
            If Value.Dimension > Dimension Then Dimension = Value.Dimension
            For n As Integer = 0 To Dimension - 1
                MatrixValue(Index, n) = Value(n)
            Next
        End Set
    End Property
    ''' <summary>列向量</summary>
    <DebuggerBrowsable(DebuggerBrowsableState.Never)> Public Property Column(ByVal Index As Byte) As Vector
        Get
            If Index > Dimension - 1 Then Return Nothing
            Dim ReturnVector As New Vector(Dimension)
            For n As Integer = 0 To Dimension - 1
                ReturnVector(n) = Matrix(n, Index)
            Next
            Return ReturnVector
        End Get
        Set(ByVal Value As Vector)
            If Index > Dimension - 1 Then Dimension = Index + 1
            If Value.Dimension > Dimension Then Dimension = Value.Dimension
            For n As Integer = 0 To Dimension - 1
                MatrixValue(n, Index) = Value(n)
            Next
        End Set
    End Property
    ''' <summary>逆变换</summary>
    Public Function Invert() As HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Dim M As Matrix = New Matrix(MatrixValue).Transpose
        Temp.MatrixValue = M.Element
        Temp.RefPos = M * (-RefPos)
        Temp.Scaler = -Scaler
        Return Temp
    End Function
#End Region

#Region " 方法 "
    ''' <summary>初始化函数</summary>
    Public Sub New()
    End Sub
    ''' <summary>初始化函数</summary>
    Public Sub New(ByVal Dimension As Byte)
        ReDim MatrixValue(Dimension - 1, Dimension - 1)
        For n As Integer = 0 To Dimension - 1
            MatrixValue(n, n) = 1
        Next
        RefPos = New Vector(Dimension)
    End Sub
    ''' <summary>初始化函数</summary>
    Public Sub New(ByVal MatrixOut As Matrix, ByVal RefPosOut As Vector, Optional ByVal ScalerOut As Double = 0)
        If Not MatrixOut Is Nothing Then MatrixValue = MatrixOut.Element
        RefPos = RefPosOut
        Scaler = ScalerOut
    End Sub
    ''' <summary>坐标系存在性检测代码</summary>
    Public Function Check() As Boolean
        If MatrixValue Is Nothing Then Return True
        Dim SumSqrA, SumSqrB, SumSqrC As Double
        For a As Integer = 0 To MatrixValue.GetUpperBound(0)
            For b As Integer = 0 To MatrixValue.GetUpperBound(1)
                SumSqrA += MatrixValue(a, b) ^ 2
                SumSqrB += MatrixValue(b, a) ^ 2
                SumSqrC += MatrixValue(a, b) * MatrixValue((a + 1) Mod Dimension, b)
            Next
            If Not (Equal(SumSqrA, 1) AndAlso Equal(SumSqrB, 1) AndAlso Equal(SumSqrC, 0)) Then
                Return False
            End If
            SumSqrA = 0
            SumSqrB = 0
            SumSqrC = 0
        Next
        Return True
    End Function
    ''' <summary>变换与向量的乘法</summary>
    Public Overloads Shared Operator *(ByVal Left As HomotheticTransformation, ByVal Right As Vector) As Vector
        If Left Is Nothing Then Return +Right
        Return Left.Matrix * (Right * 2 ^ Left.Scaler)
    End Operator
    ''' <summary>坐标转换</summary>
    ''' <param name="Pos">欲转换的坐标向量</param>
    ''' <returns>坐标向量</returns>
    Public Function CPos(ByVal Pos As Vector) As Vector Implements ITransformation(Of Vector, Vector).CPos
        Return Me * Pos + RefPos
    End Function
    ''' <summary>坐标逆转换</summary>
    ''' <param name="Pos">欲逆转换的坐标向量</param>
    ''' <returns>坐标向量</returns>
    Public Function UnCPos(ByVal Pos As Vector) As Vector
        Return Me.Matrix.Transpose * (Pos - RefPos)
    End Function
    ''' <summary>坐标系的乘法</summary>
    ''' <param name="Left">乘上的坐标系</param><param name="Right">基坐标系</param>
    ''' <returns>新坐标系</returns>
    Public Overloads Shared Operator *(ByVal Left As HomotheticTransformation, ByVal Right As HomotheticTransformation) As HomotheticTransformation
        If Left Is Nothing Then Return +Right
        If Right Is Nothing Then Return +Left
        Dim Dimension As Integer = Max(Left.Dimension, Right.Dimension)
        Dim ElementTemp As Double(,) = New Double(Dimension - 1, Dimension - 1) {}
        For j As Integer = 0 To Dimension - 1
            For i As Integer = 0 To Dimension - 1
                ElementTemp(i, j) = 0
                For n As Integer = 0 To Dimension - 1
                    ElementTemp(i, j) += Left.Matrix(i, n) * Right.Matrix(n, j)
                Next
            Next
        Next
        Dim CoordTemp As New HomotheticTransformation
        CoordTemp.MatrixValue = ElementTemp
        CoordTemp.RefPos = Left * Right.RefPos + Left.RefPos
        CoordTemp.Scaler = Left.Scaler + Right.Scaler
        Return CoordTemp
    End Operator
    ''' <summary>坐标系取正</summary>
    ''' <remarks>可用于复制一份坐标系</remarks> 
    Public Shared Shadows Operator +(ByVal Right As HomotheticTransformation) As HomotheticTransformation
        If Right Is Nothing Then Return Nothing
        Dim Clone As HomotheticTransformation = Right.MemberwiseClone
        If Clone Is Nothing Then Return Nothing
        Clone.RefPos = +Right.RefPos
        Return Clone
    End Operator
    ''' <summary>坐标系的乘法</summary>
    ''' <param name="CoordMix">乘上的坐标系</param>
    Public Sub Mix(ByVal CoordMix As HomotheticTransformation)
        Dim CoordTemp As HomotheticTransformation = CoordMix * Me
        If CoordTemp Is Nothing Then Return
        MatrixValue = CoordTemp.MatrixValue
        RefPos = CoordTemp.RefPos
        Scaler = CoordTemp.Scaler
    End Sub
    ''' <summary>坐标系的除法</summary>
    ''' <param name="CoordMix">除去的坐标系</param>
    Public Sub UnMix(ByVal CoordMix As HomotheticTransformation)
        Dim CoordTemp As HomotheticTransformation = CoordMix.Invert * Me
        If CoordTemp Is Nothing Then Return
        MatrixValue = CoordTemp.MatrixValue
        RefPos = CoordTemp.RefPos
        Scaler = CoordTemp.Scaler
    End Sub
    ''' <summary>旋转</summary>
    ''' <param name="i">旋转的轴1</param>
    ''' <param name="j">旋转的轴2</param>
    ''' <param name="delta">旋转角</param>
    ''' <returns>是否成功</returns>
    ''' <remarks>
    ''' Row(i)向着Column(j)旋转
    ''' Leg(i,)是Row(i)在Base中的方向向量坐标
    ''' Leg(,i)是Column(i)在Real中的方向向量坐标
    ''' </remarks>
    Public Function Rotate(ByVal i As Byte, ByVal j As Byte, ByVal delta As Double) As Boolean
        If i = j Then Return False
        If delta = 0 Then Return True
        If i > Dimension - 1 OrElse j > Dimension - 1 Then Dimension = Max(i, j) + 1

        'R = [cos(delta),-sin(delta);sin(delta),cos(delta)]
        'R * (1,0) = (cos(theta),sin(theta))
        'R * (0,1) = (-sin(theta),cos(theta))
        Dim CoordMix As New HomotheticTransformation
        CoordMix.Matrix(i, i) = Cos(delta)
        CoordMix.Matrix(i, j) = -Sin(delta)
        CoordMix.Matrix(j, i) = Sin(delta)
        CoordMix.Matrix(j, j) = Cos(delta)
        MatrixValue = (CoordMix * Me).MatrixValue
        Return True
    End Function
    ''' <summary>格式化字符串</summary>
    Public Shadows Function ToString() As String
        Return ToString(False)
    End Function
    ''' <summary>格式化字符串</summary>
    Public Shadows Function ToString(ByVal Details As Boolean) As String
        Dim FormatString As New StringBuilder
        For n As Integer = 0 To Vector.Decimaldigits - 1
            FormatString.Append("0")
        Next
        Return ToString(Details, String.Format(" 0.{0};-0.{0}", FormatString.ToString)) 'Default:" 0.0000;-0.0000"
    End Function
    ''' <summary>格式化字符串</summary>
    ''' <param name="Format">格式字符串</param>
    Public Shadows Function ToString(ByVal Details As Boolean, ByVal Format As String, Optional ByVal Spliter As String = " ") As String
        Dim ThisString As String = "." & Matrix.ToString
        ThisString &= Environment.NewLine & SingleLineFieldString(".RefPos", RefPos.ToString(Format, Spliter))
        ThisString &= Environment.NewLine & SingleLineFieldString(".Scaler", Scaler)
        If Details Then
            ThisString &= Environment.NewLine & SingleLineFieldString(".Check", Check())
        End If
        ThisString = MultiLineFieldString("HomotheticTransformation", ThisString, False)
        Return ThisString
    End Function
    ''' <summary>字符串初始化</summary>
    ''' <remarks>
    ''' 格式样例为:
    ''' HomotheticTransformation{
    '''     .Matrix(
    '''          1  0
    '''          0  1
    '''     )
    '''     .RefPos( 0  0)
    '''     .Scaler 2
    ''' }
    ''' 其中Element里的回车可用";"代替
    ''' 分隔符可混合使用" "或"," 或自定义Spliters
    ''' 空格可恰当使用以增强可读性
    ''' </remarks>
    Public Shared Shadows Function FromString(ByVal HomotheticTransformationString As String) As HomotheticTransformation
        If HomotheticTransformationString Is Nothing Then Return Nothing
        Dim CoordSystem As New HomotheticTransformation
        CoordSystem.RefPos = Vector.FromString(GetMultiLineFieldFromString(HomotheticTransformationString, ".RefPos", False, True))
        CoordSystem.Scaler = GetDoubleFromString(HomotheticTransformationString, ".Scaler")
        Dim Mat As Matrix = GraphSystem.Matrix.FromString(GetMultiLineFieldFromString(HomotheticTransformationString, ".Matrix", True, True))
        If Mat Is Nothing Then Return CoordSystem
        CoordSystem.MatrixValue = Mat.Element
        If Not CoordSystem.Check Then CoordSystem.MatrixValue = Nothing
        Return CoordSystem
    End Function
    ''' <summary>转化为字符串</summary>
    Public Shared Shadows Widening Operator CType(ByVal ThisCoord As HomotheticTransformation) As String
        If ThisCoord Is Nothing Then Return Nothing
        Return ThisCoord.ToString
    End Operator
    ''' <summary>从字符串转化</summary>
    Public Shared Shadows Widening Operator CType(ByVal ThisString As String) As HomotheticTransformation
        Return FromString(ThisString)
    End Operator
#End Region

End Class
#End Region

#Region " 基础函数库类库 "

<DebuggerStepThrough()> Public Module BasicFunctions

    '角度三角函数
    Public Function CRad(ByVal Expression As Double) As Double
        Return Expression * PI / 180
    End Function
    Public Function CDeg(ByVal Expression As Double) As Double
        Return Expression * 180 / PI
    End Function
    Public Function sinDeg(ByVal Expression As Double) As Double
        sinDeg = Math.Sin(CRad(Expression))
    End Function
    Public Function cosDeg(ByVal Expression As Double) As Double
        cosDeg = Math.Cos(CRad(Expression))
    End Function
    Public Function tanDeg(ByVal Expression As Double) As Double
        tanDeg = Math.Tan(CRad(Expression))
    End Function
    Public Function asinDeg(ByVal Expression As Double) As Double
        asinDeg = CDeg(Math.Asin(Expression))
    End Function
    Public Function acosDeg(ByVal Expression As Double) As Double
        acosDeg = CDeg(Math.Acos(Expression))
    End Function
    Public Function atanDeg(ByVal Expression As Double) As Double
        atanDeg = CDeg(Math.Atan(Expression))
    End Function


    ''' <summary>三角形面积</summary>
    Public Function AreaTriangle(ByVal PosA As Vector, ByVal PosB As Vector, ByVal PosC As Vector) As Double
        Dim a As Vector = PosA - PosC
        Dim b As Vector = PosB - PosC
        Return 0.5 * Sqrt(a.Sqr * b.Sqr - (a * b) ^ 2) 'S=0.5*|a|*|b|*sin<a,b>
    End Function
    ''' <summary>平面上的三角形面积 顶点逆时针为负 顺时针为正</summary>
    Public Function AreaTriangle2D(ByVal PosAx As Double, ByVal PosAy As Double, ByVal PosBx As Double, ByVal PosBy As Double, ByVal PosCx As Double, ByVal PosCy As Double) As Double
        Dim Temp(1, 1) As Double
        Temp(0, 0) = PosBx - PosAx
        Temp(0, 1) = PosBy - PosAy
        Temp(1, 0) = PosCx - PosAx
        Temp(1, 1) = PosCy - PosAy
        Return (New Matrix(Temp)).Determinant * 0.5D
    End Function
    Public Function IsOnLine(ByVal PosArray As Vector()) As Boolean
        If PosArray Is Nothing Then Return True
        If PosArray.GetUpperBound(0) < 2 Then Return True

        Dim PosA As Vector = PosArray(0)
        Dim IndexB As Integer = 1
        Dim PosB As Vector = PosArray(1)

        While PosA.EqualsTo(PosB)
            If IndexB + 2 > PosArray.GetUpperBound(0) Then Return True
            IndexB += 1
            PosB = PosArray(IndexB)
        End While

        Dim IndexP As Integer = IndexB + 1
        Dim PosP As Vector
        While Not (IndexP > PosArray.GetUpperBound(0))
            PosP = PosArray(IndexP)
            While PosA.EqualsTo(PosP) OrElse PosB.EqualsTo(PosP)
                If IndexP + 1 > PosArray.GetUpperBound(0) Then Return True
                IndexP += 1
                PosP = PosArray(IndexP)
            End While
            If Not Equal(AreaTriangle(PosA, PosB, PosP), 0) Then Return False
            IndexP += 1
        End While
        Return True
    End Function
    ''' <summary>是否在三角形ABC内或上或线段ABC上</summary>
    Public Function IsInTriangle(ByVal PosP As Vector, ByVal PosA As Vector, ByVal PosB As Vector, ByVal PosC As Vector) As Boolean
        If IsOnLine(New Vector() {PosA, PosB, PosC}) Then
            If (PosP - PosA).Norm + (PosP - PosB).Norm + (PosP - PosC).Norm > 2 * Max((PosA - PosB).Norm, Max((PosB - PosC).Norm, (PosC - PosA).Norm)) Then Return False
            Return True
        Else
            Dim s As Double = AreaTriangle(PosA, PosB, PosC)
            Dim s2 As Double = AreaTriangle(PosP, PosB, PosC) + AreaTriangle(PosA, PosP, PosC) + AreaTriangle(PosA, PosB, PosP)
            If Equal(s, 0) AndAlso Equal(s2, 0) Then Return True
            Dim k As Double
            If Equal(s, 0) Then
                k = s / s2
            Else
                k = s2 / s
            End If
            If Equal(k, 1) Then Return True
            Return False
        End If
    End Function
    ''' <summary>判断点是否在凸多边形内</summary>
    Public Function IsInConvexRegion(ByVal P As Vector, ByVal PosArray As Vector()) As Boolean
        If PosArray.GetLength(0) < 3 Then Return False
        Dim A As Vector = PosArray(0)
        Dim B As Vector
        Dim C As Vector = PosArray(1)
        For i As Integer = 2 To PosArray.GetUpperBound(0)
            B = C
            C = PosArray(i)
            If IsInTriangle(P, A, B, C) Then Return True
        Next
        Return False
    End Function


    ''' <summary>取正负号运算</summary>
    Public Function sgn(ByVal Expression As Double) As Integer
        Select Case Expression
            Case Is > 0
                Return 1
            Case Is < 0
                Return -1
            Case 0
                Return 0
            Case Else
                Throw New ArgumentException
        End Select
    End Function
    ''' <summary>正的模数运算</summary>
    Public Function MathMod(ByVal Datium As Integer, ByVal Moder As Integer) As Integer
        Dim Result = Datium Mod Moder
        If Result < 0 Xor Moder < 0 Then Result += Moder
        Return Result
    End Function
    ''' <summary>正的模数运算</summary>
    Public Function MathMod(ByVal Datium As Double, ByVal Moder As Double) As Double
        Dim Result = Datium Mod Moder
        If Result < 0 Xor Moder < 0 Then Result += Moder
        Return Result
    End Function
    ''' <summary>近似相等</summary>
    ''' <param name="Exp">精确的位的指数</param>
    ''' <returns>是否近似相等</returns>
    Public Function Equal(ByVal A As Double, ByVal B As Double, Optional ByVal Exp As Double = -4) As Boolean
        If Double.IsNaN(A) OrElse Double.IsNaN(B) Then Return False
        If Double.IsPositiveInfinity(A) AndAlso Double.IsPositiveInfinity(B) Then Return True
        If Double.IsNegativeInfinity(A) AndAlso Double.IsNegativeInfinity(B) Then Return True
        If Double.IsInfinity(A) OrElse Double.IsInfinity(B) Then Return False
        If Abs(A - B) > 10 ^ Exp Then Return False
        Return True
    End Function
    ''' <summary>近似相等</summary>
    ''' <param name="Exp">精确的位的指数</param>
    ''' <returns>是否近似相等</returns>
    Public Function Equal(ByVal A As Vector, ByVal B As Vector, Optional ByVal Exp As Double = -4) As Boolean
        For n As Integer = 0 To Max(A.Dimension, B.Dimension) - 1
            If Not Equal(A(n), B(n), Exp) Then Return False
        Next
        Return True
    End Function
    ''' <summary>带保护除法</summary>
    ''' <remarks>因为构架的除法在除以变量中的0时有BUG</remarks>
    Public Function DIV(ByVal Numerator As Double, ByVal Denominator As Double) As Double
        If Denominator = 0 Then
            Select Case Numerator
                Case Is < 0
                    Return Double.NegativeInfinity
                Case Is > 0
                    Return Double.PositiveInfinity
                Case Else
                    Return Double.NaN
            End Select
        ElseIf Double.IsInfinity(Denominator) Then
            If Double.IsInfinity(Numerator) Then Return Double.NaN
            Return 0
        End If
        Return Numerator / Denominator
    End Function

    Public Function Max(Of T As IComparable)(ByVal a As T, ByVal b As T) As T
        If Not (a Is Nothing) Then
            If a.CompareTo(b) >= 0 Then Return a
        End If
        Return b
    End Function
    Public Function Min(Of T As IComparable)(ByVal a As T, ByVal b As T) As T
        If Not (a Is Nothing) Then
            If a.CompareTo(b) >= 0 Then Return b
        End If
        Return a
    End Function
    Public Function Max(Of T As IComparable)(ByVal a As T, ByVal ParamArray b As T()) As T
        Dim ret As T = a
        For Each x As T In b
            If Not (x Is Nothing) Then
                If x.CompareTo(ret) >= 0 Then ret = x
            End If
        Next
        Return ret
    End Function
    Public Function Min(Of T As IComparable)(ByVal a As T, ByVal ParamArray b As T()) As T
        Dim ret As T = a
        For Each x As T In b
            If Not (ret Is Nothing) Then
                If ret.CompareTo(x) >= 0 Then ret = x
            End If
        Next
        Return ret
    End Function
    Public Function Exchange(Of T)(ByRef a As T, ByRef b As T) As T
        Dim Temp As T
        Temp = a
        a = b
        b = Temp
    End Function

    Public Function AddSpaceForEachLine(ByVal Source As String, Optional ByVal SpaceNumber As Byte = 4) As String
        If Source Is Nothing Then Return Nothing
        If SpaceNumber = 0 Then Return Source

        Dim SpacesBuilder As New StringBuilder
        For n As Integer = 0 To SpaceNumber - 1
            SpacesBuilder.Append(" ")
        Next
        Dim Spaces As String = SpacesBuilder.ToString

        Dim Returner As New StringBuilder
        Dim LastIndex As Integer
        Dim Index As Integer = Source.IndexOf(Environment.NewLine)
        Dim CurrentLine As String

        While Index >= 0
            CurrentLine = Source.Substring(LastIndex, Index - LastIndex)
            If CurrentLine.Length = 0 Then
                Returner.AppendLine()
            Else
                Returner.Append(Spaces)
                Returner.AppendLine(CurrentLine)
            End If
            LastIndex = Index + Environment.NewLine.Length
            Index = Source.IndexOf(Environment.NewLine, LastIndex)
        End While
        Returner.Append(Spaces)
        Returner.Append(Source.Substring(LastIndex))
        Return Returner.ToString
    End Function
    Public Function SingleLineFieldString(ByVal Name As String, ByVal Value As String) As String
        Return Name & " " & Value
    End Function
    Public Function MultiLineFieldString(ByVal Name As String, ByVal Value As String, Optional ByVal UseVectorStyle As Boolean = False) As String
        If UseVectorStyle Then Return Name & "(" & Environment.NewLine & AddSpaceForEachLine(Value) & Environment.NewLine & ")"
        Return Name & "{" & Environment.NewLine & AddSpaceForEachLine(Value) & Environment.NewLine & "}"
    End Function
    Public Function GetSingleLineFieldFromString(ByVal Source As String, ByVal Name As String) As String
        If Source Is Nothing OrElse Name Is Nothing Then Return Nothing
        Dim FieldStart, FieldEnd As Integer
        FieldStart = Source.IndexOf(Name)
        If FieldStart < 0 Then Return Nothing
        FieldStart += Name.Length + 1
        FieldEnd = Source.IndexOf(Environment.NewLine, FieldStart) - 1
        If FieldEnd < 0 Then FieldEnd = Source.IndexOf(" ", FieldStart) - 1
        If FieldEnd < 0 Then FieldEnd = Source.Length - 1
        If FieldEnd < 0 Then Return Nothing
        Return Source.Substring(FieldStart, FieldEnd - FieldStart + 1)
    End Function
    Public Function GetDoubleFromString(ByVal Source As String, ByVal Name As String) As Double
        Dim SimpleTypeString As String = GetSingleLineFieldFromString(Source, Name)
        If SimpleTypeString Is Nothing Then Return Nothing
        Return CDbl(SimpleTypeString)
    End Function
    Public Function GetBooleanFromString(ByVal Source As String, ByVal Name As String) As Boolean
        Dim SimpleTypeString As String = GetSingleLineFieldFromString(Source, Name)
        If SimpleTypeString Is Nothing Then Return False
        Return CBool(SimpleTypeString)
    End Function
    Public Function GetMultiLineFieldFromString(ByVal Source As String, ByVal Name As String, Optional ByVal IsWithCurelyBrackets As Boolean = True, Optional ByVal UseVectorStyle As Boolean = False) As String
        If Source Is Nothing OrElse Name Is Nothing Then Return Nothing
        Dim FieldStart, FieldEnd As Integer
        FieldStart = Source.IndexOf(Name)
        If FieldStart < 0 Then Return Nothing
        Dim LeftBracket As Char
        Dim RightBracket As Char
        If UseVectorStyle Then
            LeftBracket = "("
            RightBracket = ")"
        Else
            LeftBracket = "{"
            RightBracket = "}"
        End If
        If IsWithCurelyBrackets Then
            FieldEnd = Source.IndexOf(LeftBracket, FieldStart) + 1
            If FieldEnd < 1 Then Return Nothing

            Dim BracketCount As Integer = 1
            For Each Character As Char In Source.ToCharArray(FieldEnd, Source.Length - 1 - FieldEnd + 1)
                If Character = LeftBracket Then
                    BracketCount += 1
                ElseIf Character = RightBracket Then
                    BracketCount -= 1
                    If BracketCount = 0 Then
                        Return Source.Substring(FieldStart, FieldEnd - FieldStart + 1)
                    End If
                End If
                FieldEnd += 1
            Next
            Return Nothing
        Else
            FieldStart += Name.Length
            FieldStart = Source.IndexOf(LeftBracket, FieldStart) + 1
            If FieldStart < 0 Then Return Nothing
            FieldEnd = FieldStart

            Dim BracketCount As Integer = 1
            For Each Character As Char In Source.ToCharArray(FieldEnd, Source.Length - 1 - FieldEnd + 1)
                If Character = LeftBracket Then
                    BracketCount += 1
                ElseIf Character = RightBracket Then
                    BracketCount -= 1
                    If BracketCount = 0 Then
                        Return Source.Substring(FieldStart, FieldEnd - 1 - FieldStart + 1)
                    End If
                End If
                FieldEnd += 1
            Next
            Return Nothing
        End If
    End Function
    Public Function GetArrayFieldFromString(ByVal Source As String, ByVal Name As String, Optional ByVal IsWithCurelyBrackets As Boolean = True, Optional ByVal ElementsUseVectorStyle As Boolean = False) As String()
        Dim Returner As New List(Of String)

        Dim TempSource As String = GetMultiLineFieldFromString(Source, Name, False)
        If TempSource Is Nothing OrElse Name Is Nothing Then Return Nothing
        Dim FieldStart, FieldEnd As Integer
        Dim LeftBracket As Char
        Dim RightBracket As Char
        If ElementsUseVectorStyle Then
            LeftBracket = "("
            RightBracket = ")"
        Else
            LeftBracket = "{"
            RightBracket = "}"
        End If
        If IsWithCurelyBrackets Then
            While True
                Dim LastFieldEnd As Integer = FieldEnd
                FieldStart = TempSource.IndexOf(LeftBracket, LastFieldEnd + 1)
                FieldEnd = FieldStart + 1
                If FieldStart < 0 Then Exit While
                While FieldStart > LastFieldEnd
                    If TempSource(FieldStart - 1) = " " OrElse TempSource(FieldStart - 1) = ChrW(10) Then Exit While
                    FieldStart -= 1
                End While

                Dim BracketCount As Integer = 1
                For Each Character As Char In TempSource.ToCharArray(FieldEnd, TempSource.Length - 1 - FieldEnd + 1)
                    If Character = LeftBracket Then
                        BracketCount += 1
                    ElseIf Character = RightBracket Then
                        BracketCount -= 1
                        If BracketCount = 0 Then
                            Returner.Add(TempSource.Substring(FieldStart, FieldEnd - FieldStart + 1))
                            Continue While
                        End If
                    End If
                    FieldEnd += 1
                Next
                Exit While
            End While
        Else
            While True
                FieldStart = TempSource.IndexOf(LeftBracket, FieldEnd + 1) + 1
                If FieldStart < 1 Then Exit While
                FieldEnd = FieldStart

                Dim BracketCount As Integer = 1
                For Each Character As Char In TempSource.ToCharArray(FieldEnd, TempSource.Length - 1 - FieldEnd + 1)
                    If Character = LeftBracket Then
                        BracketCount += 1
                    ElseIf Character = RightBracket Then
                        BracketCount -= 1
                        If BracketCount = 0 Then
                            Returner.Add(TempSource.Substring(FieldStart, FieldEnd - 1 - FieldStart + 1))
                            Continue While
                        End If
                    End If
                    FieldEnd += 1
                Next
                Exit While
            End While
        End If
        Return Returner.ToArray
    End Function
    Public Function GetRoundString(ByVal d As Double, ByVal Digits As Integer) As String
        Dim s As String = d.ToString("g" & Digits)
        Dim n As Integer = s.Length
        If s.StartsWith("-") Then n -= 1
        If s.Contains(".") Then n -= 1
        If n >= Digits Then Return s
        If Not s.Contains(".") Then s = s & "."
        s = s & New String("0", Digits - n)
        Return s
    End Function

End Module
#End Region

#Region " 物体基础 "
''' <summary>物体基类</summary>
<DebuggerStepThrough()> Public MustInherit Class NDObject
    Public Name As String
    Public HomotheticTransformation As HomotheticTransformation '坐标系变换
    Public ColorInt As ColorInt32 '颜色 最后调用Color.Argb(Integer)或其他方法得到颜色 忽略溢出
    ''' <summary>空构造函数</summary>
    Protected Sub New()
    End Sub
    ''' <summary>构造函数</summary>
    Protected Sub New(ByVal Color As ColorInt32, ByVal NameValue As String)
        ColorInt = Color
        Name = NameValue
    End Sub
    ''' <summary>字符串构造函数</summary>
    ''' <remarks>格式样例见UpdateFromString</remarks>
    Protected Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub

    ''' <summary>旋转</summary>
    ''' <param name="i">旋转的轴1</param>
    ''' <param name="j">旋转的轴2</param>
    ''' <param name="delta">旋转角</param>
    ''' <returns>是否成功</returns>
    ''' <remarks>
    ''' Row(i)向着Column(j)旋转
    ''' Leg(i,)是Row(i)在Base中的方向向量坐标
    ''' Leg(,i)是Column(i)在Real中的方向向量坐标
    ''' </remarks>
    Public Function Rotate(ByVal i As Byte, ByVal j As Byte, ByVal delta As Double) As Boolean
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Return HomotheticTransformation.Rotate(i, j, delta)
    End Function

    ''' <summary>创建物体的浅表副本</summary>
    Public Overridable Function Clone() As NDObject
        Return MemberwiseClone()
    End Function
    ''' <summary>创建物体的深层副本</summary>
    Public MustOverride Function Copy() As NDObject
    Protected Function BaseCopy() As NDObject
        Dim CopyObject As NDObject = MemberwiseClone()
        CopyObject.HomotheticTransformation = +HomotheticTransformation
        Return CopyObject
    End Function
    ''' <summary>转换为可制图物体</summary>
    Public MustOverride Function Complete() As IPicObj()

    ''' <summary>格式化字符串</summary>
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(SingleLineFieldString(".Name", Name))
        If HomotheticTransformation Is Nothing Then
            ThisString.AppendLine("." & (New HomotheticTransformation).ToString(True))
        Else
            ThisString.AppendLine("." & HomotheticTransformation.ToString(True))
        End If
        ThisString.Append(SingleLineFieldString(".Color", ColorInt.ToString))
        Return MultiLineFieldString("NDObject", ThisString.ToString)
    End Function
    ''' <summary>从字符串更新</summary>
    ''' <remarks>
    ''' 格式样例为:
    ''' NDObject{
    '''     .Name SampleObject
    '''     .HomotheticTransformation{
    '''         .Matrix(
    '''              1  0
    '''              0  1
    '''         )
    '''         .RefPos( 0  0)
    '''         .Scaler 2
    '''     }
    '''     .Color #FFFFFFFF
    ''' }
    ''' 分隔符可混合使用" "或"," 或自定义Spliters
    ''' 空格可恰当使用以增强可读性
    ''' </remarks>
    Public Overridable Sub UpdateFromString(ByVal ThisString As String)
        HomotheticTransformation = GraphSystem.HomotheticTransformation.FromString(GetMultiLineFieldFromString(ThisString, ".HomotheticTransformation"))
        Name = GetSingleLineFieldFromString(ThisString, ".Name")
        ColorInt = ColorInt32.FromString(GetSingleLineFieldFromString(ThisString, ".Color"))
    End Sub

    ''' <summary>转化为字符串</summary>
    Public Shared Widening Operator CType(ByVal ThisObj As NDObject) As String
        If ThisObj Is Nothing Then Return Nothing
        Return ThisObj.ToString
    End Operator
End Class
''' <summary>绘图物体接口</summary>
Public Interface IPicObj
End Interface
''' <summary>32位颜色</summary>
<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)> Public Structure ColorInt32
    <System.Runtime.InteropServices.FieldOffset(3)> Public A As Byte
    <System.Runtime.InteropServices.FieldOffset(2)> Public R As Byte
    <System.Runtime.InteropServices.FieldOffset(1)> Public G As Byte
    <System.Runtime.InteropServices.FieldOffset(0)> Public B As Byte
    <System.Runtime.InteropServices.FieldOffset(0)> Public ARGB As Int32
    Public Sub New(ByVal Int32 As Integer)
        ARGB = Int32
    End Sub
    Public Sub New(ByVal A As Byte, ByVal R As Byte, ByVal G As Byte, ByVal B As Byte)
        With Me
            .A = A
            .R = R
            .G = G
            .B = B
        End With
    End Sub
    Public Overrides Function ToString() As String
        Return "#" & ARGB.ToString("X8")
    End Function
    Public Shared Function FromString(ByVal ThisString As String) As ColorInt32
        If ThisString Is Nothing Then Return 0
        Return New ColorInt32(CInt(ThisString.Substring(1)))
    End Function
    Public Shared Widening Operator CType(ByVal ColorInt32 As ColorInt32) As Integer
        Return ColorInt32.ARGB
    End Operator
    Public Function ToColor() As Drawing.Color
        Return Drawing.Color.FromArgb(ARGB)
    End Function
    Public Shared Widening Operator CType(ByVal Int32 As Integer) As ColorInt32
        Return New ColorInt32(Int32)
    End Operator
    Public Shared Widening Operator CType(ByVal Color As ColorInt32) As Drawing.Color
        Return Drawing.Color.FromArgb(Color)
    End Operator
    Public Shared Widening Operator CType(ByVal Color As Drawing.Color) As ColorInt32
        Return Color.ToArgb
    End Operator
End Structure
#End Region
