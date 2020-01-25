'==========================================================================
'
'  File:        Program.vb
'  Location:    PCKManagerHD <Visual Basic .Net>
'  Description: PCK文件管理器HD
'  Version:     2020.01.25.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.IO
Imports System.Windows.Forms
Imports System.Diagnostics
Imports Firefly
Imports Firefly.Packaging
Imports Firefly.GUI
Imports FileSystem

Public Module Program

    Public Sub Application_ThreadException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        ExceptionHandler.PopupException(e.Exception, New StackTrace(4, True))
    End Sub

    <STAThread()>
    Public Function Main() As Integer
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        Try
            AddHandler Application.ThreadException, AddressOf Application_ThreadException
            Return MainWindow()
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, If(Threading.Thread.CurrentThread.CurrentCulture.Name = "zh-CN", "发生以下异常:", "ExceptionOccurred:"), "PackageManager")
            Return -1
        Finally
            RemoveHandler Application.ThreadException, AddressOf Application_ThreadException
        End Try
    End Function

    Public Function MainWindow() As Integer
        '在这里添加所有需要的文件包类型
        PackageRegister.Register(PCK_HD.Filter, AddressOf PCK_HD.Open)

        Application.EnableVisualStyles()
        Application.Run(New GUI.PackageManager())

        Return 0
    End Function
End Module
