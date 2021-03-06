'==========================================================================
'
'  File:        H2O.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: MBI文件类
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================
'UNDONE

Option Strict On
Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging

''' <summary>H2O文件类</summary>
Public Class H2O
    Private Data As Byte()
    Private IndexTable As Byte(,)
    Private PaletteTable As UInt16()

    Public Sub New(ByVal sp As ZeroPositionStreamPasser)
        With Me
            Dim si = sp.GetStream
            Data = si.Read(CInt(si.Length))

            Using s As New ByteArrayStream(Data)
                Dim NumView = s.ReadInt32
                Dim Unknown = s.ReadInt32
                Dim ViewAddresses = New Int32(NumView - 1) {}
                For n = 0 To NumView - 1
                    ViewAddresses(n) = s.ReadInt32
                Next

                .IndexTable = New Byte(255, 255) {}
                For y As Integer = 0 To 255
                    For x As Integer = 0 To 255
                        .IndexTable(x, y) = s.ReadByte
                    Next
                Next

                .PaletteTable = New UInt16(255) {}
                For n = 0 To 255
                    .PaletteTable(n) = s.ReadUInt16()
                    s.ReadUInt16()
                Next
            End Using
        End With
    End Sub

    Public Sub WriteTo(ByVal sp As ZeroLengthStreamPasser)
        With Me
            Using s As New ByteArrayStream(Data)
                Dim NumView = s.ReadInt32
                Dim Unknown = s.ReadInt32
                Dim ViewAddresses = New Int32(NumView - 1) {}
                For n = 0 To NumView - 1
                    ViewAddresses(n) = s.ReadInt32
                Next

                s.Position = s.Position

                For y As Integer = 0 To 255
                    For x As Integer = 0 To 255
                        s.WriteByte(.IndexTable(x, y))
                    Next
                Next

                For n = 0 To 255
                    s.WriteUInt16(.PaletteTable(n))
                    s.Position += 2
                Next
            End Using

            Dim so = sp.GetStream()
            so.Write(Data)
        End With
    End Sub

    Public Function GetPalette() As Int32()
        Return PaletteTable.Select(Function(c) RGB16To32(CUS(c))).ToArray()
    End Function

    Public Sub SetPalette(ByVal p As Int32())
        If p.Length <> 256 Then Throw New ArgumentException
        For n = 0 To 255
            PaletteTable(n) = CSU(RGB32To16(p(n)))
        Next
    End Sub

    Public Function GetRectangle() As Byte(,)
        Return DirectCast(IndexTable.Clone(), Byte(,))
    End Function

    Public Sub SetRectangle(ByVal a As Byte(,))
        If a.GetLength(0) <> 256 AndAlso a.GetLength(1) <> 256 Then Throw New ArgumentException

        For y = 0 To 255
            For x = 0 To 255
                IndexTable(x, y) = a(x, y)
            Next
        Next
    End Sub
End Class
