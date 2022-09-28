Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports System.Windows.Forms.Design

Public Class XdataGridTableStyle
    Inherits DataGridTableStyle

    <Editor(GetType(MyGridColumnStylesCollectionEditor), GetType(UITypeEditor))> _
    Public Shadows ReadOnly Property GridColumnStyles() As GridColumnStylesCollection
        Get
            Return MyBase.GridColumnStyles
        End Get
    End Property

    Protected Shadows Function CreateGridColumn(ByVal prop As PropertyDescriptor, ByVal isDefault As Boolean) As DataGridColumnStyle
        Return MyBase.CreateGridColumn(prop, isDefault)
    End Function

    Private Class MyGridColumnStylesCollectionEditor
        Inherits CollectionEditor

        Sub New(ByVal p_Type As Type)
            MyBase.New(p_Type)
        End Sub

        Protected Overrides Function CreateNewItemTypes() As System.Type()
            Return New Type() { _
                   GetType(DataGridTextBoxColumn), _
                   GetType(DataGridBoolColumn), _
                   GetType(XEditTextBoxColumn), _
                   GetType(XDataGridTextButtonColumn), _
                   GetType(XDataGridComboBoxColumn), _
                   GetType(XDataGridButtonColumn), _
                   GetType(XDataGridBoolColumn), _
                   GetType(XDataGridNotFocusColumn), _
                   GetType(XDataGridLinkColumn) _
                  }


        End Function
    End Class
End Class
