<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MultiConverter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MultiConverter))
        Me.TextBox_Comm2Directory = New System.Windows.Forms.TextBox
        Me.Button_Comm2Directory = New System.Windows.Forms.Button
        Me.TextBox_Comm3Directory = New System.Windows.Forms.TextBox
        Me.Button_Comm3Directory = New System.Windows.Forms.Button
        Me.Button_OK = New System.Windows.Forms.Button
        Me.Label_Comm2Directory = New System.Windows.Forms.Label
        Me.Label_Comm3Directory = New System.Windows.Forms.Label
        Me.Label_Description = New System.Windows.Forms.Label
        Me.Label_SetSetupDirectories = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'TextBox_Comm2Directory
        '
        Me.TextBox_Comm2Directory.Location = New System.Drawing.Point(14, 216)
        Me.TextBox_Comm2Directory.Name = "TextBox_Comm2Directory"
        Me.TextBox_Comm2Directory.Size = New System.Drawing.Size(323, 21)
        Me.TextBox_Comm2Directory.TabIndex = 3
        '
        'Button_Comm2Directory
        '
        Me.Button_Comm2Directory.Location = New System.Drawing.Point(343, 214)
        Me.Button_Comm2Directory.Name = "Button_Comm2Directory"
        Me.Button_Comm2Directory.Size = New System.Drawing.Size(36, 23)
        Me.Button_Comm2Directory.TabIndex = 4
        Me.Button_Comm2Directory.Text = "..."
        Me.Button_Comm2Directory.UseVisualStyleBackColor = True
        '
        'TextBox_Comm3Directory
        '
        Me.TextBox_Comm3Directory.Location = New System.Drawing.Point(14, 262)
        Me.TextBox_Comm3Directory.Name = "TextBox_Comm3Directory"
        Me.TextBox_Comm3Directory.Size = New System.Drawing.Size(323, 21)
        Me.TextBox_Comm3Directory.TabIndex = 6
        '
        'Button_Comm3Directory
        '
        Me.Button_Comm3Directory.Location = New System.Drawing.Point(343, 260)
        Me.Button_Comm3Directory.Name = "Button_Comm3Directory"
        Me.Button_Comm3Directory.Size = New System.Drawing.Size(36, 23)
        Me.Button_Comm3Directory.TabIndex = 7
        Me.Button_Comm3Directory.Text = "..."
        Me.Button_Comm3Directory.UseVisualStyleBackColor = True
        '
        'Button_OK
        '
        Me.Button_OK.Location = New System.Drawing.Point(304, 304)
        Me.Button_OK.Name = "Button_OK"
        Me.Button_OK.Size = New System.Drawing.Size(75, 23)
        Me.Button_OK.TabIndex = 8
        Me.Button_OK.Text = "确定"
        Me.Button_OK.UseVisualStyleBackColor = True
        '
        'Label_Comm2Directory
        '
        Me.Label_Comm2Directory.AutoSize = True
        Me.Label_Comm2Directory.Location = New System.Drawing.Point(12, 201)
        Me.Label_Comm2Directory.Name = "Label_Comm2Directory"
        Me.Label_Comm2Directory.Size = New System.Drawing.Size(71, 12)
        Me.Label_Comm2Directory.TabIndex = 2
        Me.Label_Comm2Directory.Text = "盟军2文件夹"
        '
        'Label_Comm3Directory
        '
        Me.Label_Comm3Directory.AutoSize = True
        Me.Label_Comm3Directory.Location = New System.Drawing.Point(12, 247)
        Me.Label_Comm3Directory.Name = "Label_Comm3Directory"
        Me.Label_Comm3Directory.Size = New System.Drawing.Size(71, 12)
        Me.Label_Comm3Directory.TabIndex = 5
        Me.Label_Comm3Directory.Text = "盟军3文件夹"
        '
        'Label_Description
        '
        Me.Label_Description.Location = New System.Drawing.Point(12, 9)
        Me.Label_Description.Name = "Label_Description"
        Me.Label_Description.Size = New System.Drawing.Size(367, 158)
        Me.Label_Description.TabIndex = 0
        Me.Label_Description.Text = resources.GetString("Label_Description.Text")
        '
        'Label_SetSetupDirectories
        '
        Me.Label_SetSetupDirectories.AutoSize = True
        Me.Label_SetSetupDirectories.Location = New System.Drawing.Point(12, 178)
        Me.Label_SetSetupDirectories.Name = "Label_SetSetupDirectories"
        Me.Label_SetSetupDirectories.Size = New System.Drawing.Size(89, 12)
        Me.Label_SetSetupDirectories.TabIndex = 1
        Me.Label_SetSetupDirectories.Text = "设定安装文件夹"
        '
        'MultiConverter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(391, 355)
        Me.Controls.Add(Me.Label_SetSetupDirectories)
        Me.Controls.Add(Me.Label_Description)
        Me.Controls.Add(Me.Button_OK)
        Me.Controls.Add(Me.Label_Comm3Directory)
        Me.Controls.Add(Me.Label_Comm2Directory)
        Me.Controls.Add(Me.Button_Comm3Directory)
        Me.Controls.Add(Me.Button_Comm2Directory)
        Me.Controls.Add(Me.TextBox_Comm3Directory)
        Me.Controls.Add(Me.TextBox_Comm2Directory)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MultiConverter"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "万用文件转换器"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox_Comm2Directory As System.Windows.Forms.TextBox
    Friend WithEvents Button_Comm2Directory As System.Windows.Forms.Button
    Friend WithEvents TextBox_Comm3Directory As System.Windows.Forms.TextBox
    Friend WithEvents Button_Comm3Directory As System.Windows.Forms.Button
    Friend WithEvents Button_OK As System.Windows.Forms.Button
    Friend WithEvents Label_Comm2Directory As System.Windows.Forms.Label
    Friend WithEvents Label_Comm3Directory As System.Windows.Forms.Label
    Friend WithEvents Label_Description As System.Windows.Forms.Label
    Friend WithEvents Label_SetSetupDirectories As System.Windows.Forms.Label

End Class
