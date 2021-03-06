'==========================================================================
'
'  File:        RLE.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: RLE文件类
'  Version:     2012.08.25.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif

''' <summary>RLE文件类</summary>
''' <remarks>
''' 只支持8位色深。
''' 在转换成Gif时会做出一个妥协：将调色板中用得最少的颜色量化到其他颜色，为透明色提供空间
''' </remarks>
Public Class RLE
    Public Const Identifier As String = "BM"
    Protected Const Reserved As Int32 = 0
    ReadOnly Property BitmapDataOffset() As Int32
        Get
            Return 1090 + PicHeight * 4
        End Get
    End Property
    Protected Const BitmapHeaderSize As Int32 = &H28
    Protected PicWidth As Int32
    ReadOnly Property Width() As Int32
        Get
            Return PicWidth
        End Get
    End Property
    Protected PicHeight As Int32
    ReadOnly Property Height() As Int32
        Get
            Return PicHeight
        End Get
    End Property
    Protected Const Planes As Int16 = 1
    Protected Const BitsPerPixel As Int16 = 8
    Protected Const PicCompression As Int32 = 4
    Protected Const BitmapDataSize As Int32 = 0 '不用
    Protected Const HResolution As Int32 = &HB12 '不用
    Protected Const VResolution As Int32 = &HB12 '不用
    Protected Const Colors As Int32 = 0 '不用
    Protected Const ImportantColors As Int32 = 0 '不用
    Protected PicPalette As Int32()
    ReadOnly Property Palette() As Int32()
        Get
            Return PicPalette
        End Get
    End Property
    Protected PicRectangle As Int32(,)
    Property Rectangle() As Int32(,)
        Get
            Return PicRectangle
        End Get
        Set(ByVal Value As Int32(,))
            PicRectangle = Value
            If Value Is Nothing Then
                PicWidth = 0
                PicHeight = 0
            Else
                PicWidth = Rectangle.GetLength(0)
                PicHeight = Rectangle.GetLength(1)
            End If
        End Set
    End Property

    Private Sub New()
    End Sub

    Sub New(ByVal Width As Int32, ByVal Height As Int32, ByVal Palette As Int32())
        If Width < 0 OrElse Height < 0 Then Throw New ArgumentOutOfRangeException
        PicWidth = Width
        PicHeight = Height
        If Palette.GetLength(0) <> 256 Then Throw New ArgumentException
        PicPalette = Palette
    End Sub

    Sub New(ByVal Rectangle As Int32(,), ByVal Palette As Int32())
        If Palette.GetLength(0) <> 256 Then Throw New ArgumentException
        PicPalette = Palette
        If Rectangle Is Nothing Then Throw New ArgumentNullException
        PicWidth = Rectangle.GetLength(0)
        PicHeight = Rectangle.GetLength(1)
        PicRectangle = Rectangle
    End Sub

    Sub New(ByVal Path As String)
        With Me
            Using s = New StreamEx(Path, FileMode.Open, FileAccess.Read)
                For n As Integer = 0 To Identifier.Length - 1
                    If s.ReadByte() <> AscW(Identifier(n)) Then
                        Throw New InvalidDataException
                    End If
                Next
                s.ReadInt32() '跳过File Size
                s.ReadInt32() '跳过Reserved
                s.ReadInt32() '跳过Bitmap Data Offset
                s.ReadInt32() '跳过Bitmap Header Size
                .PicWidth = s.ReadInt32
                .PicHeight = s.ReadInt32
                If .PicWidth < 0 OrElse .PicHeight < 0 Then
                    Throw New InvalidDataException
                End If
                s.ReadInt16() '跳过Planes
                Dim BitsPerPixel As Int16 = s.ReadInt16
                If BitsPerPixel <> RLE.BitsPerPixel Then Throw New InvalidDataException
                Dim PicCompression As Int32 = s.ReadInt32
                s.ReadInt32() '跳过Bitmap Data Size
                s.ReadInt32() '跳过HResolution
                s.ReadInt32() '跳过VResolution
                s.ReadInt32() '跳过Colors
                s.ReadInt32() '跳过Important Colors

                If PicCompression <> RLE.PicCompression Then
                    Throw New InvalidDataException
                End If

                If BitsPerPixel <> RLE.BitsPerPixel Then
                    Throw New InvalidDataException
                End If

                .PicPalette = New Int32(CInt(2 ^ (BitsPerPixel)) - 1) {} '.Palette = New Int32((1 << ._BitsPerPixel) - 1) {}会导致Visual Studio的重构功能失效 Visual Studio Bug
                For n As Integer = 0 To (1 << BitsPerPixel) - 1
                    .PicPalette(n) = s.ReadInt32()
                Next

                s.Position += 12
                Dim LineAddress As Int32() = New Int32(.PicHeight - 1) {}
                For n As Integer = 0 To .PicHeight - 1
                    LineAddress(n) = s.ReadInt32()
                Next

                .PicRectangle = New Int32(.PicWidth - 1, .PicHeight - 1) {}

                Dim Line As Byte()
                Dim OrgLine As Int32()
                Dim offset As Integer = .BitmapDataOffset
                For y As Integer = 0 To .PicHeight - 2
                    Line = New Byte(LineAddress(y + 1) - LineAddress(y) - 1) {}
                    s.Position = offset + LineAddress(y)
                    For i As Integer = 0 To LineAddress(y + 1) - LineAddress(y) - 1
                        Line(i) = s.ReadByte
                    Next
                    OrgLine = DeRLE(Line)
                    If OrgLine.GetLength(0) <> .PicWidth Then Throw New InvalidDataException
                    For x As Integer = 0 To .PicWidth - 1
                        .PicRectangle(x, y) = OrgLine(x)
                    Next
                Next
                Line = New Byte(CInt(s.Length) - offset - LineAddress(.PicHeight - 1) - 1) {}
                s.Position = offset + LineAddress(.PicHeight - 1)
                For i As Integer = 0 To CInt(s.Length) - offset - LineAddress(.PicHeight - 1) - 1
                    Line(i) = s.ReadByte
                Next
                OrgLine = DeRLE(Line)
                If OrgLine.GetLength(0) <> .PicWidth Then Throw New InvalidDataException
                For x As Integer = 0 To .PicWidth - 1
                    .PicRectangle(x, .PicHeight - 1) = OrgLine(x)
                Next
            End Using
        End With
    End Sub
    Sub WriteToFile(ByVal Path As String)
        Using s = New StreamEx(Path, FileMode.Create)

            For n As Integer = 0 To Identifier.Length - 1
                s.WriteByte(CByte(AscW(Identifier(n))))
            Next
            s.WriteInt32(0)
            s.WriteInt32(Reserved)
            s.WriteInt32(BitmapDataOffset)
            s.WriteInt32(BitmapHeaderSize)
            s.WriteInt32(PicWidth)
            s.WriteInt32(PicHeight)
            s.WriteInt16(Planes)
            s.WriteInt16(BitsPerPixel)
            s.WriteInt32(PicCompression)
            s.WriteInt32(BitmapDataSize)
            s.WriteInt32(HResolution)
            s.WriteInt32(VResolution)
            s.WriteInt32(Colors)
            s.WriteInt32(ImportantColors)

            For n As Integer = 0 To (1 << BitsPerPixel) - 1
                s.WriteInt32(Palette(n))
            Next

            Dim p As String = GetMainFileName(Path)
            Dim Upper As Integer = Min(3, Path.Length - 1)
            For n As Integer = 0 To Upper
                s.WriteByte(CByte(AscW(p(n)) And &HFF))
            Next
            For n As Integer = Upper + 1 To 3
                s.WriteByte(0)
            Next

            s.WriteInt32(PicWidth)
            s.WriteInt32(PicHeight)

            Dim LineAddress As Int32() = New Int32(PicHeight - 1) {}
            Dim Line As Byte()
            Dim OrgLine As Int32()
            Dim offset2 As Integer = CInt(s.Position)
            Dim offset As Integer = BitmapDataOffset
            s.Position = offset

            For y As Integer = 0 To PicHeight - 1
                LineAddress(y) = CInt(s.Position) - offset
                OrgLine = New Int32(PicWidth - 1) {}
                For x As Integer = 0 To PicWidth - 1
                    OrgLine(x) = PicRectangle(x, y)
                Next
                Line = EnRLE(OrgLine)
                s.Write(Line, 0, Line.Length)
            Next

            s.Position = offset2
            For n As Integer = 0 To PicHeight - 1
                s.WriteInt32(LineAddress(n))
            Next

            s.Position = 2
            s.WriteInt32(CInt(s.Length))
        End Using
    End Sub

    Protected Shared Function EnRLE(ByVal Code As Int32()) As Byte()
        Dim src As New Queue(Of Int32)(Code)
        Dim ret As New Queue(Of Byte)
        Dim temp As New Queue(Of Byte)
        Dim IsTransparent As Boolean
        Dim Num As Integer
        Dim c As Int32 = src.Dequeue
        If c = -1 Then
            IsTransparent = True
        Else
            IsTransparent = False
            temp.Enqueue(CByte(c))
        End If
        Num += 1

        Const Max As Integer = 253
        While src.Count > 0
            c = src.Dequeue
            If IsTransparent Then
                If c = -1 Then
                    Num += 1
                Else
                    IsTransparent = False
                    While Num > 255
                        ret.Enqueue(&HFF)
                        ret.Enqueue(&HFF)
                        Num -= 255
                    End While
                    ret.Enqueue(&HFF)
                    ret.Enqueue(CByte(Num))
                    temp.Enqueue(CByte(c))
                    Num = 1
                End If
            Else
                If c = -1 Then
                    IsTransparent = True
                    ret.Enqueue(&HFE)
                    While Num > Max
                        ret.Enqueue(Max)
                        For n As Integer = 0 To Max - 1
                            ret.Enqueue(temp.Dequeue)
                        Next
                        Num -= Max
                    End While
                    ret.Enqueue(CByte(Num))
                    For n As Integer = 0 To Num - 1
                        ret.Enqueue(temp.Dequeue)
                    Next
                    Num = 1
                Else
                    temp.Enqueue(CByte(c))
                    Num += 1
                End If
            End If
        End While
        If IsTransparent Then
            While Num > 255
                ret.Enqueue(&HFF)
                ret.Enqueue(&HFF)
                Num -= 255
            End While
            ret.Enqueue(&HFF)
            ret.Enqueue(CByte(Num))
        Else
            ret.Enqueue(&HFE)
            While Num > Max
                ret.Enqueue(Max)
                For n As Integer = 0 To Max - 1
                    ret.Enqueue(temp.Dequeue)
                Next
                Num -= Max
            End While
            ret.Enqueue(CByte(Num))
            For n As Integer = 0 To Num - 1
                ret.Enqueue(temp.Dequeue)
            Next
        End If
        Return ret.ToArray
    End Function
    Protected Shared Function DeRLE(ByVal Code As Byte()) As Int32()
        Dim ret As New Queue(Of Int32)
        For n As Integer = 0 To Code.Length - 1
            Select Case Code(n)
                Case &HFF
                    For i As Integer = 0 To Code(n + 1) - 1
                        ret.Enqueue(-1)
                    Next
                    n += 1
                Case &HFE
                    For i As Integer = n + 2 To n + 2 + Code(n + 1) - 1
                        ret.Enqueue(Code(i))
                    Next
                    n += Code(n + 1) + 1
                Case Else
                    For i As Integer = n + 1 To n + 1 + Code(n) - 1
                        ret.Enqueue(Code(i))
                    Next
                    n += Code(n)
            End Select
        Next

        Return ret.ToArray
    End Function

    Shared Function FromGif(ByVal g As Gif) As RLE
        If g Is Nothing Then Throw New ArgumentNullException
        Dim f As GifImageBlock = g.Flame(0)
        If f Is Nothing Then Throw New ArgumentException
        Dim Palette As Int32()
        If f.LocalColorTableFlag Then
            Palette = f.Palette
        Else
            Palette = g.Palette
        End If
        Dim r As Integer(,) = New Integer(f.Width - 1, f.Height - 1) {}
        If f.TransparentColorFlag Then
            Dim t As Integer
            For y As Integer = 0 To f.Height - 1
                For x As Integer = 0 To f.Width - 1
                    t = f.Rectangle(x, y)
                    If t = f.TransparentColorIndex Then
                        r(x, y) = -1
                    Else
                        r(x, y) = t
                    End If
                Next
            Next
        Else
            For y As Integer = 0 To f.Height - 1
                For x As Integer = 0 To f.Width - 1
                    r(x, y) = f.Rectangle(x, y)
                Next
            Next
        End If
        Return New RLE(r, Palette)
    End Function

    Function ToGif() As Gif
        Dim r As Byte(,) = New Byte(PicWidth - 1, PicHeight - 1) {}
        Dim Num As Integer() = New Integer(256) {}
        Dim t As Integer
        For y As Integer = 0 To PicHeight - 1
            For x As Integer = 0 To PicWidth - 1
                t = PicRectangle(x, y)
                If t = -1 Then
                    Num(256) += 1
                Else
                    Num(t) += 1
                End If
            Next
        Next

        Dim g As GifImageBlock
        If Num(256) = 0 Then
            For y As Integer = 0 To PicHeight - 1
                For x As Integer = 0 To PicWidth - 1
                    r(x, y) = CByte(PicRectangle(x, y))
                Next
            Next
            g = New GifImageBlock(r, PicPalette)
        Else
            Dim Min As Integer = &H7FFFFFFF
            Dim MinIndex As Byte
            For n As Integer = 0 To 255
                If Num(n) <= Min Then
                    Min = Num(n)
                    MinIndex = CByte(n)
                End If
            Next
            Dim TranIndex As Byte
            Dim d As Integer = &H7FFFFFFF
            Dim cd As Integer
            For n As Integer = 0 To MinIndex - 1
                cd = ColorDistance(PicPalette(MinIndex), PicPalette(n))
                If cd < d Then
                    d = cd
                    TranIndex = CByte(n)
                End If
            Next
            For n As Integer = MinIndex + 1 To 255
                cd = ColorDistance(PicPalette(MinIndex), PicPalette(n))
                If cd < d Then
                    d = cd
                    TranIndex = CByte(n)
                End If
            Next

            Dim c As Integer
            For y As Integer = 0 To PicHeight - 1
                For x As Integer = 0 To PicWidth - 1
                    c = PicRectangle(x, y)
                    Select Case c
                        Case -1
                            r(x, y) = MinIndex
                        Case MinIndex
                            r(x, y) = TranIndex
                        Case Else
                            r(x, y) = CByte(c)
                    End Select
                Next
            Next
            g = New GifImageBlock(r, PicPalette)
            g.SetControl(1, MinIndex)
        End If

        Return New Gif(g)
    End Function
    Protected Shared Function ColorDistance(ByVal L As Int32, ByVal R As Int32) As Integer
        Dim dr As Integer = (L And &HFF0000 - R And &HFF0000) >> 16
        Dim dg As Integer = (L And &HFF00 - R And &HFF00) >> 8
        Dim db As Integer = L And &HFF - R And &HFF
        Return dr * dr + dg * dg + db * db
    End Function
End Class
