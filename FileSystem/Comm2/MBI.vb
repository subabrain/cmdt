'==========================================================================
'
'  File:        MBI.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: MBI文件类
'  Version:     2018.11.24.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Firefly
Imports Firefly.TextEncoding.TextEncoding
Imports Firefly.Imaging
Imports Firefly.Imaging.Gif

''' <summary>MBI文件类</summary>
Public Class MBI
    Public VersionSign As Version
    Public Enum Version
        Comm2Demo = &H4E '"N"
        Comm2 = &H31 ' "1"
        Comm3 = &H32 ' "2"
    End Enum
    Public Const Identifier As String = "IBM"
    Public ReadOnly Property NumPoint() As Int32
        Get
            Return Points.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property NumDistrict() As Int32
        Get
            Return Districts.GetLength(0)
        End Get
    End Property
    Public Points As PointInfo()
    Public Districts As DistrictInfo()
    Public ReadOnly Property NumObject() As Int32
        Get
            Return Objects.GetLength(0)
        End Get
    End Property
    Public Objects As ObjectInfo()
    Public ReadOnly Property NumTexture() As Int32
        Get
            Return Textures.GetLength(0)
        End Get
    End Property
    Public Textures As Texture()

    Public Sub New()
    End Sub
    Public Shared Function Open(ByVal Path As String, Optional ByVal EnableCommPExtension As Boolean = False) As MBI
        Dim mf As New MBI
        With mf
            Using s As New StreamEx(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim v As Version = CType(s.ReadByte, Version)
                If v <> Version.Comm2Demo AndAlso v <> Version.Comm2 AndAlso v <> Version.Comm3 Then Throw New InvalidDataException
                .VersionSign = v
                For n As Integer = 0 To Identifier.Length - 1
                    If s.ReadByte() <> AscW(Identifier(n)) Then
                        s.Close()
                        Throw New InvalidDataException
                    End If
                Next

                Dim NumPoint As Int32 = s.ReadInt32
                Dim NumDistrict As Int32 = s.ReadInt32

                .Points = New PointInfo(NumPoint - 1) {}
                For n As Integer = 0 To NumPoint - 1
                    .Points(n) = New PointInfo(s)
                Next
                .Districts = New DistrictInfo(NumDistrict - 1) {}
                For n As Integer = 0 To NumDistrict - 1
                    .Districts(n) = New DistrictInfo(s, .VersionSign, EnableCommPExtension)
                Next

                If v <> Version.Comm2Demo Then
                    Dim NumObject As Int32 = s.ReadInt32
                    .Objects = New ObjectInfo(NumObject - 1) {}
                    For n As Integer = 0 To NumObject - 1
                        .Objects(n) = New ObjectInfo(s)
                    Next
                Else
                    Dim NumObject = 1
                    .Objects = New ObjectInfo(NumObject - 1) {}
                    .Objects(0) = New ObjectInfo With {.ObjectName = "Comm2DemoDefaultObj", .StartDistrictIndex = 0, .NextStartDistrictIndex = NumDistrict}
                End If

                Dim NumTexture As Int32 = s.ReadInt32
                .Textures = New Texture(NumTexture - 1) {}
                For n As Integer = 0 To NumTexture - 1
                    .Textures(n) = New Texture(s, .VersionSign)
                Next
            End Using
        End With
        Return mf
    End Function
    Public Sub ExportToGif(ByVal PicIndex As Integer, ByVal Path As String)
        Dim i As Texture = Textures(PicIndex)
        Dim g As New GifImageBlock(i.Rectangle, i.Palette)

        Dim gf As New Gif(g)
        gf.WriteToFile(Path)
    End Sub
    Public Sub RemoveObject(ByVal Index As Integer)
        If Objects Is Nothing Then Throw New InvalidOperationException
        If Index < 0 OrElse Index > Objects.GetUpperBound(0) Then Throw New ArgumentOutOfRangeException
        Dim ObjectIndex As Integer = Index
        Dim Minus As Integer = Objects(Index).NextStartDistrictIndex - Objects(Index).StartDistrictIndex
        Dim d As DistrictInfo() = New DistrictInfo(Districts.GetUpperBound(0) - Minus) {}
        For n As Integer = 0 To Objects(Index).StartDistrictIndex - 1
            d(n) = Districts(n)
        Next
        For n As Integer = Objects(Index).NextStartDistrictIndex To Districts.GetUpperBound(0)
            d(n - Minus) = Districts(n)
        Next
        Districts = d
        For n As Integer = ObjectIndex To Objects.GetUpperBound(0) - 1
            Objects(n).NextStartDistrictIndex -= Minus
            Objects(n + 1).StartDistrictIndex -= Minus
        Next
        Objects(Objects.GetUpperBound(0)).NextStartDistrictIndex -= Minus
        Dim o As ObjectInfo() = New ObjectInfo(Objects.GetUpperBound(0) - 1) {}
        For n As Integer = 0 To Index - 1
            o(n) = Objects(n)
        Next
        For n As Integer = Index + 1 To Objects.GetUpperBound(0)
            o(n - 1) = Objects(n)
        Next
        Objects = o
    End Sub
    Public Sub RemoveComm3Feature()
        Dim q As New Queue(Of DistrictInfo)
        Dim ObjectIndex As Integer = 0
        Dim Minus As Integer = 0
        For n As Integer = 0 To NumDistrict - 1
            Dim d As DistrictInfo = Districts(n)
            If d.Attribute = 0 OrElse (d.Attribute = 1 AndAlso Not IsComm3Feature(d)) OrElse d.Attribute = 4 Then
                q.Enqueue(Districts(n))
            Else
                While n >= Objects(ObjectIndex).NextStartDistrictIndex
                    Objects(ObjectIndex).NextStartDistrictIndex -= Minus
                    ObjectIndex += 1
                    Objects(ObjectIndex).StartDistrictIndex -= Minus
                End While
                Minus += 1
            End If
        Next
        For n As Integer = ObjectIndex To Objects.GetUpperBound(0) - 1
            Objects(n).NextStartDistrictIndex -= Minus
            Objects(n + 1).StartDistrictIndex -= Minus
        Next
        Objects(Objects.GetUpperBound(0)).NextStartDistrictIndex -= Minus
        Districts = q.ToArray
        For n As Integer = 0 To Textures.GetUpperBound(0)
            If Textures(n).Palette(0) = &HFF00FF Then Textures(n).Palette(0) = 0
        Next
    End Sub
    Public Sub EnsureInBox(ByRef x As Integer, ByRef y As Integer, ByVal Width As Integer, ByVal Height As Integer)
        If x < 0 Then x = 0
        If x > Width - 1 Then x = Width - 1
        If y < 0 Then y = 0
        If y > Height - 1 Then y = Height - 1
    End Sub
    ''' <summary>判断区域是否只能在盟军3中才能正常显示，包括各种使用了透明效果的花草和小柱子，不包括带透明效果的门</summary>
    ''' <remarks>通过判断是否有一个顶点不透明来判断，一个顶点不透明通过顶点和顶点所在两条边上靠近顶点的两点的像素是否全为不透明来判断</remarks>
    Public Function IsComm3Feature(ByVal d As DistrictInfo) As Boolean
        If d Is Nothing Then Throw New ArgumentNullException
        If d.n <= 2 Then Throw New ArgumentException
        Dim Image As Texture = Textures(d.TextureID)

        Dim PrevPicX As Integer = CInt(Floor(d.U(d.n - 1) * Image.Width))
        Dim PrevPicY As Integer = CInt(Floor(d.V(d.n - 1) * Image.Height))
        Dim PicX As Integer = CInt(Floor(d.U(0) * Image.Width))
        Dim PicY As Integer = CInt(Floor(d.V(0) * Image.Height))
        Dim NextPicX As Integer = CInt(Floor(d.U(1) * Image.Width))
        Dim NextPicY As Integer = CInt(Floor(d.V(1) * Image.Height))
        For n As Integer = 0 To d.n - 1
            Dim Support1PicX As Integer = (PrevPicX + PicX * 4) \ 5
            Dim Support1PicY As Integer = (PrevPicY + PicY * 4) \ 5
            Dim Support2PicX As Integer = (NextPicX + PicX * 4) \ 5
            Dim Support2PicY As Integer = (NextPicY + PicY * 4) \ 5

            EnsureInBox(Support1PicX, Support1PicY, Image.Width, Image.Height)
            EnsureInBox(PicX, PicY, Image.Width, Image.Height)
            EnsureInBox(Support2PicX, Support2PicY, Image.Width, Image.Height)
            If (Image.Palette(Image.Rectangle(Support1PicX, Support1PicY)) <> &HFF00FF) _
            AndAlso (Image.Palette(Image.Rectangle(PicX, PicY)) <> &HFF00FF) _
            AndAlso (Image.Palette(Image.Rectangle(Support2PicX, Support2PicY)) <> &HFF00FF) Then
                Return False
            End If
            PrevPicX = PicX
            PrevPicY = PicY
            PicX = NextPicX
            PicY = NextPicY
            NextPicX = CInt(Floor(d.U((n + 2) Mod d.n) * Image.Width))
            NextPicY = CInt(Floor(d.V((n + 2) Mod d.n) * Image.Height))
        Next
        Return True
    End Function
    Public Sub WriteToFile(ByVal Path As String, ByVal VersionSign As Version, Optional ByVal EnableCommPExtension As Boolean = False)
        If (VersionSign <> Version.Comm2Demo) AndAlso (VersionSign <> Version.Comm2) AndAlso (VersionSign <> Version.Comm3) Then
            Throw New InvalidDataException
        End If

        Using s As New StreamEx(Path, FileMode.Create)
            s.WriteByte(CByte(VersionSign))
            For n As Integer = 0 To Identifier.Length - 1
                s.WriteByte(CByte(AscW(Identifier(n))))
            Next

            s.WriteInt32(NumPoint)
            s.WriteInt32(NumDistrict)

            For n As Integer = 0 To NumPoint - 1
                Points(n).WriteTo(s)
            Next
            For n As Integer = 0 To NumDistrict - 1
                Districts(n).WriteTo(s, VersionSign, EnableCommPExtension)
            Next

            If VersionSign <> Version.Comm2Demo Then
                s.WriteInt32(NumObject)
                For n As Integer = 0 To NumObject - 1
                    Objects(n).WriteTo(s)
                Next
            End If

            s.WriteInt32(NumTexture)
            For n As Integer = 0 To NumTexture - 1
                Textures(n).WriteTo(s, VersionSign)
            Next
        End Using
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class PointInfo
        Public x As Single
        Public y As Single
        Public z As Single

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx)
            x = s.ReadSingle
            y = s.ReadSingle
            z = s.ReadSingle
        End Sub
        Public Sub WriteTo(ByVal s As StreamEx)
            s.WriteSingle(x)
            s.WriteSingle(y)
            s.WriteSingle(z)
        End Sub
    End Class
    Public Class DistrictInfo
        Public Attribute As Byte
        Public n As Byte
        Public TextureID As Int32
        Public Point As Int32()
        Public U As Single()
        Public V As Single()
        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal Version As MBI.Version, ByVal EnableCommPExtension As Boolean)
            If Version = Version.Comm2Demo Then
                n = CByte(s.ReadInt32)
                TextureID = s.ReadInt32
                Point = New Int32(n - 1) {}
                U = New Single(n - 1) {}
                V = New Single(n - 1) {}
                For i As Integer = 0 To n - 1
                    Point(i) = s.ReadInt32
                    U(i) = s.ReadSingle
                    V(i) = s.ReadSingle
                Next
            Else
                n = s.ReadByte
                If Version = MBI.Version.Comm3 Then
                    Attribute = n.Bits(7, 4)
                    n = n.Bits(3, 0)
                End If
                TextureID = s.ReadByte
                If EnableCommPExtension Then
                    TextureID = CInt(n.Bits(7, 4)).ConcatBits(TextureID, 8)
                    n = n.Bits(3, 0)
                End If
                Point = New Int32(n - 1) {}
                U = New Single(n - 1) {}
                V = New Single(n - 1) {}
                For i As Integer = 0 To n - 1
                    Point(i) = s.ReadUInt16
                    U(i) = CSng(s.ReadInt16 / 4096)
                    V(i) = CSng(s.ReadInt16 / 4096)
                Next
            End If
        End Sub
        Public Sub WriteTo(ByVal s As StreamEx, ByVal Version As MBI.Version, ByVal EnableCommPExtension As Boolean)
            If Version = MBI.Version.Comm2Demo Then
                s.WriteInt32(n)

                s.WriteInt32(TextureID)
                For i As Integer = 0 To n - 1
                    s.WriteInt32(Point(i))
                    s.WriteSingle(U(i))
                    s.WriteSingle(V(i))
                Next
            Else
                If Version = MBI.Version.Comm3 Then
                    If n >= 16 Then Throw New InvalidDataException
                    If TextureID >= &H100 Then Throw New InvalidDataException
                    s.WriteByte((Attribute << 4) Or n)
                    s.WriteByte(CByte(TextureID))
                ElseIf EnableCommPExtension Then
                    '对东风4096张贴图修改的支持(CommII Plus, Comm2P)
                    If n >= 16 Then Throw New InvalidDataException
                    If TextureID >= &H1000 Then Throw New InvalidDataException
                    s.WriteByte(CByte(TextureID.Bits(11, 8).ConcatBits(n, 4)))
                    s.WriteByte(CByte(TextureID.Bits(7, 0)))
                Else
                    If n >= &H100 Then Throw New InvalidDataException
                    If TextureID >= &H100 Then Throw New InvalidDataException
                    s.WriteByte(n)
                    s.WriteByte(CByte(TextureID))
                End If

                For i As Integer = 0 To n - 1
                    If EnableCommPExtension Then
                        '对东风65536顶点数修改的支持(CommII Plus, Comm2P)
                        s.WriteUInt16(CUShort(Point(i)))
                    Else
                        s.WriteInt16(CShort(Point(i)))
                    End If
                    s.WriteInt16(CShort(U(i) * 4096))
                    s.WriteInt16(CShort(V(i) * 4096))
                Next
            End If
        End Sub
    End Class
    Public Class ObjectInfo
        Public ObjectName As String
        Public StartDistrictIndex As Int32
        Public NextStartDistrictIndex As Int32
        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx)
            ObjectName = s.ReadString(44, Windows1252)
            StartDistrictIndex = s.ReadInt32
            NextStartDistrictIndex = s.ReadInt32
        End Sub
        Public Sub WriteTo(ByVal s As StreamEx)
            s.WriteString(ObjectName, 44, Windows1252)
            s.WriteInt32(StartDistrictIndex)
            s.WriteInt32(NextStartDistrictIndex)
        End Sub
    End Class
    Public Class Texture
        Public Unknown As Int32
        Public ReadOnly Property Width() As Int32
            Get
                Return Rectangle.GetLength(0)
            End Get
        End Property
        Public ReadOnly Property Height() As Int32
            Get
                Return Rectangle.GetLength(1)
            End Get
        End Property
        Public Name As String
        Public Const PaletteCount As Int32 = 256
        Public Palette As Int32()
        Public Rectangle As Byte(,)
        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx, ByVal Version As MBI.Version)
            Unknown = s.ReadInt32
            Dim Width As Int32 = s.ReadInt32
            Dim Height As Int32 = s.ReadInt32

            If Version = MBI.Version.Comm3 Then Name = s.ReadString(32, Windows1252)

            Palette = New Int32(PaletteCount - 1) {}
            For n As Integer = 0 To PaletteCount - 1
                Dim c As Int32 = s.ReadByte
                c = (c << 8) Or s.ReadByte
                c = (c << 8) Or s.ReadByte
                Palette(n) = c
            Next

            Rectangle = New Byte(Width - 1, Height - 1) {}
            For m As Integer = 0 To Height - 1
                For n As Integer = 0 To Width - 1
                    Rectangle(n, m) = s.ReadByte
                Next
            Next
        End Sub
        Public Sub WriteTo(ByVal s As StreamEx, ByVal Version As MBI.Version)
            s.WriteInt32(Unknown)
            s.WriteInt32(Width)
            s.WriteInt32(Height)

            If Version = MBI.Version.Comm3 Then s.WriteString(Name, 32, Windows1252)

            For n As Integer = 0 To PaletteCount - 1
                Dim c As Int32 = Palette(n)
                s.WriteByte(CByte((c >> 16) And &HFF))
                s.WriteByte(CByte((c >> 8) And &HFF))
                s.WriteByte(CByte(c And &HFF))
            Next

            For m As Integer = 0 To Height - 1
                For n As Integer = 0 To Width - 1
                    s.WriteByte(Rectangle(n, m))
                Next
            Next
        End Sub
    End Class

    Public Sub ToObj(ByVal ObjPath As String)
        With New Exporter
            .ToObj(ObjPath, Me)
        End With
    End Sub

    Public Shared Function FromObj(ByVal ObjPath As String) As MBI
        Return (New Importer).FromObj(ObjPath)
    End Function

    Private Class Exporter
        Public Sub ToObj(ByVal ObjPath As String, ByVal m As MBI)
            Dim Dir As String = GetFileDirectory(ObjPath)
            If Not Directory.Exists(Dir) Then Directory.CreateDirectory(Dir)

            Dim MtlName As String = GetMainFileName(ObjPath)
            Dim MtlBlocks As New List(Of String)
            For n = 0 To m.Textures.Length - 1
                Dim mt As New StringBuilder
                m.ExportToGif(n, GetPath(Dir, n & ".gif"))
                mt.AppendLine(String.Format("newmtl {0}", n))
                mt.AppendLine("illum 0")
                mt.AppendLine(String.Format("map_Kd {0}", n & ".gif"))
                mt.AppendLine("Ka 0.2 0.2 0.2")
                mt.AppendLine("Kd 0.8 0.8 0.8")
                MtlBlocks.Add(mt.ToString)
            Next
            Using Mtl As New StreamEx(GetPath(Dir, MtlName & ".mtl"), FileMode.Create, FileAccess.ReadWrite)
                Mtl.Write(System.Text.Encoding.UTF8.GetBytes(String.Join(System.Environment.NewLine, MtlBlocks.ToArray)))
            End Using

            Dim Blocks As New List(Of String)
            Dim h As New StringBuilder
            h.AppendLine(String.Format("# NumPoint: {0}", m.NumPoint))
            h.AppendLine(String.Format("# NumDistrict: {0}", m.NumDistrict))
            h.AppendLine(String.Format("# NumObject: {0}", m.NumObject))
            h.AppendLine(String.Format("# NumTexture: {0}", m.NumTexture))
            Blocks.Add(h.ToString)

            Blocks.Add(String.Format("mtllib {0}", MtlName & ".mtl") & System.Environment.NewLine)

            Dim v As New StringBuilder
            For n = 0 To m.NumPoint - 1
                Dim p = m.Points(n)
                v.AppendLine(String.Format(System.Globalization.NumberFormatInfo.InvariantInfo, "v {0:r} {1:r} {2:r}", p.x, p.y, p.z))
            Next

            Blocks.Add(v.ToString)

            Dim vt As New StringBuilder
            Dim vtIndex As Integer
            Dim f As New StringBuilder
            For n = 0 To m.NumObject - 1
                Dim o = m.Objects(n)
                f.AppendLine(String.Format("g {0}", o.ObjectName))
                Dim CurrentTexture As Integer = -1
                For i = o.StartDistrictIndex To o.NextStartDistrictIndex - 1
                    Dim d = m.Districts(i)
                    If d.TextureID <> CurrentTexture Then
                        f.AppendLine(String.Format("usemtl {0}", d.TextureID))
                        CurrentTexture = d.TextureID
                    End If
                    If d.n > 0 Then
                        f.Append("f")
                        For k = d.n - 1 To 0 Step -1
                            f.Append(String.Format(" {0}/{1}", d.Point(k) + 1, vtIndex + 1))
                            vt.AppendLine(String.Format(System.Globalization.NumberFormatInfo.InvariantInfo, "vt {0:r} {1:r}", d.U(k), 1.0F - d.V(k)))
                            vtIndex += 1
                        Next
                        f.AppendLine()
                    End If
                Next
            Next

            Blocks.Add(vt.ToString)
            Blocks.Add(f.ToString)

            Using Obj As New StreamEx(ObjPath, FileMode.Create, FileAccess.ReadWrite)
                Obj.Write(System.Text.Encoding.UTF8.GetBytes(String.Join(System.Environment.NewLine, Blocks.ToArray)))
            End Using
        End Sub
    End Class
    Private Class Importer
        Private Shared Num As String = "(\d|\-|\.|e|E)+"
        Private Shared RegexV As New Regex("^v( +(?<Coordinate>" & Num & "))+$", RegexOptions.ExplicitCapture)
        Private Shared RegexVT As New Regex("^vt( +(?<Coordinate>" & Num & "))+$", RegexOptions.ExplicitCapture)
        Private Shared RegexF As New Regex("^f( +((?<v>" & Num & ")(/(?<vt>" & Num & "))?(/(?<vt>" & Num & "))?))+$", RegexOptions.ExplicitCapture)
        Private Shared RegexG As New Regex("^g +(?<Name>.+)$", RegexOptions.ExplicitCapture)
        Private Shared RegexMtllib As New Regex("^mtllib +(?<Path>.+)$", RegexOptions.ExplicitCapture)
        Private Shared RegexUsemtl As New Regex("^usemtl +(?<Name>.+)$", RegexOptions.ExplicitCapture)
        Private Shared RegexNewmtl As New Regex("^newmtl +(?<Name>.+)$", RegexOptions.ExplicitCapture)
        Private Shared RegexImage As New Regex("^map_Kd +(?<ImagePath>.+)$", RegexOptions.ExplicitCapture)

        Dim TextureSet As New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)
        Dim TexturePathSet As New List(Of String)

        Dim Points As New List(Of MBI.PointInfo)
        Dim U As New List(Of Single)
        Dim V As New List(Of Single)
        Dim Districts As New List(Of MBI.DistrictInfo)
        Dim Objects As New List(Of MBI.ObjectInfo)
        Dim Textures As New List(Of MBI.Texture)
        Dim ObjectSet As New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)

        Dim CurrentTextureId As Int32
        Dim CurrentObject As Integer

        Dim Line As String
        Public Function FromObj(ByVal ObjPath As String) As MBI
            Dim PreviousCurrentDirectory As String = System.Environment.CurrentDirectory
            Dim AbsObjPath = GetAbsolutePath(ObjPath, PreviousCurrentDirectory)
            System.Environment.CurrentDirectory = GetFileDirectory(ObjPath)

            Dim LastMatch As Integer
            Using Obj As New StreamReader(AbsObjPath, System.Text.Encoding.UTF8)
                While Not Obj.EndOfStream
                    Line = Obj.ReadLine()
                    If Line <> "" Then Line = Line.Trim

                    Select Case LastMatch
                        Case 0
                            If CompareMtllib() Then Continue While
                        Case 1
                            If CompareV() Then Continue While
                        Case 2
                            If CompareVT() Then Continue While
                        Case 3
                            If CompareF() Then Continue While
                        Case 4
                            If CompareG() Then Continue While
                        Case 5
                            If CompareUsemtl() Then Continue While
                    End Select

                    If LastMatch <> 0 Then
                        If CompareMtllib() Then
                            LastMatch = 0
                            Continue While
                        End If
                    End If

                    If LastMatch <> 1 Then
                        If CompareV() Then
                            LastMatch = 1
                            Continue While
                        End If
                    End If

                    If LastMatch <> 2 Then
                        If CompareVT() Then
                            LastMatch = 2
                            Continue While
                        End If
                    End If

                    If LastMatch <> 3 Then
                        If CompareF() Then
                            LastMatch = 3
                            Continue While
                        End If
                    End If

                    If LastMatch <> 4 Then
                        If CompareG() Then
                            LastMatch = 4
                            Continue While
                        End If
                    End If

                    If LastMatch <> 5 Then
                        If CompareUsemtl() Then
                            LastMatch = 5
                            Continue While
                        End If
                    End If
                End While

                If Objects.Count > 0 Then Objects(Objects.Count - 1).NextStartDistrictIndex = Districts.Count
            End Using

            System.Environment.CurrentDirectory = PreviousCurrentDirectory
            Return New MBI With {.VersionSign = MBI.Version.Comm2, .Points = Points.ToArray, .Districts = Districts.ToArray, .Objects = Objects.ToArray, .Textures = Textures.ToArray}
        End Function

        Private Function CompareMtllib() As Boolean
            Dim MatchMtllib = RegexMtllib.Match(Line)
            If MatchMtllib.Success Then
                Dim MtlPath = MatchMtllib.Result("${Path}")
                Using Mtl As New StreamReader(MtlPath, System.Text.Encoding.UTF8)
                    While Not Mtl.EndOfStream
                        Dim LineMtl = Mtl.ReadLine()
                        If LineMtl <> "" Then LineMtl = LineMtl.Trim

                        Dim MatchNewmtl = RegexNewmtl.Match(LineMtl)
                        If MatchNewmtl.Success Then
                            If TextureSet.Count <> TexturePathSet.Count Then Throw New InvalidDataException
                            TextureSet.Add(MatchNewmtl.Result("${Name}"), TexturePathSet.Count)
                            Continue While
                        End If
                        Dim MatchImage = RegexImage.Match(LineMtl)
                        If MatchImage.Success Then
                            If TextureSet.Count <> TexturePathSet.Count + 1 Then Throw New InvalidDataException
                            Dim Path = MatchImage.Result("${ImagePath}")
                            TexturePathSet.Add(Path)
                            Dim gif As New Gif(Path)
                            Dim t As New MBI.Texture With {.Name = Path, .Palette = gif.Flame(0).Palette, .Rectangle = gif.Flame(0).Rectangle}
                            If t.Palette Is Nothing Then t.Palette = gif.Palette
                            If t.Palette IsNot Nothing Then
                                t.Palette = t.Palette.Extend(MBI.Texture.PaletteCount, 0)
                            End If
                            Textures.Add(t)
                        End If
                    End While
                    If TextureSet.Count <> TexturePathSet.Count Then Throw New InvalidDataException
                End Using
                Return True
            End If
            Return False
        End Function
        Private Function CompareV() As Boolean
            Dim MatchV = RegexV.Match(Line)
            If MatchV.Success Then
                Dim Point As New MBI.PointInfo
                Dim Captures = MatchV.Groups("Coordinate").Captures

                If Captures.Count >= 1 Then Point.x = Single.Parse(Captures(0).Value, Globalization.CultureInfo.InvariantCulture)
                If Captures.Count >= 2 Then Point.y = Single.Parse(Captures(1).Value, Globalization.CultureInfo.InvariantCulture)
                If Captures.Count >= 3 Then Point.z = Single.Parse(Captures(2).Value, Globalization.CultureInfo.InvariantCulture)
                Points.Add(Point)
                Return True
            End If
            Return False
        End Function
        Private Function CompareVT() As Boolean
            Dim MatchVT = RegexVT.Match(Line)
            If MatchVT.Success Then
                Dim Captures = MatchVT.Groups("Coordinate").Captures
                If Captures.Count >= 1 Then
                    Dim UValue As Single = Single.Parse(Captures(0).Value, Globalization.CultureInfo.InvariantCulture)
                    Dim UInt As Integer = CInt(Round(UValue * 4096))
                    If UInt < -32768 OrElse UInt > 32767 Then UValue = UValue Mod 4.0F
                    U.Add(UValue)
                    If Captures.Count >= 2 Then
                        Dim VValue As Single = 1.0F - Single.Parse(Captures(1).Value, Globalization.CultureInfo.InvariantCulture)
                        Dim VInt As Integer = CInt(Round(VValue * 4096))
                        If VInt < -32768 OrElse VInt > 32767 Then VValue = VValue Mod 4.0F
                        V.Add(VValue)
                    Else
                        Throw New InvalidDataException
                    End If
                End If
                Return True
            End If
            Return False
        End Function
        Private Function CompareF() As Boolean
            Dim MatchF = RegexF.Match(Line)
            If MatchF.Success Then
                Dim d As New MBI.DistrictInfo
                Dim CapturesVertices = MatchF.Groups("v").Captures
                Dim CapturesVerticesTexture = MatchF.Groups("vt").Captures
                Dim Num = CapturesVertices.Count
                Dim Vertices = New Int32(Num - 1) {}
                For n = 0 To Num - 1
                    Vertices(n) = CInt(CapturesVertices(n).Value) - 1
                Next
                Array.Reverse(Vertices)
                Dim Us = New Single(Num - 1) {}
                Dim Vs = New Single(Num - 1) {}
                For n = 0 To Num - 1
                    Dim Index As Integer = CInt(CapturesVerticesTexture(n).Value) - 1
                    Us(n) = U(Index)
                    Vs(n) = V(Index)
                Next
                Array.Reverse(Us)
                Array.Reverse(Vs)

                d.n = CByte(Num)
                d.Attribute = 0
                d.TextureID = CurrentTextureId
                d.Point = Vertices
                d.U = Us
                d.V = Vs

                Districts.Add(d)
                Return True
            End If
            Return False
        End Function
        Private Function CompareG() As Boolean
            Dim MatchG = RegexG.Match(Line)
            If MatchG.Success Then
                Dim Name = MatchG.Result("${Name}")
                If ObjectSet.ContainsKey(Name) Then
                    Throw New InvalidDataException
                Else
                    If Objects.Count > 0 Then Objects(Objects.Count - 1).NextStartDistrictIndex = Districts.Count
                    CurrentObject = Objects.Count
                    Objects.Add(New MBI.ObjectInfo() With {.ObjectName = Name, .StartDistrictIndex = Districts.Count})
                End If
                Return True
            End If
            Return False
        End Function
        Private Function CompareUsemtl() As Boolean
            Dim MatchUsemtl = RegexUsemtl.Match(Line)
            If MatchUsemtl.Success Then
                Dim Name = MatchUsemtl.Result("${Name}")
                If TextureSet.ContainsKey(Name) Then
                    CurrentTextureId = TextureSet(Name)
                Else
                    Throw New InvalidDataException
                End If
                Return True
            End If
            Return False
        End Function
    End Class
End Class
