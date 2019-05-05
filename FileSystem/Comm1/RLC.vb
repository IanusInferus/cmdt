'==========================================================================
'
'  File:        RLC.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: RLE文件类
'  Version:     2019.05.06.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.IO
Imports System.Drawing
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging

Public Class RLC
    Public Const Identifier As String = "rlc "

    Public Palette As Int16()
    Public Pixels As Byte?(,)

    Public Sub New(ByVal Path As String)
        Using s = Streams.OpenReadable(Path)
            If s.ReadSimpleString(4) <> Identifier Then
                Throw New InvalidDataException
            End If
            Dim Width = s.ReadInt32()
            Dim Height = s.ReadInt32()
            Palette = New Int16(256 - 1) {}
            For k = 0 To Palette.Length - 1
                Palette(k) = s.ReadInt16()
            Next
            Pixels = New Byte?(Width - 1, Height - 1) {}
            For j = 0 To Height - 1
                Dim NumControlBytes = s.ReadUInt16()
                Dim ControlBytesOffset = 0
                Dim i = 0
                While ControlBytesOffset < NumControlBytes
                    Dim NumTransparentPixels = s.ReadByte()
                    ControlBytesOffset += 1
                    For k = 0 To NumTransparentPixels - 1
                        Pixels(i, j) = Nothing
                        i += 1
                    Next
                    Dim NumOpaquePixels = s.ReadByte()
                    ControlBytesOffset += 1
                    For k = 0 To NumOpaquePixels - 1
                        Pixels(i, j) = s.ReadByte()
                        i += 1
                        ControlBytesOffset += 1
                    Next
                End While
            Next
        End Using
    End Sub

    Public Function ToBitmap() As Bitmap
        Using b = New Bmp(Pixels.GetLength(0), Pixels.GetLength(1), 32)
            For j = 0 To Pixels.GetLength(1) - 1
                For i = 0 To Pixels.GetLength(0) - 1
                    Dim p = Pixels(i, j)
                    If p Is Nothing Then
                        b.SetPixel(i, j, 0)
                    Else
                        Dim c = ColorSpace.RGB16To32(Palette(p.Value))
                        b.SetPixel(i, j, c)
                    End If
                Next
            Next
            Dim Rectangle = b.GetRectangleAsARGB(0, 0, b.Width, b.Height)
            Dim bm = New Bitmap(b.Width, b.Height, Imaging.PixelFormat.Format32bppArgb)
            bm.SetRectangle(0, 0, Rectangle)
            Return bm
        End Using
    End Function
End Class
