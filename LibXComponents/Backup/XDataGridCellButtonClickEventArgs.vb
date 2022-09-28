Option Strict Off
Option Explicit On 

Imports Microsoft.VisualBasic
Imports System



Public Class XDataGridCellButtonClickEventArgs
    Inherits EventArgs
    Private _row As Integer
    Private _col As Integer
    Private m_blnExecuteLookup As Boolean = True


    Public Sub New(ByVal row As Integer, ByVal col As Integer)
        _row = row
        _col = col
    End Sub 'New


    Public Property executeLookup() As Boolean
        Get
            Return m_blnExecuteLookup
        End Get
        Set(ByVal Value As Boolean)
            m_blnExecuteLookup = Value
        End Set
    End Property

    Public ReadOnly Property RowIndex() As Integer
        Get
            Return _row
        End Get
    End Property

    Public ReadOnly Property ColIndex() As Integer
        Get
            Return _col
        End Get
    End Property
End Class 'DataGridCellButtonClickEventArgs
