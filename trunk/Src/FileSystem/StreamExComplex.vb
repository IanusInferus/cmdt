'==========================================================================
'
'  File:        StreamExComplex.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流类 - 复杂
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Firefly

Partial Public Class StreamEx
    ''' <summary>已重载。读取到字节数组。</summary>
    Public Sub Read(ByVal Buffer() As Byte)
        Read(Buffer, 0, Buffer.Length)
    End Sub
    ''' <summary>已重载。读取字节数组。</summary>
    Public Function Read(ByVal Count As Integer) As Byte()
        Dim d As Byte() = New Byte(Count - 1) {}
        Read(d, 0, Count)
        Return d
    End Function
    ''' <summary>已重载。写入字节数组。</summary>
    Public Sub Write(ByVal Buffer As Byte())
        Write(Buffer, 0, Buffer.Length)
    End Sub

    ''' <summary>读取Int32数组。</summary>
    Public Function ReadInt32Array(ByVal Count As Integer) As Int32()
        Dim d As Int32() = New Int32(Count - 1) {}
        For n As Integer = 0 To Count - 1
            d(n) = ReadInt32()
        Next
        Return d
    End Function
    ''' <summary>写入Int32数组。</summary>
    Public Sub WriteInt32Array(ByVal Buffer As Int32())
        For Each i In Buffer
            WriteInt32(i)
        Next
    End Sub

    ''' <summary>读取到外部流。</summary>
    Public Sub ReadToStream(ByVal s As StreamEx, ByVal Count As Int64)
        If Count <= 0 Then Return
        Dim Buffer As Byte() = New Byte(CInt(Min(Count, 4 * (1 << 20)) - 1)) {}
        For n As Int64 = 0 To Count - Buffer.Length Step Buffer.Length
            Read(Buffer)
            s.Write(Buffer)
        Next
        Dim LeftLength As Int32 = CInt(Count Mod Buffer.Length)
        Read(Buffer, 0, LeftLength)
        s.Write(Buffer, 0, LeftLength)
    End Sub
    ''' <summary>从外部流写入。</summary>
    Public Sub WriteFromStream(ByVal s As StreamEx, ByVal Count As Int64)
        If Count <= 0 Then Return
        Dim Buffer As Byte() = New Byte(CInt(Min(Count, 4 * (1 << 20)) - 1)) {}
        For n As Int64 = 0 To Count - Buffer.Length Step Buffer.Length
            s.Read(Buffer)
            Write(Buffer)
        Next
        Dim LeftLength As Int32 = CInt(Count Mod Buffer.Length)
        s.Read(Buffer, 0, LeftLength)
        Write(Buffer, 0, LeftLength)
    End Sub
    ''' <summary>保存到文件。</summary>
    Public Sub SaveAs(ByVal Path As String)
        Using s As New StreamEx(Path, FileMode.Create, FileAccess.ReadWrite)
            Dim Current As Int64 = Position
            Position = 0
            ReadToStream(s, Length)
            Position = Current
        End Using
    End Sub
End Class
