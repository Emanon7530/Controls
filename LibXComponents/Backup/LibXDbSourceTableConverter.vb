
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Drawing.Design
Imports System.Windows.Forms.ComponentModel
Imports System.Windows.Forms.Design
Imports System.Collections

Public Class LibXDbSourceTableConverter
    Inherits TypeConverter

    Public Overloads Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
        If (TypeOf destinationType Is InstanceDescriptor) Then
            Return True
        End If
        Return MyBase.CanConvertTo(context, destinationType)
    End Function


    Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
        If destinationType Is GetType(InstanceDescriptor) Then
            Dim objDataCol As LibXDbSourceTable = CType(value, LibXDbSourceTable)

            'Dim cr As ConstructorInfo = GetType(LibXDbSourceTable).GetConstructor(New Type() {GetType(String)})

            'If Not cr Is Nothing Then
            '    Return New InstanceDescriptor( _
            '    cr, New Object() {""}, True)
            'End If

            Dim cr As ConstructorInfo = GetType(LibXDbSourceTable).GetConstructor(New Type() {})

            If Not cr Is Nothing Then
                Return New InstanceDescriptor( _
                    cr, Nothing, True)
            End If
        End If

        Return MyBase.ConvertTo(context, culture, _
            value, destinationType)
    End Function

End Class
