'==========================================================================
'
'  File:        StreamExInts.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流类 - 整数
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports Firefly

Partial Public Class StreamEx
    ''' <summary>查看Byte。</summary>
    Public Function PeekByte() As Byte
        Dim HoldPosition = Position
        Try
            Return ReadByte()
        Finally
            Position = HoldPosition
        End Try
    End Function

    ''' <summary>读取Int8。</summary>
    Public Function ReadInt8() As SByte
        Return CUS(ReadByte())
    End Function
    ''' <summary>读取Int16。</summary>
    Public Function ReadInt16() As Int16
        Dim o As Int16
        o = CShort(ReadByte())
        o = o Or (CShort(ReadByte()) << 8)
        Return o
    End Function
    ''' <summary>读取Int32。</summary>
    Public Function ReadInt32() As Int32
        Dim o As Int32
        o = ReadByte()
        o = o Or (CInt(ReadByte()) << 8)
        o = o Or (CInt(ReadByte()) << 16)
        o = o Or (CInt(ReadByte()) << 24)
        Return o
    End Function
    ''' <summary>读取Int64。</summary>
    Public Function ReadInt64() As Int64
        Dim o As Int64
        o = ReadByte()
        o = o Or (CLng(ReadByte()) << 8)
        o = o Or (CLng(ReadByte()) << 16)
        o = o Or (CLng(ReadByte()) << 24)
        o = o Or (CLng(ReadByte()) << 32)
        o = o Or (CLng(ReadByte()) << 40)
        o = o Or (CLng(ReadByte()) << 48)
        o = o Or (CLng(ReadByte()) << 56)
        Return o
    End Function
    ''' <summary>读取Int16，高位优先字节序。</summary>
    Public Function ReadInt16B() As Int16
        Dim o As Int16
        o = CShort(ReadByte()) << 8
        o = o Or CShort(ReadByte())
        Return o
    End Function
    ''' <summary>读取Int32，高位优先字节序。</summary>
    Public Function ReadInt32B() As Int32
        Dim o As Int32
        o = CInt(ReadByte()) << 24
        o = o Or (CInt(ReadByte()) << 16)
        o = o Or (CInt(ReadByte()) << 8)
        o = o Or CInt(ReadByte())
        Return o
    End Function
    ''' <summary>读取Int64，高位优先字节序。</summary>
    Public Function ReadInt64B() As Int64
        Dim o As Int64
        o = CLng(ReadByte()) << 56
        o = o Or (CLng(ReadByte()) << 48)
        o = o Or (CLng(ReadByte()) << 40)
        o = o Or (CLng(ReadByte()) << 32)
        o = o Or (CLng(ReadByte()) << 24)
        o = o Or (CLng(ReadByte()) << 16)
        o = o Or (CLng(ReadByte()) << 8)
        o = o Or CLng(ReadByte())
        Return o
    End Function

    ''' <summary>写入Int8。</summary>
    Public Sub WriteInt8(ByVal i As SByte)
        WriteByte(CSU(i))
    End Sub
    ''' <summary>写入Int16。</summary>
    Public Sub WriteInt16(ByVal i As Int16)
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
    End Sub
    ''' <summary>写入Int32。</summary>
    Public Sub WriteInt32(ByVal i As Int32)
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
    End Sub
    ''' <summary>写入Int64。</summary>
    Public Sub WriteInt64(ByVal i As Int64)
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
        i = i >> 8
        WriteByte(CByte(i And &HFF))
    End Sub
    ''' <summary>写入Int16，高位优先字节序。</summary>
    Public Sub WriteInt16B(ByVal i As Int16)
        WriteByte(CByte(CSU(i) >> 8 And &HFF))
        WriteByte(CByte(i And &HFF))
    End Sub
    ''' <summary>写入Int32，高位优先字节序。</summary>
    Public Sub WriteInt32B(ByVal i As Int32)
        WriteByte(CByte((CSU(i) >> 24) And &HFF))
        WriteByte(CByte((CSU(i) >> 16) And &HFF))
        WriteByte(CByte((CSU(i) >> 8) And &HFF))
        WriteByte(CByte(i And &HFF))
    End Sub
    ''' <summary>写入Int64，高位优先字节序。</summary>
    Public Sub WriteInt64B(ByVal i As Int64)
        WriteByte(CByte(CLng(CSU(i) >> 56) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 48) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 40) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 32) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 24) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 16) And &HFF))
        WriteByte(CByte(CLng(CSU(i) >> 8) And &HFF))
        WriteByte(CByte(i And &HFF))
    End Sub

    ''' <summary>查看Int8。</summary>
    Public Function PeekInt8() As SByte
        Dim HoldPosition = Position
        Try
            Return ReadInt8()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int16。</summary>
    Public Function PeekInt16() As Int16
        Dim HoldPosition = Position
        Try
            Return ReadInt16()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int32。</summary>
    Public Function PeekInt32() As Int32
        Dim HoldPosition = Position
        Try
            Return ReadInt32()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int64。</summary>
    Public Function PeekInt64() As Int64
        Dim HoldPosition = Position
        Try
            Return ReadInt64()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int16，高位优先字节序。</summary>
    Public Function PeekInt16B() As Int16
        Dim HoldPosition = Position
        Try
            Return ReadInt16B()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int32，高位优先字节序。</summary>
    Public Function PeekInt32B() As Int32
        Dim HoldPosition = Position
        Try
            Return ReadInt32B()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看Int64，高位优先字节序。</summary>
    Public Function PeekInt64B() As Int64
        Dim HoldPosition = Position
        Try
            Return ReadInt64B()
        Finally
            Position = HoldPosition
        End Try
    End Function

    ''' <summary>读取UInt8。</summary>
    Public Function ReadUInt8() As Byte
        Return ReadByte()
    End Function
    ''' <summary>读取UInt16。</summary>
    Public Function ReadUInt16() As UInt16
        Return CSU(ReadInt16)
    End Function
    ''' <summary>读取UInt32。</summary>
    Public Function ReadUInt32() As UInt32
        Return CSU(ReadInt32)
    End Function
    ''' <summary>读取UInt64。</summary>
    Public Function ReadUInt64() As UInt64
        Return CSU(ReadInt64)
    End Function
    ''' <summary>读取UInt16，高位优先字节序。</summary>
    Public Function ReadUInt16B() As UInt16
        Return CSU(ReadInt16B)
    End Function
    ''' <summary>读取UInt32，高位优先字节序。</summary>
    Public Function ReadUInt32B() As UInt32
        Return CSU(ReadInt32B)
    End Function
    ''' <summary>读取UInt64，高位优先字节序。</summary>
    Public Function ReadUInt64B() As UInt64
        Return CSU(ReadInt64B)
    End Function

    ''' <summary>写入UInt8。</summary>
    Public Sub WriteUInt8(ByVal b As Byte)
        WriteByte(b)
    End Sub
    ''' <summary>写入UInt16。</summary>
    Public Sub WriteUInt16(ByVal i As UInt16)
        WriteInt16(CUS(i))
    End Sub
    ''' <summary>写入UInt32。</summary>
    Public Sub WriteUInt32(ByVal i As UInt32)
        WriteInt32(CUS(i))
    End Sub
    ''' <summary>写入UInt64。</summary>
    Public Sub WriteUInt64(ByVal i As UInt64)
        WriteInt64(CUS(i))
    End Sub
    ''' <summary>写入UInt16，高位优先字节序。</summary>
    Public Sub WriteUInt16B(ByVal i As UInt16)
        WriteInt16B(CUS(i))
    End Sub
    ''' <summary>写入UInt32，高位优先字节序。</summary>
    Public Sub WriteUInt32B(ByVal i As UInt32)
        WriteInt32B(CUS(i))
    End Sub
    ''' <summary>写入UInt64，高位优先字节序。</summary>
    Public Sub WriteUInt64B(ByVal i As UInt64)
        WriteInt64B(CUS(i))
    End Sub

    ''' <summary>查看UInt8。</summary>
    Public Function PeekUInt8() As Byte
        Dim HoldPosition = Position
        Try
            Return ReadUInt8()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt16。</summary>
    Public Function PeekUInt16() As UInt16
        Dim HoldPosition = Position
        Try
            Return ReadUInt16()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt32。</summary>
    Public Function PeekUInt32() As UInt32
        Dim HoldPosition = Position
        Try
            Return ReadUInt32()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt64。</summary>
    Public Function PeekUInt64() As UInt64
        Dim HoldPosition = Position
        Try
            Return ReadUInt64()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt16，高位优先字节序。</summary>
    Public Function PeekUInt16B() As UInt16
        Dim HoldPosition = Position
        Try
            Return ReadUInt16B()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt32，高位优先字节序。</summary>
    Public Function PeekUInt32B() As UInt32
        Dim HoldPosition = Position
        Try
            Return ReadUInt32B()
        Finally
            Position = HoldPosition
        End Try
    End Function
    ''' <summary>查看UInt64，高位优先字节序。</summary>
    Public Function PeekUInt64B() As UInt64
        Dim HoldPosition = Position
        Try
            Return ReadUInt64B()
        Finally
            Position = HoldPosition
        End Try
    End Function
End Class
