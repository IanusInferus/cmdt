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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.NumericUpDown_Point_Select = New System.Windows.Forms.NumericUpDown()
        Me.Button_District_Create = New System.Windows.Forms.Button()
        Me.TextBox_District_Create = New System.Windows.Forms.TextBox()
        Me.Label_District_Create = New System.Windows.Forms.Label()
        Me.Button_Point_Select = New System.Windows.Forms.Button()
        Me.Label_Point_Create = New System.Windows.Forms.Label()
        Me.TextBox_Point_Create = New System.Windows.Forms.TextBox()
        Me.Button_Point_Create = New System.Windows.Forms.Button()
        Me.Label_District_Select = New System.Windows.Forms.Label()
        Me.Label_TerrainBrush = New System.Windows.Forms.Label()
        Me.NumericUpDown_District_Select = New System.Windows.Forms.NumericUpDown()
        Me.TextBox_TerrainBrush = New System.Windows.Forms.TextBox()
        Me.Button_District_Select = New System.Windows.Forms.Button()
        Me.Button_District_n_Change = New System.Windows.Forms.Button()
        Me.NumericUpDown_District_n_Change = New System.Windows.Forms.NumericUpDown()
        Me.Button_TerrainBrush = New System.Windows.Forms.Button()
        Me.Label_District_n_Change = New System.Windows.Forms.Label()
        Me.Label_Point_Select = New System.Windows.Forms.Label()
        Me.Button_Compact = New System.Windows.Forms.Button()
        Me.Button_Delete = New System.Windows.Forms.Button()
        Me.Button_Split = New System.Windows.Forms.Button()
        Me.Button_Merge = New System.Windows.Forms.Button()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.ListBox_Selection = New System.Windows.Forms.ListBox()
        Me.PropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToolStripStatusLabel4 = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.NumericUpDown_Point_Select, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown_District_Select, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown_District_n_Change, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.PicBox)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Panel2MinSize = 340
        Me.SplitContainer1.Size = New System.Drawing.Size(1856, 1066)
        Me.SplitContainer1.SplitterDistance = 1162
        Me.SplitContainer1.SplitterWidth = 8
        Me.SplitContainer1.TabIndex = 1
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PicBox.KeyPressTimeLimit = 0.2R
        Me.PicBox.Location = New System.Drawing.Point(0, 0)
        Me.PicBox.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.PicBox.MultiClickTimeLimit = 0.4R
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(1162, 1066)
        Me.PicBox.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer2.Panel1MinSize = 250
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(686, 1066)
        Me.SplitContainer2.SplitterDistance = 500
        Me.SplitContainer2.SplitterWidth = 8
        Me.SplitContainer2.TabIndex = 2
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoScroll = True
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.NumericUpDown_Point_Select, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_District_Create, 2, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_District_Create, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_District_Create, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Point_Select, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_Point_Create, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Point_Create, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Point_Create, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_District_Select, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_TerrainBrush, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.NumericUpDown_District_Select, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_TerrainBrush, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_District_Select, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_District_n_Change, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.NumericUpDown_District_n_Change, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_TerrainBrush, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_District_n_Change, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label_Point_Select, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Compact, 2, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Delete, 1, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Split, 2, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Button_Merge, 1, 6)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(24, 24, 24, 24)
        Me.TableLayoutPanel1.RowCount = 8
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(686, 500)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'NumericUpDown_Point_Select
        '
        Me.NumericUpDown_Point_Select.Location = New System.Drawing.Point(264, 30)
        Me.NumericUpDown_Point_Select.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.NumericUpDown_Point_Select.Name = "NumericUpDown_Point_Select"
        Me.NumericUpDown_Point_Select.Size = New System.Drawing.Size(224, 35)
        Me.NumericUpDown_Point_Select.TabIndex = 1
        Me.NumericUpDown_Point_Select.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Button_District_Create
        '
        Me.Button_District_Create.Location = New System.Drawing.Point(500, 320)
        Me.Button_District_Create.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_District_Create.Name = "Button_District_Create"
        Me.Button_District_Create.Size = New System.Drawing.Size(150, 46)
        Me.Button_District_Create.TabIndex = 17
        Me.Button_District_Create.Text = "Create"
        Me.ToolTip1.SetToolTip(Me.Button_District_Create, "按照指定点创建新区域")
        Me.Button_District_Create.UseVisualStyleBackColor = True
        '
        'TextBox_District_Create
        '
        Me.TextBox_District_Create.Location = New System.Drawing.Point(264, 320)
        Me.TextBox_District_Create.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.TextBox_District_Create.Name = "TextBox_District_Create"
        Me.TextBox_District_Create.Size = New System.Drawing.Size(220, 35)
        Me.TextBox_District_Create.TabIndex = 16
        Me.TextBox_District_Create.Text = "0, 1, 2, 3"
        '
        'Label_District_Create
        '
        Me.Label_District_Create.Location = New System.Drawing.Point(30, 314)
        Me.Label_District_Create.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_District_Create.Name = "Label_District_Create"
        Me.Label_District_Create.Size = New System.Drawing.Size(222, 52)
        Me.Label_District_Create.TabIndex = 15
        Me.Label_District_Create.Text = "District"
        Me.Label_District_Create.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Button_Point_Select
        '
        Me.Button_Point_Select.Location = New System.Drawing.Point(500, 30)
        Me.Button_Point_Select.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Point_Select.Name = "Button_Point_Select"
        Me.Button_Point_Select.Size = New System.Drawing.Size(150, 46)
        Me.Button_Point_Select.TabIndex = 2
        Me.Button_Point_Select.Text = "Select"
        Me.ToolTip1.SetToolTip(Me.Button_Point_Select, "按住Ctrl键点击可增加选择或取消选择")
        Me.Button_Point_Select.UseVisualStyleBackColor = True
        '
        'Label_Point_Create
        '
        Me.Label_Point_Create.Location = New System.Drawing.Point(30, 256)
        Me.Label_Point_Create.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_Point_Create.Name = "Label_Point_Create"
        Me.Label_Point_Create.Size = New System.Drawing.Size(222, 48)
        Me.Label_Point_Create.TabIndex = 12
        Me.Label_Point_Create.Text = "Point"
        Me.Label_Point_Create.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox_Point_Create
        '
        Me.TextBox_Point_Create.Location = New System.Drawing.Point(264, 262)
        Me.TextBox_Point_Create.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.TextBox_Point_Create.Name = "TextBox_Point_Create"
        Me.TextBox_Point_Create.Size = New System.Drawing.Size(220, 35)
        Me.TextBox_Point_Create.TabIndex = 13
        Me.TextBox_Point_Create.Text = "0, 0"
        '
        'Button_Point_Create
        '
        Me.Button_Point_Create.Location = New System.Drawing.Point(500, 262)
        Me.Button_Point_Create.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Point_Create.Name = "Button_Point_Create"
        Me.Button_Point_Create.Size = New System.Drawing.Size(150, 46)
        Me.Button_Point_Create.TabIndex = 14
        Me.Button_Point_Create.Text = "Create"
        Me.ToolTip1.SetToolTip(Me.Button_Point_Create, "按照指定坐标创建新点")
        Me.Button_Point_Create.UseVisualStyleBackColor = True
        '
        'Label_District_Select
        '
        Me.Label_District_Select.Location = New System.Drawing.Point(30, 82)
        Me.Label_District_Select.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_District_Select.Name = "Label_District_Select"
        Me.Label_District_Select.Size = New System.Drawing.Size(222, 58)
        Me.Label_District_Select.TabIndex = 3
        Me.Label_District_Select.Text = "District"
        Me.Label_District_Select.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label_TerrainBrush
        '
        Me.Label_TerrainBrush.Location = New System.Drawing.Point(30, 140)
        Me.Label_TerrainBrush.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_TerrainBrush.Name = "Label_TerrainBrush"
        Me.Label_TerrainBrush.Size = New System.Drawing.Size(222, 58)
        Me.Label_TerrainBrush.TabIndex = 6
        Me.Label_TerrainBrush.Text = "Terrain"
        Me.Label_TerrainBrush.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NumericUpDown_District_Select
        '
        Me.NumericUpDown_District_Select.Location = New System.Drawing.Point(264, 88)
        Me.NumericUpDown_District_Select.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.NumericUpDown_District_Select.Name = "NumericUpDown_District_Select"
        Me.NumericUpDown_District_Select.Size = New System.Drawing.Size(224, 35)
        Me.NumericUpDown_District_Select.TabIndex = 4
        Me.NumericUpDown_District_Select.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox_TerrainBrush
        '
        Me.TextBox_TerrainBrush.Location = New System.Drawing.Point(264, 146)
        Me.TextBox_TerrainBrush.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.TextBox_TerrainBrush.Name = "TextBox_TerrainBrush"
        Me.TextBox_TerrainBrush.Size = New System.Drawing.Size(220, 35)
        Me.TextBox_TerrainBrush.TabIndex = 7
        '
        'Button_District_Select
        '
        Me.Button_District_Select.Location = New System.Drawing.Point(500, 88)
        Me.Button_District_Select.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_District_Select.Name = "Button_District_Select"
        Me.Button_District_Select.Size = New System.Drawing.Size(150, 46)
        Me.Button_District_Select.TabIndex = 5
        Me.Button_District_Select.Text = "Select"
        Me.ToolTip1.SetToolTip(Me.Button_District_Select, "按住Ctrl键点击可增加选择或取消选择")
        Me.Button_District_Select.UseVisualStyleBackColor = True
        '
        'Button_District_n_Change
        '
        Me.Button_District_n_Change.Location = New System.Drawing.Point(500, 204)
        Me.Button_District_n_Change.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_District_n_Change.Name = "Button_District_n_Change"
        Me.Button_District_n_Change.Size = New System.Drawing.Size(150, 46)
        Me.Button_District_n_Change.TabIndex = 11
        Me.Button_District_n_Change.Text = "Change"
        Me.ToolTip1.SetToolTip(Me.Button_District_n_Change, "修改当前区域的边数")
        Me.Button_District_n_Change.UseVisualStyleBackColor = True
        '
        'NumericUpDown_District_n_Change
        '
        Me.NumericUpDown_District_n_Change.Location = New System.Drawing.Point(264, 204)
        Me.NumericUpDown_District_n_Change.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.NumericUpDown_District_n_Change.Maximum = New Decimal(New Integer() {16, 0, 0, 0})
        Me.NumericUpDown_District_n_Change.Name = "NumericUpDown_District_n_Change"
        Me.NumericUpDown_District_n_Change.Size = New System.Drawing.Size(224, 35)
        Me.NumericUpDown_District_n_Change.TabIndex = 10
        Me.NumericUpDown_District_n_Change.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.NumericUpDown_District_n_Change.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'Button_TerrainBrush
        '
        Me.Button_TerrainBrush.Location = New System.Drawing.Point(500, 146)
        Me.Button_TerrainBrush.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_TerrainBrush.Name = "Button_TerrainBrush"
        Me.Button_TerrainBrush.Size = New System.Drawing.Size(150, 46)
        Me.Button_TerrainBrush.TabIndex = 8
        Me.Button_TerrainBrush.Text = "Brush"
        Me.ToolTip1.SetToolTip(Me.Button_TerrainBrush, "将选择的所有区域都涂为指定地形")
        Me.Button_TerrainBrush.UseVisualStyleBackColor = True
        '
        'Label_District_n_Change
        '
        Me.Label_District_n_Change.Location = New System.Drawing.Point(30, 198)
        Me.Label_District_n_Change.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_District_n_Change.Name = "Label_District_n_Change"
        Me.Label_District_n_Change.Size = New System.Drawing.Size(222, 52)
        Me.Label_District_n_Change.TabIndex = 9
        Me.Label_District_n_Change.Text = "District.n"
        Me.Label_District_n_Change.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label_Point_Select
        '
        Me.Label_Point_Select.Location = New System.Drawing.Point(30, 24)
        Me.Label_Point_Select.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label_Point_Select.Name = "Label_Point_Select"
        Me.Label_Point_Select.Size = New System.Drawing.Size(222, 58)
        Me.Label_Point_Select.TabIndex = 0
        Me.Label_Point_Select.Text = "Point"
        Me.Label_Point_Select.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Button_Compact
        '
        Me.Button_Compact.Location = New System.Drawing.Point(500, 436)
        Me.Button_Compact.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Compact.Name = "Button_Compact"
        Me.Button_Compact.Size = New System.Drawing.Size(150, 46)
        Me.Button_Compact.TabIndex = 21
        Me.Button_Compact.Text = "Compact"
        Me.ToolTip1.SetToolTip(Me.Button_Compact, "清理所有没有使用的点和区域，并重新编号")
        Me.Button_Compact.UseVisualStyleBackColor = True
        '
        'Button_Delete
        '
        Me.Button_Delete.Location = New System.Drawing.Point(264, 436)
        Me.Button_Delete.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Delete.Name = "Button_Delete"
        Me.Button_Delete.Size = New System.Drawing.Size(150, 46)
        Me.Button_Delete.TabIndex = 20
        Me.Button_Delete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.Button_Delete, "删除点和区域")
        Me.Button_Delete.UseVisualStyleBackColor = True
        '
        'Button_Split
        '
        Me.Button_Split.Location = New System.Drawing.Point(500, 378)
        Me.Button_Split.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Split.Name = "Button_Split"
        Me.Button_Split.Size = New System.Drawing.Size(150, 46)
        Me.Button_Split.TabIndex = 19
        Me.Button_Split.Text = "Split"
        Me.Button_Split.UseVisualStyleBackColor = True
        '
        'Button_Merge
        '
        Me.Button_Merge.Location = New System.Drawing.Point(264, 378)
        Me.Button_Merge.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button_Merge.Name = "Button_Merge"
        Me.Button_Merge.Size = New System.Drawing.Size(150, 46)
        Me.Button_Merge.TabIndex = 18
        Me.Button_Merge.Text = "Merge"
        Me.Button_Merge.UseVisualStyleBackColor = True
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.ListBox_Selection)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.PropertyGrid)
        Me.SplitContainer3.Size = New System.Drawing.Size(686, 558)
        Me.SplitContainer3.SplitterDistance = 71
        Me.SplitContainer3.SplitterWidth = 8
        Me.SplitContainer3.TabIndex = 3
        '
        'ListBox_Selection
        '
        Me.ListBox_Selection.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox_Selection.FormattingEnabled = True
        Me.ListBox_Selection.ItemHeight = 24
        Me.ListBox_Selection.Location = New System.Drawing.Point(0, 0)
        Me.ListBox_Selection.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.ListBox_Selection.Name = "ListBox_Selection"
        Me.ListBox_Selection.Size = New System.Drawing.Size(71, 558)
        Me.ListBox_Selection.TabIndex = 0
        '
        'PropertyGrid
        '
        Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid.HelpVisible = False
        Me.PropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.PropertyGrid.Name = "PropertyGrid"
        Me.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGrid.Size = New System.Drawing.Size(607, 558)
        Me.PropertyGrid.TabIndex = 0
        Me.PropertyGrid.ToolbarVisible = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel3, Me.ToolStripStatusLabel4})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 1035)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(2, 0, 28, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(1856, 45)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(237, 35)
        Me.ToolStripStatusLabel1.Text = "ControlMode XXXX"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(405, 35)
        Me.ToolStripStatusLabel2.Text = "NumPoint = XX, NumDistrict = XX"
        '
        'ToolStripStatusLabel3
        '
        Me.ToolStripStatusLabel3.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        Me.ToolStripStatusLabel3.Size = New System.Drawing.Size(82, 35)
        Me.ToolStripStatusLabel3.Text = "None"
        '
        'ToolStripStatusLabel4
        '
        Me.ToolStripStatusLabel4.Name = "ToolStripStatusLabel4"
        Me.ToolStripStatusLabel4.Size = New System.Drawing.Size(78, 35)
        Me.ToolStripStatusLabel4.Text = "None"
        '
        'Surface
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1856, 1080)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Menu = Me.MainMenu1
        Me.Name = "Surface"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SECEditor"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.NumericUpDown_Point_Select, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown_District_Select, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown_District_n_Change, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Button_Merge As System.Windows.Forms.Button
    Friend WithEvents Button_Split As System.Windows.Forms.Button
    Friend WithEvents ToolStripStatusLabel4 As System.Windows.Forms.ToolStripStatusLabel
End Class
