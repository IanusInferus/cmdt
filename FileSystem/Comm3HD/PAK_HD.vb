'==========================================================================
'
'  File:        PAK_HD.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: PAK文件流类(HD Remaster)
'  Version:     2022.09.10.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Packaging
Imports lzo.net
Imports System.IO.Compression

'Commandos 3 HD Remaster PAK格式
'Based on analysis by herbert3000 at https://zenhax.com/viewtopic.php?f=9&t=17364

'Header 00h
'4       String          Identifier      "30KP"
'4       Int32           Unknown         00 00 00 00
'4       Int32           NumFile         26 4D 00 00

'Index 0Ch
'4       Int32           NameHash        21 D2 01 00
'4       Int32           Unknown         00 00 00 00
'8       Int64           Address         9C 3B 07 00 00 00 00 00
'4       Int32           Length          8E 60 00 00
'4       Int32           IsCompressed    00 00 00 00 / 00 00 00 80

'Data
'(
'*       Byte()          FileData/LzoCompressedData
')

''' <summary>PAK文件流类</summary>
''' <remarks>
''' 用于打开盟军3(HD Remaster)的PAK文件
''' </remarks>
Public Class PAK_HD
    Inherits PackageDiscrete

    Private Class ExtFileDB
        Public AddressAddress As Int64
        Public LengthAddress As Int64
        Public IsCompressedAddress As Int64
        Public IsCompressed As Boolean
    End Class
    Private ExtFileDBDict As New Dictionary(Of FileDB, ExtFileDB)

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

        Dim Identifier = s.ReadSimpleString(4)
        If Identifier <> "30KP" Then Throw New InvalidDataException()
        Dim Unknown1 = s.ReadInt32()
        Dim NumFile = s.ReadInt32()

        Dim CurrentFiles = New List(Of FileDB)
        For n = 0 To NumFile - 1
            Dim NameHash = s.ReadInt32()
            Dim Unknown2 = s.ReadInt32()
            Dim AddressAddress = s.Position
            Dim Address = s.ReadInt64()
            Dim LengthAddress = s.Position
            Dim Length = s.ReadInt32()
            Dim IsCompressedAddress = s.Position
            Dim IsCompressed = s.ReadInt32() <> 0

            Dim f As New FileDB("OUT_" & n.ToString(), FileDB.FileType.File, Length, Address)
            Dim e As New ExtFileDB With {.AddressAddress = AddressAddress, .LengthAddress = LengthAddress, .IsCompressedAddress = IsCompressedAddress, .IsCompressed = IsCompressed}
            ExtFileDBDict.Add(f, e)
            CurrentFiles.Add(f)
        Next

        Dim ScanStart = s.Position

        For Each f In CurrentFiles
            s.Position = f.Address
            If f.Length >= 4 Then
                Dim FileIdentifier = ""
                Dim e = ExtFileDBDict(f)
                If e.IsCompressed Then
                    Dim i = s.ReadSimpleString(4)
                    If i <> "SDPC" Then
                        Throw New InvalidOperationException()
                    End If
                    Dim OriginalLength = s.ReadUInt32()
                    Using p = s.Partialize(f.Address, f.Length)
                        p.Position = 8
                        Try
                            Using lzo As New LzoStream(p.ToUnsafeStream(), CompressionMode.Decompress)
                                FileIdentifier = lzo.AsReadable().ReadSimpleString(4)
                            End Using
                        Catch
                            FileIdentifier = "SDPC"
                        End Try
                    End Using
                Else
                    FileIdentifier = s.ReadSimpleString(4)
                End If
                If FileIdentifier = "SDPC" Then
                    f.Name += ".bin"
                ElseIf FileIdentifier = "DDS " Then
                    f.Name += ".dds"
                ElseIf FileIdentifier = "RIFF" Then
                    f.Name += ".wav"
                ElseIf FileIdentifier = "GFRL" Then
                    f.Name += ".grl"
                ElseIf FileIdentifier = "[sha" Then
                    f.Name += ".shader"
                ElseIf FileIdentifier = ChrW(1) Then
                    f.Name += ".1"
                Else
                    f.Name += ".txt"
                End If
            End If
        Next

        For Each f In CurrentFiles
            PushFile(f)
        Next
        ScanHoles(GetSpace(ScanStart))
    End Sub

    Public Shared ReadOnly Property Filter() As String
        Get
            Return "Commandos 3 HD Remaster PAK(*.PAK)|*.PAK"
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
            Return New PAK_HD(s.AsNewReadingWriting)
        Else
            Return New PAK_HD(sRead.AsNewReading)
        End If
    End Function

    Public Overrides Property FileAddressInPhysicalFileDB(ByVal File As FileDB) As Int64
        Get
            Dim e = ExtFileDBDict(File)
            Readable.Position = e.AddressAddress
            Return Readable.ReadInt64
        End Get
        Set(ByVal Value As Int64)
            Dim e = ExtFileDBDict(File)
            Writable.Position = e.AddressAddress
            Writable.WriteInt64(Value)
        End Set
    End Property

    Public Overrides Property FileLengthInPhysicalFileDB(ByVal File As FileDB) As Int64
        Get
            Dim e = ExtFileDBDict(File)
            Readable.Position = e.LengthAddress
            Return Readable.ReadInt32
        End Get
        Set(ByVal Value As Int64)
            Dim e = ExtFileDBDict(File)
            Writable.Position = e.LengthAddress
            Writable.WriteInt32(CInt(Value))
        End Set
    End Property

    Protected Overrides Function GetSpace(ByVal Length As Int64) As Int64
        Return Length
    End Function

    Protected Overrides Sub ExtractSingleInner(File As FileDB, sp As NewWritingStreamPasser)
        Dim e = ExtFileDBDict(File)
        If e.IsCompressed Then
            Using msOutput As New MemoryStream()
                Dim s = Readable
                s.Position = File.Address
                Dim i = s.ReadSimpleString(4)
                If i <> "SDPC" Then
                    Throw New InvalidOperationException()
                End If
                Dim OriginalLength = s.ReadUInt32()
                Using p = s.Partialize(File.Address, File.Length)
                    p.Position = 8
                    Using lzo As New LzoStream(p.ToUnsafeStream(), CompressionMode.Decompress)
                        lzo.CopyTo(msOutput)
                    End Using
                End Using

                msOutput.Position = 0
                msOutput.AsIStream().ReadToStream(sp.GetStream(), msOutput.Length)
            End Using
        Else
            MyBase.ExtractSingleInner(File, sp)
        End If
    End Sub
    Protected Overrides Sub ReplaceSingleInner(File As FileDB, sp As NewReadingStreamPasser)
        MyBase.ReplaceSingleInner(File, sp)
        Dim e = ExtFileDBDict(File)
        If e.IsCompressed Then
            e.IsCompressed = False
            Dim s = Writable
            s.Position = e.IsCompressedAddress
            s.WriteInt32(0)
        End If
    End Sub
End Class
