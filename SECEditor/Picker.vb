'==========================================================================
'
'  File:        Picker.vb
'  Location:    SECEditor <Visual Basic .Net>
'  Description: 拾取器
'  Version:     2011.04.07.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports Color = System.Drawing.Color
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports FileSystem
Imports GraphSystem

Public Class Picker
    Inherits Grapher

    Public Y64File As Y64
    Public SECFile As SEC
    Public MA2File As MA2

    Public Changed As Boolean

    Public WithEvents PropertyGrid As PropertyGrid
    Public DistrictDisplayer As ToolStripStatusLabel
    Public ControlModeDisplayer As ToolStripStatusLabel

    Public Sub ResetView()
        With NDWorld
            Dim m As Decimal(,) = {{0, 1, 0}, {0, 0, -1}, {-1, 0, 0}}
            .HomotheticTransformation = New HomotheticTransformation(New Matrix(m), New Vector(0, 0, 3000))
            '.HomotheticTransformation.Rotate(1, 2, PI / 2)
            .HomotheticTransformation.Rotate(2, 0, CRad(20))
            .HomotheticTransformation.Rotate(1, 2, CRad(30))
        End With
    End Sub

    Public Shadows Sub Initialize(ByVal ProgramName As String, Optional ByVal PropertyGrid As PropertyGrid = Nothing, Optional ByVal DistrictDisplayer As ToolStripStatusLabel = Nothing, Optional ByVal ControlModeDisplayer As ToolStripStatusLabel = Nothing)
        NDWorld = New NDWorld(New World) '声明坐标系统
        With NDWorld
            .ProgramName = ProgramName
            .Clear()
            .ColorAxis = Drawing.Color.Black.ToArgb
            .ColorBack = Drawing.Color.LightGray.ToArgb

            ResetView()

            .ShowAxis = True
            .ShowText = False
            .FontName = "宋体"
            .FontSize = 9
            .Add(New Point(New Vector(500, 0, 0), Color.Red))
            .Add(New Point(New Vector(0, 500, 0), Color.Green))
            .Add(New Point(New Vector(0, 0, 500), Color.Blue))
        End With

        MyBase.Initialize(NDWorld, Nothing)

        MoveSpeed = 150
        ControlMode = Grapher.ControlModeEnum.FirstLand

        Me.PropertyGrid = PropertyGrid
        Me.DistrictDisplayer = DistrictDisplayer
        If DistrictDisplayer IsNot Nothing Then DistrictDisplayer.Text = "District None"
        Me.ControlModeDisplayer = ControlModeDisplayer
        If ControlModeDisplayer IsNot Nothing Then ControlModeDisplayer.Text = "ControlMode " & Me.ControlMode.ToString
    End Sub

    Public Sub UseGDIP()
        'ImageDistance = Resolution * RealImageDistance
        'RealImageDistance = 0.5 m
        'Resolution = 337.9 mm / 1024 pixel ~= 0.33mm / pixel
        'ImageDistance ~= 1500 pixel
        Dim ImageDistance As Double = 1500
        If Picture IsNot Nothing Then
            ImageDistance = Picture.ImageDistance
            Picture.Dispose()
        End If
        Picture = New PicGDIPlusPer(ImageDistance)
        CType(Picture, PicGDIPlusPer).EnableSort = True
        MyBase.Initialize(NDWorld, Picture)
        ReGraph()
    End Sub
    Public Sub UseD3D()
        'Dim ImageDistance As Double = 1500
        'If Picture IsNot Nothing Then
        '    ImageDistance = Picture.ImageDistance
        '    Picture.Dispose()
        'End If
        'Picture = New PicD3D(ImageDistance)
        'MyBase.Initialize(NDWorld, Picture)
        'ReGraph()
    End Sub

    Public Sub ReGraph()
        If TypeOf Picture Is PicGDIPlusPer Then
            Graph()
            'ElseIf TypeOf Picture Is PicD3D Then
            '    CheckSize()
            '    CType(Picture, PicD3D).NotifyModelChanged()
            '    Picture.Graph()
        End If
    End Sub

    Protected Map As New Dictionary(Of Polygon, SEC.DistrictInfo)
    Protected DistrictTopPolygonMap As New Dictionary(Of SEC.DistrictInfo, Polygon)
    Protected BorderOfDistrictMap As New Dictionary(Of Polygon, Integer)
    Protected CurrentDistrictValue As SEC.DistrictInfo
    Public ReadOnly Property CurrentDistrict() As SEC.DistrictInfo
        Get
            Return CurrentDistrictValue
        End Get
    End Property
    Protected CurrentPolygon As Polygon
    Protected CurrentPolygonBorderOfDistrict As Integer
    Protected CurrentPolygonColor As ColorInt32

    Protected Function IsInDistrict(ByVal P As Vector, ByVal d As SEC.DistrictInfo) As Boolean
        '传入的District必须是凸多边形
        If d.n < 3 Then Return False
        Dim PA As SEC.PointInfo = SECFile.Points(SECFile.Borders(d.Border(0)).StartPointIndex)
        Dim A As New Vector(PA.x, PA.y)
        Dim B As Vector
        Dim PC As SEC.PointInfo = SECFile.Points(SECFile.Borders(d.Border(1)).StartPointIndex)
        Dim C As New Vector(PC.x, PC.y)
        For i As Integer = 2 To d.n - 1
            B = C
            PC = SECFile.Points(SECFile.Borders(d.Border(i)).StartPointIndex)
            C = New Vector(PC.x, PC.y)
            If IsInTriangle(P, A, B, C) Then Return True
        Next
        Return False
    End Function

    Public Sub Pick(ByVal x As Integer, ByVal y As Integer)
        If SECFile Is Nothing Then Return
        If CurrentPolygon IsNot Nothing Then
            CurrentPolygon.Filler = CurrentPolygonColor
            CurrentPolygon = Nothing
            CurrentDistrictValue = Nothing
            If DistrictDisplayer IsNot Nothing Then DistrictDisplayer.Text = "District None"
            If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing
        End If
        CurrentPolygon = NDWorld.GetBestFitPicking(x - Picture.Width \ 2, y - Picture.Height \ 2, Picture.ImageDistance)
        If CurrentPolygon IsNot Nothing Then
            CurrentPolygonBorderOfDistrict = BorderOfDistrictMap(CurrentPolygon)
            CurrentPolygonColor = CurrentPolygon.Filler
            CurrentDistrictValue = Map(CurrentPolygon)
            CurrentPolygon.Filler = CSelectionColor(CurrentPolygon.Filler)
            If DistrictDisplayer IsNot Nothing Then DistrictDisplayer.Text = "District " & SECFile.DistrictID(CurrentDistrictValue).ToString("#0000")
            If PropertyGrid IsNot Nothing Then
                PropertyGrid.SelectedObject = CurrentDistrictValue
                PropertyGrid.ExpandAllGridItems()
            End If
        End If
    End Sub

    Public Sub FocusOn(ByVal d As Polygon)
        If d Is Nothing Then Return
        Dim o As New Vector
        For Each Pos As Vector In d.PosArray
            o += Pos
        Next
        o *= 1 / d.PosArray.GetLength(0)

        Dim Distance As Integer = NDWorld.HomotheticTransformation.CPos(FocusPoint)(2)

        FocusPoint = o

        If NDWorld.HomotheticTransformation Is Nothing Then NDWorld.HomotheticTransformation = New HomotheticTransformation
        NDWorld.HomotheticTransformation.RefPos = -(NDWorld.HomotheticTransformation * FocusPoint) + New Vector(0, 0, Distance)
    End Sub

    Public Sub FocusIn(ByVal d As Polygon)
        If d Is Nothing Then Return
        Dim o As New Vector
        For Each Pos As Vector In d.PosArray
            o += Pos
        Next
        o *= 1 / d.PosArray.GetLength(0)
        FocusPoint = o

        Dim RadiusSqr As Double = 0
        For Each Pos As Vector In d.PosArray
            Dim Sqr As Double = (Pos - o).Sqr
            If Sqr > RadiusSqr Then RadiusSqr = Sqr
        Next

        If NDWorld.HomotheticTransformation Is Nothing Then NDWorld.HomotheticTransformation = New HomotheticTransformation
        NDWorld.HomotheticTransformation.RefPos = -(NDWorld.HomotheticTransformation * FocusPoint) + New Vector(0, 0, Sqrt(RadiusSqr) * 10)
    End Sub


    Public Sub AttachToSEC(ByVal SECFile As SEC)
        NDWorld.Clear()
        Map.Clear()
        BorderOfDistrictMap.Clear()
        FocusPoint = Nothing
        CurrentDistrictValue = Nothing
        CurrentPolygon = Nothing
        CurrentPolygonBorderOfDistrict = 0
        CurrentPolygonColor = 0
        If PropertyGrid IsNot Nothing Then PropertyGrid.SelectedObject = Nothing
        If DistrictDisplayer IsNot Nothing Then DistrictDisplayer.Text = "District None"
        Me.SECFile = SECFile
        If SECFile Is Nothing Then
            ReGraph()
            Return
        End If

        For Each d As SEC.DistrictInfo In SECFile.Districts
            '绘制上顶面
            Dim Points As New List(Of Vector)
            For Each bi As Integer In d.Border
                Dim p As SEC.PointInfo = SECFile.Points(SECFile.Borders(bi).StartPointIndex)
                Points.Add(New Vector(p.x, p.y, d.GetZ(p.x, p.y)))
            Next
            Dim TopPolygon As New Polygon(Points.ToArray, Color.Red, True, GetColor(d), True)
            NDWorld.Add(TopPolygon)
            Map.Add(TopPolygon, d)
            BorderOfDistrictMap.Add(TopPolygon, -1)
            DistrictTopPolygonMap.Add(d, TopPolygon)

            '绘制侧面
            Dim BorderOfDistrict As Integer = 0
            For Each bi As Integer In d.Border
                Dim b As SEC.BorderInfo = SECFile.Borders(bi)
                If b.NeighborDistrictIndex = -1 Then
                    BorderOfDistrict += 1
                    Continue For
                End If

                Dim StartPoint As SEC.PointInfo = SECFile.Points(b.StartPointIndex)
                Dim EndPoint As SEC.PointInfo = SECFile.Points(b.EndPointIndex)
                Dim Az As Double = d.GetZ(StartPoint.x, StartPoint.y)
                Dim Bz As Double
                Dim Cz As Double
                Dim Dz As Double = d.GetZ(EndPoint.x, EndPoint.y)

                If b.NeighborDistrictIndex = -1 Then
                    Bz = 0
                    Cz = 0
                Else
                    Dim nd As SEC.DistrictInfo = SECFile.Districts(b.NeighborDistrictIndex)
                    Bz = nd.GetZ(StartPoint.x, StartPoint.y)
                    Cz = nd.GetZ(EndPoint.x, EndPoint.y)
                End If

                Const epsilon As Double = 0.25 '误差范围，不能过小，否则会顶点数过多，超出Short范围
                If Az - Bz < epsilon OrElse Dz - Cz < epsilon Then
                    BorderOfDistrict += 1
                    Continue For '两邻边重合或者当前区域比相邻区域低，无需绘制
                End If

                Dim OA As New Vector(StartPoint.x, StartPoint.y, Az)
                Dim OB As New Vector(StartPoint.x, StartPoint.y, Bz)
                Dim OC As New Vector(EndPoint.x, EndPoint.y, Cz)
                Dim OD As New Vector(EndPoint.x, EndPoint.y, Dz)

                Dim SidePolygon As New Polygon(New Vector() {OA, OB, OC, OD}, Color.Red, True, Color.White, True)
                NDWorld.Add(SidePolygon)
                Map.Add(SidePolygon, d)
                BorderOfDistrictMap.Add(SidePolygon, BorderOfDistrict)
            Next
        Next

        'Dim O As New Vector(SECFile.MinX, SECFile.MinY)
        'Dim PA As Vector = O + New Vector(64, 64)
        'Dim PB As Vector = O + New Vector(0, 64)
        'Dim PC As Vector = O + New Vector(0, 0)
        'Dim PD As Vector = O + New Vector(64, 0)
        'For y As Integer = 0 To SECFile.NumberMeshY - 1
        '    For x As Integer = 0 To SECFile.NumberMeshX - 1
        '        Dim P As New Vector(x * 64, y * 64)
        '        Dim R As SEC.Mesh = SECFile.MeshDS(x, y)
        '        If R.NumDistrict > 0 AndAlso Array.IndexOf(R.DistrictIndex, 109) >= 0 Then
        '            NDWorld.Add(New Polygon(New Vector() {P + PA, P + PB, P + PC, P + PD}, Color.Yellow, True, Color.Yellow))
        '        Else
        '            NDWorld.Add(New Polygon(New Vector() {P + PA, P + PB, P + PC, P + PD}, Color.Purple, False, Nothing))
        '        End If
        '    Next
        'Next

        ReGraph()
    End Sub

    Public Shared Function GetColor(ByVal d As SEC.DistrictInfo) As ColorInt32
        Dim c As ColorInt32

        '根据主类型
        Select Case d.Terrain.MajorType
            Case 0  '陆地
                c = Color.DarkOrange
            Case 1 '雪地
                c = Color.White
            Case 2 '深水
                c = Color.DarkBlue
            Case 3 '浅水
                c = Color.RoyalBlue
            Case 4 '水下
                c = Color.CadetBlue
            Case Else
                Throw New IO.InvalidDataException
        End Select

        '根据子类型
        Select Case d.Terrain.MinorType
            Case 0 '柔软地面(土壤、沙)

            Case 1 '草地
                c = Color.LimeGreen
            Case 2 '硬质地面(路、甲板)
                c = Color.Gainsboro
            Case 3 '弱硬质地面
                c = Color.Gainsboro
            Case 4 '木条地面
                c = Color.Goldenrod
            Case 5 '木质地面
                c = Color.Goldenrod
            Case 6 '半柔软地面
                c = Color.Khaki
            Case 7 '雪地

            Case 8 '冰面
                c = Color.White
            Case 9 '半硬质地面
                c = Color.Linen
            Case 10 '草丛
                c = Color.Green
            Case 11 '金属网格地面(铁栏杆、铁楼梯)
                c = Color.Silver
            Case 12 '金属地面
                c = Color.Silver
            Case 13 '浅水滩

            Case 14 '水

            Case 15 '石子地
                c = Color.Silver
            Case Else
                Throw New IO.InvalidDataException
        End Select

        '根据是否可进入
        If Not d.Terrain.IsEnterable Then
            c.R \= 4
            c.G \= 4
            c.B \= 4
        End If

        '根据是否是阴影
        If d.Terrain.IsShadow Then
            c.R \= 2
            c.G \= 2
            c.B \= 2
        End If

        '根据是否有未知的属性
        If d.Terrain.UnknownAttributes <> "None" Then
            c = &HFFFF00FF
        End If

        Return c
    End Function

    Public Shared Function CSelectionColor(ByVal c As ColorInt32) As ColorInt32
        Dim R As Byte = (CInt(Not c.R) + 191) \ 2
        Dim G As Byte = (CInt(Not c.G) + 191) \ 2
        Dim B As Byte = (CInt(Not c.B) + 191) \ 2
        Return New ColorInt32(c.A, R, G, B)
    End Function

    Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
        If Picture IsNot Nothing Then ReGraph()
        MyBase.OnSizeChanged(e)
    End Sub

    Protected Overrides Sub DoKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        Static LastTime As Double '上次处理的时间 用来处理过量按键等问题
        If DateAndTime.Timer - LastTime < 0.4 Then Exit Sub
        LastTime = DateAndTime.Timer
        e.Handled = True
        Select Case e.KeyData
            Case Keys.Tab
                e.Handled = False
                MyBase.DoKeyUp(e)
                e.Handled = True
                If ControlModeDisplayer IsNot Nothing Then ControlModeDisplayer.Text = "ControlMode " & Me.ControlMode.ToString
            Case Keys.F
                Dim FocusPoint1 As Vector = FocusPoint
                FocusOn(CurrentPolygon)
                Dim FocusPoint2 As Vector = FocusPoint
                If Equal(FocusPoint1, FocusPoint2) Then FocusIn(CurrentPolygon)
            Case Else
                e.Handled = False
        End Select
        If e.Handled Then Graph()

        MyBase.DoKeyUp(e)
    End Sub

    Protected Overrides Sub DoMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.DoMouseDown(e)
    End Sub
    Protected Overrides Sub DoMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        Select Case e.Clicks
            Case 1
                Select Case e.Button
                    Case Windows.Forms.MouseButtons.Left
                        [Select]()
                        Pick(e.X, e.Y)
                        ReGraph()
                        Return
                End Select
            Case 2
                Select Case e.Button
                    Case Windows.Forms.MouseButtons.Left
                        Pick(e.X, e.Y)
                        FocusIn(CurrentPolygon)
                        ReGraph()
                        Return
                    Case Windows.Forms.MouseButtons.Right
                        ResetView()
                End Select
                Graph()
        End Select

        MyBase.DoMouseUp(e)
    End Sub

    Private Sub PropertyGrid_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles PropertyGrid.PropertyValueChanged
        If CurrentDistrictValue Is Nothing Then Return
        Dim d As Polygon = DistrictTopPolygonMap(CurrentDistrictValue)
        If d Is Nothing Then Return
        Dim c As ColorInt32 = GetColor(CurrentDistrictValue)
        If CurrentPolygonBorderOfDistrict = -1 Then
            CurrentPolygonColor = c
            d.Filler = CSelectionColor(c)
        Else
            d.Filler = c
        End If
        ReGraph()

        Changed = True
    End Sub
End Class
