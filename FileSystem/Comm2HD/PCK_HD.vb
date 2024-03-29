'==========================================================================
'
'  File:        PCK_HD.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: PCK文件流类(HD Remaster)
'  Version:     2020.01.25.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Packaging

'Commandos 2 HD Remaster PCK格式

'Header 00h
'4       Int32           NumFile         07 00 00 00

'Index 04h
'64      String          Name            "DATA\ANIMS\GRL\ALAMBRADA.GRL"
'4       Int32           Length          75 15 01 00
'4       Int32           Address         04 49 02 00

'Data
'(
'*       Byte()          FileData
')

''' <summary>PCK文件流类</summary>
''' <remarks>
''' 用于打开盟军2(HD Remaster)的PCK文件
''' </remarks>
Public Class PCK_HD
    Inherits PackageDiscrete

    Public Sub New(ByVal sp As NewReadingStreamPasser)
        MyBase.New(sp)
        Initialize()
    End Sub
    Public Sub New(ByVal sp As NewReadingWritingStreamPasser)
        MyBase.New(sp)
        Initialize()
    End Sub

    Public Sub Initialize()
        Dim s = Readable

        Dim NumFile = s.ReadInt32()

        For n = 0 To NumFile - 1
            Dim Name = s.ReadString(64, TextEncoding.UTF8)
            Dim Length = s.ReadInt32()
            Dim Address = s.ReadInt32()

            Dim f As New FileDB(Name, FileDB.FileType.File, Length, Address)
            PushFile(f)
        Next

        ScanHoles(GetSpace(s.Position))
    End Sub

    Public Shared ReadOnly Property Filter() As String
        Get
            Return "Commandos 2 HD Remaster PCK(*.PCK)|*.PCK"
        End Get
    End Property

    Public Shared Function Open(ByVal Path As String) As PackageBase
        Dim s As IStream = Nothing
        Dim sRead As IReadableSeekableStream = Nothing
        Try
            s = Streams.OpenResizable(Path)
        Catch
            sRead = Streams.OpenReadable(Path)
        End Try
        If s IsNot Nothing Then
            Return New PCK_HD(s.AsNewReadingWriting)
        Else
            Return New PCK_HD(sRead.AsNewReading)
        End If
    End Function

    Public Overrides Property FileAddressInPhysicalFileDB(ByVal File As FileDB) As Int64
        Get
            Readable.Position = 4 + 72 * IndexOfFile(File) + 68
            Return Readable.ReadInt32
        End Get
        Set(ByVal Value As Int64)
            Writable.Position = 4 + 72 * IndexOfFile(File) + 68
            Writable.WriteInt32(CInt(Value))
        End Set
    End Property

    Public Overrides Property FileLengthInPhysicalFileDB(ByVal File As FileDB) As Int64
        Get
            Readable.Position = 4 + 72 * IndexOfFile(File) + 64
            Return Readable.ReadInt32
        End Get
        Set(ByVal Value As Int64)
            Writable.Position = 4 + 72 * IndexOfFile(File) + 64
            Writable.WriteInt32(CInt(Value))
        End Set
    End Property

    Protected Overrides Function GetSpace(ByVal Length As Int64) As Int64
        Return Length
    End Function
End Class
