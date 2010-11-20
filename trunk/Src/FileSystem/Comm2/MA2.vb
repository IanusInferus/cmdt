'==========================================================================
'
'  File:        MA2.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: MA2文件流类
'  Version:     2010.11.18.
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

            Dim NumObjectToRender As Int32() = New Int32(NumView - 1) {}
            sf.Position = ViewDataAddress
            ViewInfo = New ViewInfoBlock(NumView - 1) {}
            For n As Integer = 0 To NumView - 1
                ViewInfo(n) = New ViewInfoBlock(sf, NumObjectToRender(n), VersionSign)
            Next

            sf.Position = ObjectInfoAddress
            ObjectInfo = New ObjectInfoBlock(NumObject - 1) {}
            For n As Integer = 0 To NumObject - 1
                ObjectInfo(n) = New ObjectInfoBlock(sf)
            Next

            sf.Position = RenderIndexAddress
            RenderInfo = New RenderInfoBlock(NumView - 1)() {}
            For n As Integer = 0 To NumView - 1
                RenderInfo(n) = New RenderInfoBlock(NumObjectToRender(n) - 1) {}
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    RenderInfo(n)(m) = New RenderInfoBlock(sf)
                Next
            Next

            sf.Position = RenderDataAddress
            For n As Integer = 0 To NumView - 1
                For m As Integer = 0 To NumObjectToRender(n) - 1
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
            Dim RenderIndexOffset As Int32() = New Int32(NumView - 1) {}
            Dim RenderIndexLength As Int32() = New Int32(NumView - 1) {}
            Dim RenderDataOffset As Int32() = New Int32(NumView - 1) {}
            Dim RenderDataLength As Int32() = New Int32(NumView - 1) {}

            Dim pObjectInfo As Int32() = New Int32(NumObject - 1) {}
            Dim ObjectDistrictInfoOffset As Int32() = New Int32(NumObject - 1) {}
            Dim ObjectDistrictInfoLength As Int32() = New Int32(NumObject - 1) {}

            Dim pRenderIndex As Int32()() = New Int32(NumView - 1)() {}
            Dim RenderDataOffsetInIndex As Int32()() = New Int32(NumView - 1)() {}
            Dim RenderDataLengthInIndex As Int32()() = New Int32(NumView - 1)() {}

            ViewDataAddress = CInt(sf.Position)
            For n As Integer = 0 To NumView - 1
                pViewInfo(n) = CInt(sf.Position)
                ViewInfo(n).WriteToFile(sf, NumObjectToRender(n), VersionSign)
            Next

            ObjectInfoAddress = CInt(sf.Position)
            For n As Integer = 0 To NumObject - 1
                pObjectInfo(n) = CInt(sf.Position)
                ObjectInfo(n).Write(sf)
            Next

            RenderIndexAddress = CInt(sf.Position)
            For n As Integer = 0 To NumView - 1
                RenderIndexOffset(n) = CInt(sf.Position)
                pRenderIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    pRenderIndex(n)(m) = CInt(sf.Position)
                    RenderInfo(n)(m).Write(sf)
                Next
                RenderIndexLength(n) = CInt(sf.Position - RenderIndexOffset(n))
            Next
            For n As Integer = 0 To NumView - 1
                RenderIndexOffset(n) -= RenderIndexAddress
            Next

            RenderDataAddress = CInt(sf.Position)
            sf.Position = ((sf.Position + 3) \ 4) * 4
            For n As Integer = 0 To NumView - 1
                RenderDataOffset(n) = CInt(sf.Position)
                RenderDataOffsetInIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                RenderDataLengthInIndex(n) = New Int32(NumObjectToRender(n) - 1) {}
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    RenderDataOffsetInIndex(n)(m) = CInt(sf.Position)
                    RenderInfo(n)(m).WriteData(sf)
                    RenderDataLengthInIndex(n)(m) = CInt(sf.Position - RenderDataOffsetInIndex(n)(m))
                    sf.Position = ((sf.Position + 3) \ 4) * 4
                Next
                For m As Integer = 0 To NumObjectToRender(n) - 1
                    RenderDataOffsetInIndex(n)(m) -= RenderDataOffset(n)
                Next
                RenderDataLength(n) = CInt(sf.Position - RenderDataOffset(n))
            Next
            For n As Integer = 0 To NumView - 1
                RenderDataOffset(n) -= RenderDataAddress
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
                sf.Position = pViewInfo(n) + 20
                sf.WriteInt32(RenderIndexOffset(n))
                sf.WriteInt32(RenderIndexLength(n))
                sf.Position += 4
                sf.WriteInt32(RenderDataOffset(n))
                sf.WriteInt32(RenderDataLength(n))
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
        Public Unknown1 As Int32
        Public Unknown4 As Int32

        Public Const Length As Int32 = 52

        Public Sub New()
        End Sub
        Public Sub New(ByVal s As StreamEx, ByRef NumObjectToRender As Int32, ByVal VersionSign As Version)
            OffsetX = s.ReadSingle
            OffsetY = s.ReadSingle
            ViewLongitude = s.ReadSingle
            ViewLatitude = s.ReadSingle
            NumObjectToRender = s.ReadInt32
            s.Position += 8
            Unknown1 = s.ReadInt32
            s.Position += 8
            If s.ReadInt32 <> 0 Then Throw New InvalidDataException
            If s.ReadInt32 <> 0 Then Throw New InvalidDataException
            Select Case VersionSign
                Case Version.Comm2
                    Unknown4 = s.ReadInt32
                Case Version.Comm2Demo
                    Unknown4 = 0
                Case Else
                    Throw New InvalidOperationException
            End Select
        End Sub
        Public Sub WriteToFile(ByVal s As StreamEx, ByVal NumObjectToRender As Int32, ByVal VersionSign As Version)
            s.WriteSingle(OffsetX)
            s.WriteSingle(OffsetY)
            s.WriteSingle(ViewLongitude)
            s.WriteSingle(ViewLatitude)
            s.WriteInt32(NumObjectToRender)
            s.WriteInt32(0)
            s.WriteInt32(0)
            s.WriteInt32(Unknown1)
            s.WriteInt32(0)
            s.WriteInt32(0)
            s.WriteInt32(0)
            s.WriteInt32(0)
            Select Case VersionSign
                Case Version.Comm2
                    s.WriteInt32(Unknown4)
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
        Public Sub New()
        End Sub
        Public Sub New(ByVal BlockToViewIndexDict As Dictionary(Of RenderInfoBlock, Integer))
            Dict = BlockToViewIndexDict
        End Sub

        Public Overrides Function GetMappedObject(ByVal o As RenderInfoBlock) As RenderInfoBlockSerializationData
            Dim ViewIndex As Integer = Dict(o)
            Dim RenderMapPath = o.ObjectIndex & "_" & ViewIndex & ".A.png"

            Dim RenderMap As Array2(Of Byte) = o.RenderMap
            Dim Rectangle = New Int32(o.Width, o.Height) {}
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

        Public Function Decode() As Array2(Of Byte)
            Dim Src As New Queue(Of Byte)(RenderData)
            Dim Tar As Int32() = Nothing
            Dim Orginal = Array2(Of Byte).Create(Width, 8)

            Dim HeadByte As Byte = Src.Dequeue
            If (HeadByte And 1) = 0 Then
                Dim NumXBlock As Integer = (Width + 7) \ 8
                If NumXBlock <= 0 Then Return Orginal
                Dim Status As Byte() = New Byte(NumXBlock - 1) {}
                Dim k As Integer = 2
                Dim StatusByte As Byte = HeadByte
                For n As Integer = 0 To NumXBlock - 1
                    If k >= 8 Then
                        k = 0
                        StatusByte = Src.Dequeue
                    End If
                    Status(n) = CByte((StatusByte >> k) And 3)
                    k += 2
                Next
                For n As Integer = 0 To NumXBlock - 2
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
                Dim Num As Byte = 0
                For y As Integer = 0 To Height - 1
                    For n As Integer = 0 To Num - 1
                        Tar(n * 5 + 1) += Tar(n * 5 + 3)
                        Tar(n * 5 + 2) += Tar(n * 5 + 4)
                    Next

                    If CBool((HeadByte >> y) And 1) Then
                        Dim StatusByte As Byte = Src.Dequeue
                        If (StatusByte And 1) = 1 Then
                            Num = StatusByte >> 1
                            StatusByte = 255
                            Tar = New Int32(Num * 5 - 1) {}
                        End If

                        StatusByte >>= 1

                        For n As Integer = 0 To Num - 1
                            If CBool((StatusByte >> n) And 1) Then
                                Dim Data As Int32 = CInt(Src.Dequeue)
                                If CBool(Data And 1) Then
                                    Data = Data Or (CInt(Src.Dequeue) << 8)
                                    Data = Data Or (CInt(Src.Dequeue) << 16)
                                    Data = Data Or (CInt(Src.Dequeue) << 24)

                                    Tar(n * 5) = ((Data >> 3) And 1) Or (((Data >> 2) And 1) << 8)
                                    Tar(n * 5 + 4) = ((Data >> 4) And 15) - 7 '4~7
                                    Tar(n * 5 + 3) = ((Data >> 8) And 15) - 7 '8~11
                                    Tar(n * 5 + 2) = (Data >> 12) And &H3FF '12~21
                                    Tar(n * 5 + 1) = (Data >> 22) And &H3FF '22~31
                                Else
                                    Tar(n * 5) = ((Data >> 3) And 1) Or (((Data >> 2) And 1) << 8)
                                    Tar(n * 5 + 4) += ((Data >> 4) And 3) - 1
                                    Tar(n * 5 + 3) += ((Data >> 6) And 3) - 1
                                End If
                            End If
                        Next
                    End If
                    For n As Integer = 0 To Num - 1
                        Dim LowerInThis As Boolean = CBool(Tar(n * 5) And &HFF)
                        Dim UpperInThis As Boolean = CBool(Tar(n * 5) And &HFF00)
                        Dim Lower As Int32 = Tar(n * 5 + 1)
                        Dim Upper As Int32 = Tar(n * 5 + 2)
                        If Lower < 0 Then Lower = 0
                        If Upper > Width Then Upper = Width
                        If Lower >= Upper Then Continue For
                        If LowerInThis Then
                            Orginal(Lower, y) = 1
                            Lower += 1
                        End If
                        If UpperInThis Then
                            Orginal(Upper - 1, y) = 1
                            Upper -= 1
                        End If
                        For i As Integer = Lower To Upper - 1
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
