'==========================================================================
'
'  File:        RLC.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: RLE文件类
'  Version:     2019.05.07.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Drawing
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging

Public Class RLC
    Public Const Identifier As String = "rlc "

    Public Palette As Int16()
    Public Pixels As Byte?(,)

    Public Sub New()
    End Sub
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
    Public Sub WriteToFile(ByVal Path As String)
        Using s = Streams.CreateResizable(Path)
            s.WriteSimpleString(Identifier, 4)
            Dim Width = Pixels.GetLength(0)
            Dim Height = Pixels.GetLength(1)
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            For k = 0 To Palette.Length - 1
                s.WriteInt16(Palette(k))
            Next
            For j = 0 To Height - 1
                Dim ControlBytes = New List(Of Byte)
                Dim NumTransparentPixels = CByte(0)
                Dim OpaquePixels = New List(Of Byte)
                Dim TransparentMode = True
                For i = 0 To Width - 1
                    Dim Pixel = Pixels(i, j)
                    If Pixel Is Nothing Then
                        If TransparentMode Then
                            NumTransparentPixels = CByte(NumTransparentPixels + 1)
                            If NumTransparentPixels = &HFF Then
                                ControlBytes.Add(NumTransparentPixels)
                                NumTransparentPixels = 0
                                TransparentMode = False
                            End If
                        Else
                            ControlBytes.Add(CByte(OpaquePixels.Count))
                            ControlBytes.AddRange(OpaquePixels)
                            OpaquePixels.Clear()
                            TransparentMode = True
                            NumTransparentPixels = CByte(NumTransparentPixels + 1)
                        End If
                    Else
                        If TransparentMode Then
                            ControlBytes.Add(NumTransparentPixels)
                            NumTransparentPixels = 0
                            TransparentMode = False
                            OpaquePixels.Add(Pixel.Value)
                        Else
                            OpaquePixels.Add(Pixel.Value)
                            If OpaquePixels.Count = &HFF Then
                                ControlBytes.Add(CByte(OpaquePixels.Count))
                                ControlBytes.AddRange(OpaquePixels)
                                OpaquePixels.Clear()
                                TransparentMode = True
                            End If
                        End If
                    End If
                Next
                If TransparentMode Then
                    If NumTransparentPixels > 0 Then
                        ControlBytes.Add(NumTransparentPixels)
                        ControlBytes.Add(0)
                    End If
                Else
                    If OpaquePixels.Count > 0 Then
                        ControlBytes.Add(CByte(OpaquePixels.Count))
                        ControlBytes.AddRange(OpaquePixels)
                    End If
                End If

                s.WriteUInt16(Convert.ToUInt16(ControlBytes.Count))
                s.Write(ControlBytes.ToArray())
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
    Public Shared Function FromBitmap(ByVal bm As Bitmap) As RLC
        Using b = New System.Drawing.Bitmap(bm.Width, bm.Height, Imaging.PixelFormat.Format32bppArgb)
            Using g = System.Drawing.Graphics.FromImage(b)
                g.DrawImage(bm, 0, 0, b.Width, b.Height)
            End Using
            Dim r = b.GetRectangle(0, 0, b.Width, b.Height)
            Dim p = QuantizerARGB.Execute(r, 256)
            Dim Pixels = New Byte?(b.Width - 1, b.Height - 1) {}
            For j = 0 To b.Height - 1
                For i = 0 To b.Width - 1
                    If (r(i, j).Bits(31, 24) < &HF) Then
                        Pixels(i, j) = Nothing
                    Else
                        Pixels(i, j) = CByte(p.Value()(i, j))
                    End If
                Next
            Next
            Dim Palette = p.Key.Select(AddressOf ColorSpace.RGB32To16).ToArray()
            Return New RLC() With {.Palette = Palette, .Pixels = Pixels}
        End Using
    End Function
End Class
