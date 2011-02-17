'==========================================================================
'
'  File:        DeBsmb.vb
'  Location:    DeBsmb <Visual Basic .Net>
'  Description: BSMB文本解码器壳
'  Version:     2011.02.17.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Firefly
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem

Public Module DeBsmb

#Region " 全球化 "
    Private Title As String
    Private DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过下面的网址联系我。" & Environment.NewLine & "是否将网址和具体的错误信息复制到剪贴板？"
    Private About As String

    Sub LoadLan()
        Dim LRes As New INILocalization("Lan\DeBsmb", "")

        LRes.ReadValue("Title", Title)
        LRes.ReadValue("DebugTip", DebugTip)
        LRes.ReadValue("About", About)
    End Sub
#End Region

    Sub Main(ByVal argv As String())
        IO.Directory.SetCurrentDirectory(Application.StartupPath)
        LoadLan()
        If argv Is Nothing OrElse argv.Length = 0 Then
            MsgBox(About)
            Return
        End If
#If CONFIG <> "Debug" Then
        Try
#End If
        If argv.Length = 0 Then
            MsgBox(About, MsgBoxStyle.OkOnly, Title)
            Return
        End If
        For Each f In argv
            Using s As New StreamEx(f, FileMode.Open, FileAccess.Read)
                If s.Length < 4 OrElse s.ReadSimpleString(4) <> BSMB.Identifier Then
                    Continue For
                End If
            End Using
            Dim b = BSMB.ReadFromFile(f)
            BSMB.WriteToFile(f, b)
        Next
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
    End Sub
End Module
