'==========================================================================
'
'  File:        SEC.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: SEC文件类
'  Version:     2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Text.RegularExpressions
Imports Firefly
Imports Firefly.Mapping
Imports Firefly.TextEncoding
Imports Firefly.Texting
Imports Firefly.Setting
Imports GraphSystem

''' <summary>SEC文件类</summary>
''' <remarks>
''' 用于处理盟军2及3的SEC文件
''' </remarks>
Public Class SEC
    Public Enum Version As Int64
        Comm2Demo = &H100000000L
        Comm2 = &H100000001L
        Comm3 = &H100000002L
    End Enum
    ''' <remarks>可以设定</remarks>
    Public VersionSign As Version
    Public ReadOnly Property NumZone() As Int32
        Get
            If Zones Is Nothing Then Return 0
            Return Zones.Length
        End Get
    End Property
    Public Zones As String()
    Public Const MapToken As String = "MAP1"
    Public ReadOnly Property NumPoint() As Integer
        Get
            If Points Is Nothing Then Return 0
            Return Points.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property NumBorder() As Integer
        Get
            If Borders Is Nothing Then Return 0
            Return Borders.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property NumDistrict() As Integer
        Get
            If Districts Is Nothing Then Return 0
            Return Districts.GetLength(0)
        End Get
    End Property
    Private NumSpecialDistrictValue As Integer
    Public ReadOnly Property NumSpecialDistrict() As Integer
        Get
            Return NumSpecialDistrictValue
        End Get
    End Property
    Public Points As PointInfo()
    Public Borders As BorderInfo()
    Public Districts As DistrictInfo()
    Public Const Symbol As Int32 = &H48415332
    ''' <remarks>效率为 O(NumberMeshX * NumberMeshY)</remarks>
    Public Function TotalNumDistrict() As Integer
        If MeshDSValue Is Nothing Then Return 0
        Dim t As Integer = 0
        For y As Integer = 0 To MeshDSValue.GetUpperBound(1)
            For x As Integer = 0 To MeshDSValue.GetUpperBound(0)
                If MeshDSValue(x, y) IsNot Nothing Then t += MeshDSValue(x, y).NumDistrict
            Next
        Next
        Return t
    End Function
    Public Const UnknownSign As Int32 = 6
    Public ReadOnly Property NumMeshX() As Integer
        Get
            If MeshDSValue Is Nothing Then Return 0
            Return MeshDSValue.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property NumMeshY() As Integer
        Get
            If MeshDSValue Is Nothing Then Return 0
            Return MeshDSValue.GetLength(1)
        End Get
    End Property
    Private MeshDSValue As Array2(Of Mesh)
    Public ReadOnly Property MeshDS() As Array2(Of Mesh)
        Get
            Return MeshDSValue
        End Get
    End Property

    Public Sub New()
    End Sub
    Public Sub New(ByVal Path As String)
        Using sf As New StreamEx(Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            VersionSign = CType(sf.ReadInt64(), Version)
            If VersionSign = Version.Comm2Demo Then
                Throw New NotSupportedException
            End If
            If (VersionSign <> Version.Comm2) AndAlso (VersionSign <> Version.Comm3) Then
                Throw New InvalidDataException
            End If

            Dim NumZone = sf.ReadInt32
            Zones = New String(NumZone - 1) {}
            For n As Integer = 0 To NumZone - 1
                Zones(n) = sf.ReadString(32, Windows1252)
            Next
            If sf.ReadString(32, Windows1252) <> MapToken Then Throw New InvalidDataException

            Points = New PointInfo(sf.ReadInt32 - 1) {}
            Borders = New BorderInfo(sf.ReadInt32 - 1) {}
            Districts = New DistrictInfo(sf.ReadInt32 - 1) {}
            NumSpecialDistrictValue = sf.ReadInt32

            For n As Integer = 0 To NumPoint - 1
                Points(n) = New PointInfo(sf)
            Next
            For n As Integer = 0 To NumBorder - 1
                Borders(n) = New BorderInfo(sf)
            Next
            For n As Integer = 0 To NumDistrict - 1
                Districts(n) = New DistrictInfo(sf, VersionSign)
            Next

            sf.Position += 12
            Dim nX As Int32 = sf.ReadInt32
            Dim nY As Int32 = sf.ReadInt32
            MeshDSValue = Array2(Of Mesh).Create(nX, nY)

            For y As Integer = 0 To nY - 1
                For x As Integer = 0 To nX - 1
                    MeshDSValue(x, y) = New Mesh(sf)
                Next
            Next
        End Using
    End Sub
    Public Sub WriteToFile(ByVal Path As String)
        WriteToFile(Path, Me.VersionSign)
    End Sub
    Public Sub WriteToFile(ByVal Path As String, ByVal VersionSign As Version)
        If (VersionSign <> Version.Comm2) AndAlso (VersionSign <> Version.Comm3) Then
            Throw New InvalidDataException
        End If
        Using sf As New StreamEx(Path, FileMode.Create, FileAccess.Write, FileShare.None)
            sf.WriteInt64(VersionSign)

            sf.WriteInt32(NumZone)
            For n As Integer = 0 To NumZone - 1
                sf.WriteString(Zones(n), 32, Windows1252)
            Next
            sf.WriteString(MapToken, 32, Windows1252)

            sf.WriteInt32(NumPoint)
            sf.WriteInt32(NumBorder)
            sf.WriteInt32(NumDistrict)
            sf.WriteInt32(NumSpecialDistrict)

            For n As Integer = 0 To NumPoint - 1
                Points(n).WriteTo(sf)
            Next
            For n As Integer = 0 To NumBorder - 1
                Borders(n).WriteTo(sf)
            Next
            For n As Integer = 0 To NumDistrict - 1
                Districts(n).WriteTo(sf, VersionSign)
            Next

            sf.WriteInt32(Symbol)
            sf.WriteInt32(TotalNumDistrict)
            sf.WriteInt32(UnknownSign)
            sf.WriteInt32(NumMeshX)
            sf.WriteInt32(NumMeshY)

            For y As Integer = 0 To NumMeshY - 1
                For x As Integer = 0 To NumMeshX - 1
                    MeshDSValue(x, y).WriteTo(sf)
                Next
            Next
        End Using
    End Sub
    Private Class ByteArrayEncoder
        Inherits Xml.Mapper(Of Byte(), String)

        Public Overrides Function GetMappedObject(ByVal o As Byte()) As String
            Return String.Join(" ", (From b In o Select b.ToString("X2")).ToArray)
        End Function

        Public Overrides Function GetInverseMappedObject(ByVal o As String) As Byte()
            Dim Trimmed = o.Trim(" \t\r\n".Descape().ToCharArray())
            If Trimmed = "" Then Return New Byte() {}
            Return (From s In Regex.Split(Trimmed, "( |\t|\r|\n)+", RegexOptions.ExplicitCapture) Select Byte.Parse(s, Globalization.NumberStyles.HexNumber)).ToArray
        End Function
    End Class
    Public Shared Function OpenXml(ByVal Path As String) As SEC
        Dim f As SEC = Xml.ReadFile(Of SEC)(GetPath(Path, "Description.xml"), New Xml.IMapper() {New ByteArrayEncoder, New SEC.TerrainInfoEncoder, New Array2Mapper(Of SEC.Mesh)})
        f.GenerateMesh()
        f.RecalculateNumSpecialDistrict()
        Return f
    End Function
    Public Sub WriteToXmlFile(ByVal Path As String)
        Xml.WriteFile(Path, TextEncoding.WritingDefault, Me, New Xml.IMapper() {New ByteArrayEncoder, New SEC.TerrainInfoEncoder, New Array2Mapper(Of SEC.Mesh)})
    End Sub


    ''' <summary>SEC中最小的X坐标，用于确定辅助Mesh的原点</summary>
    Public Function MinX() As Double
        If Points Is Nothing Then Return Single.PositiveInfinity
        Dim min As Single = Single.PositiveInfinity
        For Each p As PointInfo In Points
            If p.x < min Then min = p.x
        Next
        Return min
    End Function
    ''' <summary>SEC中最小的Y坐标，用于确定辅助Mesh的原点</summary>
    Public Function MinY() As Double
        If Points Is Nothing Then Return Single.PositiveInfinity
        Dim min As Single = Single.PositiveInfinity
        For Each p As PointInfo In Points
            If p.y < min Then min = p.y
        Next
        Return min
    End Function
    ''' <summary>SEC中最大的X坐标，用于计算辅助Mesh格数</summary>
    Public Function MaxX() As Double
        If Points Is Nothing Then Return Single.NegativeInfinity
        Dim max As Single = Single.NegativeInfinity
        For Each p As PointInfo In Points
            If p.x > max Then max = p.x
        Next
        Return max
    End Function
    ''' <summary>SEC中最大的Y坐标，用于计算辅助Mesh格数</summary>
    Public Function MaxY() As Double
        If Points Is Nothing Then Return Single.NegativeInfinity
        Dim max As Single = Single.NegativeInfinity
        For Each p As PointInfo In Points
            If p.y > max Then max = p.y
        Next
        Return max
    End Function

    Public Sub GenerateMesh()
        Dim Ox As Single = CSng(MinX())
        Dim Oy As Single = CSng(MinY())
        MeshDSValue = Array2(Of Mesh).Create(CInt(Ceiling((MaxX() + 1 - Ox) / 64)), CInt(Ceiling((MaxY() + 1 - Oy) / 64)))
        For y = 0 To MeshDSValue.GetUpperBound(1)
            For x = 0 To MeshDSValue.GetUpperBound(0)
                MeshDSValue(x, y) = New Mesh
            Next
        Next
        For n As Integer = 0 To NumDistrict - 1
            Dim d = Districts(n)
            For y = CInt(Floor((d.MinPy - Oy) / 64)) To CInt(Ceiling((d.MaxPy + 1 - Oy) / 64)) - 1
                For x = CInt(Floor((d.MinPx - Ox) / 64)) To CInt(Ceiling((d.MaxPx + 1 - Ox) / 64)) - 1
                    MeshDSValue(x, y).DistrictIndex.Add(n)
                Next
            Next
        Next
    End Sub

    Public Sub RecalculateNumSpecialDistrict()
        Dim Num As Integer = 0
        For Each d In Districts
            For Each b In d.Border
                If Borders(b).NeighborDistrictIndex = -1 Then
                    Num += 1
                    Exit For
                End If
            Next
        Next
        NumSpecialDistrictValue = Num
    End Sub

    Public Sub ToObj(ByVal ObjPath As String)
        With New Exporter
            .ToObj(ObjPath, Me)
        End With
    End Sub

    Public Shared Function FromObj(ByVal ObjPath As String) As SEC
        Return (New Importer).FromObj(ObjPath)
    End Function

    'End Of Class
    'Start Of SubClasses

    Public Class PointInfo
        Public x As Single
        Public y As Single
        Public Sub New()
        End Sub
        Sub New(ByVal s As StreamEx)
            x = s.ReadSingle
            y = s.ReadSingle
        End Sub
        Sub WriteTo(ByVal s As StreamEx)
            s.WriteSingle(x)
            s.WriteSingle(y)
        End Sub
    End Class
    Public Class BorderInfo
        Public StartPointIndex As Integer
        Public EndPointIndex As Integer
        Public ParentDistrictID As Integer
        Public NeighborDistrictIndex As Integer = -1
        Public Adjacency As UInt32
        Public Sub New()
        End Sub
        Sub New(ByVal s As StreamEx)
            StartPointIndex = s.ReadInt32
            EndPointIndex = s.ReadInt32
            ParentDistrictID = s.ReadInt32
            NeighborDistrictIndex = s.ReadInt32
            Adjacency = s.ReadUInt32
        End Sub
        Sub WriteTo(ByVal s As StreamEx)
            s.WriteInt32(StartPointIndex)
            s.WriteInt32(EndPointIndex)
            s.WriteInt32(ParentDistrictID)
            s.WriteInt32(NeighborDistrictIndex)
            s.WriteUInt32(Adjacency)
        End Sub
    End Class

    Public Class DistrictInfo
        Public ReadOnly Property n() As Int32
            Get
                Return Border.GetLength(0)
            End Get
        End Property
        Public Terrain As TerrainInfo
        Public kx As Single
        Public ky As Single
        Public bz As Single
        Public Unknown1 As UInt64
        Public ZoneFlags As UInt64
        Public Unknown2 As UInt64
        Public MinPx As Single
        Public MinPy As Single
        Public MinPz As Single
        Public MaxPx As Single
        Public MaxPy As Single
        Public MaxPz As Single
        Public Border As Int32()

        Public Sub New()
        End Sub
        Sub New(ByVal s As StreamEx, ByVal VersionSign As SEC.Version)
            Dim n As Int32 = s.ReadInt32
            Terrain = New TerrainInfo(s.ReadInt64)
            kx = s.ReadSingle
            ky = s.ReadSingle
            bz = s.ReadSingle
            Unknown1 = s.ReadUInt64()
            ZoneFlags = s.ReadUInt64()
            If VersionSign = SEC.Version.Comm2 Then
                Unknown2 = s.ReadUInt64()
            Else
                Unknown2 = 0
            End If
            MinPx = s.ReadSingle
            MinPy = s.ReadSingle
            MinPz = s.ReadSingle
            MaxPx = s.ReadSingle
            MaxPy = s.ReadSingle
            MaxPz = s.ReadSingle
            Border = New Int32(n - 1) {}
            For i As Integer = 0 To n - 1
                Border(i) = s.ReadInt32
            Next
        End Sub
        Sub WriteTo(ByVal s As StreamEx, ByVal VersionSign As SEC.Version)
            s.WriteInt32(n)
            s.WriteInt64(Terrain.ValueInt)
            s.WriteSingle(kx)
            s.WriteSingle(ky)
            s.WriteSingle(bz)
            s.WriteUInt64(Unknown1)
            s.WriteUInt64(ZoneFlags)
            If VersionSign = SEC.Version.Comm2 Then
                s.WriteUInt64(Unknown2)
            End If
            s.WriteSingle(MinPx)
            s.WriteSingle(MinPy)
            s.WriteSingle(MinPz)
            s.WriteSingle(MaxPx)
            s.WriteSingle(MaxPy)
            s.WriteSingle(MaxPz)
            For i As Integer = 0 To n - 1
                s.WriteInt32(Border(i))
            Next
        End Sub
        Public Function GetZ(ByVal x As Double, ByVal y As Double) As Double
            '平面方程 z = kx * x + ky * y + bz
            Return kx * x + ky * y + bz
        End Function
    End Class
    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Explicit)> Public Structure TerrainInfo
        <Runtime.InteropServices.FieldOffset(0)> Public ValueInt As Int64
        <Runtime.InteropServices.FieldOffset(0)> Private MajorTypeValue As MajorTypes
        <Runtime.InteropServices.FieldOffset(1)> Private MinorTypeValue As MinorTypes
        <Runtime.InteropServices.FieldOffset(2)> Public Byte2 As Byte
        <Runtime.InteropServices.FieldOffset(3)> Public Byte3 As Byte
        <Runtime.InteropServices.FieldOffset(4)> Public Byte4 As Byte
        <Runtime.InteropServices.FieldOffset(5)> Public Byte5 As Byte
        <Runtime.InteropServices.FieldOffset(6)> Public Byte6 As Byte
        <Runtime.InteropServices.FieldOffset(7)> Public Byte7 As Byte
        Public Enum MajorTypes As Byte
            TIERRA
            NIEVE
            AGUA
            ORILLA
            SUBMARINO
        End Enum
        Public Enum MinorTypes As Byte
            ASFALTO
            HIERBA
            TIERRA
            ADOQUINES
            AZULEJOS
            MADERA
            ARENA
            NIEVE
            HIELO
            ROCAS
            ARBUSTOS
            METAL
            METAL_ENCHARCADO
            ORILLA
            AGUA_PROFUNDA
            GRAVILLA
        End Enum
        Public Sub New(ByVal Value As Int64)
            Me.ValueInt = Value
            If Byte2 <> 0 Then Throw New InvalidDataException
            If Byte3 <> 0 Then Throw New InvalidDataException
        End Sub
        Public Sub New(ByVal MajorType As MajorTypes, ByVal MinorType As MinorTypes)
            Me.MajorType = MajorType
            Me.MinorType = MinorType
        End Sub
        Public Shared Widening Operator CType(ByVal Obj As Int64) As TerrainInfo
            Dim TerrainInfo As New TerrainInfo
            TerrainInfo.ValueInt = Obj
            Return TerrainInfo
        End Operator
        Public Shared Widening Operator CType(ByVal Obj As TerrainInfo) As Int64
            Return Obj.ValueInt
        End Operator
        Private Shared Function Reverse(ByVal i As UInt64) As UInt64
            Return (((i And &HFF00000000000000UL) >> 56) And &HFFUL) Or ((i And &HFF000000000000UL) >> 40) Or ((i And &HFF0000000000UL) >> 24) Or ((i And &HFF00000000UL) >> 8) Or ((i And &HFFUL) << 56) Or ((i And &HFF00UL) << 40) Or ((i And &HFF0000UL) << 24) Or ((i And &HFF000000UL) << 8)
        End Function
        Public Overrides Function ToString() As String
            Return Reverse(CSU(ValueInt)).ToString("X16")
        End Function
        Public Shared Function FromString(ByVal i As String) As TerrainInfo
            Return CUS(Reverse(UInt64.Parse(i, Globalization.NumberStyles.HexNumber)))
        End Function
        Public Property ValueByIndex(ByVal ByteIndex As Integer, ByVal BitIndex As Integer) As Boolean
            Get
                Return CBool((ValueInt >> (ByteIndex * 8 + BitIndex)) And 1)
            End Get
            Set(ByVal Value As Boolean)
                ValueInt = ValueInt And Not (CLng(1) << (ByteIndex * 8 + BitIndex))
                If Value Then ValueInt = ValueInt Or (CLng(1) << (ByteIndex * 8 + BitIndex))
            End Set
        End Property
        Public Property MajorType() As MajorTypes
            Get
                Return MajorTypeValue
            End Get
            Set(ByVal value As MajorTypes)
                MajorTypeValue = value
            End Set
        End Property
        Public Property MinorType() As MinorTypes
            Get
                Return MinorTypeValue
            End Get
            Set(ByVal value As MinorTypes)
                MinorTypeValue = value
            End Set
        End Property
        Public Property Transitable() As Boolean
            Get
                Return ValueByIndex(4, 0)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 0) = Value
            End Set
        End Property
        Public Property IsOcclusion() As Boolean
            Get
                Return ValueByIndex(4, 1)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 1) = Value
            End Set
        End Property
        Public Property IsPlane() As Boolean
            Get
                Return ValueByIndex(4, 2)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 2) = Value
            End Set
        End Property
        Public Property [Double]() As Boolean
            Get
                Return ValueByIndex(4, 3)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 3) = Value
            End Set
        End Property
        Public Property PlotNotVisible() As Boolean
            Get
                Return ValueByIndex(4, 4)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 4) = Value
            End Set
        End Property
        Public Property BendOnly() As Boolean
            Get
                Return ValueByIndex(4, 5)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 5) = Value
            End Set
        End Property
        Public Property IsExplosionOcclusion() As Boolean
            Get
                Return ValueByIndex(4, 6)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 6) = Value
            End Set
        End Property
        Public Property MakeSlow() As Boolean
            Get
                Return ValueByIndex(4, 7)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(4, 7) = Value
            End Set
        End Property
        Public Property Buriable() As Boolean
            Get
                Return ValueByIndex(5, 0)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 0) = Value
            End Set
        End Property
        Public Property IsLadder() As Boolean
            Get
                Return ValueByIndex(5, 1)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 1) = Value
            End Set
        End Property
        Public Property LeaveFootprints() As Boolean
            Get
                Return ValueByIndex(5, 2)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 2) = Value
            End Set
        End Property
        Public Property IsBarbedWire() As Boolean
            Get
                Return ValueByIndex(5, 3)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 3) = Value
            End Set
        End Property
        Public Property PunyOnly() As Boolean
            Get
                Return ValueByIndex(5, 4)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 4) = Value
            End Set
        End Property
        Public Property IsFenceSector() As Boolean
            Get
                Return ValueByIndex(5, 5)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 5) = Value
            End Set
        End Property
        Public Property IsFenceOcclusion() As Boolean
            Get
                Return ValueByIndex(5, 6)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 6) = Value
            End Set
        End Property
        Public Property IsShadow() As Boolean
            Get
                Return ValueByIndex(5, 7)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(5, 7) = Value
            End Set
        End Property
        Public Property ToCeiling() As Boolean
            Get
                Return ValueByIndex(6, 0)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(6, 0) = Value
            End Set
        End Property
        Public Property IsMapOut() As Boolean
            Get
                Return ValueByIndex(6, 1)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(6, 1) = Value
            End Set
        End Property
        Public Property MouseNotHoverable() As Boolean
            Get
                Return ValueByIndex(6, 2)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(6, 2) = Value
            End Set
        End Property
        Public Property NoCliff() As Boolean
            Get
                Return ValueByIndex(6, 3)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(6, 3) = Value
            End Set
        End Property
        Public Property AutotransferPedestrians() As Boolean
            Get
                Return ValueByIndex(6, 7)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(6, 7) = Value
            End Set
        End Property
        Public Property InteriorVehicles() As Boolean
            Get
                Return ValueByIndex(7, 6)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(7, 6) = Value
            End Set
        End Property
        Public Property IsPad() As Boolean
            Get
                Return ValueByIndex(7, 7)
            End Get
            Set(ByVal Value As Boolean)
                ValueByIndex(7, 7) = Value
            End Set
        End Property

        Public ReadOnly Property UnknownAttributes() As String
            Get
                If MajorType >= 5 Then Return "0"
                If MajorType >= 16 Then Return "1"
                If Byte2 <> 0 Then Return "2"
                If Byte3 <> 0 Then Return "3"

                If ValueByIndex(6, 4) Then Return "6,4"
                If ValueByIndex(6, 5) Then Return "6,5"
                If ValueByIndex(6, 6) Then Return "6,6"

                If ValueByIndex(7, 0) Then Return "7,0"
                If ValueByIndex(7, 1) Then Return "7,1"
                If ValueByIndex(7, 2) Then Return "7,2"
                If ValueByIndex(7, 3) Then Return "7,3"
                If ValueByIndex(7, 4) Then Return "7,4"
                If ValueByIndex(7, 5) Then Return "7,5"

                Return "None"
            End Get
        End Property
        Public ReadOnly Property UnfoundAttributes() As String
            Get
                If MajorType >= 5 Then Return "0"
                If MinorType >= 16 Then Return "1"
                If Byte2 <> 0 Then Return "2"
                If Byte3 <> 0 Then Return "3"

                If ValueByIndex(6, 4) Then Return "6,4"
                If ValueByIndex(6, 5) Then Return "6,5"
                If ValueByIndex(6, 6) Then Return "6,6"

                If ValueByIndex(7, 0) Then Return "7,0"
                If ValueByIndex(7, 1) Then Return "7,1"
                If ValueByIndex(7, 2) Then Return "7,2"
                If ValueByIndex(7, 3) Then Return "7,3"
                If ValueByIndex(7, 4) Then Return "7,4"
                If ValueByIndex(7, 5) Then Return "7,5"

                Return "None"
            End Get
        End Property
    End Structure
    Public Class TerrainInfoEncoder
        Inherits Xml.Mapper(Of TerrainInfo, String)

        Public Overloads Overrides Function GetMappedObject(ByVal o As TerrainInfo) As String
            Return CSU(o.ValueInt).ToString("X16", Globalization.CultureInfo.InvariantCulture)
        End Function
        Public Overloads Overrides Function GetInverseMappedObject(ByVal o As String) As TerrainInfo
            Return New TerrainInfo With {.ValueInt = CUS(UInt64.Parse(o, Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture))}
        End Function
    End Class

    Public Class Mesh
        ReadOnly Property NumDistrict() As Int32
            Get
                If DistrictIndex Is Nothing Then Return 0
                Return DistrictIndex.Count
            End Get
        End Property
        Public DistrictIndex As New List(Of Int32)
        Public Sub New()
        End Sub
        Public Sub New(ByVal DistrictIndex As Int32())
            Me.DistrictIndex.AddRange(DistrictIndex)
        End Sub
        Sub New(ByVal s As StreamEx)
            Dim Num As Integer = s.ReadInt32
            If Num <= 0 Then Return
            For n As Integer = 0 To Num - 1
                DistrictIndex.Add(s.ReadInt32)
            Next
        End Sub
        Sub WriteTo(ByVal s As StreamEx)
            s.WriteInt32(NumDistrict)
            For n As Integer = 0 To NumDistrict - 1
                s.WriteInt32(DistrictIndex(n))
            Next
        End Sub
    End Class

    Private Class Exporter
        Public Sub ToObj(ByVal ObjPath As String, ByVal m As SEC)
            Dim Dir As String = GetFileDirectory(ObjPath)
            If Not Directory.Exists(Dir) Then Directory.CreateDirectory(Dir)

            Dim Blocks As New List(Of String)
            Blocks.Add(String.Format("# NumPoint: {0}", m.NumPoint))
            Blocks.Add(String.Format("# NumBorder: {0}", m.NumBorder))
            Blocks.Add(String.Format("# NumDistrict: {0}", m.NumDistrict))
            Blocks.Add(String.Format("# NumSpecialDistrict: {0}", m.NumSpecialDistrict))
            Blocks.Add("")

            Dim PointList As New List(Of Vector)
            Dim DistrictList As New List(Of Int16())

            Dim n As Int16 = 1S
            For Each d In m.Districts
                Dim pl As New List(Of Int16)
                For Each b In d.Border
                    Dim xy = m.Points(m.Borders(b).StartPointIndex)
                    Dim x = xy.x
                    Dim y = xy.y
                    Dim z = d.kx * x + d.ky * y + d.bz
                    PointList.Add(New Vector(x, y, z))
                    pl.Add(n)
                    n += 1S
                Next
                DistrictList.Add(pl.ToArray)
            Next

            For Each p In PointList
                Blocks.Add(String.Format(System.Globalization.NumberFormatInfo.InvariantInfo, "v {0:r} {1:r} {2:r}", p(0), p(1), p(2)))
            Next
            Blocks.Add("")

            For Each pl In DistrictList
                Blocks.Add("f " & String.Join(" ", (From pi In pl Select pi.ToString(Globalization.CultureInfo.InvariantCulture)).ToArray()))
            Next
            Blocks.Add("")

            Txt.WriteFile(ObjPath, UTF8, String.Join(System.Environment.NewLine, Blocks.ToArray))
        End Sub
    End Class
    Private Class Importer
        Private Shared Num As String = "(\d|\-|\.|e|E)+"
        Private Shared RegexV As New Regex("^v( +(?<Coordinate>" & Num & "))+$", RegexOptions.ExplicitCapture)
        Private Shared RegexF As New Regex("^f( +((?<v>" & Num & ")(/(?<vt>" & Num & "))?(/(?<vt>" & Num & "))?))+$", RegexOptions.ExplicitCapture)
        Private Shared RegexUsemtl As New Regex("^usemtl +(?<Name>[0-9A-Fa-f]{16})$", RegexOptions.ExplicitCapture)

        Private PointList As New List(Of Vector)
        Private DistrictList As New List(Of KeyValuePair(Of Int64, Int16()))
        Private CurrentTerrain As Int64 = (New TerrainInfo(TerrainInfo.MajorTypes.TIERRA, TerrainInfo.MinorTypes.ASFALTO) With {.Transitable = True}).ValueInt

        Private Line As String
        Public Function FromObj(ByVal ObjPath As String) As SEC
            Dim PreviousCurrentDirectory As String = System.Environment.CurrentDirectory
            System.Environment.CurrentDirectory = GetFileDirectory(ObjPath)
            Dim LastMatch As Integer
            Using Obj As New StreamReader(ObjPath, System.Text.Encoding.UTF8)
                While Not Obj.EndOfStream
                    Line = Obj.ReadLine()
                    If Line <> "" Then Line = Line.Trim

                    Select Case LastMatch
                        Case 1
                            If CompareV() Then Continue While
                        Case 3
                            If CompareF() Then Continue While
                        Case 5
                            If CompareUsemtl() Then Continue While
                    End Select

                    If LastMatch <> 1 Then
                        If CompareV() Then
                            LastMatch = 1
                            Continue While
                        End If
                    End If

                    If LastMatch <> 3 Then
                        If CompareF() Then
                            LastMatch = 3
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
            End Using
            System.Environment.CurrentDirectory = PreviousCurrentDirectory

            Dim Points As New List(Of PointInfo) '优化后的点
            Dim PointDict As New Dictionary(Of PointInfo, Integer)(PointInfoComparer.Default) '点->点索引
            Dim PointPointMap As New List(Of Integer) '优化前的点->优化后的点
            Dim PointIndex As Integer = 0
            For Each p In PointList
                Dim Point As New PointInfo With {.x = CSng(p(0)), .y = CSng(p(1))}
                If PointDict.ContainsKey(Point) Then
                    PointPointMap.Add(PointDict(Point))
                Else
                    Points.Add(Point)
                    PointDict.Add(Point, PointIndex)
                    PointPointMap.Add(PointIndex)
                    PointIndex += 1
                End If
            Next
            Dim PointDistrictMap = New List(Of Integer)(Points.Count - 1) {} '优化后的点->包含该点的区域
            Dim Borders As New List(Of BorderInfo) '边
            Dim Districts As New List(Of DistrictInfo) '区域
            Dim DistrictDistrictMap As New List(Of Integer) '优化后的面->优化前的面
            Dim DistrictIndex As Integer = 0
            For n = 0 To DistrictList.Count - 1
                Dim dv = DistrictList(n).Value

                If dv.Length < 3 Then Continue For

                '将优化前顶点索引转化成优化后顶点索引，如果存在相同点(z分量不算)，则判定为竖直面，忽略
                Dim dvm = New Integer(dv.Length - 1) {} '区域的优化后的点索引
                Dim dvmDict As New Dictionary(Of Integer, Integer) '区域的优化后的点索引 -> 区域上的点号
                Dim Same As Boolean = False
                For i = 0 To dv.Length - 1
                    dvm(i) = PointPointMap(dv(i))
                    If dvmDict.ContainsKey(dvm(i)) Then
                        Same = True
                        Exit For
                    Else
                        dvmDict.Add(dvm(i), i)
                    End If
                Next
                If Same Then Continue For

                Dim d = GetDistrict(dv, dvm, Districts.Count, Borders, DistrictList(n).Key)
                If d Is Nothing Then Continue For
                Districts.Add(d)
                DistrictDistrictMap.Add(n)

                '将面加入到 顶点->面 表
                Dim IgnoreList As New Dictionary(Of Integer, Integer)
                For Each Index In dvm
                    If PointDistrictMap(Index) Is Nothing Then
                        PointDistrictMap(Index) = New List(Of Integer)
                        PointDistrictMap(Index).Add(DistrictIndex)
                    Else
                        '如果已存在顶点相同的面，则判断是否重叠
                        Dim Replaced As Boolean = False
                        For i = 0 To PointDistrictMap(Index).Count - 1
                            Dim pn2 = PointDistrictMap(Index)(i)
                            If IgnoreList.ContainsKey(pn2) Then Continue For
                            IgnoreList.Add(pn2, 0)
                            Dim NumSame As Integer = 0
                            Dim dv2 = DistrictList(DistrictDistrictMap(pn2)).Value
                            Dim dvm2 = New Integer(dv2.Length - 1) {}
                            For j = 0 To dvm2.Length - 1
                                dvm2(j) = PointPointMap(dv2(j))
                            Next
                            Dim CommonPoints As New List(Of Integer)
                            Dim CommonPoints2 As New List(Of Integer)
                            For p = 0 To dv2.Length - 1
                                If dvmDict.ContainsKey(dvm2(p)) Then
                                    NumSame += 1
                                    CommonPoints.Add(dvmDict(dvm2(p)))
                                    CommonPoints2.Add(p)
                                End If
                            Next

                            '如果两区域有三个以上的公共点
                            '则两区域重叠
                            Dim Overlapped As Boolean = NumSame >= 3

                            '如果两区域有两个公共点，且有一区域两公共点在区域内不相邻
                            '则两区域重叠
                            If Not Overlapped AndAlso NumSame >= 2 Then
                                If Not Connect(CommonPoints2(0), CommonPoints2(1), dv2.Length) OrElse Not Connect(CommonPoints(0), CommonPoints(1), dv.Length) Then
                                    Overlapped = True
                                End If
                            End If

                            '从现在起两个公共点之间存在一条边
                            '如果两区域有两个公共点，而第三对点在两点连线异侧，
                            '则两区域相邻，否则重叠
                            If Not Overlapped AndAlso NumSame >= 2 Then
                                Dim c1 As Integer = dvm2(CommonPoints2(0))
                                Dim c2 As Integer = dvm2(CommonPoints2(1))
                                Dim a As Integer
                                Dim b As Integer
                                For j = 0 To dvm.Length - 1
                                    If dvm(j) <> c1 AndAlso dvm(j) <> c2 Then
                                        a = dvm(j)
                                        Exit For
                                    End If
                                Next
                                For j = 0 To dvm2.Length - 1
                                    If dvm2(j) <> c1 AndAlso dvm2(j) <> c2 Then
                                        b = dvm2(j)
                                        Exit For
                                    End If
                                Next

                                '求第三对点是否在公共边两侧
                                '(c1a x c1c2) . (c1b x c1c2)
                                '由Lagrange Formula: (a x b) . (c x d) = (a . c) (b . d) - (a . d) (b . c)，有
                                '(c1a . c1b) (c1c2 ^ 2) - (c1a . c1c2) (c1b . c1c2)
                                Dim c1x As Double = Points(c1).x
                                Dim c1y As Double = Points(c1).y
                                Dim c2x As Double = Points(c2).x
                                Dim c2y As Double = Points(c2).y
                                Dim ax As Double = Points(a).x
                                Dim ay As Double = Points(a).y
                                Dim bx As Double = Points(b).x
                                Dim by As Double = Points(b).y
                                Dim c1ax As Double = ax - c1x
                                Dim c1ay As Double = ay - c1y
                                Dim c1c2x As Double = c2x - c1x
                                Dim c1c2y As Double = c2y - c1y
                                Dim c1bx As Double = bx - c1x
                                Dim c1by As Double = by - c1y
                                Dim sign As Double = (c1ax * c1bx + c1ay * c1by) * (c1c2x ^ 2 + c1c2y ^ 2) - (c1ax * c1c2x + c1ay * c1c2y) * (c1bx * c1c2x + c1by * c1c2y)
                                Overlapped = (sign >= 0)

                                '如果两区域有两个公共点，而第三对点在两点连线异侧，两区域相邻，
                                '则连好两区域的公共边
                                If Not Overlapped Then
                                    Dim b1 = Borders(d.Border(GetBorderIndex(CommonPoints(0), CommonPoints(1), dv.Length)))

                                    Dim d2 = Districts(pn2)
                                    Dim b2 = Borders(d2.Border(GetBorderIndex(CommonPoints2(0), CommonPoints2(1), dv2.Length)))

                                    If b2.NeighborDistrictIndex = -1 Then
                                        b1.NeighborDistrictIndex = pn2
                                        b2.NeighborDistrictIndex = DistrictIndex
                                    End If
                                End If
                            End If

                            '如果两区域重叠
                            '则将近似的高的区域置为分离区域(特殊区域)，从 顶点->面 表删除，低的为普通区域
                            If Overlapped Then
                                If d.MaxPz >= Districts(pn2).MaxPz Then
                                    '默认为分离区域 Borders(d.Border(j)).NeighborDistrictIndex = -1
                                Else
                                    Dim d2 = Districts(pn2)
                                    For j = 0 To CommonPoints2.Count - 1
                                        If Not Connect(CommonPoints2(j), CommonPoints2((j + 1) Mod CommonPoints2.Count), dv2.Length) Then
                                            Continue For
                                        End If
                                        If Not Connect(CommonPoints(j), CommonPoints((j + 1) Mod CommonPoints2.Count), dv.Length) Then
                                            Continue For
                                        End If
                                        Dim b2 = Borders(GetBorderIndex(CommonPoints2(j), CommonPoints2((j + 1) Mod CommonPoints2.Count), dv2.Length))
                                        If b2.NeighborDistrictIndex = -1 Then Continue For
                                        Dim Neighbor = Districts(b2.NeighborDistrictIndex)
                                        Dim b1 = Borders(GetBorderIndex(CommonPoints(j), CommonPoints((j + 1) Mod CommonPoints2.Count), dv.Length))
                                        For k = 0 To Neighbor.n - 1
                                            If Borders(Neighbor.Border(k)).NeighborDistrictIndex = pn2 Then
                                                Borders(Neighbor.Border(k)).NeighborDistrictIndex = DistrictIndex
                                                Exit For
                                            End If
                                        Next
                                        b1.NeighborDistrictIndex = b2.NeighborDistrictIndex
                                        b2.NeighborDistrictIndex = -1
                                    Next
                                    For j = 0 To dv2.Length - 1
                                        Dim b2 = Borders(j)
                                        If b2.NeighborDistrictIndex = -1 Then Continue For
                                        Dim Neighbor = Districts(b2.NeighborDistrictIndex)
                                        For k = 0 To Neighbor.n - 1
                                            If Borders(Neighbor.Border(k)).NeighborDistrictIndex = pn2 Then
                                                Borders(Neighbor.Border(k)).NeighborDistrictIndex = -1
                                                Exit For
                                            End If
                                        Next
                                        b2.NeighborDistrictIndex = -1
                                    Next
                                    'For j = 0 To d2.Border.Length - 1
                                    '    If Borders(d2.Border(j)).NeighborDistrictIndex >= 0 Then
                                    '        Dim Neighbor = Districts(Borders(d2.Border(j)).NeighborDistrictIndex)
                                    '        For k = 0 To Neighbor.n - 1
                                    '            If Borders(Neighbor.Border(k)).NeighborDistrictIndex = pn Then
                                    '                Borders(Neighbor.Border(k)).NeighborDistrictIndex = DistrictIndex
                                    '                Exit For
                                    '            End If
                                    '        Next
                                    '        Borders(d.Border(j)).NeighborDistrictIndex = Borders(d2.Border(j)).NeighborDistrictIndex
                                    '        Borders(d2.Border(j)).NeighborDistrictIndex = -1
                                    '    End If
                                    'Next
                                    PointDistrictMap(Index)(i) = DistrictIndex
                                End If
                                Replaced = True
                                Exit For
                            End If
                        Next
                        If Replaced Then Continue For

                        PointDistrictMap(Index).Add(DistrictIndex)
                    End If
                Next
                DistrictIndex += 1
            Next

            Dim s As New SEC With {.VersionSign = SEC.Version.Comm2, .Points = Points.ToArray, .Borders = Borders.ToArray, .Districts = Districts.ToArray}
            s.GenerateMesh()
            s.RecalculateNumSpecialDistrict()
            Return s
        End Function

        Private Function Connect(ByVal PointA As Integer, ByVal PointB As Integer, ByVal NumVertexInDistrict As Integer) As Boolean
            Dim Diff As Integer = Abs(PointA - PointB)
            Return Diff = 1 OrElse Diff = NumVertexInDistrict - 1
        End Function

        Private Function GetBorderIndex(ByVal PointA As Integer, ByVal PointB As Integer, ByVal NumVertexInDistrict As Integer) As Integer
            Dim Diff As Integer = Abs(PointA - PointB)
            If Diff <= 1 Then Return Min(PointA, PointB)
            Return Max(PointA, PointB)
        End Function

        Private Function GetDistrict(ByVal dv As Int16(), ByVal dvm As Integer(), ByVal Index As Integer, ByVal Borders As List(Of BorderInfo), ByVal Terrain As Int64) As DistrictInfo
            Dim d As New DistrictInfo
            With d
                .MinPx = Single.MaxValue
                .MinPy = Single.MaxValue
                .MinPz = Single.MaxValue
                .MaxPx = Single.MinValue
                .MaxPy = Single.MinValue
                .MaxPz = Single.MinValue
                For n = 0 To dv.Length - 1
                    Dim v As Vector = PointList(dv(n))
                    If Double.IsInfinity(v.Sqr) Then Return Nothing
                    If Double.IsNaN(v.Sqr) Then Return Nothing

                    If v(0) < .MinPx Then .MinPx = CSng(v(0))
                    If v(0) > .MaxPx Then .MaxPx = CSng(v(0))
                    If v(1) < .MinPy Then .MinPy = CSng(v(1))
                    If v(1) > .MaxPy Then .MaxPy = CSng(v(1))
                    If v(2) < .MinPz Then .MinPz = CSng(v(2))
                    If v(2) > .MaxPz Then .MaxPz = CSng(v(2))
                Next

                '如果疑似垂直地面则删去
                If Equal(.MaxPx - .MinPx, 0) Then Return Nothing
                If Equal(.MaxPy - .MinPy, 0) Then Return Nothing
                For n = 0 To dv.Length - 1
                    For m = 0 To dv.Length - 1
                        If n = m Then Continue For
                        Dim v1 As Vector = PointList(dv(n))
                        Dim v2 As Vector = PointList(dv(m))
                        If Equal((v1 - v2).Part(2).Sqr, 0, -2) Then Return Nothing
                    Next
                Next

                SolveFaceParameters(dv, .kx, .ky, .bz)
                Dim IsPlane As Boolean = Equal(.kx, 0) AndAlso Equal(.ky, 0)
                If IsPlane Then
                    .kx = 0
                    .ky = 0
                End If

                .Border = New Int32(dv.Length - 1) {}
                For n As Integer = 0 To .n - 1
                    Dim Border As New BorderInfo() With {.StartPointIndex = dvm(n), .EndPointIndex = dvm((n + 1) Mod d.n), .ParentDistrictID = Index, .NeighborDistrictIndex = -1}
                    .Border(n) = Borders.Count
                    Borders.Add(Border)
                Next

                .Terrain = New TerrainInfo(Terrain) With {.IsPlane = IsPlane}
            End With
            Return d
        End Function

        Private Sub SolveFaceParameters(ByVal dv As Int16(), ByRef kx As Single, ByRef ky As Single, ByRef bz As Single)
            If dv.Length = 3 Then
                '如果是三个点直接解三元线性方程组

                'z = kx * x + ky * y + bz
                'kx * x1 + ky * y1 + bz = z1
                'kx * x2 + ky * y2 + bz = z2
                'kx * x3 + ky * y3 + bz = z3
                'M * (kx; ky; kz) = H
                'M = U * S * V'
                '(kx; ky; kz) = V * S^-1 * U' * H

                Dim M As New Matrix(New Double(,) {{CDec(PointList(dv(0))(0)), CDec(PointList(dv(0))(1)), 1}, {CDec(PointList(dv(1))(0)), CDec(PointList(dv(1))(1)), 1}, {CDec(PointList(dv(2))(0)), CDec(PointList(dv(2))(1)), 1}})
                Dim H As New Vector(PointList(dv(0))(2), PointList(dv(1))(2), PointList(dv(2))(2))
                Dim U As Matrix = Nothing
                Dim S As Matrix = Nothing
                Dim V As Matrix = Nothing
                M.SingularValueDecomposition(U, S, V)

                For i = 0 To S.Dimension - 1
                    If S.Element(CByte(i), CByte(i)) > 2 ^ -10 Then
                        S.Element(CByte(i), CByte(i)) = 1 / S.Element(CByte(i), CByte(i))
                    Else
                        S.Element(CByte(i), CByte(i)) = 0
                    End If
                Next

                Dim X As Vector = V * (S * (U.Transpose * H))
                kx = CSng(X(0))
                ky = CSng(X(1))
                bz = CSng(X(2))
            Else
                '否则使用最小二乘法

                'z = kx * x + ky * y + bz
                'kx * Sigma(xi ^ 2) + ky * Sigma(xi * yi) + bz * Sigma(xi) = Sigma(xi * zi)
                'kx * Sigma(xi * yi) + ky * Sigma(yi ^ 2) + bz * Sigma(yi) = Sigma(yi * zi)
                'kx * Sigma(xi) + ky * Sigma(yi) + bz * n = Sigma(zi)
                'M * (kx; ky; bz) = H
                'M = U * S * V'
                '(kx; ky; bz) = V * S^-1 * U' * H

                Dim SX2 As Double
                Dim SY2 As Double
                Dim SXY As Double
                Dim SYZ As Double
                Dim SXZ As Double
                Dim SX As Double
                Dim SY As Double
                Dim SZ As Double
                For i = 0 To dv.Length - 1
                    Dim p As Vector = PointList(dv(i))
                    SX2 += CDec(p(0) ^ 2)
                    SY2 += CDec(p(1) ^ 2)
                    SXY += CDec(p(0) * p(1))
                    SYZ += CDec(p(1) * p(2))
                    SXZ += CDec(p(0) * p(2))
                    SX += CDec(p(0))
                    SY += CDec(p(1))
                    SZ += CDec(p(2))
                Next
                Dim M As New Matrix(New Double(,) {{SX2, SXY, SX}, {SXY, SY2, SY}, {SX, SY, dv.Length}})
                Dim H As New Vector(SXZ, SYZ, SZ)
                Dim U As Matrix = Nothing
                Dim S As Matrix = Nothing
                Dim V As Matrix = Nothing
                M.SingularValueDecomposition(U, S, V)

                For i = 0 To S.Dimension - 1
                    If S.Element(CByte(i), CByte(i)) <> 0 Then
                        S.Element(CByte(i), CByte(i)) = 1 / S.Element(CByte(i), CByte(i))
                    Else
                        S.Element(CByte(i), CByte(i)) = 0
                    End If
                Next

                Dim X As Vector = V * (S * (U.Transpose * H))
                kx = CSng(X(0))
                ky = CSng(X(1))
                bz = CSng(X(2))
            End If
        End Sub

        Private Function CompareV() As Boolean
            Dim MatchV = RegexV.Match(Line)
            If MatchV.Success Then
                Dim Point As New Vector(3)
                Dim Captures = MatchV.Groups("Coordinate").Captures

                If Captures.Count >= 1 Then Point(0) = Single.Parse(Captures(0).Value, Globalization.CultureInfo.InvariantCulture)
                If Captures.Count >= 2 Then Point(1) = Single.Parse(Captures(1).Value, Globalization.CultureInfo.InvariantCulture)
                If Captures.Count >= 3 Then Point(2) = Single.Parse(Captures(2).Value, Globalization.CultureInfo.InvariantCulture)
                PointList.Add(Point)
                Return True
            End If
            Return False
        End Function
        Private Function CompareF() As Boolean
            Dim MatchF = RegexF.Match(Line)
            If MatchF.Success Then
                Dim CapturesVertices = MatchF.Groups("v").Captures
                Dim Num = CapturesVertices.Count
                Dim Vertices = New Int16(Num - 1) {}
                For n = 0 To Num - 1
                    Vertices(n) = CShort(CInt(CapturesVertices(n).Value) - 1)
                Next
                'Array.Reverse(Vertices)

                DistrictList.Add(New KeyValuePair(Of Int64, Int16())(CurrentTerrain, Vertices))
                Return True
            End If
            Return False
        End Function
        Private Function CompareUsemtl() As Boolean
            Dim MatchUsemtl = RegexUsemtl.Match(Line)
            If MatchUsemtl.Success Then
                Dim Name = MatchUsemtl.Result("${Name}")
                CurrentTerrain = DirectIntConvert.CUS(Reverse(UInt64.Parse(Name, Globalization.NumberStyles.HexNumber)))
                Return True
            End If
            Return False
        End Function
        Private Function Reverse(ByVal i As UInt64) As UInt64
            Return ((i And &HFFUL) << 56) Or ((i And &HFF00UL) << 40) Or ((i And &HFF0000UL) << 24) Or ((i And &HFF000000UL) << 8) Or ((i And &HFF00000000UL) >> 8) Or (((i And &HFF0000000000UL) >> 24) And &HFFUL) Or ((i And &HFF000000000000UL) >> 40) Or (((i And &HFF00000000000000UL) >> 56) And &HFFUL)
        End Function

        Public Class PointInfoComparer
            Implements IEqualityComparer(Of PointInfo)

            Private Shared Radius As Single = 0.0005

            Public Function Equal(ByVal Left As PointInfo, ByVal Right As PointInfo) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of PointInfo).Equals
                Return (Left.x - Right.x) * (Left.x - Right.x) + (Left.y - Right.y) * (Left.y - Right.y) < 4 * Radius * Radius
            End Function

            Private Function HashCode(ByVal s As Single) As Integer
                If Single.IsInfinity(s) Then Return s.GetHashCode()
                If Single.IsNaN(s) Then Return s.GetHashCode()
                Dim m = Math.Truncate(s / Radius + 0.5) * Radius
                If (m - s) * (m - s) >= 4 * Radius * Radius Then Return s.GetHashCode()
                Return m.GetHashCode()
            End Function

            Public Function HashCode(ByVal obj As PointInfo) As Integer Implements System.Collections.Generic.IEqualityComparer(Of PointInfo).GetHashCode
                Return (HashCode(obj.x) << 8) Xor ((HashCode(obj.y) >> 8) And &HFFFFFF)
            End Function

            Public Shared ReadOnly Property [Default]() As PointInfoComparer
                Get
                    Static Value As PointInfoComparer
                    If Value IsNot Nothing Then Return Value
                    Value = New PointInfoComparer
                    Return Value
                End Get
            End Property
        End Class
    End Class
End Class
