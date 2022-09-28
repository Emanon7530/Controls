Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Drawing.Design
Imports System.Windows.Forms.ComponentModel
Imports System.Windows.Forms.Design
Imports System.Collections

Public Class LibXDbSourceTableCollectionConverter
    Inherits TypeConverter

    Public Overloads Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
        If (TypeOf destinationType Is InstanceDescriptor) Then
            Return True
        End If
        Return MyBase.CanConvertTo(context, destinationType)
    End Function


    Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
        If destinationType Is GetType(InstanceDescriptor) Then

            Return New InstanceDescriptor( _
            GetType(LibXDbSourceTableCollection).GetConstructor( _
            New Type() {}), Nothing, True)
        End If

        If (destinationType Is GetType(System.String) AndAlso TypeOf value Is LibXDbSourceTableCollection) Then
            Dim objCollection As LibXDbSourceTableCollection = CType(value, LibXDbSourceTableCollection)

            Return objCollection.ToString
        Else
            If destinationType Is GetType(System.String) Then
                Return ""
            End If
        End If

        Return MyBase.ConvertTo(context, culture, _
            value, destinationType)
    End Function


    Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
        If (sourceType Is GetType(String)) Then
            Return True
        End If
        Return MyBase.CanConvertFrom(context, sourceType)
    End Function

    Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object

        Try
            'If TypeOf value Is String Then
            '    Dim strValue As String = value
            '    'Dim objContainer As NetCmdBuilderParameter = CType(context.Instance, NetCmdBuilderParameter)
            '    Dim objcollection As LibXDbSourceTableCollection = context.PropertyDescriptor.GetValue(context.Instance)
            '    '*If strValue.Length > 0 Then
            '    objcollection.fromString(strValue)
            '    '*End If
            '    Return objcollection
            'End If

            Return MyBase.ConvertFrom(context, culture, value)
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

End Class
