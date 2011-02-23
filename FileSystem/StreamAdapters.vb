'==========================================================================
'
'  File:        StreamAdapters.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流适配器类
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.IO

''' <summary>扩展流适配器类</summary>
''' <remarks>用于安全保存StreamEx的Stream形式。</remarks>
Public Class StreamAdapter
    Inherits Stream
    Protected Friend BaseStream As StreamEx

    Public Sub New(ByVal s As StreamEx)
        BaseStream = s
    End Sub
    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return BaseStream.CanRead
        End Get
    End Property
    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return BaseStream.CanSeek
        End Get
    End Property
    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return BaseStream.CanWrite
        End Get
    End Property
    Public Overrides Sub Flush()
        BaseStream.Flush()
    End Sub
    Public Overrides ReadOnly Property Length() As Int64
        Get
            Return BaseStream.Length
        End Get
    End Property
    Public Overrides Property Position() As Int64
        Get
            Return BaseStream.Position
        End Get
        Set(ByVal Value As Int64)
            BaseStream.Position = Value
        End Set
    End Property
    Public Overrides Function Seek(ByVal Offset As Int64, ByVal Origin As System.IO.SeekOrigin) As Int64
        Return BaseStream.Seek(Offset, Origin)
    End Function
    Public Overrides Sub SetLength(ByVal Value As Int64)
        BaseStream.SetLength(Value)
    End Sub
    Public Overrides Function ReadByte() As Integer
        Return BaseStream.ReadByte()
    End Function
    Public Overrides Sub WriteByte(ByVal Value As Byte)
        BaseStream.WriteByte(Value)
    End Sub
    Public Overrides Function Read(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) As Integer
        BaseStream.Read(Buffer, Offset, Count)
        Return Count
    End Function
    Public Overrides Sub Write(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer)
        BaseStream.Write(Buffer, Offset, Count)
    End Sub
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If BaseStream IsNot Nothing Then
            BaseStream.Dispose()
            BaseStream = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class

''' <summary>扩展流适配器类-适配非安全流</summary>
''' <remarks>用于安全保存StreamEx的Stream形式。</remarks>
Public Class UnsafeStreamAdapter
    Inherits Stream
    Protected Friend BaseStream As StreamEx

    Public Sub New(ByVal s As StreamEx)
        BaseStream = s
    End Sub
    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return BaseStream.CanRead
        End Get
    End Property
    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return BaseStream.CanSeek
        End Get
    End Property
    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return BaseStream.CanWrite
        End Get
    End Property
    Public Overrides Sub Flush()
        BaseStream.Flush()
    End Sub
    Public Overrides ReadOnly Property Length() As Int64
        Get
            Return BaseStream.Length
        End Get
    End Property
    Public Overrides Property Position() As Int64
        Get
            Return BaseStream.Position
        End Get
        Set(ByVal Value As Int64)
            BaseStream.Position = Value
        End Set
    End Property
    Public Overrides Function Seek(ByVal Offset As Int64, ByVal Origin As System.IO.SeekOrigin) As Int64
        Return BaseStream.Seek(Offset, Origin)
    End Function
    Public Overrides Sub SetLength(ByVal Value As Int64)
        BaseStream.SetLength(Value)
    End Sub
    Public Overrides Function ReadByte() As Integer
        Try
            Return BaseStream.ReadByte()
        Catch ex As EndOfStreamException
            Return -1
        End Try
    End Function
    Public Overrides Sub WriteByte(ByVal Value As Byte)
        BaseStream.WriteByte(Value)
    End Sub
    Public Overrides Function Read(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) As Integer
        If BaseStream.Position >= BaseStream.Length Then
            Return 0
        ElseIf BaseStream.Position + Count > BaseStream.Length Then
            Dim NewCount = CInt(BaseStream.Length - BaseStream.Position)
            BaseStream.Read(Buffer, Offset, NewCount)
            Return NewCount
        Else
            BaseStream.Read(Buffer, Offset, Count)
            Return Count
        End If
    End Function
    Public Overrides Sub Write(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer)
        BaseStream.Write(Buffer, Offset, Count)
    End Sub
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If BaseStream IsNot Nothing Then
            BaseStream.Dispose()
            BaseStream = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class
