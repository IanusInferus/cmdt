<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DistrictDataEditor
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DistrictDataEditor))
        Me.DistrictDataView = New System.Windows.Forms.DataGridView
        Me.Index = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.n = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.kx = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ky = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.bz = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.TerrainInfo = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.MajorType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.MinorType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Unknown2 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Unknown3 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.IsEnterable = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Darkness = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.IsBound = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Unknown7 = New System.Windows.Forms.DataGridViewTextBoxColumn
        CType(Me.DistrictDataView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DistrictDataView
        '
        Me.DistrictDataView.AllowUserToAddRows = False
        Me.DistrictDataView.AllowUserToDeleteRows = False
        Me.DistrictDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DistrictDataView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Index, Me.n, Me.kx, Me.ky, Me.bz, Me.TerrainInfo, Me.MajorType, Me.MinorType, Me.Unknown2, Me.Unknown3, Me.IsEnterable, Me.Darkness, Me.IsBound, Me.Unknown7})
        Me.DistrictDataView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DistrictDataView.Location = New System.Drawing.Point(0, 0)
        Me.DistrictDataView.Name = "DistrictDataView"
        Me.DistrictDataView.RowHeadersWidth = 20
        Me.DistrictDataView.RowTemplate.Height = 23
        Me.DistrictDataView.Size = New System.Drawing.Size(972, 599)
        Me.DistrictDataView.TabIndex = 1
        '
        'Index
        '
        Me.Index.HeaderText = "Index"
        Me.Index.Name = "Index"
        Me.Index.ReadOnly = True
        Me.Index.Width = 60
        '
        'n
        '
        Me.n.HeaderText = "n"
        Me.n.Name = "n"
        Me.n.ReadOnly = True
        Me.n.Width = 20
        '
        'kx
        '
        Me.kx.HeaderText = "kx"
        Me.kx.Name = "kx"
        Me.kx.ReadOnly = True
        Me.kx.Width = 80
        '
        'ky
        '
        Me.ky.HeaderText = "ky"
        Me.ky.Name = "ky"
        Me.ky.ReadOnly = True
        Me.ky.Width = 80
        '
        'bz
        '
        Me.bz.HeaderText = "bz"
        Me.bz.Name = "bz"
        Me.bz.ReadOnly = True
        Me.bz.Width = 80
        '
        'TerrainInfo
        '
        Me.TerrainInfo.HeaderText = "TerrainInfo"
        Me.TerrainInfo.Name = "TerrainInfo"
        Me.TerrainInfo.Width = 120
        '
        'MajorType
        '
        Me.MajorType.HeaderText = "MajorType"
        Me.MajorType.Name = "MajorType"
        '
        'MinorType
        '
        Me.MinorType.HeaderText = "MinorType"
        Me.MinorType.Name = "MinorType"
        '
        'Unknown2
        '
        Me.Unknown2.HeaderText = "U2"
        Me.Unknown2.Name = "Unknown2"
        Me.Unknown2.Width = 30
        '
        'Unknown3
        '
        Me.Unknown3.HeaderText = "U3"
        Me.Unknown3.Name = "Unknown3"
        Me.Unknown3.Width = 30
        '
        'IsEnterable
        '
        Me.IsEnterable.HeaderText = "IsEnterable"
        Me.IsEnterable.Name = "IsEnterable"
        Me.IsEnterable.Width = 80
        '
        'Darkness
        '
        Me.Darkness.HeaderText = "Darkness"
        Me.Darkness.Name = "Darkness"
        Me.Darkness.Width = 60
        '
        'IsBound
        '
        Me.IsBound.HeaderText = "IsBound"
        Me.IsBound.Name = "IsBound"
        Me.IsBound.Width = 60
        '
        'Unknown7
        '
        Me.Unknown7.HeaderText = "U7"
        Me.Unknown7.Name = "Unknown7"
        Me.Unknown7.Width = 30
        '
        'DistrictDataEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(972, 599)
        Me.Controls.Add(Me.DistrictDataView)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DistrictDataEditor"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DistrictDataEditor"
        CType(Me.DistrictDataView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DistrictDataView As System.Windows.Forms.DataGridView
    Friend WithEvents Index As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents n As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents kx As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ky As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents bz As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TerrainInfo As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MajorType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MinorType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Unknown2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Unknown3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IsEnterable As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Darkness As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IsBound As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Unknown7 As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
