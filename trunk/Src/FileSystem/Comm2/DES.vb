'==========================================================================
'
'  File:        DES.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: SEC文件类
'  Version:     2012.09.10.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Firefly
Imports Firefly.Streaming

''' <summary>DES文件类</summary>
''' <remarks>
''' 用于处理盟军2的DES文件
''' </remarks>
Public NotInheritable Class DES
    Private Sub New()
    End Sub

    Public Shared ReadOnly Identifier As String = "FDES0100"

    Public Shared Function ReadFile(ByVal Path As String) As DES_Root
        Using s = Streams.OpenReadable(Path)
            Return ReadFile(s.AsNewReading())
        End Using
    End Function
    Public Shared Sub WriteFile(ByVal Path As String, ByVal v As DES_Root)
        Using s = Streams.CreateResizable(Path)
            WriteFile(s.AsNewWriting(), v)
        End Using
    End Sub

    Public Shared Function ReadFile(ByVal sp As NewReadingStreamPasser) As DES_Root
        Dim s = sp.GetStream()

        If s.ReadSimpleString(8) <> Identifier Then Throw New InvalidDataException()
        Dim CoverTableAddress = s.ReadInt32()
        Dim DestructionTableAddress = s.ReadInt32()
        Dim InitialCoverTableAddress = s.ReadInt32()
        Dim Address4 = s.ReadInt32()
        Dim InitialHidenObjectTableAddress = s.ReadInt32()
        Dim NumView = s.ReadInt32()
        Dim NumCover = s.ReadInt32()
        Dim NumDestruction = s.ReadInt32()

        Dim GRLData = s.Read(CInt(CoverTableAddress - s.Position))

        s.Position = CoverTableAddress
        Dim Table1 = New List(Of DES_Cover)()
        For k = 0 To NumCover - 1
            Dim r = New DES_Cover()
            r.TextureIndex = s.ReadInt32()
            r.SecX = s.ReadFloat32()
            r.SecY = s.ReadFloat32()
            r.SecZ = s.ReadFloat32()
            r.Y64X = s.ReadInt32()
            r.Y64Y = s.ReadInt32()
            r.Unknown1 = s.ReadInt32()
            r.Unknown2 = s.ReadInt32()
            Table1.Add(r)
        Next

        s.Position = DestructionTableAddress
        Dim Table2 = New List(Of DES_Destruction)()
        For k = 0 To NumDestruction - 1
            Dim r As New DES_Destruction()
            r.Name = s.ReadSimpleString(32)
            r.Views = New DES_DestructionInView(NumView - 1) {}
            For n = 0 To NumView - 1
                Dim Indices = New Int32(3)() {}
                For j = 0 To 3
                    Dim NumItem = s.ReadInt32()
                    Dim a = New List(Of Int32)()
                    For i = 0 To NumItem - 1
                        a.Add(s.ReadInt32())
                    Next
                    Indices(j) = a.ToArray()
                Next
                r.Views(n) = New DES_DestructionInView With {.Unknown = Indices(0), .CoverIndicesToShow = Indices(1), .ObjectsToHide = Indices(2), .ObjectsToShow = Indices(3)}
            Next
            Table2.Add(r)
        Next

        s.Position = InitialCoverTableAddress
        Dim Table3 = New List(Of DES_InitialCoverInView)()
        For k = 0 To NumView - 1
            Dim r As New DES_InitialCoverInView()
            Dim NumItem = s.ReadInt32()
            Dim a = New List(Of Int32)()
            For i = 0 To NumItem - 1
                a.Add(s.ReadInt32())
            Next
            r.CoverIndices = a.ToArray()
            Table3.Add(r)
        Next

        s.Position = Address4
        If InitialHidenObjectTableAddress - Address4 <> 4 Then Throw New NotSupportedException()
        If s.ReadInt32() <> 0 Then Throw New NotSupportedException()

        s.Position = InitialHidenObjectTableAddress
        Dim Table5 = New List(Of DES_InitialHidenObjectInView)()
        For k = 0 To NumView - 1
            Dim r As New DES_InitialHidenObjectInView()
            Dim NumItem = s.ReadInt32()
            Dim a = New List(Of Int32)()
            For i = 0 To NumItem - 1
                a.Add(s.ReadInt32())
            Next
            r.ObjectIndices = a.ToArray()
            Table5.Add(r)
        Next

        Dim Root As New DES_Root
        Root.GRLData = GRLData
        Dim Description As New DES_Description
        Description.CoverTable = Table1.ToArray()
        Description.DestructionTable = Table2.ToArray()
        Description.InitialCoverTable = Table3.ToArray()
        Description.InitialHidenObjectTable = Table5.ToArray()
        Root.Description = Description

        Return Root
    End Function
    Public Shared Sub WriteFile(ByVal sp As NewWritingStreamPasser, ByVal v As DES_Root)
        Dim s = sp.GetStream()

        s.WriteSimpleString(Identifier, 8)

        If v.Description.InitialCoverTable.Length <> v.Description.InitialHidenObjectTable.Length Then Throw New InvalidOperationException()

        Dim CoverTableAddress = 0
        Dim DestructionTableAddress = 0
        Dim InitialCoverTableAddress = 0
        Dim Address4 = 0
        Dim InitialHidenObjectTableAddress = 0
        Dim NumView = v.Description.InitialCoverTable.Length
        Dim NumCover = v.Description.CoverTable.Length
        Dim NumDestruction = v.Description.DestructionTable.Length

        Dim AddressPosition = s.Position
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)
        s.WriteInt32(0)

        s.Write(v.GRLData)

        CoverTableAddress = CInt(s.Position)
        For Each r In v.Description.CoverTable
            s.WriteInt32(r.TextureIndex)
            s.WriteFloat32(r.SecX)
            s.WriteFloat32(r.SecY)
            s.WriteFloat32(r.SecZ)
            s.WriteInt32(r.Y64X)
            s.WriteInt32(r.Y64Y)
            s.WriteInt32(r.Unknown1)
            s.WriteInt32(r.Unknown2)
        Next

        DestructionTableAddress = CInt(s.Position)
        For Each r In v.Description.DestructionTable
            s.WriteSimpleString(r.Name, 32)
            If r.Views.Length <> NumView Then Throw New InvalidOperationException
            For Each rv In r.Views
                Dim Indices = New Int32()() {rv.Unknown, rv.CoverIndicesToShow, rv.ObjectsToHide, rv.ObjectsToShow}
                For Each a In Indices
                    Dim NumItem = a.Length
                    s.WriteInt32(NumItem)
                    For Each Item In a
                        s.WriteInt32(Item)
                    Next
                Next
            Next
        Next

        InitialCoverTableAddress = CInt(s.Position)
        For Each r In v.Description.InitialCoverTable
            Dim NumItem = r.CoverIndices.Length
            s.WriteInt32(NumItem)
            For Each Item In r.CoverIndices
                s.WriteInt32(Item)
            Next
        Next

        Address4 = CInt(s.Position)
        s.WriteInt32(0)

        InitialHidenObjectTableAddress = CInt(s.Position)
        For Each r In v.Description.InitialHidenObjectTable
            Dim NumItem = r.ObjectIndices.Length
            s.WriteInt32(NumItem)
            For Each Item In r.ObjectIndices
                s.WriteInt32(Item)
            Next
        Next

        s.Position = AddressPosition
        s.WriteInt32(CoverTableAddress)
        s.WriteInt32(DestructionTableAddress)
        s.WriteInt32(InitialCoverTableAddress)
        s.WriteInt32(Address4)
        s.WriteInt32(InitialHidenObjectTableAddress)
        s.WriteInt32(NumView)
        s.WriteInt32(NumCover)
        s.WriteInt32(NumDestruction)

        s.Position = s.Length
    End Sub
End Class

Public Class DES_Root
    Public GRLData As Byte()
    Public Description As DES_Description
End Class

Public Class DES_Description
    Public CoverTable As DES_Cover()
    Public DestructionTable As DES_Destruction()
    Public InitialCoverTable As DES_InitialCoverInView()
    Public InitialHidenObjectTable As DES_InitialHidenObjectInView()
End Class

Public Class DES_Cover
    Public TextureIndex As Int32
    Public SecX As Single
    Public SecY As Single
    Public SecZ As Single
    Public Y64X As Int32
    Public Y64Y As Int32
    Public Unknown1 As Int32
    Public Unknown2 As Int32
End Class

Public Class DES_Destruction
    Public Name As String
    Public Views As DES_DestructionInView()
End Class

Public Class DES_DestructionInView
    Public Unknown As Int32()
    Public CoverIndicesToShow As Int32()
    Public ObjectsToHide As Int32()
    Public ObjectsToShow As Int32()
End Class

Public Class DES_InitialCoverInView
    Public CoverIndices As Int32()
End Class

Public Class DES_InitialHidenObjectInView
    Public ObjectIndices As Int32()
End Class
