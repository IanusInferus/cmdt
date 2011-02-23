'==========================================================================
'
'  File:        StreamEx.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流类
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.IO
Imports Firefly
Imports Firefly.Streaming

''' <summary>
''' 扩展流类
''' </summary>
''' <remarks>
''' 请显式调用Close或Dispose来关闭流。
''' 如果调用了ToStream或转换到了Stream，并放弃了StreamEx，StreamEx也不会消失，因为使用了一个继承自Stream的Adapter来持有StreamEx的引用。
''' 本类与System.IO.StreamReader等类不兼容。这些类使用了ReadByte返回的结束标志-1等。本类会在位置超过文件长度时读取会抛出异常。
''' 本类主要用于封装System.IO.MemoryStream和System.IO.FileStream，对其他流可能抛出无法预期的异常。
''' 一切的异常都由调用者来处理。
''' </remarks>
Partial Public Class StreamEx
    Implements IStream, IDisposable
    Protected BaseStream As Stream

    ''' <summary>已重载。初始化新实例。</summary>
    Public Sub New()
        BaseStream = New MemoryStream
    End Sub
    ''' <summary>已重载。初始化新实例。</summary>
    Public Sub New(ByVal Path As String, ByVal Mode As FileMode, ByVal Access As FileAccess, ByVal Share As FileShare)
        BaseStream = New FileStream(Path, Mode, Access, Share)
    End Sub
    ''' <summary>已重载。初始化新实例。</summary>
    Public Sub New(ByVal Path As String, ByVal Mode As FileMode, Optional ByVal Access As FileAccess = FileAccess.ReadWrite)
        BaseStream = New FileStream(Path, Mode, Access, FileShare.Read)
    End Sub
    ''' <summary>已重载。初始化新实例。</summary>
    Public Sub New(ByVal BaseStream As Stream)
        Me.BaseStream = BaseStream
    End Sub
    Public Shared Widening Operator CType(ByVal s As Stream) As StreamEx
        Dim sa = TryCast(s, StreamAdapter)
        If sa IsNot Nothing Then Return sa.BaseStream
        Return New StreamEx(s)
    End Operator
    Public Shared Widening Operator CType(ByVal s As StreamEx) As Stream
        Return New StreamAdapter(s)
    End Operator
    Public Function ToStream() As Stream
        Return New StreamAdapter(Me)
    End Function
    Public Function ToUnsafeStream() As Stream
        Return New UnsafeStreamAdapter(Me)
    End Function

    ''' <summary>指示当前流是否支持读取。</summary>
    Public Overridable ReadOnly Property CanRead() As Boolean
        Get
            Return BaseStream.CanRead
        End Get
    End Property
    ''' <summary>指示当前流是否支持定位。</summary>
    Public Overridable ReadOnly Property CanSeek() As Boolean
        Get
            Return BaseStream.CanSeek
        End Get
    End Property
    ''' <summary>指示当前流是否支持写入。</summary>
    Public Overridable ReadOnly Property CanWrite() As Boolean
        Get
            Return BaseStream.CanWrite
        End Get
    End Property
    ''' <summary>强制同步缓冲数据。</summary>
    Public Overridable Sub Flush() Implements IFlushable.Flush
        BaseStream.Flush()
    End Sub
    ''' <summary>关闭流。</summary>
    ''' <remarks>对继承者的说明：该方法调用Dispose()，不要覆盖该方法，而应覆盖Dispose(Boolean)</remarks>
    Public Overridable Sub Close()
        Static Closed As Boolean = False
        If Closed Then Throw New InvalidOperationException
        Dispose()
        Closed = True
    End Sub
    ''' <summary>用字节表示的流的长度。</summary>
    Public Overridable ReadOnly Property Length() As Int64 Implements ISeekableStream.Length
        Get
            Return BaseStream.Length
        End Get
    End Property
    ''' <summary>流的当前位置。</summary>
    Public Overridable Property Position() As Int64 Implements ISeekableStream.Position
        Get
            Return BaseStream.Position
        End Get
        Set(ByVal Value As Int64)
            BaseStream.Position = Value
        End Set
    End Property
    ''' <summary>设置流的当前位置。</summary>
    Public Function Seek(ByVal Offset As Int64, ByVal Origin As System.IO.SeekOrigin) As Int64
        Select Case Origin
            Case SeekOrigin.Begin
                Position = Offset
            Case SeekOrigin.Current
                Position += Offset
            Case SeekOrigin.End
                Position = Length - Offset
        End Select
        Return Position
    End Function
    ''' <summary>设置流的长度。</summary>
    Public Overridable Sub SetLength(ByVal Value As Int64) Implements IResizableStream.SetLength
        BaseStream.SetLength(Value)
    End Sub

    ''' <summary>读取Byte。</summary>
    Public Overridable Function ReadByte() As Byte Implements IReadableStream.ReadByte
        Dim b As Integer = BaseStream.ReadByte
        If b = -1 Then Throw New EndOfStreamException
        Return CByte(b)
    End Function
    ''' <summary>写入Byte。</summary>
    Public Overridable Sub WriteByte(ByVal b As Byte) Implements IWritableStream.WriteByte
        BaseStream.WriteByte(b)
    End Sub

    ''' <summary>已重载。读取到字节数组。</summary>
    ''' <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据。</param>
    Public Overridable Sub Read(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) Implements IReadableStream.Read
        Dim c As Integer = BaseStream.Read(Buffer, Offset, Count)
        If c <> Count Then Throw New EndOfStreamException
    End Sub
    ''' <summary>已重载。写入字节数组。</summary>
    ''' <param name="Offset">Buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前流。</param>
    Public Overridable Sub Write(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) Implements IWritableStream.Write
        BaseStream.Write(Buffer, Offset, Count)
    End Sub

#Region " IDisposable 支持 "
    ''' <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
    Protected Overridable Sub DisposeManagedResource()
        If BaseStream IsNot Nothing Then
            BaseStream.Dispose()
            BaseStream = Nothing
        End If
    End Sub

    ''' <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
    Protected Overridable Sub DisposeUnmanagedResource()
    End Sub

    '检测冗余的调用
    Private DisposedValue As Boolean = False
    ''' <summary>释放流的资源。请优先覆盖DisposeManagedResource、DisposeUnmanagedResource、DisposeNullify方法。如果你直接保存非托管对象(Handle等)，请覆盖Finalize方法，并在其中调用Dispose(False)。</summary>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If DisposedValue Then Return
        DisposedValue = True
        If disposing Then
            DisposeManagedResource()
        End If
        DisposeUnmanagedResource()
    End Sub

    ''' <summary>释放流的资源。</summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    ''' <summary>析构。</summary>
    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub
#End Region

End Class
