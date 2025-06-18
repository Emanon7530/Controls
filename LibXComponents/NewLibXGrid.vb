Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing ' For Point, Rectangle if used in EventArgs
Imports System.Collections ' For ArrayList if used in EventArgs (not directly used by the ported arg classes, but good to have for future)
Imports System.Data ' For DataGridColumnStyle in XDataGridFooterColumnGridStyleCreatingEventArgs (will need changing)
Imports System.Linq ' For OfType

Public Class NewLibXGrid
    Inherits System.Windows.Forms.DataGridView

    ' Private fields for ported properties
    Private mUseAutoFillLines As Boolean = True
    Private mUseHandCursor As Boolean
    Private m_blnIsReadOnly As Boolean ' For the custom IsReadOnly property
    Private m_blnFullRowSelect As Boolean
    Private m_blnAutoSearch As Boolean
    Private m_blnShowFooterBar As Boolean
    Private m_AutoAdjustLastColumn As Boolean = True

    ' Fields required for AdjustColumnSizeEx
    Private m_intLastVisibleCol As Integer = -1
    Private m_blnIsFirstTimeScroolHVisible As Boolean = True

    ' Fields for ProcessCmdKey and other ported logic
    ' Private mWasTab As Boolean ' TODO: Review if needed for complex tab-out scenarios
    ' Private mFirstAfterAutoSearch As Boolean ' TODO: For AutoSearch feature

    ' Fields for OnCurrentCellChanged
    Private m_intOldCurrentRow As Integer = -1
    Private m_intOldCurrentCol As Integer = -1
    Private m_blnOkToValidate As Boolean = True ' TODO: Review usage with DGV validation model


    ' TODO: Port constructor logic from old LibXGrid if necessary, adapting for DataGridView.
    Public Sub New()
        MyBase.New()
        ' Basic DataGridView settings can be applied here
        ' Example:
        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.IsReadOnly = True ' Set our custom property, which in turn sets base.ReadOnly
        Me.FullRowSelect = True ' Example: Default to FullRowSelect
        ' Me.SelectionMode = DataGridViewSelectionMode.FullRowSelect ' This is handled by FullRowSelect property setter

        ' TODO: Review if AddHandler HeaderClicked is needed here or if DGV events are sufficient
        ' AddHandler HeaderClicked, AddressOf InternalHeaderClick

        ' TODO: VertScrollBar.VisibleChanged is not directly available. Scroll event might be an alternative for some scenarios.
        ' AddHandler Me.VertScrollBar.VisibleChanged, AddressOf verscrollVisibleChange

        AddHandler Me.Scroll, AddressOf OnGridScroll
        AddHandler Me.CellValidating, AddressOf NewLibXGrid_CellValidating ' Wire up CellValidating event
    End Sub

#Region "Custom Event Argument Classes"
    Public Class XDataGridHeaderEventArgs
        Inherits EventArgs
        Private m_intColumn As Integer = -1
        Private m_intRow As Integer = -1
        Sub New(ByVal p_intCol As Integer, ByVal p_intRow As Integer)
            m_intColumn = p_intCol
            m_intRow = p_intRow
        End Sub
        Public ReadOnly Property column() As Integer
            Get
                Return m_intColumn
            End Get
        End Property
        Public ReadOnly Property row() As Integer
            Get
                Return m_intRow
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return String.Format("Column = {0}, Row = {1}", m_intColumn, m_intRow)
        End Function
    End Class

    Public Class XDataGridFooterChangingEventArgs
        Inherits EventArgs
        Public ColumnName As String
        Public ColumnResult As Object
    End Class

    Public Class XDataGridCurrentRowChangedEventArgs
        Inherits EventArgs
        Private m_intColumn As Integer = -1
        Private m_intRow As Integer = -1
        Private m_intColumnA As Integer = -1
        Private m_intRowA As Integer = -1

        Sub New(ByVal p_intCol As Integer, ByVal p_intRow As Integer, ByVal p_intPrevRow As Integer, ByVal p_intPrevCol As Integer)
            m_intColumn = p_intCol
            m_intRow = p_intRow
            m_intColumnA = p_intPrevCol
            m_intRowA = p_intPrevRow
        End Sub
        Public ReadOnly Property column() As Integer
            Get
                Return m_intColumn
            End Get
        End Property
        Public ReadOnly Property row() As Integer
            Get
                Return m_intRow
            End Get
        End Property
        Public ReadOnly Property previousColumn() As Integer
            Get
                Return m_intColumnA
            End Get
        End Property
        Public ReadOnly Property previousRow() As Integer
            Get
                Return m_intRowA
            End Get
        End Property
    End Class

    Public Class LibXGridCellValidateEventArgs
        Inherits EventArgs
        Public value As String
        Public cell As Integer
        Public row As Integer
        Public hasErrors As Boolean
    End Class

    Public Class XDataGridFooterColumnCreatingEventArgs
        Inherits EventArgs
        Public ColName As String
        Public ColType As Type
    End Class

    Public Class XDataGridFooterColumnGridStyleCreatingEventArgs
        Inherits EventArgs
        Public ColName As String
        Public ColStyle As System.Windows.Forms.DataGridColumnStyle ' TODO: Change ColStyle to DataGridViewColumn or a more appropriate type for DataGridView
    End Class
#End Region

#Region "Ported Properties"
    Public Property UseAutoFillLines() As Boolean
        Get
            Return mUseAutoFillLines
        End Get
        Set(ByVal Value As Boolean)
            mUseAutoFillLines = Value
            ' TODO: Trigger repaint or update relevant logic if needed (e.g., Me.Invalidate())
        End Set
    End Property

    Public Property UseHandCursor() As Boolean
        Get
            Return mUseHandCursor
        End Get
        Set(ByVal Value As Boolean)
            mUseHandCursor = Value
            ' TODO: Logic in WndProc will handle cursor change, or use MouseMove event
        End Set
    End Property

    Public Overloads Property IsReadOnly() As Boolean ' Overloads to distinguish from base.ReadOnly if needed, or can shadow
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
            MyBase.ReadOnly = Value ' Sync with base DataGridView ReadOnly
        End Set
    End Property

    Public Overrides Property ReadOnly As Boolean
        Get
            Return MyBase.ReadOnly
        End Get
        Set(value As Boolean)
            MyBase.ReadOnly = value
            m_blnIsReadOnly = value ' Keep custom field in sync
        End Set
    End Property

    Public Property FullRowSelect() As Boolean
        Get
            Return m_blnFullRowSelect
        End Get
        Set(ByVal Value As Boolean)
            m_blnFullRowSelect = Value
            If Value Then
                Me.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Else
                Me.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect
            End If
        End Set
    End Property

    Public Property AutoSearch() As Boolean
        Get
            Return m_blnAutoSearch
        End Get
        Set(ByVal Value As Boolean)
            m_blnAutoSearch = Value
        End Set
    End Property

    Public Property showFooterBar() As Boolean
        Get
            Return m_blnShowFooterBar
        End Get
        Set(ByVal Value As Boolean)
            m_blnShowFooterBar = Value
        End Set
    End Property

    Public Property AutoAdjustLastColumn() As Boolean
        Get
            Return m_AutoAdjustLastColumn
        End Get
        Set(ByVal Value As Boolean)
            m_AutoAdjustLastColumn = Value
        End Set
    End Property
#End Region

#Region "Custom Event Declarations"
    Public Event FooterChanging(ByVal sender As Object, ByVal e As XDataGridFooterChangingEventArgs)
    Public Event CurrentRowChanged(ByVal sender As Object, ByVal e As XDataGridCurrentRowChangedEventArgs)
    Public Event HeaderClicked(ByVal e As XDataGridHeaderEventArgs)
    Public Event CellValidate(ByVal sender As Object, ByVal e As LibXGridCellValidateEventArgs)
    Public Event CellKeyPress(ByVal sender As Object, ByVal e As KeyEventArgs)
    Public Event rowHeaderDblClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event columnDblClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event GridScrolling(ByVal sender As Object, ByVal e As ScrollEventArgs)
    Public Event Refreshing(ByVal sender As Object, ByVal e As EventArgs)
    Public Event FooterColumnCreating(ByVal sender As Object, ByVal e As XDataGridFooterColumnCreatingEventArgs)
    Public Event FooterColumnGridStyleCreating(ByVal sender As Object, ByVal e As XDataGridFooterColumnGridStyleCreatingEventArgs)
#End Region

#Region "Data Access Methods"
    Public Function getColByName(p_strName As String) As Integer
        Try
            If Me.Columns.Contains(p_strName) Then
                Return Me.Columns(p_strName).Index
            Else
                For i As Integer = 0 To Me.Columns.Count - 1
                    If String.Equals(Me.Columns(i).DataPropertyName, p_strName, StringComparison.OrdinalIgnoreCase) Then
                        Return i
                    End If
                Next
                Return -1
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in getColByName: " & ex.ToString)
            Return -1
        End Try
    End Function

    Public Function getColByName(p_strName As String, ByRef p_objCol As DataGridViewColumn) As Integer
        p_objCol = Nothing
        Try
            If Me.Columns.Contains(p_strName) Then
                p_objCol = Me.Columns(p_strName)
                Return p_objCol.Index
            Else
                For i As Integer = 0 To Me.Columns.Count - 1
                    If String.Equals(Me.Columns(i).DataPropertyName, p_strName, StringComparison.OrdinalIgnoreCase) Then
                        p_objCol = Me.Columns(i)
                        Return i
                    End If
                Next
                Return -1
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in getColByName (with ByRef): " & ex.ToString)
            Return -1
        End Try
    End Function

    Public Function GetValue(ByVal row As Integer, ByVal col As Integer) As String
        Try
            If row < 0 OrElse row >= Me.Rows.Count OrElse Me.Rows(row).IsNewRow OrElse col < 0 OrElse col >= Me.Columns.Count Then
                Return String.Empty
            End If

            Dim cellValue As Object = Me.Rows(row).Cells(col).Value
            If cellValue Is Nothing OrElse Convert.IsDBNull(cellValue) Then
                Return String.Empty
            Else
                Return cellValue.ToString().Trim()
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in GetValue(row, col): " & ex.ToString)
            Return String.Empty
        End Try
    End Function

    Public Function GetValue(ByVal row As Integer, ByVal colName As String) As String
        Dim colIndex As Integer = getColByName(colName)
        If colIndex = -1 Then Return String.Empty
        Return GetValue(row, colIndex)
    End Function

    Public Function GetValue(ByVal colName As String) As String
        If Me.CurrentCell IsNothing Then Return String.Empty
        Return GetValue(Me.CurrentCell.RowIndex, getColByName(colName))
    End Function

    Public Function GetValue(ByVal col As Integer) As String
        If Me.CurrentCell IsNothing Then Return String.Empty
        Return GetValue(Me.CurrentCell.RowIndex, col)
    End Function


    Public Sub SetValue(ByVal row As Integer, ByVal col As Integer, ByVal value As Object)
        Try
            If row < 0 OrElse row >= Me.Rows.Count OrElse Me.Rows(row).IsNewRow OrElse col < 0 OrElse col >= Me.Columns.Count Then
                Exit Sub
            End If

            Dim valueToSet As Object
            If value Is Nothing Then
                valueToSet = DBNull.Value
            ElseIf TypeOf value Is String AndAlso String.IsNullOrEmpty(CStr(value)) Then
                valueToSet = DBNull.Value
            Else
                valueToSet = value
            End If

            Me.Rows(row).Cells(col).Value = valueToSet

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in SetValue(row, col, value): " & ex.ToString)
        End Try
    End Sub

    Public Sub SetValue(ByVal row As Integer, ByVal colName As String, ByVal value As Object)
        Dim colIndex As Integer = getColByName(colName)
        If colIndex <> -1 Then
            SetValue(row, colIndex, value)
        End If
    End Sub

    Public Sub SetValue(ByVal colName As String, ByVal value As Object)
        If Me.CurrentCell IsNothing Then Exit Sub
        SetValue(Me.CurrentCell.RowIndex, getColByName(colName), value)
    End Sub

    Public Sub SetValue(ByVal col As Integer, ByVal value As Object)
        If Me.CurrentCell IsNothing Then Exit Sub
        SetValue(Me.CurrentCell.RowIndex, col, value)
    End Sub

#End Region

    Private Sub OnGridScroll(ByVal sender As Object, ByVal e As EventArgs)
        If TypeOf e Is ScrollEventArgs Then
            RaiseEvent GridScrolling(sender, CType(e, ScrollEventArgs))
        End If
    End Sub

    Private Sub NewLibXGrid_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)
        Try
            If Me.IsReadOnly OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse e.RowIndex >= Me.RowCount OrElse e.ColumnIndex >= Me.ColumnCount Then
                Exit Sub
            End If
            If Me.Rows(e.RowIndex).IsNewRow AndAlso e.FormattedValue IsNot Nothing AndAlso e.FormattedValue.ToString() = String.Empty Then
                Exit Sub
            End If
            If Me.Columns(e.ColumnIndex).ReadOnly Then
                Exit Sub
            End If

            Dim customArgs As New LibXGridCellValidateEventArgs()
            customArgs.row = e.RowIndex
            customArgs.cell = e.ColumnIndex
            customArgs.value = CStr(e.FormattedValue)
            customArgs.hasErrors = False

            RaiseEvent CellValidate(Me, customArgs)

            If customArgs.hasErrors Then
                e.Cancel = True
            Else
                If e.RowIndex < Me.Rows.Count AndAlso Not Me.Rows(e.RowIndex).IsNewRow AndAlso e.ColumnIndex < Me.Columns.Count Then
                     Me.Rows(e.RowIndex).Cells(e.ColumnIndex).ErrorText = String.Empty
                End If
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in NewLibXGrid_CellValidating: " & ex.ToString())
        End Try
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.Enter Then
            System.Windows.Forms.SendKeys.Send("{Tab}")
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        RaiseEvent CellKeyPress(Me, e)
    End Sub

    Protected Overrides Sub OnCurrentCellChanged(e As EventArgs)
        MyBase.OnCurrentCellChanged(e)

        If Me.CurrentCell Is Nothing Then
            m_intOldCurrentRow = -1
            m_intOldCurrentCol = -1
            Exit Sub
        End If

        Dim newRow As Integer = Me.CurrentCell.RowIndex
        Dim newCol As Integer = Me.CurrentCell.ColumnIndex

        Try
            If Me.FullRowSelect AndAlso newRow >= 0 Then
                 If Me.Rows.Count > newRow AndAlso Not Me.Rows(newRow).IsNewRow Then Me.Rows(newRow).Selected = True
            End If

            If m_intOldCurrentRow <> -1 AndAlso newRow <> m_intOldCurrentRow Then
                If m_blnOkToValidate Then
                    Dim args As New XDataGridCurrentRowChangedEventArgs(newCol, newRow, m_intOldCurrentRow, m_intOldCurrentCol)
                    RaiseEvent CurrentRowChanged(Me, args)
                End If
            End If

            ' TODO: Review and port validation logic for the *previous* cell (m_intOldCurrentRow, m_intOldCurrentCol)
            ' if DataGridView.CellValidating doesn't cover all original scenarios.
            ' This involves checking m_blnOkToValidate and data source edit state.

            ' TODO: Port mWasTab logic if complex tabbing behavior is still required.

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in OnCurrentCellChanged: " & ex.ToString())
        Finally
            m_intOldCurrentRow = newRow
            m_intOldCurrentCol = newCol
        End Try
    End Sub

    Public Sub AdjustColumnSizeEx(ByVal p_blnInForce As Boolean, ByVal checkStringWidh As Boolean)
        Try
            If Not m_AutoAdjustLastColumn Then
                Exit Sub
            End If

            If Me.DesignMode Then
                Exit Sub
            End If

            If Me.ColumnCount = 0 Then Exit Sub

            Dim numCols As Integer = Me.Columns.Count
            Dim dataTable1 As DataTable = Nothing
            Dim dataSourceAsObj As Object = Me.DataSource

            If TypeOf dataSourceAsObj Is BindingSource Then
                dataSourceAsObj = CType(dataSourceAsObj, BindingSource).DataSource
            End If

            If TypeOf dataSourceAsObj Is DataTable Then
                dataTable1 = CType(dataSourceAsObj, DataTable)
            ElseIf TypeOf dataSourceAsObj Is DataSet Then
                If Not String.IsNullOrEmpty(Me.DataMember) Then
                    If CType(dataSourceAsObj, DataSet).Tables.Contains(Me.DataMember) Then
                        dataTable1 = CType(dataSourceAsObj, DataSet).Tables(Me.DataMember)
                    End If
                ElseIf CType(dataSourceAsObj, DataSet).Tables.Count > 0 Then
                     dataTable1 = CType(dataSourceAsObj, DataSet).Tables(0)
                End If
            End If

            Dim scrollBarWidth As Integer
            Dim vScroll As VScrollBar = Me.Controls.OfType(Of VScrollBar)().FirstOrDefault()
            If vScroll IsNot Nothing AndAlso vScroll.Visible Then
                scrollBarWidth = vScroll.Width
            Else
                scrollBarWidth = 0
            End If

            Dim clientWidthForColumns = Me.ClientSize.Width - scrollBarWidth - SystemInformation.BorderSize.Width * 2
            If Me.RowHeadersVisible Then
                clientWidthForColumns -= Me.RowHeadersWidth
            End If

            Dim runningWidthUsed As Integer = 0
            Dim local_m_intLastVisibleCol As Integer = -1

            For colIndex As Integer = Me.Columns.Count - 1 To 0 Step -1
                Dim gridCol As DataGridViewColumn = Me.Columns(colIndex)
                If gridCol.Visible Then
                    If gridCol.Width > 0 Then
                        If dataTable1 IsNot Nothing AndAlso Not String.IsNullOrEmpty(gridCol.DataPropertyName) AndAlso Not dataTable1.Columns.Contains(gridCol.DataPropertyName) Then
                             Continue For
                        End If
                        local_m_intLastVisibleCol = colIndex
                        Exit For
                    End If
                End If
            Next

            If local_m_intLastVisibleCol = -1 Then Exit Sub

            For colIndex As Integer = 0 To local_m_intLastVisibleCol - 1
                Dim gridCol As DataGridViewColumn = Me.Columns(colIndex)
                If gridCol.Visible Then
                     If gridCol.Width > 0 Then
                        runningWidthUsed += gridCol.Width
                    End If
                End If
            Next

            Dim lastColActual As DataGridViewColumn = Me.Columns(local_m_intLastVisibleCol)
            If lastColActual.AutoSizeMode <> DataGridViewAutoSizeColumnMode.Fill Then
                Dim lastColWidth As Integer = clientWidthForColumns - runningWidthUsed
                If lastColWidth > lastColActual.MinimumWidth Then
                    lastColActual.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    lastColActual.Width = lastColWidth
                Else
                    lastColActual.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    lastColActual.Width = lastColActual.MinimumWidth
                End If
            End If

            m_blnIsFirstTimeScroolHVisible = False

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in AdjustColumnSizeEx: " & ex.ToString)
        Finally
        End Try
    End Sub

    Public Sub AdjustColumnSize(Optional ByVal p_blnInForce As Boolean = False)
        AdjustColumnSizeEx(p_blnInForce, False)
    End Sub

    ' TODO: Port methods from old LibXGrid.vb
End Class
