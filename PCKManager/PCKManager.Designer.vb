<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PCKManager
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
        Me.components = New System.ComponentModel.Container()
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("")
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PCKManager))
        Me.FileListView = New System.Windows.Forms.ListView()
        Me.FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLength = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Offset = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
        Me.Menu_File = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Open1 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Open2 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Open3 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_OpenSF = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Create1 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Create2 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Create3 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Close = New System.Windows.Forms.MenuItem()
        Me.Menu_File_RecentFiles = New System.Windows.Forms.MenuItem()
        Me.MenuItem4 = New System.Windows.Forms.MenuItem()
        Me.Menu_File_Exit = New System.Windows.Forms.MenuItem()
        Me.Menu_About = New System.Windows.Forms.MenuItem()
        Me.Menu_About_About = New System.Windows.Forms.MenuItem()
        Me.Path = New System.Windows.Forms.TextBox()
        Me.Spliter = New System.Windows.Forms.SplitContainer()
        Me.Spliter2 = New System.Windows.Forms.SplitContainer()
        Me.Mask = New System.Windows.Forms.TextBox()
        Me.ContextMenu = New System.Windows.Forms.ContextMenu()
        Me.ContextMenu_Extract = New System.Windows.Forms.MenuItem()
        CType(Me.Spliter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Spliter.Panel1.SuspendLayout()
        Me.Spliter.Panel2.SuspendLayout()
        Me.Spliter.SuspendLayout()
        CType(Me.Spliter2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Spliter2.Panel1.SuspendLayout()
        Me.Spliter2.Panel2.SuspendLayout()
        Me.Spliter2.SuspendLayout()
        Me.SuspendLayout()
        '
        'FileListView
        '
        Me.FileListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.FileName, Me.FileLength, Me.Offset, Me.FileType})
        Me.FileListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileListView.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1})
        Me.FileListView.Location = New System.Drawing.Point(0, 0)
        Me.FileListView.Name = "FileListView"
        Me.FileListView.Size = New System.Drawing.Size(543, 393)
        Me.FileListView.TabIndex = 0
        Me.FileListView.UseCompatibleStateImageBehavior = False
        Me.FileListView.View = System.Windows.Forms.View.Details
        '
        'FileName
        '
        Me.FileName.Text = "文件名"
        Me.FileName.Width = 268
        '
        'FileLength
        '
        Me.FileLength.Text = "文件长度"
        Me.FileLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.FileLength.Width = 98
        '
        'Offset
        '
        Me.Offset.Text = "偏移量"
        Me.Offset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Offset.Width = 76
        '
        'FileType
        '
        Me.FileType.Text = "文件类型"
        Me.FileType.Width = 69
        '
        'MainMenu
        '
        Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_File, Me.Menu_About})
        '
        'Menu_File
        '
        Me.Menu_File.Index = 0
        Me.Menu_File.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_File_Open1, Me.Menu_File_Open2, Me.Menu_File_Open3, Me.Menu_File_OpenSF, Me.Menu_File_Create1, Me.Menu_File_Create2, Me.Menu_File_Create3, Me.Menu_File_Close, Me.Menu_File_RecentFiles, Me.MenuItem4, Me.Menu_File_Exit})
        Me.Menu_File.Text = "文件(&F)"
        '
        'Menu_File_Open1
        '
        Me.Menu_File_Open1.Index = 0
        Me.Menu_File_Open1.Text = "打开盟军1 DIR文件(&1)..."
        '
        'Menu_File_Open2
        '
        Me.Menu_File_Open2.Index = 1
        Me.Menu_File_Open2.Text = "打开盟军2 PCK文件(&2)..."
        '
        'Menu_File_Open3
        '
        Me.Menu_File_Open3.Index = 2
        Me.Menu_File_Open3.Text = "打开盟军3 PCK文件(&3)..."
        '
        'Menu_File_OpenSF
        '
        Me.Menu_File_OpenSF.Index = 3
        Me.Menu_File_OpenSF.Text = "打开打击力量 PAK文件(&4)..."
        '
        'Menu_File_Create1
        '
        Me.Menu_File_Create1.Index = 4
        Me.Menu_File_Create1.Text = "新建盟军1 DIR文件(&5)..."
        '
        'Menu_File_Create2
        '
        Me.Menu_File_Create2.Index = 5
        Me.Menu_File_Create2.Text = "新建盟军2 PCK文件(&6)..."
        '
        'Menu_File_Create3
        '
        Me.Menu_File_Create3.Index = 6
        Me.Menu_File_Create3.Text = "新建盟军3 PCK文件(&7)..."
        '
        'Menu_File_Close
        '
        Me.Menu_File_Close.Index = 7
        Me.Menu_File_Close.Text = "关闭(&C)"
        '
        'Menu_File_RecentFiles
        '
        Me.Menu_File_RecentFiles.Enabled = False
        Me.Menu_File_RecentFiles.Index = 8
        Me.Menu_File_RecentFiles.Text = "最近的文件(&R)"
        '
        'MenuItem4
        '
        Me.MenuItem4.Index = 9
        Me.MenuItem4.Text = "-"
        '
        'Menu_File_Exit
        '
        Me.Menu_File_Exit.Index = 10
        Me.Menu_File_Exit.Text = "退出(&X)"
        '
        'Menu_About
        '
        Me.Menu_About.Index = 1
        Me.Menu_About.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Menu_About_About})
        Me.Menu_About.Text = "关于(&A)"
        '
        'Menu_About_About
        '
        Me.Menu_About_About.Index = 0
        Me.Menu_About_About.Text = "关于(&A)..."
        '
        'Path
        '
        Me.Path.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Path.Location = New System.Drawing.Point(0, 0)
        Me.Path.Name = "Path"
        Me.Path.ReadOnly = True
        Me.Path.Size = New System.Drawing.Size(503, 21)
        Me.Path.TabIndex = 1
        '
        'Spliter
        '
        Me.Spliter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Spliter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.Spliter.IsSplitterFixed = True
        Me.Spliter.Location = New System.Drawing.Point(0, 0)
        Me.Spliter.Name = "Spliter"
        Me.Spliter.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'Spliter.Panel1
        '
        Me.Spliter.Panel1.Controls.Add(Me.Spliter2)
        Me.Spliter.Panel1MinSize = 20
        '
        'Spliter.Panel2
        '
        Me.Spliter.Panel2.Controls.Add(Me.FileListView)
        Me.Spliter.Panel2MinSize = 20
        Me.Spliter.Size = New System.Drawing.Size(543, 417)
        Me.Spliter.SplitterDistance = 22
        Me.Spliter.SplitterWidth = 2
        Me.Spliter.TabIndex = 2
        '
        'Spliter2
        '
        Me.Spliter2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Spliter2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.Spliter2.IsSplitterFixed = True
        Me.Spliter2.Location = New System.Drawing.Point(0, 0)
        Me.Spliter2.Name = "Spliter2"
        '
        'Spliter2.Panel1
        '
        Me.Spliter2.Panel1.Controls.Add(Me.Path)
        '
        'Spliter2.Panel2
        '
        Me.Spliter2.Panel2.Controls.Add(Me.Mask)
        Me.Spliter2.Size = New System.Drawing.Size(543, 22)
        Me.Spliter2.SplitterDistance = 503
        Me.Spliter2.SplitterWidth = 2
        Me.Spliter2.TabIndex = 2
        '
        'Mask
        '
        Me.Mask.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Mask.Location = New System.Drawing.Point(0, 0)
        Me.Mask.Name = "Mask"
        Me.Mask.Size = New System.Drawing.Size(38, 21)
        Me.Mask.TabIndex = 1
        Me.Mask.Text = "*.*"
        '
        'ContextMenu
        '
        Me.ContextMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ContextMenu_Extract})
        '
        'ContextMenu_Extract
        '
        Me.ContextMenu_Extract.Index = 0
        Me.ContextMenu_Extract.Text = "解压(&E)..."
        '
        'PCKManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(543, 417)
        Me.Controls.Add(Me.Spliter)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Menu = Me.MainMenu
        Me.Name = "PCKManager"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PCK文件管理器"
        Me.Spliter.Panel1.ResumeLayout(False)
        Me.Spliter.Panel2.ResumeLayout(False)
        CType(Me.Spliter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Spliter.ResumeLayout(False)
        Me.Spliter2.Panel1.ResumeLayout(False)
        Me.Spliter2.Panel1.PerformLayout()
        Me.Spliter2.Panel2.ResumeLayout(False)
        Me.Spliter2.Panel2.PerformLayout()
        CType(Me.Spliter2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Spliter2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FileListView As System.Windows.Forms.ListView
    Friend WithEvents FileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents FileLength As System.Windows.Forms.ColumnHeader
    Friend WithEvents Offset As System.Windows.Forms.ColumnHeader
    Friend WithEvents FileType As System.Windows.Forms.ColumnHeader
    Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
    Friend WithEvents Menu_File As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Open2 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Open3 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Exit As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_About As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_About_About As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_RecentFiles As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Create3 As System.Windows.Forms.MenuItem
    Friend WithEvents Path As System.Windows.Forms.TextBox
    Private WithEvents Spliter As System.Windows.Forms.SplitContainer
    Friend WithEvents Spliter2 As System.Windows.Forms.SplitContainer
    Friend WithEvents Mask As System.Windows.Forms.TextBox
    Friend Shadows WithEvents ContextMenu As System.Windows.Forms.ContextMenu
    Friend WithEvents ContextMenu_Extract As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Close As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Open1 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_OpenSF As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Create2 As System.Windows.Forms.MenuItem
    Friend WithEvents Menu_File_Create1 As System.Windows.Forms.MenuItem

End Class
