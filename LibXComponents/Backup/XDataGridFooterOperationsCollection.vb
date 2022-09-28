<Serializable()> _
    Public Class XDataGridFooterOperationsCollection
    Inherits CollectionBase

    Dim m_ownerObject As Object
    Dim m_objKeys As New ArrayList

    Public Event afterAddEelement(ByVal sender As Object, ByVal p_intIndex As Integer, ByVal p_objObject As Object)
    Public Event listChanged(ByVal sender As Object)

    Public Sub SetParent(ByVal p_objOwner As Object)
        m_ownerObject = p_objOwner
    End Sub


    Public Sub add(ByVal p_objObject As XDataGridFooterExpField)
        list.Add(p_objObject)
        m_objKeys.Add(p_objObject.ColumnName.ToLower)
    End Sub


    Public Sub add(ByVal p_strColumnName As String, ByVal p_strExpression As String, Optional ByVal p_strFilter As String = "")
        Dim objCol As New XDataGridFooterExpField
        objCol.ColumnName = p_strColumnName
        objCol.Expression = p_strExpression
        objCol.Filter = p_strFilter
        Me.add(objCol)
    End Sub


    '*----------------------------------------------------
    '* Nombre        : Remove
    '* Parametros    : index : elemento a borrar
    '* Descripción   : borrar
    '* Desarrollador : ccaraballo
    '* Modified      : 
    '*----------------------------------------------------
    Public Sub Remove(ByVal index As Integer)
        If Not (index > Count - 1 Or index < 0) Then
            List.RemoveAt(index)
            m_objKeys.RemoveAt(index)
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
    Public ReadOnly Property Item(ByVal index As Integer) As XDataGridFooterExpField
        Get
            Return CType(List.Item(index), XDataGridFooterExpField)
        End Get
    End Property

    Public ReadOnly Property Item(ByVal index As String) As XDataGridFooterExpField
        Get
            Return CType(List.Item(Me.m_objKeys.IndexOf(index.ToLower)), XDataGridFooterExpField)
        End Get
    End Property


    '*----------------------------------------------------
    '* Nombre        : onInsert
    '* Parametros    : index: indice del elemento a agregar 
    '*                  value: elemento
    '* Descripción   :  permite disparar un evento antes de agregar un elemento a la lsta 
    '* Desarrollador : ccaraballo
    '* Modified      : 
    '*----------------------------------------------------
    Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)

        MyBase.OnInsert(index, CType(value, XDataGridFooterExpField))

    End Sub

    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)
        MyBase.OnInsertComplete(index, value)
        RaiseEvent afterAddEelement(Me, index, value)
        RaiseEvent listChanged(Me)
    End Sub


    Public Overloads Sub AddRange(ByVal Items() As XDataGridFooterExpField)
        Dim Item As XDataGridFooterExpField
        Try
            m_objKeys = New ArrayList
            For Each Item In Items
                If Not Item Is Nothing Then
                    List.Add(Item)
                    m_objKeys.Add(Item.ColumnName.ToLower)
                End If
            Next
            RaiseEvent listChanged(Me)
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

End Class

