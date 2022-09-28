Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design


Public Class XDataGridTableStylesCollectionEditor
    Inherits CollectionEditor

    Sub New(ByVal p_type As Type)
        MyBase.new(p_type)
    End Sub

    Protected Overrides Function CreateNewItemTypes() As System.Type()
        Return New Type() {GetType(XDataGridTableStyle)}
    End Function

End Class


