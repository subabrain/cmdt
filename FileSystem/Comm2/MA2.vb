'==========================================================================
'
'  File:        MA2.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: MA2文件流类
'  Version:     2014.01.03.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Imaging
Imports Firefly.Setting

Public NotInheritable Class Array1(Of T)
    Public Shared Function Create(ByVal Count As Integer) As T()
        Return New T(Count - 1) {}
    End Function
End Class

''' <summary>2维数组</summary>
Public Class Array2(Of T)
    Private Count0 As Integer
    Private Count1 As Integer
    Private Value As T()

    Public Shared Function Create(ByVal Count0 As Integer, ByVal Count1 As Integer) As Array2(Of T)
        Return New Array2(Of T)(Count0, Count1)
    End Function
    Public Sub New()
        Count0 = 0
        Count1 = 0
        Value = New T() {}
    End Sub
    Public Sub New(ByVal Length0 As Integer, ByVal Length1 As Integer)
        If Length0 < 0 Then Throw New ArgumentOutOfRangeException
        If Length1 < 0 Then Throw New ArgumentOutOfRangeException
        Count0 = Length0
        Count1 = Length1
        Value = New T(Length0 * Length1 - 1) {}
    End Sub
    Public Property Length0() As Integer
        Get
            Return Count0
        End Get
        Set(ByVal Value As Integer)
            If Count0 < 0 Then Throw New ArgumentOutOfRangeException
            Count0 = Value
        End Set
    End Property
    Public Property Length1() As Integer
        Get
            Return Count1
        End Get
        Set(ByVal Value As Integer)
            If Count1 < 0 Then Throw New ArgumentOutOfRangeException
            Count1 = Value
        End Set
    End Property
    Public ReadOnly Property Upper0() As Integer
        Get
            Return Count0 - 1
        End Get
    End Property
    Public ReadOnly Property Upper1() As Integer
        Get
            Return Count1 - 1
        End Get
    End Property
    Public Property Data() As T()
        Get
            Return Value
        End Get
        Set(ByVal Value As T())
            Me.Value = Value
        End Set
    End Property
    Default Public Property Element(ByVal Index0 As Integer, ByVal Index1 As Integer) As T
        Get
            Return Value(Index0 + Index1 * Count0)
        End Get
        Set(ByVal Out As T)
            Value(Index0 + Index1 * Count0) = Out
        End Set
    End Property
    Public ReadOnly Property GetLength(ByVal Dimension As Integer) As Integer
        Get
            Select Case Dimension
                Case 0
                    Return Count0
                Case 1
                    Return Count1
                Case Else
                    Throw New ArgumentOutOfRangeException
            End Select
        End Get
    End Property
    Public ReadOnly Property GetUpperBound(ByVal Dimension As Integer) As Integer
        Get
            Select Case Dimension
                Case 0
                    Return Count0 - 1
                Case 1
                    Return Count1 - 1
                Case Else
                    Throw New ArgumentOutOfRangeException
            End Select
        End Get
    End Property
    Public Function ToMultiDimensionArray() As T(,)
        Dim v = New T(Count0 - 1, Count1 - 1) {}
        For y = 0 To Count1 - 1
            For x = 0 To Count0 - 1
                v(x, y) = Element(x, y)
            Next
        Next
        Return v
    End Function
End Class
Public Class Array2Mapper(Of T)
    Inherits Xml.Mapper(Of Array2(Of T), T()())

    Public Overloads Overrides Function GetMappedObject(ByVal o As Array2(Of T)) As T()()
        Dim arr = New T(o.Length1 - 1)() {}
        For y = 0 To o.Length1 - 1
            Dim a = New T(o.Length0 - 1) {}
            For x = 0 To o.Length0 - 1
                a(x) = o(x, y)
            Next
            arr(y) = a
        Next
        Return arr
    End Function
    Public Overloads Overrides Function GetInverseMappedObject(ByVal o()() As T) As Array2(Of T)
        Dim Length1 = o.Length
        Dim Length0 = 0
        If Length1 > 0 Then Length0 = (From e In o Select o.Length).Distinct.Single

        Dim arr = Array2(Of T).Create(Length0, Length1)
        For y = 0 To Length1 - 1
            Dim a = o(y)
            For x = 0 To Length0 - 1
                arr(x, y) = a(x)
            Next
        Next
        Return arr
    End Function
End Class

''' <summary>MA2文件流类</summary>
''' <remarks>
''' 用于打开盟军2及3的MA2文件
''' 注意：压缩仅用了两种压缩方法中的一种，并没有实现最优
''' </remarks>
Public Class MA2
    Public Enum Version As Byte
        Comm2Demo = 2
        Comm2 = 3
    End Enum
    Public VersionSign As Version
    Public Unknown As Byte()
    Public ReadOnly Property NumView() As Int32
        Get
            If ViewInfo Is Nothing Then Return 0
            Return ViewInfo.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property NumObject() As Int32
        Get
            If ObjectInfo Is Nothing Then Return 0
            Return ObjectInfo.GetLength(0)
        End Get
    End Property

    Public ViewInfo As ViewInfoBlock()
    Public ObjectInfo As ObjectInfoBlock()
    Public WaterMaskInfo As WaterMaskInfoBlock()
    Public WaterMaskHighQualityInfo As WaterMaskInfoBlock()
    Public RenderInfo As RenderInfoBlock()()

    Sub New()
    End Sub
    Sub New(ByVal Path As String)
        Using sf As New StreamEx(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            VersionSign = CType(sf.ReadByte, Version)
            If VersionSign <> Version.Comm2 AndAlso VersionSign <> Version.Comm2Demo Then
                sf.Close()
                Throw New InvalidDataException
            End If
            Unknown = sf.Read(28)

            Dim NumView As Int32
            Dim NumObject As Int32
            Dim NumObjectAllView As Int32
            NumView = sf.ReadInt32
            NumObject = sf.ReadInt32
            NumObjectAllView = sf.ReadInt32
            If sf.ReadInt32 <> NumObjectAllView Then Throw New InvalidDataException
            If sf.ReadInt32 <> 0 Then Throw New InvalidDataException

            Dim ViewDataAddress As Int32
            Dim ObjectInfoAddress As Int32
            Dim RenderIndexAddress As Int32
            Dim RenderDataAddress As Int32
            Dim ObjectDistrictInfoDataAddress As Int32
            ViewDataAddress = sf.ReadInt32
            ObjectInfoAddress = sf.ReadInt32
            RenderIndexAddress = sf.ReadInt32
            RenderDataAddress = sf.ReadInt32
            ObjectDistrictInfoDataAddress = sf.ReadInt32
            sf.Position += 4

            sf.Position = ViewDataAddress
            ViewInfo = New ViewInfoBlock(NumView - 1) {}
            Dim ViewInfoRaw = New ViewInfoBlockRaw(NumView - 1) {}
            For n As Integer = 0 To NumView - 1
                Dim r = New ViewInfoBlockRaw(sf, VersionSign)
                ViewInfoRaw(n) = r
                ViewInfo(n) = New ViewInfoBlock With {.OffsetX = r.OffsetX, .OffsetY = r.OffsetY, .ViewLongitude = r.ViewLongitude, .ViewLatitude = r.ViewLatitude}
            Next

            sf.Position = ObjectInfoAddress
            ObjectInfo = New ObjectInfoBlock(NumObject - 1) {}
            For n As Integer = 0 To NumObject - 1
                ObjectInfo(n) = New ObjectInfoBlock(sf)
            Next

            sf.Position = RenderIndexAddress
            RenderInfo = New RenderInfoBlock(NumView - 1)() {}
            For n As Integer = 0 To NumView - 1
                RenderInfo(n) = New RenderInfoBlock(ViewInfoRaw(n).NumObjectToRender - 1) {}
                For m As Integer = 0 To ViewInfoRaw(n).NumObjectToRender - 1
                    RenderInfo(n)(m) = New RenderInfoBlock(sf)
                Next
            Next

            WaterMaskInfo = New WaterMaskInfoBlock(NumView - 1) {}
            WaterMaskHighQualityInfo = New WaterMaskInfoBlock(NumView - 1) {}
            For n As Integer = 0 To NumView - 1
                Dim a = ViewInfoRaw(n).WaterMaskAddress
                If a <> 0 Then
                    sf.Position = a
                    WaterMaskInfo(n) = New WaterMaskInfoBlock(sf)
                End If
                Dim b = ViewInfoRaw(n).WaterMaskHighQualityAddress
                If b <> 0 Then
                    sf.Position = b
                    WaterMaskHighQualityInfo(n) = New WaterMaskInfoBlock(sf)
                End If
            Next

            sf.Position = RenderDataAddress
            For n As Integer = 0 To NumView - 1
                For m As Integer = 0 To ViewInfoRaw(n).NumObjectToRender - 1
                    sf.Position = ((sf.Position + 3) \ 4) * 4
                    RenderInfo(n)(m).ReadData(sf)
                Next
            Next

            sf.Position = ObjectDistrictInfoDataAddress
            For n As Integer = 0 To NumObject - 1
                ObjectInfo(n).ReadDistrictInfo(sf)
            Next
        End Using
    End Sub
    Public Sub WriteToFile(ByVal Path As String)
        Using sf As New StreamEx(Path, FileMode.Create, FileAccess.ReadWrite)
            sf.WriteByte(VersionSign)
            sf.Write(Unknown)

            sf.WriteInt32(NumView)
            sf.WriteInt32(NumObject)
            Dim NumObjectToRender As Int32() = New Int32(NumView - 1) {}
            Dim NumObjectAllView As Int32 = 0
            For n As Integer = 0 To NumView - 1
                NumObjectToRender(n) = Me.NumObjectToRender(n)
                NumObjectAllView += NumObjectToRender(n)
            Next
            sf.WriteInt32(NumObjectAllView)
            sf.WriteInt32(NumObjectAllView)
            sf.WriteInt32(0)

            Dim pAddress As Int32 = CInt(sf.Position)
            Dim ViewDataAddress As Int32
            Dim ObjectInfoAddress As Int32
            Dim RenderIndexAddress As Int32
            Dim RenderDataAddress As Int32
            Dim ObjectDistrictInfoDataAddress As Int32
            sf.Position += 24

            Dim pViewInfo As Int32() = New Int32(NumView - 1) {}
            Dim ViewInfoRaw As ViewInfoBlockRaw() = New ViewInfoBlockRaw(NumView - 1) {}
            For n As Integer = 0 To NumView - 1
                Dim i = ViewInfo(n)
                Dim r = New ViewInfoBlockRaw With {.OffsetX = i.OffsetX, .OffsetY = i.OffsetY, .ViewLongitude = i.ViewLongitude, .ViewLatitude = i.ViewLatitude}
                r.NumObjectToRender = NumObjectToRender(n)
                ViewInfoRaw(n) = r
            Next

            Dim pObjectInfo As Int32() = New Int32(NumObject - 1) {}
            Dim ObjectDistrictInfoOffset As Int32() = New Int32(NumObject - 1) {}
            Dim ObjectDistrictInfoLength As Int32() = New Int32(NumObject - 1) {}

            Dim pRenderIndex As Int32()() = New Int32(NumView - 1)() {}
            Dim RenderDataOffsetInIndex As Int32()() = New Int32(NumView - 1)() {}
            Dim RenderDataLengthInIndex As Int32()() = New Int32(NumView - 1)() {}

            ViewDataAddress = CInt(sf.Position)
            For n As Integer = 0 To NumView - 1
                pViewInfo(n) = CInt(sf.Position)
                ViewInfoRaw(n).WriteToFile(sf, VersionSign)
            Next

            ObjectInfoAddress = CInt(sf.Position)
            For n As Integer = 0 To NumObject - 1
                pObjectInfo(n) = CInt(sf.Position)
                ObjectInfo(n).Write(sf)
            Next

            RenderIndexAddress = CInt(sf.Position)
            For n As Integer = 0 To NumView - 1
                ViewInfoRaw(n).RenderIndexOffset = CInt(sf.Position)
                pRenderIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    pRenderIndex(n)(m) = CInt(sf.Position)
                    RenderInfo(n)(m).Write(sf)
                Next
                ViewInfoRaw(n).RenderIndexLength = CInt(sf.Position - ViewInfoRaw(n).RenderIndexOffset)
            Next
            For n As Integer = 0 To NumView - 1
                ViewInfoRaw(n).RenderIndexOffset -= RenderIndexAddress
            Next

            For n As Integer = 0 To NumView - 1
                If WaterMaskInfo(n) IsNot Nothing Then
                    ViewInfoRaw(n).WaterMaskAddress = CInt(sf.Position)
                    WaterMaskInfo(n).Write(sf)
                End If
                If WaterMaskHighQualityInfo(n) IsNot Nothing Then
                    ViewInfoRaw(n).WaterMaskHighQualityAddress = CInt(sf.Position)
                    WaterMaskHighQualityInfo(n).Write(sf)
                End If
            Next

            RenderDataAddress = CInt(sf.Position)
            sf.Position = ((sf.Position + 3) \ 4) * 4
            For n As Integer = 0 To NumView - 1
                ViewInfoRaw(n).RenderDataOffset = CInt(sf.Position)
                RenderDataOffsetInIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                RenderDataLengthInIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    RenderDataOffsetInIndex(n)(m) = CInt(sf.Position)
                    RenderInfo(n)(m).WriteData(sf)
                    RenderDataLengthInIndex(n)(m) = CInt(sf.Position - RenderDataOffsetInIndex(n)(m))
                    sf.Position = ((sf.Position + 3) \ 4) * 4
                Next
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    RenderDataOffsetInIndex(n)(m) -= ViewInfoRaw(n).RenderDataOffset
                Next
                ViewInfoRaw(n).RenderDataLength = CInt(sf.Position - ViewInfoRaw(n).RenderDataOffset)
            Next
            For n As Integer = 0 To NumView - 1
                ViewInfoRaw(n).RenderDataOffset -= RenderDataAddress
            Next

            ObjectDistrictInfoDataAddress = CInt(sf.Position)
            For n As Integer = 0 To NumObject - 1
                ObjectDistrictInfoOffset(n) = CInt(sf.Position)
                ObjectInfo(n).WriteDistrictInfo(sf)
                ObjectDistrictInfoLength(n) = CInt(sf.Position - ObjectDistrictInfoOffset(n))
            Next
            For n As Integer = 0 To NumObject - 1
                ObjectDistrictInfoOffset(n) -= ObjectDistrictInfoDataAddress
            Next

            sf.Position = pAddress
            sf.WriteInt32(ViewDataAddress)
            sf.WriteInt32(ObjectInfoAddress)
            sf.WriteInt32(RenderIndexAddress)
            sf.WriteInt32(RenderDataAddress)
            sf.WriteInt32(ObjectDistrictInfoDataAddress)
            sf.WriteInt32(ObjectDistrictInfoDataAddress)

            For n As Integer = 0 To NumView - 1
                sf.Position = pViewInfo(n)
                ViewInfoRaw(n).WriteToFile(sf, VersionSign)
            Next
            For n As Integer = 0 To NumObject - 1
                sf.Position = pObjectInfo(n) + 20
                sf.WriteInt32(ObjectDistrictInfoOffset(n))
                sf.WriteInt32(ObjectDistrictInfoLength(n))
            Next
            For n As Integer = 0 To NumView - 1
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    sf.Position = pRenderIndex(n)(m) + 24
                    sf.WriteInt32(RenderDataOffsetInIndex(n)(m))
                    sf.WriteInt32(RenderDataLengthInIndex(n)(m))
                Next
            Next
        End Using
    End Sub

    Public ReadOnly Property NumObjectToRender(ByVal View As Int32) As Int32
        Get
            If RenderInfo Is Nothing Then Return 0
            If RenderInfo(View) Is Nothing Then Return 0
            Return RenderInfo(View).GetLength(0)
        End Get
    End Property

    'End Of Class
    'Start Of SubClasses

    Public Class ViewInfoBlock
        Public OffsetX As Single
        Public OffsetY As Single
        Public ViewLongitude As Single
        Public ViewLatitude As Single
    End Class

    Public Class ViewInfoBlockRaw
        Public OffsetX As Single
        Public OffsetY As Single
        Public ViewLongitude As Single
        Public ViewLatitude As Single
        Public NumObjectToRender As Int32
        Public RenderIndexOffset As Int32
        Public RenderIndexLength As Int32
        Public WaterMaskAddress As Int32
        Public RenderDataOffset As Int32
        Public RenderDataLength As Int32
        Public WaterMaskHighQualityAddress As Int32

        Public Const Length As Int32 = 52

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal VersionSign As Version)
            OffsetX = s.ReadSingle
            OffsetY = s.ReadSingle
            ViewLongitude = s.ReadSingle
            ViewLatitude = s.ReadSingle
            NumObjectToRender = s.ReadInt32
            RenderIndexOffset = s.ReadInt32
            RenderIndexLength = s.ReadInt32
            WaterMaskAddress = s.ReadInt32
            RenderDataOffset = s.ReadInt32
            RenderDataLength = s.ReadInt32
            If s.ReadInt32 <> 0 Then Throw New InvalidDataException
            If s.ReadInt32 <> 0 Then Throw New InvalidDataException
            Select Case VersionSign
                Case Version.Comm2
                    WaterMaskHighQualityAddress = s.ReadInt32
                Case Version.Comm2Demo
                    WaterMaskHighQualityAddress = 0
                Case Else
                    Throw New InvalidOperationException
            End Select
        End Sub
        Public Sub WriteToFile(ByVal s As StreamEx, ByVal VersionSign As Version)
            s.WriteSingle(OffsetX)
            s.WriteSingle(OffsetY)
            s.WriteSingle(ViewLongitude)
            s.WriteSingle(ViewLatitude)
            s.WriteInt32(NumObjectToRender)
            s.WriteInt32(RenderIndexOffset)
            s.WriteInt32(RenderIndexLength)
            s.WriteInt32(WaterMaskAddress)
            s.WriteInt32(RenderDataOffset)
            s.WriteInt32(RenderDataLength)
            s.WriteInt32(0)
            s.WriteInt32(0)
            Select Case VersionSign
                Case Version.Comm2
                    s.WriteInt32(WaterMaskHighQualityAddress)
                Case Version.Comm2Demo

                Case Else
                    Throw New InvalidOperationException
            End Select
        End Sub
    End Class

    Public Class ObjectInfoBlock
        Public Type As Int32
        Public n As Int32
        Public CenterX As Single
        Public CenterY As Single
        Public CenterZ As Single

        Public Const Length As Int32 = 28

        Public nx As Single
        Public ny As Single
        Public nz As Single
        Public D As Single
        Public X As Single()
        Public Y As Single()

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx)
            Type = s.ReadInt32
            n = s.ReadInt32
            CenterX = s.ReadSingle
            CenterY = s.ReadSingle
            CenterZ = s.ReadSingle
            s.Position += 8
        End Sub
        Public Sub Write(ByVal s As StreamEx)
            s.WriteInt32(Type)
            s.WriteInt32(n)
            s.WriteSingle(CenterX)
            s.WriteSingle(CenterY)
            s.WriteSingle(CenterZ)
            s.WriteInt32(0)
            s.WriteInt32(0)
        End Sub
        Public Sub ReadDistrictInfo(ByVal s As StreamEx)
            If n = 0 Then
                X = New Single(0) {s.ReadSingle}
                Y = New Single(0) {s.ReadSingle}
            Else
                If s.ReadInt32 <> n Then Throw New InvalidDataException
                nx = s.ReadSingle
                ny = s.ReadSingle
                nz = s.ReadSingle
                D = s.ReadSingle
                X = New Single(n - 1) {}
                Y = New Single(n - 1) {}
                For i As Integer = 0 To n - 1
                    X(i) = s.ReadSingle
                    Y(i) = s.ReadSingle
                Next
            End If
        End Sub
        Public Sub WriteDistrictInfo(ByVal s As StreamEx)
            If n = 0 Then
                s.WriteSingle(X(0))
                s.WriteSingle(Y(0))
            Else
                s.WriteInt32(n)
                s.WriteSingle(nx)
                s.WriteSingle(ny)
                s.WriteSingle(nz)
                s.WriteSingle(D)
                For i As Integer = 0 To n - 1
                    s.WriteSingle(X(i))
                    s.WriteSingle(Y(i))
                Next
            End If
        End Sub
    End Class

    Public Class WaterMaskInfoBlock
        Public WaterMask As Array2(Of Byte)

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx)
            Dim Width = s.ReadInt32()
            Dim Height = s.ReadInt32()
            Dim ByteWidths = s.ReadInt32Array(Height)
            WaterMask = Array2(Of Byte).Create(Width, Height)
            For y = 0 To Height - 1
                Dim ByteWidth = ByteWidths(y)
                Dim n = ByteWidth \ 2
                If n * 2 <> ByteWidth Then Throw New InvalidOperationException
                Dim x = 0
                For k = 0 To n - 1
                    Dim Value = s.ReadByte()
                    Dim Repeat = s.ReadByte()
                    For i = 0 To Repeat - 1
                        WaterMask(x + i, y) = Value
                    Next
                    x += Repeat
                Next
            Next
        End Sub
        Public Sub Write(ByVal s As StreamEx)
            Dim Width = WaterMask.GetLength(0)
            Dim Height = WaterMask.GetLength(1)
            Dim BytesList = New List(Of Byte())
            For y = 0 To Height - 1
                Dim Bytes = New List(Of Byte)
                Dim Value = CByte(0)
                Dim Repeat = 0
                For x = 0 To Width - 1
                    Dim CurrentValue = WaterMask(x, y)
                    If CurrentValue = Value Then
                        Repeat += 1
                        If Repeat = 255 Then
                            Bytes.Add(Value)
                            Bytes.Add(CByte(Repeat))
                            Repeat = 0
                        End If
                    Else
                        If Repeat <> 0 Then
                            Bytes.Add(Value)
                            Bytes.Add(CByte(Repeat))
                        End If
                        Value = CurrentValue
                        Repeat = 1
                    End If
                Next
                If Repeat <> 0 Then
                    Bytes.Add(Value)
                    Bytes.Add(CByte(Repeat))
                End If
                BytesList.Add(Bytes.ToArray())
            Next
            Dim ByteWidths = BytesList.Select(Function(l) l.Length).ToArray()
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            s.WriteInt32Array(ByteWidths)
            For Each Bytes In BytesList
                s.Write(Bytes)
            Next
        End Sub
    End Class
    Public Class WaterMaskInfoBlockSerializationData
        Public WaterMaskPath As String
    End Class
    Public Class WaterMaskInfoBlockEncoder
        Inherits Xml.Mapper(Of WaterMaskInfoBlock, WaterMaskInfoBlockSerializationData)

        Private Dict As Dictionary(Of WaterMaskInfoBlock, Integer)
        Private DictH As Dictionary(Of WaterMaskInfoBlock, Integer)
        Public Sub New()
        End Sub
        Public Sub New(ByVal BlockToViewIndexDict As Dictionary(Of WaterMaskInfoBlock, Integer), ByVal BlockHighToViewIndexDict As Dictionary(Of WaterMaskInfoBlock, Integer))
            Dict = BlockToViewIndexDict
            DictH = BlockHighToViewIndexDict
        End Sub

        Public Overrides Function GetMappedObject(ByVal o As WaterMaskInfoBlock) As WaterMaskInfoBlockSerializationData
            If o Is Nothing Then Return Nothing

            Dim WaterMaskPath As String
            If Dict.ContainsKey(o) Then
                Dim ViewIndex As Integer = Dict(o)
                WaterMaskPath = "WaterMask" & "_" & ViewIndex & ".png"
            Else
                Dim ViewIndex As Integer = DictH(o)
                WaterMaskPath = "WaterMaskHigh" & "_" & ViewIndex & ".png"
            End If

            Dim WaterMask As Array2(Of Byte) = o.WaterMask
            Dim Rectangle = New Int32(WaterMask.GetLength(0) - 1, WaterMask.GetLength(1) - 1) {}
            For y As Integer = 0 To WaterMask.GetUpperBound(1)
                For x As Integer = 0 To WaterMask.GetUpperBound(0)
                    Select Case WaterMask(x, y)
                        Case 0
                            Rectangle(x, y) = &HFF000000
                        Case 1
                            Rectangle(x, y) = &HFF7F7F7F
                        Case 2
                            Rectangle(x, y) = &HFFFFFFFF
                        Case Else
                            Throw New InvalidDataException
                    End Select
                Next
            Next
            Using RenderBitmap As New Bmp(WaterMask.GetLength(0), WaterMask.GetLength(1), 24)
                RenderBitmap.SetRectangle(0, 0, Rectangle)
                RenderBitmap.ToBitmap.Save(WaterMaskPath, System.Drawing.Imaging.ImageFormat.Png)
            End Using

            Return New WaterMaskInfoBlockSerializationData With {.WaterMaskPath = WaterMaskPath}
        End Function

        Public Overrides Function GetInverseMappedObject(ByVal o As WaterMaskInfoBlockSerializationData) As WaterMaskInfoBlock
            If o Is Nothing Then Return Nothing

            Dim WaterMaskPath = o.WaterMaskPath

            Dim Rectangle As Int32(,)
            Dim Width As Integer
            Dim Height As Integer
            Using Png = New Bitmap(WaterMaskPath)
                Width = Png.Width
                Height = Png.Height
                Using RenderBitmap As New Bitmap(Width, Height, Drawing.Imaging.PixelFormat.Format32bppArgb)
                    Using g = Graphics.FromImage(RenderBitmap)
                        Dim r = New Rectangle(0, 0, Width, Height)
                        g.DrawImage(Png, r, r, GraphicsUnit.Pixel)
                    End Using
                    Rectangle = RenderBitmap.GetRectangle(0, 0, Width, Height)
                End Using
            End Using
            Dim RectangleBytes As Byte(,)
            Using RenderBitmap As New Bmp(Width, Height, 4) With {.Palette = (New Int32() {&HFF000000, &HFF7F7F7F, &HFFFFFFFF}).Extend(16, &HFF000000)}
                RenderBitmap.SetRectangleFromARGB(0, 0, Rectangle)
                RectangleBytes = RenderBitmap.GetRectangleBytes(0, 0, Width, Height)
            End Using
            Dim WaterMask = Array2(Of Byte).Create(Width, Height)
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    WaterMask(x, y) = RectangleBytes(x, y)
                Next
            Next

            Return New WaterMaskInfoBlock With {.WaterMask = WaterMask}
        End Function
    End Class

    Public Class RenderInfoBlock
        Public ObjectIndex As Int32
        Public Const NumObject As Int32 = 1
        Public x As Int32
        Public y As Int32
        Public ReadOnly Property Width() As Int32
            Get
                If RenderMap Is Nothing Then Return 0
                Return RenderMap.GetLength(0)
            End Get
        End Property
        Public ReadOnly Property Height() As Int32
            Get
                If RenderMap Is Nothing Then Return 0
                Return RenderMap.GetLength(1)
            End Get
        End Property

        Public Const Length As Int32 = 32

        Public Const DataIdentifyingSign As String = "Mdlt"
        Public RenderMap As Array2(Of Byte)

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx)
            ObjectIndex = s.ReadInt32
            If s.ReadInt32 <> NumObject Then Throw New InvalidDataException
            x = s.ReadInt32
            y = s.ReadInt32
            Dim Width As Int32 = s.ReadInt32
            Dim Height As Int32 = s.ReadInt32
            RenderMap = Array2(Of Byte).Create(Width, Height)
            s.Position += 8
        End Sub
        Public Sub Write(ByVal s As StreamEx)
            s.WriteInt32(ObjectIndex)
            s.WriteInt32(NumObject)
            s.WriteInt32(x)
            s.WriteInt32(y)
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            s.WriteInt32(0)
            s.WriteInt32(0)
        End Sub
        Public Sub ReadData(ByVal s As StreamEx)
            For n As Integer = 0 To 3
                If s.ReadByte <> AscW(DataIdentifyingSign(n)) Then Throw New InvalidDataException
            Next
            If s.ReadInt32 <> Width Then Throw New InvalidDataException
            If s.ReadInt32 <> Height Then Throw New InvalidDataException
            Dim RemainedLength As Int32 = s.ReadInt32
            Dim SubRenderDataCount As Int32 = (Height + 7) \ 8
            If SubRenderDataCount = 0 Then Return

            Dim SubRenderDataOffset As Int32() = New Int32(SubRenderDataCount - 1) {}
            For n As Integer = 0 To SubRenderDataCount - 1
                SubRenderDataOffset(n) = s.ReadInt32
            Next
            RemainedLength -= SubRenderDataCount * 4
            If Height Mod 8 = 0 Then
                RemainedLength -= 4
                s.Position += 4 '此时有一个假索引
            End If

            Dim Orgin As Int32 = CInt(s.Position)
            Dim SubRenderData As SubRenderDataCodec
            If SubRenderDataCount > 0 Then
                For n As Integer = 0 To SubRenderDataCount - 2
                    s.Position = Orgin + SubRenderDataOffset(n)
                    SubRenderData = New SubRenderDataCodec(s, SubRenderDataOffset(n + 1) - SubRenderDataOffset(n), Width, 8)
                    ArrayCopy(SubRenderData.Decode, 0, RenderMap, 8 * n, 8)
                Next
                s.Position = Orgin + SubRenderDataOffset(SubRenderDataCount - 1)
                SubRenderData = New SubRenderDataCodec(s, RemainedLength - SubRenderDataOffset(SubRenderDataCount - 1), Width, (Height - 1) Mod 8 + 1)
                ArrayCopy(SubRenderData.Decode, 0, RenderMap, 8 * (SubRenderDataCount - 1), (Height - 1) Mod 8 + 1)
            End If
        End Sub
        Private Sub ArrayCopy(ByVal Source As Array2(Of Byte), ByVal SourceLine As Integer, ByVal Destination As Array2(Of Byte), ByVal DestinationLine As Integer, ByVal NumLine As Integer)
            For y As Integer = 0 To NumLine - 1
                For x As Integer = 0 To Source.GetLength(0) - 1
                    Destination(x, DestinationLine + y) = Source(x, SourceLine + y)
                Next
            Next
        End Sub
        Public Sub WriteData(ByVal s As StreamEx)
            For n As Integer = 0 To 3
                s.WriteByte(CByte(AscW(DataIdentifyingSign(n))))
            Next
            s.WriteInt32(Width)
            s.WriteInt32(Height)
            Dim Position As Int32 = CInt(s.Position)
            s.Position += 4

            Dim SubRenderDataCount As Int32 = (Height + 7) \ 8
            If SubRenderDataCount = 0 Then Return

            Dim SubRenderDataOffset As Int32() = New Int32(SubRenderDataCount - 1) {}
            s.Position += SubRenderDataCount * 4
            If Height Mod 8 = 0 Then s.WriteInt32(-1) '此时需补充一个假索引

            Dim Orgin As Int32 = CInt(s.Position)
            If SubRenderDataCount > 0 Then
                Dim TempRenderMap = Array2(Of Byte).Create(Width, 8)
                Dim SubRenderData As SubRenderDataCodec
                For n As Integer = 0 To SubRenderDataCount - 2
                    SubRenderDataOffset(n) = CInt(s.Position - Orgin)
                    ArrayCopy(RenderMap, 8 * n, TempRenderMap, 0, 8)
                    SubRenderData = SubRenderDataCodec.Encode(TempRenderMap)
                    SubRenderData.Write(s)
                Next
                TempRenderMap = Array2(Of Byte).Create(Width, ((Height - 1) Mod 8) + 1)
                SubRenderDataOffset(SubRenderDataCount - 1) = CInt(s.Position - Orgin)
                ArrayCopy(RenderMap, 8 * (SubRenderDataCount - 1), TempRenderMap, 0, (Height - 1) Mod 8 + 1)
                SubRenderData = SubRenderDataCodec.Encode(TempRenderMap)
                SubRenderData.Write(s)
            End If
            Dim RemainedLength As Int32 = CInt(s.Position - Orgin + SubRenderDataCount * 4)
            If Height Mod 8 = 0 Then RemainedLength += 4

            Dim BlockEndPosition As Integer = CInt(s.Position)
            s.Position = Position
            s.WriteInt32(RemainedLength)
            For n As Integer = 0 To SubRenderDataCount - 1
                s.WriteInt32(SubRenderDataOffset(n))
            Next
            s.Position = BlockEndPosition
        End Sub
    End Class
    Public Class RenderInfoBlockSerializationData
        Public ObjectIndex As Int32
        Public x As Int32
        Public y As Int32
        Public RenderMapPath As String
    End Class
    Public Class RenderInfoBlockEncoder
        Inherits Xml.Mapper(Of RenderInfoBlock, RenderInfoBlockSerializationData)

        Private Dict As Dictionary(Of RenderInfoBlock, Integer)
        Private Duplicated As Dictionary(Of String, Integer)
        Public Sub New()
        End Sub
        Public Sub New(ByVal BlockToViewIndexDict As Dictionary(Of RenderInfoBlock, Integer))
            Dict = BlockToViewIndexDict
            Duplicated = New Dictionary(Of String, Integer)
        End Sub

        Public Overrides Function GetMappedObject(ByVal o As RenderInfoBlock) As RenderInfoBlockSerializationData
            Dim ViewIndex As Integer = Dict(o)
            Dim RenderMapPath = o.ObjectIndex & "_" & ViewIndex & ".A.png"
            If Duplicated.ContainsKey(RenderMapPath) Then
                Dim r = o.ObjectIndex & "_" & ViewIndex & "_" & Duplicated(RenderMapPath) & ".A.png"
                Duplicated(RenderMapPath) += 1
                RenderMapPath = r
            Else
                Duplicated.Add(RenderMapPath, 1)
            End If

            Dim RenderMap As Array2(Of Byte) = o.RenderMap
            Dim Rectangle = New Int32(o.Width - 1, o.Height - 1) {}
            For y As Integer = 0 To RenderMap.GetUpperBound(1)
                For x As Integer = 0 To RenderMap.GetUpperBound(0)
                    Select Case RenderMap(x, y)
                        Case 0
                            Rectangle(x, y) = &HFF000000
                        Case 1
                            Rectangle(x, y) = &HFF7F7F7F
                        Case 2
                            Rectangle(x, y) = &HFFFFFFFF
                        Case Else
                            Throw New InvalidDataException
                    End Select
                Next
            Next
            Using RenderBitmap As New Bmp(o.Width, o.Height, 24)
                RenderBitmap.SetRectangle(0, 0, Rectangle)
                RenderBitmap.ToBitmap.Save(RenderMapPath, System.Drawing.Imaging.ImageFormat.Png)
            End Using

            Return New RenderInfoBlockSerializationData With {.ObjectIndex = o.ObjectIndex, .x = o.x, .y = o.y, .RenderMapPath = RenderMapPath}
        End Function

        Public Overrides Function GetInverseMappedObject(ByVal o As RenderInfoBlockSerializationData) As RenderInfoBlock
            Dim RenderMapPath = o.RenderMapPath

            Dim Rectangle As Int32(,)
            Dim Width As Integer
            Dim Height As Integer
            Using Png = New Bitmap(RenderMapPath)
                Width = Png.Width
                Height = Png.Height
                Using RenderBitmap As New Bitmap(Width, Height, Drawing.Imaging.PixelFormat.Format32bppArgb)
                    Using g = Graphics.FromImage(RenderBitmap)
                        Dim r = New Rectangle(0, 0, Width, Height)
                        g.DrawImage(Png, r, r, GraphicsUnit.Pixel)
                    End Using
                    Rectangle = RenderBitmap.GetRectangle(0, 0, Width, Height)
                End Using
            End Using
            Dim RectangleBytes As Byte(,)
            Using RenderBitmap As New Bmp(Width, Height, 4) With {.Palette = (New Int32() {&HFF000000, &HFF7F7F7F, &HFFFFFFFF}).Extend(16, &HFF000000)}
                RenderBitmap.SetRectangleFromARGB(0, 0, Rectangle)
                RectangleBytes = RenderBitmap.GetRectangleBytes(0, 0, Width, Height)
            End Using
            Dim RenderMap = Array2(Of Byte).Create(Width, Height)
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    RenderMap(x, y) = RectangleBytes(x, y)
                Next
            Next

            Return New RenderInfoBlock With {.ObjectIndex = o.ObjectIndex, .x = o.x, .y = o.y, .RenderMap = RenderMap}
        End Function
    End Class

    Public Class SubRenderDataCodec
        Private RenderData As Byte()
        Private Width As Int32
        Private Height As Int32

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal Length As Integer, ByVal Width As Integer, ByVal Height As Integer)
            RenderData = New Byte(Length - 1) {}
            For n As Integer = 0 To Length - 1
                RenderData(n) = s.ReadByte
            Next

            If Width < 0 OrElse Height < 0 OrElse Height > 8 Then Throw New InvalidDataException
            Me.Width = Width
            Me.Height = Height
        End Sub

        Public ReadOnly Property Length() As Int32
            Get
                If RenderData Is Nothing Then Return 0
                Return RenderData.GetLength(0)
            End Get
        End Property

        Public Sub Write(ByVal s As StreamEx)
            For n As Integer = 0 To RenderData.Length - 1
                s.WriteByte(RenderData(n))
            Next
        End Sub

        Private Class SpanDescriptor
            Public IsLineStart As Boolean
            Public IsLineEnd As Boolean
            Public StartIndex As Int32
            Public EndIndex As Int32
            Public StartAddition As Int32
            Public EndAddition As Int32
        End Class

        Public Function Decode() As Array2(Of Byte)
            Dim Src As New Queue(Of Byte)(RenderData)
            Dim Orginal = Array2(Of Byte).Create(Width, 8)

            Dim HeadByte As Byte = Src.Dequeue
            If Not HeadByte.Bit(0) Then
                Dim NumXBlock As Integer = (Width + 7) \ 8
                If NumXBlock <= 0 Then Return Orginal
                Dim Status = New Byte(NumXBlock - 1) {}
                Dim k = 2
                Dim StatusByte = HeadByte
                For n = 0 To NumXBlock - 1
                    If k >= 8 Then
                        k = 0
                        StatusByte = Src.Dequeue
                    End If
                    Status(n) = StatusByte.Bits(k + 1, k)
                    k += 2
                Next
                If Width Mod 32 = 24 Then Src.Dequeue() '此时需补充一个假索引
                For n = 0 To NumXBlock - 2
                    If Status(n) = 3 Then
                        For y As Integer = 0 To 7
                            Dim Data As Int16 = Src.Dequeue
                            Data = Data Or (CShort(Src.Dequeue) << 8)
                            For i As Integer = 0 To 7
                                Orginal(n * 8 + i, y) = CByte((Data >> (i * 2)) And 3)
                            Next
                        Next
                    Else
                        For y As Integer = 0 To 7
                            For i As Integer = 0 To 7
                                Orginal(n * 8 + i, y) = Status(n)
                            Next
                        Next
                    End If
                Next
                If Status(NumXBlock - 1) = 3 Then
                    For y As Integer = 0 To 7
                        Dim Data As Int16 = Src.Dequeue
                        Data = Data Or (CShort(Src.Dequeue) << 8)
                        For i As Integer = 0 To (Width - 1) Mod 8
                            Orginal((NumXBlock - 1) * 8 + i, y) = CByte((Data >> (i * 2)) And 3)
                        Next
                    Next
                Else
                    For y As Integer = 0 To 7
                        For i As Integer = 0 To (Width - 1) Mod 8
                            Orginal((NumXBlock - 1) * 8 + i, y) = Status(NumXBlock - 1)
                        Next
                    Next
                End If
            Else
                Dim LineSpans As SpanDescriptor() = Nothing
                Dim Num As Byte = 0
                For y = 0 To Height - 1
                    For n = 0 To Num - 1
                        Dim LineSpan = LineSpans(n)
                        With LineSpan
                            .StartIndex += .StartAddition
                            .EndIndex += .EndAddition
                        End With
                    Next

                    If HeadByte.Bit(y) Then
                        Dim StatusByte = Src.Dequeue
                        If StatusByte.Bit(0) Then
                            Num = StatusByte >> 1
                            StatusByte = 255
                            LineSpans = New SpanDescriptor(Num - 1) {}
                        End If

                        StatusByte >>= 1

                        For n = 0 To Num - 1
                            If CBool((StatusByte >> n) And 1) Then
                                Dim Data As Int32 = CInt(Src.Dequeue)
                                If CBool(Data And 1) Then
                                    Data = Data Or (CInt(Src.Dequeue) << 8)
                                    Data = Data Or (CInt(Src.Dequeue) << 16)
                                    Data = Data Or (CInt(Src.Dequeue) << 24)

                                    Dim LineSpan As New SpanDescriptor
                                    With LineSpan
                                        .IsLineStart = Data.Bit(3)
                                        .IsLineEnd = Data.Bit(2)
                                        .StartIndex = Data.Bits(31, 22)
                                        .EndIndex = Data.Bits(21, 12)
                                        .StartAddition = Data.Bits(11, 8) - 7
                                        .EndAddition = Data.Bits(7, 4) - 7
                                    End With
                                    LineSpans(n) = LineSpan
                                Else
                                    Dim LineSpan = LineSpans(n)
                                    With LineSpan
                                        .IsLineStart = Data.Bit(3)
                                        .IsLineEnd = Data.Bit(2)
                                        .StartAddition += Data.Bits(7, 6) - 1
                                        .EndAddition += Data.Bits(5, 4) - 1
                                    End With
                                End If
                            End If
                        Next
                    End If
                    For n = 0 To Num - 1
                        Dim LineSpan = LineSpans(n)
                        Dim StartIndex = LineSpan.StartIndex
                        Dim EndIndex = LineSpan.EndIndex
                        If StartIndex < 0 Then StartIndex = 0
                        If EndIndex > Width Then EndIndex = Width
                        If StartIndex >= EndIndex Then Continue For
                        If LineSpan.IsLineStart Then
                            Orginal(StartIndex, y) = 1
                            StartIndex += 1
                        End If
                        If LineSpan.IsLineEnd Then
                            Orginal(EndIndex - 1, y) = 1
                            EndIndex -= 1
                        End If
                        For i = StartIndex To EndIndex - 1
                            Orginal(i, y) = 2
                        Next
                    Next
                Next
            End If
            Return Orginal
        End Function

        Public Shared Function Encode(ByVal Orginal As Array2(Of Byte)) As SubRenderDataCodec
            Dim ret As New SubRenderDataCodec
            ret.Width = Orginal.GetLength(0)
            ret.Height = Orginal.GetLength(1)

            ret.RenderData = Encode0(Orginal)

            'Dim RenderData0 As Byte() = Encode0(Orginal)
            'Dim Renderdata1 As Byte() = Encode1(Orginal)
            'If Renderdata1 Is Nothing OrElse RenderData0.GetLength(0) <= Renderdata1.GetLength(0) Then
            '    ret.RenderData = RenderData0
            'Else
            '    ret.RenderData = Renderdata1
            'End If

            Return ret
        End Function

        Protected Shared Function Encode0(ByVal Orginal As Array2(Of Byte)) As Byte()
            Dim Width As Int32 = Orginal.GetLength(0)
            Dim Height As Int32 = Orginal.GetLength(1)
            Dim RenderData As New Queue(Of Byte)

            Dim NumXBlock As Integer = (Width + 7) \ 8
            If NumXBlock <= 0 Then
                Return New Byte() {0}
            End If

            Dim Status As Byte() = New Byte(NumXBlock - 1) {}

            Dim IsSame As Boolean
            Dim First As Byte
            For n As Integer = 0 To NumXBlock - 2
                IsSame = True
                First = Orginal(n * 8, 0)
                For y As Integer = 0 To Height - 1
                    For x As Integer = n * 8 To n * 8 + 7
                        If Orginal(x, y) >= 3 Then Throw New InvalidDataException
                        If Orginal(x, y) <> First Then
                            IsSame = False
                            Exit For
                        End If
                    Next
                Next
                If IsSame Then
                    Status(n) = First
                Else
                    Status(n) = 3
                End If
            Next
            IsSame = True
            First = Orginal((NumXBlock - 1) * 8, 0)
            For y As Integer = 0 To Height - 1
                For x As Integer = (NumXBlock - 1) * 8 To Width - 1
                    If Orginal(x, y) <> First Then
                        IsSame = False
                        Exit For
                    End If
                Next
            Next
            If IsSame Then
                Status(NumXBlock - 1) = First
            Else
                Status(NumXBlock - 1) = 3
            End If

            Dim k As Integer = 2
            Dim StatusByte As Byte = 0
            For n As Integer = 0 To NumXBlock - 1
                StatusByte = StatusByte Or (Status(n) << k)
                k += 2
                If k >= 8 Then
                    k = 0
                    RenderData.Enqueue(StatusByte)
                    StatusByte = 0
                End If
            Next
            If k > 0 AndAlso k < 8 Then
                RenderData.Enqueue(StatusByte)
            End If
            If Width Mod 32 = 24 Then RenderData.Enqueue(0) '此时需补充一个假索引

            For n As Integer = 0 To NumXBlock - 2
                If Status(n) = 3 Then
                    For y As Integer = 0 To Height - 1
                        Dim Data As Int16 = 0
                        For i As Integer = 0 To 7
                            Data = Data Or (CShort(Orginal(n * 8 + i, y)) << (i * 2))
                        Next
                        RenderData.Enqueue(CByte(Data And &HFF))
                        RenderData.Enqueue(CByte(((Data And &HFF00) >> 8) And &HFF))
                    Next
                    For y As Integer = Height To 7
                        RenderData.Enqueue(0)
                        RenderData.Enqueue(0)
                    Next
                End If
            Next
            If Status(NumXBlock - 1) = 3 Then
                For y As Integer = 0 To Height - 1
                    Dim Data As Int16 = 0
                    For i As Integer = 0 To (Width - 1) Mod 8
                        Data = Data Or (CShort(Orginal((NumXBlock - 1) * 8 + i, y)) << (i * 2))
                    Next
                    RenderData.Enqueue(CByte(Data And &HFF))
                    RenderData.Enqueue(CByte(((Data And &HFF00) >> 8) And &HFF))
                Next
                For y As Integer = Height To 7
                    RenderData.Enqueue(0)
                    RenderData.Enqueue(0)
                Next
            End If
            Return RenderData.ToArray
        End Function
        Protected Shared Function Encode1(ByVal Orginal As Byte(,)) As Byte()
            'TODO 无法使用该压缩方法，可能是因为格式没有完全正确分析
            Dim Width As Int32 = Orginal.GetLength(0)
            Dim Height As Int32 = Orginal.GetLength(1)
            If Orginal.GetLength(0) > 1024 Then
                Return Nothing
            End If
            Dim RenderData As New Queue(Of Byte)

            Dim HeadByte As Byte = 255
            For y As Integer = 1 To Height - 1
                Dim IsSame As Boolean = True
                For x As Integer = 0 To Width - 1
                    If Orginal(x, y) <> Orginal(x, y - 1) Then
                        IsSame = False
                        Exit For
                    End If
                Next
                If IsSame Then
                    HeadByte = HeadByte And Not (CByte(1) << y)
                End If
            Next
            RenderData.Enqueue(HeadByte)

            Dim Tar As New List(Of Int32)

            For y As Integer = 0 To Height - 1
                If CBool(HeadByte And (CByte(1) << y)) Then
                    Dim n As Integer = 0
                    Dim Lower As Integer = 0
                    Dim SpaceFlag As Boolean = True
                    Dim HalfFlag As Boolean = False
                    For x As Integer = 0 To Width - 1
                        If SpaceFlag Then
                            Select Case Orginal(x, y)
                                Case 0
                                Case 1
                                    SpaceFlag = False
                                    HalfFlag = True
                                    Lower = x
                                Case 2
                                    SpaceFlag = False
                                    HalfFlag = False
                                    Lower = x
                            End Select
                        Else
                            Select Case Orginal(x, y)
                                Case 0
                                    If HalfFlag Then
                                        Tar.Add(&H100)
                                    Else
                                        Tar.Add(0)
                                    End If
                                    Tar.Add(Lower)
                                    Tar.Add(x)
                                    Tar.Add(0)
                                    Tar.Add(0)
                                    SpaceFlag = True
                                    n += 1
                                Case 1
                                    If HalfFlag Then
                                        Tar.Add(&H101)
                                    Else
                                        Tar.Add(1)
                                    End If
                                    Tar.Add(Lower)
                                    Tar.Add(x)
                                    Tar.Add(0)
                                    Tar.Add(0)
                                    SpaceFlag = True
                                    n += 1
                                Case 2
                            End Select
                        End If
                    Next
                    If Not SpaceFlag Then
                        If HalfFlag Then
                            Tar.Add(&H100)
                        Else
                            Tar.Add(0)
                        End If
                        Tar.Add(Lower)
                        Tar.Add(Width - 1)
                        Tar.Add(0)
                        Tar.Add(0)
                        n += 1
                    End If

                    If n > 7 Then Return Nothing
                    RenderData.Enqueue(CByte((n << 1) Or 1))

                    For i As Integer = 0 To n - 1
                        Dim Data As Integer = ((Tar(i * 5 + 1) And &H3FF) << 22) Or ((Tar(i * 5 + 2) And &H3FF) << 12)
                        Data = Data Or ((Tar(i * 5) And 1) << 3)
                        Data = Data Or (((Tar(i * 5) >> 8) And 1) << 2)
                        Data = Data Or (((Tar(i * 5 + 3) + 7) And 15) << 8)
                        Data = Data Or (((Tar(i * 5 + 4) + 7) And 15) << 4)
                        Data = Data Or 1
                        RenderData.Enqueue(CByte(Data And &HFF))
                        RenderData.Enqueue(CByte((Data >> 8) And &HFF))
                        RenderData.Enqueue(CByte((Data >> 16) And &HFF))
                        RenderData.Enqueue(CByte((Data >> 24) And &HFF))
                    Next
                    Tar.Clear()
                End If
            Next
            Return RenderData.ToArray
        End Function
    End Class
End Class
