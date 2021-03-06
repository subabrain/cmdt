'==========================================================================
'
'  File:        Y64.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: Y64文件类
'  Version:     2020.02.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing
Imports System.IO
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.Streaming
Imports Firefly.Imaging
Imports Firefly.Setting

''' <summary>Y64文件流类</summary>
''' <remarks>
''' 用于打开、修改和创建盟军2和盟军3的Y64文件
''' </remarks>
Public Class Y64
    Public Const IdentifyingSign As String = "FCDE" '识别头'FCDE

    Public VersionSign As Version
    Public Enum Version As Short
        Comm2Version1 = 1
        Comm2Version2 = 2
        Comm3Version3 = 3
        Comm3Version4 = 4
    End Enum

    Public UnknownInts As Int32()
    Public UnknownFloats As Single()

    Public Shared CommPaletteY As Byte() = {8, 24, 40, 56, 72, 88, 104, 120, 136, 152, 168, 184, 200, 216, 232, 248, 4, 12, 20, 28, 36, 44, 52, 60, 68, 76, 84, 92, 100, 108, 116, 124, 68, 76, 84, 92, 100, 108, 116, 124, 132, 140, 148, 156, 164, 172, 180, 188, 132, 140, 148, 156, 164, 172, 180, 188, 196, 204, 212, 220, 228, 236, 244, 252, 2, 6, 10, 14, 18, 22, 26, 30, 34, 38, 42, 46, 50, 54, 58, 62, 66, 70, 74, 78, 82, 86, 90, 94, 98, 102, 106, 110, 114, 118, 122, 126, 130, 134, 138, 142, 146, 150, 154, 158, 162, 166, 170, 174, 178, 182, 186, 190, 194, 198, 202, 206, 210, 214, 218, 222, 226, 230, 234, 238, 242, 246, 250, 254, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46, 33, 36, 39, 42, 45, 48, 51, 54, 57, 60, 63, 66, 69, 72, 75, 78, 65, 68, 71, 74, 77, 80, 83, 86, 89, 92, 95, 98, 101, 104, 107, 110, 97, 100, 103, 106, 109, 112, 115, 118, 121, 124, 127, 130, 133, 136, 139, 142, 129, 132, 135, 138, 141, 144, 147, 150, 153, 156, 159, 162, 165, 168, 171, 174, 161, 164, 167, 170, 173, 176, 179, 182, 185, 188, 191, 194, 197, 200, 203, 206, 193, 196, 199, 202, 205, 208, 211, 214, 217, 220, 223, 226, 229, 232, 235, 238, 225, 227, 229, 231, 233, 235, 237, 239, 241, 243, 245, 247, 249, 251, 253, 255}
    Public Shared Comm2PaletteCb As Byte() = {&H3B, &H3E, &H40, &H43, &H45, &H47, &H4A, &H4C, &H4E, &H51, &H53, &H55, &H58, &H5A, &H5C, &H5F, &H61, &H63, &H66, &H68, &H6A, &H6D, &H6F, &H71, &H74, &H76, &H78, &H7B, &H7D, &H7F, &H82, &H84}
    Public Shared Comm2PaletteCr As Byte() = {&H6A, &H6D, &H6F, &H72, &H74, &H77, &H79, &H7C, &H7E, &H81, &H83, &H86, &H88, &H8B, &H8D, &H90, &H92, &H94, &H97, &H99, &H9C, &H9E, &HA1, &HA3, &HA6, &HA8, &HAB, &HAD, &HB0, &HB2, &HB5, &HB7}

    Public PaletteY As Byte()
    Public PaletteCb As Byte()
    Public PaletteCr As Byte()

    ReadOnly Property NumView() As Int32
        Get
            Return PicArea.GetLength(0)
        End Get
    End Property
    ReadOnly Property NumPic() As Int32
        Get
            Return PicArea.GetLength(1)
        End Get
    End Property

    Public PicArea As PictureArea(,)

    Public Sub New(ByVal VersionSign As Version, ByVal CbDS As Byte(), ByVal CrDS As Byte(), ByVal PAs As PictureArea(,))
        Select Case VersionSign
            Case Version.Comm2Version1, Version.Comm2Version2, Version.Comm3Version3, Version.Comm3Version4
            Case Else
                Throw New InvalidDataException
        End Select
        If CbDS Is Nothing OrElse CbDS.GetUpperBound(0) <> 31 Then Throw New InvalidDataException
        If CrDS Is Nothing OrElse CrDS.GetUpperBound(0) <> 31 Then Throw New InvalidDataException
        If PAs Is Nothing Then Throw New InvalidDataException

        Me.VersionSign = VersionSign
        Me.PaletteY = CommPaletteY
        Me.PaletteCb = CbDS
        Me.PaletteCr = CrDS
        Me.PicArea = PAs

        Palette = New Int32((1 << 18) - 1) {}
        For n1 As Integer = 0 To 31
            For n2 As Integer = 0 To 31
                For n0 As Integer = 0 To 255
                    Palette((n1 << 13) Or (n2 << 8) Or n0) = YCbCr2RGB(PaletteY(n0), PaletteCb(n1), PaletteCr(n2))
                Next
            Next
        Next
    End Sub

    Public Shared Function Open(ByVal Path As String) As Y64
        Using s As New StreamEx(Path, FileMode.Open, FileAccess.Read)
            If s.ReadSimpleString(4) <> IdentifyingSign Then Throw New InvalidDataException
            If s.ReadInt16() <> 0 Then Throw New InvalidDataException
            Dim VersionSign = CType(s.ReadInt16, Version)
            Dim UnknownInts As Int32() = {}
            Dim UnknownFloats As Single() = {}
            If VersionSign = Version.Comm2Version1 OrElse VersionSign = Version.Comm2Version2 Then
                If s.ReadInt32() <> 1 Then Throw New InvalidDataException
                s.Position = &H30
            ElseIf VersionSign = Version.Comm3Version3 OrElse VersionSign = Version.Comm3Version4 Then
                If s.ReadInt32() <> 1 Then Throw New InvalidDataException
                If s.ReadByte() <> 0 Then Throw New InvalidDataException
                UnknownInts = Enumerable.Range(0, 8).Select(Function(i) s.ReadInt32()).ToArray()
                UnknownFloats = Enumerable.Range(0, 8).Select(Function(i) s.ReadFloat32()).ToArray()
                s.Position = &H71
            Else
                Throw New NotSupportedException
            End If

            Dim PaletteY = s.Read(256)
            Dim PaletteCb = s.Read(32)
            Dim PaletteCr = s.Read(32)
            Dim NumView = s.ReadInt32
            Dim NumPic = s.ReadInt32
            Dim PicAreaAddress = New Int32(NumView * NumPic - 1) {}
            For a As Integer = 0 To NumView - 1
                For b As Integer = 0 To NumPic - 1
                    PicAreaAddress(a * NumPic + b) = s.ReadInt32
                Next
            Next
            Dim PicAreaLength = GetDifference(PicAreaAddress, CInt(s.Length))
            Dim PicArea = New PictureArea(NumView - 1, NumPic - 1) {}
            For a As Integer = 0 To NumView - 1
                For b As Integer = 0 To NumPic - 1
                    s.Position = PicAreaAddress(a * NumPic + b)
                    PicArea(a, b) = New PictureArea(s, VersionSign, PicAreaLength(a * NumPic + b))
                Next
            Next
            Return New Y64(VersionSign, PaletteCb, PaletteCr, PicArea) With {.UnknownInts = UnknownInts, .UnknownFloats = UnknownFloats}
        End Using
    End Function

    Public Sub WriteTo(ByVal sp As ZeroLengthStreamPasser)
        Dim s = sp.GetStream

        Dim PicAreaAddress = New Int32(NumView - 1, NumPic - 1) {}

        Dim p As Integer
        If VersionSign = Version.Comm2Version1 OrElse VersionSign = Version.Comm2Version2 Then
            p = 376 + NumView * NumPic * 4
        Else
            p = 441 + NumView * NumPic * 4
        End If

        For x As Integer = 0 To NumView - 1
            For y As Integer = 0 To NumPic - 1
                PicAreaAddress(x, y) = p
                p += PicArea(x, y).Length(VersionSign)
            Next
        Next

        s.SetLength(p)

        s.WriteSimpleString(IdentifyingSign, 4)
        s.WriteInt16(0)
        s.WriteInt16(Me.VersionSign)
        If VersionSign = Y64.Version.Comm2Version1 OrElse VersionSign = Y64.Version.Comm2Version2 Then
            s.Write(New Byte() {1, 0, 0, 0, 78, 111, 109, 98, 114, 101, 32, 100, 101, 108, 32, 69, 115, 99, 101, 110, 97, 114, 105, 111, 32, 48, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 112, 1, 0, 0})
        Else
            s.Write(New Byte() {1, 0, 0, 0, 0})
            For Each i In UnknownInts
                s.WriteInt32(i)
            Next
            For Each f In UnknownFloats
                s.WriteFloat32(f)
            Next
            s.Write(New Byte() {78, 111, 109, 98, 114, 101, 32, 100, 101, 108, 32, 69, 115, 99, 101, 110, 97, 114, 105, 111, 32, 48, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 1, 0, 0})
        End If
        s.Write(PaletteY)
        s.Write(PaletteCb)
        s.Write(PaletteCr)
        s.WriteInt32(NumView)
        s.WriteInt32(NumPic)
        For x As Integer = 0 To NumView - 1
            For y As Integer = 0 To NumPic - 1
                s.WriteInt32(PicAreaAddress(x, y))
            Next
        Next
        For x As Integer = 0 To NumView - 1
            For y As Integer = 0 To NumPic - 1
                If s.Position <> PicAreaAddress(x, y) Then Throw New InvalidOperationException
                PicArea(x, y).WriteTo(s, VersionSign)
            Next
        Next
    End Sub

    Public Sub Export(ByVal MainTargetFilePath As String, ByVal ExtraTargetFilePath As String, ByVal IndexView As Integer, ByVal IndexPic As Integer)
        Dim yf As Y64 = Me
        If IndexView < 0 OrElse IndexView >= yf.NumView Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If
        If IndexPic < 0 OrElse IndexPic >= yf.NumPic Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If

        Dim PA As PictureArea = yf.PicArea(IndexView, IndexPic)
        Dim w As Integer = PA.NumPicBlockHorizontal
        Dim h As Integer = PA.NumPicBlockVertical

        Dim ab = New AbstractBitmap(Of Int32)(PA.Width, PA.Height)
        Dim DoBlock =
            Sub(i As Integer)
                Dim x3 = i Mod w
                Dim y3 = i \ w
                Dim Block = PA.PicBlock(PA.IndexTable(x3, y3))
                If Block Is Nothing Then Return
                ab.SetRectangle2(64 * x3, 64 * y3, Block.GetRectangle(yf))
            End Sub
        Enumerable.Range(0, w * h).AsParallel().ForAll(DoBlock)
        Using b As New Bmp(PA.Width, PA.Height, 24)
            b.SetRectangle(0, 0, ab.GetRectangle2(0, 0, PA.Width, PA.Height))
            Using s As New StreamEx(MainTargetFilePath, FileMode.Create, FileAccess.ReadWrite)
                b.SaveTo(s.AsNewWriting)
            End Using
        End Using

        If PA.NumberPicBlockExtra <> 0 Then
            Using be = New Bmp(PA.NumberPicBlockExtra * 64, 64, 24)
                For n As Integer = 0 To PA.NumberPicBlockExtra - 1
                    Dim Block = PA.PicBlock(PA.ExtraIndexTableI(n))
                    If Block Is Nothing Then Continue For
                    be.SetRectangle(64 * n, 0, Block.GetRectangle(yf))
                Next
                Using s As New StreamEx(ExtraTargetFilePath, FileMode.Create, FileAccess.ReadWrite)
                    be.SaveTo(s.AsNewWriting)
                End Using
            End Using
        End If
    End Sub
    Private Shared Function LoadPngRgb48(ByVal Path As String) As AbstractBitmap(Of UInt64)
        Dim Rect As AbstractBitmap(Of UInt64)
        Dim Bytes As Byte()
        Using s = Streams.OpenReadable(Path)
            Dim d As New PngBitmapDecoder(s.ToUnsafeStream(), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad)
            Dim i = DirectCast(d.Frames.First, BitmapSource)
            If i.Format <> PixelFormats.Rgb48 Then Throw New InvalidDataException(Path)
            Rect = New AbstractBitmap(Of UInt64)(i.PixelWidth, i.PixelHeight)
            Bytes = New Byte(i.PixelWidth * i.PixelHeight * 6 - 1) {}
            i.CopyPixels(Bytes, i.PixelWidth * 6, 0)
        End Using
        Using s As New ByteArrayStream(Bytes)
            For y = 0 To Rect.Height - 1
                For x = 0 To Rect.Width - 1
                    Dim b = s.ReadUInt16()
                    Dim g = s.ReadUInt16()
                    Dim r = s.ReadUInt16()
                    Rect(x, y) = &HFFFFUL.ConcatBits(r, 16).ConcatBits(g, 16).ConcatBits(b, 16)
                Next
            Next
            Return Rect
        End Using
    End Function
    Private Shared Function LoadPngGray16(ByVal Path As String) As AbstractBitmap(Of UInt16)
        Dim Rect As AbstractBitmap(Of UInt16)
        Dim Bytes As Byte()
        Using s = Streams.OpenReadable(Path)
            Dim d As New PngBitmapDecoder(s.ToUnsafeStream(), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad)
            Dim i = DirectCast(d.Frames.First, BitmapSource)
            If i.Format <> PixelFormats.Gray16 Then Throw New InvalidDataException(Path)
            Rect = New AbstractBitmap(Of UInt16)(i.PixelWidth, i.PixelHeight)
            Bytes = New Byte(i.PixelWidth * i.PixelHeight * 2 - 1) {}
            i.CopyPixels(Bytes, i.PixelWidth * 2, 0)
        End Using
        Using s As New ByteArrayStream(Bytes)
            For y = 0 To Rect.Height - 1
                For x = 0 To Rect.Width - 1
                    Rect(x, y) = s.ReadUInt16()
                Next
            Next
            Return Rect
        End Using
    End Function
    Private Shared Sub SavePngRgb48(ByVal Path As String, ByVal Rect As AbstractBitmap(Of UInt64))
        Dim Bytes = New Byte(Rect.Width * Rect.Height * 6 - 1) {}
        Using s As New ByteArrayStream(Bytes)
            For y = 0 To Rect.Height - 1
                For x = 0 To Rect.Width - 1
                    s.WriteUInt16(CUShort(Rect(x, y).Bits(47, 32)))
                    s.WriteUInt16(CUShort(Rect(x, y).Bits(31, 16)))
                    s.WriteUInt16(CUShort(Rect(x, y).Bits(15, 0)))
                Next
            Next
        End Using
        Dim i = BitmapSource.Create(Rect.Width, Rect.Height, 96, 96, PixelFormats.Rgb48, Nothing, Bytes, Rect.Width * 6)
        Dim e As New PngBitmapEncoder()
        e.Frames.Add(BitmapFrame.Create(i))
        Using s = Streams.CreateWritable(Path)
            e.Save(s.ToStream())
        End Using
    End Sub
    Private Shared Sub SavePngGray16(ByVal Path As String, ByVal Rect As AbstractBitmap(Of UInt16))
        Dim Bytes = New Byte(Rect.Width * Rect.Height * 2 - 1) {}
        Using s As New ByteArrayStream(Bytes)
            For y = 0 To Rect.Height - 1
                For x = 0 To Rect.Width - 1
                    s.WriteUInt16(Rect(x, y))
                Next
            Next
        End Using
        Dim i = BitmapSource.Create(Rect.Width, Rect.Height, 96, 96, PixelFormats.Gray16, Nothing, Bytes, Rect.Width * 2)
        Dim e As New PngBitmapEncoder()
        e.Frames.Add(BitmapFrame.Create(i))
        Using s As New StreamEx(Path, FileMode.Create, FileAccess.ReadWrite)
            e.Save(s)
        End Using
    End Sub
    Public Sub ExportMask(ByVal ZFilePath As String, ByVal UnknownFilePath As String, ByVal TransparentFilePath As String, ByVal IndexView As Integer, ByVal IndexPic As Integer)
        Dim yf As Y64 = Me
        If IndexView < 0 OrElse IndexView >= yf.NumView Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If
        If IndexPic < 0 OrElse IndexPic >= yf.NumPic Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If

        Dim PA As PictureArea = yf.PicArea(IndexView, IndexPic)
        Dim w As Integer = PA.NumPicBlockHorizontal
        Dim h As Integer = PA.NumPicBlockVertical

        Dim bZ = New AbstractBitmap(Of UInt16)(PA.Width, PA.Height)
        Dim bUnknown = New AbstractBitmap(Of UInt16)(PA.Width, PA.Height)
        Dim bTransparent = New AbstractBitmap(Of UInt16)(PA.Width, PA.Height)
        For y3 = 0 To h - 1
            For x3 = 0 To w - 1
                Dim Block = PA.PicBlock(PA.IndexTable(x3, y3))
                If Block Is Nothing Then Continue For
                Dim m = Block.GetMask()
                bZ.SetRectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height, Function(p) p.Z, m.Mask)
                bUnknown.SetRectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height, Function(p) p.N, m.Mask)
                bTransparent.Rectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height) = m.HalfTransparent
            Next
        Next
        SavePngGray16(ZFilePath, bZ)
        SavePngGray16(UnknownFilePath, bUnknown)
        SavePngGray16(TransparentFilePath, bTransparent)
    End Sub
    Public Sub ExportDes(ByVal DesFilePath As String)
        Dim yf As Y64 = Me
        Dim s As New Ini(DesFilePath, False)
        s.WriteValue("Y64", "Version", CStr(yf.VersionSign))
        If yf.VersionSign = Version.Comm3Version3 OrElse yf.VersionSign = Version.Comm3Version4 Then
            s.WriteValue("Y64", "UnknownInts", yf.UnknownInts.Select(Function(i) i.ToInvariantString()).ToArray())
            s.WriteValue("Y64", "UnknownFloats", yf.UnknownFloats.Select(Function(f) f.ToInvariantString()).ToArray())
        End If
        s.WriteValue("Y64", "NumberView", CStr(yf.NumView))
        s.WriteValue("Y64", "NumberPic", CStr(yf.NumPic))

        For x As Integer = 0 To yf.NumView - 1
            For y As Integer = 0 To yf.NumPic - 1
                Dim SectionName As String = "Pic(" & x & "," & y & ")"
                Dim PA As PictureArea = yf.PicArea(x, y)
                If PA.ZoomLayerNumber <> y Then s.WriteValue(SectionName, "ZoomLayerNumber", CStr(PA.ZoomLayerNumber))
                s.WriteValue(SectionName, "Width", CStr(PA.Width))
                s.WriteValue(SectionName, "Height", CStr(PA.Height))

                Dim IsDefaultIndexTable As Boolean = True
                Dim iw = PA.IndexTable.GetLength(0)
                Dim ih = PA.IndexTable.GetLength(1)
                For iy = 0 To ih - 1
                    For ix = 0 To iw - 1
                        If PA.IndexTable(ix, iy) <> ix + iy * iw Then
                            IsDefaultIndexTable = False
                            Exit For
                        End If
                    Next
                    If Not IsDefaultIndexTable Then Exit For
                Next
                If Not IsDefaultIndexTable OrElse PA.NumberPicBlockExtra <> 0 OrElse PA.NumberPicBlockReal <> iw * ih Then s.WriteValue(SectionName, "NumberPicBlockReal", CStr(PA.NumberPicBlockReal))
                If yf.VersionSign <> Version.Comm2Version1 AndAlso PA.NumberPicBlockExtra <> 0 Then s.WriteValue(SectionName, "NumberPicBlockExtra", CStr(PA.NumberPicBlockExtra))
                If Not IsDefaultIndexTable Then
                    s.WriteValue(SectionName, "IndexTable", ConvertShort(PA.IndexTable))
                End If
                If yf.VersionSign <> Version.Comm2Version1 Then
                    s.WriteValue(SectionName, "ExtraIndexTableF", ConvertInt(PA.ExtraIndexTableF))
                    s.WriteValue(SectionName, "ExtraIndexTableX", ConvertInt(PA.ExtraIndexTableX))
                    s.WriteValue(SectionName, "ExtraIndexTableY", ConvertInt(PA.ExtraIndexTableY))
                    s.WriteValue(SectionName, "ExtraIndexTableI", ConvertInt(PA.ExtraIndexTableI))
                End If
                If yf.VersionSign = Version.Comm3Version3 OrElse yf.VersionSign = Version.Comm3Version4 Then
                    'TODO: UnknownData
                End If
            Next
        Next
        s.WriteToFile()
    End Sub
    Protected Shared Function ConvertByte(ByVal s As Byte()) As String()
        If s Is Nothing Then Return Nothing
        Dim a As String() = New String(s.GetUpperBound(0)) {}
        For x As Integer = 0 To s.GetUpperBound(0)
            a(x) = s(x).ToString("X2")
        Next
        Return a
    End Function
    Protected Shared Function ConvertShort(ByVal s As UInt16(,)) As String(,)
        If s Is Nothing Then Return Nothing
        Dim a As String(,) = New String(s.GetUpperBound(0), s.GetUpperBound(1)) {}
        For y As Integer = 0 To s.GetUpperBound(1)
            For x As Integer = 0 To s.GetUpperBound(0)
                a(x, y) = s(x, y).ToString("X4")
            Next
        Next
        Return a
    End Function
    Protected Shared Function ConvertInt(ByVal s As Integer()) As String()
        If s Is Nothing Then Return Nothing
        Dim a As String() = New String(s.GetUpperBound(0)) {}
        For x As Integer = 0 To s.GetUpperBound(0)
            a(x) = s(x).ToString("X8")
        Next
        Return a
    End Function

    Public Sub Import(ByVal MainTargetFilePath As String, ByVal ExtraTargetFilePath As String, ByVal IndexView As Integer, ByVal IndexPic As Integer)
        Dim yf As Y64 = Me
        yf.GetReflectionTable()
        If IndexView < 0 OrElse IndexView >= yf.NumView Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If
        If IndexPic < 0 OrElse IndexPic >= yf.NumPic Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If

        Dim PA As PictureArea = yf.PicArea(IndexView, IndexPic)
        Dim w As Integer = PA.NumPicBlockHorizontal
        Dim h As Integer = PA.NumPicBlockVertical

        Dim ab As AbstractBitmap(Of Int32)
        Using b = Bmp.Open(MainTargetFilePath)
            If b.Width <> PA.Width OrElse b.Height <> PA.Height OrElse (b.BitsPerPixel <> 16 AndAlso b.BitsPerPixel <> 24 AndAlso b.BitsPerPixel <> 32) Then
                Throw New PicSizeDismatchException(PA.Width, PA.Height)
            End If
            ab = New AbstractBitmap(Of Int32)(b.Width, b.Height)
            ab.SetRectangle2(0, 0, b.GetRectangleAsARGB(0, 0, b.Width, b.Height))
        End Using
        Dim DoBlock =
            Sub(i As Integer)
                Dim x3 = i Mod w
                Dim y3 = i \ w
                Dim PicBlockIndex = PA.IndexTable(x3, y3)
                Dim Block = PA.PicBlock(PicBlockIndex)
                If Block Is Nothing Then Return
                Block.SetRectangle(ab.GetRectangle2(64 * x3, 64 * y3, 64, 64), yf)
                PA.PicBlock(PicBlockIndex) = Block
            End Sub
        Enumerable.Range(0, w * h).AsParallel().ForAll(DoBlock)

        If PA.NumberPicBlockExtra <> 0 Then
            Using be = Bmp.Open(ExtraTargetFilePath)
                If be.Width <> (PA.NumberPicBlockExtra * 64) Or be.Height <> 64 Or (be.BitsPerPixel <> 16 AndAlso be.BitsPerPixel <> 24) Then
                    Throw New PicSizeDismatchException((PA.NumberPicBlockExtra * 64), 64)
                End If
                For n As Integer = 0 To PA.NumberPicBlockExtra - 1
                    Dim PicBlockIndex = PA.ExtraIndexTableI(n)
                    Dim Block = PA.PicBlock(PicBlockIndex)
                    If Block Is Nothing Then Continue For
                    Block.SetRectangle(be.GetRectangleAsARGB(64 * n, 0, 64, 64), yf)
                    PA.PicBlock(PicBlockIndex) = Block
                Next
            End Using
        End If
    End Sub
    Public Sub ImportMask(ByVal ZFilePath As String, ByVal UnknownFilePath As String, ByVal TransparentFilePath As String, ByVal IndexView As Integer, ByVal IndexPic As Integer)
        Dim yf As Y64 = Me
        If IndexView < 0 OrElse IndexView >= yf.NumView Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If
        If IndexPic < 0 OrElse IndexPic >= yf.NumPic Then
            Throw New PicOutOfBoundException(yf.NumView, yf.NumPic)
        End If

        Dim PA As PictureArea = yf.PicArea(IndexView, IndexPic)
        Dim w As Integer = PA.NumPicBlockHorizontal
        Dim h As Integer = PA.NumPicBlockVertical

        Dim bZ = LoadPngGray16(ZFilePath)
        Dim bUnknown = LoadPngGray16(UnknownFilePath)
        Dim bTransparent = LoadPngGray16(TransparentFilePath)
        For y3 = 0 To h - 1
            For x3 = 0 To w - 1
                Dim Block = PA.PicBlock(PA.IndexTable(x3, y3))
                If Block Is Nothing Then Continue For
                Dim m As New PictureBlockMask
                Dim MaskZ = bZ.GetRectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height, Function(z) z)
                Dim MaskN = bUnknown.GetRectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height, Function(n) n)
                m.Mask = MaskZ.Zip(MaskN, Function(z, n) New PictureBlockMaskPixel With {.Z = z, .N = n}).ToArray()
                m.HalfTransparent = bTransparent.Rectangle(PictureBlock.Width * x3, PictureBlock.Height * y3, PictureBlock.Width, PictureBlock.Height)
                Block.SetMask(m)
            Next
        Next
    End Sub
    Public Shared Function Create(ByVal Directory As String) As Y64
        Dim DesFilePath = GetPath(Directory, "Description.ini")
        Dim s As New Ini(DesFilePath)
        Dim VersionInt As Integer
        s.ReadValue("Y64", "Version", VersionInt)
        Dim VersionSign As Version = CType(VersionInt, Version)
        Select Case VersionSign
            Case Version.Comm2Version1, Version.Comm2Version2, Version.Comm3Version3, Version.Comm3Version4
            Case Else
                Throw New VersionDismatchException
        End Select

        Dim UnknownIntsS As String() = New String() {}
        Dim UnknownFloatsS As String() = New String() {}
        Dim NumberView As Int32
        Dim NumberPic As Int32

        If VersionSign = Version.Comm3Version3 OrElse VersionSign = Version.Comm3Version4 Then
            s.ReadValue("Y64", "UnknownInts", UnknownIntsS)
            s.ReadValue("Y64", "UnknownFloats", UnknownFloatsS)
        End If
        s.ReadValue("Y64", "NumberView", NumberView)
        s.ReadValue("Y64", "NumberPic", NumberPic)

        Dim UnknownInts = UnknownIntsS.Select(Function(str) NumericStrings.InvariantParseInt32(str)).ToArray()
        Dim UnknownFloats = UnknownFloatsS.Select(Function(str) NumericStrings.InvariantParseFloat32(str)).ToArray()

        Dim PAs As PictureArea(,) = New PictureArea(NumberView - 1, NumberPic - 1) {}

        Dim PAName As String
        Dim ZoomLayerNumber As Int32, Width As Int32, Height As Int32, NumberPicBlockReal As Int32, NumberPicBlockExtra As Int32
        Dim IndexTable As UInt16(,), ExtraIndexTableF As Int32(), ExtraIndexTableX As Int32(), ExtraIndexTableY As Int32(), ExtraIndexTableI As Int32()
        Dim IndexTableS As String(,), ExtraIndexTableFS As String(), ExtraIndexTableXS As String(), ExtraIndexTableYS As String(), ExtraIndexTableIS As String()
        For y As Integer = 0 To NumberPic - 1
            For x As Integer = 0 To NumberView - 1
                PAName = "Pic(" & x & "," & y & ")"
                If Not s.ReadValue(PAName, "ZoomLayerNumber", ZoomLayerNumber) Then ZoomLayerNumber = y
                s.ReadValue(PAName, "Width", Width)
                s.ReadValue(PAName, "Height", Height)

                Dim iw = (Width + 63) \ 64
                Dim ih = (Height + 63) \ 64

                If Not s.ReadValue(PAName, "NumberPicBlockReal", NumberPicBlockReal) Then NumberPicBlockReal = iw * ih
                If VersionSign <> Version.Comm2Version1 Then
                    If Not s.ReadValue(PAName, "NumberPicBlockExtra", NumberPicBlockExtra) Then NumberPicBlockExtra = 0
                End If

                IndexTableS = New String(iw - 1, ih - 1) {}
                If Not s.ReadValue(PAName, "IndexTable", IndexTableS) Then
                    IndexTable = New UInt16(iw - 1, ih - 1) {}
                    For iy = 0 To ih - 1
                        For ix = 0 To iw - 1
                            IndexTable(ix, iy) = CUShort(ix + iy * iw)
                        Next
                    Next
                Else
                    IndexTable = ConvertInt(IndexTableS)
                End If

                If VersionSign <> Version.Comm2Version1 AndAlso NumberPicBlockExtra <> 0 Then
                    ExtraIndexTableFS = New String(NumberPicBlockExtra - 1) {}
                    ExtraIndexTableXS = New String(NumberPicBlockExtra - 1) {}
                    ExtraIndexTableYS = New String(NumberPicBlockExtra - 1) {}
                    ExtraIndexTableIS = New String(NumberPicBlockExtra - 1) {}
                    s.ReadValue(PAName, "ExtraIndexTableF", ExtraIndexTableFS)
                    s.ReadValue(PAName, "ExtraIndexTableX", ExtraIndexTableXS)
                    s.ReadValue(PAName, "ExtraIndexTableY", ExtraIndexTableYS)
                    s.ReadValue(PAName, "ExtraIndexTableI", ExtraIndexTableIS)
                    ExtraIndexTableF = ConvertInt(ExtraIndexTableFS)
                    ExtraIndexTableX = ConvertInt(ExtraIndexTableXS)
                    ExtraIndexTableY = ConvertInt(ExtraIndexTableYS)
                    ExtraIndexTableI = ConvertInt(ExtraIndexTableIS)
                Else
                    ExtraIndexTableF = Nothing
                    ExtraIndexTableX = Nothing
                    ExtraIndexTableY = Nothing
                    ExtraIndexTableI = Nothing
                End If

                If VersionSign = Version.Comm3Version3 OrElse VersionSign = Version.Comm3Version4 Then

                End If

                PAs(x, y) = New PictureArea(VersionSign, ZoomLayerNumber, Width, Height, NumberPicBlockReal, NumberPicBlockExtra, IndexTable, ExtraIndexTableF, ExtraIndexTableX, ExtraIndexTableY, ExtraIndexTableI)
            Next
        Next
        Return New Y64(VersionSign, Comm2PaletteCb, Comm2PaletteCr, PAs) With {.UnknownInts = UnknownInts, .UnknownFloats = UnknownFloats}
    End Function
    Protected Shared Function ConvertByte(ByVal s As String()) As Byte()
        If s Is Nothing Then Return Nothing
        Dim a As Byte() = New Byte(s.GetUpperBound(0)) {}
        For x As Integer = 0 To s.GetUpperBound(0)
            a(x) = Byte.Parse(s(x), Globalization.NumberStyles.HexNumber)
        Next
        Return a
    End Function
    Protected Shared Function ConvertInt(ByVal s As String(,)) As UInt16(,)
        If s Is Nothing Then Return Nothing
        Dim a As UInt16(,) = New UInt16(s.GetUpperBound(0), s.GetUpperBound(1)) {}
        For y As Integer = 0 To s.GetUpperBound(1)
            For x As Integer = 0 To s.GetUpperBound(0)
                a(x, y) = UShort.Parse(s(x, y), Globalization.NumberStyles.HexNumber)
            Next
        Next
        Return a
    End Function
    Protected Shared Function ConvertInt(ByVal s As String()) As Integer()
        If s Is Nothing Then Return Nothing
        Dim a As Integer() = New Integer(s.GetUpperBound(0)) {}
        For x As Integer = 0 To s.GetUpperBound(0)
            a(x) = Integer.Parse(s(x), Globalization.NumberStyles.HexNumber)
        Next
        Return a
    End Function

    Shared Sub Convert(ByVal SourceFilePath As String, ByVal TargetFilePath As String, ByVal TargetVersion As Version, ByVal ReplaceComm2Palette As Boolean, ByVal Lightness As Double, ByVal Saturation As Double)
        Dim SaturationRetrieve As Boolean = Lightness <> 1 OrElse Saturation <> 1
        Dim src As Y64 = Y64.Open(SourceFilePath)
        Dim tar As Y64 = Nothing

        Dim PAs As PictureArea(,) = src.PicArea

        If ReplaceComm2Palette Then
            tar = New Y64(TargetVersion, Comm2PaletteCb, Comm2PaletteCr, PAs) With {.UnknownInts = src.UnknownInts, .UnknownFloats = src.UnknownFloats}
        Else
            tar = New Y64(TargetVersion, src.PaletteCb, src.PaletteCr, PAs) With {.UnknownInts = src.UnknownInts, .UnknownFloats = src.UnknownFloats}
        End If
        If SaturationRetrieve Then
            tar.GetRetrieveTable(Lightness, Saturation)
            tar.GetReflectionTable()
        End If

        If SaturationRetrieve Then
            For x As Integer = 0 To src.NumView - 1
                For y As Integer = 0 To src.NumPic - 1
                    Dim PA As PictureArea = PAs(x, y)
                    For i As Integer = 0 To PA.NumberPicBlockReal - 1
                        Dim Block = PA.PicBlockReal(i)
                        Block.SetRectangle(Block.GetRectangle(src), tar)
                    Next
                Next
            Next
        End If

        Using fs As New StreamEx(TargetFilePath, FileMode.Create, FileAccess.ReadWrite)
            tar.WriteTo(fs)
        End Using
    End Sub

    Public Palette As Int32()
    Public ReflectionTableCbToIndex As Byte()
    Public ReflectionTableCrToIndex As Byte()
    Public ReflectionTableYAndYHighToDiff As Byte(,)
    Public ReflectionTableYAndYHighToYIndex As Int16(,)
    Public RetrieveTable As Int32()
    Private Sub GetReflectionTableCbCr()
        ReflectionTableCbToIndex = New Byte(255) {}
        For n As Integer = 0 To 255
            Dim X As Int32
            Dim I As Int32
            Dim Cb = n

            'Cb
            For X = 0 To 31
                If Cb < PaletteCb(X) Then Exit For
            Next
            If X = 0 Then
                I = X
            ElseIf X = 32 Then
                I = X - 1
            Else
                Dim Dif1 = Cb - PaletteCb(X - 1)
                Dim Dif2 = PaletteCb(X) - Cb
                If Dif1 < Dif2 Then
                    I = X - 1
                Else
                    I = X
                End If
            End If
            ReflectionTableCbToIndex(n) = CByte(I)
        Next

        ReflectionTableCrToIndex = New Byte(255) {}
        For n As Integer = 0 To 255
            Dim X As Int32
            Dim I As Int32
            Dim Cr = n

            'Cr
            For X = 0 To 31
                If Cr < PaletteCr(X) Then Exit For
            Next
            If X = 0 Then
                I = X
            ElseIf X = 32 Then
                I = X - 1
            Else
                Dim Dif1 = Cr - PaletteCr(X - 1)
                Dim Dif2 = PaletteCr(X) - Cr
                If Dif1 < Dif2 Then
                    I = X - 1
                Else
                    I = X
                End If
            End If
            ReflectionTableCrToIndex(n) = CByte(I)
        Next
    End Sub
    Private Sub GetReflectionTableY()
        ReflectionTableYAndYHighToDiff = New Byte(255, 15) {}
        ReflectionTableYAndYHighToYIndex = New Int16(255, 15) {}

        'Y
        Dim f As Int32() = {16, 8, 8, 8, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 2}
        Dim d As Int32() = {8, 4, 68, 132, 2, 66, 130, 194, 1, 33, 65, 97, 129, 161, 193, 225}
        'Palette DB 0
        '16x+8
        '8x+4
        '8x+68
        '8x+132
        '4x+2
        '4x+66
        '4x+130
        '4x+194
        '3x+1
        '3x+33
        '3x+65
        '3x+97
        '3x+129
        '3x+161
        '3x+193
        '2x+225
        For Y As Integer = 0 To 255
            For m As Integer = 0 To 15
                Dim X = CInt(Floor((Y - d(m)) / f(m)))
                If X < 0 Then
                    ReflectionTableYAndYHighToDiff(Y, m) = CByte(d(m) - Y)
                    ReflectionTableYAndYHighToYIndex(Y, m) = CShort(m * 16)
                ElseIf X >= 15 Then
                    ReflectionTableYAndYHighToDiff(Y, m) = CByte(Y - (15 * f(m) + d(m)))
                    ReflectionTableYAndYHighToYIndex(Y, m) = CShort(m * 16 + 15)
                Else
                    Dim Dif1 = CByte(Y - (X * f(m) + d(m)))
                    Dim Dif2 = CByte(((X + 1) * f(m) + d(m)) - Y)
                    If Dif1 < Dif2 Then
                        ReflectionTableYAndYHighToDiff(Y, m) = Dif1
                        ReflectionTableYAndYHighToYIndex(Y, m) = CShort(m * 16 + X)
                    Else
                        ReflectionTableYAndYHighToDiff(Y, m) = Dif2
                        ReflectionTableYAndYHighToYIndex(Y, m) = CShort(m * 16 + X + 1)
                    End If
                End If
            Next
        Next
    End Sub
    Public Sub GetReflectionTable()
        GetReflectionTableCbCr()
        GetReflectionTableY()
    End Sub
    Public Sub GetRetrieveTable(ByVal Lightness As Double, ByVal Saturation As Double)
        RetrieveTable = New Int32(65535) {}
        Dim R, G, B As Int32
        'Dim H, S, V As Double
        'Dim Y, Cb, Cr As Byte
        Dim gray, a As Double
        For rl As Integer = 0 To 31
            For gl As Integer = 0 To 63
                For bl As Integer = 0 To 31
                    R = rl << 3
                    G = gl << 2
                    B = bl << 3
                    a = Saturation
                    gray = 0.2126 * R + 0.7152 * G + 0.0722 * B
                    'gray = 0.299 * R + 0.587 * G + 0.114 * B
                    R = CInt(R * a + gray * (1 - a))
                    G = CInt(G * a + gray * (1 - a))
                    B = CInt(B * a + gray * (1 - a))
                    R = CInt(R * Lightness)
                    G = CInt(G * Lightness)
                    B = CInt(B * Lightness)
                    If R < 0 Then R = 0
                    If G < 0 Then G = 0
                    If B < 0 Then B = 0
                    If R > 255 Then R = 255
                    If G > 255 Then G = 255
                    If B > 255 Then B = 255
                    'RGB2HSV(rl << 3, gl << 2, bl << 3, H, S, V)
                    ''S = S + (1 - S) * 0.4
                    'S += 0.15
                    ''V += 0.05
                    'If S > 1 Then S = 1
                    'If V > 1 Then V = 1
                    'HSV2RGB(H, S, V, R, G, B)

                    'RGB2YCbCr(rl << 3, gl << 2, bl << 3, Y, Cb, Cr)

                    'If Cb * 1.4 > 255 Then Cb = 255 Else Cb = Cb * 1.4
                    'If Cr * 1.4 > 255 Then Cr = 255 Else Cr = Cr * 1.4

                    'YCbCr2RGB(Y, Cb, Cr, R, G, B)
                    RetrieveTable((rl << 11) Or (gl << 5) Or bl) = ConcatBits(&HFF, 8, CByte(R), 8, CByte(G), 8, CByte(B), 8)
                Next
            Next
        Next
    End Sub
    Shared Sub TurnBit(ByRef v As Integer, ByVal x As Byte, ByVal y As Byte)
        If CBool(v And (1 << x)) Xor CBool(v And (1 << y)) Then v = v Xor ((1 << x) Or (1 << y))
    End Sub
    Shared Function RGB32To16(ByVal ARGB As Int32) As Int16
        Return CID(((ARGB And &HF80000) >> 8) Or ((ARGB And &HFC00) >> 5) Or ((ARGB And &HF8) >> 3))
    End Function
    Shared Function RGB16To32(ByVal RGB16 As Int16) As Int32
        Dim r As Int32 = (EID(RGB16) And &HF800) >> 8
        Dim g As Int32 = (EID(RGB16) And &H7E0) >> 3
        Dim b As Int32 = (EID(RGB16) And &H1F) << 3
        r = r Or (r >> 5)
        g = g Or (g >> 6)
        b = b Or (b >> 5)
        Return &HFF000000 Or (r << 16) Or (g << 8) Or b
    End Function

    'End Of Class
    'Start Of SubClasses

    Public Class PictureArea
        Protected ZoomNumber As Int32
        Public ReadOnly Property ZoomLayerNumber() As Int32
            Get
                Return ZoomNumber
            End Get
        End Property
        Protected PicWidth As Int32
        Public ReadOnly Property Width() As Int32
            Get
                Return PicWidth
            End Get
        End Property
        Protected PicHeight As Int32
        Public ReadOnly Property Height() As Int32
            Get
                Return PicHeight
            End Get
        End Property
        Public ReadOnly Property NumPicBlockHorizontal() As Int32
            Get
                Return (PicWidth + 63) \ 64
            End Get
        End Property
        Public ReadOnly Property NumPicBlockVertical() As Int32
            Get
                Return (PicHeight + 63) \ 64
            End Get
        End Property
        Protected NumPicBlockReal As Int32
        Public ReadOnly Property NumberPicBlockReal() As Int32
            Get
                Return NumPicBlockReal
            End Get
        End Property
        Protected NumPicBlockExtra As Int32
        Public ReadOnly Property NumberPicBlockExtra() As Int32
            Get
                Return NumPicBlockExtra
            End Get
        End Property
        Public PicBlockReal As PictureBlock()
        Public IndexTable As UInt16(,)
        Public ExtraIndexTableF As Int32()
        Public ExtraIndexTableX As Int32()
        Public ExtraIndexTableY As Int32()
        Public ExtraIndexTableI As Int32()

        Private Function GetPicBlockAddressOffset(ByVal VersionSign As Version) As Int32
            Select Case VersionSign
                Case Y64.Version.Comm2Version1
                    Return 16 + NumPicBlockHorizontal * NumPicBlockVertical * 2
                Case Y64.Version.Comm2Version2
                    Return 20 + NumPicBlockHorizontal * NumPicBlockVertical * 2 + NumPicBlockExtra * 16
                Case Y64.Version.Comm3Version3, Y64.Version.Comm3Version4
                    Return 20 + NumPicBlockHorizontal * NumPicBlockVertical * 4 + NumPicBlockExtra * 16
                Case Else
                    Throw New InvalidDataException
            End Select
        End Function

        Protected LengthValue As Int32
        Public Sub New(ByVal s As StreamEx, ByVal VersionSign As Version, ByVal Length As Int32)
            Dim StartPosition = s.Position

            LengthValue = Length

            ZoomNumber = s.ReadInt32
            PicWidth = s.ReadInt32
            PicHeight = s.ReadInt32
            NumPicBlockReal = s.ReadInt32
            If VersionSign <> Y64.Version.Comm2Version1 Then NumPicBlockExtra = s.ReadInt32()
            Dim w As Integer = NumPicBlockHorizontal
            Dim h As Integer = NumPicBlockVertical
            Dim PicBlockRealOffset As Int32()
            Dim PicBlockRealLength As Int32()
            PicBlockReal = New PictureBlock(NumPicBlockReal - 1) {}
            IndexTable = New UInt16(w - 1, h - 1) {}

            Dim PicBlockAddressOffset As Int32 = GetPicBlockAddressOffset(VersionSign)
            If VersionSign = Y64.Version.Comm2Version1 OrElse VersionSign = Y64.Version.Comm2Version2 Then
                For y As Integer = 0 To h - 1
                    For x As Integer = 0 To w - 1
                        IndexTable(x, y) = s.ReadUInt16
                    Next
                Next
                If VersionSign <> Y64.Version.Comm2Version1 AndAlso NumberPicBlockExtra <> 0 Then
                    ExtraIndexTableF = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableX = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableY = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableI = New Int32(NumPicBlockExtra - 1) {}
                    For n As Integer = 0 To NumPicBlockExtra - 1
                        ExtraIndexTableF(n) = s.ReadInt32
                        ExtraIndexTableX(n) = s.ReadInt32
                        ExtraIndexTableY(n) = s.ReadInt32
                        ExtraIndexTableI(n) = s.ReadInt32
                    Next
                End If
                PicBlockRealLength = (New Int32(-1) {}).Extend(NumPicBlockReal, PictureBlock.NormalDataLength)
                PicBlockRealOffset = GetSummation(CInt(StartPosition + PicBlockAddressOffset), PicBlockRealLength)
            Else
                Dim AddressTable As Int32(,) = New Int32(w - 1, h - 1) {}
                Dim ExtraAddressTable As Int32()
                Dim SortedList As New SortedList(Of Int32, Int32)
                For y As Integer = 0 To h - 1
                    For x As Integer = 0 To w - 1
                        AddressTable(x, y) = s.ReadInt32
                        If AddressTable(x, y) = &HFFFF Then
                            IndexTable(x, y) = &HFFFF
                        Else
                            SortedList.Add(AddressTable(x, y), x + y * w)
                        End If
                    Next
                Next
                If NumberPicBlockExtra <> 0 Then
                    ExtraAddressTable = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableF = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableX = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableY = New Int32(NumPicBlockExtra - 1) {}
                    ExtraIndexTableI = New Int32(NumPicBlockExtra - 1) {}
                    For n As Integer = 0 To NumPicBlockExtra - 1
                        ExtraIndexTableF(n) = s.ReadInt32
                        ExtraIndexTableX(n) = s.ReadInt32
                        ExtraIndexTableY(n) = s.ReadInt32
                        ExtraAddressTable(n) = s.ReadInt32
                        If ExtraAddressTable(n) = &HFFFF Then
                            ExtraIndexTableI(n) = &HFFFF
                        Else
                            SortedList.Add(ExtraAddressTable(n), w * h + n)
                        End If
                    Next
                End If

                If NumPicBlockReal <> SortedList.Count Then Throw New InvalidDataException

                PicBlockRealOffset = SortedList.Keys.ToArray
                PicBlockRealLength = GetDifference(PicBlockRealOffset, CInt(StartPosition + Length))

                Dim Keys As IList(Of Int32) = SortedList.Keys
                Dim Values As IList(Of Int32) = SortedList.Values
                For n As Integer = 0 To SortedList.Count - 1
                    If Values(n) < w * h Then
                        IndexTable(Values(n) Mod w, Values(n) \ w) = CUShort(n)
                    Else
                        ExtraIndexTableI(Values(n) - w * h) = n
                    End If
                Next
            End If

            For n = 0 To NumPicBlockReal - 1
                s.Position = PicBlockRealOffset(n)
                PicBlockReal(n) = New PictureBlock(s, PicBlockRealLength(n))
            Next
        End Sub
        ''' <remarks>没有设置内部流和地址</remarks>
        Public Sub New(ByVal VersionSign As Y64.Version, ByVal ZoomLayerNumber As Int32, ByVal Width As Int32, ByVal Height As Int32, ByVal NumberPicBlockReal As Int32, ByVal NumberPicBlockExtra As Int32, ByVal IndexTable As UInt16(,), ByVal ExtraIndexTableF As Int32(), ByVal ExtraIndexTableX As Int32(), ByVal ExtraIndexTableY As Int32(), ByVal ExtraIndexTableI As Int32())
            If ZoomLayerNumber < 0 OrElse Width < 0 OrElse Height < 0 OrElse NumberPicBlockReal < 0 OrElse NumberPicBlockExtra < 0 Then Throw New InvalidDataException
            ZoomNumber = ZoomLayerNumber
            PicWidth = Width
            PicHeight = Height
            NumPicBlockReal = NumberPicBlockReal
            NumPicBlockExtra = NumberPicBlockExtra
            If IndexTable Is Nothing OrElse IndexTable.GetUpperBound(0) <> NumPicBlockHorizontal - 1 OrElse IndexTable.GetUpperBound(1) <> NumPicBlockVertical - 1 Then Throw New InvalidDataException
            Me.IndexTable = IndexTable
            If VersionSign <> Y64.Version.Comm2Version1 AndAlso NumberPicBlockExtra <> 0 Then
                If ExtraIndexTableF Is Nothing OrElse ExtraIndexTableF.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                If ExtraIndexTableX Is Nothing OrElse ExtraIndexTableX.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                If ExtraIndexTableY Is Nothing OrElse ExtraIndexTableY.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                If ExtraIndexTableI Is Nothing OrElse ExtraIndexTableI.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                Me.ExtraIndexTableF = ExtraIndexTableF
                Me.ExtraIndexTableX = ExtraIndexTableX
                Me.ExtraIndexTableY = ExtraIndexTableY
                Me.ExtraIndexTableI = ExtraIndexTableI
            End If
            Dim PicBlockAddressOffset As Int32 = GetPicBlockAddressOffset(VersionSign)
            PicBlockReal = New PictureBlock(NumPicBlockReal - 1) {}
            For n = 0 To NumPicBlockReal - 1
                PicBlockReal(n) = New PictureBlock()
            Next
        End Sub
        Public Sub WriteTo(ByVal s As StreamEx, ByVal VersionSign As Version)
            Dim StartPosition = s.Position
            s.WriteInt32(ZoomNumber)
            s.WriteInt32(PicWidth)
            s.WriteInt32(PicHeight)
            s.WriteInt32(NumPicBlockReal)
            If VersionSign <> Y64.Version.Comm2Version1 Then s.WriteInt32(NumPicBlockExtra)
            If IndexTable Is Nothing OrElse IndexTable.GetUpperBound(0) <> NumPicBlockHorizontal - 1 OrElse IndexTable.GetUpperBound(1) <> NumPicBlockVertical - 1 Then Throw New InvalidDataException
            Dim PicBlockRealLength As Int32() = (From b In PicBlockReal Select b.BlockLength(VersionSign)).ToArray
            Dim PicBlockAddressOffset As Int32 = GetPicBlockAddressOffset(VersionSign)
            Dim PicBlockRealOffset As Int32() = GetSummation(CInt(StartPosition + PicBlockAddressOffset), PicBlockRealLength)
            If VersionSign = Y64.Version.Comm2Version1 OrElse VersionSign = Y64.Version.Comm2Version2 Then
                For y As Integer = 0 To IndexTable.GetUpperBound(1)
                    For x As Integer = 0 To IndexTable.GetUpperBound(0)
                        s.WriteUInt16(IndexTable(x, y))
                    Next
                Next
                If VersionSign = Y64.Version.Comm2Version2 AndAlso NumberPicBlockExtra <> 0 Then
                    If ExtraIndexTableF Is Nothing OrElse ExtraIndexTableF.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableX Is Nothing OrElse ExtraIndexTableX.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableY Is Nothing OrElse ExtraIndexTableY.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableI Is Nothing OrElse ExtraIndexTableI.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    For x As Integer = 0 To ExtraIndexTableI.GetUpperBound(0)
                        s.WriteInt32(ExtraIndexTableF(x))
                        s.WriteInt32(ExtraIndexTableX(x))
                        s.WriteInt32(ExtraIndexTableY(x))
                        s.WriteInt32(ExtraIndexTableI(x))
                    Next
                End If
            Else
                For y As Integer = 0 To IndexTable.GetUpperBound(1)
                    For x As Integer = 0 To IndexTable.GetUpperBound(0)
                        If IndexTable(x, y) = &HFFFF Then
                            s.WriteInt32(&HFFFF)
                        Else
                            s.WriteInt32(PicBlockRealOffset(IndexTable(x, y)))
                        End If
                    Next
                Next
                If NumberPicBlockExtra <> 0 Then
                    If ExtraIndexTableF Is Nothing OrElse ExtraIndexTableF.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableX Is Nothing OrElse ExtraIndexTableX.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableY Is Nothing OrElse ExtraIndexTableY.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    If ExtraIndexTableI Is Nothing OrElse ExtraIndexTableI.GetUpperBound(0) <> NumberPicBlockExtra - 1 Then Throw New InvalidDataException
                    For x As Integer = 0 To ExtraIndexTableI.GetUpperBound(0)
                        s.WriteInt32(ExtraIndexTableF(x))
                        s.WriteInt32(ExtraIndexTableX(x))
                        s.WriteInt32(ExtraIndexTableY(x))
                        If ExtraIndexTableI(x) = &HFFFF Then
                            s.WriteInt32(&HFFFF)
                        Else
                            s.WriteInt32(PicBlockRealOffset(ExtraIndexTableI(x)))
                        End If
                    Next
                End If
            End If

            For n = 0 To NumPicBlockReal - 1
                s.Position = PicBlockRealOffset(n)
                PicBlockReal(n).WriteTo(s, VersionSign)
            Next
        End Sub

        Public ReadOnly Property Length(ByVal VersionSign As Version) As Integer
            Get
                Dim PicBlockAddressOffset As Int32 = GetPicBlockAddressOffset(VersionSign)
                Return PicBlockAddressOffset + (From b In PicBlockReal Select b.BlockLength(VersionSign)).Sum
            End Get
        End Property
        Public Property PicBlock(ByVal i As Int32) As PictureBlock
            Get
                If i < 0 Or i > NumberPicBlockReal - 1 Then Return Nothing
                If i = &HFFFF Then Return Nothing
                Return PicBlockReal(i)
            End Get
            Set(ByVal Value As PictureBlock)
                If i < 0 Or i > NumberPicBlockReal - 1 Then Throw New ArgumentOutOfRangeException
                If i = &HFFFF Then
                    If Value IsNot Nothing Then Throw New ArgumentException
                    Return
                End If
                If Value Is Nothing Then Throw New ArgumentNullException
                PicBlockReal(i) = Value
            End Set
        End Property
    End Class

    Public Class PictureBlock
        Public Const Width As Integer = 64
        Public Const Height As Integer = 64

        Public Sub New()
            SubPicBlocksData = New Byte(SubPicBlockLength * 64 - 1) {}
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal Length As Integer)
            If Length < NormalDataLength Then Throw New InvalidDataException
            Separator = s.ReadInt64
            SubPicBlocksData = s.Read(SubPicBlockLength * 64)
            If Length > NormalDataLength Then MaskData = s.Read(Length - NormalDataLength)
        End Sub

        Public ReadOnly Property BlockLength(ByVal VersionSign As Version) As Integer
            Get
                If VersionSign = Version.Comm3Version3 OrElse VersionSign = Version.Comm3Version4 Then
                    If MaskData IsNot Nothing Then
                        Return NormalDataLength + MaskData.Length
                    Else
                        Return NormalDataLength + 6
                    End If
                End If
                Return NormalDataLength
            End Get
        End Property
        Public Const NormalDataLength As Integer = 3592
        Private Const SubPicBlockLength As Integer = 56
        Private Separator As Int64 = &HFFFFFFFFFFFFFFFFL
        Public SubPicBlocksData As Byte()
        Public MaskData As Byte()
        Public Sub WriteTo(ByVal s As StreamEx, ByVal VersionSign As Version)
            s.WriteInt64(Separator)
            s.Write(SubPicBlocksData)
            If VersionSign = Version.Comm3Version3 OrElse VersionSign = Version.Comm3Version4 Then
                If MaskData IsNot Nothing Then
                    s.Write(MaskData)
                Else
                    s.Write(New Byte() {&H14, &HFF, 0, 0, 0, 0})
                End If
            End If
        End Sub
        Public Function GetRectangle(ByVal yf As Y64) As Int32(,)
            Dim Pixel = New Int32(63, 63) {}
            For y2 As Integer = 0 To 7
                For x2 As Integer = 0 To 7
                    Using s As New ByteArrayStream(SubPicBlocksData, SubPicBlockLength * (x2 + y2 * 8), SubPicBlockLength)
                        Dim DS1 = New Byte(19) {}
                        Dim DS2 = New Byte(31) {}

                        Dim DS0 = s.ReadInt32
                        For n As Integer = 0 To 4
                            DS1(n * 4 + 3) = s.ReadByte
                            DS1(n * 4 + 2) = s.ReadByte
                            DS1(n * 4 + 1) = s.ReadByte
                            DS1(n * 4) = s.ReadByte
                        Next
                        For n As Integer = 0 To 7
                            DS2(n * 4 + 3) = s.ReadByte
                            DS2(n * 4 + 2) = s.ReadByte
                            DS2(n * 4 + 1) = s.ReadByte
                            DS2(n * 4) = s.ReadByte
                        Next

                        '填满Pixel
                        Dim PixelIndex As Int32() = New Int32(63) {}
                        Dim c As Int32

                        '中位索引
                        c = (DS0 >> 12) << 4
                        For x As Integer = 0 To 3
                            For y As Integer = 0 To 3
                                PixelIndex(x + y * 8) = c
                            Next
                        Next
                        c = ((DS0 >> 8) And 15) << 4
                        For x As Integer = 4 To 7
                            For y As Integer = 0 To 3
                                PixelIndex(x + y * 8) = c
                            Next
                        Next
                        c = DS0 And 240
                        For x As Integer = 0 To 3
                            For y As Integer = 4 To 7
                                PixelIndex(x + y * 8) = c
                            Next
                        Next
                        c = (DS0 And 15) << 4
                        For x As Integer = 4 To 7
                            For y As Integer = 4 To 7
                                PixelIndex(x + y * 8) = c
                            Next
                        Next

                        '高位索引
                        For n As Integer = 0 To 3
                            c = (CInt(DS1(n * 5)) << 2) Or (CInt(DS1(n * 5 + 1)) >> 6)
                            c = (c And 1023) << 8
                            For x As Integer = 0 To 1
                                For y As Integer = n * 2 To n * 2 + 1
                                    PixelIndex(x + y * 8) = PixelIndex(x + y * 8) Or c
                                Next
                            Next
                            c = (CInt(DS1(n * 5 + 1)) << 4) Or (CInt(DS1(n * 5 + 2)) >> 4)
                            c = (c And 1023) << 8
                            For x As Integer = 2 To 3
                                For y As Integer = n * 2 To n * 2 + 1
                                    PixelIndex(x + y * 8) = PixelIndex(x + y * 8) Or c
                                Next
                            Next
                            c = (CInt(DS1(n * 5 + 2)) << 6) Or (CInt(DS1(n * 5 + 3)) >> 2)
                            c = (c And 1023) << 8
                            For x As Integer = 4 To 5
                                For y As Integer = n * 2 To n * 2 + 1
                                    PixelIndex(x + y * 8) = PixelIndex(x + y * 8) Or c
                                Next
                            Next
                            c = (CInt(DS1(n * 5 + 3)) << 8) Or CInt(DS1(n * 5 + 4))
                            c = (c And 1023) << 8
                            For x As Integer = 6 To 7
                                For y As Integer = n * 2 To n * 2 + 1
                                    PixelIndex(x + y * 8) = PixelIndex(x + y * 8) Or c
                                Next
                            Next
                        Next

                        '低位索引
                        For n As Integer = 0 To 31
                            PixelIndex(n * 2) = PixelIndex(n * 2) Or (DS2(n) >> 4)
                            PixelIndex(n * 2 + 1) = PixelIndex(n * 2 + 1) Or (DS2(n) And 15)
                        Next

                        For y As Integer = 0 To 7
                            For x As Integer = 0 To 7
                                Pixel(x + x2 * 8, y + y2 * 8) = yf.Palette(PixelIndex(x + y * 8))
                            Next
                        Next
                    End Using
                Next
            Next
            Return Pixel
        End Function
        Public Sub SetRectangle(ByVal Pixel As Int32(,), ByVal yf As Y64)
            If Pixel.GetLength(0) <> 64 OrElse Pixel.GetLength(1) <> 64 Then Throw New ArgumentException
            Dim PixelYCbCr = New Int32(64 - 1, 64 - 1) {}
            For y = 0 To 64 - 1
                For x = 0 To 64 - 1
                    Dim p = Pixel(x, y)
                    PixelYCbCr(x, y) = RGB2YCbCr(CByte(p.Bits(23, 16)), CByte(p.Bits(15, 8)), CByte(p.Bits(7, 0)))
                Next
            Next
            Dim ReflectionTableYAndYHighToDiff = yf.ReflectionTableYAndYHighToDiff
            For y2 As Integer = 0 To 7
                For x2 As Integer = 0 To 7
                    Using s As New ByteArrayStream(SubPicBlocksData, SubPicBlockLength * (x2 + y2 * 8), SubPicBlockLength)
                        Dim DS1 = New Byte(19) {}
                        Dim DS2 = New Byte(31) {}

                        '高位索引
                        Dim CbValue = Function(x As Integer, y As Integer) PixelYCbCr(x, y).Bits(15, 8)
                        Dim CrValue = Function(x As Integer, y As Integer) PixelYCbCr(x, y).Bits(7, 0)
                        Dim AvgCbCrValue =
                            Function(x As Integer, y As Integer) As Integer
                                Dim Cb = (CbValue(x, y) + CbValue(x + 1, y) + CbValue(x + 1, y + 1) + CbValue(x, y + 1) + 2) \ 4
                                Dim Cr = (CrValue(x, y) + CrValue(x + 1, y) + CrValue(x + 1, y + 1) + CrValue(x, y + 1) + 2) \ 4
                                Dim CbIndex = CInt(yf.ReflectionTableCbToIndex(Cb))
                                Dim CrIndex = CInt(yf.ReflectionTableCrToIndex(Cr))
                                Return CbIndex.ConcatBits(CrIndex, 5)
                            End Function
                        For n As Integer = 0 To 3
                            Dim c = AvgCbCrValue(0 + x2 * 8, n * 2 + y2 * 8)
                            DS1(n * 5) = CByte(c >> 2)
                            Dim d = AvgCbCrValue(2 + x2 * 8, n * 2 + y2 * 8)
                            DS1(n * 5 + 1) = CByte(((c << 6) Or (d >> 4)) And 255)
                            c = AvgCbCrValue(4 + x2 * 8, n * 2 + y2 * 8)
                            DS1(n * 5 + 2) = CByte(((d << 4) Or (c >> 6)) And 255)
                            d = AvgCbCrValue(6 + x2 * 8, n * 2 + y2 * 8)
                            DS1(n * 5 + 3) = CByte(((c << 2) Or (d >> 8)) And 255)
                            DS1(n * 5 + 4) = CByte(d And 255)
                        Next

                        '中位索引
                        Dim xx2 = x2
                        Dim yy2 = y2
                        Dim YValue = New Byte(8 * 8 - 1) {}
                        For y = 0 To 7
                            For x = 0 To 7
                                YValue(x + y * 8) = CByte(PixelYCbCr(x + xx2 * 8, y + yy2 * 8).Bits(23, 16))
                            Next
                        Next
                        Dim Index As Byte() = New Byte(3) {}
                        For yy As Integer = 0 To 1
                            For xx As Integer = 0 To 1
                                Dim DifMin = Integer.MaxValue
                                Dim IndexMin As Int32 = -1
                                For n As Integer = 0 To 15
                                    Dim Dif = 0
                                    For y As Integer = yy * 4 To yy * 4 + 3
                                        For x As Integer = xx * 4 To xx * 4 + 3
                                            Dim i = YValue(x + y * 8)
                                            Dim t = CInt(ReflectionTableYAndYHighToDiff(i, n))
                                            Dif += t * t
                                        Next
                                    Next
                                    If Dif < DifMin Then
                                        DifMin = Dif
                                        IndexMin = n
                                    End If
                                Next
                                Index(xx + yy * 2) = CByte(IndexMin)
                            Next
                        Next
                        Dim DS0 = (CInt(Index(0)) << 12) Or (CInt(Index(1)) << 8) Or (CInt(Index(2)) << 4) Or CInt(Index(3))

                        '低位索引
                        For yy As Integer = 0 To 1S
                            For xx As Integer = 0 To 1
                                For y As Integer = yy * 4 To yy * 4 + 3
                                    For x As Integer = xx * 2 To xx * 2 + 1
                                        DS2(x + y * 4) = CByte(((yf.ReflectionTableYAndYHighToYIndex(YValue(x * 2 + y * 8), Index(xx + yy * 2)) And 15) << 4) Or (yf.ReflectionTableYAndYHighToYIndex(YValue(x * 2 + 1 + y * 8), Index(xx + yy * 2)) And 15))
                                    Next
                                Next
                            Next
                        Next

                        s.WriteInt32(DS0)
                        For n As Integer = 0 To 4
                            s.WriteByte(DS1(n * 4 + 3))
                            s.WriteByte(DS1(n * 4 + 2))
                            s.WriteByte(DS1(n * 4 + 1))
                            s.WriteByte(DS1(n * 4))
                        Next
                        For n As Integer = 0 To 7
                            s.WriteByte(DS2(n * 4 + 3))
                            s.WriteByte(DS2(n * 4 + 2))
                            s.WriteByte(DS2(n * 4 + 1))
                            s.WriteByte(DS2(n * 4))
                        Next
                    End Using
                Next
            Next
        End Sub
        Public Function GetMask() As PictureBlockMask
            If MaskData Is Nothing Then Return PictureBlockMask.Zero()
            Return PictureBlockMask.FromByteArray(MaskData)
        End Function
        Public Sub SetMask(ByVal Mask As PictureBlockMask)
            If Mask.Mask.All(Function(p) p.Z = 0 AndAlso p.N = 0) AndAlso Mask.HalfTransparent.All(Function(t) t = 0) Then
                MaskData = Nothing
            Else
                MaskData = Mask.ToByteArray()
            End If
        End Sub
    End Class

    Public Structure PictureBlockMaskPixel
        Public Z As UInt16 '高度，数值越高高度越高
        Public N As UInt16 '未知
    End Structure

    Public Class PictureBlockMask
        Public Const Width As Integer = PictureBlock.Width
        Public Const Height As Integer = PictureBlock.Height

        Public Mask As PictureBlockMaskPixel() '高度图
        Public HalfTransparent As UInt16()

        Public Shared Function Zero() As PictureBlockMask
            Dim Mask = New PictureBlockMaskPixel(Width * Height - 1) {}
            Dim HalfTransparent = New UInt16(Width * Height - 1) {}
            Return New PictureBlockMask With {.Mask = Mask, .HalfTransparent = HalfTransparent}
        End Function
        Public Shared Function FromByteArray(ByVal Bytes As Byte()) As PictureBlockMask
            Dim Mask = New PictureBlockMaskPixel(Width * Height - 1) {}
            Dim HalfTransparent = New UInt16(Width * Height - 1) {}
            Dim ReadMask =
                Sub(s As ByteArrayStream, sm As ArrayStream(Of PictureBlockMaskPixel))
                    While sm.Position < sm.Length
                        Dim Head = s.ReadUInt16()
                        If Head = 65100 Then
                            Dim z = s.ReadUInt16()
                            For k = 0 To 64 - 1
                                sm.WriteElement(New PictureBlockMaskPixel With {.Z = z, .N = 0})
                            Next
                        ElseIf Head > 65100 Then
                            Dim n = s.ReadUInt16()
                            Dim z = s.ReadUInt16()
                            If z = &H4885 Then
                                Dim a = 0
                            End If
                            Dim Count = Head - 65100
                            If Count <= 3 Then Throw New InvalidDataException
                            For k = 0 To Count - 1
                                sm.WriteElement(New PictureBlockMaskPixel With {.Z = z, .N = n})
                            Next
                        ElseIf Head > 65000 Then
                            Dim z = s.ReadUInt16()
                            Dim Count = Head - 65000
                            If Count <= 2 Then Throw New InvalidDataException
                            For k = 0 To Count - 1
                                sm.WriteElement(New PictureBlockMaskPixel With {.Z = z, .N = 0})
                            Next
                        Else
                            Dim z = Head
                            sm.WriteElement(New PictureBlockMaskPixel With {.Z = z, .N = 0})
                        End If
                    End While
                End Sub

            Using s As New ByteArrayStream(Bytes)
                Using sm As New ArrayStream(Of PictureBlockMaskPixel)(Mask)
                    Using st As New ArrayStream(Of UInt16)(HalfTransparent)
                        Dim Type = s.ReadUInt16()
                        If Type = 65300 Then '0xFF14
                            Dim zTop = s.ReadUInt16()
                            Dim zBottom = s.ReadUInt16()
                            For y = 0 To Height - 1
                                Dim z = CUShort((zTop * (Height - y) + zBottom * y) \ Height)
                                For x = 0 To Width - 1
                                    sm.WriteElement(New PictureBlockMaskPixel With {.Z = z, .N = 0})
                                Next
                            Next
                            If sm.Position <> sm.Length Then Throw New InvalidDataException
                            While st.Position < st.Length
                                st.WriteElement(0)
                            End While
                        ElseIf Type = 65301 Then '0xFF15
                            ReadMask(s, sm)
                            While st.Position < st.Length
                                st.WriteElement(0)
                            End While
                        ElseIf Type = 65302 Then '0xFF16
                            ReadMask(s, sm)
                            While st.Position < st.Length
                                Dim Head = s.ReadUInt16()
                                If Head > 65000 Then
                                    Dim t = CUShort(0)
                                    For k = 0 To Head - 65000 - 1
                                        st.WriteElement(t)
                                    Next
                                Else
                                    Dim t = Head
                                    st.WriteElement(t)
                                End If
                            End While
                        Else
                            Throw New InvalidDataException
                        End If
                    End Using
                End Using

                If s.Position <> s.Length Then Throw New InvalidDataException
            End Using
            Return New PictureBlockMask With {.Mask = Mask, .HalfTransparent = HalfTransparent}
        End Function
        Private Function CheckUseType14(ByVal IsHalfTransparentEmpty As Boolean) As Boolean
            If Not IsHalfTransparentEmpty Then Return False
            Dim zTop = Mask.First.Z
            Dim zBottom = Mask.Last.Z
            If Not Mask.All(Function(m) m.N = 0) Then Return False
            For y = 0 To Height - 1
                Dim z = CUShort((zTop * (Height - y) + zBottom * y) \ Height)
                For x = 0 To Width - 1
                    If Mask(x + y * Width).Z <> z Then Return False
                Next
            Next
            Return True
        End Function
        Public Function ToByteArray() As Byte()
            Dim IsHalfTransparentEmpty = HalfTransparent.All(Function(t) t = 0)
            Dim UseType14 = CheckUseType14(IsHalfTransparentEmpty)

            Dim WriteMask =
                Sub(s As IWritableStream)
                    For y = 0 To Height - 1
                        Dim zLeft = Mask(y * Width).Z
                        If Mask.Skip(y * Width).Take(Width).All(Function(p) p.Z = zLeft AndAlso p.N = 0) Then
                            If zLeft > 65000 Then zLeft = 65000
                            s.WriteUInt16(65100)
                            s.WriteUInt16(zLeft)
                            Continue For
                        End If
                        For x = 0 To Width - 1
                            Dim p = Mask(x + y * Width)
                            Dim z = p.Z
                            If z > 65000 Then z = 65000
                            s.WriteUInt16(z)
                        Next
                    Next
                End Sub

            Using s = Streams.CreateMemoryStream()
                If UseType14 Then
                    Dim zTop = Mask.First.Z
                    Dim zBottom = Mask.Last.Z
                    s.WriteUInt16(65300)
                    s.WriteUInt16(zTop)
                    s.WriteUInt16(zBottom)
                ElseIf IsHalfTransparentEmpty Then
                    s.WriteUInt16(65301)
                    WriteMask(s)
                Else
                    s.WriteUInt16(65302)
                    WriteMask(s)
                    For y = 0 To Height - 1
                        Dim x = 0
                        While x < Width
                            Dim n = 0
                            While x < Width
                                If HalfTransparent(x + y * Width) = 0 Then
                                    n += 1
                                Else
                                    Exit While
                                End If
                                x += 1
                            End While
                            While n >= 535
                                s.WriteUInt16(65535)
                                n -= 535
                            End While
                            If n >= 3 Then
                                s.WriteUInt16(CUShort(65000 + n))
                                n = 0
                            End If
                            While n > 0
                                s.WriteUInt16(0)
                                n -= 1
                            End While
                            If x < Width Then
                                Dim t = HalfTransparent(x + y * Width)
                                If t <> 0 Then
                                    If t > 65000 Then t = 65000
                                    s.WriteUInt16(t)
                                    x += 1
                                End If
                            End If
                        End While
                    Next
                End If

                s.Position = 0
                Return s.Read(CInt(s.Length))
            End Using
        End Function
    End Class

    Public Class PicOutOfBoundException
        Inherits Exception
        Public Shared Error_PicOutOfBound As String = "图片索引越界"
        Public Sub New(ByVal NumberView As Integer, ByVal NumberPic As Integer)
            MyBase.New(Error_PicOutOfBound & Environment.NewLine & "NumberView = " & NumberView & Environment.NewLine & "NumberPic = " & NumberPic)
        End Sub
    End Class
    Public Class PicSizeDismatchException
        Inherits Exception
        Public Shared Error_PicSizeDismatch As String = "图片大小或色深不匹配"
        Public Sub New(ByVal Width As Integer, ByVal Height As Integer)
            MyBase.New(Error_PicSizeDismatch & Environment.NewLine & "PicWidth = " & Width & Environment.NewLine & "PicHeight = " & Height & Environment.NewLine & "BitsPerPixel = 16/24")
        End Sub
    End Class
    Public Class ExtraPicSizeDismatchException
        Inherits Exception
        Public Shared Error_ExtraPicSizeDismatch As String = "附加图片大小或色深不匹配"
        Public Sub New(ByVal Width As Integer, ByVal Height As Integer)
            MyBase.New(Error_ExtraPicSizeDismatch & Environment.NewLine & "PicWidth = " & Width & Environment.NewLine & "PicHeight = " & Height & Environment.NewLine & "BitsPerPixel = 16/24")
        End Sub
    End Class
    Public Class VersionDismatchException
        Inherits Exception
        Public Shared Error_VersionDismatch As String = "版本号不匹配"
        Public Sub New()
            MyBase.New(Error_VersionDismatch)
        End Sub
    End Class
End Class
