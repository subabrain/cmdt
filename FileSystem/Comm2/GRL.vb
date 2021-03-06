'==========================================================================
'
'  File:        GRL.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: GRL文件类
'  Version:     2016.07.28.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif
Imports Firefly.Setting

''' <summary>GRL文件类</summary>
''' <remarks>
''' 只支持8位色深。
''' 在转换成Gif时会做出一个妥协：将调色板中用得最少的颜色量化到其他颜色，为透明色提供空间
''' </remarks>
Public Class GRL
    Public Const Identifier As String = "GFRL"
    Public VersionSign As Version
    Public Enum Version As Int32
        Comm2 = &H64
        Comm3 = &H65
    End Enum
    ReadOnly Property ImageCount() As Integer
        Get
            If Image Is Nothing Then Return 0
            Return Image.GetLength(0)
        End Get
    End Property
    ReadOnly Property PaletteCount() As Integer
        Get
            If Palette Is Nothing Then Return 0
            Return Palette.GetLength(0)
        End Get
    End Property
    Protected ImageInfoLength As Integer = 64
    Protected PaletteInfoLength As Integer = 44
    Public Image As ImageDB()
    Public Palette As PaletteDB()

    Public Sub New()
    End Sub

    Public Sub New(ByVal Path As String)
        With Me
            Using s = New StreamEx(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
                For n As Integer = 0 To Identifier.Length - 1
                    If s.ReadByte() <> AscW(Identifier(n)) Then
                        Throw New InvalidDataException
                    End If
                Next
                .VersionSign = CType(s.ReadInt32, Version)
                If .VersionSign <> Version.Comm2 AndAlso .VersionSign <> Version.Comm3 Then
                    Throw New InvalidDataException
                End If
                Dim ImageCount As Integer = s.ReadInt32()
                Dim PaletteCount As Integer = s.ReadInt32()
                .ImageInfoLength = s.ReadInt32()
                .PaletteInfoLength = s.ReadInt32()

                .Image = New ImageDB(ImageCount - 1) {}
                .Palette = New PaletteDB(PaletteCount - 1) {}

                For n As Integer = 0 To ImageCount - 1
                    .Image(n) = New ImageDB(s, .VersionSign)
                Next
                For n As Integer = 0 To PaletteCount - 1
                    .Palette(n) = New PaletteDB(s)
                Next

                Dim OffsetO As Long = s.Position
                For n As Integer = 0 To ImageCount - 1
                    .Image(n).ReadImage(s, OffsetO, .Palette(.Image(n).PaletteIndex))
                Next
                For n As Integer = 0 To PaletteCount - 1
                    .Palette(n).ReadPalette(s, OffsetO)
                Next
                Dim Images As New Queue(Of ImageDB)
                For Each i As ImageDB In .Image
                    If Not i.IsNothing Then Images.Enqueue(i)
                Next
                .Image = Images.ToArray
            End Using
        End With
    End Sub

    Public Shared Function Create(ByVal DesPath As String, ByVal Directory As String) As GRL
        Dim gf As New GRL
        With gf
            Dim s As New INI(DesPath)
            Dim VersionSign As String = ""
            s.ReadValue("GRL", "Version", VersionSign)
            .VersionSign = CType(AscW(CChar(VersionSign)), GRL.Version)
            Dim ImageCount As Integer
            s.ReadValue("GRL", "ImageCount", ImageCount)
            If ImageCount < 0 Then Throw New InvalidDataException
            .Image = New ImageDB(ImageCount - 1) {}

            For n As Integer = 0 To ImageCount - 1
                Dim SectionName As String = "Pic(" & n & ")"
                Dim Name As String = Nothing
                s.ReadValue(SectionName, "Name", Name)
                .Image(n) = New ImageDB(Name)
                Dim i As ImageDB = .Image(n)
                s.ReadValue(SectionName, "Compression", i.Compression)
                s.ReadValue(SectionName, "CenterX", i.CenterX)
                s.ReadValue(SectionName, "CenterY", i.CenterY)
                If .VersionSign >= Version.Comm3 Then
                    Dim Unknown1 As String = Nothing
                    Dim Unknown2 As String = Nothing
                    Dim Unknown3 As String = Nothing
                    s.ReadValue(SectionName, "Unknown1", Unknown1)
                    s.ReadValue(SectionName, "Unknown2", Unknown2)
                    s.ReadValue(SectionName, "Unknown3", Unknown3)
                    i.Unknown1 = Integer.Parse(Unknown1, Globalization.NumberStyles.HexNumber)
                    i.Unknown2 = Integer.Parse(Unknown2, Globalization.NumberStyles.HexNumber)
                    i.Unknown3 = Integer.Parse(Unknown3, Globalization.NumberStyles.HexNumber)
                End If
            Next
            Directory = Directory.TrimEnd("\"c)
            Dim p As New List(Of PaletteDB)
            For n As Integer = 0 To ImageCount - 1
                Dim i As ImageDB = .Image(n)
                Dim g As New Gif(GetPath(Directory, i.Name & ".gif"))
                If g.Flame Is Nothing OrElse g.Flame(0) Is Nothing Then Throw New InvalidDataException
                If g.Palette IsNot Nothing Then
                    i.Palette = New PaletteDB(i.Name, g.Palette)
                ElseIf g.Flame(0).Palette IsNot Nothing Then
                    i.Palette = New PaletteDB(i.Name, g.Flame(0).Palette)
                Else
                    Throw New InvalidDataException
                End If
                p.Add(i.Palette)
                i.PaletteIndex = n
                Dim r As Byte(,) = g.Flame(0).Rectangle
                Dim r2 As Int32(,) = New Int32(r.GetUpperBound(0), r.GetUpperBound(1)) {}
                If g.Flame(0).TransparentColorFlag Then
                    Dim c As Byte
                    Dim TranIndex As Byte = g.Flame(0).TransparentColorIndex
                    For y As Integer = 0 To r.GetUpperBound(1)
                        For x As Integer = 0 To r.GetUpperBound(0)
                            c = r(x, y)
                            If c = TranIndex Then
                                r2(x, y) = -1
                            Else
                                r2(x, y) = c
                            End If
                        Next
                    Next
                Else
                    For y As Integer = 0 To r.GetUpperBound(1)
                        For x As Integer = 0 To r.GetUpperBound(0)
                            r2(x, y) = r(x, y)
                        Next
                    Next
                End If
                i.Rectangle = r2
            Next
            gf.Palette = p.ToArray
        End With
        Return gf
    End Function

    Public Sub ExportToGif(ByVal Index As Integer, ByVal Directory As String)
        Dim i As ImageDB = Image(Index)
        Dim g As GifImageBlock

        Dim Width As Integer = i.Width
        Dim Height As Integer = i.Height
        Dim Rectangle As Integer(,) = i.Rectangle
        Dim Palette As Integer() = i.Palette.Color
        Dim r As Byte(,) = New Byte(Width - 1, Height - 1) {}
        Dim Num As Integer() = New Integer(256) {}
        Dim t As Integer
        For y As Integer = 0 To Height - 1
            For x As Integer = 0 To Width - 1
                t = Rectangle(x, y)
                If t = -1 Then
                    Num(256) += 1
                Else
                    Num(t) += 1
                End If
            Next
        Next

        If Num(256) = 0 Then
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    r(x, y) = CByte(Rectangle(x, y))
                Next
            Next
            g = New GifImageBlock(r, Palette)
        Else
            Dim Min As Integer = &H7FFFFFFF
            Dim MinIndex As Byte
            For n As Integer = 0 To 255
                If Num(n) <= Min Then
                    Min = Num(n)
                    MinIndex = CByte(n)
                End If
            Next
            Dim TranIndex As Byte
            Dim d As Integer = &H7FFFFFFF
            Dim cd As Integer
            For n As Integer = 0 To MinIndex - 1
                cd = ColorDistance(Palette(MinIndex), Palette(n))
                If cd < d Then
                    d = cd
                    TranIndex = CByte(n)
                End If
            Next
            For n As Integer = MinIndex + 1 To 255
                cd = ColorDistance(Palette(MinIndex), Palette(n))
                If cd < d Then
                    d = cd
                    TranIndex = CByte(n)
                End If
            Next

            Dim c As Integer
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    c = Rectangle(x, y)
                    Select Case c
                        Case -1
                            r(x, y) = MinIndex
                        Case MinIndex
                            r(x, y) = TranIndex
                        Case Else
                            r(x, y) = CByte(c)
                    End Select
                Next
            Next
            g = New GifImageBlock(r, Palette)
            g.SetControl(100, MinIndex)
        End If

        Dim gf As New Gif(g)
        Directory = Directory.TrimEnd("\"c)
        gf.WriteToFile(GetPath(Directory, i.Name & ".gif"))
    End Sub

    Public Sub ExportDes(ByVal DesPath As String)
        Dim s As New INI(DesPath, False)
        s.WriteValue("GRL", "Version", ChrW(VersionSign))
        s.WriteValue("GRL", "ImageCount", CStr(ImageCount))

        For n As Integer = 0 To ImageCount - 1
            Dim SectionName As String = "Pic(" & n & ")"
            Dim i As ImageDB = Image(n)
            s.WriteValue(SectionName, "Name", i.Name)
            s.WriteValue(SectionName, "Compression", i.Compression.ToString("X8"))
            s.WriteValue(SectionName, "CenterX", CStr(i.CenterX))
            s.WriteValue(SectionName, "CenterY", CStr(i.CenterY))
            If VersionSign >= Version.Comm3 Then
                s.WriteValue(SectionName, "Unknown1", i.Unknown1.ToString("X8"))
                s.WriteValue(SectionName, "Unknown2", i.Unknown2.ToString("X8"))
                s.WriteValue(SectionName, "Unknown3", i.Unknown3.ToString("X8"))
            End If
        Next
        s.WriteToFile()
    End Sub

    Public Function ToGif() As Gif
        Dim GifWidth As Integer
        Dim GifHeight As Integer

        For Each i As ImageDB In Image
            If i.Width > GifWidth Then GifWidth = i.Width
            If i.Height > GifHeight Then GifHeight = i.Height
        Next

        Dim gc As New List(Of GifImageBlock)
        Dim g As GifImageBlock
        For Each i As ImageDB In Image
            Dim Width As Integer = i.Width
            Dim Height As Integer = i.Height
            Dim Rectangle As Integer(,) = i.Rectangle
            Dim Palette As Integer() = i.Palette.Color
            Dim r As Byte(,) = New Byte(Width - 1, Height - 1) {}
            Dim Num As Integer() = New Integer(256) {}
            Dim t As Integer
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    t = Rectangle(x, y)
                    If t = -1 Then
                        Num(256) += 1
                    Else
                        Num(t) += 1
                    End If
                Next
            Next

            If Num(256) = 0 Then
                For y As Integer = 0 To Height - 1
                    For x As Integer = 0 To Width - 1
                        r(x, y) = CByte(Rectangle(x, y))
                    Next
                Next
                g = New GifImageBlock(r, Palette)
            Else
                Dim Min As Integer = &H7FFFFFFF
                Dim MinIndex As Byte
                For n As Integer = 0 To 255
                    If Num(n) <= Min Then
                        Min = Num(n)
                        MinIndex = CByte(n)
                    End If
                Next
                Dim TranIndex As Byte
                Dim d As Integer = &H7FFFFFFF
                Dim cd As Integer
                For n As Integer = 0 To MinIndex - 1
                    cd = ColorDistance(Palette(MinIndex), Palette(n))
                    If cd < d Then
                        d = cd
                        TranIndex = CByte(n)
                    End If
                Next
                For n As Integer = MinIndex + 1 To 255
                    cd = ColorDistance(Palette(MinIndex), Palette(n))
                    If cd < d Then
                        d = cd
                        TranIndex = CByte(n)
                    End If
                Next

                Dim c As Integer
                For y As Integer = 0 To Height - 1
                    For x As Integer = 0 To Width - 1
                        c = Rectangle(x, y)
                        Select Case c
                            Case -1
                                r(x, y) = MinIndex
                            Case MinIndex
                                r(x, y) = TranIndex
                            Case Else
                                r(x, y) = CByte(c)
                        End Select
                    Next
                Next
                g = New GifImageBlock(r, Palette)
                g.SetControl(100, MinIndex)
            End If
            gc.Add(g)
        Next

        Return New Gif(CShort(GifWidth), CShort(GifHeight), 8, gc.ToArray)
    End Function
    Protected Shared Function ColorDistance(ByVal L As Int32, ByVal R As Int32) As Integer
        Dim dr As Integer = (L And &HFF0000 - R And &HFF0000) >> 16
        Dim dg As Integer = (L And &HFF00 - R And &HFF00) >> 8
        Dim db As Integer = L And &HFF - R And &HFF
        Return dr * dr + dg * dg + db * db
    End Function

    Public Sub WriteToFile(ByVal Path As String)
        Using s = New StreamEx(Path, FileMode.Create)
            For n As Integer = 0 To Identifier.Length - 1
                s.WriteByte(CByte(AscW(Identifier(n))))
            Next
            If VersionSign <> Version.Comm2 AndAlso VersionSign <> Version.Comm3 Then
                Throw New InvalidDataException
            End If
            If VersionSign = Version.Comm2 Then
                ImageInfoLength = 64
                PaletteInfoLength = 44
            Else
                ImageInfoLength = 76
                PaletteInfoLength = 44
            End If
            s.WriteInt32(VersionSign)
            s.WriteInt32(ImageCount)
            s.WriteInt32(PaletteCount)
            s.WriteInt32(ImageInfoLength)
            s.WriteInt32(PaletteInfoLength)

            Dim Address As Int32 = CInt(s.Position)
            s.Position += ImageInfoLength * ImageCount + PaletteInfoLength * PaletteCount

            Dim OffsetO As Long = s.Position
            For n As Integer = 0 To ImageCount - 1
                Image(n).WriteImage(s, OffsetO)
            Next
            For n As Integer = 0 To PaletteCount - 1
                Palette(n).WritePalette(s, OffsetO)
            Next

            s.Position = Address
            For n As Integer = 0 To ImageCount - 1
                Image(n).WriteToFile(s, VersionSign)
            Next
            For n As Integer = 0 To PaletteCount - 1
                Palette(n).WriteToFile(s)
            Next
        End Using
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class ImageDB
        Public Name As String
        Protected Offset As Int32
        Protected Length As Int32
        Public PaletteIndex As Int32
        Public Compression As Int32 = 2
        Public ReadOnly Property Width() As Int32
            Get
                If Rectangle Is Nothing Then Return 0
                Return Rectangle.GetLength(0)
            End Get
        End Property
        Public ReadOnly Property Height() As Int32
            Get
                If Rectangle Is Nothing Then Return 0
                Return Rectangle.GetLength(1)
            End Get
        End Property
        Public CenterX As Int32
        Public CenterY As Int32
        Public Unknown1 As Int32
        Public Unknown2 As Int32
        Public Unknown3 As Int32

        Public Palette As PaletteDB
        Public Rectangle As Int32(,)


        Public ReadOnly Property IsNothing() As Boolean
            Get
                Return (Length = 0)
            End Get
        End Property

        Public Sub New(ByVal Name As String)
            If Name.Length > 32 Then Throw New ArgumentException
            Me.Name = Name
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal v As GRL.Version)
            Name = s.ReadString(32, Windows1252)
            Offset = s.ReadInt32
            Length = s.ReadInt32
            PaletteIndex = s.ReadInt32
            Compression = s.ReadInt32
            Dim Width As Int32 = s.ReadInt32
            Dim Height As Int32 = s.ReadInt32
            Rectangle = New Int32(Width - 1, Height - 1) {}
            CenterX = s.ReadInt32
            CenterY = s.ReadInt32
            If v > GRL.Version.Comm2 Then
                Unknown1 = s.ReadInt32
                Unknown2 = s.ReadInt32
                Unknown3 = s.ReadInt32
            End If
        End Sub
        Public Sub ReadImage(ByVal s As StreamEx, ByVal OffsetO As Long, ByVal Palette As PaletteDB)
            Me.Palette = Palette
            If Length <= 0 Then Return
            s.Position = OffsetO + Me.Offset + 4

            Dim LineAddress As Int32() = New Int32(Height - 1) {}
            For n As Integer = 0 To Height - 1
                LineAddress(n) = s.ReadInt32()
            Next

            Dim Line As Byte()
            Dim OrgLine As Int32()
            Dim offset As Int32 = CInt(s.Position)
            Select Case Compression
                Case 2, 4
                    For y As Integer = 0 To Height - 2
                        Line = New Byte(LineAddress(y + 1) - LineAddress(y) - 1) {}
                        s.Position = offset + LineAddress(y)
                        For i As Integer = 0 To LineAddress(y + 1) - LineAddress(y) - 1
                            Line(i) = s.ReadByte
                        Next
                        OrgLine = DeRLE(Line)
                        If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                        For x As Integer = 0 To Width - 1
                            Rectangle(x, y) = OrgLine(x)
                        Next
                    Next
                    Dim LastLength As Integer = Length - LineAddress(Height - 1) - Height * 4 - 4
                    Line = New Byte(LastLength - 1) {}
                    s.Position = offset + LineAddress(Height - 1)
                    For i As Integer = 0 To LastLength - 1
                        Line(i) = s.ReadByte
                    Next
                    OrgLine = DeRLE(Line)
                    If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                    For x As Integer = 0 To Width - 1
                        Rectangle(x, Height - 1) = OrgLine(x)
                    Next
                Case &H142
                    For y As Integer = 0 To Height - 2
                        Line = New Byte(LineAddress(y + 1) - LineAddress(y) - 1) {}
                        s.Position = offset + LineAddress(y)
                        For i As Integer = 0 To LineAddress(y + 1) - LineAddress(y) - 1
                            Line(i) = s.ReadByte
                        Next
                        OrgLine = DeRLE2(Line)
                        If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                        For x As Integer = 0 To Width - 1
                            Rectangle(x, y) = OrgLine(x)
                        Next
                    Next
                    Dim LastLength As Integer = Length - LineAddress(Height - 1) - Height * 4 - 4
                    Line = New Byte(LastLength - 1) {}
                    s.Position = offset + LineAddress(Height - 1)
                    For i As Integer = 0 To LastLength - 1
                        Line(i) = s.ReadByte
                    Next
                    OrgLine = DeRLE3(Line)
                    If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                    For x As Integer = 0 To Width - 1
                        Rectangle(x, Height - 1) = OrgLine(x)
                    Next
                Case Else
                    For y As Integer = 0 To Height - 2
                        Line = New Byte(LineAddress(y + 1) - LineAddress(y) - 1) {}
                        s.Position = offset + LineAddress(y)
                        For i As Integer = 0 To LineAddress(y + 1) - LineAddress(y) - 1
                            Line(i) = s.ReadByte
                        Next
                        OrgLine = DeRLE3(Line)
                        If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                        For x As Integer = 0 To Width - 1
                            Rectangle(x, y) = OrgLine(x)
                        Next
                    Next
                    Dim LastLength As Integer = Length - LineAddress(Height - 1) - Height * 4 - 4
                    Line = New Byte(LastLength - 1) {}
                    s.Position = offset + LineAddress(Height - 1)
                    For i As Integer = 0 To LastLength - 1
                        Line(i) = s.ReadByte
                    Next
                    OrgLine = DeRLE3(Line)
                    If OrgLine.GetLength(0) <> Width Then Throw New InvalidDataException
                    For x As Integer = 0 To Width - 1
                        Rectangle(x, Height - 1) = OrgLine(x)
                    Next
            End Select
        End Sub
        Public Sub WriteImage(ByVal s As StreamEx, ByVal OffsetO As Long)
            Dim LineAddress As Int32() = New Int32(Height - 1) {}

            Dim HeadAddress As Integer = CInt(s.Position)
            Offset = CInt(s.Position - OffsetO)
            s.Position += 4 + Height * 4

            Dim Line As Byte()
            Dim OrgLine As Int32()
            Dim Address As Int32 = 0
            Select Case Compression
                Case 2, 4
                    For y As Integer = 0 To Height - 1
                        OrgLine = New Int32(Width - 1) {}
                        For x As Integer = 0 To Width - 1
                            OrgLine(x) = Rectangle(x, y)
                        Next
                        Line = EnRLE(OrgLine)
                        LineAddress(y) = Address
                        Address += Line.GetLength(0)
                        s.Write(Line, 0, Line.GetLength(0))
                    Next
                Case &H142
                    For y As Integer = 0 To Height - 1
                        OrgLine = New Int32(Width - 1) {}
                        For x As Integer = 0 To Width - 1
                            OrgLine(x) = Rectangle(x, y)
                        Next
                        Line = EnRLE2(OrgLine)
                        LineAddress(y) = Address
                        Address += Line.GetLength(0)
                        s.Write(Line, 0, Line.GetLength(0))
                    Next
                Case Else
                    For y As Integer = 0 To Height - 1
                        OrgLine = New Int32(Width - 1) {}
                        For x As Integer = 0 To Width - 1
                            OrgLine(x) = Rectangle(x, y)
                        Next
                        Line = EnRLE3(OrgLine)
                        LineAddress(y) = Address
                        Address += Line.GetLength(0)
                        s.Write(Line, 0, Line.GetLength(0))
                    Next
            End Select
            Length = CInt(s.Position - HeadAddress)

            s.Position = HeadAddress
            s.WriteInt32(Length - 4 - Height * 4)
            For n As Integer = 0 To Height - 1
                s.WriteInt32(LineAddress(n))
            Next
            s.Position = HeadAddress + Length
        End Sub
        Public Sub WriteToFile(ByVal s As StreamEx, ByVal v As GRL.Version)
            s.WriteString(Name, 32, Windows1252)
            s.WriteInt32(Offset)
            s.WriteInt32(Length)
            s.WriteInt32(PaletteIndex)
            s.WriteInt32(Compression)
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            s.WriteInt32(CenterX)
            s.WriteInt32(CenterY)
            If v > GRL.Version.Comm2 Then
                s.WriteInt32(Unknown1)
                s.WriteInt32(Unknown2)
                s.WriteInt32(Unknown3)
            End If
        End Sub

        Protected Shared Function EnRLE(ByVal Code As Int32()) As Byte()
            Dim ret As New Queue(Of Byte)
            Dim temp As New Queue(Of Byte)
            Dim TranMode As Boolean = True
            Dim CodecNum As Integer
            Dim Num As Integer
            For Each c As Int32 In Code
                If c = -1 Then
                    If TranMode Then
                        If Num = 255 Then
                            While CodecNum Mod 4 <> 0
                                ret.Enqueue(0)
                                CodecNum += 1
                            End While
                            ret.Enqueue(&HFF)
                            CodecNum += 1
                            Num = 0
                        End If
                    Else
                        While CodecNum Mod 4 <> 2
                            ret.Enqueue(0)
                            CodecNum += 1
                        End While
                        ret.Enqueue(CByte(temp.Count))
                        For n As Integer = 0 To temp.Count - 1
                            ret.Enqueue(temp.Dequeue)
                        Next
                        CodecNum += 1
                        TranMode = True
                    End If
                    Num += 1
                Else
                    If TranMode Then
                        While CodecNum Mod 4 <> 0
                            ret.Enqueue(0)
                            CodecNum += 1
                        End While
                        ret.Enqueue(CByte(Num))
                        CodecNum += 1
                        Num = 0
                        TranMode = False
                    Else
                        If temp.Count = 255 Then
                            While CodecNum Mod 4 <> 2
                                ret.Enqueue(0)
                                CodecNum += 1
                            End While
                            ret.Enqueue(&HFF)
                            For n As Integer = 0 To 254
                                ret.Enqueue(temp.Dequeue)
                            Next
                            CodecNum += 1
                        End If
                    End If
                    temp.Enqueue(CByte(c))
                End If
            Next
            If TranMode Then
                If Num > 0 Then
                    While CodecNum Mod 4 <> 0
                        ret.Enqueue(0)
                        CodecNum += 1
                    End While
                    ret.Enqueue(CByte(Num))
                    CodecNum += 1
                    Num = 0
                End If
            Else
                If temp.Count > 0 Then
                    While CodecNum Mod 4 <> 2
                        ret.Enqueue(0)
                        CodecNum += 1
                    End While
                    ret.Enqueue(CByte(temp.Count))
                    CodecNum += 1
                    For n As Integer = 0 To temp.Count - 1
                        ret.Enqueue(temp.Dequeue)
                    Next
                End If
            End If
            Return ret.ToArray
        End Function
        Protected Shared Function EnRLE2(ByVal Code As Int32()) As Byte()
            Dim ret As New Queue(Of Byte)
            Dim temp As New Queue(Of Byte)
            Dim TranMode As Boolean = True
            Dim Num As Integer
            For Each c As Int32 In Code
                If c = -1 Then
                    If TranMode Then
                        If Num = 255 Then
                            ret.Enqueue(&HFF)
                            Num = 0
                        End If
                    Else
                        ret.Enqueue(CByte(temp.Count))
                        For n As Integer = 0 To temp.Count - 1
                            ret.Enqueue(temp.Dequeue)
                        Next
                        TranMode = True
                    End If
                    Num += 1
                Else
                    If TranMode Then
                        ret.Enqueue(CByte(Num))
                        Num = 0
                        TranMode = False
                    Else
                        If temp.Count = 255 Then
                            ret.Enqueue(&HFF)
                            For n As Integer = 0 To 254
                                ret.Enqueue(temp.Dequeue)
                            Next
                        End If
                    End If
                    temp.Enqueue(CByte(c))
                End If
            Next
            If TranMode Then
                If Num > 0 Then
                    ret.Enqueue(CByte(Num))
                    Num = 0
                End If
            Else
                If temp.Count > 0 Then
                    ret.Enqueue(CByte(temp.Count))
                    For n As Integer = 0 To temp.Count - 1
                        ret.Enqueue(temp.Dequeue)
                    Next
                End If
            End If
            Return ret.ToArray
        End Function
        Protected Shared Function EnRLE3(ByVal Code As Int32()) As Byte()
            Dim ret As New Queue(Of Byte)
            Dim temp As New Queue(Of Byte)
            Dim TranMode As Boolean = True
            Dim Num As Integer
            For Each c As Int32 In Code
                If c = -1 Then
                    If TranMode Then
                        If Num = 255 Then
                            ret.Enqueue(&HFF)
                            Num = 0
                        End If
                    Else
                        ret.Enqueue(CByte(temp.Count))
                        For n As Integer = 0 To temp.Count - 1
                            ret.Enqueue(temp.Dequeue)
                            ret.Enqueue(1)
                        Next
                        TranMode = True
                    End If
                    Num += 1
                Else
                    If TranMode Then
                        ret.Enqueue(CByte(Num))
                        Num = 0
                        TranMode = False
                    Else
                        If temp.Count = 255 Then
                            ret.Enqueue(&HFF)
                            For n As Integer = 0 To 254
                                ret.Enqueue(temp.Dequeue)
                                ret.Enqueue(1)
                            Next
                        End If
                    End If
                    temp.Enqueue(CByte(c))
                End If
            Next
            If TranMode Then
                If Num > 0 Then
                    ret.Enqueue(CByte(Num))
                    Num = 0
                End If
            Else
                If temp.Count > 0 Then
                    ret.Enqueue(CByte(temp.Count))
                    For n As Integer = 0 To temp.Count - 1
                        ret.Enqueue(temp.Dequeue)
                        ret.Enqueue(1)
                    Next
                End If
            End If
            Return ret.ToArray
        End Function
        Protected Shared Function DeRLE(ByVal Code As Byte()) As Int32()
            Dim ret As New Queue(Of Int32)
            Dim CodecNum As Integer = 0
            For n As Integer = 0 To Code.Length - 1
                If CodecNum Mod 4 = 0 Then
                    For i As Integer = 0 To Code(n) - 1
                        ret.Enqueue(-1)
                    Next
                Else
                    For i As Integer = n + 1 To n + 1 + Code(n) - 1
                        ret.Enqueue(Code(i))
                    Next
                    n += Code(n)
                End If
                CodecNum += 1
            Next
            Return ret.ToArray
        End Function
        Protected Shared Function DeRLE2(ByVal Code As Byte()) As Int32()
            Dim ret As New Queue(Of Int32)
            Dim IsTran As Boolean = True
            For n As Integer = 0 To Code.Length - 1
                If IsTran Then
                    For i As Integer = 0 To Code(n) - 1
                        ret.Enqueue(-1)
                    Next
                Else
                    For i As Integer = n + 1 To n + 1 + Code(n) - 1
                        ret.Enqueue(Code(i))
                    Next
                    n += Code(n)
                End If
                IsTran = Not IsTran
            Next
            Return ret.ToArray
        End Function
        Protected Shared Function DeRLE3(ByVal Code As Byte()) As Int32()
            Dim ret As New Queue(Of Int32)
            Dim IsTran As Boolean = True
            For n As Integer = 0 To Code.Length - 1
                If IsTran Then
                    For i As Integer = 0 To Code(n) - 1
                        ret.Enqueue(-1)
                    Next
                Else
                    For i As Integer = n + 1 To n + 1 + Code(n) * 2 - 1 Step 2
                        ret.Enqueue(Code(i))
                    Next
                    n += Code(n) * 2
                End If
                IsTran = Not IsTran
            Next
            Return ret.ToArray
        End Function
    End Class
    Public Class PaletteDB
        Public Name As String
        Protected Offset As Int32
        Public Const Length As Int32 = 768
        Public Const Count As Int32 = 256
        Public Color As Int32()

        Public Sub New(ByVal Name As String, ByVal Palette As Int32())
            If Name.Length > 32 Then Throw New ArgumentException
            Me.Name = Name
            If Palette Is Nothing Then Throw New ArgumentNullException
            If Palette.GetLength(0) <> 256 Then Throw New ArgumentException
            Color = Palette
        End Sub
        Public Sub New(ByVal s As StreamEx)
            Name = s.ReadString(32, Windows1252)
            Offset = s.ReadInt32
            s.Position += 8
        End Sub
        Public Sub ReadPalette(ByVal s As StreamEx, ByVal OffsetO As Long)
            s.Position = OffsetO + Offset
            Color = New Int32(Count - 1) {}
            Dim c As Int32
            For n As Integer = 0 To Count - 1
                c = s.ReadByte
                c = (c << 8) Or s.ReadByte
                c = (c << 8) Or s.ReadByte
                Color(n) = c
            Next
        End Sub
        Public Sub WritePalette(ByVal s As StreamEx, ByVal OffsetO As Long)
            Offset = CInt(s.Position - OffsetO)
            Dim c As Int32
            For n As Integer = 0 To Count - 1
                c = Color(n)
                s.WriteByte(CByte((c >> 16) And &HFF))
                s.WriteByte(CByte((c >> 8) And &HFF))
                s.WriteByte(CByte(c And &HFF))
            Next
        End Sub
        Public Sub WriteToFile(ByVal s As StreamEx)
            s.WriteString(Name, 32, Windows1252)
            s.WriteInt32(Offset)
            s.WriteInt32(Length)
            s.WriteInt32(Count)
        End Sub
    End Class
End Class
