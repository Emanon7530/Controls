
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Drawing.Design
Imports System.Windows.Forms.ComponentModel
Imports System.Windows.Forms.Design
Imports System.Collections

Public Class XDataGridFooterExpFieldConverter
    Inherits TypeConverter

    Public Overloads Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
        If (TypeOf destinationType Is InstanceDescriptor) Then
            Return True
        End If
        Return MyBase.CanConvertTo(context, destinationType)
    End Function


    Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
        If destinationType Is GetType(InstanceDescriptor) Then
            Dim objDataCol As XDataGridFooterExpField = CType(value, XDataGridFooterExpField)
            ' If we have defined different value to our
            ' instance that the default values then lets 
            ' us the overloaded constructor

            'If mestilo.mName.CompareTo( _
            '        mestilo.DefaultName) <> 0 Or _
            '        mestilo.mAge <> mestilo.DefaultAge Then
            '    Return New InstanceDescriptor( _
            '        GetType(Estilo).GetConstructor( _
            '            New Type() {GetType(String), _
            '            GetType(Int32)}), _
            '            New Object() {mestilo.mName, _
            '                mestilo.mAge})
            '    ' We will use the default one with the default values
            'Else

            'Return New InstanceDescriptor( _
            'GetType(XDataGridFooterExpFieldConverter).GetConstructor( _
            'New Type() {}), Nothing, False)


            Return New InstanceDescriptor( _
            GetType(XDataGridFooterExpField).GetConstructor( _
            New Type() {}), Nothing, False)
            'End If
        End If

        If (destinationType Is GetType(System.String) AndAlso TypeOf value Is XDataGridFooterExpField) Then
            Dim objDataCol As XDataGridFooterExpField = CType(value, XDataGridFooterExpField)
            If Trim(objdatacol.ColumnName) <> "" Then
                Return objdatacol.ColumnName
            End If
        End If

        Return MyBase.ConvertTo(context, culture, _
            value, destinationType)
    End Function

End Class
