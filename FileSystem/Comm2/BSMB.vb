'==========================================================================
'
'  File:        BSMB.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: BSMB文件类
'  Version:     2023.10.27.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Firefly
Imports Firefly.TextEncoding

Public MustInherit Class BSMB
    Public Shared ReadOnly Identifier As String = "BSMB"

    Private Sub New()
    End Sub

    Private Enum NodeTypes As Int32
        StringLiteral = &H43
        Array = &H49
        CandidateList = &H4D
        NumericLiteral = &H4E
        StructureNode = &H54
    End Enum

    Private Class BSMBReader
        Private s As StreamEx
        Private NumNodeTable As Int32
        Private NodeTable_Types As NodeTypes()
        Private NodeTable_Parameters As Object()
        Private NumStructureTable As Int32
        Private StructureTable_NumField As Int32()
        Private StructureTable_FieldStart As Int32()
        Private NumFieldTable As Int32
        Private FieldTable_NameAddress As Int32()
        Private FieldTable_ValueIndex As Int32()
        Private NumArrayTable As Int32
        Private ArrayTable_NumElement As Int32()
        Private ArrayTable_ElementStart As Int32()
        Private NumElementTable As Int32
        Private ElementTable_ElementNodeIndex As Int32()
        Private StringIdentifierTableLength As Int32
        Private StringLiteralTableLength As Int32
        Private StringIdentifierTableStart As Int32
        Private StringLiteralTableStart As Int32

        Public Root As StructureNode

        Public Sub New(ByVal sp As ZeroPositionStreamPasser)
            s = sp.GetStream
            If s.ReadSimpleString(4) <> Identifier Then Throw New InvalidDataException

            NumNodeTable = s.ReadInt32
            NodeTable_Types = New NodeTypes(NumNodeTable - 1) {}
            NodeTable_Parameters = New Object(NumNodeTable - 1) {}
            For n = 0 To NumNodeTable - 1
                NodeTable_Types(n) = CType(s.ReadInt32, Global.FileSystem.BSMB.NodeTypes)
                Select Case NodeTable_Types(n)
                    Case NodeTypes.StringLiteral
                        NodeTable_Parameters(n) = s.ReadInt32
                    Case NodeTypes.Array
                        NodeTable_Parameters(n) = s.ReadInt32
                    Case NodeTypes.CandidateList
                        NodeTable_Parameters(n) = s.ReadInt32
                    Case NodeTypes.NumericLiteral
                        NodeTable_Parameters(n) = s.ReadSingle
                    Case NodeTypes.StructureNode
                        NodeTable_Parameters(n) = s.ReadInt32
                    Case Else
                        Throw New InvalidDataException
                End Select
            Next

            NumStructureTable = s.ReadInt32
            StructureTable_NumField = New Int32(NumStructureTable - 1) {}
            StructureTable_FieldStart = New Int32(NumStructureTable - 1) {}
            For n = 0 To NumStructureTable - 1
                StructureTable_NumField(n) = s.ReadInt32
                StructureTable_FieldStart(n) = s.ReadInt32
            Next

            NumFieldTable = s.ReadInt32
            FieldTable_NameAddress = New Int32(NumFieldTable - 1) {}
            FieldTable_ValueIndex = New Int32(NumFieldTable - 1) {}
            For n = 0 To NumFieldTable - 1
                FieldTable_NameAddress(n) = s.ReadInt32
                FieldTable_ValueIndex(n) = s.ReadInt32
            Next

            NumArrayTable = s.ReadInt32
            ArrayTable_NumElement = New Int32(NumArrayTable - 1) {}
            ArrayTable_ElementStart = New Int32(NumArrayTable - 1) {}
            For n = 0 To NumArrayTable - 1
                ArrayTable_NumElement(n) = s.ReadInt32
                ArrayTable_ElementStart(n) = s.ReadInt32
            Next

            NumElementTable = s.ReadInt32
            ElementTable_ElementNodeIndex = New Int32(NumElementTable - 1) {}
            For n = 0 To NumElementTable - 1
                ElementTable_ElementNodeIndex(n) = s.ReadInt32
            Next

            StringIdentifierTableLength = s.ReadInt32
            StringLiteralTableLength = s.ReadInt32
            StringIdentifierTableStart = CInt(s.Position)
            StringLiteralTableStart = CInt(s.Position + StringIdentifierTableLength)

            Root = CType(ReadValue(0), StructureNode)
        End Sub

        Public Function ReadValue(ByVal NodeIndex As Int32) As ValueNode
            Dim Type = NodeTable_Types(NodeIndex)
            Dim Parameter = NodeTable_Parameters(NodeIndex)
            Select Case Type
                Case NodeTypes.StringLiteral
                    Dim Address As Int32 = CInt(Parameter)
                    s.Position = StringLiteralTableStart + Address
                    Return New StringLiteralNode With {.Value = s.ReadString(StringLiteralTableLength - Address, Windows1252)}
                Case NodeTypes.Array
                    Return ReadArray(CInt(Parameter))
                Case NodeTypes.CandidateList
                    Dim l = ReadArray(CInt(Parameter))
                    If l.Elements.Count = 0 Then Throw New InvalidDataException
                    Return l.Elements(l.Elements.Count - 1)
                Case NodeTypes.NumericLiteral
                    Return New NumericLiteralNode With {.Value = CSng(Parameter)}
                Case NodeTypes.StructureNode
                    Return ReadStructure(CInt(Parameter))
                Case Else
                    Throw New InvalidDataException
            End Select
        End Function

        Public Function ReadArray(ByVal ArrayIndex As Int32) As ArrayNode
            Dim n As New ArrayNode
            Dim NumElement = ArrayTable_NumElement(ArrayIndex)
            Dim ElementStart = ArrayTable_ElementStart(ArrayIndex)
            For i = 0 To NumElement - 1
                Dim ElementIndex = ElementStart + i
                n.Elements.Add(ReadValue(ElementTable_ElementNodeIndex(ElementIndex)))
            Next
            Return n
        End Function

        Public Function ReadStructure(ByVal StructureIndex As Int32) As StructureNode
            Dim n As New StructureNode
            Dim NumField = StructureTable_NumField(StructureIndex)
            Dim FieldStart = StructureTable_FieldStart(StructureIndex)
            For i = 0 To NumField - 1
                Dim FieldIndex = FieldStart + i
                Dim NameAddress = FieldTable_NameAddress(FieldIndex)
                Dim ValueIndex = FieldTable_ValueIndex(FieldIndex)
                s.Position = StringIdentifierTableStart + NameAddress
                Dim f As New Field With {.Name = s.ReadString(StringIdentifierTableLength - NameAddress, Windows1252)}
                f.Value = ReadValue(ValueIndex)
                n.Fields.Add(f)
            Next
            Return n
        End Function
    End Class

    Private Class BSMBTextWriter
        Private s As StreamEx
        Private IsNewLine As Boolean
        Private IndentationLevel As Integer
        Private IndentationWritten As Boolean
        Private ScriptLevel As Integer

        Public Sub New(ByVal sp As ZeroLengthStreamPasser)
            s = sp.GetStream
            IsNewLine = True
            IndentationLevel = 0
            IndentationWritten = False
            ScriptLevel = 0
        End Sub

        Private Sub WriteString(ByVal Value As String)
            Dim Bytes = Windows1252.GetBytes(Value)
            s.Write(Bytes)
        End Sub

        Public Sub NewLine()
            If Not IsNewLine Then
                WriteString(CrLf)
                IsNewLine = True
                IndentationWritten = False
            End If
        End Sub

        Public Sub WriteIndentation()
            WriteString(New String(" "c, IndentationLevel * 4))
            IndentationWritten = True
            IsNewLine = False
        End Sub

        Public Sub WriteValue(ByVal Value As ValueNode)
            Dim sln = TryCast(Value, StringLiteralNode)
            If sln IsNot Nothing Then
                If sln.Value = "!" Then
                    If ScriptLevel = 0 Then
                        ScriptLevel += 1
                    ElseIf ScriptLevel = 1 Then
                        NewLine()
                    End If
                End If
                If IndentationWritten Then
                    WriteString(" ")
                Else
                    WriteIndentation()
                End If
                If sln.Value.Contains(" ") Then
                    WriteString("""" & sln.Value & """")
                Else
                    WriteString(sln.Value)
                End If
                Return
            End If
            Dim an = TryCast(Value, ArrayNode)
            If an IsNot Nothing Then
                If ScriptLevel > 0 Then
                    WriteString(" ")
                    WriteString("(")
                    ScriptLevel += 1
                    For Each n In an.Elements
                        WriteValue(n)
                    Next
                    ScriptLevel -= 1
                    WriteString(" ")
                    WriteString(")")
                Else
                    NewLine()
                    WriteIndentation()
                    WriteString("(")
                    IndentationLevel += 1
                    NewLine()
                    For Each n In an.Elements
                        WriteValue(n)
                    Next
                    IndentationLevel -= 1
                    If ScriptLevel > 0 Then ScriptLevel -= 1
                    NewLine()
                    WriteIndentation()
                    WriteString(")")
                    NewLine()
                End If
                Return
            End If
            Dim nln = TryCast(Value, NumericLiteralNode)
            If nln IsNot Nothing Then
                If IndentationWritten Then
                    WriteString(" ")
                Else
                    WriteIndentation()
                End If
                WriteString(nln.Value.ToInvariantString())
                Return
            End If
            Dim sn = TryCast(Value, StructureNode)
            If sn IsNot Nothing Then
                NewLine()
                WriteIndentation()
                WriteString("[")
                IndentationLevel += 1
                NewLine()
                For Each n In sn.Fields
                    NewLine()
                    WriteIndentation()
                    WriteString("." & n.Name)
                    WriteValue(n.Value)
                Next
                IndentationLevel -= 1
                NewLine()
                WriteIndentation()
                WriteString("]")
                NewLine()
                Return
            End If
        End Sub
    End Class

    Public Shared Function ReadFromFile(ByVal sp As ZeroPositionStreamPasser) As ValueNode
        Dim r As New BSMBReader(sp)
        Return r.Root
    End Function

    Public Shared Function ReadFromFile(ByVal Path As String) As ValueNode
        Using s As New StreamEx(Path, FileMode.Open, FileAccess.Read)
            Return BSMB.ReadFromFile(s)
        End Using
    End Function

    Public Shared Sub WriteToFile(ByVal sp As ZeroLengthStreamPasser, ByVal Value As ValueNode)
        Dim w As New BSMBTextWriter(sp)
        w.WriteValue(Value)
    End Sub

    Public Shared Sub WriteToFile(ByVal Path As String, ByVal Value As ValueNode)
        Using s As New StreamEx
            WriteToFile(s, Value)
            s.Position = 0
            Using so As New StreamEx(Path, FileMode.Create, FileAccess.ReadWrite)
                so.WriteFromStream(s, s.Length)
            End Using
        End Using
    End Sub

    Public MustInherit Class ValueNode
    End Class

    Public Class NumericLiteralNode
        Inherits ValueNode
        Public Value As Single
    End Class

    Public Class StringLiteralNode
        Inherits ValueNode
        Public Value As String
    End Class

    Public Class ArrayNode
        Inherits ValueNode
        Public Elements As New List(Of ValueNode)
    End Class

    Public Class Field
        Public Name As String
        Public Value As ValueNode
    End Class

    Public Class StructureNode
        Inherits ValueNode
        Public Fields As New List(Of Field)
    End Class
End Class