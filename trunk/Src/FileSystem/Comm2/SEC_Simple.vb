'==========================================================================
'
'  File:        SEC_Simple.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: SEC文件类
'  Version:     2013.01.18.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Firefly
Imports Firefly.Mapping
Imports Firefly.Mapping.XmlText
Imports Firefly.Texting
Imports SEC_Complex = FileSystem.SEC

Public NotInheritable Class SEC_Simple
    Public Shared Function Open(ByVal Path As String) As FileInfo
        Dim Sec As New SEC(Path)
        Return ComplexToSimple(Sec)
    End Function
    Public Shared Sub Save(ByVal Path As String, ByVal f As FileInfo)
        Dim Sec = SimpleToComplex(f)
        Sec.WriteToFile(Path)
    End Sub

    Private Shared xs As XmlSerializer
    Shared Sub New()
        xs = New XmlSerializer(True)
        Dim tit As New TerrainInfoTranslator
        xs.PutReaderTranslator(tit)
        xs.PutWriterTranslator(tit)
    End Sub
    Public Class TerrainInfoTranslator
        Implements IProjectorToProjectorDomainTranslator(Of SEC_Complex.TerrainInfo, String)
        Implements IProjectorToProjectorRangeTranslator(Of SEC_Complex.TerrainInfo, String)

        Public Function TranslateProjectorToProjectorDomain(Of R)(Projector As Func(Of String, R)) As Func(Of SEC.TerrainInfo, R) Implements IProjectorToProjectorDomainTranslator(Of SEC.TerrainInfo, String).TranslateProjectorToProjectorDomain
            Dim f =
                Function(o As SEC.TerrainInfo) As R
                    Dim s = o.ValueInt.ToString("X16", Globalization.CultureInfo.InvariantCulture)
                    Return Projector(s)
                End Function
            Return f
        End Function
        Public Function TranslateProjectorToProjectorRange1(Of D)(Projector As Func(Of D, String)) As Func(Of D, SEC.TerrainInfo) Implements IProjectorToProjectorRangeTranslator(Of SEC.TerrainInfo, String).TranslateProjectorToProjectorRange
            Dim f =
                Function(o As D) As SEC.TerrainInfo
                    Dim s = Projector(o)
                    Return New SEC.TerrainInfo With {.ValueInt = CUS(UInt64.Parse(s, Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture))}
                End Function
            Return f
        End Function
    End Class

    Public Shared Function OpenXml(ByVal Path As String) As FileInfo
        Dim x = XmlFile.ReadFile(Path)
        Dim f = xs.Read(Of FileInfo)(x)
        Return f
    End Function
    Public Shared Sub SaveXml(ByVal Path As String, ByVal f As FileInfo)
        Dim x = xs.Write(f)
        x.Name = "SEC"
        XmlFile.WriteFile(Path, x)
    End Sub

    Public Shared Function ComplexToSimple(ByVal SEC As SEC) As FileInfo
        Dim f As New FileInfo
        f.VersionSign = SEC.VersionSign
        f.Tokens = SEC.Tokens.ToList()
        f.Points = SEC.Points.Select(Function(p) New PointInfo With {.x = p.x, .y = p.y}).ToList()
        Dim Borders = SEC.Borders.Select(Function(b) New BorderInfo With {.StartPointIndex = b.StartPointIndex, .EndPointIndex = b.EndPointIndex, .NeighborDistrictIndex = b.NeighborDistrictIndex, .Unknown = b.Unknown}).ToArray()
        f.Districts = SEC.Districts.Select(Function(d) New DistrictInfo With {.Terrain = d.Terrain, .kx = d.kx, .ky = d.ky, .bz = d.bz, .UnknownData = d.UnknownData.ToArray(), .MinPx = d.MinPx, .MinPy = d.MinPy, .MinPz = d.MinPz, .MaxPx = d.MaxPx, .MaxPy = d.MaxPy, .MaxPz = d.MaxPz, .Borders = d.Border.Select(Function(b) Borders(b)).ToList()}).ToList()
        Return f
    End Function

    Public Shared Function SimpleToComplex(ByVal f As FileInfo) As SEC
        Dim SEC As New SEC
        SEC.VersionSign = f.VersionSign
        SEC.Tokens = f.Tokens.ToArray()
        SEC.Points = f.Points.Select(Function(p) New SEC_Complex.PointInfo With {.x = p.x, .y = p.y}).ToArray()
        SEC.Borders = f.Districts.SelectMany(Function(d, i) d.Borders.Select(Function(b) New SEC_Complex.BorderInfo With {.StartPointIndex = b.StartPointIndex, .EndPointIndex = b.EndPointIndex, .ParentDistrictID = i, .NeighborDistrictIndex = b.NeighborDistrictIndex, .Unknown = b.Unknown})).ToArray()
        Dim DistrictBorders = GetSummation(0, f.Districts.Select(Function(d) d.Borders.Count).ToArray())
        SEC.Districts = f.Districts.Select(Function(d, i) New SEC_Complex.DistrictInfo With {.Terrain = d.Terrain, .kx = d.kx, .ky = d.ky, .bz = d.bz, .UnknownData = d.UnknownData.ToArray(), .MinPx = d.MinPx, .MinPy = d.MinPy, .MinPz = d.MinPz, .MaxPx = d.MaxPx, .MaxPy = d.MaxPy, .MaxPz = d.MaxPz, .Border = d.Borders.Select(Function(b, k) DistrictBorders(i) + k).ToArray()}).ToArray()
        SEC.GenerateMesh()
        SEC.RecalculateNumSpecialDistrict()
        Return SEC
    End Function

    Public Class FileInfo
        Public VersionSign As SEC_Complex.Version
        Public Tokens As List(Of String)

        Public Points As List(Of PointInfo)
        Public Districts As List(Of DistrictInfo)
    End Class

    <ComponentModel.TypeConverter(GetType(RecordExpandableObjectConverter))> _
    Public Class PointInfo
        Public x As Single
        Public y As Single
    End Class

    Public Class BorderInfo
        Public StartPointIndex As Integer
        Public EndPointIndex As Integer
        Public NeighborDistrictIndex As Integer = -1
        Public Unknown As Integer
    End Class

    <ComponentModel.TypeConverter(GetType(RecordExpandableObjectConverter))> _
    Public Class DistrictInfo
        Public ReadOnly Property n() As Int32
            Get
                If Borders Is Nothing Then Return 0
                Return Borders.Count
            End Get
        End Property

        <ComponentModel.TypeConverter(GetType(SEC_Complex.TerrainInfoConverter))> _
        Public Terrain As SEC_Complex.TerrainInfo
        Public kx As Single
        Public ky As Single
        Public bz As Single
        Public UnknownData As Byte()
        Public MinPx As Single
        Public MinPy As Single
        Public MinPz As Single
        Public MaxPx As Single
        Public MaxPy As Single
        Public MaxPz As Single
        Public Borders As List(Of BorderInfo)

        Public Function GetZ(ByVal x As Double, ByVal y As Double) As Double
            '平面方程 z = kx * x + ky * y + bz
            Return kx * x + ky * y + bz
        End Function
    End Class
End Class
