'==========================================================================
'
'  File:        PAK.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: PAK文件流类
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports Firefly
Imports Firefly.TextEncoding
Imports zlib

''' <summary>PAK文件流类</summary>
''' <remarks>
''' 用于打开盟军打击力量的PAK文件
''' </remarks>
Public Class PAK
    Inherits StreamEx
    Public Const Identifier As String = "PAK"
    Public TypeSign As Type
    Public Enum Type As Byte
        TypeA = &H41
        TypeC = &H43
    End Enum
    Public VersionSign As Version
    Public Enum Version As Int32
        CSF_Prototype = 3
        CSF_Demo = 4
        CSF = 5
    End Enum
    Public PlatformSign As Platform
    Public Enum Platform As Int32
        PC = 1
        PS2 = 2
        XBOX = 3
    End Enum
    Public FileCount As Int32

    Public DataOffset As Int32

    Private Sub New(ByVal Path As String, ByVal FileMode As FileMode, ByVal Access As FileAccess, ByVal Share As FileShare)
        MyBase.New(Path, FileMode, Access, Share)
    End Sub
    Shared Function Open(ByVal Path As String) As PAK
        Dim Success = False
        Dim pf As PAK = Nothing
        Try
            pf = New PAK(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            With pf
                .Position = 0
                For n As Integer = 0 To Identifier.Length - 1
                    If .BaseStream.ReadByte() <> AscW(Identifier(n)) Then
                        pf.Close()
                        Throw New InvalidDataException
                    End If
                Next
                .TypeSign = CType(.ReadByte, PAK.Type)
                .VersionSign = CType(.ReadInt32, PAK.Version)
                .PlatformSign = CType(.ReadInt32, PAK.Platform)
                .FileCount = .ReadInt32
                Dim FileSet As New List(Of FileDB)
                For n As Integer = 0 To .FileCount - 1
                    FileSet.Add(New FileDB(pf))
                Next
                .DataOffset = CInt(.Position)
                .RootValue = FileDB.CreateDirectory(Nothing)
                For Each f As FileDB In FileSet
                    f.Address += .DataOffset
                    .PushFileToDir(f, .RootValue)
                Next
            End With
            Success = True
            Return pf
        Finally
            If Not Success Then
                Try
                    pf.Close()
                Catch
                End Try
            End If
        End Try
    End Function
    Private Sub PushFileToDir(ByVal f As FileDB, ByRef d As FileDB)
        Dim Dir As String = ""
        If f.Name.Contains("/") Then Dir = PopFirstDir(f.Name)
        If Dir = "" Then
            d.SubFileDB.Add(f)
            f.ParentFileDB = d
        Else
            If d.SubFileDBRef Is Nothing Then d.SubFileDBRef = New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)
            If Not d.SubFileDBRef.ContainsKey(Dir) Then
                Dim DirDB As FileDB = FileDB.CreateDirectory(Dir)
                d.SubFileDBRef.Add(DirDB.Name, d.SubFileDB.Count)
                d.SubFileDB.Add(DirDB)
                DirDB.ParentFileDB = d
            End If
            PushFileToDir(f, d.SubFileDB(d.SubFileDBRef(Dir)))
        End If
    End Sub
    Private Shared Function PopFirstDir(ByRef Path As String) As String
        Dim ret As String
        If Path = "" Then Return ""
        Dim NameS As Integer
        NameS = Path.IndexOf("/"c, NameS)
        If NameS < 0 Then
            ret = Path
            Path = ""
            Return ret
        Else
            ret = Path.Substring(0, NameS)
            Path = Path.Substring(NameS + 1)
            Return ret
        End If
    End Function


    Private RootValue As FileDB
    Public ReadOnly Property Root() As FileDB
        Get
            Return RootValue
        End Get
    End Property

    Private Shared Function ZlibDecompress(ByVal Data As Byte()) As Byte()
        Using Input As New StreamEx
            Input.Write(Data)
            Input.Position = 0
            Using Output As New StreamEx
                Using s As New zlib.ZInputStream(Input.ToUnsafeStream)
                    Dim b = s.Read()
                    Dim PreviousTotalOut As Int64 = 0
                    While True
                        Output.WriteByte(CByte(b))
                        b = s.Read()
                        If s.TotalOut > PreviousTotalOut Then
                            PreviousTotalOut = s.TotalOut
                        Else
                            Exit While
                        End If
                    End While
                End Using
                Output.Position = 0
                Return Output.Read(CInt(Output.Length))
            End Using
        End Using
    End Function

    Private Shared Function ZlibCompress(ByVal Data As Byte(), Optional ByVal Level As Integer = zlibConst.Z_DEFAULT_COMPRESSION) As Byte()
        Using Output As New StreamEx
            Using s As New ZOutputStream(Output.ToUnsafeStream(), Level)
                s.Write(Data, 0, Data.Length)
                s.finish()
                Output.Position = 0
                Return Output.Read(CInt(Output.Length))
            End Using
        End Using
    End Function

    Public Sub Extract(ByVal File As FileDB, ByVal Directory As String, Optional ByVal Mask As String = "*.*")
        With File
            Dim Dir As String = Directory.Trim.TrimEnd("\"c)
            If Dir <> "" AndAlso Not IO.Directory.Exists(Dir) Then IO.Directory.CreateDirectory(Dir)
            Select Case .Type
                Case FileDB.FileType.File
                    If IsMatchFileMask(.Name, Mask) Then
                        Dim t As New StreamEx(GetPath(Dir, .Name), FileMode.Create)
                        Me.Position = .Address
                        If TypeSign = Type.TypeC Then
                            Dim Length As Int32 = Me.ReadInt32()
                            While Length > 0
                                Dim OriginalLength As Int32 = Me.ReadInt32()
                                Dim SrcBuffer As Byte() = New Byte(Length - 1) {}
                                For n As Integer = 0 To Length - 1
                                    SrcBuffer(n) = Me.ReadByte
                                Next
                                Dim DestBuffer As Byte() = ZlibDecompress(SrcBuffer)
                                If DestBuffer.Length <> OriginalLength Then DestBuffer = DestBuffer.Extend(OriginalLength, 0)
                                For n As Integer = 0 To OriginalLength - 1
                                    t.WriteByte(DestBuffer(n))
                                Next
                                Length = Me.ReadInt32()
                            End While
                        Else
                            For n As Integer = 0 To .Length - 1
                                t.WriteByte(Me.ReadByte)
                            Next
                        End If
                        t.Close()
                    End If
                Case FileDB.FileType.Directory
                    Dim d As String = GetPath(Dir, .Name)
                    If d <> "" AndAlso Not IO.Directory.Exists(d) Then IO.Directory.CreateDirectory(d)
                    For Each FileDB As FileDB In File.SubFileDB
                        Extract(FileDB, GetPath(Dir, .Name), Mask)
                    Next
                Case Else
            End Select
        End With
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class FileDB
        Public Name As String
        Public Address As Int32
        Public Length As Int32
        Public Unknown1 As Int64 'Hash Code?

        Public Type As FileType
        Public ParentFileDB As FileDB
        Public SubFileDB As New List(Of FileDB)
        Public SubFileDBRef As Dictionary(Of String, Integer)
        Private Sub New(ByVal Name As String, ByVal Type As FileType, ByVal Length As Int32, ByVal Address As Int32)
            Me.Name = Name
            Me.Type = Type
            Me.Length = Length
            Me.Address = Address
        End Sub
        Public Sub New(ByVal s As PAK)
            Dim c As New List(Of Byte)
            Dim b As Byte = s.ReadByte
            While b <> 0
                c.Add(b)
                b = s.ReadByte
            End While
            Name = Windows1252.GetChars(c.ToArray)
            If s.VersionSign = Version.CSF Or s.PlatformSign = Platform.XBOX Or s.PlatformSign = Platform.PS2 Then
                Address = s.ReadInt32 - 13
            ElseIf s.VersionSign = Version.CSF_Demo Or s.VersionSign = Version.CSF_Prototype Then
                Address = s.ReadInt32
            Else
                Throw New InvalidDataException()
            End If
            Length = s.ReadInt32
            Unknown1 = s.ReadInt64()
        End Sub
        Public Enum FileType As Byte
            File = 0
            Directory = 1
            DirectoryEnd = 255
        End Enum
        Public Shared Function CreateFile(ByVal Name As String, ByVal Length As Int32, ByVal Address As Int32) As FileDB
            Return New FileDB(Name, FileType.File, Length, Address)
        End Function
        Public Shared Function CreateDirectory(ByVal Name As String) As FileDB
            Return New FileDB(Name, FileType.Directory, &HFFFFFFFF, 0)
        End Function
        Public Sub WriteToFile(ByVal s As PAK)
            s.Write(Windows1252.GetBytes(Name))
            s.WriteByte(0)
            s.WriteInt32(Address + 13 - s.DataOffset)
            s.WriteInt32(Length)
            s.WriteInt64(Unknown1)
        End Sub
    End Class
End Class
