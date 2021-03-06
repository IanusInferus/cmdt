'==========================================================================
'
'  File:        XmlConverter.vb
'  Location:    XmlConverter <Visual Basic .Net>
'  Description: Xml转换器
'  Version:     2013.08.14.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Text.RegularExpressions
Imports Firefly
Imports Firefly.Mapping
Imports Firefly.Mapping.XmlText
Imports Firefly.Streaming
Imports Firefly.Texting
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem

Public Module XmlConverter

#Region " 全球化 "
    Private Title As String
    Private DebugTip As String = "程序出现错误，如果你确定它不应该出现，请通过Readme.zh.txt中的邮箱或网址联系我。"
    Private NotSupported As String = "没有从""{0}""开始的转换。继续？"
    Private About As String

    Sub LoadLan()
        Dim LRes As New IniLocalization("Lan\XmlConverter", "")

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
        Dim CurDir = IO.Directory.GetCurrentDirectory()
        Try
            IO.Directory.SetCurrentDirectory(Application.StartupPath)
            LoadLan()
        Finally
            IO.Directory.SetCurrentDirectory(CurDir)
        End Try
        If argv Is Nothing OrElse argv.Length = 0 Then
            MessageBox.Show(About, Title)
            Return
        End If
        Dim xs As New XmlSerializer(True)
        For Each Path As String In argv
            Path = Path.Replace("/", "\").TrimEnd("\")
            Dim Name As String = GetFileName(Path)
            Dim MainName As String = GetMainFileName(Path)
            Dim Dir As String = GetFileDirectory(Path)
            Dim FileDir As String = Path & ".xfiles"
            Dim TempDir As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\CommDevToolkitTemp"

            If IsMatchFileMask(Name, "*.SEC") Then
                Dim f = SEC_Simple.Open(Path)
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                SEC_Simple.SaveXml(GetPath(FileDir, "Description.xml"), f)

            ElseIf IsMatchFileMask(Name, "*.SEC.xfiles") Then
                Dim f = SEC_Simple.OpenXml(GetPath(Path, "Description.xml"))
                SEC_Simple.Save(GetPath(Dir, MainName), f)

            ElseIf IsMatchFileMask(Path, "*.SEC.xfiles\Description.xml") Then
                Dim f = SEC_Simple.OpenXml(Path)

                Dim ObjectInfoBlocks = New List(Of MA2.ObjectInfoBlock)
                For Each d In f.Districts
                    Dim oib = New MA2.ObjectInfoBlock
                    If (d.kx - 0) ^ 2 + (d.ky - 0) ^ 2 < 0.00001 Then
                        oib.Type = 0
                    Else
                        oib.Type = 4
                    End If
                    oib.n = d.Borders.Count

                    Dim X = New Single(oib.n - 1) {}
                    Dim Y = New Single(oib.n - 1) {}
                    For n = 0 To oib.n - 1
                        Dim p = f.Points(d.Borders(n).StartPointIndex)
                        X(n) = p.x
                        Y(n) = p.y
                    Next

                    oib.CenterX = X.Average()
                    oib.CenterY = Y.Average()
                    oib.X = X.Select(Function(v) v - oib.CenterX).ToArray()
                    oib.Y = Y.Select(Function(v) v - oib.CenterY).ToArray()

                    Dim UpperZ = d.GetZ(oib.CenterX, oib.CenterY)
                    Dim LowestZ = UpperZ
                    For Each b In d.Borders
                        If b.NeighborDistrictIndex < 0 Then Continue For
                        Dim dNeighbor = f.Districts(b.NeighborDistrictIndex)
                        Dim pa = f.Points(b.StartPointIndex)
                        Dim pb = f.Points(b.EndPointIndex)
                        LowestZ = Math.Min(dNeighbor.GetZ(pa.x, pa.y), LowestZ)
                        LowestZ = Math.Min(dNeighbor.GetZ(pb.x, pb.y), LowestZ)
                    Next
                    If Math.Abs(UpperZ - LowestZ) < 0.001 Then Continue For

                    oib.CenterZ = LowestZ

                    Dim s = Math.Sqrt(d.kx ^ 2 + d.ky ^ 2 + 1)
                    oib.nx = -d.kx / s
                    oib.ny = -d.ky / s
                    oib.nz = 1 / s

                    oib.D = -oib.nz * (UpperZ - oib.CenterZ)

                    ObjectInfoBlocks.Add(oib)
                Next

                XmlFile.WriteFile(GetPath(Dir, MainName) & ".ma2.xml", xs.Write(ObjectInfoBlocks))

            ElseIf IsMatchFileMask(Name, "*.MA2") Then
                Dim f As MA2 = New MA2(Path)
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)

                Dim CurrentDir = Environment.CurrentDirectory
                Environment.CurrentDirectory = FileDir
                Dim wd As New Dictionary(Of MA2.WaterMaskInfoBlock, Integer)
                Dim wdh As New Dictionary(Of MA2.WaterMaskInfoBlock, Integer)
                For n As Integer = 0 To f.WaterMaskInfo.GetUpperBound(0)
                    Dim ro As MA2.WaterMaskInfoBlock = f.WaterMaskInfo(n)
                    If ro Is Nothing Then Continue For
                    wd.Add(ro, n)
                Next
                For n As Integer = 0 To f.WaterMaskHighQualityInfo.GetUpperBound(0)
                    Dim ro As MA2.WaterMaskInfoBlock = f.WaterMaskHighQualityInfo(n)
                    If ro Is Nothing Then Continue For
                    wdh.Add(ro, n)
                Next
                Dim we As New MA2.WaterMaskInfoBlockEncoder(wd, wdh)
                Dim d As New Dictionary(Of MA2.RenderInfoBlock, Integer)
                For n As Integer = 0 To f.RenderInfo.GetUpperBound(0)
                    For k As Integer = 0 To f.RenderInfo(n).GetUpperBound(0)
                        Dim ro As MA2.RenderInfoBlock = f.RenderInfo(n)(k)
                        d.Add(ro, n)
                    Next
                Next
                Dim e As New MA2.RenderInfoBlockEncoder(d)
                Xml.WriteFile(GetAbsolutePath(GetPath(FileDir, "Description.xml"), CurrentDir), TextEncoding.WritingDefault, f, New Xml.IMapper() {New ByteArrayEncoder, we, e})

                Environment.CurrentDirectory = CurrentDir

                If IsMatchFileMask(MainName, "*EX") Then MainName = MainName.Substring(0, MainName.Length - 2)
                Dim yfPath As String = GetPath(Dir, MainName & "EX.Y64") 'ECL
                If Not IO.File.Exists(yfPath) Then yfPath = GetPath(Dir, MainName & ".Y64")
                If Not IO.File.Exists(yfPath) Then Continue For

                Dim yf As Y64 = Y64.Open(yfPath)
                If Not IO.Directory.Exists(TempDir) Then IO.Directory.CreateDirectory(TempDir)
                For n As Integer = 0 To f.RenderInfo.GetUpperBound(0)
                    Dim bName As String = GetPath(TempDir, n & "_" & 0 & ".bmp")
                    Dim beName As String = GetPath(TempDir, n & "_" & 0 & "_Extra.bmp")
                    yf.Export(bName, beName, n, 0)
                    Dim Duplicated As New Dictionary(Of String, Integer)
                    Using Bitmap = Bmp.Open(GetPath(TempDir, n & "_" & 0 & ".bmp"))
                        For k As Integer = 0 To f.RenderInfo(n).GetUpperBound(0)
                            Dim ro As MA2.RenderInfoBlock = f.RenderInfo(n)(k)
                            Dim RenderMap As Array2(Of Byte) = ro.RenderMap
                            Dim Rectangle As Int32(,) = Bitmap.GetRectangleAsARGB(ro.x, ro.y, ro.Width, ro.Height)
                            For y As Integer = 0 To RenderMap.GetUpperBound(1)
                                For x As Integer = 0 To RenderMap.GetUpperBound(0)
                                    Select Case RenderMap(x, y)
                                        Case 0
                                            Rectangle(x, y) = 0
                                        Case 1
                                            Rectangle(x, y) = ConcatBits(&H7F, 8, Rectangle(x, y).Bits(23, 0), 24)
                                        Case 2
                                            Rectangle(x, y) = ConcatBits(&HFF, 8, Rectangle(x, y).Bits(23, 0), 24)
                                        Case Else
                                            Throw New InvalidDataException
                                    End Select
                                Next
                            Next
                            Using RenderBitmap As New Bitmap(ro.Width, ro.Height, Drawing.Imaging.PixelFormat.Format32bppArgb)
                                RenderBitmap.SetRectangle(0, 0, Rectangle)
                                Dim MapPath = ro.ObjectIndex & "_" & n & ".png"
                                If Duplicated.ContainsKey(MapPath) Then
                                    Dim r = ro.ObjectIndex & "_" & n & "_" & Duplicated(MapPath) & ".png"
                                    Duplicated(MapPath) += 1
                                    MapPath = r
                                Else
                                    Duplicated.Add(MapPath, 1)
                                End If
                                RenderBitmap.Save(GetPath(FileDir, MapPath), System.Drawing.Imaging.ImageFormat.Png)
                            End Using
                        Next
                    End Using
                    IO.File.Delete(GetPath(TempDir, n & "_" & 0 & ".bmp"))
                Next
                IO.Directory.Delete(TempDir, True)
            ElseIf IsMatchFileMask(Name, "*.MA2.xfiles") Then
                Dim MA2Path = GetPath(Dir, MainName)
                Dim CurrentDir = Environment.CurrentDirectory
                Environment.CurrentDirectory = Path
                Dim f As MA2 = Xml.ReadFile(Of MA2)(GetAbsolutePath(GetPath(Path, "Description.xml"), CurrentDir), New Xml.IMapper() {New ByteArrayEncoder, New MA2.WaterMaskInfoBlockEncoder, New MA2.RenderInfoBlockEncoder})
                Environment.CurrentDirectory = CurrentDir
                f.WriteToFile(MA2Path)
            ElseIf IsMatchFileMask(Name, "*.DES") Then
                Dim DESPath = Path
                Dim DescriptionPath = GetPath(FileDir, "Description.xml")
                Dim GRLPath = GetPath(FileDir, "Textures.GRL")

                Dim f As DES_Root = DES.ReadFile(DESPath)
                Dim x = xs.Write(Of DES_Description)(f.Description)
                If Not IO.Directory.Exists(FileDir) Then IO.Directory.CreateDirectory(FileDir)
                XmlFile.WriteFile(DescriptionPath, x)
                Using s = Streams.CreateResizable(GRLPath)
                    s.Write(f.GRLData)
                End Using

            ElseIf IsMatchFileMask(Name, "*.DES.xfiles") Then
                Dim DESPath = GetPath(Dir, MainName)
                Dim DescriptionPath = GetPath(Path, "Description.xml")
                Dim GRLPath = GetPath(Path, "Textures.GRL")

                Dim x = XmlFile.ReadFile(DescriptionPath)
                Dim f As New DES_Root
                f.Description = xs.Read(Of DES_Description)(x)
                Using s = Streams.OpenReadable(GRLPath)
                    f.GRLData = s.Read(s.Length)
                End Using
                DES.WriteFile(DESPath, f)

            Else
                If MessageBox.Show(String.Format(NotSupported, Path), Title, MessageBoxButtons.YesNo) = DialogResult.No Then
                    Return
                End If
            End If
        Next
    End Sub

    Public Class ByteArrayEncoder
        Inherits Xml.Mapper(Of Byte(), String)

        Public Overrides Function GetMappedObject(ByVal o As Byte()) As String
            Return String.Join(" ", (From b In o Select b.ToString("X2")).ToArray)
        End Function

        Public Overrides Function GetInverseMappedObject(ByVal o As String) As Byte()
            Dim Trimmed = o.Trim(" \t\r\n".Descape)
            If Trimmed = "" Then Return New Byte() {}
            Return (From s In Regex.Split(Trimmed, "( |\t|\r|\n)+", RegexOptions.ExplicitCapture) Select Byte.Parse(s, Globalization.NumberStyles.HexNumber)).ToArray
        End Function
    End Class

End Module
