'==========================================================================
'
'  File:        DIR.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: DIR文件流类
'  Version:     2014.09.30.
'  Copyright(C) F.R.C.
'
'==========================================================================
'TODO:新建DIR游戏无法完全读取

Imports System
Imports System.Math
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports Firefly
Imports Firefly.TextEncoding

''' <summary>DIR文件流类</summary>
''' <remarks>
''' 用于打开盟军1的DIR文件
''' </remarks>
Public Class DIR
    Inherits StreamEx
    Private Sub New(ByVal Path As String, ByVal FileMode As FileMode, ByVal Access As FileAccess, ByVal Share As FileShare)
        MyBase.New(Path, FileMode, Access, Share)
    End Sub
    Shared Function Open(ByVal Path As String) As DIR
        Dim Success = False
        Dim pf As DIR = Nothing
        Try
            pf = New DIR(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
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

    Private RootValue As FileDB
    Public ReadOnly Property Root() As FileDB
        Get
            Return RootValue
        End Get
    End Property


    Public Sub Extract(ByVal File As FileDB, ByVal Directory As String, Optional ByVal Mask As String = "*.*")
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
                        Extract(FileDB, GetPath(Dir, .Name), Mask)
                    Next
                Case Else
            End Select
        End With
    End Sub

    Public Sub New(ByVal Source As String, ByVal Path As String)
        MyBase.New(Path, FileMode.Create)
        Dim FileList As New List(Of FileDB)
        Dim FileToLengthAddressPointer As New Dictionary(Of FileDB, Integer)
        Dim FileToPath As New Dictionary(Of FileDB, String)
        Dim DirList As New List(Of FileDB)
        Dim DirToAddressPointer As New Dictionary(Of FileDB, Integer)

        Dim RootName As String = GetFileName(Source)
        If RootName.Length > 36 Then Throw New InvalidDataException(Source)

        Me.SetLength(16777216)
        RootValue = FileDB.CreateDirectory(RootName, -1)
        RootValue.WriteToFile(Me)
        DirList.Add(RootValue)
        DirToAddressPointer.Add(RootValue, CInt(Me.Position - 4))
        FileDB.CreateDirectoryEnd().WriteToFile(Me)
        ImportDirectory(GetFileDirectory(Source), RootValue, FileList, FileToLengthAddressPointer, FileToPath, DirList, DirToAddressPointer)

        For Each cFileDB In FileList
            Dim f = FileToPath(cFileDB)
            Using File As New StreamEx(f, FileMode.Open)
                cFileDB.Length = CInt(File.Length)
                cFileDB.Address = CInt(Me.Position)
                If Me.Length - Me.Position < File.Length Then
                    Me.SetLength(CLng(Me.Length + Max(16777216, Ceiling(File.Length / 16777216) * 16777216)))
                End If
                For n As Integer = 0 To CInt(File.Length - 1)
                    WriteByte(File.ReadByte)
                Next
            End Using
        Next
        Me.SetLength(Me.Position)

        For Each cFileDB In FileList
            Me.Position = FileToLengthAddressPointer(cFileDB)
            WriteInt32(cFileDB.Length)
            WriteInt32(cFileDB.Address)
        Next
        For Each cFileDB In DirList
            Me.Position = DirToAddressPointer(cFileDB)
            WriteInt32(cFileDB.Address)
        Next
        Me.Position = 0
    End Sub
    Private Sub ImportDirectory(ByVal Dir As String, ByVal DirDB As FileDB, ByVal FileList As List(Of FileDB), ByVal FileToLengthAddressPointer As Dictionary(Of FileDB, Integer), ByVal FileToPath As Dictionary(Of FileDB, String), ByVal DirList As List(Of FileDB), ByVal DirToAddressPointer As Dictionary(Of FileDB, Integer))
        DirDB.SubFileDB = New List(Of FileDB)
        DirDB.Address = CInt(Me.Position)
        For Each f As String In Directory.GetFiles(GetPath(Dir, DirDB.Name))
            Dim Name = GetFileName(f)
            If Name.Length > 36 Then Throw New InvalidDataException(f)
            Dim cFileDB = FileDB.CreateFile(Name, -1, -1)
            cFileDB.WriteToFile(Me)
            cFileDB.ParentFileDB = DirDB
            DirDB.SubFileDB.Add(cFileDB)
            FileList.Add(cFileDB)
            FileToLengthAddressPointer.Add(cFileDB, CInt(Me.Position - 8))
            FileToPath.Add(cFileDB, f)
        Next
        Dim DirListInner As New Queue(Of FileDB)
        For Each d As String In Directory.GetDirectories(GetPath(Dir, DirDB.Name))
            Dim Name = GetFileName(d)
            If Name.Length > 36 Then Throw New InvalidDataException(d)
            Dim cFileDB = FileDB.CreateDirectory(Name, -1)
            cFileDB.WriteToFile(Me)
            cFileDB.ParentFileDB = DirDB
            DirDB.SubFileDB.Add(cFileDB)
            DirList.Add(cFileDB)
            DirListInner.Enqueue(cFileDB)
            DirToAddressPointer.Add(cFileDB, CInt(Me.Position - 4))
        Next
        FileDB.CreateDirectoryEnd().WriteToFile(Me)

        For Each d As String In Directory.GetDirectories(GetPath(Dir, DirDB.Name))
            Dim cFileDB = DirListInner.Dequeue
            ImportDirectory(GetFileDirectory(d), cFileDB, FileList, FileToLengthAddressPointer, FileToPath, DirList, DirToAddressPointer)
        Next
    End Sub

    'End Of Class
    'Start Of SubClasses

    Public Class FileDB
        Public Name As String
        Public Type As FileType
        Public Length As Int32
        Public Address As Int32
        Public Const DBLength As Integer = 44
        Public ParentFileDB As FileDB
        Public SubFileDB As List(Of FileDB)
        Private Sub New(ByVal Name As String, ByVal Type As FileType, ByVal Length As Int32, ByVal Address As Int32)
            If Name <> "" Then Me.Name = Name.ToUpper
            Me.Type = Type
            Me.Length = Length
            Me.Address = Address
        End Sub
        Public Sub New(ByVal s As DIR)
            Name = s.ReadString(32, ASCII)
            Type = CType(s.ReadByte, FileType)
            s.Position += 3
            Length = s.ReadInt32
            Address = s.ReadInt32
            If Type = FileType.Directory Then
                SubFileDB = New List(Of FileDB)
                Dim TempAddress As Integer = CInt(s.Position)
                s.Position = Address
                Dim f As New FileDB(s)
                While f.Type <> FileType.DirectoryEnd
                    f.ParentFileDB = Me
                    SubFileDB.Add(f)
                    f = New FileDB(s)
                End While
                s.Position = TempAddress
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
            Return New FileDB(Name, FileType.Directory, 0, Address)
        End Function
        Public Shared Function CreateDirectoryEnd() As FileDB
            Return New FileDB("DIRECTOR.FIN", FileType.DirectoryEnd, &HFFFFFFFF, &HFFFFFFFF)
        End Function
        Public Sub WriteToFile(ByVal s As DIR)
            s.WriteString(Name, 32, ASCII)
            s.WriteInt32(Type)
            s.WriteInt32(Length)
            s.WriteInt32(Address)
        End Sub
    End Class
End Class
