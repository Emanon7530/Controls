Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Reflection
Imports System.Drawing.Design
Imports System.Windows.Forms.ComponentModel
Imports System.Windows.Forms.Design
Imports System.Collections



<Serializable(), _
TypeConverter(GetType(XDataGridFooterExpFieldConverter))> _
Public Class XDataGridFooterExpField
    Private m_strColumnName As String
    Private m_strExpression As String
    Private m_strFilter As String

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property ColumnName() As String
        Get
            Return m_strColumnName
        End Get
        Set(ByVal Value As String)
            m_strColumnName = Value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property Expression() As String
        Get
            Return m_strExpression
        End Get
        Set(ByVal Value As String)
            m_strExpression = Value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property Filter() As String
        Get
            Return m_strFilter
        End Get
        Set(ByVal Value As String)
            m_strFilter = Value
        End Set
    End Property
End Class

