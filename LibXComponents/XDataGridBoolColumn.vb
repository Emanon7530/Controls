Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms



Public Class XDataGridBoolColumn
    Inherits DataGridBoolColumn

    Public Delegate Sub BoolValueChangedEventHandler(ByVal sender As Object, ByVal e As BoolValueChangedEventArgs)

    Private saveValue As Boolean
    Private saveRow As Integer
    Private lockValue As Boolean
    Private beingEdited As Boolean
    Private _column As Integer
    Public Const VK_SPACE As Integer = 32

    <System.Runtime.InteropServices.DllImport("user32.dll")> _
    Public Shared Function GetKeyState(ByVal nVirtKey As Integer) As Short
    End Function

    Public Event SetCellFormat As XDataGridFormatCellEventHandler
    Private m_blnUseCustomCellFormat As Boolean

    Public Property UseCustomCellFormat() As Boolean
        Get
            Return m_blnUseCustomCellFormat
        End Get
        Set(ByVal Value As Boolean)
            m_blnUseCustomCellFormat = Value
        End Set
    End Property

    Public Sub New()

        saveValue = False
        saveRow = -(1)
        lockValue = False
        beingEdited = False
        _column = -1
        FalseValue = 0
        TrueValue = 1
        NullValue = 0

    End Sub


    Public Sub New(ByVal column As Integer)
        MyBase.New()
        saveValue = False
        saveRow = -(1)
        lockValue = False
        beingEdited = False
        _column = column

        FalseValue = 0
        TrueValue = 1
        NullValue = 0

    End Sub

    Public Event BoolValueChanged As BoolValueChangedEventHandler

    Protected Overloads Overrides Sub Paint(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal source As CurrencyManager, ByVal rowNum As Integer, ByVal backBrush As Brush, ByVal foreBrush As Brush, ByVal alignToRight As Boolean)

        Dim changing As Boolean
        Dim parent As DataGrid = DataGridTableStyle.DataGrid
        Dim ForeColor As Color
        Dim BackColor As Color
        Dim objFont As Font


        If _column < 0 Then
            _column = DataGridTableStyle.GridColumnStyles.IndexOf(Me)
        End If

        Dim mousePos As Point = Me.DataGridTableStyle.DataGrid.PointToClient(Control.MousePosition)
        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid
        Dim isClickInCell As Boolean = ((Control.MouseButtons = MouseButtons.Left) And _
         dg.GetCellBounds(dg.CurrentCell).Contains(mousePos))

        changing = dg.Focused And (isClickInCell _
        Or GetKeyState(VK_SPACE) < 0)   ' or spacebar


        If ((Not lockValue) And beingEdited And changing And saveRow = rowNum) Then
            Me.Commit(source, rowNum)
            saveValue = Not (saveValue)
            lockValue = False
            Dim e As New BoolValueChangedEventArgs(rowNum, _column, saveValue)
            If Not source.Current Is Nothing Then
                e.dataRow = CType(source.Current, DataRowView)
                e.source = source
            End If
            onBoolValueChanged(e)
        Else
            '--> Es un despliegue
            If Not source.Current Is Nothing And Not Trim(Me.MappingName) = "" Then
                If TypeOf source.Current Is DataRowView Then
                    Dim objRow As DataRowView = source.List(rowNum)
                    If IsDBNull(objRow(Me.MappingName)) Then
                        objRow(Me.MappingName) = 0
                        If objRow.Row.RowState = DataRowState.Unchanged Then
                            objRow.Row.AcceptChanges()
                        End If
                    End If
                End If
            End If
        End If

        If (saveRow = rowNum) Then
            lockValue = False
        End If

        ForeColor = parent.ForeColor
        BackColor = parent.BackColor

        '-->backBrush = New SolidBrush(BackColor)
        '-->foreBrush = New SolidBrush(ForeColor)

        Dim objE As XDataGridFormatCellEventArgs = Nothing

        If m_blnUseCustomCellFormat Then
            objE = New XDataGridFormatCellEventArgs(rowNum, _column, Me.GetColumnValueAtRow([source], rowNum))
            RaiseEvent SetCellFormat(Me, objE)

            If objE.UseBaseClassDrawing Then
                m_blnUseCustomCellFormat = False
            End If

            If m_blnUseCustomCellFormat Then
                backBrush = New SolidBrush(objE.BackColor)
                foreBrush = New SolidBrush(objE.ForeColor)

                If Not objE.TextFont Is Nothing Then
                    objFont = objE.TextFont
                End If

            End If

        End If '-->If m_blnUseCustomCellFormat Then

        MyBase.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight)

        If Not objFont Is Nothing Then
            'objFont.Dispose()
        End If

    End Sub
    Protected Overloads Overrides Sub Edit(ByVal source As CurrencyManager, ByVal rowNum As Integer, ByVal bounds As Rectangle, ByVal [readOnly] As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)

        lockValue = True
        beingEdited = True
        saveRow = rowNum

        If Not TypeOf MyBase.GetColumnValueAtRow(source, rowNum) Is System.DBNull Then
            saveValue = CType(CType(source.Current, DataRowView)(Me.MappingName), Boolean)
        End If
        MyBase.Edit(source, rowNum, bounds, [readOnly], instantText, cellIsVisible)

    End Sub

    Protected Overridable Sub onBoolValueChanged(ByVal e As BoolValueChangedEventArgs)

        RaiseEvent BoolValueChanged(Me, e)


    End Sub

    Protected Overloads Overrides Function Commit(ByVal dataSource As CurrencyManager, ByVal rowNum As Integer) As Boolean

        lockValue = True
        beingEdited = False
        Return MyBase.Commit(dataSource, rowNum)

    End Function

    Private Sub XDataGridBoolColumn_TrueValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.TrueValueChanged
        sender.TrueValue = CType(sender.TrueValue, Int16)
    End Sub

    Private Sub XDataGridBoolColumn_FalseValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.FalseValueChanged
        sender.FalseValue = CType(sender.FalseValue, Int16)
    End Sub

End Class

Public Class BoolValueChangedEventArgs
    Inherits EventArgs
    Private _column As Integer
    Private _row As Integer
    Private _value As Boolean

    Public dataRow As DataRowView
    Public source As BindingManagerBase


    Public Sub New(ByVal row As Integer, ByVal col As Integer, ByVal val As Boolean)
        MyBase.New()
        _row = row
        _column = col
        _value = val

    End Sub

    Public Property Column() As Integer
        Get

            Return _column

        End Get
        Set(ByVal Value As Integer)

            _column = Value

        End Set
    End Property

    Public Property Row() As Integer
        Get

            Return _row

        End Get
        Set(ByVal Value As Integer)

            _row = Value

        End Set
    End Property
    Public Property BoolValue() As Boolean
        Get

            Return _value

        End Get
        Set(ByVal Value As Boolean)

            _value = Value

        End Set
    End Property
End Class
