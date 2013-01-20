'==========================================================================
'
'  File:        Surface.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: SEC地形文件编辑器
'  Version:     2013.01.21.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Mapping.XmlText
Imports Firefly.Setting
Imports Firefly.GUI
Imports FileSystem
Imports GraphSystem

Public Class Surface
    Public SECXml As Boolean
    Public SECPath As String
    Public Changed As Boolean

    Private xs As New XmlSerializer(True)
    Private Class ByteArrayEncoder
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

    Public Shared Sub Application_ThreadException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        ExceptionHandler.PopupException(e.Exception, New StackTrace(4, True))
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
        My.Forms.Surface.Height = 640
        My.Forms.Surface.CenterToScreen()
        My.Forms.Surface.Show()
        Dim argv = CommandLine.GetCmdLine().Arguments
        If argv IsNot Nothing AndAlso argv.Length >= 1 Then
            With My.Forms.Surface
                .OpenSEC(argv(0))
                If .Menu_GraphicsInterface_GDIP.Checked Then
                    .PicBox.UseGDIP()
                ElseIf .Menu_GraphicsInterface_SlimDX.Checked Then
                    .PicBox.UseD3D()
                End If
            End With
        Else
            My.Forms.Surface.AttachToSEC(Nothing)
        End If
        Application.Run(My.Forms.Surface)
        Return 0
    End Function


    Public Sub AttachToSEC(ByVal SECFile As SEC_Simple.FileInfo)
        PicBox.AttachToSEC(SECFile)
        If SECFile IsNot Nothing Then
            Menu_File_Save.Enabled = True
            Menu_File_SaveAs.Enabled = True
            Menu_File_Close.Enabled = True
            Menu_View_DistrictDataEditor.Enabled = True

            NumericUpDown_Point_Select.Minimum = 0
            NumericUpDown_Point_Select.Maximum = SECFile.Points.Count - 1
            NumericUpDown_District_Select.Minimum = 0
            NumericUpDown_District_Select.Maximum = SECFile.Districts.Count - 1
            Button_Point_Select.Enabled = True
            Button_District_Select.Enabled = True
            NumericUpDown_Point_Select.Enabled = True
            NumericUpDown_District_Select.Enabled = True
            Button_Point_Create.Enabled = True
            Button_District_Create.Enabled = True
            TextBox_Point_Create.Enabled = True
            TextBox_District_Create.Enabled = True
            Button_Compact.Enabled = True
            ToolStripStatusLabel2.Text = "NumPoint = {0}, NumDistrict = {1}".Formats(SECFile.Points.Count, SECFile.Districts.Count)
        Else
            Menu_File_Save.Enabled = False
            Menu_File_SaveAs.Enabled = False
            Menu_File_Close.Enabled = False
            Menu_View_DistrictDataEditor.Enabled = False

            Button_Point_Select.Enabled = False
            Button_District_Select.Enabled = False
            NumericUpDown_Point_Select.Enabled = False
            NumericUpDown_District_Select.Enabled = False
            Button_Point_Create.Enabled = False
            Button_District_Create.Enabled = False
            TextBox_Point_Create.Enabled = False
            TextBox_District_Create.Enabled = False
            Button_Compact.Enabled = False
            ToolStripStatusLabel2.Text = "NumPoint = 0, NumDistrict = 0"
        End If
        Button_TerrainBrush.Enabled = False
        TextBox_TerrainBrush.Enabled = False
        Button_District_n_Change.Enabled = False
        NumericUpDown_District_n_Change.Enabled = False
        Button_Delete.Enabled = False
    End Sub

    Private Sub Surface_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PicBox.Initialize("SECEditor")
        PicBox.Select()

        ToolTip1.SetToolTip(Me.Button_Point_Select, "按住Ctrl键点击可增加选择或取消选择")
        ToolTip1.SetToolTip(Me.Button_District_Select, "按住Ctrl键点击可增加选择或取消选择")
        ToolTip1.SetToolTip(Me.Button_TerrainBrush, "将选择的所有面都涂为指定地形")
        ToolTip1.SetToolTip(Me.Button_District_n_Change, "修改当前面的边数")
        ToolTip1.SetToolTip(Me.Button_Point_Create, "按照指定坐标创建新点")
        ToolTip1.SetToolTip(Me.Button_District_Create, "按照指定点创建新面")
        ToolTip1.SetToolTip(Me.Button_Compact, "清理所有没有使用的点和面，并重新编号")
    End Sub

    Private Sub Surface_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not SaveSEC() Then Return
        Me.Hide()
        End
    End Sub

    Private Sub Surface_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyData
            Case Keys.Control Or Keys.O
                Menu_File_Open_Click(sender, Nothing)
            Case Keys.Control Or Keys.S
                Menu_File_Save_Click(sender, Nothing)
            Case Keys.Control Or Keys.D
                Menu_View_DistrictDataEditor_Click(sender, Nothing)
        End Select
    End Sub

    Private Sub PicBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles PicBox.KeyUp
        Static LastTime As Double '上次处理的时间 用来处理过量按键等问题
        If DateAndTime.Timer - LastTime < 0.04 Then Exit Sub
        LastTime = DateAndTime.Timer
        Select Case e.KeyData
            Case Keys.F1
                PicBox.NDWorld.ShowText = Not PicBox.NDWorld.ShowText
            Case Keys.Delete
                Button_Delete_Click(Nothing, Nothing)
            Case Else
                PicBox.Graph()
                Return
        End Select
        e.Handled = True
        PicBox.Graph()
    End Sub

    Private Sub PicBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles PicBox.KeyDown
        Static LastTime As Double '上次处理的时间 用来处理过量按键等问题
        If DateAndTime.Timer - LastTime < 0.04 Then Return
        LastTime = DateAndTime.Timer
        Select Case e.KeyData
            Case Else
                PicBox.Graph()
                Return
        End Select
        e.Handled = True
        PicBox.Graph()
    End Sub

    Private Sub PicBox_PointSelected(ByVal Point As SecPoint) Handles PicBox.PointSelected
        Dim PointIndexString = String.Format("P{0}", Point.Index.ToInvariantString())
        ToolStripStatusLabel3.Text = String.Format("{0}({1})", PointIndexString, String.Join(" ", PicBox.SecState.PointToDistrict(Point.Index).Select(Function(dIndex) "D" & dIndex.ToInvariantString()).ToArray()))
        ListBox_Selection.Items.Add(PointIndexString)
        ListBox_Selection.SelectedItem = PointIndexString
        PropertyGridSelectPoint(PicBox.SecState.Sec.Points(Point.Index))
        Button_Delete.Enabled = ListBox_Selection.Items.Count > 0
    End Sub

    Private Sub PicBox_PointUnselected(ByVal Point As SecPoint) Handles PicBox.PointUnselected
        ToolStripStatusLabel3.Text = "None"
        ListBox_Selection.Items.Remove(String.Format("P{0}", Point.Index.ToInvariantString()))
        PropertyGridSelectNone()
        Button_Delete.Enabled = ListBox_Selection.Items.Count > 0
    End Sub

    Private Sub PicBox_DistrictSelected(ByVal District As SecDistrict) Handles PicBox.DistrictSelected
        Dim DistrictIndexString = String.Format("D{0}", District.Index.ToInvariantString())
        ToolStripStatusLabel3.Text = String.Format("{0}({1})", DistrictIndexString, String.Join(" ", PicBox.SecState.DistrictToPoint(District.Index).Select(Function(pIndex) "P" & pIndex.ToInvariantString()).ToArray()))
        ListBox_Selection.Items.Add(DistrictIndexString)
        ListBox_Selection.SelectedItem = DistrictIndexString
        PropertyGridSelectDistrict(PicBox.SecState.Sec.Districts(District.Index))

        Dim TerrainBrushEnabled = ListBox_Selection.Items.OfType(Of String).Any(Function(s) s.StartsWith("D"))
        Button_TerrainBrush.Enabled = TerrainBrushEnabled
        TextBox_TerrainBrush.Enabled = TerrainBrushEnabled

        Button_Delete.Enabled = ListBox_Selection.Items.Count > 0
    End Sub

    Private Sub PicBox_DistrictUnselected(ByVal District As SecDistrict) Handles PicBox.DistrictUnselected
        ToolStripStatusLabel3.Text = "None"
        ListBox_Selection.Items.Remove(String.Format("D{0}", District.Index.ToInvariantString()))
        PropertyGridSelectNone()

        Dim TerrainBrushEnabled = ListBox_Selection.Items.OfType(Of String).Any(Function(s) s.StartsWith("D"))
        Button_TerrainBrush.Enabled = TerrainBrushEnabled
        TextBox_TerrainBrush.Enabled = TerrainBrushEnabled

        Button_Delete.Enabled = ListBox_Selection.Items.Count > 0
    End Sub

    Private Sub ListBox_Selection_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox_Selection.SelectedIndexChanged
        Dim o = ListBox_Selection.SelectedItem
        If o Is Nothing Then Return
        If Not TypeOf o Is String Then Return
        Dim s = DirectCast(o, String)
        If s.StartsWith("P") Then
            Dim PointIndex = Integer.Parse(s.Substring(1))
            PropertyGridSelectPoint(PicBox.SecState.Sec.Points(PointIndex))
        ElseIf s.StartsWith("D") Then
            Dim DistrictIndex = Integer.Parse(s.Substring(1))
            PropertyGridSelectDistrict(PicBox.SecState.Sec.Districts(DistrictIndex))
        End If
    End Sub

    Private Sub PropertyGridSelectPoint(ByVal p As SEC_Simple.PointInfo)
        PropertyGrid.SelectedObject = p

        Button_District_n_Change.Enabled = False
        NumericUpDown_District_n_Change.Enabled = False
    End Sub
    Private Sub PropertyGridSelectDistrict(ByVal d As SEC_Simple.DistrictInfo)
        PropertyGrid.SelectedObject = d

        Button_District_n_Change.Enabled = True
        NumericUpDown_District_n_Change.Enabled = True
    End Sub
    Private Sub PropertyGridSelectNone()
        PropertyGrid.SelectedObject = Nothing

        Button_District_n_Change.Enabled = False
        NumericUpDown_District_n_Change.Enabled = False
    End Sub

    Private Sub PicBox_ControlModeChanged(ByVal ControlMode As Grapher.ControlModeEnum) Handles PicBox.ControlModeChanged
        ToolStripStatusLabel1.Text = "ControlMode " & ControlMode.ToString()
    End Sub

    Public Sub CreateSEC(ByVal VersionSign As SEC.Version)
        SECPath = "无标题.sec"

        Dim SECFile = New SEC_Simple.FileInfo
        SECFile.VersionSign = VersionSign
        SECFile.Tokens = New List(Of String)()
        SECFile.Points = New List(Of SEC_Simple.PointInfo)()
        SECFile.Districts = New List(Of SEC_Simple.DistrictInfo)()

        AttachToSEC(SECFile)
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
    End Sub
    Public Function OpenSEC() As Boolean
        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "SEC(*.SEC;*.xml)|*.SEC;*.sec;*.xml;*.XML"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            OpenSEC(d.FileName)
            Return True
        End If
        Return False
    End Function
    Public Sub OpenSEC(ByVal Path As String)
        SECPath = Path
        If GetExtendedFileName(Path).ToLower() = "xml" Then
            SECXml = True
        Else
            SECXml = False
        End If
        Dim SECFile As SEC_Simple.FileInfo
        If SECXml Then
            SECFile = SEC_Simple.OpenXml(Path)
        Else
            SECFile = SEC_Simple.Open(Path)
        End If
        AttachToSEC(SECFile)
        Me.Text = "SECEditor - " & SECPath
        Changed = False
    End Sub
    Public Function SaveSEC() As Boolean
        Return SaveAsSEC(SECPath)
    End Function
    Public Function SaveAsSEC(ByVal Path As String) As Boolean
        If PicBox.SecState Is Nothing Then Return True
        Dim SaveAs = Not Path Is SECPath
        If Changed OrElse SaveAs Then
            If Path = "" Then
                Static d As Windows.Forms.SaveFileDialog
                If d Is Nothing Then d = New Windows.Forms.SaveFileDialog
                d.Filter = "SEC(*.SEC;*.xml)|*.SEC;*.sec;*.xml;*.XML"
                If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    Path = d.FileName
                    SECXml = GetExtendedFileName(d.FileName).Equals("xml", StringComparison.OrdinalIgnoreCase)
                Else
                    Return False
                End If
            End If
            If Not SaveAs Then
                Dim r = MsgBox("SEC文件已修改, 保存吗?", MsgBoxStyle.YesNoCancel)
                Select Case r
                    Case MsgBoxResult.Yes

                    Case MsgBoxResult.No
                        Return True
                    Case Else
                        Return False
                End Select
            End If
            If SECXml Then
                SEC_Simple.SaveXml(Path, PicBox.SecState.Sec)
            Else
                SEC_Simple.Save(Path, PicBox.SecState.Sec)
            End If
            Changed = False
            SECPath = Path
            Me.Text = "SECEditor - " & Path
        End If
        Return True
    End Function

    Private Sub Menu_File_Open_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Open.Click
        If Not SaveSEC() Then Return

        If OpenSEC() Then
            If Menu_GraphicsInterface_GDIP.Checked Then
                PicBox.UseGDIP()
            ElseIf Menu_GraphicsInterface_SlimDX.Checked Then
                PicBox.UseD3D()
            End If
            Me.Text = "SECEditor - " & SECPath

            Menu_File_Save.Enabled = True
            Menu_File_Close.Enabled = True
            Menu_View_DistrictDataEditor.Enabled = True
        End If
    End Sub


    Private Sub Menu_File_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Exit.Click
        If Not SaveSEC() Then Return
        Me.Hide()
        End
    End Sub

    Private Sub Menu_GraphicsInterface_GDIP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_GraphicsInterface_GDIP.Click
        Menu_GraphicsInterface_GDIP.Checked = True
        Menu_GraphicsInterface_SlimDX.Checked = False
        PicBox.UseGDIP()
    End Sub

    Private Sub Menu_GraphicsInterface_SlimDX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_GraphicsInterface_SlimDX.Click
        Menu_GraphicsInterface_GDIP.Checked = False
        Menu_GraphicsInterface_SlimDX.Checked = True
        PicBox.UseD3D()
    End Sub

    Private Sub Menu_File_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Close.Click
        If Not SaveSEC() Then Return

        AttachToSEC(Nothing)
        SECPath = Nothing
        Me.Text = "SECEditor"

        Menu_File_Save.Enabled = False
        Menu_File_Close.Enabled = False
        Menu_View_DistrictDataEditor.Enabled = False
    End Sub

    Private Sub Menu_View_InitView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_View_InitView.Click
        PicBox.ResetView()
        PicBox.Graph()
    End Sub

    Private Sub Menu_New_Comm2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Menu_New_Comm2.Click
        If Not SaveSEC() Then Return
        CreateSEC(SEC.Version.Comm2)
    End Sub

    Private Sub Menu_New_Comm2Demo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Menu_New_Comm2Demo.Click
        If Not SaveSEC() Then Return
        CreateSEC(SEC.Version.Comm2Demo)
    End Sub

    Private Sub Menu_New_Comm3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Menu_New_Comm3.Click
        If Not SaveSEC() Then Return
        CreateSEC(SEC.Version.Comm3)
    End Sub

    Private Sub Menu_File_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Save.Click
        SaveSEC()
    End Sub

    Private Sub Menu_SaveAs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Menu_File_SaveAs.Click
        SaveAsSEC("")
    End Sub

    Private Sub PropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid.PropertyValueChanged
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        If TypeOf PropertyGrid.SelectedObject Is SEC_Simple.DistrictInfo Then
            Dim District = DirectCast(PropertyGrid.SelectedObject, SEC_Simple.DistrictInfo)
            PicBox.NotifyDistrictUpdated(PicBox.SecState.Sec.Districts.FindIndex(Function(d) d Is District))
            My.Forms.DistrictDataEditor.RefreshRowByDistrict(District)
        End If
        PicBox.Graph()
    End Sub

    Private Sub Menu_View_DistrictDataEditor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Menu_View_DistrictDataEditor.Click
        If PicBox.SecState Is Nothing Then Return
        My.Forms.DistrictDataEditor.Show()
        My.Forms.DistrictDataEditor.Activate()
    End Sub

    Private Sub Surface_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        If My.Forms.DistrictDataEditor.Changed Then
            Changed = True
            My.Forms.DistrictDataEditor.Changed = False
            Me.Text = "SECEditor - " & SECPath & "*"
            PropertyGrid.SelectedObject = PropertyGrid.SelectedObject
            PicBox.Graph()
        End If
    End Sub

    Private Sub Button_Point_Select_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Point_Select.Click
        Dim v = CInt(NumericUpDown_Point_Select.Value)
        If v >= 0 AndAlso v < PicBox.SecState.SecPoints.Count Then
            PicBox.Select()
            PicBox.SelectPoint(PicBox.SecState.SecPoints(v), (Control.ModifierKeys And Keys.Control) <> 0)
            PicBox.FocusOn()
            PicBox.Graph()
        End If
    End Sub

    Private Sub Button_District_Select_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_District_Select.Click
        Dim v = CInt(NumericUpDown_District_Select.Value)
        If v >= 0 AndAlso v < PicBox.SecState.SecDistricts.Count Then
            PicBox.Select()
            PicBox.SelectDistrict(PicBox.SecState.SecDistricts(v), (Control.ModifierKeys And Keys.Control) <> 0)
            PicBox.FocusOn()
            PicBox.Graph()
        End If
    End Sub

    Private Sub Button_TerrainBrush_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_TerrainBrush.Click
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        Dim Terrain = SEC.TerrainInfo.FromString(TextBox_TerrainBrush.Text.Trim(" "))
        For Each o In ListBox_Selection.Items
            If TypeOf o Is String Then
                Dim s = DirectCast(o, String)
                If s.StartsWith("D") Then
                    Dim dIndex = Integer.Parse(s.Substring(1))
                    PicBox.SecState.Sec.Districts(dIndex).Terrain = Terrain
                End If
            End If
        Next
        PicBox.Graph()
    End Sub

    Private Sub Button_District_n_Change_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_District_n_Change.Click
        Dim n = CInt(NumericUpDown_District_n_Change.Value)
        Dim District = DirectCast(PropertyGrid.SelectedObject, SEC_Simple.DistrictInfo)
        If n = District.n Then Return
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        If n < District.n Then
            Dim LastEndPointIndex = 0
            If District.Borders.Count > 0 Then
                LastEndPointIndex = District.Borders(District.Borders.Count - 1).EndPointIndex
            End If
            District.Borders.RemoveRange(n, District.n - n)
            If District.Borders.Count > 0 Then
                District.Borders(District.Borders.Count - 1).EndPointIndex = LastEndPointIndex
            End If
        Else
            Dim LastEndPointIndex = 0
            If District.Borders.Count > 0 Then
                LastEndPointIndex = District.Borders(District.Borders.Count - 1).EndPointIndex
                District.Borders(District.Borders.Count - 1).EndPointIndex = 0
            End If
            For k = District.n To n - 1
                District.Borders.Add(New SEC_Simple.BorderInfo)
            Next
            District.Borders(District.Borders.Count - 1).EndPointIndex = LastEndPointIndex
        End If
        PicBox.NotifyDistrictUpdated(PicBox.SecState.Sec.Districts.FindIndex(Function(d) d Is District))
        My.Forms.DistrictDataEditor.RefreshRowByDistrict(District)
        PropertyGridSelectDistrict(District)
        PicBox.Graph()
    End Sub

    Private Sub Button_Point_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Point_Create.Click
        Dim PosArray = TextBox_Point_Create.Text.Split(",").Select(Function(v) Single.Parse(v.Trim(" "))).ToArray()
        If PosArray.Length <> 2 Then Throw New InvalidOperationException
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        Dim pIndex = PicBox.CreatePoint(PosArray)
        ToolStripStatusLabel2.Text = "NumPoint = {0}, NumDistrict = {1}".Formats(PicBox.SecState.Sec.Points.Count, PicBox.SecState.Sec.Districts.Count)
        PicBox.Select()
        PicBox.SelectPoint(PicBox.SecState.SecPoints(pIndex), True)
        PicBox.FocusOn()
        PicBox.Graph()
    End Sub

    Private Sub Button_District_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_District_Create.Click
        Dim PointIndices = TextBox_District_Create.Text.Split(",").Select(Function(v) Integer.Parse(v.Trim(" "))).ToArray()
        If PointIndices.Length < 3 Then Throw New InvalidOperationException
        If PointIndices.Any(Function(i) i < 0 OrElse i >= PicBox.SecState.Sec.Points.Count) Then Throw New InvalidOperationException
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        Dim dIndex = PicBox.CreateDistrict(PointIndices)
        ToolStripStatusLabel2.Text = "NumPoint = {0}, NumDistrict = {1}".Formats(PicBox.SecState.Sec.Points.Count, PicBox.SecState.Sec.Districts.Count)
        PicBox.Select()
        PicBox.SelectDistrict(PicBox.SecState.SecDistricts(dIndex), True)
        PicBox.FocusOn()
        PicBox.Graph()
    End Sub

    Private Sub Button_Compact_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Compact.Click
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        PicBox.Compact()
        PropertyGridSelectNone()
        PicBox.Select()
        PicBox.Graph()
    End Sub

    Private Sub Button_Delete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Delete.Click
        Me.Text = "SECEditor - " & SECPath & "*"
        Changed = True
        PicBox.DeleteSelected()
        ListBox_Selection.Items.Clear()
        PropertyGridSelectNone()
        PicBox.Graph()
    End Sub
End Class
