'==========================================================================
'
'  File:        MA2Handler.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: MA2文件类
'  Version:     2011.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text

Imports System
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Firefly
Imports Firefly.Imaging
Imports FileSystem

Public Class MA2Handler

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim Path As String = TextBox1.Text.Replace("/", "\").TrimEnd("\")
        Dim Name As String = GetMainFileName(Path)
        Dim Dir As String = GetFileDirectory(Path)
        If IsMatchFileMask(Path, "*.MA2") Then
            'Dim m As New MA2(Path)
            'Dim mySerializer As XmlSerializer = New XmlSerializer(GetType(MA2), "FileSystem")
            'Dim myWriter As StreamWriter = New StreamWriter(GetPath(Dir, Name) & ".xml")
            'mySerializer.Serialize(myWriter, m)
            'myWriter.Close()
            'MsgBox("生成完成，因为文件可能较大，推荐使用UltraEdit打开。")
            Dim m As New MA2(Path)
            If TextBox2.Text = "" Then
                Dim Index As Integer = NumericUpDown1.Value
                Dim RenderInfo As New List(Of MA2.RenderInfoBlock())
                For n As Integer = 0 To m.RenderInfo.GetUpperBound(0)
                    Dim RenderObject As New List(Of MA2.RenderInfoBlock)
                    For k As Integer = 0 To m.RenderInfo(n).GetUpperBound(0)
                        Dim ro As MA2.RenderInfoBlock = m.RenderInfo(n)(k)
                        If ro.ObjectIndex = Index Then
                            ro.ObjectIndex = 0
                            'If ro.Height > 8 Then
                            '    ro.Height = 8
                            '    ro.SubRenderData = New MA2.SubRenderDataSection() {ro.SubRenderData(0), ro.SubRenderData(1)}
                            'End If
                            RenderObject.Add(ro)
                        End If
                        Dim i As New Gif.GifImageBlock(ro.RenderMap.ToMultiDimensionArray)
                        Dim b As New Gif(ro.Width, ro.Height, 4, New Gif.GifImageBlock() {i}, New Int32() {0, &HFFFF0000, &HFFFFFFFF, &HFF0000FF})
                        If Not IO.Directory.Exists(Path & ".files") Then IO.Directory.CreateDirectory(Path & ".files")
                        b.WriteToFile(GetPath(Path & ".files", Name & "_" & ro.ObjectIndex & "_" & n & ".gif"))
                    Next
                    RenderInfo.Add(RenderObject.ToArray)
                Next
                'm.ObjectInfo = New MA2.ObjectInfoBlock() {m.ObjectInfo(Index)}
                'm.RenderInfo = RenderInfo.ToArray
                'm.WriteToFile(TextBox2.Text)
            Else
                m.WriteToFile(TextBox2.Text)
            End If

            MsgBox("完成")
        ElseIf IsMatchFileMask(Path, "*.Y64") Then
            'Dim f As Y64 = Y64.Open(Path, FileAccess.Read)
            'Dim s As New StreamWriter(Path & ".TXT", False)
            'For Pic As Integer = 0 To f.NumberPic - 1
            '    For View As Integer = 0 To f.NumberView - 1
            '        s.WriteLine("Pic(" & View & "," & Pic & ")")
            '        Dim PA As Y64.PictureArea = f.PicArea(View, Pic)
            '        Dim Address As Int32 = PA.PicBlockAddress(0)
            '        Dim OffsetTable As Int32() = PA.OffsetTable
            '        For Index As Integer = 0 To PA.NumberPicBlockReal - 1
            '            Address += 3592
            '            s.Write(ReplaceHeadZeros(Index.ToString("0000")))
            '            s.Write("--")
            '            s.Write("0x" & Address.ToString("X8"))
            '            s.Write("--")
            '            s.Write(ReplaceHeadZeros(OffsetTable(Index).ToString("000000")))

            '            Dim PB As Y64.PictureBlock = PA.PicBlock(Index)
            '            f.Position = Address
            '            Address += OffsetTable(Index)
            '            s.Write("--")
            '            s.Write("0x" & Address.ToString("X8"))
            '            s.Write("-")
            '            Dim n As Integer = 0
            '            While n <= OffsetTable(Index) - 2
            '                Dim b As Byte = f.ReadByte
            '                n += 1
            '                Select Case b
            '                    Case &H14, &H15, &H16
            '                        If f.ReadByte = &HFF Then
            '                            s.Write(" " & b.ToString("X2") & "FF-")
            '                            s.Write("0x" & (f.Position - 2).ToString("X8"))
            '                        End If
            '                        n += 1
            '                End Select
            '            End While
            '            s.WriteLine()
            '        Next
            '    Next
            'Next
            'f.Close()
            's.Close()
            'MsgBox("完成")
        ElseIf IsMatchFileMask(Path, "*.BMP") Then
            Dim r As Int32(,)
            Using f = Bmp.Open(Path)
                r = f.GetRectangle(0, 0, 64, 64)
                For y As Integer = 0 To 63
                    For x As Integer = 0 To 63
                        r(x, y) = Y64.RGB32To16(r(x, y))
                    Next
                Next
            End Using
            Using f2 As New Bmp(Path & "_2.bmp", 64, 64, 16)
                f2.SetRectangle(0, 0, r)
            End Using
        End If
    End Sub
    Public Function ReplaceHeadZeros(ByVal s As String) As String
        Dim c As Char() = s
        For n As Integer = 0 To c.GetUpperBound(0) - 1
            If c(n) = "0"c Then
                c(n) = " "c
            Else
                Exit For
            End If
        Next
        Return c
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        With TextBox1
            Static d As Windows.Forms.OpenFileDialog
            If d Is Nothing Then d = New Windows.Forms.OpenFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "MA2,Y64(*.MA2;*.Y64)|*.MA2;*.Y64"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" And T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        With TextBox2
            Static d As Windows.Forms.SaveFileDialog
            If d Is Nothing Then d = New Windows.Forms.SaveFileDialog
            Dim dir As String = GetFileDirectory(.Text)
            If IO.Directory.Exists(dir) Then
                d.FileName = .Text
            End If
            d.Filter = "MA2,Y64(*.MA2;*.Y64)|*.MA2;*.Y64"
            If d.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim T As String = GetRelativePath(d.FileName, Application.StartupPath)
                If T <> "" AndAlso d.FileName <> "" And T.Length < d.FileName.Length Then
                    .Text = T
                Else
                    .Text = d.FileName
                End If
            End If
        End With
        My.Computer.FileSystem.CurrentDirectory = Application.StartupPath
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim Path As String = TextBox1.Text.Replace("/", "\").TrimEnd("\")
        Dim Name As String = GetMainFileName(Path)
        Dim Dir As String = GetFileDirectory(Path)
        If IsMatchFileMask(Path, "*.MA2") Then
            Dim m As New MA2(Path)
            If m.NumObject > 0 Then
                NumericUpDown1.Minimum = 0
                NumericUpDown1.Maximum = m.NumObject() - 1
            Else
                NumericUpDown1.Minimum = -1
                NumericUpDown1.Maximum = -1
            End If
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim Path As String = TextBox1.Text.Replace("/", "\").TrimEnd("\")
        Dim Name As String = GetMainFileName(Path)
        Dim Dir As String = GetFileDirectory(Path)
        If IsMatchFileMask(TextBox1.Text, "*.Y64") Then
            'Dim f As Y64 = Y64.Open(Path, FileAccess.ReadWrite)
            'Dim View As Integer = NumericUpDown2.Value
            'If View > f.NumberView - 1 Then
            '    MsgBox("视角不存在")
            '    f.Close()
            '    Return
            'End If
            'Dim Index As Integer = NumericUpDown3.Value
            'Dim PA As Y64.PictureArea = f.PicArea(View, 0)
            'If Index > PA.NumberPicBlockReal - 1 Then
            '    MsgBox("图像块不存在")
            '    f.Close()
            '    Return
            'End If
            'Dim PB As Y64.PictureBlock = PA.PicBlock(Index)
            'Dim r = PB.GetRectangle(f)
            'For y = 0 To 63
            '    For x = 0 To 63
            '        r(x, y) = Not r(x, y)
            '    Next
            'Next
            'f.GetReflectionTable()
            'PB.SetRectangle(r, f)
            'PA.PicBlock(Index) = PB
            'f.Close()
            'MsgBox("成功")
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim Path As String = TextBox1.Text.Replace("/", "\").TrimEnd("\")
        Dim Name As String = GetMainFileName(Path)
        Dim Dir As String = GetFileDirectory(Path)
        If IsMatchFileMask(TextBox1.Text, "*.Y64") Then
            'Dim f As Y64 = Y64.Open(Path, FileAccess.ReadWrite)
            'If f.VersionSign <> Y64.Version.Comm3Version3 AndAlso f.VersionSign <> Y64.Version.Comm3Version4 Then Throw New InvalidDataException
            'Dim View As Integer = NumericUpDown2.Value
            'If View > f.NumberView - 1 Then
            '    MsgBox("视角不存在")
            '    f.Close()
            '    Return
            'End If
            'Dim Index As Integer = NumericUpDown3.Value
            'Dim PA As Y64.PictureArea = f.PicArea(View, 0)
            'If Index > PA.NumberPicBlockReal - 1 Then
            '    MsgBox("图像块不存在")
            '    f.Close()
            '    Return
            'End If
            'For n As Integer = 0 To PA.NumberPicBlockReal - 1
            '    If n = Index Then Continue For
            '    Dim PB As Y64.PictureBlock = PA.PicBlock(n)
            '    If f.ReadInt16() = &HFF15S Then
            '        f.Position -= 2
            '        f.WriteInt16(&HFF16S)
            '    End If
            'Next
            'f.Close()
            'MsgBox("成功")
        End If
    End Sub
End Class
