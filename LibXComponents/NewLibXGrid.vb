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
    Private m_blnAutoSearch As Boolean ' Property Backing Field
    Private m_blnShowFooterBar As Boolean
    Private m_AutoAdjustLastColumn As Boolean = True

    ' Fields required for AdjustColumnSizeEx
    Private m_intLastVisibleCol As Integer = -1
    Private m_blnIsFirstTimeScroolHVisible As Boolean = True

    ' Fields for ProcessCmdKey and other ported logic
    Private mWasTab As Boolean ' TODO: Review if needed for complex tab-out scenarios

    ' Fields for OnCurrentCellChanged
    Private m_intOldCurrentRow As Integer = -1
    Private m_intOldCurrentCol As Integer = -1
    Private m_blnOkToValidate As Boolean = True ' TODO: Review usage with DGV validation model

    ' Fields for OnDataSourceChanged
    Private m_currentDataTable As DataTable
    Private m_currentConnector As Object ' TODO: Change to LibXConnector if direct reference is kept
    Private m_objCols As Collection ' For NewLibXGrid_DataTableRowChanged logic

    ' Fields for AutoSearch feature
    Private m_strSearchText As String = ""
    Private m_intSelectedHeaderColumn As Integer = -1 ' Used to know which column to search on
    Private m_blnAutoSearching As Boolean ' Different from m_blnAutoSearch which is the public property enabling the feature
    Private m_blnAutoSearchingCancel As Boolean
    Friend mFirstAfterAutoSearch As Boolean ' TODO: Review Friend access modifier, may need to be Private
    Private m_blnStartEdit As Boolean ' TODO: Review and implement full edit lifecycle logic

    ' Color fields for CheckBankColor and related methods
    Private m_objSavedColor As Color
    Private m_objSavedEditColor As Color
    Private m_objSavedEditForeColor As Color
    Private m_objSavedForeColor As Color
    Private m_blnSetForDetail As Boolean
    Private m_blnIsDetail As Boolean
    Private mBack As Color ' Used in original onDsChangeState for background color restore

    Private Const WM_LBUTTONDOWN As Integer = &H201
    Private Const WM_MOUSEMOVE As Integer = &H200


    Public Sub New()
        MyBase.New()
        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.IsReadOnly = True
        Me.FullRowSelect = True

        AddHandler Me.Scroll, AddressOf OnGridScroll
        AddHandler Me.CellValidating, AddressOf NewLibXGrid_CellValidating
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
            Me.Invalidate()
        End Set
    End Property

    Public Property UseHandCursor() As Boolean
        Get
            Return mUseHandCursor
        End Get
        Set(ByVal Value As Boolean)
            mUseHandCursor = Value
        End Set
    End Property

    Public Overloads Property IsReadOnly() As Boolean
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
            MyBase.ReadOnly = Value
        End Set
    End Property

    Public Overrides Property ReadOnly As Boolean
        Get
            Return MyBase.ReadOnly
        End Get
        Set(value As Boolean)
            MyBase.ReadOnly = value
            m_blnIsReadOnly = value
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

#Region "AutoSearch Related Properties and Methods"
    Public Property searchText() As String
        Get
            Return m_strSearchText
        End Get
        Set(ByVal Value As String)
            m_strSearchText = Value
            If IsSearchMode() Then
                Search(m_strSearchText)
            Else
                ' If not in search mode but text is set (e.g. programmatically), clear any existing filter
                If String.IsNullOrEmpty(Value) Then Search(String.Empty)
            End If
        End Set
    End Property

    Private Function IsSearchMode() As Boolean
        Return Me.AutoSearch AndAlso m_intSelectedHeaderColumn > -1 AndAlso m_intSelectedHeaderColumn < Me.ColumnCount
    End Function

    Private Sub Search(ByVal p_strSearchText As String)
        Try
            Dim filterString As String
            If String.IsNullOrEmpty(p_strSearchText) Then
                filterString = String.Empty ' Clear filter
            Else
                filterString = GenerateFilterExpression(p_strSearchText)
            End If

            If TypeOf Me.DataSource Is BindingSource Then
                Dim bs As BindingSource = CType(Me.DataSource, BindingSource)
                If bs.SupportsFiltering Then
                    bs.Filter = filterString
                Else
                    System.Diagnostics.Debug.WriteLine("Warning: DataSource (BindingSource) does not support filtering.")
                End If
            ElseIf TypeOf Me.DataSource Is DataTable Then
                CType(Me.DataSource, DataTable).DefaultView.RowFilter = filterString
            ElseIf Me.DataSource IsNot Nothing AndAlso Not String.IsNullOrEmpty(Me.DataMember) AndAlso TypeOf Me.DataSource Is DataSet Then
                Dim ds As DataSet = CType(Me.DataSource, DataSet)
                If ds.Tables.Contains(Me.DataMember) Then
                    ds.Tables(Me.DataMember).DefaultView.RowFilter = filterString
                End If
            Else
                System.Diagnostics.Debug.WriteLine("Warning: AutoSearch filtering not applied. Unsupported DataSource type or configuration.")
            End If

            DisplaySearchText() ' Update UI feedback for search text
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in Search: " & ex.ToString())
            ' Optionally, clear filter on error:
            Try
                If TypeOf Me.DataSource Is BindingSource Then
                    CType(Me.DataSource, BindingSource).Filter = Nothing
                ElseIf TypeOf Me.DataSource Is DataTable Then
                    CType(Me.DataSource, DataTable).DefaultView.RowFilter = Nothing
                ElseIf Me.DataSource IsNot Nothing AndAlso Not String.IsNullOrEmpty(Me.DataMember) AndAlso TypeOf Me.DataSource Is DataSet Then
                     Dim ds As DataSet = CType(Me.DataSource, DataSet)
                    If ds.Tables.Contains(Me.DataMember) Then
                         ds.Tables(Me.DataMember).DefaultView.RowFilter = Nothing
                    End If
                End If
            Catch exClear As Exception
                System.Diagnostics.Debug.WriteLine("Error clearing filter in Search: " & exClear.ToString())
            End Try
        End Try
    End Sub

    Public Sub ResetautoSearch()
        m_blnAutoSearching = False
        Me.searchText = ""
    End Sub

    <Browsable(False)> _
    Public ReadOnly Property isAutoSearching() As Boolean
        Get
            If m_blnAutoSearching Then
                If Me.m_blnAutoSearchingCancel Then
                    m_blnAutoSearching = False
                    m_blnAutoSearchingCancel = False
                    Return True
                End If
            End If
            Return m_blnAutoSearching
        End Get
    End Property

    Private Sub DisplaySearchText()
        Dim strTextToDisplay As String
        If String.IsNullOrEmpty(Me.searchText) Then
            strTextToDisplay = "Search cleared."
        Else
            Dim headerText As String = ""
            If m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.Columns.Count Then
                headerText = Me.Columns(m_intSelectedHeaderColumn).HeaderText
            End If
            If String.IsNullOrEmpty(headerText) AndAlso m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.Columns.Count Then
                 headerText = Me.Columns(m_intSelectedHeaderColumn).Name
            End If
            strTextToDisplay = $"Searching in [{headerText}]: {Me.searchText}"
        End If
        System.Diagnostics.Debug.WriteLine(strTextToDisplay)
    End Sub

    Private Function GenerateFilterExpression(ByVal searchValue As String) As String
        If m_intSelectedHeaderColumn < 0 OrElse String.IsNullOrEmpty(Me.SelectedHeaderField) Then
            Return String.Empty
        End If
        If String.IsNullOrEmpty(searchValue) Then Return String.Empty


        Dim fieldName As String = Me.SelectedHeaderField
        Dim fieldType As Type = Me.SelectedHeaderFieldType

        Dim escapedSearchValue As String = searchValue.Replace("'", "''").Replace("%", "[%]").Replace("*", "[*]")

        If fieldType Is GetType(String) Then
            Return String.Format("[{0}] LIKE '%{1}%'", fieldName, escapedSearchValue)
        ElseIf fieldType Is GetType(DateTime) Then
            ' TODO: Robust DateTime filtering might require parsing searchValue and specific DB syntax, or converting column to a specific string format.
            Return String.Format("CONVERT([{0}], 'System.String') LIKE '%{1}%'", fieldName, escapedSearchValue)
        ElseIf Numerics.NumericTypeHelper.IsNumericType(fieldType) Then
            If IsNumeric(searchValue) Then
                Return String.Format("CONVERT([{0}], 'System.String') LIKE '{1}%'", fieldName, escapedSearchValue)
            Else
                Return "1=0" ' Invalid search for numeric type, return no rows
            End If
        ElseIf fieldType Is GetType(Boolean) Then
            Dim boolVal As Boolean
            If Boolean.TryParse(searchValue, boolVal) Then
                Return String.Format("[{0}] = {1}", fieldName, boolVal)
            Else
                ' Handle cases like "y", "n", "true", "false" based on desired behavior
                If "yes".StartsWith(searchValue.ToLower) OrElse "true".StartsWith(searchValue.ToLower) OrElse searchValue = "1" Then
                    Return String.Format("[{0}] = True", fieldName)
                ElseIf "no".StartsWith(searchValue.ToLower) OrElse "false".StartsWith(searchValue.ToLower) OrElse searchValue = "0" Then
                     Return String.Format("[{0}] = False", fieldName)
                Else
                    Return "1=0" ' Invalid boolean search
                End If
            End If
        Else
            Return String.Format("CONVERT([{0}], 'System.String') LIKE '%{1}%'", fieldName, escapedSearchValue)
        End If
    End Function

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderField() As String
        Get
            If m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.Columns.Count Then
                If Not String.IsNullOrEmpty(Me.Columns(m_intSelectedHeaderColumn).DataPropertyName) Then
                    Return Me.Columns(m_intSelectedHeaderColumn).DataPropertyName
                Else
                    Return Me.Columns(m_intSelectedHeaderColumn).Name
                End If
            End If
            Return String.Empty
        End Get
    End Property

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderFieldCaption() As String
        Get
            If m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.Columns.Count Then
                Return Me.Columns(m_intSelectedHeaderColumn).HeaderText
            End If
            Return String.Empty
        End Get
    End Property

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderFieldType() As Type
        Get
            If Me.DataSource IsNot Nothing AndAlso m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.Columns.Count Then
                Dim fieldNameToCheck As String = Me.Columns(m_intSelectedHeaderColumn).DataPropertyName
                If String.IsNullOrEmpty(fieldNameToCheck) Then fieldNameToCheck = Me.Columns(m_intSelectedHeaderColumn).Name

                If Not String.IsNullOrEmpty(fieldNameToCheck) Then
                    Dim dataView As DataView = Nothing
                    Dim listSource As Object = Me.DataSource
                    Dim dataMemberProperty As String = Me.DataMember

                    If TypeOf listSource Is BindingSource Then
                        Dim bs = CType(listSource, BindingSource)
                        listSource = bs.DataSource
                        If String.IsNullOrEmpty(dataMemberProperty) Then dataMemberProperty = bs.DataMember
                    End If

                    If TypeOf listSource Is DataSet Then
                        If Not String.IsNullOrEmpty(dataMemberProperty) AndAlso CType(listSource, DataSet).Tables.Contains(dataMemberProperty) Then
                            dataView = CType(listSource, DataSet).Tables(dataMemberProperty).DefaultView
                        ElseIf CType(listSource, DataSet).Tables.Count > 0 Then
                            dataView = CType(listSource, DataSet).Tables(0).DefaultView
                        End If
                    ElseIf TypeOf listSource Is DataTable Then
                        dataView = CType(listSource, DataTable).DefaultView
                    ElseIf TypeOf listSource Is DataView Then
                        dataView = CType(listSource, DataView)
                    End If

                    If dataView IsNot Nothing AndAlso dataView.Table.Columns.Contains(fieldNameToCheck) Then
                        Return dataView.Table.Columns(fieldNameToCheck).DataType
                    End If
                End If
            End If
            Return GetType(String)
        End Get
    End Property

    Private Function IsValidChar(ByVal c As Char) As Boolean
        Dim currentType As Type = SelectedHeaderFieldType
        If currentType Is Nothing Then Return False

        If currentType Is GetType(String) Then
            Return True
        ElseIf currentType Is GetType(DateTime) Then
            Return Char.IsDigit(c) OrElse "/: ".Contains(c) OrElse c = "-"c OrElse Char.IsLetter(c) ' Allow letters for month names etc.
        ElseIf Numerics.NumericTypeHelper.IsNumericType(currentType) Then
            Return Char.IsDigit(c) OrElse (c = "."c AndAlso Me.searchText.IndexOf(".") = -1) OrElse (c = "-"c AndAlso Me.searchText.Length = 0) OrElse (c = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator(0) AndAlso Me.searchText.IndexOf(Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) = -1)
        ElseIf currentType Is GetType(Boolean) Then
            Return "ynotruefal".Contains(Char.ToLower(c)) ' y, n, o, t, r, u, e, f, a, l
        End If
        Return False
    End Function
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
            If row < 0 OrElse row >= Me.Rows.Count OrElse (Me.Rows.Count > 0 AndAlso Me.Rows(row).IsNewRow) OrElse col < 0 OrElse col >= Me.Columns.Count Then
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
            If row < 0 OrElse row >= Me.Rows.Count OrElse (Me.Rows.Count > 0 AndAlso Me.Rows(row).IsNewRow) OrElse col < 0 OrElse col >= Me.Columns.Count Then
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

#Region "Event Handlers and Overrides"
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

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        If IsSearchMode() Then
            If m_blnAutoSearchingCancel Then
                m_blnAutoSearching = False
                m_blnAutoSearchingCancel = False
            End If

            If Char.IsControl(e.KeyChar) Then
                Select Case CType(e.KeyChar, Byte)
                    Case Convert.ToByte(Keys.Back)
                        If searchText.Length > 0 Then
                            Me.searchText = Me.searchText.Substring(0, Me.searchText.Length - 1)
                        End If
                        e.Handled = True
                    Case Convert.ToByte(Keys.Escape)
                        mFirstAfterAutoSearch = True
                        Me.searchText = ""
                        m_blnAutoSearchingCancel = True
                        If Me.RowCount > 0 Then
                            Me.Focus()
                            If Me.Rows.Count > 0 AndAlso Me.Columns.Count > 0 AndAlso Me.Rows(0).Cells.Count > 0 Then
                                Dim targetColIndex As Integer = If(m_intSelectedHeaderColumn >= 0 AndAlso m_intSelectedHeaderColumn < Me.ColumnCount, m_intSelectedHeaderColumn, 0)
                                If targetColIndex < Me.Columns.Count AndAlso Me.Rows(0).Cells.Count > targetColIndex Then
                                   Me.CurrentCell = Me.Rows(0).Cells(targetColIndex)
                                End If
                            End If
                        End If
                        e.Handled = True
                    Case Else
                         MyBase.OnKeyPress(e)
                End Select
            ElseIf IsValidChar(e.KeyChar) Then
                Me.searchText += e.KeyChar
                m_blnAutoSearching = True
                e.Handled = True
            Else
                MyBase.OnKeyPress(e)
            End If
        Else
            MyBase.OnKeyPress(e)
        End If
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

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in OnCurrentCellChanged: " & ex.ToString())
        Finally
            m_intOldCurrentRow = newRow
            m_intOldCurrentCol = newCol
        End Try
    End Sub

    Private Sub ProcessHeaderClick(columnIndex As Integer, rowIndex As Integer)
        Try
            If Me.m_intSelectedHeaderColumn <> columnIndex Then
                Me.m_intSelectedHeaderColumn = columnIndex
                If Me.AutoSearch Then Me.searchText = ""
            End If

            If Not Me.Focused Then
                Me.Focus()
            End If

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in ProcessHeaderClick: " & ex.ToString())
        End Try
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        Try
            Dim hitTestInfo As DataGridView.HitTestInfo = Me.HitTest(e.X, e.Y)

            If hitTestInfo.Type = DataGridViewHitTestType.ColumnHeader Then
                If hitTestInfo.ColumnIndex >= 0 Then
                    ProcessHeaderClick(hitTestInfo.ColumnIndex, hitTestInfo.RowIndex)

                    Dim headerClickArgs As New XDataGridHeaderEventArgs(hitTestInfo.ColumnIndex, hitTestInfo.RowIndex)
                    RaiseEvent HeaderClicked(headerClickArgs)
                End If
            End If
        Finally
            MyBase.OnMouseUp(e)
        End Try
    End Sub

    Protected Overrides Sub OnDataSourceChanged(e As EventArgs)
        MyBase.OnDataSourceChanged(e)

        If Me.DesignMode Then
            Exit Sub
        End If

        If m_currentDataTable IsNot Nothing Then
            RemoveHandler m_currentDataTable.RowChanged, AddressOf Me.NewLibXGrid_DataTableRowChanged
            RemoveHandler m_currentDataTable.ColumnChanged, AddressOf Me.NewLibXGrid_DataTableColumnChanged
        End If

        m_currentDataTable = Nothing
        m_currentConnector = Nothing

        Dim currentDataSourceAsObj As Object = Me.DataSource

        If currentDataSourceAsObj Is Nothing Then
            MyBase.ReadOnly = True
            Exit Sub
        End If

        If TypeOf currentDataSourceAsObj Is BindingSource Then
            Dim bs As BindingSource = CType(currentDataSourceAsObj, BindingSource)
            If TypeOf bs.DataSource Is DataSet Then
                Dim ds As DataSet = CType(bs.DataSource, DataSet)
                If Not String.IsNullOrEmpty(bs.DataMember) AndAlso ds.Tables.Contains(bs.DataMember) Then
                    m_currentDataTable = ds.Tables(bs.DataMember)
                ElseIf ds.Tables.Count > 0 Then
                    m_currentDataTable = ds.Tables(0)
                End If
                 If ds.ExtendedProperties.Contains("xcone") Then
                    m_currentConnector = TryCast(ds.ExtendedProperties("xcone"), Object)
                End If
            ElseIf TypeOf bs.DataSource Is DataTable Then
                m_currentDataTable = CType(bs.DataSource, DataTable)
            End If
        ElseIf TypeOf currentDataSourceAsObj Is DataTable Then
            m_currentDataTable = CType(currentDataSourceAsObj, DataTable)
        ElseIf TypeOf currentDataSourceAsObj Is DataSet Then
             Dim ds As DataSet = CType(currentDataSourceAsObj, DataSet)
            If Not String.IsNullOrEmpty(Me.DataMember) AndAlso ds.Tables.Contains(Me.DataMember) Then
                m_currentDataTable = ds.Tables(Me.DataMember)
            ElseIf ds.Tables.Count > 0 Then
                m_currentDataTable = ds.Tables(0)
            End If
            If ds.ExtendedProperties.Contains("xcone") Then
                m_currentConnector = TryCast(ds.ExtendedProperties("xcone"), Object)
            End If
        End If

        If m_currentDataTable IsNot Nothing Then
            AddHandler m_currentDataTable.RowChanged, AddressOf Me.NewLibXGrid_DataTableRowChanged
            AddHandler m_currentDataTable.ColumnChanged, AddressOf Me.NewLibXGrid_DataTableColumnChanged
        End If

        For Each col As DataGridViewColumn In Me.Columns
            col.DefaultCellStyle.NullValue = String.Empty
        Next

        AdjustColumnSize()

        MyBase.ReadOnly = True
    End Sub

    Private Sub NewLibXGrid_DataTableRowChanged(sender As Object, e As DataRowChangeEventArgs)
        Try
            If e.Action = DataRowAction.Add Then
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in NewLibXGrid_DataTableRowChanged: " & ex.ToString())
        End Try
    End Sub

    Private Sub NewLibXGrid_DataTableColumnChanged(sender As Object, e As DataColumnChangeEventArgs)
        Try
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in NewLibXGrid_DataTableColumnChanged: " & ex.ToString())
        End Try
    End Sub
#End Region

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

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        MyBase.OnEnabledChanged(e)
        ' TODO: TASK_MARKER_REF_CheckBankColor - Port CheckBankColor() method and its dependencies, then call it here.
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        Try
            If Me.FullRowSelect AndAlso Me.CurrentRow IsNot Nothing AndAlso Me.CurrentRow.Index >= 0 Then
                If Me.Rows.Count > Me.CurrentRow.Index AndAlso Me.Rows(Me.CurrentRow.Index).IsNewRow = False Then ' Ensure row exists and is not the 'new row' placeholder
                    Me.Rows(Me.CurrentRow.Index).Selected = True
                End If
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in OnVisibleChanged (NewLibXGrid): " & ex.ToString())
        End Try
    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)
        MyBase.OnLeave(e)
        Try
            ' TODO: Review and port logic related to m_blnStartEdit.
            ' This was intended to potentially commit or validate an edit when focus leaves the grid.
            ' DataGridView has CellLeave and RowLeave events that might be relevant.
            ' Original logic sketch:
            ' If m_blnStartEdit Then
            '     If Me.ColumnCount > 0 Then ' Was TableStyles(0).GridColumnStyles.Count
            '         If Me.CurrentCell IsNot Nothing AndAlso Me.CurrentCell.ColumnIndex >= 0 AndAlso Me.CurrentCell.RowIndex >= 0 Then
            '             ' Potentially validate or commit Me.CurrentCell here
            '         End If
            '     End If
            ' End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in OnLeave (NewLibXGrid): " & ex.ToString())
            ' Original re-threw, consider if this is desired. For now, just log.
        End Try
    End Sub

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)
        Try
            Me.AdjustColumnSize()
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in OnSizeChanged (NewLibXGrid) calling AdjustColumnSize: " & ex.ToString())
            ' Original re-threw.
        End Try
    End Sub

    Protected Overrides Sub OnCellBeginEdit(e As DataGridViewCellCancelEventArgs)
        MyBase.OnCellBeginEdit(e)
        Me.m_blnStartEdit = True
        ' TODO: Original ColumnStartedEditing checked if m_objDataSource.IsEditing was false and exited.
        ' Need to integrate similar check based on new data source or a dedicated property.
    End Sub

    Public Sub CheckBankColor()
        ' TODO: This method requires a way to determine the current operational state (View, Edit, Add)
        ' and whether overall editing is allowed, previously obtained from LibXConnector (m_objDataSource).
        ' For now, using placeholder variables.
        Dim isCurrentlyEditing As Boolean = False ' Placeholder: True if in Add/Edit mode
        Dim isGridDisabled As Boolean = Not Me.Enabled ' Grid's own Enabled state
        ' Dim allowGridEditingFromConnector As Boolean = True ' Placeholder: from connector's AllowGridEditing
        ' Dim currentGridDisabledColor As Color = SystemColors.ControlDark ' Placeholder: from CAM.App.Configuration
        Dim currentGridDisabledColor As Color = SystemColors.ControlLight ' TODO: Replace with CAM.App.Configuration logic (actual value from old code was CAM.App.configuration.controlDisableColor())


        ' TODO: Determine isCurrentlyEditing based on a new mechanism (e.g., a property set by LibXConnector, or grid's own state reflecting if a cell is in edit mode or if the row is a new row being added)
        ' TODO: Determine allowGridEditingFromConnector (or an equivalent) to know if editing is generally permitted for this grid.
        ' TODO: Determine currentGridDisabledColor from a new configuration source for the application.

        If m_objSavedColor.IsEmpty AndAlso m_objSavedEditColor.IsEmpty Then
            ' If work colors (both normal and edit) haven't been set (e.g. via setWorkColors), no specific styling to apply.
            ' The IsEmpty check is more robust than .Equals(Color.Empty) if the Color struct could be uninitialized.
            Exit Sub
        End If

        Dim targetBackColor As Color
        Dim targetForeColor As Color

        If isGridDisabled Then
            targetBackColor = currentGridDisabledColor
            targetForeColor = SystemColors.GrayText ' Or SystemColors.InactiveCaptionText, or a specific configured disabled ForeColor
        Else
            ' The original logic complexly intertwined m_blnIsDetail, m_objDataSource.isEditing, m_objDataSource.isDetailEditing,
            ' m_objDataSource.AllowGridEditing, and m_objDataSource.AllowDetailGridEditing.
            ' Here's a simplified interpretation for the stub:
            ' If isCurrentlyEditing is true (placeholder for actual edit state) AND overall editing is allowed for this context (placeholder for AllowGridEditing etc.)
            ' then use edit colors. Otherwise, use normal saved colors.
            ' The Me.m_blnIsDetail field is available to refine this logic further.

            Dim effectiveEditingAllowed As Boolean = True ' TODO: Placeholder for: If Me.m_blnIsDetail Then m_objDataSource.AllowDetailGridEditing Else m_objDataSource.AllowGridEditing

            If isCurrentlyEditing AndAlso effectiveEditingAllowed Then
                targetBackColor = m_objSavedEditColor
                targetForeColor = m_objSavedEditForeColor
            Else ' Viewing or editing is not allowed for this specific context/row, or grid is generally not in edit mode
                targetBackColor = m_objSavedColor
                targetForeColor = m_objSavedForeColor
            End If
        End If

        ' Apply colors if they have been initialized (not Empty)
        If Not targetBackColor.IsEmpty Then
            If Me.DefaultCellStyle.BackColor <> targetBackColor Then Me.DefaultCellStyle.BackColor = targetBackColor
            If Me.RowsDefaultCellStyle.BackColor <> targetBackColor Then Me.RowsDefaultCellStyle.BackColor = targetBackColor
            If Me.AlternatingRowsDefaultCellStyle.BackColor <> targetBackColor Then Me.AlternatingRowsDefaultCellStyle.BackColor = targetBackColor
            If Me.BackgroundColor <> targetBackColor Then Me.BackgroundColor = targetBackColor ' For area outside cells
        End If

        If Not targetForeColor.IsEmpty Then
            If Me.DefaultCellStyle.ForeColor <> targetForeColor Then Me.DefaultCellStyle.ForeColor = targetForeColor
            If Me.RowsDefaultCellStyle.ForeColor <> targetForeColor Then Me.RowsDefaultCellStyle.ForeColor = targetForeColor
            If Me.AlternatingRowsDefaultCellStyle.ForeColor <> targetForeColor Then Me.AlternatingRowsDefaultCellStyle.ForeColor = targetForeColor
        End If

        ' TODO: Consider ColumnHeadersDefaultCellStyle if headers should also change based on these states.
        ' Example:
        ' If Not targetBackColor.IsEmpty Then Me.ColumnHeadersDefaultCellStyle.BackColor = targetBackColor
        ' If Not targetForeColor.IsEmpty Then Me.ColumnHeadersDefaultCellStyle.ForeColor = targetForeColor

        ' Force repaint if styles changed and it's not part of an existing paint cycle or if changes aren't immediate.
        ' Me.Invalidate() ' Generally, changing style properties should trigger a repaint. Use if needed.
        System.Diagnostics.Debug.WriteLine("CheckBankColor executed. Applied BackColor: " & targetBackColor.ToString() & " ForeColor: " & targetForeColor.ToString())
    End Sub

    Public Sub setIsDetail(ByVal p_blnIsDetail As Boolean)
        Me.m_blnIsDetail = p_blnIsDetail
    End Sub

    Public Sub setWorkColors(p_objBColor As Color, p_objFColor As Color, p_objSBcolor As Color, p_objSFcolor As Color, p_blnDetail As Boolean)
        If p_blnDetail AndAlso Not m_blnSetForDetail Then
            m_blnSetForDetail = True
            m_objSavedColor = Color.Empty ' Reset saved color if setting for detail for the first time
        End If

        ' Original LibXGrid only set these if m_objSavedColor was Color.Empty or (p_blnDetail And m_blnSetForDetail made it Empty for the first detail set)
        ' This implies colors are set once for master, and once for detail, but not overwritten on subsequent calls for the same context unless m_objSavedColor is reset.
        If Not m_objSavedColor.IsEmpty AndAlso Not (p_blnDetail AndAlso m_blnSetForDetail AndAlso m_objSavedColor.IsEmpty) Then
            ' If colors are already set (m_objSavedColor is not Empty) AND
            ' we are NOT in the specific case of setting detail colors for the very first time (where m_objSavedColor was just made Empty),
            ' then exit. This preserves the "set once" behavior for a given context (master/detail).
            ' The check "m_objSavedColor.IsEmpty" for the detail case is a bit redundant due to the line above but makes the condition explicit.
            ' A simpler way might be:
            ' If Not m_objSavedColor.IsEmpty AndAlso Not (p_blnDetail AndAlso m_blnSetForDetail) Then Exit Sub
            ' But the above is closer to the original's "Exit Sub if Not m_objSavedColor.Equals(Color.Empty)" after the detail flag check.
            ' Let's stick to a direct interpretation: If m_objSavedColor is not Empty, it means colors have been set and shouldn't be overridden
            ' unless it's the first time setting detail colors (in which case m_objSavedColor was just reset).
            If Not m_objSavedColor.Equals(Color.Empty) Then
                 Exit Sub
            End If
        End If

        Me.m_objSavedEditForeColor = p_objFColor
        Me.m_objSavedEditColor = p_objBColor
        Me.m_objSavedForeColor = p_objSFcolor
        Me.m_objSavedColor = p_objSBcolor

        ' Reset the flag after colors for detail are set, so next call to non-detail would re-evaluate m_blnSetForDetail
        ' The original code set m_blnSetForDetail = True at the start of a detail setWorkColors call and didn't reset it within the same call.
        ' It seems m_blnSetForDetail was more about ensuring m_objSavedColor was reset once when switching to detail context.
        ' The current logic for m_blnSetForDetail:
        ' - If p_blnDetail is true AND m_blnSetForDetail is false: set m_blnSetForDetail = true, m_objSavedColor = Color.Empty
        ' This ensures that the first time setWorkColors is called FOR DETAIL, the colors are indeed set.
        ' Subsequent calls for DETAIL would find m_blnSetForDetail as true, and m_objSavedColor would not be Empty, so they would exit.
        ' If setWorkColors is then called for MASTER (p_blnDetail = false), m_blnSetForDetail is not changed, and m_objSavedColor (if set for master) would prevent overwrite.
        ' This seems to replicate the "set once for master, set once for detail" behavior.

        CheckBankColor() ' Call to apply/check colors
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e) ' Call base painting

        If Me.DesignMode OrElse Not Me.mUseAutoFillLines OrElse Me.ColumnCount = 0 Then
            Exit Sub
        End If

        ' Determine the starting Y for empty lines
        Dim lastDataRowIndex As Integer = -1
        If Me.RowCount > 0 Then
            ' Find the last visible, non-new row
            For i As Integer = Me.RowCount - 1 To 0 Step -1
                If Me.Rows(i).Visible AndAlso Not Me.Rows(i).IsNewRow Then
                    lastDataRowIndex = i
                    Exit For
                End If
            Next
        End If

        Dim startY As Integer
        If lastDataRowIndex <> -1 Then
            Dim lastRowRect As Rectangle = Me.GetRowDisplayRectangle(lastDataRowIndex, False)
            startY = lastRowRect.Bottom
        Else
            startY = Me.ColumnHeadersHeight + 1 ' Start after header if no data rows
            If Me.ColumnHeadersVisible = False Then startY = 1 ' Or from top if no headers either
        End If

        ' If there are no rows at all (not even the "new row" placeholder if AllowUserToAddRows is true),
        ' and headers are not visible, startY might need to be adjusted to the very top of the client area.
        If Me.RowCount = 0 AndAlso Not Me.ColumnHeadersVisible Then
             startY = Me.ClientRectangle.Top
        ElseIf Me.RowCount = 0 AndAlso Me.ColumnHeadersVisible Then
             startY = Me.ColumnHeadersHeight + 1
        ElseIf Me.Rows.GetRowCount(DataGridViewElementStates.Visible) = 0 AndAlso Me.ColumnHeadersVisible Then
            ' This case handles when all rows are filtered out
            startY = Me.ColumnHeadersHeight + 1
        ElseIf Me.Rows.GetRowCount(DataGridViewElementStates.Visible) = 0 AndAlso Not Me.ColumnHeadersVisible Then
            startY = Me.ClientRectangle.Top
        End If


        Dim rowHeight As Integer = If(Me.RowTemplate.Height > 0, Me.RowTemplate.Height, 22)
        If rowHeight <= 0 Then rowHeight = 22 ' Ensure positive row height

        Dim clientRectBottom As Integer = Me.ClientRectangle.Height

        ' Adjust startY if it's already past the client rect bottom (e.g. grid is full)
        If startY >= clientRectBottom Then
            Exit Sub
        End If

        Dim rowHeadersWidth As Integer = If(Me.RowHeadersVisible, Me.RowHeadersWidth, 0)
        'Dim gridContentWidth As Integer = Me.ClientSize.Width - rowHeadersWidth
        ' Let's use display rectangle for content width calculation to be more precise
        Dim displayRect As Rectangle = Me.DisplayRectangle
        Dim gridContentWidth As Integer = displayRect.Width


        Dim currentY As Integer = startY

        ' TODO: Refine line positions and handle scroll states if drawing doesn't align perfectly.
        ' Consider HorizontalScrollOffset and VerticalScrollOffset if needed.
        ' The original LibXGrid also had specific pens (pDGray, pLGray, pWhite) for different lines,
        ' which are simplified to Me.GridColor here. This might need revisiting for pixel-perfect visuals.

        Using gridPen As New Pen(Me.GridColor), backBrush As New SolidBrush(Me.DefaultCellStyle.BackColor)
            Do While currentY < clientRectBottom
                ' Draw background for the empty row area
                ' The X coordinate should start after row headers
                e.Graphics.FillRectangle(backBrush, New Rectangle(displayRect.X + rowHeadersWidth, currentY, gridContentWidth - rowHeadersWidth, rowHeight))

                ' Draw horizontal line at the bottom of the "empty" row
                e.Graphics.DrawLine(gridPen, displayRect.X + rowHeadersWidth, currentY + rowHeight - 1, displayRect.X + gridContentWidth, currentY + rowHeight - 1)

                ' Draw vertical lines for columns
                Dim currentX As Integer = displayRect.X + rowHeadersWidth
                For Each col As DataGridViewColumn In Me.Columns
                    If col.Visible AndAlso col.Displayed Then
                        ' Ensure we don't draw outside the column's width or grid content area
                        Dim colRight As Integer = currentX + col.Width -1
                        If colRight > displayRect.X + gridContentWidth Then
                             colRight = displayRect.X + gridContentWidth -1
                        End If
                        If colRight >= currentX Then 'Only draw if there's space
                           e.Graphics.DrawLine(gridPen, colRight, currentY, colRight, currentY + rowHeight - 1)
                        End If
                        currentX += col.Width
                    End If
                Next
                ' Draw the far right vertical line of the grid body if not drawn by last column
                If currentX < displayRect.X + gridContentWidth Then
                     e.Graphics.DrawLine(gridPen, displayRect.X + gridContentWidth -1, currentY, displayRect.X + gridContentWidth-1, currentY + rowHeight -1)
                End If


                currentY += rowHeight
            Loop
        End Using
    End Sub

    ' TODO: Port methods from old LibXGrid.vb
End Class
' Helper class for IsNumericType (can be in a separate file or module)
Public Module Numerics
    Public Module NumericTypeHelper
        Private ReadOnly NumericTypes As HashSet(Of Type) = New HashSet(Of Type) From {
            GetType(Byte), GetType(SByte),
            GetType(UInt16), GetType(Int16),
            GetType(UInt32), GetType(Int32),
            GetType(UInt64), GetType(Int64),
            GetType(Single), GetType(Double), GetType(Decimal),
            GetType(System.Numerics.BigInteger)
        }

        Public Function IsNumericType(type As Type) As Boolean
            If type Is Nothing Then Return False
            Return NumericTypes.Contains(Nullable.GetUnderlyingType(type) OrElse type)
        End Function
    End Module
End Module

[end of LibXComponents/NewLibXGrid.vb]
