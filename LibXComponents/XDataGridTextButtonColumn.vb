Option Strict Off
Option Explicit On 

Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports System.ComponentModel

Public Class XDataGridTextButtonColumn
    Inherits DataGridTextBoxColumn
    Public Event CellButtonClicked As XDataGridCellButtonClickEventHandler

    Private m_objLookup As LibXLookup
    Private m_blnHasLookup As Boolean
    Private InEdit As Boolean
    Private m_blnBegingEdit As Boolean

    Public Enum XDataGridTextButtonColumnButtonTypes
        SpinButton = 1
        RecButton = 2
        Lookup = 3
    End Enum


    Private m_udtButtonType = XDataGridTextButtonColumnButtonTypes.SpinButton
    Private _buttonFace As Bitmap
    Private _buttonFacePressed As Bitmap
    Private m_intColumn As Integer
    Private m_intPressedRow As Integer
    Private m_blnHandleCreated As Boolean
    Private m_blnExecuteFindOnFill As Boolean = True
    Private m_TabStop As Boolean = True

    Public Event SetCellFormat As XDataGridFormatCellEventHandler
    Private m_blnUseCustomCellFormat As Boolean
    Private m_blnIsReadOnly As Boolean

    Public Property TabStop() As Boolean
        Get
            Return m_TabStop
        End Get
        Set(ByVal Value As Boolean)
            m_TabStop = Value
        End Set
    End Property

    Public Property CharacterCasing() As CharacterCasing
        Get
            Return Me.TextBox.CharacterCasing
        End Get
        Set(ByVal Value As CharacterCasing)
            Me.TextBox.CharacterCasing = Value
        End Set
    End Property

    Public Property MaxLength() As Integer
        Get
            Return Me.TextBox.MaxLength
        End Get
        Set(ByVal Value As Integer)
            Me.TextBox.MaxLength = Value
        End Set
    End Property

    Public Property UseCustomCellFormat() As Boolean
        Get
            Return m_blnUseCustomCellFormat
        End Get
        Set(ByVal Value As Boolean)
            m_blnUseCustomCellFormat = Value
        End Set
    End Property
    Public Property isReadOnly() As Boolean
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
        End Set
    End Property

    Public ReadOnly Property Column() As Integer
        Get

            Return Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me)

        End Get
    End Property


    Public Event setLookup(ByVal sender As Object, ByVal e As NetSetLookupEventArgs)



    Public Sub New()
        m_intColumn = -1
        m_intPressedRow = -1

        'Try
        '    Dim strm As System.IO.Stream = Me.GetType().Assembly.GetManifestResourceStream("buttonface.bmp")
        '    _buttonFace = New Bitmap(strm)
        '    strm = Me.GetType().Assembly.GetManifestResourceStream("buttonfacepressed.bmp")
        '    _buttonFacePressed = New Bitmap(strm)
        'Catch
        'End Try
    End Sub

    Public Sub New(ByVal colNum As Integer)
        m_intColumn = colNum
        m_intPressedRow = -1

        'Try
        '    Dim strm As System.IO.Stream = Me.GetType().Assembly.GetManifestResourceStream("buttonface.bmp")
        '    _buttonFace = New Bitmap(strm)
        '    strm = Me.GetType().Assembly.GetManifestResourceStream("buttonfacepressed.bmp")
        '    _buttonFacePressed = New Bitmap(strm)
        'Catch
        'End Try
    End Sub 'New

    Private Sub getbutton()
        Dim strBmp As String
        Dim strBmpP As String
        Try
            strBmp = "LibX.buttonface.bmp"
            strBmpP = "LibX.buttonfacepressed.bmp"
            If m_udtButtonType = XDataGridTextButtonColumnButtonTypes.RecButton Or _
               m_udtButtonType = XDataGridTextButtonColumnButtonTypes.Lookup Then
                strBmp = "LibX.buttonface2.bmp"
                strBmpP = "LibX.buttonfacepressed2.bmp"
            End If
            Dim strm As System.IO.Stream = Me.GetType().Assembly.GetManifestResourceStream(strBmp)
            _buttonFace = New Bitmap(strm)
            strm = Me.GetType().Assembly.GetManifestResourceStream(strBmpP)
            _buttonFacePressed = New Bitmap(strm)
        Catch
        End Try

        Dim objText As TextBox
        objText = Me.TextBox
        AddHandler objText.KeyPress, AddressOf onTextKeyPress
        AddHandler objText.Leave, AddressOf onTextBoxLeave
        AddHandler objText.Enter, AddressOf onTextBoxEnter

    End Sub

    Private Sub onTextKeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
        If parent.ReadOnly Or Not parent.Enabled Then
            Return
        End If

        m_blnBegingEdit = True
    End Sub

    Private Sub onTextBoxLeave(ByVal sender As Object, ByVal e As System.EventArgs)
        'm_blnLeaving = True
    End Sub

    Private Sub onTextBoxEnter(ByVal sender As Object, ByVal e As System.EventArgs)
        'm_blnLeaving = False
    End Sub

    'DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    '<TypeConverter(GetType(NetLookupConverter))> _
    Public Property Lookup() As LibXLookup
        Get
            If m_objLookup Is Nothing Then
                m_blnHasLookup = False
                m_objLookup = New LibXLookup
                m_objLookup.m_blnIsGrid = True
                m_objLookup.Visible = False
                If Not Me.DesignMode Then
                    Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
                    m_objLookup.SetContainerGrid(parent)
                End If
            End If
            Return m_objLookup
        End Get
        Set(ByVal Value As LibXLookup)
            Try
                m_objLookup = Value

                If Not m_objLookup Is Nothing Then
                    m_blnHasLookup = True
                    m_objLookup.Visible = False
                    If Not Me.DesignMode Then
                        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
                        m_objLookup.SetContainerGrid(parent)
                    End If
                End If

            Catch ex As Exception
                Log.Show(ex)
            End Try
        End Set
    End Property

    Public ReadOnly Property hasLookup() As Boolean
        Get
            Return m_blnHasLookup
        End Get
    End Property

    Public Property executeFindDuringFill() As Boolean
        Get
            Return m_blnExecuteFindOnFill
        End Get
        Set(ByVal Value As Boolean)
            m_blnExecuteFindOnFill = Value
        End Set
    End Property

    Private Sub onCellButtonClicked(ByVal e As XDataGridCellButtonClickEventArgs)
        RaiseEvent CellButtonClicked(Me, e)

        If e.executeLookup And Me.isReadOnly = False Then
            Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
            If parent.ReadOnly Or Not parent.Enabled Then
                Return
            End If
            If Not Me.m_objLookup Is Nothing And m_blnHasLookup Then
                m_objLookup.ExecuteLookupGrid()
                Invalidate()
            End If
        End If
    End Sub

    Protected Overloads Overrides Sub Edit(ByVal [source] As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal bounds As System.Drawing.Rectangle, ByVal [readOnly] As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)
        Try

            If _buttonFace Is Nothing Then
                init()
            End If

            Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
            If Not (parent.ReadOnly Or Not parent.Enabled) Then
                bounds.Width = bounds.Width - Me._buttonFace.Width
            End If

            If m_blnIsReadOnly Then
                Exit Sub
            End If

            MyBase.Edit(source, rowNum, bounds, [readOnly], instantText, cellIsVisible)

            InEdit = True

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Property ButtonType() As XDataGridTextButtonColumnButtonTypes
        Get
            Return m_udtButtonType
        End Get
        Set(ByVal value As XDataGridTextButtonColumnButtonTypes)
            m_udtButtonType = value
        End Set
    End Property

    Public Sub HandleMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid

        If dg.ReadOnly Or Not dg.Enabled Then
            Exit Sub
        End If

        Dim hti As DataGrid.HitTestInfo = dg.HitTest(New Point(e.X, e.Y))
        Dim isClickInCell As Boolean = hti.Column = Me.m_intColumn


        m_intPressedRow = -1

        Dim rect As New Rectangle(0, 0, 0, 0)

        If isClickInCell Then
            If hti.Row >= 0 And hti.Column >= 0 Then
                rect = dg.GetCellBounds(hti.Row, hti.Column)
            End If
            isClickInCell = e.X > rect.Right - Me._buttonFace.Width
        End If
        If isClickInCell Then
            Dim g As Graphics = Graphics.FromHwnd(dg.Handle)
            g.DrawImage(Me._buttonFace, rect.Right - Me._buttonFace.Width, rect.Y)
            g.Dispose()
            'If Not (CellButtonClicked Is Nothing) Then
            'RaiseEvent CellButtonClicked(Me, New XDataGridCellButtonClickEventArgs(hti.Row, hti.Column))
            onCellButtonClicked(New XDataGridCellButtonClickEventArgs(hti.Row, hti.Column))
            'End If
        End If
    End Sub 'HandleMouseUp

    Public Sub HandleMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid

        If dg.ReadOnly Or Not dg.Enabled Then
            Exit Sub
        End If
        Dim hti As DataGrid.HitTestInfo = dg.HitTest(New Point(e.X, e.Y))
        Dim isClickInCell As Boolean = hti.Column = Me.m_intColumn
        Dim rect As New Rectangle(0, 0, 0, 0)
        If isClickInCell Then
            If hti.Row >= 0 And hti.Column >= 0 Then
                rect = dg.GetCellBounds(hti.Row, hti.Column)
            End If
            isClickInCell = e.X > rect.Right - Me._buttonFace.Width
        End If

        If isClickInCell Then
            'Console.WriteLine("HandleMouseDown " + hti.Row.ToString());
            Dim g As Graphics = Graphics.FromHwnd(dg.Handle)
            g.DrawImage(Me._buttonFacePressed, rect.Right - Me._buttonFacePressed.Width, rect.Y)
            g.Dispose()
            m_intPressedRow = hti.Row
        End If
    End Sub 'HandleMouseDown

    Public Sub init()
        Dim objArgs As New NetSetLookupEventArgs
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid

        Try

            RaiseEvent setLookup(Me, objArgs)
            If Not objArgs.Lookup Is Nothing Then
                Lookup = objArgs.Lookup
            End If

            m_blnHandleCreated = True
            AddHandler parent.MouseDown, AddressOf HandleMouseDown
            AddHandler parent.MouseUp, AddressOf HandleMouseUp

            'Dim oGrid As LibXGrid = Me.DataGridTableStyle.DataGrid
            'RemoveHandler oGrid.cellValidate, AddressOf OnCellValidate
            'AddHandler oGrid.cellValidate, AddressOf OnCellValidate


            getbutton()

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    'Private Sub OnCellValidate(ByVal sender As Object, ByVal e As LibXGrid.LibXGridCellValidateEventArgs)
    '    Try
    '        If e.cell <> Me.m_intColumn Then
    '            Exit Sub
    '        End If
    '        If Not Me.hasLookup Then
    '            Me.doFind(e.row, e.cell, e.value)
    '            If Not m_objLookup.DataFound Then
    '                e.hasErrors = True
    '            End If
    '        End If
    '    Catch ex As Exception
    '        Log.Add(ex)
    '    End Try
    'End Sub

    Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal [source] As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal backBrush As System.Drawing.Brush, ByVal foreBrush As System.Drawing.Brush, ByVal alignToRight As Boolean)
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
        Dim objFont As Font
        Try
            If Not m_blnHandleCreated Then
                init()
            End If

            If m_intColumn < 0 Then
                m_intColumn = DataGridTableStyle.GridColumnStyles.IndexOf(Me)
            End If

            Dim current As Boolean = parent.IsSelected(rowNum) Or (parent.CurrentRowIndex = rowNum And parent.CurrentCell.ColumnNumber = Me.m_intColumn)

            Dim BackColor As Color
            'If current Then BackColor = parent.SelectionBackColor Else BackColor = parent.BackColor
            Dim ForeColor As Color

            objFont = parent.Font
            ForeColor = parent.ForeColor
            BackColor = parent.BackColor


            backBrush = New SolidBrush(BackColor)
            foreBrush = New SolidBrush(ForeColor)

            '-->
            Dim e As XDataGridFormatCellEventArgs = Nothing

            If m_blnUseCustomCellFormat Then
                e = New XDataGridFormatCellEventArgs(rowNum, m_intColumn, Me.GetColumnValueAtRow([source], rowNum))
                RaiseEvent SetCellFormat(Me, e)

                If Not e.UseBaseClassDrawing Then
                    m_blnUseCustomCellFormat = False
                Else
                    backBrush = New SolidBrush(e.BackColor)
                    foreBrush = New SolidBrush(e.ForeColor)

                    If Not e.TextFont Is Nothing Then
                        objFont = e.TextFont
                    End If

                End If

            End If '-->If m_blnUseCustomCellFormat Then
            '-->

            'Limpiar la cellda
            g.FillRectangle(backBrush, bounds)

            Dim s As String = Me.GetColumnValueAtRow([source], rowNum).ToString() 'parent[rowNum, 0].ToString() + ((parent[rowNum, 1].ToString())+ "  ").Substring(0,2);
            g.DrawString(s, objFont, foreBrush, bounds.X, bounds.Y)

            If parent.ReadOnly Or Not parent.Enabled Then
                Exit Sub
            End If

            Dim bm As Bitmap
            If m_intPressedRow = rowNum Then bm = Me._buttonFacePressed Else bm = Me._buttonFace

            If current Then
                g.DrawImage(bm, bounds.Right - bm.Width, bounds.Y)
            End If

        Catch ex As Exception
            Log.Show(ex)
        Finally
            If Not backBrush Is Nothing Then
                'backBrush.Dispose()
                'foreBrush.Dispose()
                'objFont.Dispose()
            End If
        End Try
    End Sub

    Public Class NetSetLookupEventArgs
        Public Lookup As LibXLookup
    End Class


    'Protected Overrides Function Commit(ByVal dataSource As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer) As Boolean
    '    Dim intOk As Integer
    '    return = MyBase.Commit(dataSource, rowNum)
    '    Return intOk

    '    'If Not InEdit Then
    '    '    Return True
    '    'End If

    '    'Try
    '    '    'Dim Value As Object = combo.SelectedValue
    '    '    'If NullText.Equals(Value) Then
    '    '    'Value = Convert.DBNull
    '    '    'End If
    '    '    'If Value Is Nothing Then
    '    '    'Value = Convert.DBNull
    '    '    'End If
    '    '    If Not m_objLookup Is Nothing Then
    '    '        ' m_objLookup.doFindGrid(Nothing, m_intColumn)
    '    '        'doxx()
    '    '        'Return False
    '    '    End If
    '    '    'SetColumnValueAtRow(dataSource, rowNum, Value)
    '    '    Return MyBase.Commit(dataSource, rowNum)
    '    'Catch e As Exception
    '    '    Return False
    '    'End Try
    '    'InEdit = False
    '    'endedit()
    '    'Return True
    'End Function

    Public Function doFind(ByRef p_objDataRow As DataRow, ByVal col As Integer, ByVal p_strValue As String) As Boolean
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
        Try
            If Not m_objLookup Is Nothing And m_blnExecuteFindOnFill And m_blnHasLookup Then
                m_objLookup.ExecuteFindGridOnRetrieve(p_objDataRow, Nothing, col, p_strValue)
            End If
            Return True
        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Function

    Public Function doFind(ByVal row As Integer, ByVal col As Integer, ByVal p_strValue As String) As Boolean
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid
        Try

            If Not m_blnBegingEdit Then
                Return True
            End If

            If Not m_objLookup Is Nothing And m_blnHasLookup Then
                m_objLookup.ExecuteFindGrid(, col, p_strValue)

            End If

            m_blnBegingEdit = False

            If Not m_objLookup.DataFound Then
                Return False
            End If

            Return True
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function getColumnNumber() As Integer
        If m_intColumn < 0 Then
            m_intColumn = DataGridTableStyle.GridColumnStyles.IndexOf(Me)
        End If
        Return m_intColumn
    End Function


    Public Function GetValue() As String
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid
            Dim oValue As Object = Grid(Grid.CurrentRowIndex, Me.getColumnNumber)

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub SetValue(ByVal value As Object)
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid

            SetValue(Grid.CurrentRowIndex, value)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Function GetValue(ByVal row As Integer) As String
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid
            Dim oValue As Object = Grid(Grid.CurrentRowIndex, Me.getColumnNumber)

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub SetValue(ByVal row As Integer, ByVal value As Object)
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid

            If Not value Is DBNull.Value Then
                Grid(row, Me.getColumnNumber) = value
            Else
                Grid(row, Me.getColumnNumber) = DBNull.Value
            End If

            Dim cm As CurrencyManager
            cm = CType(Grid, LibXGrid).GetCM

            If cm Is Nothing OrElse cm.Position < 0 Then
                Exit Sub
            End If

            Dim cellRect As Rectangle = Grid.GetCellBounds(row, Me.getColumnNumber)
            Dim isVis As Boolean = True

            If Me.Width = 0 Then
                isVis = False
            End If

            If Not value Is DBNull.Value Then
                MyBase.Edit(cm, row, cellRect, Me.ReadOnly, value, isVis)
            Else
                MyBase.Edit(cm, row, cellRect, Me.ReadOnly, String.Empty, isVis)
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub


End Class 'DataGridButtonColumn


