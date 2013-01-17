'==========================================================================
'
'  File:        TypeConverters.vb
'  Location:    FileSystem <Visual Basic .Net>
'  Description: PropertyGrid用类型转换和描述类型
'  Version:     2013.01.18.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.ComponentModel

Public Class IEnumerableExpandableObjectConverter
    Inherits ExpandableObjectConverter

    Public Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, ByVal value As Object, ByVal attributes() As Attribute) As PropertyDescriptorCollection
        Dim l As New List(Of PropertyDescriptor)
        Dim v = DirectCast(value, IEnumerable)
        Dim IEnumerableType = value.GetType()
        Dim k = 0
        For Each o As Object In v
            Dim pd As New IEnumerablePropertyDescriptor(IEnumerableType, o, k)
            l.Add(pd)
            k += 1
        Next
        Return New PropertyDescriptorCollection(l.ToArray())
    End Function
End Class

Public Class IEnumerablePropertyDescriptor
    Inherits PropertyDescriptor

    Private IEnumerableType As Type
    Private o As Object
    Private Index As Integer
    Private c As TypeConverter

    Public Sub New(ByVal IEnumerableType As Type, ByVal o As Object, ByVal Index As Integer)
        MyBase.New("[" & Index.ToString() & "]", Nothing)
        Me.IEnumerableType = IEnumerableType
        Me.o = o
        Me.Index = Index
        Dim t = o.GetType()
        c = New RecordExpandableObjectConverter
    End Sub

    Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
        Return False
    End Function

    Public Overrides ReadOnly Property ComponentType As Type
        Get
            Return IEnumerableType
        End Get
    End Property

    Public Overrides Function GetValue(ByVal component As Object) As Object
        Return o
    End Function

    Public Overrides ReadOnly Property IsReadOnly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property PropertyType As Type
        Get
            Return o.GetType()
        End Get
    End Property

    Public Overrides Sub ResetValue(ByVal component As Object)
    End Sub

    Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
    End Sub

    Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
        Return False
    End Function

    Public Overrides ReadOnly Property Converter As TypeConverter
        Get
            Return c
        End Get
    End Property
End Class

Public Class RecordExpandableObjectConverter
    Inherits ExpandableObjectConverter

    Public Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, ByVal value As Object, ByVal attributes() As Attribute) As PropertyDescriptorCollection
        Dim l As New List(Of PropertyDescriptor)
        Dim Type = value.GetType()
        For Each m In Type.GetMembers()
            If m.MemberType <> MemberTypes.Field AndAlso m.MemberType <> MemberTypes.Property Then Continue For
            If m.MemberType = MemberTypes.Property Then
                Dim p = DirectCast(m, PropertyInfo)
                If p.GetIndexParameters().Count > 0 Then Continue For
            End If
            If m.MemberType = MemberTypes.Field OrElse m.MemberType = MemberTypes.Property Then
                Dim aa = m.GetCustomAttributes(GetType(ComponentModel.TypeConverterAttribute), True)
                If aa.Length > 0 Then
                    Dim a = DirectCast(aa.First(), ComponentModel.TypeConverterAttribute)
                    Dim ct = System.Type.GetType(a.ConverterTypeName)
                    Dim c = DirectCast(Activator.CreateInstance(ct), TypeConverter)
                    If m.MemberType = MemberTypes.Field Then
                        Dim pd As New SimpleFieldDescriptor(Type, DirectCast(m, FieldInfo), c)
                        l.Add(pd)
                    ElseIf m.MemberType = MemberTypes.Property Then
                        Dim pd As New SimplePropertyDescriptor(Type, DirectCast(m, PropertyInfo), c)
                        l.Add(pd)
                    Else
                        Throw New InvalidOperationException
                    End If
                    Continue For
                End If
                Dim t As Type
                If m.MemberType = MemberTypes.Field Then
                    t = DirectCast(m, FieldInfo).FieldType
                ElseIf m.MemberType = MemberTypes.Property Then
                    t = DirectCast(m, PropertyInfo).PropertyType
                Else
                    Throw New InvalidOperationException
                End If
                If t = GetType(IEnumerable) OrElse t.GetInterfaces().Contains(GetType(IEnumerable)) Then
                    Dim et As Type = Nothing
                    If t.IsArray Then
                        et = t.GetElementType()
                    ElseIf t.IsGenericType() AndAlso t.GetGenericTypeDefinition() = GetType(IEnumerable(Of )) Then
                        et = t.GetGenericArguments()(0)
                    Else
                        Dim Interfaces = t.GetInterfaces().Where(Function(i) i.IsGenericType() AndAlso i.GetGenericTypeDefinition() = GetType(IEnumerable(Of ))).ToArray()
                        If Interfaces.Count > 0 Then
                            et = Interfaces.First.GetGenericArguments(0)
                        End If
                    End If
                    If et Is Nothing OrElse et.IsClass Then
                        Dim c = New IEnumerableExpandableObjectConverter
                        If m.MemberType = MemberTypes.Field Then
                            Dim pd As New SimpleFieldDescriptor(Type, DirectCast(m, FieldInfo), c)
                            l.Add(pd)
                        ElseIf m.MemberType = MemberTypes.Property Then
                            Dim pd As New SimplePropertyDescriptor(Type, DirectCast(m, PropertyInfo), c)
                            l.Add(pd)
                        Else
                            Throw New InvalidOperationException
                        End If
                        Continue For
                    End If
                End If
            End If
            If m.MemberType = MemberTypes.Field Then
                Dim pd As New SimpleFieldDescriptor(Type, DirectCast(m, FieldInfo))
                l.Add(pd)
            ElseIf m.MemberType = MemberTypes.Property Then
                Dim pd As New SimplePropertyDescriptor(Type, DirectCast(m, PropertyInfo))
                l.Add(pd)
            End If
        Next
        Return New PropertyDescriptorCollection(l.ToArray())
    End Function

    Private Class SimpleFieldDescriptor
        Inherits PropertyDescriptor

        Private ObjectType As Type
        Private FieldInfo As FieldInfo
        Private c As TypeConverter

        Public Sub New(ByVal ObjectType As Type, ByVal FieldInfo As FieldInfo, Optional ByVal Converter As TypeConverter = Nothing)
            MyBase.New(FieldInfo.Name, Nothing)
            Me.ObjectType = ObjectType
            Me.FieldInfo = FieldInfo
            If Converter IsNot Nothing Then
                c = Converter
            Else
                c = MyBase.Converter
            End If
        End Sub

        Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property ComponentType As Type
            Get
                Return ObjectType
            End Get
        End Property

        Public Overrides Function GetValue(ByVal component As Object) As Object
            Return FieldInfo.GetValue(component)
        End Function

        Public Overrides ReadOnly Property IsReadOnly As Boolean
            Get
                Return FieldInfo.IsInitOnly
            End Get
        End Property

        Public Overrides ReadOnly Property PropertyType As Type
            Get
                Return FieldInfo.FieldType
            End Get
        End Property

        Public Overrides Sub ResetValue(ByVal component As Object)
        End Sub

        Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
            FieldInfo.SetValue(component, value)
        End Sub

        Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property Converter As TypeConverter
            Get
                Return c
            End Get
        End Property
    End Class

    Private Shadows Class SimplePropertyDescriptor
        Inherits PropertyDescriptor

        Private ObjectType As Type
        Private PropertyInfo As PropertyInfo
        Private c As TypeConverter

        Public Sub New(ByVal ObjectType As Type, ByVal PropertyInfo As PropertyInfo, Optional ByVal Converter As TypeConverter = Nothing)
            MyBase.New(PropertyInfo.Name, Nothing)
            Me.ObjectType = ObjectType
            Me.PropertyInfo = PropertyInfo
            If Converter IsNot Nothing Then
                c = Converter
            Else
                c = MyBase.Converter
            End If
        End Sub

        Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property ComponentType As Type
            Get
                Return ObjectType
            End Get
        End Property

        Public Overrides Function GetValue(ByVal component As Object) As Object
            Return PropertyInfo.GetValue(component, Nothing)
        End Function

        Public Overrides ReadOnly Property IsReadOnly As Boolean
            Get
                Return Not PropertyInfo.CanWrite
            End Get
        End Property

        Public Overrides ReadOnly Property PropertyType As Type
            Get
                Return PropertyInfo.PropertyType
            End Get
        End Property

        Public Overrides Sub ResetValue(ByVal component As Object)
        End Sub

        Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
            PropertyInfo.SetValue(component, value, Nothing)
        End Sub

        Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property Converter As TypeConverter
            Get
                Return c
            End Get
        End Property
    End Class
End Class
