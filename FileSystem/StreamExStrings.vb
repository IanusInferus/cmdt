'==========================================================================
'
'  File:        StreamExStrings.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流类 - 字符串
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Firefly
Imports Firefly.TextEncoding

Partial Public Class StreamEx
    ''' <summary>读取\0字节结尾的字符串(UTF-16等不适用)。</summary>
    Public Function ReadString(ByVal Count As Integer, ByVal Encoding As System.Text.Encoding) As String
        Dim Bytes As New List(Of Byte)
        For n = 0 To Count - 1
            Dim b = ReadByte()
            If b = Nul Then
                Position += Count - 1 - n
                Exit For
            Else
                Bytes.Add(b)
            End If
        Next
        Return Encoding.GetChars(Bytes.ToArray)
    End Function
    ''' <summary>写入\0字节结尾的字符串(UTF-16等不适用)。</summary>
    Public Sub WriteString(ByVal s As String, ByVal Count As Integer, ByVal Encoding As System.Text.Encoding)
        If s = "" Then
            For n = 0 To Count - 1
                WriteByte(0)
            Next
        Else
            Dim Bytes = Encoding.GetBytes(s)
            If Bytes.Length > Count Then Throw New InvalidDataException
            Write(Bytes)
            For n = Bytes.Length To Count - 1
                WriteByte(0)
            Next
        End If
    End Sub
    ''' <summary>读取包括\0字节的字符串(如UTF-16)。</summary>
    Public Function ReadStringWithNull(ByVal Count As Integer, ByVal Encoding As System.Text.Encoding) As String
        Return Encoding.GetChars(Read(Count))
    End Function

    ''' <summary>读取ASCII字符串。</summary>
    Public Function ReadSimpleString(ByVal Count As Integer) As String
        Return ReadString(Count, ASCII)
    End Function
    ''' <summary>写入ASCII字符串。</summary>
    Public Sub WriteSimpleString(ByVal s As String, ByVal Count As Integer)
        WriteString(s, Count, ASCII)
    End Sub
    ''' <summary>写入ASCII字符串。</summary>
    Public Sub WriteSimpleString(ByVal s As String)
        WriteSimpleString(s, s.Length)
    End Sub
    ''' <summary>读取ASCII字符串(包括\0)。</summary>
    Public Function ReadSimpleStringWithNull(ByVal Count As Integer) As String
        Return ReadStringWithNull(Count, ASCII)
    End Function
    ''' <summary>查看ASCII字符串。</summary>
    Public Function PeekSimpleString(ByVal Count As Integer) As String
        Dim HoldPosition = Position
        Try
            Return ReadSimpleString(Count)
        Finally
            Position = HoldPosition
        End Try
    End Function
End Class
