'==========================================================================
'
'  File:        SEC_Simple.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: SEC文件类
'  Version:     2023.01.30.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
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

        Public Function TranslateProjectorToProjectorDomain(Of R)(ByVal Projector As Func(Of String, R)) As Func(Of SEC.TerrainInfo, R) Implements IProjectorToProjectorDomainTranslator(Of SEC.TerrainInfo, String).TranslateProjectorToProjectorDomain
            Dim f =
                Function(o As SEC.TerrainInfo) As R
                    Dim s = o.ValueInt.ToString("X16", Globalization.CultureInfo.InvariantCulture)
                    Return Projector(s)
                End Function
            Return f
        End Function
        Public Function TranslateProjectorToProjectorRange1(Of D)(ByVal Projector As Func(Of D, String)) As Func(Of D, SEC.TerrainInfo) Implements IProjectorToProjectorRangeTranslator(Of SEC.TerrainInfo, String).TranslateProjectorToProjectorRange
            Dim f =
                Function(o As D) As SEC.TerrainInfo
                    Dim s = Projector(o)
                    Return New SEC.TerrainInfo With {.ValueInt = CUS(UInt64.Parse(s, Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture))}
                End Function
            Return f
        End Function
    End Class
    Public Shared ReadOnly Property XmlSerializer() As XmlSerializer
        Get
            Return xs
        End Get
    End Property

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
        f.Zones = SEC.Zones.ToList()
        f.Points = SEC.Points.Select(Function(p) New PointInfo With {.x = p.x, .y = p.y}).ToList()
        Dim Borders = SEC.Borders.Select(Function(b) New BorderInfo With {.StartPointIndex = b.StartPointIndex, .EndPointIndex = b.EndPointIndex, .NeighborDistrictIndex = b.NeighborDistrictIndex, .Adjacency = b.Adjacency}).ToArray()
        f.Districts = SEC.Districts.Select(Function(d) New DistrictInfo With {.Terrain = d.Terrain, .kx = d.kx, .ky = d.ky, .bz = d.bz, .Unknown1 = d.Unknown1, .ZoneFlags = d.ZoneFlags, .Unknown2 = d.Unknown2, .Borders = d.Border.Select(Function(b) Borders(b)).ToList()}).ToList()
        Return f
    End Function

    Private Shared Function GenerateDistrict(ByVal f As FileInfo, ByVal d As DistrictInfo, ByVal i As Integer, ByVal DistrictBorders As Integer()) As SEC_Complex.DistrictInfo
        Dim MinPx = Single.MaxValue
        Dim MinPy = Single.MaxValue
        Dim MinPz = Single.MaxValue
        Dim MaxPx = Single.MinValue
        Dim MaxPy = Single.MinValue
        Dim MaxPz = Single.MinValue
        If d.Borders.Count = 0 Then
            Return New SEC_Complex.DistrictInfo With
            {
                .Terrain = d.Terrain,
                .kx = d.kx,
                .ky = d.ky,
                .bz = d.bz,
                .Unknown1 = d.Unknown1,
                .ZoneFlags = d.ZoneFlags,
                .Unknown2 = d.Unknown2,
                .MinPx = 0,
                .MinPy = 0,
                .MinPz = 0,
                .MaxPx = 0,
                .MaxPy = 0,
                .MaxPz = 0,
                .Border = New Integer() {}
            }
        End If
        For Each p In d.Borders.SelectMany(Function(b) New Integer() {b.StartPointIndex, b.EndPointIndex}).Distinct.Select(Function(pIndex) f.Points(pIndex))
            Dim x = p.x
            Dim y = p.y
            Dim z = d.GetZ(x, y)

            If x < MinPx Then MinPx = CSng(x)
            If x > MaxPx Then MaxPx = CSng(x)
            If y < MinPy Then MinPy = CSng(y)
            If y > MaxPy Then MaxPy = CSng(y)
            If z < MinPz Then MinPz = CSng(z)
            If z > MaxPz Then MaxPz = CSng(z)
        Next
        Return New SEC_Complex.DistrictInfo With
        {
            .Terrain = d.Terrain,
            .kx = d.kx,
            .ky = d.ky,
            .bz = d.bz,
            .Unknown1 = d.Unknown1,
            .ZoneFlags = d.ZoneFlags,
            .Unknown2 = d.Unknown2,
            .MinPx = MinPx,
            .MinPy = MinPy,
            .MinPz = MinPz,
            .MaxPx = MaxPx,
            .MaxPy = MaxPy,
            .MaxPz = MaxPz,
            .Border = d.Borders.Select(Function(b, k) DistrictBorders(i) + k).ToArray()
        }
    End Function
    Public Shared Function SimpleToComplex(ByVal f As FileInfo) As SEC
        Dim SEC As New SEC
        SEC.VersionSign = f.VersionSign
        SEC.Zones = f.Zones.ToArray()
        SEC.Points = f.Points.Select(Function(p) New SEC_Complex.PointInfo With {.x = p.x, .y = p.y}).ToArray()
        SEC.Borders = f.Districts.SelectMany(Function(d, i) d.Borders.Select(Function(b) New SEC_Complex.BorderInfo With {.StartPointIndex = b.StartPointIndex, .EndPointIndex = b.EndPointIndex, .ParentDistrictID = i, .NeighborDistrictIndex = b.NeighborDistrictIndex, .Adjacency = b.Adjacency})).ToArray()
        Dim DistrictBorders = GetSummation(0, f.Districts.Select(Function(d) d.Borders.Count).ToArray())
        SEC.Districts = f.Districts.Select(Function(d, i) GenerateDistrict(f, d, i, DistrictBorders)).ToArray()
        SEC.GenerateMesh()
        SEC.RecalculateNumSpecialDistrict()
        Return SEC
    End Function

    Public Class FileInfo
        Public VersionSign As SEC_Complex.Version
        Public Zones As List(Of String)

        Public Points As List(Of PointInfo)
        Public Districts As List(Of DistrictInfo)
    End Class

    <ComponentModel.TypeConverter(GetType(RecordExpandableObjectConverter))>
    Public Class PointInfo
        Public x As Single
        Public y As Single

        Public Overrides Function ToString() As String
            Return "({0}, {1})".Formats(x.ToInvariantString(), y.ToInvariantString())
        End Function
    End Class

    <ComponentModel.TypeConverter(GetType(RecordExpandableObjectConverter))>
    Public Class BorderInfo
        Public StartPointIndex As Integer
        Public EndPointIndex As Integer
        Public NeighborDistrictIndex As Integer = -1
        Public Adjacency As UInt32

        Public Overrides Function ToString() As String
            Return "{0}->{1}".Formats(StartPointIndex.ToInvariantString(), EndPointIndex.ToInvariantString())
        End Function
    End Class

    <ComponentModel.TypeConverter(GetType(RecordExpandableObjectConverter))>
    Public Class DistrictInfo
        Public ReadOnly Property n() As Int32
            Get
                If Borders Is Nothing Then Return 0
                Return Borders.Count
            End Get
        End Property

        <ComponentModel.TypeConverter(GetType(TerrainInfoConverter))>
        Public Terrain As SEC_Complex.TerrainInfo
        Public kx As Single
        Public ky As Single
        Public bz As Single
        <ComponentModel.TypeConverter(GetType(UInt64Converter))>
        Public Unknown1 As UInt64
        <ComponentModel.TypeConverter(GetType(UInt64Converter))>
        Public ZoneFlags As UInt64
        <ComponentModel.TypeConverter(GetType(UInt64Converter))>
        Public Unknown2 As UInt64
        Public Borders As List(Of BorderInfo)

        Public Function GetZ(ByVal x As Double, ByVal y As Double) As Double
            '平面方程 z = kx * x + ky * y + bz
            Return kx * x + ky * y + bz
        End Function

        Public Overrides Function ToString() As String
            Return "n = {0}, kx = {1}, ky = {2}, bz = {3}".Formats(n.ToInvariantString(), kx.ToInvariantString(), ky.ToInvariantString(), bz.ToInvariantString())
        End Function
    End Class

    Protected Friend Class UInt32Converter
        Inherits ComponentModel.TypeConverter
        Public Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
            If (sourceType Is GetType(String)) Then Return True
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function
        Public Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As System.Type) As Boolean
            If (destinationType Is GetType(String)) Then Return True
            Return MyBase.CanConvertTo(context, destinationType)
        End Function
        Private Shared Function Reverse(ByVal i As UInt32) As UInt32
            Return (((i And &HFF000000UI) >> 24) And &HFFUI) Or ((i And &HFF0000UI) >> 8) Or ((i And &HFFUI) << 24) Or ((i And &HFF00UI) << 8)
        End Function
        Public Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object
            If (TypeOf value Is String) Then
                Dim s = DirectCast(value, String)
                Return Reverse(UInt32.Parse(s, Globalization.NumberStyles.HexNumber))
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function
        Public Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
            If (destinationType Is GetType(String)) Then
                Dim v = Reverse(DirectCast(value, UInt32))
                Return v.ToString("X8", System.Globalization.CultureInfo.InvariantCulture)
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function GetPropertiesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return False
        End Function

        Public Overrides Function GetStandardValuesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return True
        End Function
    End Class

    Protected Friend Class UInt64Converter
        Inherits ComponentModel.TypeConverter
        Public Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
            If (sourceType Is GetType(String)) Then Return True
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function
        Public Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As System.Type) As Boolean
            If (destinationType Is GetType(String)) Then Return True
            Return MyBase.CanConvertTo(context, destinationType)
        End Function
        Private Shared Function Reverse(ByVal i As UInt64) As UInt64
            Return (((i And &HFF00000000000000UL) >> 56) And &HFFUL) Or ((i And &HFF000000000000UL) >> 40) Or ((i And &HFF0000000000UL) >> 24) Or ((i And &HFF00000000UL) >> 8) Or ((i And &HFFUL) << 56) Or ((i And &HFF00UL) << 40) Or ((i And &HFF0000UL) << 24) Or ((i And &HFF000000UL) << 8)
        End Function
        Public Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object
            If (TypeOf value Is String) Then
                Dim s = DirectCast(value, String)
                Return Reverse(UInt64.Parse(s, Globalization.NumberStyles.HexNumber))
            End If
            Return MyBase.ConvertFrom(context, culture, value)
        End Function
        Public Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
            If (destinationType Is GetType(String)) Then
                Dim v = Reverse(DirectCast(value, UInt64))
                Return v.ToString("X16", System.Globalization.CultureInfo.InvariantCulture)
            End If
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function GetPropertiesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return False
        End Function

        Public Overrides Function GetStandardValuesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return True
        End Function
    End Class

    Protected Friend Class TerrainInfoConverter
        Inherits ComponentModel.ExpandableObjectConverter
        Public Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
            If (sourceType Is GetType(String)) Then Return True
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function
        Public Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As System.Type) As Boolean
            If (destinationType Is GetType(String)) Then Return True
            Return MyBase.CanConvertTo(context, destinationType)
        End Function
        Public Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object
            If (TypeOf value Is String) Then Return SEC_Complex.TerrainInfo.FromString(CType(value, String))
            Return MyBase.ConvertFrom(context, culture, value)
        End Function
        Public Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
            If (destinationType Is GetType(String)) Then Return value.ToString
            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overrides Function GetPropertiesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return True
        End Function

        Public Overrides Function GetStandardValuesSupported(ByVal context As ComponentModel.ITypeDescriptorContext) As Boolean
            Return True
        End Function
    End Class
End Class
