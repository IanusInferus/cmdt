<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Surface
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Surface))
        Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.Menu_File = New System.Windows.Forms.MenuItem()
        Me.Menu_New = New System.Windows.Forms.MenuItem()
        Me.Menu_New_Comm2 = New System.Windows.Forms.MenuItem()
        Me.Menu_New_Comm2Demo = New System.Windows.Forms.MenuItem()
        Me.Menu_New_Comm3 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Open = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Save = New System.Windows.Forms.MenuItem()
        Me.Menu_File_SaveAs = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Close = New System.Windows.Forms.MenuItem()
        Me.MenuItem3 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Exit = New System.Windows.Forms.MenuItem()
        Me.Menu_View = New System.Windows.Forms.MenuItem()
        Me.Menu_View_InitView = New System.Windows.Forms.MenuItem()
        Me.Menu_View_DistrictDataEditor = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface_GDIP = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface_SlimDX = New System.Windows.Forms.MenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.PicBox = New SECEditor.Picker()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button_Compact = New System.Windows.Forms.Button()
        Me.Label_District_Create = New System.Windows.Forms.Label()
        Me.Label_Point_Create = New System.Windows.Forms.Label()
        Me.Label_TerrainBrush = New System.Windows.Forms.Label()
        Me.TextBox_District_Create = New System.Windows.Forms.TextBox()
        Me.TextBox_Point_Create = New System.Windows.Forms.TextBox()
        Me.TextBox_TerrainBrush = New System.Windows.Forms.TextBox()
        Me.Button_District_Create = New System.Windows.Forms.Button()
        Me.Button_Point_Create = New System.Windows.Forms.Button()
        Me.Button_TerrainBrush = New System.Windows.Forms.Button()
        Me.NumericUpDown_District_n_Change = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown_District_Select = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown_Point_Select = New System.Windows.Forms.NumericUpDown()
        Me.Label_District_n_Change = New System.Windows.Forms.Label()
        Me.Label_District_Select = New System.Windows.Forms.Label()
        Me.Label_Point_Select = New System.Windows.Forms.Label()
        Me.Button_District_n_Change = New System.Windows.Forms.Button()
        Me.Button_District_Select = New System.Windows.Forms.Button()
        Me.Button_Point_Select = New System.Windows.Forms.Button()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.ListBox_Selection = New System.Windows.Forms.ListBox()
        Me.PropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Button_Delete = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.NumericUpDown_District_n_Change, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown_District_Select, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown_Point_Select, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_File, Me.Menu_View, Me.Menu_GraphicsInterface})
        '
        'Menu_File
        '
        Me.Menu_File.Index = 0
        Me.Menu_File.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_New, Me.Menu_File_Open, Me.Menu_File_Save, Me.Menu_File_SaveAs, Me.Menu_File_Close, Me.MenuItem3, Me.Menu_File_Exit})
        Me.Menu_File.Text = "文件(&F)"
        '
        'Menu_New
        '
        Me.Menu_New.Index = 0
        Me.Menu_New.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_New_Comm2, Me.Menu_New_Comm2Demo, Me.Menu_New_Comm3})
        Me.Menu_New.Text = "新建(&N)"
        '
        'Menu_New_Comm2
        '
        Me.Menu_New_Comm2.Index = 0
        Me.Menu_New_Comm2.Text = "盟军2(&2)" & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+N"
        '
        'Menu_New_Comm2Demo
        '
        Me.Menu_New_Comm2Demo.Index = 1
        Me.Menu_New_Comm2Demo.Text = "盟军2测试版(&D)"
        '
        'Menu_New_Comm3
        '
        Me.Menu_New_Comm3.Index = 2
        Me.Menu_New_Comm3.Text = "盟军3(&3)"
        '
        'Menu_File_Open
        '
        Me.Menu_File_Open.Index = 1
        Me.Menu_File_Open.Text = "打开(&O)..." & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+O"
        '
        'Menu_File_Save
        '
        Me.Menu_File_Save.Enabled = False
        Me.Menu_File_Save.Index = 2
        Me.Menu_File_Save.Text = "保存(&S)" & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+S"
        '
        'Menu_File_SaveAs
        '
        Me.Menu_File_SaveAs.Enabled = False
        Me.Menu_File_SaveAs.Index = 3
        Me.Menu_File_SaveAs.Text = "另存为(&A)..."
        '
        'Menu_File_Close
        '
        Me.Menu_File_Close.Enabled = False
        Me.Menu_File_Close.Index = 4
        Me.Menu_File_Close.Text = "关闭(&C)"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 5
        Me.MenuItem3.Text = "-"
        '
        'Menu_File_Exit
        '
        Me.Menu_File_Exit.Index = 6
        Me.Menu_File_Exit.Text = "退出(&X)"
        '
        'Menu_View
        '
        Me.Menu_View.Index = 1
        Me.Menu_View.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_View_InitView, Me.Menu_View_DistrictDataEditor})
        Me.Menu_View.Text = "查看(&V)"
        '
        'Menu_View_InitView
        '
        Me.Menu_View_InitView.Index = 0
        Me.Menu_View_InitView.Text = "初始视图(&O)"
        '
        'Menu_View_DistrictDataEditor
        '
        Me.Menu_View_DistrictDataEditor.Enabled = False
        Me.Menu_View_DistrictDataEditor.Index = 1
        Me.Menu_View_DistrictDataEditor.Text = "地形数据编辑器(&D)..." & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+D"
        '
        'Menu_GraphicsInterface
        '
        Me.Menu_GraphicsInterface.Index = 2
        Me.Menu_GraphicsInterface.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_GraphicsInterface_GDIP, Me.Menu_GraphicsInterface_SlimDX})
        Me.Menu_GraphicsInterface.Text = "图形接口(&I)"
        '
        'Menu_GraphicsInterface_GDIP
        '
        Me.Menu_GraphicsInterface_GDIP.Checked = True
        Me.Menu_GraphicsInterface_GDIP.Index = 0
        Me.Menu_GraphicsInterface_GDIP.RadioCheck = True
        Me.Menu_GraphicsInterface_GDIP.Text = "&GDI+"
        '
        'Menu_GraphicsInterface_SlimDX
        '
        Me.Menu_GraphicsInterface_SlimDX.Index = 1
        Me.Menu_GraphicsInterface_SlimDX.RadioCheck = True
        Me.Menu_GraphicsInterface_SlimDX.Text = "&SlimDX"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.PicBox)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(928, 392)
        Me.SplitContainer1.SplitterDistance = 571
        Me.SplitContainer1.TabIndex = 1
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PicBox.KeyPressTimeLimit = 0.2R
        Me.PicBox.Location = New System.Drawing.Point(0, 0)
        Me.PicBox.MultiClickTimeLimit = 0.4R
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(571, 392)
        Me.PicBox.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.Panel1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(353, 392)
        Me.SplitContainer2.SplitterDistance = 220
        Me.SplitContainer2.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Button_Delete)
        Me.Panel1.Controls.Add(Me.Button_Compact)
        Me.Panel1.Controls.Add(Me.Label_District_Create)
        Me.Panel1.Controls.Add(Me.Label_Point_Create)
        Me.Panel1.Controls.Add(Me.Label_TerrainBrush)
        Me.Panel1.Controls.Add(Me.TextBox_District_Create)
        Me.Panel1.Controls.Add(Me.TextBox_Point_Create)
        Me.Panel1.Controls.Add(Me.TextBox_TerrainBrush)
        Me.Panel1.Controls.Add(Me.Button_District_Create)
        Me.Panel1.Controls.Add(Me.Button_Point_Create)
        Me.Panel1.Controls.Add(Me.Button_TerrainBrush)
        Me.Panel1.Controls.Add(Me.NumericUpDown_District_n_Change)
        Me.Panel1.Controls.Add(Me.NumericUpDown_District_Select)
        Me.Panel1.Controls.Add(Me.NumericUpDown_Point_Select)
        Me.Panel1.Controls.Add(Me.Label_District_n_Change)
        Me.Panel1.Controls.Add(Me.Label_District_Select)
        Me.Panel1.Controls.Add(Me.Label_Point_Select)
        Me.Panel1.Controls.Add(Me.Button_District_n_Change)
        Me.Panel1.Controls.Add(Me.Button_District_Select)
        Me.Panel1.Controls.Add(Me.Button_Point_Select)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(353, 220)
        Me.Panel1.TabIndex = 0
        '
        'Button_Compact
        '
        Me.Button_Compact.Location = New System.Drawing.Point(177, 187)
        Me.Button_Compact.Name = "Button_Compact"
        Me.Button_Compact.Size = New System.Drawing.Size(75, 23)
        Me.Button_Compact.TabIndex = 18
        Me.Button_Compact.Text = "Compact"
        Me.Button_Compact.UseVisualStyleBackColor = True
        '
        'Label_District_Create
        '
        Me.Label_District_Create.AutoSize = True
        Me.Label_District_Create.Location = New System.Drawing.Point(13, 163)
        Me.Label_District_Create.Name = "Label_District_Create"
        Me.Label_District_Create.Size = New System.Drawing.Size(53, 12)
        Me.Label_District_Create.TabIndex = 15
        Me.Label_District_Create.Text = "District"
        '
        'Label_Point_Create
        '
        Me.Label_Point_Create.AutoSize = True
        Me.Label_Point_Create.Location = New System.Drawing.Point(13, 134)
        Me.Label_Point_Create.Name = "Label_Point_Create"
        Me.Label_Point_Create.Size = New System.Drawing.Size(35, 12)
        Me.Label_Point_Create.TabIndex = 12
        Me.Label_Point_Create.Text = "Point"
        '
        'Label_TerrainBrush
        '
        Me.Label_TerrainBrush.AutoSize = True
        Me.Label_TerrainBrush.Location = New System.Drawing.Point(13, 76)
        Me.Label_TerrainBrush.Name = "Label_TerrainBrush"
        Me.Label_TerrainBrush.Size = New System.Drawing.Size(47, 12)
        Me.Label_TerrainBrush.TabIndex = 6
        Me.Label_TerrainBrush.Text = "Terrain"
        '
        'TextBox_District_Create
        '
        Me.TextBox_District_Create.Location = New System.Drawing.Point(86, 160)
        Me.TextBox_District_Create.Name = "TextBox_District_Create"
        Me.TextBox_District_Create.Size = New System.Drawing.Size(166, 21)
        Me.TextBox_District_Create.TabIndex = 16
        Me.TextBox_District_Create.Text = "0, 1, 2, 3"
        '
        'TextBox_Point_Create
        '
        Me.TextBox_Point_Create.Location = New System.Drawing.Point(86, 131)
        Me.TextBox_Point_Create.Name = "TextBox_Point_Create"
        Me.TextBox_Point_Create.Size = New System.Drawing.Size(166, 21)
        Me.TextBox_Point_Create.TabIndex = 13
        Me.TextBox_Point_Create.Text = "0, 0"
        '
        'TextBox_TerrainBrush
        '
        Me.TextBox_TerrainBrush.Location = New System.Drawing.Point(86, 73)
        Me.TextBox_TerrainBrush.Name = "TextBox_TerrainBrush"
        Me.TextBox_TerrainBrush.Size = New System.Drawing.Size(166, 21)
        Me.TextBox_TerrainBrush.TabIndex = 7
        '
        'Button_District_Create
        '
        Me.Button_District_Create.Location = New System.Drawing.Point(275, 158)
        Me.Button_District_Create.Name = "Button_District_Create"
        Me.Button_District_Create.Size = New System.Drawing.Size(75, 23)
        Me.Button_District_Create.TabIndex = 17
        Me.Button_District_Create.Text = "Create"
        Me.Button_District_Create.UseVisualStyleBackColor = True
        '
        'Button_Point_Create
        '
        Me.Button_Point_Create.Location = New System.Drawing.Point(275, 129)
        Me.Button_Point_Create.Name = "Button_Point_Create"
        Me.Button_Point_Create.Size = New System.Drawing.Size(75, 23)
        Me.Button_Point_Create.TabIndex = 14
        Me.Button_Point_Create.Text = "Create"
        Me.Button_Point_Create.UseVisualStyleBackColor = True
        '
        'Button_TerrainBrush
        '
        Me.Button_TerrainBrush.Location = New System.Drawing.Point(275, 71)
        Me.Button_TerrainBrush.Name = "Button_TerrainBrush"
        Me.Button_TerrainBrush.Size = New System.Drawing.Size(75, 23)
        Me.Button_TerrainBrush.TabIndex = 8
        Me.Button_TerrainBrush.Text = "Brush"
        Me.Button_TerrainBrush.UseVisualStyleBackColor = True
        '
        'NumericUpDown_District_n_Change
        '
        Me.NumericUpDown_District_n_Change.Location = New System.Drawing.Point(86, 103)
        Me.NumericUpDown_District_n_Change.Maximum = New Decimal(New Integer() {16, 0, 0, 0})
        Me.NumericUpDown_District_n_Change.Name = "NumericUpDown_District_n_Change"
        Me.NumericUpDown_District_n_Change.Size = New System.Drawing.Size(166, 21)
        Me.NumericUpDown_District_n_Change.TabIndex = 10
        Me.NumericUpDown_District_n_Change.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.NumericUpDown_District_n_Change.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'NumericUpDown_District_Select
        '
        Me.NumericUpDown_District_Select.Location = New System.Drawing.Point(86, 45)
        Me.NumericUpDown_District_Select.Name = "NumericUpDown_District_Select"
        Me.NumericUpDown_District_Select.Size = New System.Drawing.Size(166, 21)
        Me.NumericUpDown_District_Select.TabIndex = 4
        Me.NumericUpDown_District_Select.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'NumericUpDown_Point_Select
        '
        Me.NumericUpDown_Point_Select.Location = New System.Drawing.Point(86, 16)
        Me.NumericUpDown_Point_Select.Name = "NumericUpDown_Point_Select"
        Me.NumericUpDown_Point_Select.Size = New System.Drawing.Size(166, 21)
        Me.NumericUpDown_Point_Select.TabIndex = 1
        Me.NumericUpDown_Point_Select.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label_District_n_Change
        '
        Me.Label_District_n_Change.AutoSize = True
        Me.Label_District_n_Change.Location = New System.Drawing.Point(13, 105)
        Me.Label_District_n_Change.Name = "Label_District_n_Change"
        Me.Label_District_n_Change.Size = New System.Drawing.Size(65, 12)
        Me.Label_District_n_Change.TabIndex = 9
        Me.Label_District_n_Change.Text = "District.n"
        '
        'Label_District_Select
        '
        Me.Label_District_Select.AutoSize = True
        Me.Label_District_Select.Location = New System.Drawing.Point(13, 47)
        Me.Label_District_Select.Name = "Label_District_Select"
        Me.Label_District_Select.Size = New System.Drawing.Size(53, 12)
        Me.Label_District_Select.TabIndex = 3
        Me.Label_District_Select.Text = "District"
        '
        'Label_Point_Select
        '
        Me.Label_Point_Select.AutoSize = True
        Me.Label_Point_Select.Location = New System.Drawing.Point(13, 18)
        Me.Label_Point_Select.Name = "Label_Point_Select"
        Me.Label_Point_Select.Size = New System.Drawing.Size(35, 12)
        Me.Label_Point_Select.TabIndex = 0
        Me.Label_Point_Select.Text = "Point"
        '
        'Button_District_n_Change
        '
        Me.Button_District_n_Change.Location = New System.Drawing.Point(275, 100)
        Me.Button_District_n_Change.Name = "Button_District_n_Change"
        Me.Button_District_n_Change.Size = New System.Drawing.Size(75, 23)
        Me.Button_District_n_Change.TabIndex = 11
        Me.Button_District_n_Change.Text = "Change"
        Me.Button_District_n_Change.UseVisualStyleBackColor = True
        '
        'Button_District_Select
        '
        Me.Button_District_Select.Location = New System.Drawing.Point(275, 42)
        Me.Button_District_Select.Name = "Button_District_Select"
        Me.Button_District_Select.Size = New System.Drawing.Size(75, 23)
        Me.Button_District_Select.TabIndex = 5
        Me.Button_District_Select.Text = "Select"
        Me.Button_District_Select.UseVisualStyleBackColor = True
        '
        'Button_Point_Select
        '
        Me.Button_Point_Select.Location = New System.Drawing.Point(275, 13)
        Me.Button_Point_Select.Name = "Button_Point_Select"
        Me.Button_Point_Select.Size = New System.Drawing.Size(75, 23)
        Me.Button_Point_Select.TabIndex = 2
        Me.Button_Point_Select.Text = "Select"
        Me.Button_Point_Select.UseVisualStyleBackColor = True
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.ListBox_Selection)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.PropertyGrid)
        Me.SplitContainer3.Size = New System.Drawing.Size(353, 168)
        Me.SplitContainer3.SplitterDistance = 71
        Me.SplitContainer3.TabIndex = 3
        '
        'ListBox_Selection
        '
        Me.ListBox_Selection.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox_Selection.FormattingEnabled = True
        Me.ListBox_Selection.ItemHeight = 12
        Me.ListBox_Selection.Location = New System.Drawing.Point(0, 0)
        Me.ListBox_Selection.Name = "ListBox_Selection"
        Me.ListBox_Selection.Size = New System.Drawing.Size(71, 168)
        Me.ListBox_Selection.TabIndex = 0
        '
        'PropertyGrid
        '
        Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid.HelpVisible = False
        Me.PropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid.Name = "PropertyGrid"
        Me.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGrid.Size = New System.Drawing.Size(278, 168)
        Me.PropertyGrid.TabIndex = 0
        Me.PropertyGrid.ToolbarVisible = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel3})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 395)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(928, 26)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(126, 21)
        Me.ToolStripStatusLabel1.Text = "ControlMode XXXX"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(206, 21)
        Me.ToolStripStatusLabel2.Text = "NumPoint = XX, NumDistrict = XX"
        '
        'ToolStripStatusLabel3
        '
        Me.ToolStripStatusLabel3.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        Me.ToolStripStatusLabel3.Size = New System.Drawing.Size(44, 21)
        Me.ToolStripStatusLabel3.Text = "None"
        '
        'Button_Delete
        '
        Me.Button_Delete.Location = New System.Drawing.Point(275, 187)
        Me.Button_Delete.Name = "Button_Delete"
        Me.Button_Delete.Size = New System.Drawing.Size(75, 23)
        Me.Button_Delete.TabIndex = 18
        Me.Button_Delete.Text = "Delete"
        Me.Button_Delete.UseVisualStyleBackColor = True
        '
        'Surface
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(928, 421)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Menu = Me.MainMenu1
        Me.Name = "Surface"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SECEditor"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.NumericUpDown_District_n_Change, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown_District_Select, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown_Point_Select, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PicBox As Picker
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents Menu_File As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Open As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Exit As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_GraphicsInterface As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_GraphicsInterface_GDIP As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_GraphicsInterface_SlimDX As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Close As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_View As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_View_InitView As System.Windows.Forms.MenuItem
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents PropertyGrid As System.Windows.Forms.PropertyGrid
    Friend WithEvents Menu_File_Save As System.Windows.Forms.MenuItem
    Friend WithEvents ToolStripStatusLabel3 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Menu_View_DistrictDataEditor As System.Windows.Forms.MenuItem
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListBox_Selection As System.Windows.Forms.ListBox
    Friend WithEvents NumericUpDown_District_Select As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpDown_Point_Select As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label_District_Select As System.Windows.Forms.Label
    Friend WithEvents Label_Point_Select As System.Windows.Forms.Label
    Friend WithEvents Button_District_Select As System.Windows.Forms.Button
    Friend WithEvents Button_Point_Select As System.Windows.Forms.Button
    Friend WithEvents Menu_New As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_New_Comm2 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_New_Comm2Demo As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_New_Comm3 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_SaveAs As System.Windows.Forms.MenuItem
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents Label_TerrainBrush As System.Windows.Forms.Label
    Friend WithEvents TextBox_TerrainBrush As System.Windows.Forms.TextBox
    Friend WithEvents Button_TerrainBrush As System.Windows.Forms.Button
    Friend WithEvents Button_Compact As System.Windows.Forms.Button
    Friend WithEvents NumericUpDown_District_n_Change As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label_District_n_Change As System.Windows.Forms.Label
    Friend WithEvents Button_District_n_Change As System.Windows.Forms.Button
    Friend WithEvents Label_District_Create As System.Windows.Forms.Label
    Friend WithEvents Label_Point_Create As System.Windows.Forms.Label
    Friend WithEvents TextBox_District_Create As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Point_Create As System.Windows.Forms.TextBox
    Friend WithEvents Button_District_Create As System.Windows.Forms.Button
    Friend WithEvents Button_Point_Create As System.Windows.Forms.Button
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Button_Delete As System.Windows.Forms.Button

End Class
