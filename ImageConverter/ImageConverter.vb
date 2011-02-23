'==========================================================================
'
'  File:        ImageConverter.vb
'  Location:    ImageConverter <Visual Basic .Net>
'  Description: 图像转换器
'  Version:     2011.02.17.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem

Public Module ImageConverter

#Region " 全球化 "
    Private Title As String
    Private DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过下面的网址联系我。" & Environment.NewLine & "是否将网址和具体的错误信息复制到剪贴板？"
    Private NotSupported As String = "没有从""{0}""开始的转换。继续？"
    Private About As String

    Sub LoadLan()
        Dim LRes As New INILocalization("Lan\ImageConverter", "")

        LRes.ReadValue("Title", Title)
        LRes.ReadValue("DebugTip", DebugTip)
        LRes.ReadValue("NotSupported", NotSupported)
        LRes.ReadValue("About", About)
    End Sub
#End Region

    Public Sub Main()
        Application.EnableVisualStyles()
        If System.Diagnostics.Debugger.IsAttached Then
            MainInner()
        Else
            Try
                MainInner()
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
            End Try
        End If
    End Sub

    Public Sub MainInner()
        Dim argv = CommandLine.GetCmdLine().Arguments
        Application.EnableVisualStyles()
        IO.Directory.SetCurrentDirectory(Application.StartupPath)
        LoadLan()
        If argv Is Nothing OrElse argv.Length = 0 Then
            MessageBox.Show(About, Title)
            Return
        End If

        For Each Path As String In argv
            Path = Path.Replace("/", "\").TrimEnd("\")
            Dim Name As String = GetFileName(Path)
            Dim MainName As String = GetMainFileName(Path)
            Dim Dir As String = GetFileDirectory(Path)
            Dim FileDir As String = Path & ".files"
            Dim TempDir As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\CommDevToolkitTemp"

            If IsMatchFileMask(Name, "*.Y64") Then
                Dim f As Y64 = Y64.Open(Path)
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)

                f.ExportDes(GetPath(FileDir, "Description.ini"))

                Dim bName As String
                Dim beName As String
                For x As Integer = 0 To f.NumView - 1
                    For y As Integer = 0 To f.NumPic - 1
                        bName = GetPath(FileDir, x & "_" & y & ".bmp")
                        beName = GetPath(FileDir, x & "_" & y & "_Extra.bmp")
                        f.Export(bName, beName, x, y)
                    Next
                Next
            ElseIf IsMatchFileMask(Name, "*.Y64.files") Then
                Dim Y64Path = GetPath(Dir, MainName)
                Dim Directory = Path
                Dim f As Y64 = Y64.Create(Directory)

                Dim bName As String
                Dim beName As String
                For y As Integer = 0 To f.NumPic - 1
                    For x As Integer = 0 To f.NumView - 1
                        bName = GetPath(Directory, x & "_" & y & ".bmp")
                        beName = GetPath(Directory, x & "_" & y & "_Extra.bmp")
                        If IO.File.Exists(bName) Then
                            f.Import(bName, beName, x, y)
                        Else
                            bName = GetPath(Directory, x & "_" & y & ".bmp")
                            beName = GetPath(Directory, x & "_" & y & "_Extra.bmp")
                            If IO.File.Exists(bName) Then
                                f.Import(bName, beName, x, y)
                            Else
                                Throw New IO.FileNotFoundException(GetPath(Path, x & "_" & y & ".bmp"))
                            End If
                        End If
                    Next
                Next

                Using fs As New StreamEx(Y64Path, IO.FileMode.Create, IO.FileAccess.ReadWrite)
                    f.WriteTo(fs)
                End Using
            ElseIf IsMatchFileMask(Name, "*.RLE") Then
                Dim f As New RLE(Path)
                f.ToGif.WriteToFile(Path & ".gif")
            ElseIf IsMatchFileMask(Name, "*.RLE.gif") Then
                Dim f As New Gif(Path)
                RLE.FromGif(f).WriteToFile(GetPath(Dir, MainName))

            ElseIf IsMatchFileMask(Name, "*.GRL") Then
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                Dim f As New GRL(Path)
                f.ExportDes(FileDir & "\Description.ini")
                For n As Integer = 0 To f.ImageCount - 1
                    f.ExportToGif(n, FileDir)
                Next
            ElseIf IsMatchFileMask(Name, "*.GRL.files") Then
                Dim f As GRL = GRL.Create(Path & "\Description.ini", Path)
                f.WriteToFile(GetPath(Dir, MainName))

            ElseIf IsMatchFileMask(Name, "*.ABI") Then
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                Dim f As New ABI(Path)
                For n As Integer = 0 To f.ImageCount - 1
                    f.ExportToGif(n, FileDir)
                Next

            ElseIf IsMatchFileMask(Name, "*.MBI") Then
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                Dim f As MBI = MBI.Open(Path)
                f.ToObj(Path & ".files\" & GetMainFileName(Path) & ".obj")
                If f.VersionSign = MBI.Version.Comm3 Then
                    f.RemoveComm3Feature()
                    f.WriteToFile(Path & ".files\" & MainName & "_C2.MBI", MBI.Version.Comm2)
                End If
            ElseIf IsMatchFileMask(Name, "*.MBI.files") Then
                Dim f As MBI = MBI.FromObj(GetPath(Path, GetMainFileName(MainName) & ".obj"))
                f.WriteToFile(GetPath(GetFileDirectory(Path), MainName), MBI.Version.Comm2)

            ElseIf IsMatchFileMask(Name, "*.SEC") Then
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                Dim f As SEC = New SEC(Path)
                f.ToObj(Path & ".files\" & MainName & ".obj")

            ElseIf IsMatchFileMask(Name, "*.SEC.files") Then
                Dim f As SEC = SEC.FromObj(GetPath(Path, GetMainFileName(MainName) & ".obj"))
                f.WriteToFile(GetPath(GetFileDirectory(Path), MainName), SEC.Version.Comm2)

            ElseIf IsMatchFileMask(Path, "*.H2O") Then
                Using s As New StreamEx(Path, FileMode.Open, FileAccess.Read)
                    Dim h As New H2O(s)
                    Dim p = h.GetPalette()
                    Dim a = h.GetRectangle()
                    Using b As New Bmp(a.GetLength(0), a.GetLength(1), 8)
                        b.Palette = p
                        b.SetRectangle(0, 0, a)
                        Using os As New StreamEx(GetPath(Dir, Name & ".bmp"), FileMode.Create, FileAccess.ReadWrite)
                            b.SaveTo(os.AsNewWriting)
                        End Using
                    End Using
                End Using

            ElseIf IsMatchFileMask(Path, "*.H2O.bmp") Then
                Dim h As H2O
                Using s As New StreamEx(GetPath(Dir, MainName), FileMode.Open, FileAccess.Read)
                    h = New H2O(s)
                End Using
                Dim p = h.GetPalette()
                Dim a = h.GetRectangle()
                Using b = Bmp.Open(Path)
                    h.SetPalette(b.Palette)
                    h.SetRectangle(b.GetRectangleBytes(0, 0, b.Width, b.Height))
                End Using
                Using s As New StreamEx(GetPath(Dir, MainName), FileMode.Create, FileAccess.ReadWrite)
                    h.WriteTo(s)
                End Using

            Else
                If MessageBox.Show(String.Format(NotSupported, Path), Title, MessageBoxButtons.YesNo) = DialogResult.No Then
                    Return
                End If
            End If
        Next
    End Sub

End Module
