'==========================================================================
'
'  File:        PCKManager.vb
'  Location:    PCKManager <Visual Basic .Net>
'  Description: PCK文件管理器
'
'==========================================================================

Option Compare Text
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Diagnostics
Imports System.IO
Imports System.Windows.Forms
Imports Firefly
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem

Public Class PCKManager

#Region " 设置和全球化 "
    Private Opt As Ini
    Private LanFull As String
    Private INISettingNotice As String = "PCK文件管理器初始化配置文件" & Environment.NewLine & "在不了解此文件用法的时候请不要编辑此文件。"
    Private Shared Title As String = "PCK文件管理器"
    Private Shared DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过Readme.zh.txt中的邮箱或网址联系我。"
    Private RecentFiles(5) As String
    Private Readme As String

    Private Sub PCKManager_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Opt = New Ini("..\Ini\PCKManager.ini")
        LoadOpt()
        LoadLan()

        Dim ImageList As New ImageList()
        ImageList.Images.Add(My.Resources.File)
        ImageList.Images.Add(My.Resources.Directory)
        ImageList.ColorDepth = ColorDepth.Depth32Bit
        FileListView.SmallImageList = ImageList
        FileListView.ContextMenu = ContextMenu

        RefreshRecent()
    End Sub
    Private Sub PCKManager_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not pfClosed Then pf.Close()
        ChDir(Application.StartupPath)
        SaveOpt()
        Opt.WriteToFile("/*" & Environment.NewLine & INISettingNotice & Environment.NewLine & "*/" & Environment.NewLine)
    End Sub

    Sub LoadOpt()
        LanFull = ""
        Opt.ReadValue("Option", "CurrentCulture", LanFull)
        Opt.ReadValue("Option", "Recent0", RecentFiles(0))
        Opt.ReadValue("Option", "Recent1", RecentFiles(1))
        Opt.ReadValue("Option", "Recent2", RecentFiles(2))
        Opt.ReadValue("Option", "Recent3", RecentFiles(3))
        Opt.ReadValue("Option", "Recent4", RecentFiles(4))
        Opt.ReadValue("Option", "Recent5", RecentFiles(5))
    End Sub
    Sub SaveOpt()
        Opt.WriteValue("Option", "CurrentCulture", LanFull)
        Opt.WriteValue("Option", "Recent0", RecentFiles(0), False)
        Opt.WriteValue("Option", "Recent1", RecentFiles(1), False)
        Opt.WriteValue("Option", "Recent2", RecentFiles(2), False)
        Opt.WriteValue("Option", "Recent3", RecentFiles(3), False)
        Opt.WriteValue("Option", "Recent4", RecentFiles(4), False)
        Opt.WriteValue("Option", "Recent5", RecentFiles(5), False)
    End Sub
    Sub LoadLan()
        Dim LRes As New IniLocalization("Lan\PCKManager", LanFull)
        LanFull = LRes.LanguageIndentiySignFull

        Dim Font As String = ""
        LRes.ReadValue("Font", Font)
        If Font <> "" Then Me.Font = New Font(Font, 9, FontStyle.Regular, GraphicsUnit.Point)
        LRes.ReadValue("Title", Title)
        Me.Text = Title
        LRes.ReadValue("DebugTip", DebugTip)

        LRes.ReadValue("Menu_File", Menu_File.Text)
        LRes.ReadValue("Menu_File_Open1", Menu_File_Open1.Text)
        LRes.ReadValue("Menu_File_Open2", Menu_File_Open2.Text)
        LRes.ReadValue("Menu_File_Open3", Menu_File_Open3.Text)
        LRes.ReadValue("Menu_File_OpenSF", Menu_File_OpenSF.Text)
        LRes.ReadValue("Menu_File_Create1", Menu_File_Create1.Text)
        LRes.ReadValue("Menu_File_Create2", Menu_File_Create2.Text)
        LRes.ReadValue("Menu_File_Create3", Menu_File_Create3.Text)
        LRes.ReadValue("Menu_File_Close", Menu_File_Close.Text)
        LRes.ReadValue("Menu_File_RecentFiles", Menu_File_RecentFiles.Text)
        LRes.ReadValue("Menu_File_Exit", Menu_File_Exit.Text)
        LRes.ReadValue("Menu_About", Menu_About.Text)
        LRes.ReadValue("Menu_About_About", Menu_About_About.Text)

        LRes.ReadValue("ContextMenu_Extract", ContextMenu_Extract.Text)

        LRes.ReadValue("FileListView_FileName", FileListView.Columns.Item(0).Text)
        LRes.ReadValue("FileListView_FileLength", FileListView.Columns.Item(1).Text)
        LRes.ReadValue("FileListView_Offset", FileListView.Columns.Item(2).Text)
        LRes.ReadValue("FileListView_FileType", FileListView.Columns.Item(3).Text)

        LRes.ReadValue("TextBox_Readme", Readme)

        LRes.ReadValue("INISettingNotice", INISettingNotice)
    End Sub
#End Region

    Public Shared Sub Application_ThreadException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        ExceptionHandler.PopupException(e.Exception, New StackTrace(4, True), DebugTip, Title)
    End Sub

    <STAThread()> _
    Public Shared Function Main() As Integer
        If System.Diagnostics.Debugger.IsAttached Then
            Return MainInner()
        Else
            Try
                Return MainInner()
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
                Return -1
            End Try
        End If
    End Function

    Public Shared Function MainInner() As Integer
        If Debugger.IsAttached Then
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException)
            Return MainWindow()
        Else
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
            Try
                AddHandler Application.ThreadException, AddressOf Application_ThreadException
                Return MainWindow()
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
                Return -1
            Finally
                RemoveHandler Application.ThreadException, AddressOf Application_ThreadException
            End Try
        End If
    End Function

    Public Shared Function MainWindow() As Integer
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        My.Forms.PCKManager.CenterToScreen()
        My.Forms.PCKManager.Show()
        Application.Run(My.Forms.PCKManager)
        Return 0
    End Function

    Private pf As Object
    Private pfCurDirDB As Object
    Private pfClosed As Boolean = True
    Private TempDir As String = GetPath(My.Computer.FileSystem.SpecialDirectories.Temp, "CommDevToolkitTemp")
    Private TempList As Collections.Specialized.StringCollection

    Private Sub PCKManager_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If IO.Directory.Exists(TempDir) Then
            Try
                IO.Directory.Delete(TempDir, True)
            Catch
            End Try
        End If
    End Sub

    Private Sub RefreshList()
        If pfCurDirDB Is Nothing Then Return
        Dim Item As ListViewItem
        Dim n As Integer = 0
        Dim Sorter As New List(Of ListViewItem)
        Dim FileMask As String = Mask.Text
        Dim DirectoryOffsetFlag As Boolean = Not TypeOf pf Is PAK '判断是否是打击力量的PAK文件(其中的文件夹没有偏移量)
        For Each f As Object In pfCurDirDB.SubFileDB
            Select Case f.Type
                Case PCK.FileDB.FileType.File
                    If IsMatchFileMask(f.Name, FileMask) Then
                        Item = New ListViewItem(New String() {f.Name, f.Length, f.Address, GetExtendedFileName(f.Name), n, 1}, 0)
                    Else
                        Item = Nothing
                    End If
                Case PCK.FileDB.FileType.Directory
                    If DirectoryOffsetFlag Then
                        Item = New ListViewItem(New String() {f.Name, "", f.Address, "", n, 0}, 1)
                    Else
                        Item = New ListViewItem(New String() {f.Name, "", "", "", n, 0}, 1)
                    End If
                Case Else
                    Item = Nothing
            End Select
            If Item IsNot Nothing Then Sorter.Add(Item)
            n += 1
        Next
        If FileListViewMajorCompareeIndex <> -1 Then Sorter.Sort(AddressOf Comparison)

        With FileListView.Items
            .Clear()
            If pfCurDirDB.ParentFileDB IsNot Nothing Then
                .Add(New ListViewItem(New String() {"..", "", "", "", -1, 0}, 1))
            End If
            .AddRange(Sorter.ToArray)
        End With
    End Sub
    Public FileListViewMajorCompareeIndex As Integer = -1

    Private Function Comparison(ByVal x As ListViewItem, ByVal y As ListViewItem) As Integer
        If x.SubItems(5).Text < y.SubItems(5).Text Then Return -1
        If x.SubItems(5).Text > y.SubItems(5).Text Then Return 1

        Select Case FileListViewMajorCompareeIndex
            Case 0, 3
                If x.SubItems(FileListViewMajorCompareeIndex).Text < y.SubItems(FileListViewMajorCompareeIndex).Text Then Return -1
                If x.SubItems(FileListViewMajorCompareeIndex).Text > y.SubItems(FileListViewMajorCompareeIndex).Text Then Return 1
            Case 1, 2
                If CInt(x.SubItems(5).Text) <> 0 Then
                    If CInt(x.SubItems(FileListViewMajorCompareeIndex).Text) < CInt(y.SubItems(FileListViewMajorCompareeIndex).Text) Then Return -1
                    If CInt(x.SubItems(FileListViewMajorCompareeIndex).Text) > CInt(y.SubItems(FileListViewMajorCompareeIndex).Text) Then Return 1
                End If
        End Select

        If x.SubItems(0).Text < y.SubItems(0).Text Then Return -1
        If x.SubItems(0).Text > y.SubItems(0).Text Then Return 1

        If x.SubItems(2).Text < y.SubItems(2).Text Then Return -1
        If x.SubItems(2).Text > y.SubItems(2).Text Then Return 1
        Return 0
    End Function

    Private Sub Menu_File_Open1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Open1.Click
        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "DIR(*.DIR)|*.DIR"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If Not pfClosed Then pf.Close()
            pf = DIR.Open(d.FileName)
            pfCurDirDB = pf.Root
            Path.Text = pfCurDirDB.Name & "\"
            Path.Text = Path.Text.TrimStart("\")
            RefreshList()
            AddRecent(d.FileName & ",1")
            pfClosed = False
            Me.Text = Title & " - " & d.FileName
        End If
    End Sub
    Private Sub Menu_File_Open2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Open2.Click
        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "PCK(*.PCK)|*.PCK"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If Not pfClosed Then pf.Close()
            pf = PCK.Open(d.FileName, PCK.Version.Comm2)
            pfCurDirDB = pf.Root
            Path.Text = pfCurDirDB.Name & "\"
            Path.Text = Path.Text.TrimStart("\")
            RefreshList()
            AddRecent(d.FileName & ",2")
            pfClosed = False
            Me.Text = Title & " - " & d.FileName
        End If
    End Sub
    Private Sub Menu_File_Open3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Open3.Click
        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "PCK(*.PCK)|*.PCK"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If Not pfClosed Then pf.Close()
            pf = PCK.Open(d.FileName, PCK.Version.Comm3)
            pfCurDirDB = pf.Root
            Path.Text = pfCurDirDB.Name & "\"
            Path.Text = Path.Text.TrimStart("\")
            RefreshList()
            AddRecent(d.FileName & ",3")
            pfClosed = False
            Me.Text = Title & " - " & d.FileName
        End If
    End Sub

    Private Sub Menu_File_OpenSF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_OpenSF.Click
        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "PAK(*.PAK)|*.PAK"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If Not pfClosed Then pf.Close()
            Dim p = PAK.Open(d.FileName)
            pf = p
            pfCurDirDB = pf.Root
            Path.Text = ""
            RefreshList()
            AddRecent(d.FileName & ",SF")
            pfClosed = False
            Me.Text = $"{Title} - {d.FileName} - {p.VersionSign},{p.PlatformSign}"
        End If
    End Sub

    Private Sub Menu_File_Create1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Create1.Click
        Static d As Windows.Forms.FolderBrowserDialog
        If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Static d2 As Windows.Forms.SaveFileDialog
            If d2 Is Nothing Then d2 = New Windows.Forms.SaveFileDialog
            d2.Filter = "DIR(*.DIR)|*.DIR"

            If d2.ShowDialog() = Windows.Forms.DialogResult.OK Then
                If Not pfClosed Then pf.Close()
                pfCurDirDB = Nothing
                FileListView.Items.Clear()
                Path.Text = ""
                pfClosed = True
                Application.DoEvents()

                pf = New DIR(d.SelectedPath, d2.FileName)
                pfCurDirDB = pf.Root
                Path.Text = pfCurDirDB.Name & "\"
                Path.Text = Path.Text.TrimStart("\")
                RefreshList()
                pfClosed = False
                Me.Text = Title & " - " & d2.FileName
            End If
        End If
    End Sub

    Private Sub Menu_File_Create2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Create2.Click
        Static d As Windows.Forms.FolderBrowserDialog
        If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Static d2 As Windows.Forms.SaveFileDialog
            If d2 Is Nothing Then d2 = New Windows.Forms.SaveFileDialog
            d2.Filter = "PCK(*.PCK)|*.PCK"

            If d2.ShowDialog() = Windows.Forms.DialogResult.OK Then
                If Not pfClosed Then pf.Close()
                pfCurDirDB = Nothing
                FileListView.Items.Clear()
                Path.Text = ""
                pfClosed = True
                Application.DoEvents()

                pf = New PCK(d.SelectedPath, d2.FileName, PCK.Version.Comm2)
                pfCurDirDB = pf.Root
                Path.Text = pfCurDirDB.Name & "\"
                Path.Text = Path.Text.TrimStart("\")
                RefreshList()
                pfClosed = False
                Me.Text = Title & " - " & d2.FileName
            End If
        End If
    End Sub

    Private Sub Menu_File_Create3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Create3.Click
        Static d As Windows.Forms.FolderBrowserDialog
        If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Static d2 As Windows.Forms.SaveFileDialog
            If d2 Is Nothing Then d2 = New Windows.Forms.SaveFileDialog
            d2.Filter = "PCK(*.PCK)|*.PCK"

            If d2.ShowDialog() = Windows.Forms.DialogResult.OK Then
                If Not pfClosed Then pf.Close()
                pfCurDirDB = Nothing
                FileListView.Items.Clear()
                Path.Text = ""
                pfClosed = True
                Application.DoEvents()

                pf = New PCK(d.SelectedPath, d2.FileName, PCK.Version.Comm3)
                pfCurDirDB = pf.Root
                Path.Text = pfCurDirDB.Name & "\"
                Path.Text = Path.Text.TrimStart("\")
                RefreshList()
                pfClosed = False
                Me.Text = Title & " - " & d2.FileName
            End If
        End If
    End Sub

    Private Sub Menu_File_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Close.Click
        If Not pfClosed Then pf.Close()
        pfCurDirDB = Nothing
        FileListView.Items.Clear()
        Path.Text = ""
        pfClosed = True
        Me.Text = Title
    End Sub

    Private d As Windows.Forms.FolderBrowserDialog

    Private Sub FileListView_ItemActivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileListView.ItemActivate
        If pfClosed Then Return
        Dim Item As ListViewItem = FileListView.SelectedItems.Item(0)
        Dim n As Integer = Item.SubItems(4).Text
        If Item.SubItems(5).Text = 1 Then
            If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                pf.Extract(pfCurDirDB.SubFileDB(n), d.SelectedPath, Mask.Text)
            End If
        Else
            If n < 0 Then
                pfCurDirDB = pfCurDirDB.ParentFileDB
                Path.Text = Path.Text.Substring(0, Path.Text.Length - 1)
                If Path.Text.Contains("\") Then
                    Path.Text = Path.Text.Substring(0, Path.Text.LastIndexOf("\")) & "\"
                Else
                    Path.Text = ""
                End If
            Else
                pfCurDirDB = pfCurDirDB.SubFileDB(n)
                Path.Text = (GetPath(Path.Text, pfCurDirDB.Name) & "\").TrimStart("\")
            End If
            Path.Text = Path.Text.TrimStart("\")
            RefreshList()
        End If
    End Sub

    Private Sub ContextMenu_Extract_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContextMenu_Extract.Click
        If d Is Nothing Then d = New Windows.Forms.FolderBrowserDialog
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            For Each Item As ListViewItem In FileListView.SelectedItems
                Dim n As Integer = Item.SubItems(4).Text
                If n < 0 Then Continue For
                pf.Extract(pfCurDirDB.SubFileDB(n), d.SelectedPath, Mask.Text)
            Next
        End If
    End Sub

    Private Sub Menu_File_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Exit.Click
        Me.Close()
    End Sub

    Private Sub FileListView_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles FileListView.ItemDrag
        Dim Data As New DataObject

        TempList = New Collections.Specialized.StringCollection
        For Each Item As ListViewItem In FileListView.SelectedItems
            If Item.SubItems.Count < 4 Then Return
            Dim n As Integer = Item.SubItems(4).Text
            If n < 0 Then Continue For
            pf.Extract(pfCurDirDB.SubFileDB(n), TempDir, Mask.Text)
            TempList.Add(TempDir & "\" & pfCurDirDB.SubFileDB(n).Name)
        Next
        Data.SetFileDropList(TempList)

        FileListView.DoDragDrop(Data, DragDropEffects.Move)
    End Sub

    Private Sub Mask_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Mask.TextChanged
        If pfClosed Then Return
        Dim Timer As Double = DateAndTime.Timer
        While DateAndTime.Timer - Timer < 2
            Application.DoEvents()
        End While
        RefreshList()
    End Sub

    Private Sub Menu_About_About_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_About_About.Click
        MsgBox(Readme)
    End Sub

    Private Sub AddRecent(ByVal s As String)
        For n As Integer = 0 To 5
            If RecentFiles(n) = s Then
                For m As Integer = n To 4
                    RecentFiles(m) = RecentFiles(m + 1)
                Next
                RecentFiles(5) = Nothing
                Exit For
            End If
        Next
        For n As Integer = 4 To 0 Step -1
            RecentFiles(n + 1) = RecentFiles(n)
        Next
        RecentFiles(0) = s
        RefreshRecent()
    End Sub
    Private Sub RemoveRecent(ByVal Index As Integer)
        RecentFiles(Index) = Nothing
        For m As Integer = Index To 4
            RecentFiles(m) = RecentFiles(m + 1)
        Next
        RecentFiles(5) = Nothing
        RefreshRecent()
    End Sub
    Private Sub RefreshRecent()
        Dim c As Menu.MenuItemCollection = Menu_File_RecentFiles.MenuItems
        c.Clear()
        For n As Integer = 0 To 5
            If RecentFiles(n) = Nothing Then
                Exit For
            End If
            Dim i As New MenuItem("&" & n & " " & RecentFiles(n))
            c.Add(i)
            AddHandler i.Click, AddressOf RecentFilesHandler
        Next
        Menu_File_RecentFiles.Enabled = (c.Count <> 0)
    End Sub
    Private Sub RecentFilesHandler(ByVal sender As Object, ByVal e As EventArgs)
#If CONFIG <> "Debug" Then
        Try
#End If
        Dim r As MenuItem = sender
        Dim Path As String = r.Text.Substring(3)
        If Not r.Text.Contains(",") Then RemoveRecent(r.Text.Substring(0, 1))
        Dim Version As String = ""
        If Path.Contains(",") Then
            Version = Path.Substring(Path.LastIndexOf(",") + 1)
        End If
        Path = Path.Substring(0, Path.Length - Version.Length - 1)
        Select Case Version
            Case "1"
                If Not pfClosed Then pf.Close()
                pf = DIR.Open(Path)
                pfCurDirDB = pf.Root
                Me.Path.Text = pfCurDirDB.Name & "\"
                Me.Path.Text = Me.Path.Text.TrimStart("\")
                RefreshList()
                AddRecent(Path & ",1")
                pfClosed = False
                Me.Text = Title & " - " & Path
            Case "3"
                If Not pfClosed Then pf.Close()
                pf = PCK.Open(Path, PCK.Version.Comm3)
                pfCurDirDB = pf.Root
                Me.Path.Text = pfCurDirDB.Name & "\"
                Me.Path.Text = Me.Path.Text.TrimStart("\")
                RefreshList()
                AddRecent(Path & ",3")
                pfClosed = False
                Me.Text = Title & " - " & Path
            Case "SF"
                Dim a As FileAttribute = File.GetAttributes(Path)
                If a And FileAttribute.ReadOnly Then
                    Try
                        File.SetAttributes(Path, a And Not FileAttribute.ReadOnly)
                    Catch
                    End Try
                End If
                If Not pfClosed Then pf.Close()
                Dim p = PAK.Open(Path)
                pf = p
                pfCurDirDB = pf.Root
                Me.Path.Text = ""
                RefreshList()
                AddRecent(Path & ",SF")
                pfClosed = False
                Me.Text = $"{Title} - {Path} - {p.VersionSign},{p.PlatformSign}"
            Case Else
                If Not pfClosed Then pf.Close()
                pf = PCK.Open(Path, PCK.Version.Comm2)
                pfCurDirDB = pf.Root
                Me.Path.Text = pfCurDirDB.Name & "\"
                Me.Path.Text = Me.Path.Text.TrimStart("\")
                RefreshList()
                AddRecent(Path & ",2")
                pfClosed = False
                Me.Text = Title & " - " & Path
        End Select
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex, DebugTip, Title)
            RemoveRecent(sender.Text.Substring(1, 1))
        End Try
#End If
    End Sub

    Private Sub FileListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles FileListView.KeyUp
        Select Case e.KeyData
            Case Keys.Back
                If pfClosed Then Return
                If pfCurDirDB.ParentFileDB Is Nothing Then Return
                pfCurDirDB = pfCurDirDB.ParentFileDB
                Path.Text = Path.Text.Substring(0, Path.Text.Length - 1)
                If Path.Text.Contains("\") Then
                    Path.Text = Path.Text.Substring(0, Path.Text.LastIndexOf("\")) & "\"
                Else
                    Path.Text = ""
                End If
                Path.Text = Path.Text.TrimStart("\")
                RefreshList()
            Case Keys.Control + Keys.A
                FileListView.BeginUpdate()
                For Each Item As ListViewItem In FileListView.Items
                    Item.Selected = True
                Next
                If FileListView.Items(0).SubItems(0).Text = ".." Then FileListView.Items(0).Selected = False
                FileListView.EndUpdate()
        End Select
    End Sub

    Private Sub FileListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles FileListView.ColumnClick
        FileListViewMajorCompareeIndex = e.Column
        RefreshList()
    End Sub
End Class
