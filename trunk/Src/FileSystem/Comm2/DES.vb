'==========================================================================
'
'  File:        DES.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: SEC文件类
'  Version:     2012.09.05.
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
        Dim Address1 = s.ReadInt32()
        Dim Address2 = s.ReadInt32()
        Dim Address3 = s.ReadInt32()
        Dim Address4 = s.ReadInt32()
        Dim Address5 = s.ReadInt32()
        Dim NumView = s.ReadInt32()
        Dim NumEntryInTable1 = s.ReadInt32()
        Dim NumEntryInTable2 = s.ReadInt32()

        Dim GRLData = s.Read(CInt(Address1 - s.Position))

        s.Position = Address1
        Dim Table1 = New List(Of DES_Table1Record)()
        For k = 0 To NumEntryInTable1 - 1
            Dim r = New DES_Table1Record()
            r.TextureIndex = s.ReadInt32()
            r.Float1 = s.ReadFloat32()
            r.Float2 = s.ReadFloat32()
            r.Int1 = s.ReadInt32()
            r.Int2 = s.ReadInt32()
            r.Int3 = s.ReadInt32()
            r.Int4 = s.ReadInt32()
            r.Int5 = s.ReadInt32()
            Table1.Add(r)
        Next

        s.Position = Address2
        Dim Table2 = New List(Of DES_Table2Record)()
        For k = 0 To NumEntryInTable2 - 1
            Dim r As New DES_Table2Record()
            r.Name = s.ReadSimpleString(32)
            r.Unknown = New Int32(15)() {}
            For j = 0 To 15
                Dim NumItem = s.ReadInt32()
                Dim a = New List(Of Int32)()
                For i = 0 To NumItem - 1
                    a.Add(s.ReadInt32())
                Next
                r.Unknown(j) = a.ToArray()
            Next
            Table2.Add(r)
        Next

        s.Position = Address3
        Dim Table3 = New List(Of DES_Table3Record)()
        For k = 0 To NumView - 1
            Dim r As New DES_Table3Record()
            Dim NumItem = s.ReadInt32()
            Dim a = New List(Of Int32)()
            For i = 0 To NumItem - 1
                a.Add(s.ReadInt32())
            Next
            r.Unknown = a.ToArray()
            Table3.Add(r)
        Next

        s.Position = Address4
        If Address5 - Address4 <> 4 Then Throw New NotSupportedException()
        If s.ReadInt32() <> 0 Then Throw New NotSupportedException()

        s.Position = Address5
        Dim Table5 = New List(Of DES_Table5Record)()
        For k = 0 To NumView - 1
            Dim r As New DES_Table5Record()
            Dim NumItem = s.ReadInt32()
            Dim a = New List(Of Int32)()
            For i = 0 To NumItem - 1
                a.Add(s.ReadInt32())
            Next
            r.Unknown = a.ToArray()
            Table5.Add(r)
        Next

        Dim Root As New DES_Root
        Root.GRLData = GRLData
        Dim Description As New DES_Description
        Description.Table1 = Table1.ToArray()
        Description.Table2 = Table2.ToArray()
        Description.Table3 = Table3.ToArray()
        Description.Table5 = Table5.ToArray()
        Root.Description = Description

        Return Root
    End Function
    Public Shared Sub WriteFile(ByVal sp As NewWritingStreamPasser, ByVal v As DES_Root)
        Dim s = sp.GetStream()

        s.WriteSimpleString(Identifier, 8)

        If v.Description.Table3.Length <> v.Description.Table5.Length Then Throw New InvalidOperationException()

        Dim Address1 = 0
        Dim Address2 = 0
        Dim Address3 = 0
        Dim Address4 = 0
        Dim Address5 = 0
        Dim NumView = v.Description.Table3.Length
        Dim NumEntryInTable1 = v.Description.Table1.Length
        Dim NumEntryInTable2 = v.Description.Table2.Length

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

        Address1 = CInt(s.Position)
        For Each r In v.Description.Table1
            s.WriteInt32(r.TextureIndex)
            s.WriteFloat32(r.Float1)
            s.WriteFloat32(r.Float2)
            s.WriteInt32(r.Int1)
            s.WriteInt32(r.Int2)
            s.WriteInt32(r.Int3)
            s.WriteInt32(r.Int4)
            s.WriteInt32(r.Int5)
        Next

        Address2 = CInt(s.Position)
        For Each r In v.Description.Table2
            s.WriteSimpleString(r.Name, 32)
            If r.Unknown.Length <> 16 Then Throw New InvalidOperationException
            For Each a In r.Unknown
                Dim NumItem = a.Length
                s.WriteInt32(NumItem)
                For Each Item In a
                    s.WriteInt32(Item)
                Next
            Next
        Next

        Address3 = CInt(s.Position)
        For Each r In v.Description.Table3
            Dim NumItem = r.Unknown.Length
            s.WriteInt32(NumItem)
            For Each Item In r.Unknown
                s.WriteInt32(Item)
            Next
        Next

        Address4 = CInt(s.Position)
        s.WriteInt32(0)

        Address5 = CInt(s.Position)
        For Each r In v.Description.Table5
            Dim NumItem = r.Unknown.Length
            s.WriteInt32(NumItem)
            For Each Item In r.Unknown
                s.WriteInt32(Item)
            Next
        Next

        s.Position = AddressPosition
        s.WriteInt32(Address1)
        s.WriteInt32(Address2)
        s.WriteInt32(Address3)
        s.WriteInt32(Address4)
        s.WriteInt32(Address5)
        s.WriteInt32(NumView)
        s.WriteInt32(NumEntryInTable1)
        s.WriteInt32(NumEntryInTable2)

        s.Position = s.Length
    End Sub
End Class

Public Class DES_Root
    Public GRLData As Byte()
    Public Description As DES_Description
End Class

Public Class DES_Description
    Public Table1 As DES_Table1Record()
    Public Table2 As DES_Table2Record()
    Public Table3 As DES_Table3Record()
    Public Table5 As DES_Table5Record()
End Class

Public Class DES_Table1Record
    Public TextureIndex As Int32
    Public Float1 As Single
    Public Float2 As Single
    Public Int1 As Int32
    Public Int2 As Int32
    Public Int3 As Int32
    Public Int4 As Int32
    Public Int5 As Int32
End Class

Public Class DES_Table2Record
    Public Name As String
    Public Unknown As Int32()()
End Class

Public Class DES_Table3Record
    Public Unknown As Int32()
End Class

Public Class DES_Table5Record
    Public Unknown As Int32()
End Class
