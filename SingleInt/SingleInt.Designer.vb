<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SingleInt
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SingleInt))
        Me.Button_Single = New System.Windows.Forms.Button
        Me.TextBox_LittleEndian = New System.Windows.Forms.TextBox
        Me.Button_LittleEndian = New System.Windows.Forms.Button
        Me.Label_Single = New System.Windows.Forms.Label
        Me.Label_LittleEndian = New System.Windows.Forms.Label
        Me.TextBox_Single = New System.Windows.Forms.TextBox
        Me.TextBox_BigEndian = New System.Windows.Forms.TextBox
        Me.Button_BigEndian = New System.Windows.Forms.Button
        Me.Label_BigEndian = New System.Windows.Forms.Label
        Me.Label_LittleEndian2 = New System.Windows.Forms.Label
        Me.ErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.ErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button_Single
        '
        Me.Button_Single.Location = New System.Drawing.Point(196, 42)
        Me.Button_Single.Name = "Button_Single"
        Me.Button_Single.Size = New System.Drawing.Size(75, 23)
        Me.Button_Single.TabIndex = 0
        Me.Button_Single.Text = "ToInt"
        Me.Button_Single.UseVisualStyleBackColor = True
        '
        'TextBox_LittleEndian
        '
        Me.TextBox_LittleEndian.Location = New System.Drawing.Point(70, 148)
        Me.TextBox_LittleEndian.Name = "TextBox_LittleEndian"
        Me.TextBox_LittleEndian.Size = New System.Drawing.Size(100, 21)
        Me.TextBox_LittleEndian.TabIndex = 1
        Me.TextBox_LittleEndian.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Button_LittleEndian
        '
        Me.Button_LittleEndian.Location = New System.Drawing.Point(196, 148)
        Me.Button_LittleEndian.Name = "Button_LittleEndian"
        Me.Button_LittleEndian.Size = New System.Drawing.Size(75, 23)
        Me.Button_LittleEndian.TabIndex = 3
        Me.Button_LittleEndian.Text = "ToSingle"
        Me.Button_LittleEndian.UseVisualStyleBackColor = True
        '
        'Label_Single
        '
        Me.Label_Single.AutoSize = True
        Me.Label_Single.Location = New System.Drawing.Point(12, 29)
        Me.Label_Single.Name = "Label_Single"
        Me.Label_Single.Size = New System.Drawing.Size(89, 12)
        Me.Label_Single.TabIndex = 4
        Me.Label_Single.Text = "Single IEEE754"
        '
        'Label_LittleEndian
        '
        Me.Label_LittleEndian.AutoSize = True
        Me.Label_LittleEndian.Location = New System.Drawing.Point(12, 133)
        Me.Label_LittleEndian.Name = "Label_LittleEndian"
        Me.Label_LittleEndian.Size = New System.Drawing.Size(281, 12)
        Me.Label_LittleEndian.TabIndex = 4
        Me.Label_LittleEndian.Text = "Int LittleEndian  Least Significant Byte First"
        '
        'TextBox_Single
        '
        Me.TextBox_Single.Location = New System.Drawing.Point(70, 44)
        Me.TextBox_Single.Name = "TextBox_Single"
        Me.TextBox_Single.Size = New System.Drawing.Size(100, 21)
        Me.TextBox_Single.TabIndex = 5
        Me.TextBox_Single.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox_BigEndian
        '
        Me.TextBox_BigEndian.Location = New System.Drawing.Point(70, 96)
        Me.TextBox_BigEndian.Name = "TextBox_BigEndian"
        Me.TextBox_BigEndian.Size = New System.Drawing.Size(100, 21)
        Me.TextBox_BigEndian.TabIndex = 1
        Me.TextBox_BigEndian.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Button_BigEndian
        '
        Me.Button_BigEndian.Location = New System.Drawing.Point(194, 96)
        Me.Button_BigEndian.Name = "Button_BigEndian"
        Me.Button_BigEndian.Size = New System.Drawing.Size(75, 23)
        Me.Button_BigEndian.TabIndex = 3
        Me.Button_BigEndian.Text = "ToSingle"
        Me.Button_BigEndian.UseVisualStyleBackColor = True
        '
        'Label_BigEndian
        '
        Me.Label_BigEndian.AutoSize = True
        Me.Label_BigEndian.Location = New System.Drawing.Point(12, 81)
        Me.Label_BigEndian.Name = "Label_BigEndian"
        Me.Label_BigEndian.Size = New System.Drawing.Size(257, 12)
        Me.Label_BigEndian.TabIndex = 4
        Me.Label_BigEndian.Text = "Int BigEndian  Most Significant Byte First"
        '
        'Label_LittleEndian2
        '
        Me.Label_LittleEndian2.AutoSize = True
        Me.Label_LittleEndian2.Location = New System.Drawing.Point(12, 153)
        Me.Label_LittleEndian2.Name = "Label_LittleEndian2"
        Me.Label_LittleEndian2.Size = New System.Drawing.Size(53, 12)
        Me.Label_LittleEndian2.TabIndex = 6
        Me.Label_LittleEndian2.Text = "盟军格式"
        '
        'ErrorProvider
        '
        Me.ErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.ErrorProvider.ContainerControl = Me
        '
        'SingleInt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(300, 195)
        Me.Controls.Add(Me.Label_LittleEndian2)
        Me.Controls.Add(Me.TextBox_Single)
        Me.Controls.Add(Me.Label_BigEndian)
        Me.Controls.Add(Me.Label_LittleEndian)
        Me.Controls.Add(Me.Button_BigEndian)
        Me.Controls.Add(Me.Label_Single)
        Me.Controls.Add(Me.TextBox_BigEndian)
        Me.Controls.Add(Me.Button_LittleEndian)
        Me.Controls.Add(Me.TextBox_LittleEndian)
        Me.Controls.Add(Me.Button_Single)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SingleInt"
        Me.Text = "SingleInt"
        CType(Me.ErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button_Single As System.Windows.Forms.Button
    Friend WithEvents TextBox_LittleEndian As System.Windows.Forms.TextBox
    Friend WithEvents Button_LittleEndian As System.Windows.Forms.Button
    Friend WithEvents Label_Single As System.Windows.Forms.Label
    Friend WithEvents Label_LittleEndian As System.Windows.Forms.Label
    Friend WithEvents TextBox_Single As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_BigEndian As System.Windows.Forms.TextBox
    Friend WithEvents Button_BigEndian As System.Windows.Forms.Button
    Friend WithEvents Label_BigEndian As System.Windows.Forms.Label
    Friend WithEvents Label_LittleEndian2 As System.Windows.Forms.Label
    Friend WithEvents ErrorProvider As System.Windows.Forms.ErrorProvider

End Class
