<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Y64Manager
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Y64Manager))
        Me.ToolTab = New System.Windows.Forms.TabControl
        Me.Exporter = New System.Windows.Forms.TabPage
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Exporter_Button_YCbCr2RGB = New System.Windows.Forms.Button
        Me.Exporter_Button_RGB2YCbCr = New System.Windows.Forms.Button
        Me.Exporter_NumericUpDown_B = New System.Windows.Forms.NumericUpDown
        Me.Exporter_NumericUpDown_G = New System.Windows.Forms.NumericUpDown
        Me.Exporter_NumericUpDown_Cr = New System.Windows.Forms.NumericUpDown
        Me.Exporter_NumericUpDown_Cb = New System.Windows.Forms.NumericUpDown
        Me.Exporter_NumericUpDown_Y = New System.Windows.Forms.NumericUpDown
        Me.Exporter_NumericUpDown_R = New System.Windows.Forms.NumericUpDown
        Me.Importer = New System.Windows.Forms.TabPage
        Me.Importer_FileButton_PicFolder = New System.Windows.Forms.Button
        Me.Importer_FileButton_ExtraPic = New System.Windows.Forms.Button
        Me.Importer_FileButton_MainPic = New System.Windows.Forms.Button
        Me.Importer_FileButton_Y64File = New System.Windows.Forms.Button
        Me.Importer_Label_PictureIndex = New System.Windows.Forms.Label
        Me.Importer_Label_Y64File = New System.Windows.Forms.Label
        Me.Importer_Label_ExtraPic = New System.Windows.Forms.Label
        Me.Importer_Label_PicFolder = New System.Windows.Forms.Label
        Me.Importer_Label_MainPic = New System.Windows.Forms.Label
        Me.Importer_Button_FullImport = New System.Windows.Forms.Button
        Me.Importer_Button_Import = New System.Windows.Forms.Button
        Me.Importer_NumericUpDown_Pic = New System.Windows.Forms.NumericUpDown
        Me.Importer_NumericUpDown_View = New System.Windows.Forms.NumericUpDown
        Me.Importer_TextBox_Y64File = New System.Windows.Forms.TextBox
        Me.Importer_TextBox_PicFolder = New System.Windows.Forms.TextBox
        Me.Importer_TextBox_ExtraPic = New System.Windows.Forms.TextBox
        Me.Importer_TextBox_MainPic = New System.Windows.Forms.TextBox
        Me.Importer_Label_Notice = New System.Windows.Forms.Label
        Me.Creator = New System.Windows.Forms.TabPage
        Me.Creator_FileButton_Y64File = New System.Windows.Forms.Button
        Me.Creator_FileButton_DescriptionFile = New System.Windows.Forms.Button
        Me.Creator_Button_Create = New System.Windows.Forms.Button
        Me.Creator_Label_Y64File = New System.Windows.Forms.Label
        Me.Creator_TextBox_Y64File = New System.Windows.Forms.TextBox
        Me.Creator_Label_DescriptionFile = New System.Windows.Forms.Label
        Me.Creator_TextBox_DescriptionFile = New System.Windows.Forms.TextBox
        Me.Converter = New System.Windows.Forms.TabPage
        Me.Converter_CheckBox_ReplacePalette = New System.Windows.Forms.CheckBox
        Me.Converter_RadioButton_Comm3Version4 = New System.Windows.Forms.RadioButton
        Me.Converter_RadioButton_Comm3Version3 = New System.Windows.Forms.RadioButton
        Me.Converter_RadioButton_Comm2Version2 = New System.Windows.Forms.RadioButton
        Me.Converter_RadioButton_Comm2Version1 = New System.Windows.Forms.RadioButton
        Me.Converter_FileButton_TargetFile = New System.Windows.Forms.Button
        Me.Converter_FileButton_SourceFile = New System.Windows.Forms.Button
        Me.Converter_NumericUpDown_SaturationFactor = New System.Windows.Forms.NumericUpDown
        Me.Converter_NumericUpDown_LightnessFactor = New System.Windows.Forms.NumericUpDown
        Me.Converter_Label_Notice2 = New System.Windows.Forms.Label
        Me.Converter_Button_Convert = New System.Windows.Forms.Button
        Me.Converter_Label_SaturationFactor = New System.Windows.Forms.Label
        Me.Converter_Label_LightnessFactor = New System.Windows.Forms.Label
        Me.Converter_Label_TargetFile = New System.Windows.Forms.Label
        Me.Converter_TextBox_TargetFile = New System.Windows.Forms.TextBox
        Me.Converter_Label_SourceFile = New System.Windows.Forms.Label
        Me.Converter_TextBox_SourceFile = New System.Windows.Forms.TextBox
        Me.Readme = New System.Windows.Forms.TabPage
        Me.Readme_TextBox_Readme = New System.Windows.Forms.TextBox
        Me.ToolTab.SuspendLayout()
        Me.Exporter.SuspendLayout()
        CType(Me.Exporter_NumericUpDown_B, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Exporter_NumericUpDown_G, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Exporter_NumericUpDown_Cr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Exporter_NumericUpDown_Cb, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Exporter_NumericUpDown_Y, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Exporter_NumericUpDown_R, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Importer.SuspendLayout()
        CType(Me.Importer_NumericUpDown_Pic, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Importer_NumericUpDown_View, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Creator.SuspendLayout()
        Me.Converter.SuspendLayout()
        CType(Me.Converter_NumericUpDown_SaturationFactor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Converter_NumericUpDown_LightnessFactor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Readme.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolTab
        '
        Me.ToolTab.Controls.Add(Me.Exporter)
        Me.ToolTab.Controls.Add(Me.Importer)
        Me.ToolTab.Controls.Add(Me.Creator)
        Me.ToolTab.Controls.Add(Me.Converter)
        Me.ToolTab.Controls.Add(Me.Readme)
        Me.ToolTab.Location = New System.Drawing.Point(13, 10)
        Me.ToolTab.Name = "ToolTab"
        Me.ToolTab.SelectedIndex = 0
        Me.ToolTab.Size = New System.Drawing.Size(429, 445)
        Me.ToolTab.TabIndex = 0
        '
        'Exporter
        '
        Me.Exporter.Controls.Add(Me.Label1)
        Me.Exporter.Controls.Add(Me.Label6)
        Me.Exporter.Controls.Add(Me.Label5)
        Me.Exporter.Controls.Add(Me.Exporter_Button_YCbCr2RGB)
        Me.Exporter.Controls.Add(Me.Exporter_Button_RGB2YCbCr)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_B)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_G)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_Cr)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_Cb)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_Y)
        Me.Exporter.Controls.Add(Me.Exporter_NumericUpDown_R)
        Me.Exporter.Location = New System.Drawing.Point(4, 21)
        Me.Exporter.Name = "Exporter"
        Me.Exporter.Padding = New System.Windows.Forms.Padding(3)
        Me.Exporter.Size = New System.Drawing.Size(421, 420)
        Me.Exporter.TabIndex = 0
        Me.Exporter.Text = "导出器"
        Me.Exporter.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(93, 128)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(167, 36)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "导出请使用ImageConvert" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Use ImageConvert for Export"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(9, 388)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(35, 12)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "YCbCr"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 356)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(23, 12)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "RGB"
        '
        'Exporter_Button_YCbCr2RGB
        '
        Me.Exporter_Button_YCbCr2RGB.Location = New System.Drawing.Point(303, 383)
        Me.Exporter_Button_YCbCr2RGB.Name = "Exporter_Button_YCbCr2RGB"
        Me.Exporter_Button_YCbCr2RGB.Size = New System.Drawing.Size(109, 23)
        Me.Exporter_Button_YCbCr2RGB.TabIndex = 23
        Me.Exporter_Button_YCbCr2RGB.Text = "YCbCr到RGB(&R)"
        Me.Exporter_Button_YCbCr2RGB.UseVisualStyleBackColor = True
        '
        'Exporter_Button_RGB2YCbCr
        '
        Me.Exporter_Button_RGB2YCbCr.Location = New System.Drawing.Point(303, 351)
        Me.Exporter_Button_RGB2YCbCr.Name = "Exporter_Button_RGB2YCbCr"
        Me.Exporter_Button_RGB2YCbCr.Size = New System.Drawing.Size(108, 23)
        Me.Exporter_Button_RGB2YCbCr.TabIndex = 19
        Me.Exporter_Button_RGB2YCbCr.Text = "RGB到YCbCr(&Y)"
        Me.Exporter_Button_RGB2YCbCr.UseVisualStyleBackColor = True
        '
        'Exporter_NumericUpDown_B
        '
        Me.Exporter_NumericUpDown_B.Location = New System.Drawing.Point(233, 354)
        Me.Exporter_NumericUpDown_B.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_B.Name = "Exporter_NumericUpDown_B"
        Me.Exporter_NumericUpDown_B.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_B.TabIndex = 18
        Me.Exporter_NumericUpDown_B.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Exporter_NumericUpDown_G
        '
        Me.Exporter_NumericUpDown_G.Location = New System.Drawing.Point(163, 354)
        Me.Exporter_NumericUpDown_G.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_G.Name = "Exporter_NumericUpDown_G"
        Me.Exporter_NumericUpDown_G.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_G.TabIndex = 17
        Me.Exporter_NumericUpDown_G.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Exporter_NumericUpDown_Cr
        '
        Me.Exporter_NumericUpDown_Cr.InterceptArrowKeys = False
        Me.Exporter_NumericUpDown_Cr.Location = New System.Drawing.Point(233, 386)
        Me.Exporter_NumericUpDown_Cr.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_Cr.Name = "Exporter_NumericUpDown_Cr"
        Me.Exporter_NumericUpDown_Cr.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_Cr.TabIndex = 22
        Me.Exporter_NumericUpDown_Cr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Exporter_NumericUpDown_Cb
        '
        Me.Exporter_NumericUpDown_Cb.InterceptArrowKeys = False
        Me.Exporter_NumericUpDown_Cb.Location = New System.Drawing.Point(163, 386)
        Me.Exporter_NumericUpDown_Cb.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_Cb.Name = "Exporter_NumericUpDown_Cb"
        Me.Exporter_NumericUpDown_Cb.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_Cb.TabIndex = 21
        Me.Exporter_NumericUpDown_Cb.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Exporter_NumericUpDown_Y
        '
        Me.Exporter_NumericUpDown_Y.InterceptArrowKeys = False
        Me.Exporter_NumericUpDown_Y.Location = New System.Drawing.Point(93, 386)
        Me.Exporter_NumericUpDown_Y.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_Y.Name = "Exporter_NumericUpDown_Y"
        Me.Exporter_NumericUpDown_Y.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_Y.TabIndex = 20
        Me.Exporter_NumericUpDown_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Exporter_NumericUpDown_R
        '
        Me.Exporter_NumericUpDown_R.Location = New System.Drawing.Point(93, 354)
        Me.Exporter_NumericUpDown_R.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Exporter_NumericUpDown_R.Name = "Exporter_NumericUpDown_R"
        Me.Exporter_NumericUpDown_R.Size = New System.Drawing.Size(64, 21)
        Me.Exporter_NumericUpDown_R.TabIndex = 16
        Me.Exporter_NumericUpDown_R.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Importer
        '
        Me.Importer.Controls.Add(Me.Importer_FileButton_PicFolder)
        Me.Importer.Controls.Add(Me.Importer_FileButton_ExtraPic)
        Me.Importer.Controls.Add(Me.Importer_FileButton_MainPic)
        Me.Importer.Controls.Add(Me.Importer_FileButton_Y64File)
        Me.Importer.Controls.Add(Me.Importer_Label_PictureIndex)
        Me.Importer.Controls.Add(Me.Importer_Label_Y64File)
        Me.Importer.Controls.Add(Me.Importer_Label_ExtraPic)
        Me.Importer.Controls.Add(Me.Importer_Label_PicFolder)
        Me.Importer.Controls.Add(Me.Importer_Label_MainPic)
        Me.Importer.Controls.Add(Me.Importer_Button_FullImport)
        Me.Importer.Controls.Add(Me.Importer_Button_Import)
        Me.Importer.Controls.Add(Me.Importer_NumericUpDown_Pic)
        Me.Importer.Controls.Add(Me.Importer_NumericUpDown_View)
        Me.Importer.Controls.Add(Me.Importer_TextBox_Y64File)
        Me.Importer.Controls.Add(Me.Importer_TextBox_PicFolder)
        Me.Importer.Controls.Add(Me.Importer_TextBox_ExtraPic)
        Me.Importer.Controls.Add(Me.Importer_TextBox_MainPic)
        Me.Importer.Controls.Add(Me.Importer_Label_Notice)
        Me.Importer.Location = New System.Drawing.Point(4, 21)
        Me.Importer.Name = "Importer"
        Me.Importer.Padding = New System.Windows.Forms.Padding(3)
        Me.Importer.Size = New System.Drawing.Size(421, 420)
        Me.Importer.TabIndex = 1
        Me.Importer.Text = "导入器"
        Me.Importer.UseVisualStyleBackColor = True
        '
        'Importer_FileButton_PicFolder
        '
        Me.Importer_FileButton_PicFolder.Location = New System.Drawing.Point(361, 319)
        Me.Importer_FileButton_PicFolder.Name = "Importer_FileButton_PicFolder"
        Me.Importer_FileButton_PicFolder.Size = New System.Drawing.Size(31, 23)
        Me.Importer_FileButton_PicFolder.TabIndex = 25
        Me.Importer_FileButton_PicFolder.Text = "..."
        Me.Importer_FileButton_PicFolder.UseVisualStyleBackColor = True
        '
        'Importer_FileButton_ExtraPic
        '
        Me.Importer_FileButton_ExtraPic.Location = New System.Drawing.Point(361, 228)
        Me.Importer_FileButton_ExtraPic.Name = "Importer_FileButton_ExtraPic"
        Me.Importer_FileButton_ExtraPic.Size = New System.Drawing.Size(31, 23)
        Me.Importer_FileButton_ExtraPic.TabIndex = 25
        Me.Importer_FileButton_ExtraPic.Text = "..."
        Me.Importer_FileButton_ExtraPic.UseVisualStyleBackColor = True
        '
        'Importer_FileButton_MainPic
        '
        Me.Importer_FileButton_MainPic.Location = New System.Drawing.Point(361, 197)
        Me.Importer_FileButton_MainPic.Name = "Importer_FileButton_MainPic"
        Me.Importer_FileButton_MainPic.Size = New System.Drawing.Size(31, 23)
        Me.Importer_FileButton_MainPic.TabIndex = 25
        Me.Importer_FileButton_MainPic.Text = "..."
        Me.Importer_FileButton_MainPic.UseVisualStyleBackColor = True
        '
        'Importer_FileButton_Y64File
        '
        Me.Importer_FileButton_Y64File.Location = New System.Drawing.Point(361, 137)
        Me.Importer_FileButton_Y64File.Name = "Importer_FileButton_Y64File"
        Me.Importer_FileButton_Y64File.Size = New System.Drawing.Size(31, 23)
        Me.Importer_FileButton_Y64File.TabIndex = 25
        Me.Importer_FileButton_Y64File.Text = "..."
        Me.Importer_FileButton_Y64File.UseVisualStyleBackColor = True
        '
        'Importer_Label_PictureIndex
        '
        Me.Importer_Label_PictureIndex.AutoSize = True
        Me.Importer_Label_PictureIndex.Location = New System.Drawing.Point(22, 271)
        Me.Importer_Label_PictureIndex.Name = "Importer_Label_PictureIndex"
        Me.Importer_Label_PictureIndex.Size = New System.Drawing.Size(41, 12)
        Me.Importer_Label_PictureIndex.TabIndex = 6
        Me.Importer_Label_PictureIndex.Text = "图片号"
        '
        'Importer_Label_Y64File
        '
        Me.Importer_Label_Y64File.AutoSize = True
        Me.Importer_Label_Y64File.Location = New System.Drawing.Point(22, 142)
        Me.Importer_Label_Y64File.Name = "Importer_Label_Y64File"
        Me.Importer_Label_Y64File.Size = New System.Drawing.Size(47, 12)
        Me.Importer_Label_Y64File.TabIndex = 5
        Me.Importer_Label_Y64File.Text = "Y64文件"
        '
        'Importer_Label_ExtraPic
        '
        Me.Importer_Label_ExtraPic.AutoSize = True
        Me.Importer_Label_ExtraPic.Location = New System.Drawing.Point(22, 233)
        Me.Importer_Label_ExtraPic.Name = "Importer_Label_ExtraPic"
        Me.Importer_Label_ExtraPic.Size = New System.Drawing.Size(65, 12)
        Me.Importer_Label_ExtraPic.TabIndex = 3
        Me.Importer_Label_ExtraPic.Text = "副位图文件"
        '
        'Importer_Label_PicFolder
        '
        Me.Importer_Label_PicFolder.AutoSize = True
        Me.Importer_Label_PicFolder.Location = New System.Drawing.Point(22, 324)
        Me.Importer_Label_PicFolder.Name = "Importer_Label_PicFolder"
        Me.Importer_Label_PicFolder.Size = New System.Drawing.Size(65, 12)
        Me.Importer_Label_PicFolder.TabIndex = 2
        Me.Importer_Label_PicFolder.Text = "位图文件夹"
        '
        'Importer_Label_MainPic
        '
        Me.Importer_Label_MainPic.AutoSize = True
        Me.Importer_Label_MainPic.Location = New System.Drawing.Point(22, 202)
        Me.Importer_Label_MainPic.Name = "Importer_Label_MainPic"
        Me.Importer_Label_MainPic.Size = New System.Drawing.Size(65, 12)
        Me.Importer_Label_MainPic.TabIndex = 2
        Me.Importer_Label_MainPic.Text = "主位图文件"
        '
        'Importer_Button_FullImport
        '
        Me.Importer_Button_FullImport.Location = New System.Drawing.Point(292, 364)
        Me.Importer_Button_FullImport.Name = "Importer_Button_FullImport"
        Me.Importer_Button_FullImport.Size = New System.Drawing.Size(100, 23)
        Me.Importer_Button_FullImport.TabIndex = 13
        Me.Importer_Button_FullImport.Text = "全导入(&F)"
        Me.Importer_Button_FullImport.UseVisualStyleBackColor = True
        '
        'Importer_Button_Import
        '
        Me.Importer_Button_Import.Location = New System.Drawing.Point(292, 266)
        Me.Importer_Button_Import.Name = "Importer_Button_Import"
        Me.Importer_Button_Import.Size = New System.Drawing.Size(100, 23)
        Me.Importer_Button_Import.TabIndex = 13
        Me.Importer_Button_Import.Text = "导入(&I)"
        Me.Importer_Button_Import.UseVisualStyleBackColor = True
        '
        'Importer_NumericUpDown_Pic
        '
        Me.Importer_NumericUpDown_Pic.Location = New System.Drawing.Point(209, 268)
        Me.Importer_NumericUpDown_Pic.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Importer_NumericUpDown_Pic.Name = "Importer_NumericUpDown_Pic"
        Me.Importer_NumericUpDown_Pic.Size = New System.Drawing.Size(60, 21)
        Me.Importer_NumericUpDown_Pic.TabIndex = 12
        Me.Importer_NumericUpDown_Pic.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Importer_NumericUpDown_View
        '
        Me.Importer_NumericUpDown_View.Location = New System.Drawing.Point(143, 268)
        Me.Importer_NumericUpDown_View.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.Importer_NumericUpDown_View.Name = "Importer_NumericUpDown_View"
        Me.Importer_NumericUpDown_View.Size = New System.Drawing.Size(60, 21)
        Me.Importer_NumericUpDown_View.TabIndex = 11
        Me.Importer_NumericUpDown_View.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Importer_TextBox_Y64File
        '
        Me.Importer_TextBox_Y64File.Location = New System.Drawing.Point(143, 139)
        Me.Importer_TextBox_Y64File.Name = "Importer_TextBox_Y64File"
        Me.Importer_TextBox_Y64File.Size = New System.Drawing.Size(212, 21)
        Me.Importer_TextBox_Y64File.TabIndex = 10
        '
        'Importer_TextBox_PicFolder
        '
        Me.Importer_TextBox_PicFolder.Location = New System.Drawing.Point(143, 321)
        Me.Importer_TextBox_PicFolder.Name = "Importer_TextBox_PicFolder"
        Me.Importer_TextBox_PicFolder.Size = New System.Drawing.Size(212, 21)
        Me.Importer_TextBox_PicFolder.TabIndex = 7
        '
        'Importer_TextBox_ExtraPic
        '
        Me.Importer_TextBox_ExtraPic.Location = New System.Drawing.Point(143, 230)
        Me.Importer_TextBox_ExtraPic.Name = "Importer_TextBox_ExtraPic"
        Me.Importer_TextBox_ExtraPic.Size = New System.Drawing.Size(212, 21)
        Me.Importer_TextBox_ExtraPic.TabIndex = 8
        '
        'Importer_TextBox_MainPic
        '
        Me.Importer_TextBox_MainPic.Location = New System.Drawing.Point(143, 199)
        Me.Importer_TextBox_MainPic.Name = "Importer_TextBox_MainPic"
        Me.Importer_TextBox_MainPic.Size = New System.Drawing.Size(212, 21)
        Me.Importer_TextBox_MainPic.TabIndex = 7
        '
        'Importer_Label_Notice
        '
        Me.Importer_Label_Notice.Location = New System.Drawing.Point(22, 42)
        Me.Importer_Label_Notice.Name = "Importer_Label_Notice"
        Me.Importer_Label_Notice.Size = New System.Drawing.Size(370, 40)
        Me.Importer_Label_Notice.TabIndex = 0
        Me.Importer_Label_Notice.Text = "请先备份你的Y64文件。副位图文件没有请留空。"
        '
        'Creator
        '
        Me.Creator.Controls.Add(Me.Creator_FileButton_Y64File)
        Me.Creator.Controls.Add(Me.Creator_FileButton_DescriptionFile)
        Me.Creator.Controls.Add(Me.Creator_Button_Create)
        Me.Creator.Controls.Add(Me.Creator_Label_Y64File)
        Me.Creator.Controls.Add(Me.Creator_TextBox_Y64File)
        Me.Creator.Controls.Add(Me.Creator_Label_DescriptionFile)
        Me.Creator.Controls.Add(Me.Creator_TextBox_DescriptionFile)
        Me.Creator.Location = New System.Drawing.Point(4, 21)
        Me.Creator.Name = "Creator"
        Me.Creator.Size = New System.Drawing.Size(421, 420)
        Me.Creator.TabIndex = 2
        Me.Creator.Text = "创建器"
        Me.Creator.UseVisualStyleBackColor = True
        '
        'Creator_FileButton_Y64File
        '
        Me.Creator_FileButton_Y64File.Location = New System.Drawing.Point(361, 143)
        Me.Creator_FileButton_Y64File.Name = "Creator_FileButton_Y64File"
        Me.Creator_FileButton_Y64File.Size = New System.Drawing.Size(31, 23)
        Me.Creator_FileButton_Y64File.TabIndex = 26
        Me.Creator_FileButton_Y64File.Text = "..."
        Me.Creator_FileButton_Y64File.UseVisualStyleBackColor = True
        '
        'Creator_FileButton_DescriptionFile
        '
        Me.Creator_FileButton_DescriptionFile.Location = New System.Drawing.Point(361, 96)
        Me.Creator_FileButton_DescriptionFile.Name = "Creator_FileButton_DescriptionFile"
        Me.Creator_FileButton_DescriptionFile.Size = New System.Drawing.Size(31, 23)
        Me.Creator_FileButton_DescriptionFile.TabIndex = 25
        Me.Creator_FileButton_DescriptionFile.Text = "..."
        Me.Creator_FileButton_DescriptionFile.UseVisualStyleBackColor = True
        '
        'Creator_Button_Create
        '
        Me.Creator_Button_Create.Location = New System.Drawing.Point(292, 375)
        Me.Creator_Button_Create.Name = "Creator_Button_Create"
        Me.Creator_Button_Create.Size = New System.Drawing.Size(100, 23)
        Me.Creator_Button_Create.TabIndex = 12
        Me.Creator_Button_Create.Text = "创建(&C)"
        Me.Creator_Button_Create.UseVisualStyleBackColor = True
        '
        'Creator_Label_Y64File
        '
        Me.Creator_Label_Y64File.AutoSize = True
        Me.Creator_Label_Y64File.Location = New System.Drawing.Point(22, 148)
        Me.Creator_Label_Y64File.Name = "Creator_Label_Y64File"
        Me.Creator_Label_Y64File.Size = New System.Drawing.Size(47, 12)
        Me.Creator_Label_Y64File.TabIndex = 10
        Me.Creator_Label_Y64File.Text = "Y64文件"
        '
        'Creator_TextBox_Y64File
        '
        Me.Creator_TextBox_Y64File.Location = New System.Drawing.Point(143, 145)
        Me.Creator_TextBox_Y64File.Name = "Creator_TextBox_Y64File"
        Me.Creator_TextBox_Y64File.Size = New System.Drawing.Size(212, 21)
        Me.Creator_TextBox_Y64File.TabIndex = 11
        '
        'Creator_Label_DescriptionFile
        '
        Me.Creator_Label_DescriptionFile.AutoSize = True
        Me.Creator_Label_DescriptionFile.Location = New System.Drawing.Point(22, 101)
        Me.Creator_Label_DescriptionFile.Name = "Creator_Label_DescriptionFile"
        Me.Creator_Label_DescriptionFile.Size = New System.Drawing.Size(53, 12)
        Me.Creator_Label_DescriptionFile.TabIndex = 8
        Me.Creator_Label_DescriptionFile.Text = "描述文件"
        '
        'Creator_TextBox_DescriptionFile
        '
        Me.Creator_TextBox_DescriptionFile.Location = New System.Drawing.Point(143, 98)
        Me.Creator_TextBox_DescriptionFile.Name = "Creator_TextBox_DescriptionFile"
        Me.Creator_TextBox_DescriptionFile.Size = New System.Drawing.Size(212, 21)
        Me.Creator_TextBox_DescriptionFile.TabIndex = 9
        '
        'Converter
        '
        Me.Converter.Controls.Add(Me.Converter_CheckBox_ReplacePalette)
        Me.Converter.Controls.Add(Me.Converter_RadioButton_Comm3Version4)
        Me.Converter.Controls.Add(Me.Converter_RadioButton_Comm3Version3)
        Me.Converter.Controls.Add(Me.Converter_RadioButton_Comm2Version2)
        Me.Converter.Controls.Add(Me.Converter_RadioButton_Comm2Version1)
        Me.Converter.Controls.Add(Me.Converter_FileButton_TargetFile)
        Me.Converter.Controls.Add(Me.Converter_FileButton_SourceFile)
        Me.Converter.Controls.Add(Me.Converter_NumericUpDown_SaturationFactor)
        Me.Converter.Controls.Add(Me.Converter_NumericUpDown_LightnessFactor)
        Me.Converter.Controls.Add(Me.Converter_Label_Notice2)
        Me.Converter.Controls.Add(Me.Converter_Button_Convert)
        Me.Converter.Controls.Add(Me.Converter_Label_SaturationFactor)
        Me.Converter.Controls.Add(Me.Converter_Label_LightnessFactor)
        Me.Converter.Controls.Add(Me.Converter_Label_TargetFile)
        Me.Converter.Controls.Add(Me.Converter_TextBox_TargetFile)
        Me.Converter.Controls.Add(Me.Converter_Label_SourceFile)
        Me.Converter.Controls.Add(Me.Converter_TextBox_SourceFile)
        Me.Converter.Location = New System.Drawing.Point(4, 21)
        Me.Converter.Name = "Converter"
        Me.Converter.Size = New System.Drawing.Size(421, 420)
        Me.Converter.TabIndex = 3
        Me.Converter.Text = "转换器"
        Me.Converter.UseVisualStyleBackColor = True
        '
        'Converter_CheckBox_ReplacePalette
        '
        Me.Converter_CheckBox_ReplacePalette.AutoSize = True
        Me.Converter_CheckBox_ReplacePalette.Location = New System.Drawing.Point(24, 275)
        Me.Converter_CheckBox_ReplacePalette.Name = "Converter_CheckBox_ReplacePalette"
        Me.Converter_CheckBox_ReplacePalette.Size = New System.Drawing.Size(138, 16)
        Me.Converter_CheckBox_ReplacePalette.TabIndex = 28
        Me.Converter_CheckBox_ReplacePalette.Text = "替换为盟军2的调色板"
        Me.Converter_CheckBox_ReplacePalette.UseVisualStyleBackColor = True
        '
        'Converter_RadioButton_Comm3Version4
        '
        Me.Converter_RadioButton_Comm3Version4.AutoSize = True
        Me.Converter_RadioButton_Comm3Version4.Location = New System.Drawing.Point(291, 240)
        Me.Converter_RadioButton_Comm3Version4.Name = "Converter_RadioButton_Comm3Version4"
        Me.Converter_RadioButton_Comm3Version4.Size = New System.Drawing.Size(83, 16)
        Me.Converter_RadioButton_Comm3Version4.TabIndex = 27
        Me.Converter_RadioButton_Comm3Version4.Text = "盟军3版本4"
        Me.Converter_RadioButton_Comm3Version4.UseVisualStyleBackColor = True
        '
        'Converter_RadioButton_Comm3Version3
        '
        Me.Converter_RadioButton_Comm3Version3.AutoSize = True
        Me.Converter_RadioButton_Comm3Version3.Location = New System.Drawing.Point(202, 240)
        Me.Converter_RadioButton_Comm3Version3.Name = "Converter_RadioButton_Comm3Version3"
        Me.Converter_RadioButton_Comm3Version3.Size = New System.Drawing.Size(83, 16)
        Me.Converter_RadioButton_Comm3Version3.TabIndex = 27
        Me.Converter_RadioButton_Comm3Version3.Text = "盟军3版本3"
        Me.Converter_RadioButton_Comm3Version3.UseVisualStyleBackColor = True
        '
        'Converter_RadioButton_Comm2Version2
        '
        Me.Converter_RadioButton_Comm2Version2.AutoSize = True
        Me.Converter_RadioButton_Comm2Version2.Checked = True
        Me.Converter_RadioButton_Comm2Version2.Location = New System.Drawing.Point(113, 240)
        Me.Converter_RadioButton_Comm2Version2.Name = "Converter_RadioButton_Comm2Version2"
        Me.Converter_RadioButton_Comm2Version2.Size = New System.Drawing.Size(83, 16)
        Me.Converter_RadioButton_Comm2Version2.TabIndex = 27
        Me.Converter_RadioButton_Comm2Version2.TabStop = True
        Me.Converter_RadioButton_Comm2Version2.Text = "盟军2版本2"
        Me.Converter_RadioButton_Comm2Version2.UseVisualStyleBackColor = True
        '
        'Converter_RadioButton_Comm2Version1
        '
        Me.Converter_RadioButton_Comm2Version1.AutoSize = True
        Me.Converter_RadioButton_Comm2Version1.Location = New System.Drawing.Point(24, 240)
        Me.Converter_RadioButton_Comm2Version1.Name = "Converter_RadioButton_Comm2Version1"
        Me.Converter_RadioButton_Comm2Version1.Size = New System.Drawing.Size(83, 16)
        Me.Converter_RadioButton_Comm2Version1.TabIndex = 27
        Me.Converter_RadioButton_Comm2Version1.Text = "盟军2版本1"
        Me.Converter_RadioButton_Comm2Version1.UseVisualStyleBackColor = True
        '
        'Converter_FileButton_TargetFile
        '
        Me.Converter_FileButton_TargetFile.Location = New System.Drawing.Point(361, 174)
        Me.Converter_FileButton_TargetFile.Name = "Converter_FileButton_TargetFile"
        Me.Converter_FileButton_TargetFile.Size = New System.Drawing.Size(31, 23)
        Me.Converter_FileButton_TargetFile.TabIndex = 26
        Me.Converter_FileButton_TargetFile.Text = "..."
        Me.Converter_FileButton_TargetFile.UseVisualStyleBackColor = True
        '
        'Converter_FileButton_SourceFile
        '
        Me.Converter_FileButton_SourceFile.Location = New System.Drawing.Point(361, 141)
        Me.Converter_FileButton_SourceFile.Name = "Converter_FileButton_SourceFile"
        Me.Converter_FileButton_SourceFile.Size = New System.Drawing.Size(31, 23)
        Me.Converter_FileButton_SourceFile.TabIndex = 25
        Me.Converter_FileButton_SourceFile.Text = "..."
        Me.Converter_FileButton_SourceFile.UseVisualStyleBackColor = True
        '
        'Converter_NumericUpDown_SaturationFactor
        '
        Me.Converter_NumericUpDown_SaturationFactor.DecimalPlaces = 2
        Me.Converter_NumericUpDown_SaturationFactor.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.Converter_NumericUpDown_SaturationFactor.Location = New System.Drawing.Point(143, 340)
        Me.Converter_NumericUpDown_SaturationFactor.Maximum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.Converter_NumericUpDown_SaturationFactor.Minimum = New Decimal(New Integer() {5, 0, 0, -2147483648})
        Me.Converter_NumericUpDown_SaturationFactor.Name = "Converter_NumericUpDown_SaturationFactor"
        Me.Converter_NumericUpDown_SaturationFactor.Size = New System.Drawing.Size(120, 21)
        Me.Converter_NumericUpDown_SaturationFactor.TabIndex = 20
        Me.Converter_NumericUpDown_SaturationFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Converter_NumericUpDown_SaturationFactor.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Converter_NumericUpDown_LightnessFactor
        '
        Me.Converter_NumericUpDown_LightnessFactor.DecimalPlaces = 2
        Me.Converter_NumericUpDown_LightnessFactor.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.Converter_NumericUpDown_LightnessFactor.Location = New System.Drawing.Point(143, 311)
        Me.Converter_NumericUpDown_LightnessFactor.Maximum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.Converter_NumericUpDown_LightnessFactor.Minimum = New Decimal(New Integer() {5, 0, 0, -2147483648})
        Me.Converter_NumericUpDown_LightnessFactor.Name = "Converter_NumericUpDown_LightnessFactor"
        Me.Converter_NumericUpDown_LightnessFactor.Size = New System.Drawing.Size(120, 21)
        Me.Converter_NumericUpDown_LightnessFactor.TabIndex = 20
        Me.Converter_NumericUpDown_LightnessFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Converter_NumericUpDown_LightnessFactor.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Converter_Label_Notice2
        '
        Me.Converter_Label_Notice2.Location = New System.Drawing.Point(22, 42)
        Me.Converter_Label_Notice2.Name = "Converter_Label_Notice2"
        Me.Converter_Label_Notice2.Size = New System.Drawing.Size(370, 40)
        Me.Converter_Label_Notice2.TabIndex = 18
        Me.Converter_Label_Notice2.Text = "盟军3版本4的Y64转换到盟军2版本2或盟军3版本3会损失一些未知数据。其他版本转换到盟军2版本1会损失副绘图文件的数据。"
        '
        'Converter_Button_Convert
        '
        Me.Converter_Button_Convert.Location = New System.Drawing.Point(292, 375)
        Me.Converter_Button_Convert.Name = "Converter_Button_Convert"
        Me.Converter_Button_Convert.Size = New System.Drawing.Size(100, 23)
        Me.Converter_Button_Convert.TabIndex = 17
        Me.Converter_Button_Convert.Text = "转换(&C)"
        Me.Converter_Button_Convert.UseVisualStyleBackColor = True
        '
        'Converter_Label_SaturationFactor
        '
        Me.Converter_Label_SaturationFactor.AutoSize = True
        Me.Converter_Label_SaturationFactor.Location = New System.Drawing.Point(22, 342)
        Me.Converter_Label_SaturationFactor.Name = "Converter_Label_SaturationFactor"
        Me.Converter_Label_SaturationFactor.Size = New System.Drawing.Size(65, 12)
        Me.Converter_Label_SaturationFactor.TabIndex = 15
        Me.Converter_Label_SaturationFactor.Text = "饱和度系数"
        '
        'Converter_Label_LightnessFactor
        '
        Me.Converter_Label_LightnessFactor.AutoSize = True
        Me.Converter_Label_LightnessFactor.Location = New System.Drawing.Point(22, 313)
        Me.Converter_Label_LightnessFactor.Name = "Converter_Label_LightnessFactor"
        Me.Converter_Label_LightnessFactor.Size = New System.Drawing.Size(53, 12)
        Me.Converter_Label_LightnessFactor.TabIndex = 15
        Me.Converter_Label_LightnessFactor.Text = "亮度系数"
        '
        'Converter_Label_TargetFile
        '
        Me.Converter_Label_TargetFile.AutoSize = True
        Me.Converter_Label_TargetFile.Location = New System.Drawing.Point(22, 179)
        Me.Converter_Label_TargetFile.Name = "Converter_Label_TargetFile"
        Me.Converter_Label_TargetFile.Size = New System.Drawing.Size(53, 12)
        Me.Converter_Label_TargetFile.TabIndex = 15
        Me.Converter_Label_TargetFile.Text = "目标文件"
        '
        'Converter_TextBox_TargetFile
        '
        Me.Converter_TextBox_TargetFile.Location = New System.Drawing.Point(143, 176)
        Me.Converter_TextBox_TargetFile.Name = "Converter_TextBox_TargetFile"
        Me.Converter_TextBox_TargetFile.Size = New System.Drawing.Size(212, 21)
        Me.Converter_TextBox_TargetFile.TabIndex = 16
        '
        'Converter_Label_SourceFile
        '
        Me.Converter_Label_SourceFile.AutoSize = True
        Me.Converter_Label_SourceFile.Location = New System.Drawing.Point(22, 146)
        Me.Converter_Label_SourceFile.Name = "Converter_Label_SourceFile"
        Me.Converter_Label_SourceFile.Size = New System.Drawing.Size(41, 12)
        Me.Converter_Label_SourceFile.TabIndex = 13
        Me.Converter_Label_SourceFile.Text = "源文件"
        '
        'Converter_TextBox_SourceFile
        '
        Me.Converter_TextBox_SourceFile.Location = New System.Drawing.Point(143, 143)
        Me.Converter_TextBox_SourceFile.Name = "Converter_TextBox_SourceFile"
        Me.Converter_TextBox_SourceFile.Size = New System.Drawing.Size(212, 21)
        Me.Converter_TextBox_SourceFile.TabIndex = 14
        '
        'Readme
        '
        Me.Readme.Controls.Add(Me.Readme_TextBox_Readme)
        Me.Readme.Location = New System.Drawing.Point(4, 21)
        Me.Readme.Name = "Readme"
        Me.Readme.Size = New System.Drawing.Size(421, 420)
        Me.Readme.TabIndex = 4
        Me.Readme.Text = "说明"
        Me.Readme.UseVisualStyleBackColor = True
        '
        'Readme_TextBox_Readme
        '
        Me.Readme_TextBox_Readme.Location = New System.Drawing.Point(0, 0)
        Me.Readme_TextBox_Readme.Multiline = True
        Me.Readme_TextBox_Readme.Name = "Readme_TextBox_Readme"
        Me.Readme_TextBox_Readme.ReadOnly = True
        Me.Readme_TextBox_Readme.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Readme_TextBox_Readme.Size = New System.Drawing.Size(421, 420)
        Me.Readme_TextBox_Readme.TabIndex = 0
        Me.Readme_TextBox_Readme.Text = "说明"
        '
        'Y64Manager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(454, 466)
        Me.Controls.Add(Me.ToolTab)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Y64Manager"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Y64管理器"
        Me.ToolTab.ResumeLayout(False)
        Me.Exporter.ResumeLayout(False)
        Me.Exporter.PerformLayout()
        CType(Me.Exporter_NumericUpDown_B, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Exporter_NumericUpDown_G, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Exporter_NumericUpDown_Cr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Exporter_NumericUpDown_Cb, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Exporter_NumericUpDown_Y, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Exporter_NumericUpDown_R, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Importer.ResumeLayout(False)
        Me.Importer.PerformLayout()
        CType(Me.Importer_NumericUpDown_Pic, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Importer_NumericUpDown_View, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Creator.ResumeLayout(False)
        Me.Creator.PerformLayout()
        Me.Converter.ResumeLayout(False)
        Me.Converter.PerformLayout()
        CType(Me.Converter_NumericUpDown_SaturationFactor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Converter_NumericUpDown_LightnessFactor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Readme.ResumeLayout(False)
        Me.Readme.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolTab As System.Windows.Forms.TabControl
    Friend WithEvents Exporter As System.Windows.Forms.TabPage
    Friend WithEvents Importer As System.Windows.Forms.TabPage
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Exporter_Button_YCbCr2RGB As System.Windows.Forms.Button
    Friend WithEvents Exporter_Button_RGB2YCbCr As System.Windows.Forms.Button
    Friend WithEvents Exporter_NumericUpDown_B As System.Windows.Forms.NumericUpDown
    Friend WithEvents Exporter_NumericUpDown_G As System.Windows.Forms.NumericUpDown
    Friend WithEvents Exporter_NumericUpDown_Cr As System.Windows.Forms.NumericUpDown
    Friend WithEvents Exporter_NumericUpDown_Cb As System.Windows.Forms.NumericUpDown
    Friend WithEvents Exporter_NumericUpDown_Y As System.Windows.Forms.NumericUpDown
    Friend WithEvents Exporter_NumericUpDown_R As System.Windows.Forms.NumericUpDown
    Friend WithEvents Creator As System.Windows.Forms.TabPage
    Friend WithEvents Importer_Label_PictureIndex As System.Windows.Forms.Label
    Friend WithEvents Importer_Label_Y64File As System.Windows.Forms.Label
    Friend WithEvents Importer_Label_ExtraPic As System.Windows.Forms.Label
    Friend WithEvents Importer_Label_MainPic As System.Windows.Forms.Label
    Friend WithEvents Importer_Button_Import As System.Windows.Forms.Button
    Friend WithEvents Importer_NumericUpDown_Pic As System.Windows.Forms.NumericUpDown
    Friend WithEvents Importer_NumericUpDown_View As System.Windows.Forms.NumericUpDown
    Friend WithEvents Importer_TextBox_Y64File As System.Windows.Forms.TextBox
    Friend WithEvents Importer_TextBox_ExtraPic As System.Windows.Forms.TextBox
    Friend WithEvents Importer_TextBox_MainPic As System.Windows.Forms.TextBox
    Friend WithEvents Importer_Label_Notice As System.Windows.Forms.Label
    Friend WithEvents Creator_Label_Y64File As System.Windows.Forms.Label
    Friend WithEvents Creator_TextBox_Y64File As System.Windows.Forms.TextBox
    Friend WithEvents Creator_Label_DescriptionFile As System.Windows.Forms.Label
    Friend WithEvents Creator_TextBox_DescriptionFile As System.Windows.Forms.TextBox
    Friend WithEvents Creator_Button_Create As System.Windows.Forms.Button
    Friend WithEvents Converter As System.Windows.Forms.TabPage
    Friend WithEvents Converter_Button_Convert As System.Windows.Forms.Button
    Friend WithEvents Converter_Label_TargetFile As System.Windows.Forms.Label
    Friend WithEvents Converter_TextBox_TargetFile As System.Windows.Forms.TextBox
    Friend WithEvents Converter_Label_SourceFile As System.Windows.Forms.Label
    Friend WithEvents Converter_TextBox_SourceFile As System.Windows.Forms.TextBox
    Friend WithEvents Converter_Label_Notice2 As System.Windows.Forms.Label
    Friend WithEvents Converter_NumericUpDown_SaturationFactor As System.Windows.Forms.NumericUpDown
    Friend WithEvents Converter_NumericUpDown_LightnessFactor As System.Windows.Forms.NumericUpDown
    Friend WithEvents Importer_Label_PicFolder As System.Windows.Forms.Label
    Friend WithEvents Importer_Button_FullImport As System.Windows.Forms.Button
    Friend WithEvents Importer_TextBox_PicFolder As System.Windows.Forms.TextBox
    Friend WithEvents Importer_FileButton_PicFolder As System.Windows.Forms.Button
    Friend WithEvents Importer_FileButton_ExtraPic As System.Windows.Forms.Button
    Friend WithEvents Importer_FileButton_MainPic As System.Windows.Forms.Button
    Friend WithEvents Importer_FileButton_Y64File As System.Windows.Forms.Button
    Friend WithEvents Creator_FileButton_Y64File As System.Windows.Forms.Button
    Friend WithEvents Creator_FileButton_DescriptionFile As System.Windows.Forms.Button
    Friend WithEvents Converter_FileButton_TargetFile As System.Windows.Forms.Button
    Friend WithEvents Converter_FileButton_SourceFile As System.Windows.Forms.Button
    Friend WithEvents Converter_Label_SaturationFactor As System.Windows.Forms.Label
    Friend WithEvents Converter_Label_LightnessFactor As System.Windows.Forms.Label
    Friend WithEvents Readme As System.Windows.Forms.TabPage
    Friend WithEvents Readme_TextBox_Readme As System.Windows.Forms.TextBox
    Friend WithEvents Converter_RadioButton_Comm3Version3 As System.Windows.Forms.RadioButton
    Friend WithEvents Converter_RadioButton_Comm2Version2 As System.Windows.Forms.RadioButton
    Friend WithEvents Converter_RadioButton_Comm2Version1 As System.Windows.Forms.RadioButton
    Friend WithEvents Converter_RadioButton_Comm3Version4 As System.Windows.Forms.RadioButton
    Friend WithEvents Converter_CheckBox_ReplacePalette As System.Windows.Forms.CheckBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
