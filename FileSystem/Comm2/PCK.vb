'==========================================================================
'
'  File:        PCK.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: PCK文件流类
'  Version:     2012.08.25.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports Firefly
Imports Firefly.TextEncoding

''' <summary>PCK文件流类</summary>
''' <remarks>
''' 用于打开和创建盟军2及3的PCK文件
''' </remarks>
Public Class PCK
    Inherits StreamEx
    Private Sub New(ByVal Path As String, ByVal FileMode As FileMode, ByVal Access As FileAccess, ByVal Share As FileShare)
        MyBase.New(Path, FileMode, Access, Share)
    End Sub
    Shared Function Open(ByVal Path As String, ByVal VersionSign As Version) As PCK
        Dim Success = False
        Dim pf As PCK = Nothing
        Try
            pf = New PCK(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            pf.PCKVersionSign = VersionSign
            pf.Position = 0
            pf.RootValue = New FileDB(pf)
            Success = True
            Return pf
        Finally
            If Not Success Then
                Try
                    pf.Close()
                Catch
                End Try
            End If
        End Try
    End Function

    Private PCKVersionSign As Version
    Public ReadOnly Property VersionSign() As Version
        Get
            Return PCKVersionSign
        End Get
    End Property
    Public Enum Version
        Comm2 = 2
        Comm3 = 3
    End Enum

    Private RootValue As FileDB
    Public ReadOnly Property Root() As FileDB
        Get
            Return RootValue
        End Get
    End Property

    Public Function TryGetFileDB(ByVal Path As String) As FileDB
        Dim p As String = Path
        Dim ret As FileDB = Root
        Dim d As String = PopFirstDir(p)
        If d = "" Then Return ret
        If d <> ret.Name Then Return Nothing
        While ret IsNot Nothing
            d = PopFirstDir(p)
            If d = "" Then Return ret
            For n As Integer = 0 To ret.SubFileDB.Count - 1
                If d = ret.SubFileDB(n).Name Then
                    ret = ret.SubFileDB(n)
                    Continue While
                End If
            Next
            Return Nothing
        End While
        Return Nothing
    End Function

    Public Sub Extract(ByVal File As FileDB, ByVal Directory As String, Optional ByVal Mask As String = "*.*")
        Select Case PCKVersionSign
            Case Version.Comm2
                Extract2(File, Directory, Mask)
            Case Version.Comm3
                Extract3(File, Directory, Mask)
        End Select
    End Sub
    Private Sub Extract2(ByVal File As FileDB, ByVal Directory As String, ByVal Mask As String)
        With File
            Dim Dir As String = Directory.Trim.TrimEnd("\"c)
            If Dir <> "" AndAlso Not IO.Directory.Exists(Dir) Then IO.Directory.CreateDirectory(Dir)
            Select Case .Type
                Case FileDB.FileType.File
                    If IsMatchFileMask(.Name, Mask) Then
                        Dim t As New StreamEx(GetPath(Dir, .Name), FileMode.Create)
                        BaseStream.Position = .Address
                        For n As Integer = 0 To .Length - 1
                            t.WriteByte(CByte(BaseStream.ReadByte))
                        Next
                        t.Close()
                    End If
                Case FileDB.FileType.Directory
                    Dim d As String = GetPath(Dir, .Name)
                    If d <> "" AndAlso Not IO.Directory.Exists(d) Then IO.Directory.CreateDirectory(d)
                    Dim FileSet As New Queue(Of FileDB)
                    BaseStream.Position = .Address
                    Dim f As New FileDB(Me)
                    While f.Type <> FileDB.FileType.DirectoryEnd
                        FileSet.Enqueue(f)
                        f = New FileDB(Me)
                    End While
                    For Each FileDB As FileDB In FileSet
                        Extract2(FileDB, GetPath(Dir, .Name), Mask)
                    Next
                Case Else
            End Select
        End With
    End Sub
    Private Sub Extract3(ByVal File As FileDB, ByVal Directory As String, ByVal Mask As String)
        Static KeyTran As Byte() = {0, 4, 9, &HD, 2, 6, &HB}
        With File
            Dim Dir As String = Directory.Trim.TrimEnd("\"c)
            If Dir <> "" AndAlso Not IO.Directory.Exists(Dir) Then IO.Directory.CreateDirectory(Dir)
            Select Case .Type
                Case FileDB.FileType.File
                    If IsMatchFileMask(.Name, Mask) Then
                        Dim t As New StreamEx(GetPath(Dir, .Name), FileMode.Create)
                        t.SetLength(.Length)
                        BaseStream.Position = .Address
                        Dim n As Integer = 0
                        Dim keyhigh As Byte = 0
                        Dim keylow As Byte = KeyTran(.Address Mod 7)
                        While n < .Length - 16
                            t.WriteByte(CByte(BaseStream.ReadByte Xor ((keyhigh << 4) Or keylow)))
                            For x As Integer = 1 To 15
                                t.WriteByte(CByte(BaseStream.ReadByte))
                            Next
                            keyhigh = CByte((keyhigh + 1) Mod 16)
                            keylow = CByte(keylow + 1)
                            If keylow Mod 8 = 7 Then keylow = CByte((keylow + 1) Mod 16)
                            n += 16
                        End While
                        t.WriteByte(CByte(BaseStream.ReadByte Xor ((keyhigh << 4) Or keylow)))
                        For x As Integer = 1 To .Length - n - 1
                            t.WriteByte(CByte(BaseStream.ReadByte))
                        Next
                        t.Close()
                    End If
                Case FileDB.FileType.Directory
                    Dim d As String = GetPath(Dir, .Name)
                    If d <> "" AndAlso Not IO.Directory.Exists(d) Then IO.Directory.CreateDirectory(d)
                    Dim FileSet As New Queue(Of FileDB)
                    BaseStream.Position = .Address
                    Dim f As New FileDB(Me)
                    While f.Type <> FileDB.FileType.DirectoryEnd
                        FileSet.Enqueue(f)
                        f = New FileDB(Me)
                    End While
                    For Each FileDB As FileDB In FileSet
                        Extract3(FileDB, GetPath(Dir, .Name), Mask)
                    Next
                Case Else
            End Select
        End With
    End Sub

    Sub New(ByVal Source As String, ByVal Path As String, ByVal VersionSign As Version)
        MyBase.New(Path, FileMode.Create)
        If VersionSign <> Version.Comm2 AndAlso VersionSign <> Version.Comm3 Then
            MyBase.Close()
            Throw New InvalidDataException
        End If
        Me.PCKVersionSign = VersionSign
        Dim FileQueue As New Queue(Of FileDB)
        Dim FileLengthAddressPointerQueue As New Queue(Of Integer)
        Dim FilePathQueue As New Queue(Of String)
        Dim FileLengthQueue As New Queue(Of Integer)
        Dim FileAddressQueue As New Queue(Of Integer)

        Dim RootName As String = GetFileName(Source)
        If RootName.Length > 36 Then Throw New InvalidDataException(Source)

        Me.SetLength(16777216)
        Dim cFileDB As FileDB = FileDB.CreateDirectory(RootName, FileDB.DBLength)
        RootValue = cFileDB
        cFileDB.WriteToFile(Me)
        ImportDirectory(GetFileDirectory(Source), cFileDB, FileQueue, FileLengthAddressPointerQueue, FilePathQueue)
        FileDB.CreateDirectoryEnd().WriteToFile(Me)

        Dim File As StreamEx
        Select Case PCKVersionSign
            Case Version.Comm2
                For Each f As String In FilePathQueue
                    File = New StreamEx(f, FileMode.Open)
                    GotoNextFilePoint()
                    FileLengthQueue.Enqueue(CInt(File.Length))
                    FileAddressQueue.Enqueue(CInt(Me.Position))
                    If Me.Length - Me.Position < File.Length Then
                        Me.SetLength(CLng(Me.Length + Max(16777216, Ceiling(File.Length / 16777216) * 16777216)))
                    End If
                    For n As Integer = 0 To CInt(File.Length - 1)
                        WriteByte(File.ReadByte)
                    Next
                    File.Close()
                Next
            Case Version.Comm3
                Static KeyTran As Byte() = {0, 4, 9, &HD, 2, 6, &HB}
                For Each f As String In FilePathQueue
                    File = New StreamEx(f, FileMode.Open)
                    GotoNextFilePoint()
                    FileLengthQueue.Enqueue(CInt(File.Length))
                    FileAddressQueue.Enqueue(CInt(Me.Position))
                    If Me.Length - Me.Position < File.Length Then
                        Me.SetLength(CLng(Me.Length + Max(16777216, Ceiling(File.Length / 16777216) * 16777216)))
                    End If

                    Dim n As Integer = 0
                    Dim keyhigh As Byte = 0
                    Dim keylow As Byte = KeyTran(CInt(Me.Position Mod 7))
                    While n < File.Length - 16
                        WriteByte(File.ReadByte Xor ((keyhigh << 4) Or keylow))
                        For x As Integer = 1 To 15
                            WriteByte(File.ReadByte)
                        Next
                        keyhigh = CByte((keyhigh + 1) Mod 16)
                        keylow = CByte(keylow + 1)
                        If keylow Mod 8 = 7 Then keylow = CByte((keylow + 1) Mod 16)
                        n += 16
                    End While
                    WriteByte(File.ReadByte Xor ((keyhigh << 4) Or keylow))
                    For x As Integer = 1 To CInt(File.Length - n - 1)
                        WriteByte(File.ReadByte)
                    Next
                    File.Close()
                Next
        End Select
        GotoNextFilePoint()
        Me.SetLength(Me.Position)

        Dim fn As FileDB
        Dim pl As Integer
        Dim pa As Integer
        For Each p As Integer In FileLengthAddressPointerQueue
            Me.Position = p
            fn = FileQueue.Dequeue
            pl = FileLengthQueue.Dequeue
            pa = FileAddressQueue.Dequeue
            fn.Length = pl
            fn.Address = pa
            WriteInt32(pl)
            WriteInt32(pa)
        Next
        Me.Position = 0
    End Sub
    Private Sub ImportDirectory(ByVal Dir As String, ByVal DirDB As FileDB, ByVal FileQueue As Queue(Of FileDB), ByVal FileLengthAddressPointerQueue As Queue(Of Integer), ByVal FilePathQueue As Queue(Of String))
        Dim cFileDB As FileDB
        Dim Name As String
        DirDB.SubFileDB = New List(Of FileDB)
        For Each f As String In Directory.GetFiles(GetPath(Dir, DirDB.Name))
            Name = GetFileName(f)
            If Name.Length > 36 Then Throw New InvalidDataException(f)
            cFileDB = FileDB.CreateFile(Name, -1, -1)
            cFileDB.WriteToFile(Me)
            cFileDB.ParentFileDB = DirDB
            DirDB.SubFileDB.Add(cFileDB)
            FileQueue.Enqueue(cFileDB)
            FileLengthAddressPointerQueue.Enqueue(CInt(Me.Position - 8))
            FilePathQueue.Enqueue(f)
        Next
        For Each d As String In Directory.GetDirectories(GetPath(Dir, DirDB.Name))
            Name = GetFileName(d)
            If Name.Length > 36 Then Throw New InvalidDataException(d)
            cFileDB = FileDB.CreateDirectory(Name, CInt(Me.Position + FileDB.DBLength))
            cFileDB.WriteToFile(Me)
            cFileDB.ParentFileDB = DirDB
            DirDB.SubFileDB.Add(cFileDB)
            ImportDirectory(GetFileDirectory(d), cFileDB, FileQueue, FileLengthAddressPointerQueue, FilePathQueue)
            FileDB.CreateDirectoryEnd().WriteToFile(Me)
        Next
    End Sub
    Sub GotoNextFilePoint()
        If Position Mod &H800 <> 0 Then Position = (Position \ &H800 + 1) * &H800
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class FileDB
        Public Name As String
        Public Type As FileType
        Public Length As Int32
        Public Address As Int32
        Public Const DBLength As Integer = 48
        Public ParentFileDB As FileDB
        Public SubFileDB As List(Of FileDB)
        Private Sub New(ByVal Name As String, ByVal Type As FileType, ByVal Length As Int32, ByVal Address As Int32)
            If Name <> "" Then Me.Name = Name.ToUpper
            Me.Type = Type
            Me.Length = Length
            Me.Address = Address
        End Sub
        Public Sub New(ByVal s As PCK)
            Name = s.ReadString(36, Windows1252)
            Type = CType(s.ReadByte, FileType)
            s.Position += 3
            Length = s.ReadInt32
            Address = s.ReadInt32
            If Type = FileType.Directory Then
                SubFileDB = New List(Of FileDB)
                s.Position = Address
                Dim f As New FileDB(s)
                While f.Type <> FileType.DirectoryEnd
                    f.ParentFileDB = Me
                    SubFileDB.Add(f)
                    f = New FileDB(s)
                End While
            End If
        End Sub
        Public Enum FileType As Byte
            File = 0
            Directory = 1
            DirectoryEnd = 255
        End Enum
        Public Shared Function CreateFile(ByVal Name As String, ByVal Length As Int32, ByVal Address As Int32) As FileDB
            Return New FileDB(Name, FileType.File, Length, Address)
        End Function
        Public Shared Function CreateDirectory(ByVal Name As String, ByVal Address As Int32) As FileDB
            Return New FileDB(Name, FileType.Directory, &HFFFFFFFF, Address)
        End Function
        Public Shared Function CreateDirectoryEnd() As FileDB
            Return New FileDB(Nothing, FileType.DirectoryEnd, &HFFFFFFFF, &HFFFFFFFF)
        End Function
        Public Sub WriteToFile(ByVal s As PCK)
            s.WriteString(Name, 36, Windows1252)
            s.WriteByte(Type)
            s.Position += 3
            s.WriteInt32(Length)
            s.WriteInt32(Address)
        End Sub
    End Class
End Class
