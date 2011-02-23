'==========================================================================
'
'  File:        StreamExPasser.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: 扩展流传递器，用于显式确定函数传参时的流是否包含长度位置信息。
'  Version:     2011.02.23.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.IO

''' <summary>零长度零位置扩展流传递器。用于保证在函数传参时传递零长度零位置的流。</summary>
Public Class ZeroLengthStreamPasser
    Protected BaseStream As StreamEx

    Public Sub New(ByVal s As StreamEx)
        If s.Length <> 0 Then Throw New ArgumentException("LengthNotZero")
        If s.Position <> 0 Then Throw New ArgumentException("PositionNotZero")
        BaseStream = s
    End Sub

    Public Function GetStream() As StreamEx
        If BaseStream.Length <> 0 Then Throw New ArgumentException("LengthNotZero")
        If BaseStream.Position <> 0 Then Throw New ArgumentException("PositionNotZero")
        Return BaseStream
    End Function

    Public Shared Widening Operator CType(ByVal s As StreamEx) As ZeroLengthStreamPasser
        Return New ZeroLengthStreamPasser(s)
    End Operator

    Public Shared Widening Operator CType(ByVal p As ZeroLengthStreamPasser) As StreamEx
        Return p.GetStream()
    End Operator
End Class

''' <summary>零位置扩展流传递器。用于保证在函数传参时传递零位置的流。</summary>
Public Class ZeroPositionStreamPasser
    Protected BaseStream As StreamEx

    Public Sub New(ByVal s As StreamEx)
        If s.Position <> 0 Then Throw New ArgumentException("PositionNotZero")
        BaseStream = s
    End Sub

    Public Function GetStream() As StreamEx
        If BaseStream.Position <> 0 Then Throw New ArgumentException("PositionNotZero")
        Return BaseStream
    End Function

    Public Shared Widening Operator CType(ByVal s As StreamEx) As ZeroPositionStreamPasser
        Return New ZeroPositionStreamPasser(s)
    End Operator

    Public Shared Widening Operator CType(ByVal p As ZeroPositionStreamPasser) As StreamEx
        Return p.GetStream()
    End Operator

    Public Shared Widening Operator CType(ByVal p As ZeroLengthStreamPasser) As ZeroPositionStreamPasser
        Return New ZeroPositionStreamPasser(p.GetStream())
    End Operator
End Class

''' <summary>有位置的扩展流传递器。用于显式申明函数传参时传递的流有位置信息。</summary>
Public Class PositionedStreamPasser
    Protected BaseStream As StreamEx

    Public Sub New(ByVal s As StreamEx)
        BaseStream = s
    End Sub

    Public Function GetStream() As StreamEx
        Return BaseStream
    End Function

    Public Shared Widening Operator CType(ByVal s As StreamEx) As PositionedStreamPasser
        Return New PositionedStreamPasser(s)
    End Operator

    Public Shared Widening Operator CType(ByVal p As PositionedStreamPasser) As StreamEx
        Return p.GetStream()
    End Operator
End Class
