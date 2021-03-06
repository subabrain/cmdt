'==========================================================================
'
'  File:        DistrictDataEditor.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: 地形数据编辑器
'  Version:     2013.01.20.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System.Windows.Forms
Imports FileSystem

Public Class DistrictDataEditor
    Public Changed As Boolean = False

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DistrictDataView_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DistrictDataView.CellEndEdit
        If e.ColumnIndex = TerrainInfo.Index Then
            RefreshDistrictByValue(DistrictDataView.Rows.Item(e.RowIndex))
        Else
            RefreshDistrictByProperty(DistrictDataView.Rows.Item(e.RowIndex))
        End If
    End Sub

    Private Sub OpenSEC(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If My.Forms.Surface.PicBox.SecState Is Nothing Then Return
        Dim SecFile = My.Forms.Surface.PicBox.SecState.Sec
        Dim Rows As DataGridViewRowCollection = DistrictDataView.Rows
        Rows.Clear()

        For n As Integer = 0 To SecFile.Districts.Count - 1
            With SecFile.Districts(n)
                Rows.Add(n, .n, .kx, .ky, .bz, .Terrain.ToString, _
                .Terrain.MajorType.ToString & " " & CStr(.Terrain.MajorType), _
                .Terrain.MinorType.ToString & " " & CStr(.Terrain.MinorType), _
                .Terrain.Byte2.ToString.TrimStart("0"), _
                .Terrain.Byte3.ToString.TrimStart("0"), _
                .Terrain.Byte4.ToString.TrimStart("0"), _
                .Terrain.Byte5.ToString.TrimStart("0"), _
                .Terrain.Byte6.ToString.TrimStart("0"), _
                .Terrain.Byte7.ToString.TrimStart("0"))
            End With
        Next

        Changed = False
    End Sub
    Public Sub RefreshRowByDistrict(ByVal District As SEC_Simple.DistrictInfo)
        Dim r As DataGridViewRow = Nothing
        For Each Row As DataGridViewRow In DistrictDataView.Rows
            If Row.Cells(0).Value = Index Then
                r = Row
                Exit For
            End If
        Next
        If r Is Nothing Then Return
        With District
            Dim o As Object() = {Index, .n, .kx, .ky, .bz, .Terrain.ToString, _
            .Terrain.MajorType.ToString & " " & CStr(.Terrain.MajorType), _
            .Terrain.MinorType.ToString & " " & CStr(.Terrain.MinorType), _
            .Terrain.Byte2.ToString.TrimStart("0"), _
            .Terrain.Byte3.ToString.TrimStart("0"), _
            .Terrain.Byte4.ToString.TrimStart("0"), _
            .Terrain.Byte5.ToString.TrimStart("0"), _
            .Terrain.Byte6.ToString.TrimStart("0"), _
            .Terrain.Byte7.ToString.TrimStart("0")}
            For i As Integer = 1 To r.Cells.Count - 1
                r.Cells(i).Value = o(i)
            Next
        End With
        DistrictDataView.Refresh()
    End Sub
    Public Sub RefreshDistrictByValue(ByVal r As DataGridViewRow)
        Dim k As Integer = r.Cells("TerrainInfo").ColumnIndex
        Dim t As SEC.TerrainInfo = SEC.TerrainInfo.FromString(r.Cells(k).Value)
        r.Cells(k + 1).Value = (t.MajorType.ToString & " " & CStr(t.MajorType))
        r.Cells(k + 2).Value = (t.MinorType.ToString & " " & CStr(t.MinorType))
        r.Cells(k + 3).Value = t.Byte2.ToString.TrimStart("0")
        r.Cells(k + 4).Value = t.Byte3.ToString.TrimStart("0")
        r.Cells(k + 5).Value = t.Byte4.ToString.TrimStart("0")
        r.Cells(k + 6).Value = t.Byte5.ToString.TrimStart("0")
        r.Cells(k + 7).Value = t.Byte6.ToString.TrimStart("0")
        r.Cells(k + 8).Value = t.Byte7.ToString.TrimStart("0")

        My.Forms.Surface.PicBox.SecState.Sec.Districts(r.Cells(0).Value).Terrain = t

        Changed = True
    End Sub
    Public Sub RefreshDistrictByProperty(ByVal r As DataGridViewRow)
        Dim k As Integer = r.Cells("TerrainInfo").ColumnIndex
        Dim t As New SEC.TerrainInfo()
        t.MajorType = CType(r.Cells(k + 1).Value, String).Split(" ")(1)
        t.MinorType = CType(r.Cells(k + 2).Value, String).Split(" ")(1)
        t.Byte2 = "0" & r.Cells(k + 3).Value
        t.Byte3 = "0" & r.Cells(k + 4).Value
        t.Byte4 = "0" & r.Cells(k + 5).Value
        t.Byte5 = "0" & r.Cells(k + 6).Value
        t.Byte6 = "0" & r.Cells(k + 7).Value
        t.Byte7 = "0" & r.Cells(k + 8).Value

        r.Cells(k).Value = t.ToString
        r.Cells(k + 1).Value = t.MajorType.ToString & " " & CStr(t.MajorType)
        r.Cells(k + 2).Value = t.MinorType.ToString & " " & CStr(t.MinorType)

        My.Forms.Surface.PicBox.SecState.Sec.Districts(r.Cells(0).Value).Terrain = t

        Changed = True
    End Sub
End Class
