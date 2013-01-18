'==========================================================================
'
'  File:        Surface.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: SEC地形文件编辑器
'  Version:     2013.01.18.
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

    Public Function OpenSEC(ByVal Path As String) As SEC_Simple.FileInfo
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
        Return SECFile
    End Function
    Public Sub SaveSEC(ByVal SECFile As SEC_Simple.FileInfo)
        If SECXml Then
            SEC_Simple.SaveXml(SECPath, SECFile)
        Else
            SEC_Simple.Save(SECPath, SECFile)
        End If
    End Sub

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
                Dim SECFile = .OpenSEC(argv(0))
                .AttachToSEC(SECFile)
                If .Menu_GraphicsInterface_GDIP.Checked Then
                    .PicBox.UseGDIP()
                ElseIf .Menu_GraphicsInterface_SlimDX.Checked Then
                    .PicBox.UseD3D()
                End If
                .Text = "SECEditor - " & .SECPath
                .PicBox.Changed = False

                .Menu_File_Save.Enabled = True
                .Menu_File_Close.Enabled = True
                .Menu_View_DistrictDataEditor.Enabled = True
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
            NumericUpDown_Point_Select.Minimum = 0
            NumericUpDown_Point_Select.Maximum = SECFile.Points.Count - 1
            NumericUpDown_District_Select.Minimum = 0
            NumericUpDown_District_Select.Maximum = SECFile.Districts.Count - 1
            Button_Point_Select.Enabled = True
            Button_District_Select.Enabled = True
            NumericUpDown_Point_Select.Enabled = True
            NumericUpDown_District_Select.Enabled = True
        Else
            Button_Point_Select.Enabled = False
            Button_District_Select.Enabled = False
            NumericUpDown_Point_Select.Enabled = False
            NumericUpDown_District_Select.Enabled = False
        End If
        Button_District_n_Change.Enabled = False
        NumericUpDown_District_n_Change.Enabled = False
    End Sub

    Private Sub Surface_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PicBox.Initialize("SECEditor", PropertyGrid, ToolStripStatusLabel2, ToolStripStatusLabel1)
        PicBox.Select()

        ToolTip1.SetToolTip(Me.Button_Point_Select, "按住Ctrl键点击可增加选择或取消选择")
        ToolTip1.SetToolTip(Me.Button_District_Select, "按住Ctrl键点击可增加选择或取消选择")
        ToolTip1.SetToolTip(Me.Button_TerrainBrush, "将选择的所有面都涂为指定地形")
        ToolTip1.SetToolTip(Me.Button_Compact, "清理所有没有使用的点和面，并重新编号")
    End Sub

    Private Sub Surface_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If PicBox.Changed Then
            Select Case MsgBox("SEC文件已修改, 保存吗?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    SaveSEC(PicBox.SECFile)
                Case MsgBoxResult.Cancel
                    e.Cancel = True
                    Return
            End Select
            PicBox.Changed = False
        End If
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

    Private Sub Menu_File_Open_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Open.Click
        If PicBox.Changed Then
            Select Case MsgBox("SEC文件已修改, 保存吗?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    SaveSEC(PicBox.SECFile)
                Case MsgBoxResult.Cancel
                    Return
            End Select
            PicBox.Changed = False
        End If

        Static d As Windows.Forms.OpenFileDialog
        If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
        d.Filter = "SEC(*.SEC;*.xml)|*.SEC;*.xml"
        If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim SECFile = OpenSEC(d.FileName)
            SECPath = d.FileName
            If Menu_GraphicsInterface_GDIP.Checked Then
                PicBox.UseGDIP()
            ElseIf Menu_GraphicsInterface_SlimDX.Checked Then
                PicBox.UseD3D()
            End If
            AttachToSEC(SECFile)
            Me.Text = "SECEditor - " & SECPath
            PicBox.Changed = False

            Menu_File_Save.Enabled = True
            Menu_File_Close.Enabled = True
            Menu_View_DistrictDataEditor.Enabled = True
        End If
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub


    Private Sub Menu_File_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Exit.Click
        If PicBox.Changed Then
            Select Case MsgBox("SEC文件已修改, 保存吗?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    SaveSEC(PicBox.SECFile)
                Case MsgBoxResult.Cancel
                    Return
            End Select
            PicBox.Changed = False
        End If
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
        If PicBox.Changed Then
            Select Case MsgBox("SEC文件已修改, 保存吗?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    SaveSEC(PicBox.SECFile)
                Case MsgBoxResult.Cancel
                    Return
            End Select
            PicBox.Changed = False
        End If
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

    Private Sub Menu_File_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Save.Click
        If PicBox.SECFile IsNot Nothing Then
            SaveSEC(PicBox.SECFile)
            PicBox.Changed = False
            Me.Text = "SECEditor - " & SECPath
        End If
    End Sub

    Private Sub Menu_File_SaveAndClean_Click(sender As Object, e As EventArgs)
        If PicBox.SECFile IsNot Nothing Then
            SaveSEC(PicBox.SECFile)
            OpenSEC(SECPath)
            PicBox.Changed = False
            Me.Text = "SECEditor - " & SECPath
        End If
    End Sub

    Private Sub PropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid.PropertyValueChanged
        Me.Text = "SECEditor - " & SECPath & "*"
        If TypeOf PicBox.PropertyGrid.SelectedObject Is SEC_Simple.DistrictInfo Then
            My.Forms.DistrictDataEditor.RefreshRowByDistrict(DirectCast(PicBox.PropertyGrid.SelectedObject, SEC_Simple.DistrictInfo))
        End If
    End Sub

    Private Sub Menu_View_DistrictDataEditor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Menu_View_DistrictDataEditor.Click
        If PicBox.SECFile Is Nothing Then Return
        My.Forms.DistrictDataEditor.Show()
        My.Forms.DistrictDataEditor.Activate()
    End Sub

    Private Sub Surface_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        If My.Forms.DistrictDataEditor.Changed Then
            PicBox.Changed = True
            My.Forms.DistrictDataEditor.Changed = False
            Me.Text = "SECEditor - " & SECPath & "*"
            PropertyGrid.SelectedObject = PropertyGrid.SelectedObject
            PicBox.ReGraph()
        End If
    End Sub

    Private Sub Button_Point_Select_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Point_Select.Click
        Dim v = CInt(NumericUpDown_Point_Select.Value)
        If v >= 0 AndAlso v < PicBox.SecState.SecPoints.Count Then
            PicBox.Select()
            PicBox.PickPoint(PicBox.SecState.SecPoints(v), (Control.ModifierKeys And Keys.Control) <> 0)
            PicBox.FocusOn()
            PicBox.ReGraph()
        End If
    End Sub

    Private Sub Button_District_Select_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button_District_Select.Click
        Dim v = CInt(NumericUpDown_District_Select.Value)
        If v >= 0 AndAlso v < PicBox.SecState.SecDistricts.Count Then
            PicBox.Select()
            PicBox.PickDistrict(PicBox.SecState.SecDistricts(v), (Control.ModifierKeys And Keys.Control) <> 0)
            PicBox.FocusOn()
            PicBox.ReGraph()
        End If
    End Sub

    Private Sub Button_District_n_Change_Click(sender As Object, e As EventArgs) Handles Button_District_n_Change.Click

    End Sub
End Class
