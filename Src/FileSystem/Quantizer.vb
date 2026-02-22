'==========================================================================
'
'  File:        Quantizer.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 量化器
'  Version:     2020.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Numerics
Imports Firefly

Public Class Quantizer(Of T)
    Public Shared Function Execute(ByVal Colors As T(), ByVal NumPaletteColor As Integer, ByVal MeanFactory As Func(Of Tuple(Of Action(Of T), Func(Of T, T))), ByVal Distance As Func(Of T, T, Double), Optional ByVal NumSample As Integer? = Nothing, Optional ByVal Quality As Double = 0.99) As KeyValuePair(Of T(), Integer())
        Dim QuantizeOnPalette =
            Function(ByVal Color As T, ByVal Palette As T()) As Integer
                If Palette.Length = 0 Then Throw New ArgumentException()
                Dim Index = 0
                Dim d = Double.MaxValue
                For i = 0 To Palette.Length - 1
                    Dim cd = Distance(Color, Palette(i))
                    If cd < d Then
                        d = cd
                        Index = i
                    End If
                Next
                Return Index
            End Function

        Dim Samples = Colors
        If NumSample IsNot Nothing Then
            If NumSample.Value = 0 OrElse NumSample.Value > Colors.Length Then Throw New ArgumentException()
            Samples = New T(NumSample.Value - 1) {}
            Dim Ratio = Colors.Length / Samples.Length
            For i = 0 To Samples.Length - 1
                Samples(i) = Colors(Convert.ToInt32(Math.Floor(i * Ratio)))
            Next
        End If

        'K-means
        If NumPaletteColor <= 0 Then Throw New ArgumentException()
        If Samples.Length <= NumPaletteColor Then Return New KeyValuePair(Of T(), Integer())(DirectCast(Samples.Clone(), T()), Enumerable.Range(0, Samples.Length).ToArray())
        Dim k = NumPaletteColor
        Dim ColorClusterIndex = New Integer(Samples.Length - 1) {}
        With Nothing
            Dim Cache = New Dictionary(Of T, Integer)
            Dim j = 0
            For i = 0 To Samples.Length - 1
                If Cache.ContainsKey(Samples(i)) Then
                    ColorClusterIndex(i) = Cache(Samples(i))
                Else
                    ColorClusterIndex(i) = j
                    Cache.Add(Samples(i), j)
                    j = (j + 1) Mod k
                End If
            Next
        End With
        Dim NewColorClusterIndex = New Integer(Samples.Length - 1) {}
        Dim MeanFunctors = New Tuple(Of Action(Of T), Func(Of T, T))(k - 1) {}
        Dim Means = New T(k - 1) {}
        Dim n = Environment.ProcessorCount
        While True
            For i = 0 To k - 1
                MeanFunctors(i) = MeanFactory()
            Next
            For i = 0 To Samples.Length - 1
                MeanFunctors(ColorClusterIndex(i)).Item1()(Samples(i))
            Next
            For i = 0 To k - 1
                Means(i) = MeanFunctors(i).Item2()(Means(i))
            Next
            Dim PartitionLength = (Samples.Length + n - 1) \ n
            ParallelEnumerable.Range(0, n).ForAll(
                Sub(i)
                    Dim Lower = i * PartitionLength
                    Dim Upper = Math.Min(Lower + PartitionLength, Samples.Length) - 1
                    Dim Cache = New Dictionary(Of T, Integer)
                    For i = Lower To Upper
                        If Cache.ContainsKey(Samples(i)) Then
                            NewColorClusterIndex(i) = Cache(Samples(i))
                        Else
                            NewColorClusterIndex(i) = QuantizeOnPalette(Samples(i), Means)
                            Cache.Add(Samples(i), NewColorClusterIndex(i))
                        End If
                    Next
                End Sub
            )
            Dim SamePercent = Enumerable.Range(0, Samples.Length).Where(Function(i) NewColorClusterIndex(i) = ColorClusterIndex(i)).Count() / Samples.Length
            If SamePercent > Quality Then Exit While
            Array.Copy(NewColorClusterIndex, ColorClusterIndex, Samples.Length)
        End While
        If NumSample Is Nothing Then
            Return New KeyValuePair(Of T(), Integer())(Means, NewColorClusterIndex)
        Else
            Dim Indices = New Integer(Colors.Length - 1) {}
            Dim PartitionLength = (Colors.Length + n - 1) \ n
            ParallelEnumerable.Range(0, n).ForAll(
                Sub(i)
                    Dim Lower = i * PartitionLength
                    Dim Upper = Math.Min(Lower + PartitionLength, Colors.Length) - 1
                    Dim Cache = New Dictionary(Of T, Integer)
                    For i = Lower To Upper
                        If Cache.ContainsKey(Colors(i)) Then
                            Indices(i) = Cache(Colors(i))
                        Else
                            Indices(i) = QuantizeOnPalette(Colors(i), Means)
                            Cache.Add(Colors(i), Indices(i))
                        End If
                    Next
                End Sub
            )
            Return New KeyValuePair(Of T(), Integer())(Means, Indices)
        End If
    End Function
End Class

Public Class QuantizerARGB
    Public Shared Function Execute(ByVal Colors As Int32(), ByVal NumPaletteColor As Integer, Optional ByVal Quality As Double = 0.99) As KeyValuePair(Of Int32(), Integer())
        Dim MapToVector = Function(c As Int32) New Vector4(c.Bits(31, 24), c.Bits(23, 16), c.Bits(15, 8), c.Bits(7, 0))
        Dim MapFromVector =
            Function(v As Vector4)
                Dim A = If(v.X <= 0, 0, If(v.X >= 255, 255, Convert.ToInt32(v.X)))
                Dim R = If(v.Y <= 0, 0, If(v.Y >= 255, 255, Convert.ToInt32(v.Y)))
                Dim G = If(v.Z <= 0, 0, If(v.Z >= 255, 255, Convert.ToInt32(v.Z)))
                Dim B = If(v.W <= 0, 0, If(v.W >= 255, 255, Convert.ToInt32(v.W)))
                Return A.ConcatBits(R, 8).ConcatBits(G, 8).ConcatBits(B, 8)
            End Function
        Dim MeanFactory =
            Function()
                Dim x As New Vector4(0, 0, 0, 0)
                Dim k = 0
                Dim Sum As Action(Of Vector4) =
                    Sub(v)
                        x += v
                        k += 1
                    End Sub
                Dim Mean As Func(Of Vector4, Vector4) =
                    Function(InitialValue As Vector4) As Vector4
                        If k = 0 Then Return InitialValue
                        Return x / k
                    End Function
                Return Tuple.Create(Sum, Mean)
            End Function
        Dim Sqr = Function(v As Double) v * v
        Dim Distance = Function(a As Vector4, b As Vector4) (a - b).Length
        Dim Result = Quantizer(Of Vector4).Execute(Colors.Select(MapToVector).ToArray(), NumPaletteColor, MeanFactory, Distance, Math.Min(Colors.Length, 256 * 256), Quality)
        Return New KeyValuePair(Of Int32(), Integer())(Result.Key.Select(MapFromVector).ToArray(), Result.Value)
    End Function
    Public Shared Function Execute(ByVal Colors As Int32(,), ByVal NumPaletteColor As Integer, Optional ByVal Quality As Double = 0.99) As KeyValuePair(Of Int32(), Integer(,))
        Dim c = New Int32(Colors.GetLength(0) * Colors.GetLength(1) - 1) {}
        Buffer.BlockCopy(Colors, 0, c, 0, Colors.GetLength(0) * Colors.GetLength(1) * 4)
        Dim Result = Execute(c, NumPaletteColor, Quality)
        Dim Indices = New Integer(Colors.GetLength(0) - 1, Colors.GetLength(1) - 1) {}
        Buffer.BlockCopy(Result.Value, 0, Indices, 0, Colors.GetLength(0) * Colors.GetLength(1) * 4)
        Return New KeyValuePair(Of Integer(), Integer(,))(Result.Key, Indices)
    End Function
End Class

Public Class QuantizerGray
    Public Shared Function Execute(ByVal Colors As Byte(), ByVal NumPaletteColor As Integer, Optional ByVal Quality As Double = 0.99) As KeyValuePair(Of Byte(), Integer())
        Dim MapToDouble = Function(c As Byte) CDbl(c)
        Dim MapFromDouble =
            Function(v As Double)
                If v <= 0 Then Return CByte(0)
                If v >= 255 Then Return CByte(255)
                Return CByte(v)
            End Function
        Dim MeanFactory =
            Function()
                Dim x As Double = 0
                Dim k = 0
                Dim Sum As Action(Of Double) =
                    Sub(v)
                        x += v
                        k += 1
                    End Sub
                Dim Mean As Func(Of Double, Double) =
                    Function(InitialValue As Double) As Double
                        If k = 0 Then Return InitialValue
                        Return CDbl(CByte(x / k))
                    End Function
                Return Tuple.Create(Sum, Mean)
            End Function
        Dim Sqr = Function(v As Double) v * v
        Dim Distance = Function(a As Double, b As Double) Math.Abs(a - b)
        Dim Result = Quantizer(Of Double).Execute(Colors.Select(MapToDouble).ToArray(), NumPaletteColor, MeanFactory, Distance, Math.Min(Colors.Length, 256 * 256), Quality)
        Return New KeyValuePair(Of Byte(), Integer())(Result.Key.Select(MapFromDouble).ToArray(), Result.Value)
    End Function
End Class
