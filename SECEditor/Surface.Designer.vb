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
        Me.Menu_File_Open = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Save = New System.Windows.Forms.MenuItem()
        Me.Menu_File_SaveAndClean = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Close = New System.Windows.Forms.MenuItem()
        Me.MenuItem3 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Exit = New System.Windows.Forms.MenuItem()
        Me.Menu_View = New System.Windows.Forms.MenuItem()
        Me.Menu_View_InitView = New System.Windows.Forms.MenuItem()
        Me.Menu_View_DistrictDataEditor = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface_GDIP = New System.Windows.Forms.MenuItem()
        Me.Menu_GraphicsInterface_SlimDX = New System.Windows.Forms.MenuItem()
        Me.PicBox = New SECEditor.Picker()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.PropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
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
        Me.Menu_File.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_File_Open, Me.Menu_File_Save, Me.Menu_File_SaveAndClean, Me.Menu_File_Close, Me.MenuItem3, Me.Menu_File_Exit})
        Me.Menu_File.Text = "文件(&F)"
        '
        'Menu_File_Open
        '
        Me.Menu_File_Open.Index = 0
        Me.Menu_File_Open.Text = "打开(&O)..." & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+O"
        '
        'Menu_File_Save
        '
        Me.Menu_File_Save.Enabled = False
        Me.Menu_File_Save.Index = 1
        Me.Menu_File_Save.Text = "保存(&S)" & Global.Microsoft.VisualBasic.ChrW(9) & "Ctrl+S"
        '
        'Menu_File_SaveAndClean
        '
        Me.Menu_File_SaveAndClean.Enabled = False
        Me.Menu_File_SaveAndClean.Index = 2
        Me.Menu_File_SaveAndClean.Text = "保存并清理(&C)"
        '
        'Menu_File_Close
        '
        Me.Menu_File_Close.Enabled = False
        Me.Menu_File_Close.Index = 3
        Me.Menu_File_Close.Text = "关闭(&C)"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 4
        Me.MenuItem3.Text = "-"
        '
        'Menu_File_Exit
        '
        Me.Menu_File_Exit.Index = 5
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
        Me.Menu_GraphicsInterface_GDIP.Text = "GDI+"
        '
        'Menu_GraphicsInterface_SlimDX
        '
        Me.Menu_GraphicsInterface_SlimDX.Index = 1
        Me.Menu_GraphicsInterface_SlimDX.RadioCheck = True
        Me.Menu_GraphicsInterface_SlimDX.Text = "SlimDX"
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PicBox.KeyPressTimeLimit = 0.2R
        Me.PicBox.Location = New System.Drawing.Point(0, 0)
        Me.PicBox.MinFlamePeriod = 0.02R
        Me.PicBox.MultiClickTimeLimit = 0.4R
        Me.PicBox.Name = "PicBox"
        Me.PicBox.PropertyGrid = Nothing
        Me.PicBox.Size = New System.Drawing.Size(650, 600)
        Me.PicBox.TabIndex = 0
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
        Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid)
        Me.SplitContainer1.Size = New System.Drawing.Size(928, 600)
        Me.SplitContainer1.SplitterDistance = 650
        Me.SplitContainer1.TabIndex = 1
        '
        'PropertyGrid
        '
        Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid.HelpVisible = False
        Me.PropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid.Name = "PropertyGrid"
        Me.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGrid.Size = New System.Drawing.Size(274, 600)
        Me.PropertyGrid.TabIndex = 1
        Me.PropertyGrid.ToolbarVisible = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 596)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(928, 26)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(88, 21)
        Me.ToolStripStatusLabel2.Text = "District XXXX"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(126, 21)
        Me.ToolStripStatusLabel1.Text = "ControlMode XXXX"
        '
        'Surface
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(928, 622)
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
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Menu_View_DistrictDataEditor As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_SaveAndClean As System.Windows.Forms.MenuItem

End Class
