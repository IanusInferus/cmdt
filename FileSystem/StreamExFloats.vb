'==========================================================================
'
'  File:        StreamExFloats.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流类 - 浮点数
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
    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)>
    Private Structure SingleInt32
        <System.Runtime.InteropServices.FieldOffset(0)>
        Public Float32Value As Single
        <System.Runtime.InteropServices.FieldOffset(0)>
        Public Int32Value As Int32
    End Structure
    ''' <summary>读取单精度浮点数。</summary>
    Public Function ReadFloat32() As Single
        Dim a As SingleInt32
        a.Int32Value = ReadInt32()
        Return a.Float32Value
    End Function
    ''' <summary>写入单精度浮点数。</summary>
    Public Sub WriteFloat32(ByVal f As Single)
        Dim a As SingleInt32
        a.Float32Value = f
        WriteInt32(a.Int32Value)
    End Sub

    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)>
    Private Structure DoubleInt64
        <System.Runtime.InteropServices.FieldOffset(0)>
        Public Float64Value As Double
        <System.Runtime.InteropServices.FieldOffset(0)>
        Public Int64Value As Int64
    End Structure
    ''' <summary>读取双精度浮点数。</summary>
    Public Function ReadFloat64() As Double
        Dim a As DoubleInt64
        a.Int64Value = ReadInt64()
        Return a.Float64Value
    End Function
    ''' <summary>写入双精度浮点数。</summary>
    Public Sub WriteFloat64(ByVal f As Double)
        Dim a As DoubleInt64
        a.Float64Value = f
        WriteInt64(a.Int64Value)
    End Sub

    ''' <summary>读取单精度浮点数，高位优先字节序。</summary>
    Public Function ReadFloat32B() As Single
        Dim a As SingleInt32
        a.Int32Value = ReadInt32B()
        Return a.Float32Value
    End Function
    ''' <summary>写入单精度浮点数，高位优先字节序。</summary>
    Public Sub WriteFloat32B(ByVal f As Single)
        Dim a As SingleInt32
        a.Float32Value = f
        WriteInt32B(a.Int32Value)
    End Sub
    ''' <summary>读取双精度浮点数，高位优先字节序。</summary>
    Public Function ReadFloat64B() As Double
        Dim a As DoubleInt64
        a.Int64Value = ReadInt64B()
        Return a.Float64Value
    End Function
    ''' <summary>写入双精度浮点数，高位优先字节序。</summary>
    Public Sub WriteFloat64B(ByVal f As Double)
        Dim a As DoubleInt64
        a.Float64Value = f
        WriteInt64B(a.Int64Value)
    End Sub

    ''' <summary>读取单精度浮点数。</summary>
    Public Function ReadSingle() As Single
        Return ReadFloat32()
    End Function
    ''' <summary>写入单精度浮点数。</summary>
    Public Sub WriteSingle(ByVal f As Single)
        WriteFloat32(f)
    End Sub
    ''' <summary>读取双精度浮点数。</summary>
    Public Function ReadDouble() As Double
        Return ReadFloat64()
    End Function
    ''' <summary>写入双精度浮点数。</summary>
    Public Sub WriteDouble(ByVal f As Double)
        WriteFloat64(f)
    End Sub
End Class
