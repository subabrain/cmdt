'==========================================================================
'
'  File:        Graphics.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: NDSystem动作设施
'  Created:     2005.02.09.16:39(GMT+8:00)
'  Version:     0.5 2007.09.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Drawing

Public Class NDWorld
    Inherits World

#Region " 声明 "

    '数据部分
    Public ColorBack As Integer = Color.LightGray.ToArgb  '背景色
    Public ColorFore As Integer = Color.Blue.ToArgb '前景色
    Public ColorAxis As Integer = Color.Gold.ToArgb '坐标轴颜色
    Public ColorFill As Integer = Color.White.ToArgb  '填涂颜色

    '选项
    Public ShowText As Boolean = True '是否显示调试信息
    Public ShowAxis As Boolean = True '是否显示坐标轴
    Public ShowError As Boolean = True '是否显示错误信息
    Public ShowObject As Boolean = True '是否显示物体

    '绘图参数
    Public ProgramName As String = "NDGrapher" '程序名和版本号
    Public FontName As String = "宋体" '字体名称
    Public FontSize As Byte = 9 '字高度
    Public DebugInfo As String
    Public Message As String

#End Region

#Region " 方法 "
    Public Sub New(ByVal World As World, Optional ByVal NameValue As String = Nothing)
        MyBase.New(NameValue)
        If Not World Is Nothing Then
            For n As Integer = 0 To World.UpperBound
                Add(World.Obj(n))
            Next
        End If
    End Sub
    Public Sub MessageAdd(ByVal s As String)
        Message &= Environment.NewLine & s
    End Sub
    Public Sub MessageRoll()
        Dim n As Integer = Message.IndexOf(Environment.NewLine)
        If n < 0 Then Return
        Message = Message.Substring(n + Environment.NewLine.Length)
    End Sub

    ''' <summary>选取函数，从空间中选取一个在指定选取直线上的实现了IPickable的物体</summary>
    Public Function GetBestFitPicking(ByVal x As Integer, ByVal y As Integer, ByVal ImageDistance As Double) As NDObject
        '在屏幕坐标系下选取直线为 N = (x, y, i) * t, t <- [1, +inf)
        '(x, y, i) ^ 2 != 0
        Dim PickingLine As Line = New Line(Nothing, New Vector(x, y, ImageDistance), 1, Double.PositiveInfinity, Color.Transparent)

        Dim Picking As NDObject = Nothing
        Dim MinT As Double = GetT(PickingLine, Picking)
        Return Picking
    End Function
#End Region

#Region " 摄像机镜头 "

    Public Sub CamTurnLeft(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(2, 0, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurnRight(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(0, 2, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurnUp(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(2, 1, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurnDown(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(1, 2, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurnClockwise(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(1, 0, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurnAnticlockwise(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(0, 1, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamTurn(ByVal i As Byte, ByVal j As Byte, ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim Temp As New HomotheticTransformation
        Temp.Rotate(i, j, Theta)
        HomotheticTransformation = Temp * HomotheticTransformation
    End Sub
    Public Sub CamRotateLeft(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(2, 0, Theta)
    End Sub
    Public Sub CamRotateRight(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(0, 2, Theta)
    End Sub
    Public Sub CamRotateUp(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(2, 1, Theta)
    End Sub
    Public Sub CamRotateDown(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(1, 2, Theta)
    End Sub
    Public Sub CamRotateClockwise(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(0, 1, Theta)
    End Sub
    Public Sub CamRotateAnticlockwise(ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(1, 0, Theta)
    End Sub
    Public Sub CamRotate(ByVal i As Byte, ByVal j As Byte, ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.Rotate(i, j, Theta)
    End Sub
    Public Sub CamOrbit(ByVal i As Byte, ByVal j As Byte, ByVal Theta As Double)
        Dim t As New HomotheticTransformation
        t.Rotate(i, j, Theta)
        HomotheticTransformation = HomotheticTransformation * t
    End Sub
    Public Sub CamTurnAgainstLand(ByVal i As Byte, ByVal j As Byte, ByVal Theta As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim RefPos As Vector = HomotheticTransformation.Invert * HomotheticTransformation.RefPos
        Dim t As New HomotheticTransformation
        t.Rotate(i, j, Theta)
        HomotheticTransformation = HomotheticTransformation * t
        HomotheticTransformation.RefPos = HomotheticTransformation * RefPos
    End Sub
    Public Sub CamMove(ByVal Displacement As Vector)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.RefPos -= Displacement
    End Sub
    Public Sub CamMoveOnLand(ByVal Displacement As Vector)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        Dim r As Vector = (Displacement * HomotheticTransformation.Column(2)) * HomotheticTransformation.Column(2)
        If Displacement * r >= 0 Then
            r = Displacement - r
        Else
            r = Displacement + r
        End If
        If Equal(r.Sqr, 0) Then Return
        r = r.Dir * Displacement.Norm
        HomotheticTransformation.RefPos -= r
    End Sub
    Public Sub CamElevateAgainstLand(ByVal MoveDistance As Double)
        If HomotheticTransformation Is Nothing Then HomotheticTransformation = New HomotheticTransformation
        HomotheticTransformation.RefPos -= MoveDistance * HomotheticTransformation.Column(2)
    End Sub
#End Region

#Region " 主绘图函数 "


    ''' <summary>清屏</summary>
    Public Sub GraphClear(ByVal Picture As IPic)
        Picture.Clear(ColorBack)
    End Sub

    ''' <summary>用于绘制所有物体</summary>
    Public Sub GraphObject(ByVal Picture As IPic)
        If Picture Is Nothing Then Return

        GraphClear(Picture)
        If ShowObject = True Then
            If ShowAxis = True Then
                Dim Dimension As Byte
                If Not (HomotheticTransformation Is Nothing) Then
                    Dimension = HomotheticTransformation.Dimension
                End If
                Dim TempDir As New Vector(Dimension)
                Dim TempBasicObjectArray As IPicObj()
                For n As Integer = 0 To Max(CInt(Dimension), 3) - 1
                    TempDir(n) = 1
                    Dim Line As New Line(Nothing, TempDir, Double.PositiveInfinity, ColorAxis)
                    Line.HomotheticTransformation = HomotheticTransformation
                    TempBasicObjectArray = Line.Complete()
                    If Not TempBasicObjectArray Is Nothing Then Picture.Draw(TempBasicObjectArray(0))
                    TempDir(n) = 0
                Next
            End If

            '绘图
            For Each PicObj As IPicObj In Me.Complete()
                Picture.Draw(PicObj)
            Next
        End If
    End Sub

    ''' <summary>用于绘制文字，应在GraphObject之后调用</summary>
    Public Sub GraphText(ByVal Picture As IPic)
        If Picture Is Nothing Then Return

        Dim PrintX As Integer = 80
        Dim PrintY As Integer = 80
        If Not (FontSize > 0) Then FontSize = 9
        Dim Font As Font = New System.Drawing.Font(FontName, FontSize)
        Dim PrintYSpeed As Integer = CInt(FontSize * 4 / 3)

        If ShowText Then
            '文字说明
            Picture.Draw2DString(ProgramName, Font, Color.Green.ToArgb, PrintX, PrintY)
            PrintY += PrintYSpeed
            Picture.Draw2DString("NDGrapher Engines by   R.C.", Font, Color.Green.ToArgb, PrintX, PrintY)
            Dim FontWidth As Integer = CInt(FontSize * 2 / 3)

            '标志绘图
            PrintX -= Picture.Width \ 2
            PrintY -= Picture.Height \ 2
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 2, PrintY + 2, PrintX + 3, PrintY + 2)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 3, PrintY + 2, PrintX + 3, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 2, PrintY + 9, PrintX + 4, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 4, PrintY + 4, PrintX + 4, PrintY + 5)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 5, PrintY + 6, PrintX + 5, PrintY + 7)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 6, PrintY + 2, PrintX + 6, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 5, PrintY + 2, PrintX + 11, PrintY + 2)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 9, PrintY + 2, PrintX + 9, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 8, PrintY + 9, PrintX + 11, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 12, PrintY + 3, PrintX + 12, PrintY + 8)

            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 2, PrintY + 1, PrintX + 21 * FontWidth + 9, PrintY + 1)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 1, PrintY + 2, PrintX + 21 * FontWidth + 1, PrintY + 10)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 2, PrintY + 3, PrintX + 21 * FontWidth + 8, PrintY + 3)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 3, PrintY + 5, PrintX + 21 * FontWidth + 3, PrintY + 9)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 4, PrintY + 10, PrintX + 21 * FontWidth + 8, PrintY + 10)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 5, PrintY + 5, PrintX + 21 * FontWidth + 7, PrintY + 5)
            Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 21 * FontWidth + 6, PrintY + 5, PrintX + 21 * FontWidth + 6, PrintY + 8)

            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 2, PrintY + 2, PrintX + 23 * FontWidth + 5, PrintY + 2)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 3, PrintY + 2, PrintX + 23 * FontWidth + 3, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 2, PrintY + 9, PrintX + 23 * FontWidth + 4, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 6, PrintY + 3, PrintX + 23 * FontWidth + 6, PrintY + 4)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 4, PrintY + 5, PrintX + 23 * FontWidth + 5, PrintY + 5)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 4, PrintY + 5, PrintX + 23 * FontWidth + 5, PrintY + 6)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 6, PrintY + 7, PrintX + 23 * FontWidth + 6, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 23 * FontWidth + 6, PrintY + 9, PrintX + 23 * FontWidth + 7, PrintY + 9)

            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 25 * FontWidth + 6, PrintY + 3, PrintX + 25 * FontWidth + 6, PrintY + 2)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 25 * FontWidth + 5, PrintY + 2, PrintX + 25 * FontWidth + 3, PrintY + 2)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 25 * FontWidth + 2, PrintY + 3, PrintX + 25 * FontWidth + 2, PrintY + 8)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 25 * FontWidth + 3, PrintY + 9, PrintX + 25 * FontWidth + 5, PrintY + 9)
            'Picture.Draw2DLine(Color.Green.ToArgb, 1, PrintX + 25 * FontWidth + 5, PrintY + 9, PrintX + 25 * FontWidth + 6, PrintY + 8)
            PrintX += Picture.Width \ 2
            PrintY += Picture.Height \ 2
            PrintY += 2 * PrintYSpeed

            '显示坐标与X、Y轴夹角和边的数据
            Dim Words As String
            If HomotheticTransformation Is Nothing Then
                Words = ""
            Else
                Words = HomotheticTransformation.ToString(True)
            End If
            Words &= Environment.NewLine
            Words &= DebugInfo
            Picture.Draw2DString(Words, Font, Color.Green.ToArgb, PrintX, PrintY)
            PrintX = 148
            PrintY = Picture.Height - 128
            Font = New System.Drawing.Font(FontName, FontSize * 1.5)
            Picture.Draw2DString(Message, Font, Color.Navy.ToArgb, PrintX, PrintY) 'Not Color.Green.ToArgb Or &HFF000000 反色
        End If
    End Sub

#End Region

End Class

#Region " 图片对象 "
''' <summary>图片接口</summary>
''' <remarks>
''' 接口要求使用X轴向右，Y轴向下，Z轴垂直纸面向内的右手坐标系
''' </remarks>
Public Interface IPic
    Inherits IDisposable
    '是否透视由继承类自行决定 但必须实现ImageDistanceValue
    Property ImageDistance() As Double '相距 眼睛到焦点(原点)的距离
    ReadOnly Property Width() As Integer '宽度
    ReadOnly Property Height() As Integer '高度
    Sub AttachToGrapher(ByVal Grapher As Grapher, ByRef UseImageBuffer As Boolean)
    Sub Clear(ByVal Color As ColorInt32)
    Sub Draw2DLine(ByVal Color As ColorInt32, ByVal PenWidth As Single, ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single)
    Sub Draw2DRegion(ByVal Color As ColorInt32, ByVal Points As Vector())
    Sub Draw(ByVal Obj As IPicObj)
    ''' <summary>绘制文字(用于显示注释)(坐标系为桌面坐标系)</summary>
    Sub Draw2DString(ByVal s As String, ByVal Font As Drawing.Font, ByVal Color As ColorInt32, ByVal x As Single, ByVal y As Single)
    Sub Graph()
End Interface
Public Enum PicObjType
    Line
    Region
    ImageQuadrangle
End Enum
#End Region

#Region " 选取接口 "
Public Interface IPickable
    Function Pickable() As Boolean
    Function GetT(ByVal PickingLine As Line) As Double
End Interface
Public Interface IPickableGroup
    Function GetT(ByVal PickingLine As Line, ByRef SubPicking As NDObject) As Double
End Interface
#End Region
