Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization

'<Serializable(), TypeConverter(GetType(LibXDbSourceTableCollectionConverter))> _
<TypeConverter(GetType(LibXDbSourceTableCollectionConverter))> _
Public Class LibXDbSourceTableCollection
    Inherits CollectionBase


    Public Sub New()

    End Sub

    Public Event ListChanged(ByVal sender As Object, ByVal e As EventArgs)

    Public Sub add(ByVal item As LibXDbSourceTable)
        list.Add(item)
    End Sub

    Public Sub Remove(ByVal index As Integer)
        If Not (index > Count - 1 Or index < 0) Then
            List.RemoveAt(index)
        End If
    End Sub

    '*---------------------------------------------------
    '* Nombre        : item
    '* Parametros    : index : extraer un elemento.
    '* Descripción   : borrar
    '*                 Aquí puedo también convetirlo antes de devolverlo, 
    '*                 de esta forma se puede poner que el elemento no retorne object, sino otro tipo
    '* Desarrollador : ccaraballo
    '* Modified      : 
    '*----------------------------------------------------
    Public ReadOnly Property Item(ByVal index As Integer) As LibXDbSourceTable
        Get
            Return CType(List.Item(index), LibXDbSourceTable)
        End Get
    End Property

    Public ReadOnly Property Item(ByVal index As String) As LibXDbSourceTable
        Get
            Dim i As Integer = 0
            For Each t As LibXDbSourceTable In list
                If t.TableName = index Then
                    Return CType(List.Item(i), LibXDbSourceTable)
                End If
                i = i + 1
            Next

        End Get
    End Property


    Public Overloads Sub AddRange(ByVal Items() As LibXDbSourceTable)
        Dim Item As LibXDbSourceTable
        Try
            For Each Item In Items
                If Not Item Is Nothing Then
                    List.Add(Item)
                End If
            Next
            'RaiseEvent ListChanged(Me, New EventArgs)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)
        MyBase.OnInsertComplete(index, value)
        RaiseEvent ListChanged(Me, New EventArgs)
    End Sub

    Protected Overrides Sub OnSetComplete(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
        MyBase.OnSetComplete(index, oldValue, newValue)
        RaiseEvent ListChanged(Me, New EventArgs)
    End Sub

    Protected Overrides Sub OnRemoveComplete(ByVal index As Integer, ByVal value As Object)
        MyBase.OnRemoveComplete(index, value)
        RaiseEvent ListChanged(Me, New EventArgs)
    End Sub


End Class
