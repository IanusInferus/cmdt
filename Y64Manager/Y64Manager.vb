'==========================================================================
'
'  File:        Y64Manager.vb
'  Location:    Y64Manager <Visual Basic .Net>
'  Description: Y64文件管理器
'  Version:     2013.01.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports Firefly
Imports Firefly.Setting
Imports Firefly.Imaging
Imports Firefly.GUI
Imports FileSystem
Imports FileSystem.Y64

Public Class Y64Manager

#Region " 设置和全球化 "
    Private Opt As INI
    Private LanFull As String
    Private INISettingNotice As String = "Y64文件管理器初始化配置文件" & Environment.NewLine & "在不了解此文件用法的时候请不要编辑此文件。"
    Private Title As String
    Private DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过Readme.zh.txt中的邮箱或网址联系我。"

    Private Sub Y64Manager_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        IO.Directory.SetCurrentDirectory(Application.StartupPath)
        Opt = New INI("..\Ini\Y64Manager.ini")
        If Not IO.File.Exists("..\Ini\Y64Manager.ini") Then
            LoadOptDefault()
        Else
            LoadOpt()
        End If
        LoadLan()
        Title = Me.Text
    End Sub
    Private Sub Y64Manager_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        SaveOpt()
        Opt.WriteToFile("/*" & Environment.NewLine & INISettingNotice & Environment.NewLine & "*/" & Environment.NewLine)
    End Sub

    Sub LoadOptDefault()
        Importer_TextBox_MainPic.Text = "<Comm2Directory>\MISIONES\TU01_0_0.BMP"
        Importer_TextBox_Y64File.Text = "<Comm2Directory>\MISIONES\TU01\TU01_NEW.Y64"
        Creator_TextBox_DescriptionFile.Text = "<Comm2Directory>\MISIONES\TU01.ini"
        Creator_TextBox_Y64File.Text = "<Comm2Directory>\MISIONES\TU01\TU01_NEW.Y64"
        Converter_TextBox_SourceFile.Text = "<Comm3Directory>\MISIONES\TUT1\TUT1.Y64"
        Converter_TextBox_TargetFile.Text = "<Comm2Directory>\MISIONES\TUT1\TUT1_C2.Y64"
    End Sub
    Sub LoadOpt()
        LanFull = ""
        Opt.ReadValue("Option", "CurrentCulture", LanFull)
        Dim SelectedIndex = ToolTab.SelectedIndex
        Opt.ReadValue("Option", "CurrentPage", SelectedIndex)
        If SelectedIndex >= 0 AndAlso SelectedIndex < ToolTab.TabCount Then
            ToolTab.SelectedIndex = SelectedIndex
        End If

        Opt.ReadValue("Importer", "MainBitmapFile", Importer_TextBox_MainPic.Text)
        Opt.ReadValue("Importer", "ExtraBitmapFile", Importer_TextBox_ExtraPic.Text)
        Opt.ReadValue("Importer", "Y64File", Importer_TextBox_Y64File.Text)
        Opt.ReadValue("Importer", "IndexView", Importer_NumericUpDown_View.Value)
        Opt.ReadValue("Importer", "IndexPic", Importer_NumericUpDown_Pic.Value)
        Opt.ReadValue("Importer", "PicFolder", Importer_TextBox_PicFolder.Text)

        Opt.ReadValue("Creator", "DescriptionFile", Creator_TextBox_DescriptionFile.Text)
        Opt.ReadValue("Creator", "Y64File", Creator_TextBox_Y64File.Text)

        Opt.ReadValue("Converter", "SourceFile", Converter_TextBox_SourceFile.Text)
        Opt.ReadValue("Converter", "TargetFile", Converter_TextBox_TargetFile.Text)
        Dim Version As Integer = 2
        Opt.ReadValue("Converter", "Version", Version)
        Select Case Version
            Case 1
                Converter_RadioButton_Comm2Version1.Checked = True
            Case 2
                Converter_RadioButton_Comm2Version2.Checked = True
            Case 3
                Converter_RadioButton_Comm3Version3.Checked = True
            Case 4
                Converter_RadioButton_Comm3Version4.Checked = True
        End Select
    End Sub
    Sub SaveOpt()
        Opt.WriteValue("Option", "CurrentCulture", LanFull)
        Opt.WriteValue("Option", "CurrentPage", ToolTab.SelectedIndex)

        Opt.WriteValue("Importer", "MainBitmapFile", Importer_TextBox_MainPic.Text, False)
        Opt.WriteValue("Importer", "ExtraBitmapFile", Importer_TextBox_ExtraPic.Text, False)
        Opt.WriteValue("Importer", "Y64File", Importer_TextBox_Y64File.Text, False)
        Opt.WriteValue("Importer", "IndexView", Importer_NumericUpDown_View.Value)
        Opt.WriteValue("Importer", "IndexPic", Importer_NumericUpDown_Pic.Value)
        Opt.WriteValue("Importer", "PicFolder", Importer_TextBox_PicFolder.Text)

        Opt.WriteValue("Creator", "DescriptionFile", Creator_TextBox_DescriptionFile.Text, False)
        Opt.WriteValue("Creator", "Y64File", Creator_TextBox_Y64File.Text, False)

        Opt.WriteValue("Converter", "SourceFile", Converter_TextBox_SourceFile.Text, False)
        Opt.WriteValue("Converter", "TargetFile", Converter_TextBox_TargetFile.Text, False)
        Dim Version As Integer = 2
        If Converter_RadioButton_Comm2Version1.Checked Then
            Version = 1
        ElseIf Converter_RadioButton_Comm2Version2.Checked Then
            Version = 2
        ElseIf Converter_RadioButton_Comm3Version3.Checked Then
            Version = 3
        ElseIf Converter_RadioButton_Comm3Version4.Checked Then
            Version = 4
        End If
        Opt.WriteValue("Converter", "Version", Version)
    End Sub
    Sub LoadLan()
        Dim LRes As New INILocalization("Lan\Y64Manager", LanFull)
        LanFull = LRes.LanguageIndentiySignFull

        Dim Font As String = ""
        LRes.ReadValue("Font", Font)
        If Font <> "" Then Me.Font = New Font(Font, 9, FontStyle.Regular, GraphicsUnit.Point)
        LRes.ReadValue("Title", Me.Text)
        LRes.ReadValue("DebugTip", DebugTip)

        LRes.ReadValue("Exporter", ToolTab.TabPages(0).Text)
        LRes.ReadValue("Button_RGB2YCbCr", Exporter_Button_RGB2YCbCr.Text)
        LRes.ReadValue("Button_YCbCr2RGB", Exporter_Button_YCbCr2RGB.Text)

        LRes.ReadValue("Importer", ToolTab.TabPages(1).Text)
        LRes.ReadValue("Label_Notice", Importer_Label_Notice.Text)
        LRes.ReadValue("Label_Y64File", Importer_Label_Y64File.Text)
        LRes.ReadValue("Label_MainPic", Importer_Label_MainPic.Text)
        LRes.ReadValue("Label_ExtraPic", Importer_Label_ExtraPic.Text)
        LRes.ReadValue("Label_PicFolder", Importer_Label_PicFolder.Text)
        LRes.ReadValue("Label_PictureIndex", Importer_Label_PictureIndex.Text)
        LRes.ReadValue("Button_Import", Importer_Button_Import.Text)
        LRes.ReadValue("Button_FullImport", Importer_Button_FullImport.Text)

        LRes.ReadValue("Creator", ToolTab.TabPages(2).Text)
        LRes.ReadValue("Label_DescriptionFile", Creator_Label_DescriptionFile.Text)
        LRes.ReadValue("Label_Y64File", Creator_Label_Y64File.Text)
        LRes.ReadValue("Button_Create", Creator_Button_Create.Text)

        LRes.ReadValue("Converter", ToolTab.TabPages(3).Text)
        LRes.ReadValue("Label_Notice2", Converter_Label_Notice2.Text)
        LRes.ReadValue("RadioButton_Comm2Version1", Converter_RadioButton_Comm2Version1.Text)
        LRes.ReadValue("RadioButton_Comm2Version2", Converter_RadioButton_Comm2Version2.Text)
        LRes.ReadValue("RadioButton_Comm3Version3", Converter_RadioButton_Comm3Version3.Text)
        LRes.ReadValue("RadioButton_Comm3Version4", Converter_RadioButton_Comm3Version4.Text)
        LRes.ReadValue("Label_SourceFile", Converter_Label_SourceFile.Text)
        LRes.ReadValue("Label_TargetFile", Converter_Label_TargetFile.Text)
        LRes.ReadValue("CheckBox_ReplacePalette", Converter_CheckBox_ReplacePalette.Text)
        LRes.ReadValue("Label_LightnessFactor", Converter_Label_LightnessFactor.Text)
        LRes.ReadValue("Label_SaturationFactor", Converter_Label_SaturationFactor.Text)
        LRes.ReadValue("Button_Convert", Converter_Button_Convert.Text)

        LRes.ReadValue("Readme", ToolTab.TabPages(4).Text)
        LRes.ReadValue("TextBox_Readme", Readme_TextBox_Readme.Text)

        LRes.ReadValue("INISettingNotice", INISettingNotice)

        LRes.ReadValue("Error_PicOutOfBound", PicOutOfBoundException.Error_PicOutOfBound)
        LRes.ReadValue("Error_PicSizeDismatch", PicSizeDismatchException.Error_PicSizeDismatch)
        LRes.ReadValue("Error_ExtraPicSizeDismatch", ExtraPicSizeDismatchException.Error_ExtraPicSizeDismatch)
        LRes.ReadValue("Error_VersionDismatch", VersionDismatchException.Error_VersionDismatch)
    End Sub
#End Region

#Region " 导出器 "
    Private Sub Exporter_Button_RGB2YCbCr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Exporter_Button_RGB2YCbCr.Click
        Dim c As Int32 = YCbCr2RGB(Exporter_NumericUpDown_R.Value, Exporter_NumericUpDown_G.Value, Exporter_NumericUpDown_B.Value)
        Exporter_NumericUpDown_Y.Value = (c And &HFF0000) >> 16
        Exporter_NumericUpDown_Cb.Value = (c And &HFF00) >> 8
        Exporter_NumericUpDown_Cr.Value = c And &HFF
    End Sub

    Private Sub Exporter_Button_YCbCr2RGB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Exporter_Button_YCbCr2RGB.Click
        Dim c As Int32 = RGB2YCbCr(Exporter_NumericUpDown_Y.Value, Exporter_NumericUpDown_Cb.Value, Exporter_NumericUpDown_Cr.Value)
        Exporter_NumericUpDown_R.Value = (c And &HFF0000) >> 16
        Exporter_NumericUpDown_G.Value = (c And &HFF00) >> 8
        Exporter_NumericUpDown_B.Value = c And &HFF
    End Sub
#End Region

#Region " 导入器 "
    Private Sub Importer_Button_Import_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_Button_Import.Click
#If CONFIG <> "Debug" Then
        Try
#End If
        Dim yf = Y64.Open(Importer_TextBox_Y64File.Text)
        yf.Import(Importer_TextBox_MainPic.Text, Importer_TextBox_ExtraPic.Text, Importer_NumericUpDown_View.Value, Importer_NumericUpDown_Pic.Value)
        Using fs As New StreamEx(Importer_TextBox_Y64File.Text, IO.FileMode.Create, IO.FileAccess.ReadWrite)
            yf.WriteTo(fs)
        End Using
        Dim time As Integer = Environment.TickCount
        While Environment.TickCount - time < 500
        End While
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
    End Sub
    Private Sub Importer_Button_FullImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_Button_FullImport.Click
#If CONFIG <> "Debug" Then
        Try
#End If
        Dim TargetName As String = Importer_TextBox_Y64File.Text

        Dim Name As String = GetMainFileName(TargetName)

        Dim Directory As String = Importer_TextBox_PicFolder.Text.TrimEnd("\")
        If Not IO.Directory.Exists(Directory) Then
            Throw New DirectoryNotFoundException(Directory)
        End If
        Dim bName As String
        Dim beName As String

        Dim yf = Y64.Open(TargetName)
        For y As Integer = 0 To yf.NumPic - 1
            For x As Integer = 0 To yf.NumView - 1
                bName = Directory & "\" & x & "_" & y & ".bmp"
                beName = Directory & "\" & x & "_" & y & "_Extra.bmp"
                If IO.File.Exists(bName) Then
                    yf.Import(bName, beName, x, y)
                Else
                    bName = Directory & "\" & x & "_" & y & ".bmp"
                    beName = Directory & "\" & x & "_" & y & "_Extra.bmp"
                    If IO.File.Exists(bName) Then
                        yf.Import(bName, beName, x, y)
                    End If
                End If
            Next
        Next
        Using fs As New StreamEx(TargetName, IO.FileMode.Create, IO.FileAccess.ReadWrite)
            yf.WriteTo(fs)
        End Using
        Dim time As Integer = Environment.TickCount
        While Environment.TickCount - time < 500
        End While
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
    End Sub
#End Region

#Region " 创建器 "
    Private Sub Creator_Button_Create_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Creator_Button_Create.Click
#If CONFIG <> "Debug" Then
        Try
#End If
        Dim Directory As String = GetFileDirectory(Creator_TextBox_Y64File.Text)
        If Not IO.Directory.Exists(Directory) Then
            IO.Directory.CreateDirectory(Directory)
        End If
        Dim yf = Y64.Create(GetFileDirectory(Creator_TextBox_DescriptionFile.Text))
        Using fs As New StreamEx(Creator_TextBox_Y64File.Text, IO.FileMode.Create, IO.FileAccess.ReadWrite)
            yf.WriteTo(fs)
        End Using
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
    End Sub
#End Region

#Region " 转换器 "
    Private Sub Converter_Button_Convert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Converter_Button_Convert.Click
#If CONFIG <> "Debug" Then
        Try
#End If
        Dim Directory As String = GetFileDirectory(Converter_TextBox_TargetFile.Text)
        If Not IO.Directory.Exists(Directory) Then
            IO.Directory.CreateDirectory(Directory)
        End If
        Dim TargetVersion As Y64.Version
        If Converter_RadioButton_Comm2Version1.Checked Then
            TargetVersion = Y64.Version.Comm2Version1
        ElseIf Converter_RadioButton_Comm2Version2.Checked Then
            TargetVersion = Y64.Version.Comm2Version2
        ElseIf Converter_RadioButton_Comm3Version3.Checked Then
            TargetVersion = Y64.Version.Comm3Version3
        ElseIf Converter_RadioButton_Comm3Version4.Checked Then
            TargetVersion = Y64.Version.Comm3Version4
        End If
        Y64.Convert(Converter_TextBox_SourceFile.Text, Converter_TextBox_TargetFile.Text, TargetVersion, Converter_CheckBox_ReplacePalette.Checked, Converter_NumericUpDown_LightnessFactor.Value, Converter_NumericUpDown_SaturationFactor.Value)
        Dim time As Integer = Environment.TickCount
        While Environment.TickCount - time < 500
        End While
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
        End Try
#End If
    End Sub
#End Region

#Region " 文件打开 "
    Private Sub Importer_FileButton_Y64File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_FileButton_Y64File.Click
        With Importer_TextBox_Y64File
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "Y64(*.Y64)|*.Y64"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Importer_FileButton_MainPic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_FileButton_MainPic.Click
        With Importer_TextBox_MainPic
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "BMP(*.BMP)|*.BMP"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Importer_FileButton_ExtraPic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_FileButton_ExtraPic.Click
        With Importer_TextBox_ExtraPic
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "ExtraBMP(*_extra.Y64)|*_extra.Y64"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Importer_FileButton_PicFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Importer_FileButton_PicFolder.Click
        With Importer_TextBox_PicFolder
            Static d As Windows.Forms.FolderBrowserDialog
            If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.SelectedPath = dir
            End If
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.SelectedPath, Application.StartupPath)
                If T <> "" AndAlso d.SelectedPath <> "" AndAlso T.Length < d.SelectedPath.Length Then
                    .Text = T
                Else
                    .Text = d.SelectedPath
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Creator_FileButton_DescriptionFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Creator_FileButton_DescriptionFile.Click
        With Creator_TextBox_DescriptionFile
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "INI(*.INI)|*.INI"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Creator_FileButton_Y64File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Creator_FileButton_Y64File.Click
        With Creator_TextBox_Y64File
            Static d As Windows.Forms.SaveFileDialog
            If d Is Nothing Then d = New Windows.Forms.SaveFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "Y64(*.Y64)|*.Y64"
            d.CheckFileExists = False
            d.CheckPathExists = False
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Converter_FileButton_SourceFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Converter_FileButton_SourceFile.Click
        With Converter_TextBox_SourceFile
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "Y64(*.Y64)|*.Y64"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Converter_FileButton_TargetFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Converter_FileButton_TargetFile.Click
        With Converter_TextBox_TargetFile
            Static d As Windows.Forms.SaveFileDialog
            If d Is Nothing Then d = New Windows.Forms.SaveFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "Y64(*.Y64)|*.Y64"
            d.CheckFileExists = False
            d.CheckPathExists = False
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" AndAlso T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
#End Region

End Class