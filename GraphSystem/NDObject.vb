'==========================================================================
'
'  File:        NDObject.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: NDSystem基础物体
'  Created:     2007.08.05.03:11:56(GMT+8:00)
'  Version:     0.5 2013.01.15.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Text
Imports System.Diagnostics
Imports Microsoft.VisualBasic

#Region " 线 "
''' <summary>线</summary>
<DebuggerStepThrough()> Public Class Line
    Inherits NDObject
    Implements IPicObj
    Public Pos As Vector '点坐标
    Public Dir As Vector '方向向量
    Public LowerBound As Double '下段长度 从Pos到PosA
    Public UpperBound As Double '上段长度 从Pos到PosB
    Public Sub New()
    End Sub
    Public Sub New(ByVal Pos As Vector, ByVal Dir As Vector, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = +Pos
        Me.Dir = +Dir
        Me.LowerBound = LowerBound
        Me.UpperBound = UpperBound
    End Sub
    Public Sub New(ByVal Pos As Vector, ByVal Dir As Vector, ByVal Length As Double, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = +Pos
        Me.Dir = +Dir
        Me.LowerBound = -Length / 2
        Me.UpperBound = Length / 2
    End Sub
    Public Sub New(ByVal PosA As Vector, ByVal PosB As Vector, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = 0.5 * (PosA + PosB)
        Me.Dir = PosB - PosA
        Dim Length As Double = Dir.Norm
        Me.LowerBound = -Length / 2
        Me.UpperBound = Length / 2
        Me.Dir = Dir.Dir
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public ReadOnly Property PosA() As Vector
        Get
            Dim Vector As Vector = Pos + LowerBound * Dir
            For n As Integer = 0 To Dir.UpperBound
                If Dir(n) = 0 Then Vector(n) = Pos(n)
            Next
            Return Vector
        End Get
    End Property
    Public ReadOnly Property PosB() As Vector
        Get
            Dim Vector As Vector = Pos + UpperBound * Dir
            For n As Integer = 0 To Dir.UpperBound
                If Dir(n) = 0 Then Vector(n) = Pos(n)
            Next
            Return Vector
        End Get
    End Property
    Public Overrides Function Copy() As NDObject
        Dim CloneObject As Line
        CloneObject = MyBase.BaseCopy()
        CloneObject.Pos = +Pos
        CloneObject.Dir = +Dir
        Return CloneObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        If Not HomotheticTransformation Is Nothing Then Pos = HomotheticTransformation.CPos(Pos)
        Dir = HomotheticTransformation * Dir
        HomotheticTransformation = Nothing
        Return New IPicObj() {Me}
    End Function
    ''' <summary>格式化字符串</summary>
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.AppendLine(SingleLineFieldString(".Pos", Pos.ToString))
        ThisString.AppendLine(SingleLineFieldString(".Dir", Dir.ToString))
        ThisString.AppendLine(SingleLineFieldString(".LowerBound", LowerBound.ToString))
        ThisString.Append(SingleLineFieldString(".UpperBound", UpperBound.ToString))
        Return MultiLineFieldString("Line", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Pos = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".Pos", False, True))
        Dir = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".Dir", False, True))
        LowerBound = GetDoubleFromString(ThisString, ".LowerBound")
        UpperBound = GetDoubleFromString(ThisString, ".UpperBound")
    End Sub
End Class
#End Region

#Region " 区域 "
''' <summary>区域</summary>
<DebuggerStepThrough()> Public Class Region
    Inherits NDObject
    Implements IPicObj
    Public PosArray As Vector() '点坐标
    Public Sub New()
    End Sub
    Public Sub New(ByVal PosArray As Vector(), ByVal Color As Integer, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosArray = PosArray.Clone
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CloneObject As Region
        CloneObject = MyBase.BaseCopy()
        If PosArray IsNot Nothing Then
            Dim t As Vector() = PosArray.Clone
            For n As Integer = 0 To t.GetUpperBound(0)
                t(n) = +PosArray(n)
            Next
            CloneObject.PosArray = t
        End If
        Return CloneObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作区域函数

        If PosArray Is Nothing Then Return Nothing
        If PosArray.GetUpperBound(0) = 0 Then Return Nothing
        If PosArray.GetUpperBound(0) = 1 Then Return Nothing
        If Not HomotheticTransformation Is Nothing Then
            With HomotheticTransformation
                For n As Integer = 0 To PosArray.GetUpperBound(0)
                    PosArray(n) = .CPos(PosArray(n))
                Next
            End With
        End If
        Return New IPicObj() {Me}
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        Dim Temp As New StringBuilder
        If Not PosArray Is Nothing Then
            For n As Integer = 0 To PosArray.GetUpperBound(0) - 1
                Temp.AppendLine(SingleLineFieldString("", PosArray(n).ToString))
            Next
            Temp.Append(SingleLineFieldString("", PosArray(PosArray.GetUpperBound(0)).ToString))
        End If
        ThisString.Append(MultiLineFieldString(".PosArray", Temp.ToString))
        Return MultiLineFieldString("Region", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Dim Temp As String = GetMultiLineFieldFromString(ThisString, ".PosArray", False)
        If Temp = "" Then Return
        Dim TempStrings As String() = Temp.Replace(Environment.NewLine, ChrW(10)).Split(ChrW(10))
        Dim List As New List(Of Vector)
        For Each TempString As String In TempStrings
            If Not (TempString.Trim = "") Then List.Add(Vector.FromString(TempString))
        Next
        PosArray = List.ToArray
    End Sub
End Class
#End Region

#Region " 点 "
''' <summary>点</summary>
<DebuggerStepThrough()> Public Class Point
    Inherits NDObject

    Public Pos As Vector '点坐标
    Public Sub New()
    End Sub
    Public Sub New(ByVal Pos As Vector, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = +Pos
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As Point
        CopyObject = MyBase.BaseCopy()
        CopyObject.Pos = +Pos
        Return CopyObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        If Not HomotheticTransformation Is Nothing Then Pos = HomotheticTransformation.CPos(Pos)
        HomotheticTransformation = Nothing
        Return New IPicObj() {New Line(Pos, Nothing, 0, ColorInt)}
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.Append(SingleLineFieldString(".Pos", Pos.ToString))
        Return MultiLineFieldString("Point", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Pos = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".Pos", False, True))
    End Sub
End Class
#End Region

#Region " 圆锥曲线 "
''' <summary>圆锥曲线</summary>
Public Class Conic '<DebuggerStepThrough()>
    Inherits NDObject
    Public PosF As Vector '焦点坐标
    Public PosP As Vector '通径端点坐标
    Public PosQ As Vector '曲线通径与同侧曲线上的任一点(不包括通径两端点)坐标
    Public StartAng As Double = 0 '曲线起始弧度
    Public EndAng As Double = 2 * PI '曲线结束弧度
    Public EnableFill As Boolean '是否填涂 非封闭图形自动False
    Public Filler As ColorInt32 '填涂颜色
    Public Sub New()
    End Sub
    Public Sub New(ByVal PosF As Vector, ByVal PosP As Vector, ByVal PosQ As Vector, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosF = +PosF
        Me.PosP = +PosP
        Me.PosQ = +PosQ
    End Sub
    Public Sub New(ByVal PosF As Vector, ByVal PosP As Vector, ByVal PosQ As Vector, ByVal Color As ColorInt32, ByVal FillerColor As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosF = +PosF
        Me.PosP = +PosP
        Me.PosQ = +PosQ
        Me.EnableFill = True
        Me.Filler = FillerColor
    End Sub
    Public Sub New(ByVal PosF As Vector, ByVal PosP As Vector, ByVal PosQ As Vector, ByVal StartAng As Double, ByVal EndAng As Double, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosF = +PosF
        Me.PosP = +PosP
        Me.PosQ = +PosQ
        Me.StartAng = StartAng
        Me.EndAng = EndAng
    End Sub
    Public Sub New(ByVal PosF As Vector, ByVal PosP As Vector, ByVal PosQ As Vector, ByVal StartAng As Double, ByVal EndAng As Double, ByVal Color As ColorInt32, ByVal Filler As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosF = +PosF
        Me.PosP = +PosP
        Me.PosQ = +PosQ
        Me.StartAng = StartAng
        Me.EndAng = EndAng
        Me.EnableFill = True
        Me.Filler = Filler
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As Conic
        CopyObject = MyBase.BaseCopy()
        CopyObject.PosF = +PosF
        CopyObject.PosP = +PosP
        CopyObject.PosQ = +PosQ
        Return CopyObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作圆锥曲线函数，双曲线无左支
        'R=ep/(1-e*cos(theta)) 圆锥曲线统一方程  R半径 ep半通径(不用焦准距，为了绘圆)
        '对于椭圆 ep=A*(1-e^2)
        '对于抛物线 ep为y^2=2*P*x中P
        '对于双曲线 ep=A*(e^2-1)
        'PointF是圆锥曲线焦点
        'PointP是圆锥曲线焦点同侧半通径与曲线交点
        'PointQ是圆锥曲线上非通径端点的任一点

        If Not HomotheticTransformation Is Nothing Then PosF = HomotheticTransformation.CPos(PosF)
        If Not HomotheticTransformation Is Nothing Then PosP = HomotheticTransformation.CPos(PosP)
        If Not HomotheticTransformation Is Nothing Then PosQ = HomotheticTransformation.CPos(PosQ)
        HomotheticTransformation = Nothing
        Dim VectorP As Vector = PosP - PosF
        Dim ep As Double = VectorP.Norm 'ep=FP
        If ep = 0 Then Return Nothing
        Dim VectorQ As Vector = PosQ - PosF
        Dim q As Double = VectorQ.Norm 'q=FQ
        If q = 0 Then Return Nothing
        Dim d As Double = (PosP - PosQ).Norm 'd=PQ
        If d = 0 Then Return Nothing
        Dim cosphi As Double = (ep ^ 2 + q ^ 2 - d ^ 2) / (2 * ep * q) 'phi=AnglePFQ
        If Abs(cosphi) = 1 Then Return Nothing
        Dim e As Double = Abs(1 - ep / q) * (1 - cosphi ^ 2) ^ -0.5 '离心率 e=C/A

        VectorP = VectorP.Dir
        VectorQ = VectorQ.Dir
        If q > ep Then '调整使Q指向右边的theta=0位置
            VectorQ = (cosphi * VectorP + VectorQ) 'Q指向右边的theta=0位置时完整的VectorQ = ep / ((1 - e) * Sin(phi)) * (Cos(phi) * VectorP + VectorQ)
        Else
            VectorQ = (cosphi * VectorP - VectorQ) 'Q指向右边的theta=0位置时完整的VectorQ = ep / ((1 - e) * Sin(phi)) * (Cos(phi) * VectorP - VectorQ)
        End If
        VectorQ = VectorQ.Dir

        If StartAng = EndAng Then Return Nothing
        If Abs(StartAng - EndAng) >= 2 * PI Then
            StartAng = 0
            EndAng = 2 * PI
        End If

        Dim Steper As Double = (EndAng - StartAng) / ep / 128
        Dim theta As Double = StartAng
        Dim PosArray As New List(Of Vector)
        Dim r As Double
        Dim rDenominator As Double

        If StartAng > 0 Then
            PosArray.Add(PosF)
        End If

        For theta = StartAng To EndAng Step Steper
            rDenominator = 1 - e * Cos(theta)
            If rDenominator > 0 Then
                r = ep / rDenominator
                PosArray.Add(PosF + r * (VectorP * Sin(theta) + VectorQ * Cos(theta)))
            End If
        Next

        If EndAng < 2 * PI Then
            PosArray.Add(PosF)
        End If

        If PosArray.Count < 2 Then Return Nothing
        Dim Returner As New List(Of IPicObj)
        If e < 1 Then
            If EnableFill Then
                Returner.Add(New Region(PosArray.ToArray, Filler))
            End If
            For n As Integer = 0 To PosArray.Count - 1
                Returner.Add(New Line(PosArray(n), PosArray((n + 1) Mod PosArray.Count), ColorInt))
            Next
        Else
            For n As Integer = 0 To PosArray.Count - 2
                Returner.Add(New Line(PosArray(n), PosArray(n + 1), ColorInt))
            Next
        End If
        Return Returner.ToArray
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.AppendLine(SingleLineFieldString(".PosF", PosF.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosP", PosP.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosQ", PosQ.ToString))
        ThisString.AppendLine(SingleLineFieldString(".StartAng", StartAng.ToString))
        ThisString.AppendLine(SingleLineFieldString(".EndAng", EndAng.ToString))
        ThisString.AppendLine(SingleLineFieldString(".EnableFill", EnableFill.ToString))
        ThisString.Append(SingleLineFieldString(".Filler", Filler.ToString))
        Return MultiLineFieldString("Conic", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        PosF = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosF", False, True))
        PosP = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosP", False, True))
        PosQ = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosQ", False, True))
        StartAng = GetDoubleFromString(ThisString, ".StartAng")
        EndAng = GetDoubleFromString(ThisString, ".EndAng")
        EnableFill = GetBooleanFromString(ThisString, ".EnableFill")
        Filler = ColorInt32.FromString(GetSingleLineFieldFromString(ThisString, ".Filler"))
    End Sub
End Class
#End Region

#Region " 三角形 "
''' <summary>三角形</summary>
<DebuggerStepThrough()> Public Class Triangle
    Inherits NDObject
    Public PosA As Vector '点坐标
    Public PosB As Vector '点坐标
    Public PosC As Vector '点坐标
    Public EnableFill As Boolean '是否填涂
    Public Filler As ColorInt32 '填涂颜色
    Public Sub New()
    End Sub
    Public Sub New(ByVal PosA As Vector, ByVal PosB As Vector, ByVal PosC As Vector, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosA = +PosA
        Me.PosB = +PosB
        Me.PosC = +PosC
    End Sub
    Public Sub New(ByVal PosA As Vector, ByVal PosB As Vector, ByVal PosC As Vector, ByVal Color As ColorInt32, ByVal FillerColor As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosA = +PosA
        Me.PosB = +PosB
        Me.PosC = +PosC
        Me.EnableFill = True
        Me.Filler = FillerColor
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As Triangle
        CopyObject = MyBase.BaseCopy()
        CopyObject.PosA = +PosA
        CopyObject.PosB = +PosB
        CopyObject.PosC = +PosC
        Return CopyObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作三角形函数

        Dim NDBasicObjectList As New List(Of IPicObj)
        If Not HomotheticTransformation Is Nothing Then
            With HomotheticTransformation
                PosA = .CPos(PosA)
                PosB = .CPos(PosB)
                PosC = .CPos(PosC)
            End With
        End If

        If EnableFill Then
            '绘面
            NDBasicObjectList.Add(New Region(New Vector() {PosA, PosB, PosC}, Filler))
        End If

        '绘线
        NDBasicObjectList.Add(New Line(PosA, PosB, ColorInt))
        NDBasicObjectList.Add(New Line(PosB, PosC, ColorInt))
        NDBasicObjectList.Add(New Line(PosC, PosA, ColorInt))

        Return NDBasicObjectList.ToArray
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.AppendLine(SingleLineFieldString(".PosA", PosA.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosB", PosB.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosC", PosC.ToString))
        ThisString.AppendLine(SingleLineFieldString(".EnableFill", EnableFill.ToString))
        ThisString.Append(SingleLineFieldString(".Filler", Filler.ToString))
        Return MultiLineFieldString("Triangle", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        PosA = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosA", False, True))
        PosB = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosB", False, True))
        PosC = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosC", False, True))
        EnableFill = GetBooleanFromString(ThisString, ".EnableFill")
        Filler = ColorInt32.FromString(GetSingleLineFieldFromString(ThisString, ".Filler"))
    End Sub
End Class
#End Region

#Region " 多维矩 "
''' <summary>多维矩</summary>
<DebuggerStepThrough()> Public Class Rect
    Inherits NDObject
    Public Pos As Vector '点坐标
    Public Leg As Vector '棱长
    Public EnableFill As Boolean '是否填涂
    Public Filler As ColorInt32 '填涂颜色
    Public Sub New()
    End Sub
    Public Sub New(ByVal Pos As Vector, ByVal Leg As Vector, ByVal Color As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = +Pos
        Me.Leg = +Leg
    End Sub
    Public Sub New(ByVal Pos As Vector, ByVal Leg As Vector, ByVal Color As ColorInt32, ByVal Filler As ColorInt32, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.Pos = +Pos
        Me.Leg = +Leg
        Me.EnableFill = True
        Me.Filler = Filler
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As Rect
        CopyObject = MyBase.BaseCopy()
        CopyObject.Leg = +Leg
        Return CopyObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作矩函数
        'TODO 可以优化 现在点坐标重复计算

        Dim NDBasicObjectList As New List(Of IPicObj)
        Dim Dimension As Integer = Leg.Dimension

        If EnableFill Then
            '绘面
            Dim PosArray(3) As Vector '一个矩形的坐标
            Dim PosBool(Dimension - 1) As Boolean '确定点坐标的方向 0负 1正
            Dim PosVector As New Vector
            Dim Flag As Boolean '为真则先改变PosBool(IndexA) 为假则先改变PosBool(IndexB)
            For IndexB As Integer = 1 To Dimension - 1
                For IndexA As Integer = 0 To IndexB - 1
                    For n As Integer = 0 To 2 ^ (Dimension - 2) - 1
                        For k As Integer = 0 To Dimension - 1
                            PosVector(k) = (Abs(CInt(PosBool(k))) - 0.5) * Leg(k)
                        Next
                        PosArray(0) = Pos + PosVector
                        Flag = (IndexA + IndexB) Mod 2
                        For Each k As Boolean In PosBool
                            Flag = Flag Xor k
                        Next
                        If Flag Then
                            PosVector(IndexB) = -PosVector(IndexB)
                            PosArray(1) = Pos + PosVector
                            PosVector(IndexA) = -PosVector(IndexA)
                            PosArray(2) = Pos + PosVector
                            PosVector(IndexB) = -PosVector(IndexB)
                            PosArray(3) = Pos + PosVector
                        Else
                            PosVector(IndexA) = -PosVector(IndexA)
                            PosArray(1) = Pos + PosVector
                            PosVector(IndexB) = -PosVector(IndexB)
                            PosArray(2) = Pos + PosVector
                            PosVector(IndexA) = -PosVector(IndexA)
                            PosArray(3) = Pos + PosVector
                        End If
                        If Not HomotheticTransformation Is Nothing Then
                            For l As Integer = 0 To 3
                                PosArray(l) = HomotheticTransformation.CPos(PosArray(l))
                            Next
                        End If
                        NDBasicObjectList.Add(New Region(PosArray, Filler))
                        PosBool(IndexA) = True
                        PosBool(IndexB) = True
                        For m As Integer = 0 To Dimension - 1 '将PosBoolBool的某一位+1
                            If Not PosBool(m) Then
                                PosBool(m) = True
                                Exit For
                            Else
                                PosBool(m) = False
                            End If
                        Next
                        PosBool(IndexA) = False
                        PosBool(IndexB) = False
                    Next
                Next
                PosBool = New Boolean(Dimension - 1) {}
            Next

            '原来的无序绘面法
            'For n As Integer = 0 To 3
            '    PosArray(n) = New Vector(Dimension)
            'Next
            'For n As Integer = 0 To 2 ^ Dimension - 1
            '    For i As Integer = 0 To Dimension - 2
            '        If PosBool(i) = 0 Then
            '            For j As Integer = i + 1 To Dimension - 1
            '                If PosBool(j) = 0 Then
            '                    For k As Integer = 0 To Dimension - 1
            '                        PosArray(0)(k) = Pos(k) + (Abs(CInt(PosBool(k))) - 0.5) * Leg(k)
            '                        PosArray(1)(k) = Pos(k) + (Abs(CInt(PosBool(k) Xor (k = i))) - 0.5) * Leg(k)
            '                        PosArray(2)(k) = Pos(k) + (Abs(CInt(PosBool(k) Xor (k = i OrElse k = j))) - 0.5) * Leg(k)
            '                        PosArray(3)(k) = Pos(k) + (Abs(CInt(PosBool(k) Xor (k = j))) - 0.5) * Leg(k)
            '                    Next
            '                    If Not CoordSystem Is Nothing Then
            '                        For l As Integer = 0 To 3
            '                            PosArray(l) = CoordSystem.CPos(PosArray(l))
            '                        Next
            '                    End If
            '                    NDBasicObjectList.Add(New Region(PosArray, Filler))
            '                End If
            '            Next
            '        End If
            '    Next
            '    For m As Integer = 0 To Dimension - 1 '将PosBoolBool的某一位+1
            '        If PosBool(m) = 0 Then
            '            PosBool(m) = 1
            '            Exit For
            '        Else
            '            PosBool(m) = 0
            '        End If
            '    Next
            'Next
        End If

        '绘线
        Dim PosBoolA(Dimension - 1), PosBoolB(Dimension - 1) As Boolean '循环用的两个点的布尔值
        Dim PosA(Dimension - 1), PosB(Dimension - 1) As Double  '循环用的两个点的多维坐标

        For n As Integer = 0 To Dimension - 1
            PosBoolA(n) = False
        Next
        For n As Integer = 0 To 2 ^ Dimension - 1
            For m As Integer = 0 To Dimension - 1
                PosBoolB(m) = PosBoolA(m)
            Next
            For m As Integer = 0 To Dimension - 1
                If Not PosBoolB(m) Then '连结只有一个坐标不同的点
                    PosBoolB(m) = True
                    ReDim PosA(Dimension - 1), PosB(Dimension - 1)  '清空多维坐标
                    For k As Integer = 0 To Dimension - 1
                        PosA(k) = Pos(k) + (Abs(CInt(PosBoolA(k))) - 0.5) * Leg(k)
                        PosB(k) = Pos(k) + (Abs(CInt(PosBoolB(k))) - 0.5) * Leg(k)
                    Next
                    Dim CPosedA, CPosedB As Vector
                    If Not HomotheticTransformation Is Nothing Then
                        CPosedA = HomotheticTransformation.CPos(New Vector(PosA))
                        CPosedB = HomotheticTransformation.CPos(New Vector(PosB))
                    Else
                        CPosedA = New Vector(PosA)
                        CPosedB = New Vector(PosB)
                    End If
                    NDBasicObjectList.Add(New Line(CPosedA, CPosedB, ColorInt))
                    PosBoolB(m) = False
                End If
            Next
            For m As Integer = 0 To Dimension - 1 '将PosBoolBoolA的某一位+1
                If Not PosBoolA(m) Then
                    PosBoolA(m) = True
                    m = Dimension - 1
                Else
                    PosBoolA(m) = False
                End If
            Next
        Next

        Return NDBasicObjectList.ToArray
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.AppendLine(SingleLineFieldString(".Pos", Pos.ToString))
        ThisString.AppendLine(SingleLineFieldString(".Leg", Leg.ToString))
        ThisString.AppendLine(SingleLineFieldString(".EnableFill", EnableFill.ToString))
        ThisString.Append(SingleLineFieldString(".Filler", Filler.ToString))
        Return MultiLineFieldString("Rect", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Pos = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".Pos", False, True))
        Leg = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".Leg", False, True))
        EnableFill = GetBooleanFromString(ThisString, ".EnableFill")
        Filler = ColorInt32.FromString(GetSingleLineFieldFromString(ThisString, ".Filler"))
    End Sub
End Class
#End Region

#Region " 多边形 "
''' <summary>多边形</summary>
<DebuggerStepThrough()> Public Class Polygon
    Inherits NDObject
    Implements IPickable

    Public PosArray As Vector() '点坐标
    Public EnableFill As Boolean '是否填涂
    Public Filler As ColorInt32 '填涂颜色
    Public PickableValue As Boolean '能否选择
    Public Sub New()
    End Sub
    Public Sub New(ByVal PosArray As Vector(), ByVal Color As ColorInt32, ByVal EnableFill As Boolean, ByVal Filler As ColorInt32, Optional ByVal Pickable As Boolean = False, Optional ByVal Name As String = Nothing)
        MyBase.New(Color, Name)
        Me.PosArray = PosArray
        Me.EnableFill = EnableFill
        Me.Filler = Filler
        Me.PickableValue = Pickable
    End Sub
    Public Sub New(ByVal ThisString As String)
        UpdateFromString(ThisString)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CloneObject As Polygon
        CloneObject = MyBase.BaseCopy()
        If PosArray IsNot Nothing Then
            Dim t As Vector() = PosArray.Clone
            For n As Integer = 0 To t.GetUpperBound(0)
                t(n) = +PosArray(n)
            Next
            CloneObject.PosArray = t
        End If
        Return CloneObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作多边形函数

        Dim NDBasicObjectList As New List(Of IPicObj)
        If PosArray Is Nothing Then Return Nothing
        If Not HomotheticTransformation Is Nothing Then
            With HomotheticTransformation
                For n As Integer = 0 To PosArray.GetUpperBound(0)
                    PosArray(n) = .CPos(PosArray(n))
                Next
            End With
        End If
        If PosArray.GetUpperBound(0) = 0 Then
            Return New IPicObj() {New Line(PosArray(0), Nothing, 0, ColorInt)}
        End If
        If PosArray.GetUpperBound(0) = 1 Then
            Return New IPicObj() {New Line(PosArray(0), PosArray(1), ColorInt)}
        End If
        If EnableFill Then
            NDBasicObjectList.Add(New Region(PosArray, Filler))
        End If
        For n As Integer = 0 To PosArray.GetUpperBound(0)
            NDBasicObjectList.Add(New Line(PosArray(n), PosArray((n + 1) Mod PosArray.GetLength(0)), ColorInt))
        Next
        Return NDBasicObjectList.ToArray
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        Dim Temp As New StringBuilder
        If Not PosArray Is Nothing Then
            For n As Integer = 0 To PosArray.GetUpperBound(0) - 1
                Temp.AppendLine(SingleLineFieldString("", PosArray(n).ToString))
            Next
            Temp.Append(SingleLineFieldString("", PosArray(PosArray.GetUpperBound(0)).ToString))
        End If
        ThisString.AppendLine(MultiLineFieldString(".PosArray", Temp.ToString))
        ThisString.AppendLine(SingleLineFieldString(".EnableFill", EnableFill.ToString))
        ThisString.Append(SingleLineFieldString(".Filler", Filler.ToString))
        Return MultiLineFieldString("Polygon", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Dim TempStrings As String() = GetArrayFieldFromString(ThisString, ".PosArray", False, True)
        If TempStrings Is Nothing Then Return
        Dim List As New List(Of Vector)
        For Each TempString As String In TempStrings
            If Not (TempString.Trim = "") Then List.Add(Vector.FromString(TempString))
        Next
        PosArray = List.ToArray
        EnableFill = GetBooleanFromString(ThisString, ".EnableFill")
        Filler = ColorInt32.FromString(GetSingleLineFieldFromString(ThisString, ".Filler"))
    End Sub

    Public Function Pickable() As Boolean Implements IPickable.Pickable
        Return PickableValue
    End Function
    ''' <summary>
    ''' 得到平面与选取直线交点在直线的参数式中的t。
    ''' 直线方程 N = Pos + t * Dir, t (- [1, +inf)
    ''' 平面方程 N * Normal + D = 0
    ''' 仅对顶点在正面成逆时针顺序的凸多边形适用。
    ''' 没有交点返回+inf。
    ''' 法线和选取直线正方向相同则返回+inf。
    ''' </summary>
    Public Function GetT(ByVal PickingLine As Line) As Double Implements IPickable.GetT
        '如果物品不应被选取应直接退出
        If Not PickableValue Then Return Double.PositiveInfinity

        If PosArray Is Nothing OrElse PosArray.Length < 3 Then Return Double.PositiveInfinity
        Dim Normal As Vector = Vector.VecPro(True, PosArray(1) - PosArray(0), PosArray(2) - PosArray(0))
        Dim D As Double = -Normal * PosArray(0)
        '直线方程 N = Pos + t * Dir, t <- [1, +inf)
        '平面方程 N * Normal + D = 0
        '合并，可得 t = - (Normal * Pos + D) / (Normal * Dir)
        Dim Divisor As Double = Normal * PickingLine.Dir

        '法线和选取直线正方向相同则返回+inf。
        If Divisor >= 0 Then Return Double.PositiveInfinity

        Dim t As Double = -(Normal * PickingLine.Pos + D) / Divisor
        Dim P As Vector = PickingLine.Pos + t * PickingLine.Dir

        '如果不在每条边的箭头方向在正面的左侧，则返回+inf。
        For n As Integer = 0 To PosArray.Length - 2
            If Vector.VecPro(True, PosArray(n + 1) - PosArray(n), P - PosArray(n)) * Normal < 0 Then
                Return Double.PositiveInfinity
            End If
        Next
        If Vector.VecPro(True, PosArray(0) - PosArray(PosArray.Length - 1), P - PosArray(PosArray.Length - 1)) * Normal < 0 Then
            Return Double.PositiveInfinity
        End If

        Return t
    End Function
End Class
#End Region

#Region " 图片四边形 "
''' <summary>图片四边形</summary>
<DebuggerStepThrough()> Public Class ImageQuadrangle
    Inherits NDObject
    Implements IPicObj
    Public PosA As Vector '点坐标
    Public PosB As Vector
    Public PosC As Vector
    Public PosD As Vector
    Public Bitmap As Drawing.Bitmap
    Public Sub New()
    End Sub
    Public Sub New(ByVal PosAValue As Vector, ByVal PosBValue As Vector, ByVal PosCValue As Vector, ByVal PosDValue As Vector, ByVal BitmapValue As Drawing.Bitmap, Optional ByVal Name As String = Nothing)
        MyBase.New(Nothing, Name)
        PosA = PosAValue
        PosB = PosBValue
        PosC = PosCValue
        PosD = PosDValue
        Bitmap = BitmapValue
    End Sub
    Public Sub New(ByVal ThisString As String, ByVal BitmapValue As Drawing.Bitmap)
        UpdateFromString(ThisString)
        Bitmap = BitmapValue
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As ImageQuadrangle
        CopyObject = MyBase.BaseCopy()
        CopyObject.PosA = +PosA
        CopyObject.PosB = +PosB
        CopyObject.PosC = +PosC
        CopyObject.PosD = +PosD
        Return CopyObject
    End Function
    Public Overrides Function Complete() As IPicObj()
        '作图片四边形函数
        If Not HomotheticTransformation Is Nothing Then
            With HomotheticTransformation
                PosA = .CPos(PosA)
                PosB = .CPos(PosB)
                PosC = .CPos(PosC)
                PosD = .CPos(PosD)
            End With
        End If
        Return New IPicObj() {Me}
    End Function
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        ThisString.AppendLine(SingleLineFieldString(".PosA", PosA.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosB", PosB.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosC", PosC.ToString))
        ThisString.AppendLine(SingleLineFieldString(".PosD", PosC.ToString))
        Return MultiLineFieldString("ImageQuadrangle", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        PosA = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosA", False, True))
        PosB = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosB", False, True))
        PosC = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosC", False, True))
        PosD = Vector.FromString(GetMultiLineFieldFromString(ThisString, ".PosD", False, True))
    End Sub
End Class
#End Region

#Region " 世界 "
''' <summary>世界</summary>
<DebuggerStepThrough()> Public Class World
    Inherits NDObject
    Implements IPickableGroup

    Protected ObjectList As New List(Of NDObject) '物体列表
    Protected PickableList As New List(Of IPickable) '可选择物体列表
    Protected PickableGroupList As New List(Of IPickableGroup) '可选物体组列表
    Public Sub New()
    End Sub
    Public Sub New(ByVal Name As String)
        MyBase.New(Nothing, Name)
    End Sub
    Public Overrides Function Copy() As NDObject
        Dim CopyObject As World
        CopyObject = MyBase.BaseCopy()
        CopyObject.ObjectList = New List(Of NDObject)(ObjectList)
        CopyObject.PickableList = New List(Of IPickable)(PickableList)
        CopyObject.PickableGroupList = New List(Of IPickableGroup)(PickableGroupList)
        Return CopyObject
    End Function
    ''' <summary>将包含的所有物体并行化到只有一层的新的World实例中</summary>
    Protected Function Parallelize() As World()
        Dim ret As New List(Of World)
        Dim c As World = Nothing '自身去掉子World的部分
        For Each o As NDObject In ObjectList
            Dim ow As World = TryCast(o, World)
            If ow IsNot Nothing Then
                ow = ow.Clone
                'MB(MA(V))=(MB*MA)(V)  V As Vector, MA, MB As Transformation
                If HomotheticTransformation IsNot Nothing Then ow.HomotheticTransformation = HomotheticTransformation * ow.HomotheticTransformation
                Dim ReturnListTemp As World() = ow.Parallelize()
                For Each w As World In ReturnListTemp
                    ret.Add(w)
                Next
            Else
                If c Is Nothing Then
                    c = New World
                    c.HomotheticTransformation = HomotheticTransformation
                    ret.Add(c)
                End If
                c.Add(o)
            End If
        Next
        Return ret.ToArray
    End Function
    Protected Function Unbox() As NDObject()
        Dim Parallelized As World() = Me.Parallelize
        Dim ret As New List(Of NDObject)
        For Each w As World In Parallelized
            For Each o As NDObject In w.ObjectList
                '减少Clone开销，加快执行速度
                Dim Clone As NDObject = o.Clone
                If w.HomotheticTransformation IsNot Nothing Then Clone.HomotheticTransformation = w.HomotheticTransformation * Clone.HomotheticTransformation
                ret.Add(Clone)
            Next
        Next
        Return ret.ToArray
    End Function
    Public Overrides Function Complete() As IPicObj()
        If ObjectList Is Nothing Then Return Nothing
        Dim Final As NDObject() = Me.Unbox
        Dim ret As New List(Of IPicObj)
        Dim retTemp As IPicObj()
        For Each obj As NDObject In Final
            retTemp = obj.Copy.Complete()
            If Not retTemp Is Nothing Then
                For Each ReturnObj As IPicObj In retTemp
                    ret.Add(ReturnObj)
                Next
            End If
        Next
        Return ret.ToArray
    End Function
    Public Property Obj(ByVal Index As Integer) As NDObject
        Get
            Return ObjectList(Index)
        End Get
        Set(ByVal Value As NDObject)
            ObjectList(Index) = Value
        End Set
    End Property
    Public ReadOnly Property Obj() As List(Of NDObject)
        Get
            Return ObjectList
        End Get
    End Property
    Public Sub Add(ByVal NDObject As NDObject)
        ObjectList.Add(NDObject)
        If TypeOf NDObject Is IPickable AndAlso CType(NDObject, IPickable).Pickable Then
            PickableList.Add(NDObject)
        ElseIf TypeOf NDObject Is IPickableGroup Then
            PickableGroupList.Add(NDObject)
        End If
    End Sub
    Public Sub Clear()
        ObjectList.Clear()
        PickableList.Clear()
        PickableGroupList.Clear()
    End Sub
    Public ReadOnly Property UpperBound() As Integer
        Get
            Return ObjectList.Count - 1
        End Get
    End Property
    Public Overrides Function ToString() As String
        Dim ThisString As New StringBuilder
        ThisString.AppendLine(MyBase.ToString)
        Dim Temp As New StringBuilder
        If Not ObjectList Is Nothing Then
            For n As Integer = 0 To ObjectList.Count - 2
                Temp.AppendLine(SingleLineFieldString("", ObjectList(n).ToString))
            Next
            Temp.Append(SingleLineFieldString("", ObjectList(ObjectList.Count - 1).ToString))
        End If
        ThisString.Append(MultiLineFieldString(".Obj", Temp.ToString))
        Return MultiLineFieldString("World", ThisString.ToString)
    End Function
    Public Overrides Sub UpdateFromString(ByVal ThisString As String)
        MyBase.UpdateFromString(GetMultiLineFieldFromString(ThisString, "NDObject"))
        Dim TempStrings As String() = GetArrayFieldFromString(ThisString, ".Obj", False)
        If TempStrings Is Nothing Then Return

        For n As Integer = 0 To Min(ObjectList.Count - 1, TempStrings.GetUpperBound(0))
            If Not (TempStrings(n).Trim = "") Then ObjectList(n).UpdateFromString(TempStrings(n))
        Next
    End Sub

    Public Function GetT(ByVal PickingLine As Line, ByRef SubPicking As NDObject) As Double Implements IPickableGroup.GetT
        Dim ht As HomotheticTransformation
        If HomotheticTransformation Is Nothing Then
            ht = New HomotheticTransformation
        Else
            ht = HomotheticTransformation.Invert
        End If
        PickingLine = New Line(ht.CPos(PickingLine.Pos), ht * PickingLine.Dir, PickingLine.LowerBound, PickingLine.UpperBound, Drawing.Color.Transparent)

        Dim MinT As Double = Double.PositiveInfinity
        Dim Picking As NDObject = Nothing

        For Each obj As IPickable In PickableList
            Dim t As Double = obj.GetT(PickingLine)
            If t < MinT AndAlso t > 0 Then
                MinT = t
                Picking = obj
            End If
        Next
        For Each w As IPickableGroup In PickableGroupList
            Dim o As NDObject = Nothing
            Dim t As Double = w.GetT(PickingLine, o)
            If t < MinT AndAlso t > 0 Then
                MinT = t
                Picking = o
            End If
        Next

        SubPicking = Picking
        Return MinT
    End Function
End Class
#End Region
