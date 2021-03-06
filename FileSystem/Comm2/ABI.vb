'==========================================================================
'
'  File:        ABI.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: ABI文件类
'  Version:     2017.03.06.
'  Copyright(C) F.R.C.
'
'==========================================================================
'UNDONE

Option Strict Off
Imports System
Imports System.Math
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.TextEncoding.TextEncoding
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif

''' <summary>ABI文件类</summary>
Public Class ABI
    Public Const Identifier As String = "LDMB"
    Public Version As VersionSign
    Public Data2 As Int32
    Public Data3 As Int32
    Public ImageCount As Int32
    Public Image As ImageDB()
    Public Enum VersionSign As Integer
        v1010 = &H30313031 'Comm2
        v1011 = &H31313031 'Comm3
        v1040 = &H30343031 'Comm2
        v1050 = &H30353031 'Comm2
        v1060 = &H30363031 'Comm2
    End Enum

    Sub New(ByVal Path As String)
        Using s = New StreamEx(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            With Me
                For n As Integer = 0 To Identifier.Length - 1
                    If s.ReadByte() <> AscW(Identifier(n)) Then
                        Throw New InvalidDataException
                    End If
                Next
                .Version = s.ReadInt32
                Select Case .Version
                    Case VersionSign.v1010, VersionSign.v1011, VersionSign.v1040, VersionSign.v1050, VersionSign.v1060
                    Case Else
                        Throw New InvalidDataException
                End Select
                If .Version <> VersionSign.v1011 Then
                    .Data2 = s.ReadInt32
                    .Data3 = s.ReadInt32
                    .ImageCount = s.ReadInt32

                    .Image = New ImageDB(.ImageCount - 1) {}
                    For n As Integer = 0 To .ImageCount - 1
                        .Image(n) = New ImageDB(s, .Version)
                    Next
                Else
                    s.ReadInt16()
                    s.ReadInt16()
                    s.ReadInt16()
                    .ImageCount = s.ReadInt16()
                    s.ReadInt16()
                    s.ReadInt16()
                    Dim ImageAddress As Int32 = s.ReadInt32
                    Dim UnknownFileAddress As Int32 = s.ReadInt32
                    Dim UnknownInfoAddress As Int32 = s.ReadInt32
                    s.ReadInt32()
                    s.ReadInt32()

                    .Image = New ImageDB(.ImageCount - 1) {}
                    For n As Integer = 0 To .ImageCount - 1
                        .Image(n) = New ImageDB(s, .Version)
                    Next
                End If
            End With
        End Using
    End Sub
    Public Sub ExportToGif(ByVal Index As Integer, ByVal Directory As String)
        Dim i As ImageDB = Image(Index)
        Dim g As New GifImageBlock(i.Rectangle, i.Palette)

        Dim gf As New Gif(g)
        If i.Name = "" Then
            gf.WriteToFile(GetPath(Directory, "Untitled-" & Index & ".gif"))
        Else
            gf.WriteToFile(GetPath(Directory, i.Name & ".gif"))
        End If
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class ImageDB
        Public Data5 As Int32
        ReadOnly Property Width() As Int32
            Get
                If Rectangle Is Nothing Then Return 0
                Return Rectangle.GetLength(0)
            End Get
        End Property
        ReadOnly Property Height() As Int32
            Get
                If Rectangle Is Nothing Then Return 0
                Return Rectangle.GetLength(1)
            End Get
        End Property
        Public Name As String
        Public Palette As Int32()
        Public Rectangle As Byte(,)

        'Protected ReadOnly Property LineWidth() As Integer
        '    Get
        '        If Width Mod 4 = 0 Then
        '            Return Width
        '        Else
        '            Return ((Width >> 2) + 1) << 2
        '        End If
        '    End Get
        'End Property

        Public Sub New(ByVal Name As String)
            If Name.Length > 32 Then Throw New ArgumentException
            Me.Name = Name
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal v As ABI.VersionSign)
            If v <> ABI.VersionSign.v1011 Then
                Data5 = s.ReadInt32
                Dim Width As Int32 = s.ReadInt32
                Dim Height As Int32 = s.ReadInt32
                If Width < 0 OrElse Height < 0 Then Throw New ArgumentOutOfRangeException
                Rectangle = New Byte(Width - 1, Height - 1) {}

                If v <> ABI.VersionSign.v1010 Then Name = s.ReadString(32, Windows1252)

                Palette = New Int32(255) {}
                Dim col As Int32
                For i As Integer = 0 To 255
                    col = s.ReadByte
                    col = (col << 8) Or s.ReadByte
                    col = (col << 8) Or s.ReadByte
                    Palette(i) = col
                Next

                For y As Integer = 0 To Height - 1
                    For x As Integer = 0 To Width - 1
                        Rectangle(x, y) = s.ReadByte
                    Next
                Next
            Else
                Dim Width As Int16 = s.ReadInt16
                Dim Height As Int16 = s.ReadInt16
                If Width < 0 OrElse Height < 0 Then Throw New InvalidDataException
                Rectangle = New Byte(Width - 1, Height - 1) {}

                Palette = New Int32(255) {}
                Dim col As Int32
                For i As Integer = 0 To 255
                    col = s.ReadByte
                    col = (col << 8) Or s.ReadByte
                    col = (col << 8) Or s.ReadByte
                    Palette(i) = col
                Next

                Name = s.ReadString(32, Windows1252)

                Dim Unknown As Int32 = s.ReadInt32
                Dim Address As Int32 = s.ReadInt32

                Dim Position As Int32 = s.Position '备份当前位置
                s.Position = Address
                For y As Integer = 0 To Height - 1
                    For x As Integer = 0 To Width - 1
                        Rectangle(x, y) = s.ReadByte
                    Next
                Next
                s.Position = Position '恢复位置
            End If
        End Sub
        Public Sub WriteToFile(ByVal s As StreamEx, ByVal v As ABI.VersionSign)
            s.WriteInt32(Data5)
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            s.WriteString(Name, 32, Windows1252)

            Dim col As Int32
            For n As Integer = 0 To 255
                col = Palette(n)
                s.WriteByte(CByte((col >> 16) And &HFF))
                s.WriteByte(CByte((col >> 8) And &HFF))
                s.WriteByte(CByte(col And &HFF))
            Next

            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    s.WriteByte(Rectangle(x, y))
                Next
            Next
        End Sub
    End Class
End Class
