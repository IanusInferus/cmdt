'==========================================================================
'
'  File:        MultiConverter.vb
'  Location:    MultiConverter <Visual Basic .Net>
'  Description: 万用文件转换器
'  Version:     2013.01.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports System
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem

Public Class MultiConverter

#Region " 设置和全球化 "
    Private Opt As INI
    Private LanFull As String
    Private INISettingNotice As String = "万用文件管理器初始化配置文件" & Environment.NewLine & "在不了解此文件用法的时候请不要编辑此文件。"
    Private Shared Title As String = "万用文件管理器"
    Private Shared DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过Readme.zh.txt中的邮箱或网址联系我。"

    Private Sub MultiConverter_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        IO.Directory.SetCurrentDirectory(Application.StartupPath)
        If Not IO.File.Exists("..\Ini\MultiConverter.ini") Then
            LoadOptDefault()
        Else
            LoadOpt()
        End If
        LoadLan()
        Main()
    End Sub
    Private Sub MultiConverter_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        SaveOpt()
        Main()
        End
    End Sub

    Sub LoadOptDefault()
        Opt = New INI("..\Ini\MultiConverter.ini")
    End Sub
    Sub LoadOpt()
        Opt = New INI("..\Ini\MultiConverter.ini")

        LanFull = ""
        Opt.ReadValue("Option", "CurrentCulture", LanFull)
        Opt.ReadValue("Option", "Comm2Directory", TextBox_Comm2Directory.Text)
        Opt.ReadValue("Option", "Comm3Directory", TextBox_Comm3Directory.Text)
    End Sub
    Sub SaveOpt()
        Opt.WriteValue("Option", "CurrentCulture", LanFull)
        Opt.WriteValue("Option", "Comm2Directory", TextBox_Comm2Directory.Text, False)
        Opt.WriteValue("Option", "Comm3Directory", TextBox_Comm3Directory.Text, False)

        Opt.WriteToFile("/*" & Environment.NewLine & INISettingNotice & Environment.NewLine & "*/" & Environment.NewLine)
    End Sub
    Sub LoadLan()
        Dim LRes As New INILocalization("Lan\MultiConverter", LanFull)
        LanFull = LRes.LanguageIndentiySignFull

        Dim Font As String = ""
        LRes.ReadValue("Font", Font)
        If Font <> "" Then Me.Font = New Font(Font, 9, FontStyle.Regular, GraphicsUnit.Point)
        LRes.ReadValue("Title", Title)
        Me.Text = Title
        LRes.ReadValue("DebugTip", DebugTip)

        LRes.ReadValue("Label_Description", Label_Description.Text)
        LRes.ReadValue("Label_SetSetupDirectories", Label_SetSetupDirectories.Text)
        LRes.ReadValue("Label_Comm2Directory", Label_Comm2Directory.Text)
        LRes.ReadValue("Label_Comm3Directory", Label_Comm3Directory.Text)
        LRes.ReadValue("Button_OK", Button_OK.Text)

        LRes.ReadValue("INISettingNotice", INISettingNotice)
    End Sub
    Sub MsgBox(ByVal ex As Exception)
        Dim r As MsgBoxResult = Microsoft.VisualBasic.MsgBox(DebugTip & Environment.NewLine & Environment.NewLine & "http://www.cnblogs.com/Rex/Contact.aspx?id=1" & Environment.NewLine & Environment.NewLine & ex.ToString, MsgBoxStyle.Critical Or MsgBoxStyle.YesNo, Title)
        If r = MsgBoxResult.Yes Then
            My.Computer.Clipboard.SetText("http://www.cnblogs.com/Rex/Contact.aspx?id=1" & Environment.NewLine & Environment.NewLine & ex.ToString)
        End If
    End Sub
#End Region

    Private Sub Main()
        Dim argv As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
        If argv.Count = 0 Then Return
        If Not Directory.Exists(TextBox_Comm2Directory.Text) Then Return
        If Not Directory.Exists(TextBox_Comm3Directory.Text) Then Return

        Dim SrcFile As String = Nothing
        Dim TarFile As String = Nothing
        Dim Parameter As String = Nothing
        For Each v As String In argv
            If Parameter = "" AndAlso v.StartsWith("-p") Then
                Parameter = v.Substring(2)
                Continue For
            End If
            If SrcFile = "" Then
                SrcFile = v
                Continue For
            End If
            If TarFile = "" Then
                TarFile = v
                Continue For
            End If
        Next

        If SrcFile = "" Then Return
        If TarFile = "" Then Return

        Dim SrcPath As String = GetPath(TextBox_Comm3Directory.Text, SrcFile)
        Dim TarPath As String = GetPath(TextBox_Comm2Directory.Text, TarFile)

        If Not File.Exists(SrcPath) Then
            SrcPath = GetPath(TextBox_Comm2Directory.Text, TarFile)
            Dim PCK As PCK = PCK.Open(GetPath(TextBox_Comm3Directory.Text, "DATA.PCK"), PCK.Version.Comm3)
            Dim f As PCK.FileDB = PCK.TryGetFileDB(SrcFile)
            PCK.Extract(f, GetFileDirectory(SrcPath))
            PCK.Close()
        End If

#If CONFIG <> "Debug" Then
        Try
#End If
        Dim TempDir As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\CommDevToolkitTemp"

        If IsMatchFileMask(SrcPath, "*.Y64") Then
            Dim ReplacePalette As Boolean
            Dim LightnessFactor As Double
            Dim SaturationFactor As Double

            If Parameter <> "" Then
                Dim p As String() = Parameter.Split(",")
                If p(0) = "0" Then
                    ReplacePalette = False
                ElseIf p(0) = "1" Then
                    ReplacePalette = True
                Else
                    Throw New ArgumentOutOfRangeException("<ReplacePalette>")
                End If
                If Not Double.TryParse(p(1), LightnessFactor) OrElse LightnessFactor < -5 OrElse LightnessFactor > 5 Then
                    Throw New ArgumentOutOfRangeException("<LightnessFactor>")
                End If
                If Not Double.TryParse(p(1), SaturationFactor) OrElse SaturationFactor < -5 OrElse SaturationFactor > 5 Then
                    Throw New ArgumentOutOfRangeException("<SaturationFactor>")
                End If
            End If

            Y64.Convert(SrcPath, TarPath, Y64.Version.Comm2Version2, ReplacePalette, LightnessFactor, SaturationFactor)

        ElseIf IsMatchFileMask(SrcPath, "*.SEC") Then
            Dim f As New SEC(SrcPath)
            f.WriteToFile(TarPath, SEC.Version.Comm2)
        ElseIf IsMatchFileMask(SrcPath, "*.GRL") Then
            Dim f As New GRL(SrcPath)
            f.VersionSign = GRL.Version.Comm2
            f.WriteToFile(TarPath)
        ElseIf IsMatchFileMask(SrcPath, "*.MBI") Then
            Dim f As MBI = MBI.Open(SrcPath)
            If f.VersionSign = MBI.Version.Comm3 Then f.RemoveComm3Feature()
            f.WriteToFile(TarPath, MBI.Version.Comm2)
        Else
            If SrcPath <> TarPath Then
                If Not Directory.Exists(GetFileDirectory(TarPath)) Then Directory.CreateDirectory(GetFileDirectory(TarPath))
                IO.File.Copy(SrcPath, TarPath)
            End If
        End If
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
        End
    End Sub

    Private Sub Button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_OK.Click
        SaveOpt()
        Main()
        End
    End Sub

    Private Sub Button_Comm2Directory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Comm2Directory.Click
        With TextBox_Comm2Directory
            Static d As Windows.Forms.FolderBrowserDialog
            If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.SelectedPath = dir
            End If
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.SelectedPath, Application.StartupPath)
                If T <> "" AndAlso d.SelectedPath <> "" And T.Length < d.SelectedPath.Length Then
                    .Text = T
                Else
                    .Text = d.SelectedPath
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub

    Private Sub Button_Comm3Directory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Comm3Directory.Click
        With TextBox_Comm3Directory
            Static d As Windows.Forms.FolderBrowserDialog
            If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.SelectedPath = dir
            End If
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.SelectedPath, Application.StartupPath)
                If T <> "" AndAlso d.SelectedPath <> "" And T.Length < d.SelectedPath.Length Then
                    .Text = T
                Else
                    .Text = d.SelectedPath
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
End Class
