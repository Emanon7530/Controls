Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design

<Serializable()> _
Public Class LibXGrid
    Inherits DataGrid



    Private m_blnSetForDetail As Boolean
    Private m_blnIsDetail As Boolean
    Private m_objSavedColor As Color
    Private m_objSavedEditColor As Color
    Private m_objSavedEditForeColor As Color
    Private m_objSavedForeColor As Color

    ' Private m_blnManualChanged As Boolean
    Private m_intNewCurrentRow As Integer = -1
    Private m_intNewCurrentCol As Integer = -1
    Private m_intOldCurrentRow As Integer = 0
    Private m_intOldCurrentCol As Integer = 0
    Private m_blnOkToValidate As Boolean = True
    Private m_blnStartEdit As Boolean
    Private m_blnUseControlText As Boolean
    Private m_objCol As Object
    Private m_objTable As DataTable
    Private m_blnAutoSearch As Boolean
    Private m_blnRowSelect As Boolean
    Private m_strSearchText As String = ""
    Private m_intSelectedHeaderColumn As Integer = -1
    Private m_blnAutoSearching As Boolean
    Private m_blnAutoSearchingCancel As Boolean

    Public Event FooterChanging(ByVal sender As Object, ByVal e As XDataGridFooterChangingEventArgs)
    Private m_blnShowFooter As Boolean
    Private m_objFooterGrid As LibXGrid
    Private m_objFooterTable As DataTable
    Private m_objFooterExprFields As XDataGridFooterOperationsCollection

    Public Event CurrentRowChanged(ByVal sender As Object, ByVal e As XDataGridCurrentRowChangedEventArgs)
    Public Event HeaderClicked(ByVal e As XDataGridHeaderEventArgs)

    Public Event CellValidate(ByVal sender As Object, ByVal e As LibXGridCellValidateEventArgs)

    Public Event CellKeyPress(ByVal sender As Object, ByVal e As KeyEventArgs)
    Public Event rowHeaderDblClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event columnDblClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event GridScrolling(ByVal sender As Object, ByVal e As EventArgs)
    Public Event Refreshing(ByVal sender As Object, ByVal e As EventArgs)
    Public Event FooterColumnCreating(ByVal sender As Object, ByVal e As XDataGridFooterColumnCreatingEventArgs)
    Public Event FooterColumnGridStyleCreating(ByVal sender As Object, ByVal e As XDataGridFooterColumnGridStyleCreatingEventArgs)

    Private m_blnIsReadOnly As Boolean
    Private m_objCols As Collection
    Private m_objDataSource As LibXConnector
    Private m_blnIsFirstTimeScroolHVisible As Boolean = True
    <NonSerialized()> _
    Private m_intLastVisibleCol As Integer

    Private mUseAuotFillLines As Boolean = True

    Sub New()
        MyBase.New()

        If Not Me.DesignMode Then
            MyBase.ReadOnly = True

            m_intLastVisibleCol = -1
            AddHandler HeaderClicked, AddressOf InternalHeaderClick
            AddHandler Me.VertScrollBar.VisibleChanged, AddressOf verscrollVisibleChange
            'AddHandler Me.VertScrollBar.Scroll, AddressOf OnGridScroll
            AddHandler Me.Scroll, AddressOf OnGridScroll

        End If

    End Sub

    Public Property UseAutoFillLines() As Boolean
        Get
            Return mUseAuotFillLines
        End Get
        Set(ByVal Value As Boolean)
            mUseAuotFillLines = Value
        End Set
    End Property

    'Private Sub OnGridScroll(ByVal sender As Object, ByVal e As ScrollEventArgs)
    Private Sub OnGridScroll(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent GridScrolling(sender, e)
    End Sub

    Private Sub verscrollVisibleChange(ByVal sender As Object, ByVal e As EventArgs)
        Me.AdjustColumnSize()
    End Sub

    Public Property IsReadOnly() As Boolean
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
        End Set
    End Property


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

    '*<Editor(GetType(MyDataGridTableStylesCollectionEditor), GetType(UITypeEditor))> _
    <Editor(GetType(XDataGridTableStylesCollectionEditor), GetType(UITypeEditor))> _
    Public Shadows ReadOnly Property TableStyles() As GridTableStylesCollection
        Get
            Return MyBase.TableStyles
        End Get
    End Property


    Public ReadOnly Property footerOperations() As XDataGridFooterOperationsCollection
        Get
            If m_objFooterExprFields Is Nothing Then
                m_objFooterExprFields = New XDataGridFooterOperationsCollection
            End If
            Return m_objFooterExprFields
        End Get
    End Property

    Public ReadOnly Property lastRowIndex() As Integer
        Get
            Return m_intOldCurrentRow
        End Get
    End Property

    Public ReadOnly Property lastColIndex() As Integer
        Get
            Return m_intOldCurrentCol
        End Get
    End Property

    Public Function IsValidValue(ByVal row As Integer, ByVal col As Integer, ByVal p_strValue As String) As Boolean
        Dim objArgs As New LibXGridCellValidateEventArgs
        Dim objCol As Object


        Try
            If Not m_blnStartEdit Then
                Return True
            End If
            m_blnStartEdit = False

            If col < 0 Or row < 0 Then
                Return True
            End If
            objArgs.cell = col
            objArgs.row = row
            objArgs.value = p_strValue
            RaiseEvent cellValidate(Me, objArgs)
            If objArgs.hasErrors Then
                Return False
            End If

            If Me.TableStyles.Count = 0 Then
                Return True
            End If
            If Me.TableStyles(0).GridColumnStyles.Count = 0 Then
                Return True
            End If

            objCol = Me.TableStyles(0).GridColumnStyles(col)

            If TypeOf objCol Is XDataGridTextButtonColumn Then
                If Not objCol.doFind(row, col, objArgs.value) Then
                    Return False
                End If
            End If

            If Trim(objArgs.value) <> Trim(p_strValue) Then
                Me(row, col) = objArgs.value
            End If

            Return True

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function GetColNum(ByVal gCol As DataGridColumnStyle) As Integer
        Dim i As Integer
        i = 0
        Try
            If Me.TableStyles.Count = 0 Then
                Return -1
            End If
            Return Me.TableStyles(0).GridColumnStyles.IndexOf(gCol)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub SetValue(ByVal row As Integer, ByVal col As Integer, ByVal value As Object)
        Try
            If value Is Nothing Then
                value = ""
            End If
            Me(row, col) = Trim(value)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub SetValue(ByVal row As Integer, ByVal col As DataGridColumnStyle, ByVal value As Object)
        Try
            SetValue(row, Me.GetColNum(col), value)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub SetValue(ByVal row As Integer, ByVal colname As String, ByVal value As Object)
        Try

            SetValue(row, Me.getColByName(colname), value)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub SetValue(ByVal col As Integer, ByVal value As Object)
        Try

            SetValue(Me.CurrentRowIndex, col, value)

        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Sub

    Public Sub SetValue(ByVal col As DataGridColumnStyle, ByVal value As Object)
        Try
            SetValue(Me.CurrentRowIndex, Me.GetColNum(col), value)

        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Sub

    Public Sub SetValue(ByVal colname As String, ByVal value As Object)
        Try
            SetValue(Me.CurrentRowIndex, Me.getColByName(colname), value)

        Catch ex As Exception
            Log.Show(ex)
        End Try


    End Sub
    '''

    Public Function GetValue(ByVal col As DataGridColumnStyle) As String
        Try
            Dim oValue As Object = Me(Me.CurrentRowIndex, Me.GetColNum(col))

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function GetValue(ByVal row As Integer, ByVal col As DataGridColumnStyle) As String
        Try
            Dim oValue As Object = Me(row, Me.GetColNum(col))

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function


    Public Function GetValue(ByVal row As Integer, ByVal col As Integer) As String
        Try
            Dim oValue As Object = Me(row, col)
            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function SetValue(ByVal row As Integer, ByVal col As DataGridColumnStyle) As String
        Try
            Return GetValue(row, Me.GetColNum(col))

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function GetValue(ByVal row As Integer, ByVal colname As String) As String
        Try

            Return GetValue(row, Me.getColByName(colname))

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function SetValue(ByVal col As Integer) As String
        Try

            Return GetValue(Me.CurrentRowIndex, col)

        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Function

    Public Function SetValue(ByVal col As DataGridColumnStyle) As String
        Try
            Return GetValue(Me.CurrentRowIndex, Me.GetColNum(col))

        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Function

    Public Function GetValue(ByVal colname As String) As String
        Try
            Return GetValue(Me.CurrentRowIndex, Me.getColByName(colname))

        Catch ex As Exception
            Log.Show(ex)
        End Try


    End Function



    '''


    Public Function getColByName(ByVal p_strName As String) As Integer
        Dim objCol As Object
        Dim objColT As DataGridColumnStyle
        Dim i As Integer
        i = 0
        Try
            If Me.TableStyles.Count = 0 Then
                Return -1
            End If
            Try
                objColT = Me.TableStyles(0).GridColumnStyles.Item(p_strName)
            Catch
            End Try
            If objColT Is Nothing Then
                Return -1
            End If
            Return Me.TableStyles(0).GridColumnStyles.IndexOf(objColT)

            Return -1

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function getColByName(ByVal p_strName As String, ByRef p_objCol As Object) As Integer
        Dim objCol As Object
        Dim i As Integer
        Try
            i = 0
            If Me.TableStyles.Count = 0 Then
                Return -1
            End If
            If Me.TableStyles(0).GridColumnStyles.Contains(p_strName) Then
                p_objCol = Me.TableStyles(0).GridColumnStyles.Item(p_strName)
                Return Me.TableStyles(0).GridColumnStyles.IndexOf(p_objCol)
            End If

            Return -1

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public ReadOnly Property currentRowNum() As Integer
        Get
            Return m_intOldCurrentRow
        End Get
    End Property

    Protected Overrides Sub OnCurrentCellChanged(ByVal e As System.EventArgs)
        Dim strValue As String
        Try

            If Me.FullRowSelect Then
                If CurrentRowIndex >= 0 Then
                    Me.Select(CurrentRowIndex)
                End If
            End If

            If lastRowIndex <> CurrentRowIndex Then
                If m_blnOkToValidate Then
                    '*--> OnCurrentRowChanged(New XDataGridCurrentRowChangedEventArgs(Me.CurrentCell.ColumnNumber, Me.CurrentRowIndex, m_intOldCurrentRow, m_intOldCurrentCol))
                    OnCurrentRowChanged(New XDataGridCurrentRowChangedEventArgs(Me.CurrentCell.ColumnNumber, Me.CurrentCell.RowNumber, m_intOldCurrentRow, m_intOldCurrentCol))
                End If
            End If

            m_intNewCurrentRow = Me.CurrentCell.RowNumber
            m_intNewCurrentCol = Me.CurrentCell.ColumnNumber

            If m_blnIsReadOnly Then
                MyBase.OnCurrentCellChanged(e)
                m_intOldCurrentRow = m_intNewCurrentRow
                m_intOldCurrentCol = m_intNewCurrentCol
                Return
            End If

            If m_blnOkToValidate Then
                MyBase.OnCurrentCellChanged(e)
            End If

            Try
                If Not m_blnUseControlText Then
                    strValue = Me(m_intOldCurrentRow, m_intOldCurrentCol).ToString()
                Else
                    strValue = m_objCol.textbox.text
                End If
            Catch
            End Try

            If m_blnOkToValidate And Not strValue Is Nothing Then

                If Not IsValidValue(m_intOldCurrentRow, m_intOldCurrentCol, strValue) Then
                    m_blnOkToValidate = False
                    Me.CurrentCell = New DataGridCell(m_intOldCurrentRow, m_intOldCurrentCol)
                    m_blnOkToValidate = True
                End If
            End If
            m_intOldCurrentRow = m_intNewCurrentRow
            m_intOldCurrentCol = m_intNewCurrentCol

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Class LibXGridCellValidateEventArgs
        Public value As String
        Public cell As Integer
        Public row As Integer
        Public hasErrors As Boolean
    End Class

    Public Class XDataGridFooterColumnCreatingEventArgs
        Public ColName As String
        Public ColType As Type
    End Class

    Public Class XDataGridFooterColumnGridStyleCreatingEventArgs
        Public ColName As String
        Public ColStyle As DataGridColumnStyle
    End Class


    Protected Overloads Overrides Sub ColumnStartedEditing(ByVal editingControl As System.Windows.Forms.Control)
        MyBase.ColumnStartedEditing(editingControl)
        If Not m_blnIsReadOnly Then
            m_blnStartEdit = True
        End If
    End Sub

    Protected Overloads Overrides Sub ColumnStartedEditing(ByVal bounds As System.Drawing.Rectangle)
        MyBase.ColumnStartedEditing(bounds)
        If Not m_blnIsReadOnly Then
            m_blnStartEdit = True
        End If

    End Sub

    Protected Overrides Sub OnLeave(ByVal e As System.EventArgs)
        MyBase.OnLeave(e)
        If m_blnStartEdit Then
            If Me.TableStyles.Count > 0 Then
                If Me.TableStyles(0).GridColumnStyles.Count > 0 Then
                    If Me.CurrentCell.ColumnNumber >= 0 And Me.CurrentCell.RowNumber >= 0 Then
                    End If
                End If
            End If
        End If
    End Sub



    Protected Overrides Sub OnDataSourceChanged(ByVal e As System.EventArgs)
        Dim objDts As Object
        Dim objGs As DataGridColumnStyle
        Try
            MyBase.OnDataSourceChanged(e)

            If Me.DesignMode Then
                Exit Sub
            End If
            m_objTable = Nothing


            If m_objTable Is Nothing Then

                objDts = Me.DataSource
                If TypeOf objDts Is LibXConnector Then
                    m_objDataSource = objDts
                    objDts = CType(m_objDataSource, LibXConnector).DataSource
                Else
                    If TypeOf objDts Is DataSet Then
                        Dim ds As DataSet = objDts
                        If Not ds.ExtendedProperties Is Nothing AndAlso ds.ExtendedProperties.Count > 0 Then
                            m_objDataSource = ds.ExtendedProperties.Item("xcone")
                        End If
                    End If
                End If

                If Trim(Me.DataMember) = "" Then
                    '*--> Puede ser un datatable, o un da
                    If TypeOf objDts Is DataView Then
                        m_objTable = objDts.table
                    Else
                        m_objTable = objDts
                    End If
                Else
                    m_objTable = objDts.tables(Me.DataMember)
                End If

                If TypeOf objDts Is LibXConnector Then
                    Dim objNS As LibXConnector
                    objNS = objDts
                    m_objDataSource = objNS

                    AddHandler objNS.AfterRowChange, AddressOf onAfterDsRowChange
                    AddHandler objNS.ChangeState, AddressOf onDsChangeState


                    'If Trim(Me.DataMember).ToUpper = Trim(objDts.UpdateTableName).ToUpper Then
                    '    objDts.hasMasterGrid = True
                    'End If
                    'If Trim(Me.DataMember).ToUpper = Trim(objDts.UpdateDetailTableName).ToUpper Then
                    '    objDts.hasDetailGrid = True
                    'End If
                End If

                If Not TypeOf objDts Is LibXConnector And Not Me.m_blnIsReadOnly Then
                    Me.ReadOnly = False
                End If

                If Not m_objTable Is Nothing Then

                    RemoveHandler m_objTable.RowChanged, AddressOf OnRowChanged
                    AddHandler m_objTable.RowChanged, AddressOf OnRowChanged


                End If

                If m_blnShowFooter And Not m_objTable Is Nothing Then
                    RemoveHandler m_objTable.ColumnChanged, AddressOf onColChanged
                    AddHandler m_objTable.ColumnChanged, AddressOf onColChanged

                    If Not m_objFooterGrid Is Nothing Then
                        Me.Parent.Controls.Remove(m_objFooterGrid)
                    End If

                    m_objFooterGrid = New LibXGrid
                    m_objFooterGrid.UseAutoFillLines = False
                    m_objFooterGrid.Name = "NetFooterGrid"
                    m_objFooterGrid.Visible = False
                    m_objFooterGrid.Left = Me.Left
                    m_objFooterGrid.Width = Me.Width
                    m_objFooterGrid.Top = Me.Top + Me.Height + 1
                    m_objFooterGrid.ColumnHeadersVisible = False
                    m_objFooterGrid.RowHeadersVisible = Me.RowHeadersVisible
                    m_objFooterGrid.RowHeaderWidth = Me.RowHeaderWidth
                    m_objFooterGrid.CaptionVisible = False
                    m_objFooterGrid.PreferredRowHeight = Me.PreferredRowHeight + 8
                    m_objFooterGrid.PreferredColumnWidth = Me.PreferredColumnWidth
                    m_objFooterGrid.ForeColor = System.Drawing.Color.Blue

                    m_objFooterTable = New DataTable
                    Dim objCol As DataColumn
                    Dim oCols As XDataGridFooterColumnCreatingEventArgs
                    For Each objCol In m_objTable.Columns
                        oCols = New XDataGridFooterColumnCreatingEventArgs
                        oCols.ColName = objCol.ColumnName
                        oCols.ColType = GetType(String) 'objCol.DataType
                        RaiseEvent FooterColumnCreating(Me, oCols)
                        m_objFooterTable.Columns.Add(oCols.ColName, oCols.ColType)
                    Next
                    m_objFooterTable.DefaultView.AllowEdit = False
                    m_objFooterTable.DefaultView.AllowNew = False
                    m_objFooterTable.TableName = "FooterTable"
                    m_objFooterGrid.DataSource = m_objFooterTable
                    Dim objRow As DataRow = m_objFooterTable.NewRow
                    m_objFooterTable.Rows.Add(objRow)

                    Dim objGst As DataGridColumnStyle
                    Dim objSt As New DataGridTableStyle
                    objSt.MappingName = m_objFooterTable.TableName
                    Dim objGstL As DataGridTextBoxColumn
                    If Me.TableStyles.Count > 0 Then

                        Dim oColst As XDataGridFooterColumnGridStyleCreatingEventArgs
                        For Each objGst In Me.TableStyles(0).GridColumnStyles
                            oColst = New XDataGridFooterColumnGridStyleCreatingEventArgs

                            objGstL = New DataGridTextBoxColumn
                            objGstL.MappingName = objGst.MappingName
                            If TypeOf objGst Is DataGridTextBoxColumn Then
                                objGstL.Format = CType(objGst, DataGridTextBoxColumn).Format
                            End If
                            objGstL.Width = objGst.Width
                            objGstL.Alignment = objGst.Alignment
                            objGstL.NullText = ""

                            oColst.ColName = objGstL.MappingName
                            oColst.ColStyle = objGstL
                            RaiseEvent FooterColumnGridStyleCreating(Me, oColst)

                            objSt.GridColumnStyles.Add(objGstL)
                        Next
                    Else
                        Dim oColst As XDataGridFooterColumnGridStyleCreatingEventArgs


                        For Each objCol In m_objTable.Columns
                            oColst = New XDataGridFooterColumnGridStyleCreatingEventArgs

                            If objCol.ColumnMapping <> MappingType.Hidden Then
                                objGstL = New DataGridTextBoxColumn
                                objGstL.MappingName = objCol.ColumnName
                                objGstL.NullText = ""

                                oColst.ColName = objGstL.MappingName
                                oColst.ColStyle = objGstL
                                RaiseEvent FooterColumnGridStyleCreating(Me, oColst)

                                objSt.GridColumnStyles.Add(objGstL)
                            End If
                        Next
                    End If
                    m_objFooterGrid.Height = m_objFooterGrid.PreferredRowHeight - 3
                    m_objFooterGrid.BorderStyle = BorderStyle.FixedSingle
                    m_objFooterGrid.TableStyles.Add(objSt)
                    m_objFooterGrid.Visible = True
                    Me.Parent.Controls.Add(m_objFooterGrid)

                    m_objFooterGrid.AdjustColumnSize(True)

                    'Me.FindForm.Controls.Add(m_objFooterGrid)
                End If '/If m_blnShowFooter Then


            End If

            If Me.TableStyles.Count > 0 Then
                For Each objGs In Me.TableStyles(0).GridColumnStyles
                    objGs.NullText = ""
                Next
            End If

            Me.AdjustColumnSize()

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub


    Private Sub onAfterDsRowChange(ByVal sender As Object, ByVal e As XRowChangeEventArgs)
        If Me.m_blnShowFooter And Not Me.m_objFooterGrid Is Nothing Then
            Me.refreshFooter()
        End If
    End Sub

    Private Sub onDsChangeState(ByVal sender As Object, ByVal e As XChangeStateEventArgs)
        If Me.m_blnShowFooter And Not Me.m_objFooterGrid Is Nothing Then
            Me.refreshFooter()
        End If
    End Sub

    Public Sub enableEdit()
        Try
            If Not m_objTable Is Nothing Then
                If Me.ReadOnly Then
                    If Not m_blnIsReadOnly Then
                        Me.ReadOnly = False
                    End If
                End If
                Me.m_objTable.DefaultView.AllowEdit = True
                Me.m_objTable.DefaultView.AllowDelete = True
                Me.m_objTable.DefaultView.AllowNew = True
            End If
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub disableEdit()
        Try
            If Not m_objTable Is Nothing Then
                Me.m_objTable.DefaultView.AllowEdit = False
                Me.m_objTable.DefaultView.AllowDelete = False
                Me.m_objTable.DefaultView.AllowNew = False
            End If
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub ClearFooter()
        Me.onFooterChanging(True)
    End Sub

    '* Para que el resultado en el footer se vea al consultar los registros
    '  debe usar el UseRowRetrieveEvent=true
    Private Sub onFooterChanging(Optional ByVal clearGrid As Boolean = False)
        Dim objCol As XDataGridFooterExpField
        Dim objResult As XDataGridFooterChangingEventArgs
        Dim objRow As DataRow

        Try
            If Me.m_objTable Is Nothing Then
                Exit Sub
            End If
            For Each objCol In Me.footerOperations
                objResult = New XDataGridFooterChangingEventArgs
                objResult.ColumnName = objCol.ColumnName
                objResult.ColumnResult = m_objTable.Compute(objCol.Expression, objCol.Filter)
                RaiseEvent FooterChanging(Me, objResult)
                objRow = Me.m_objFooterTable.Rows(0)
                If clearGrid Then
                    objRow(objResult.ColumnName) = DBNull.Value
                Else
                    objRow(objResult.ColumnName) = objResult.ColumnResult
                End If
            Next

            Me.m_objFooterGrid.Refresh()

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Sub onColChanged(ByVal sender As Object, ByVal e As System.Data.DataColumnChangeEventArgs)
        Try
            'Dim intCant As Integer
            'Dim decFactor As Double
            'Dim decRes As Decimal
            'Dim decTotal As Decimal
            'Dim objRow As DataRow

            onFooterChanging()

            'If e.Column.ColumnName = "can" Then
            '    objRow = e.Row
            '    intCant = 0
            '    If Not CAM.Util.isNulo(objRow!can) Then
            '        intCant = CInt(objRow!can)
            '    End If

            '    decFactor = CDbl(objRow!nom)
            '    If m_obParams.isRetiro Then
            '        intCant = intCant * -1
            '    End If
            '    decRes = intCant * decFactor
            '    If m_obParams.isRetiro Then
            '        If Not verificarCantidad(intCant, decFactor, m_obParams) Then
            '            CAM.App.Errores.show(4031)
            '            decRes = 0
            '            objRow!can = 0
            '        End If
            '    End If
            '    objRow!res = decRes
            '    suma()

            'End If
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub


    Public Property FullRowSelect() As Boolean
        Get
            Return m_blnRowSelect
        End Get
        Set(ByVal Value As Boolean)
            m_blnRowSelect = Value
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

    Public Sub refreshFooter()
        onFooterChanging()
    End Sub

    Private Sub OnRowChanged(ByVal sender As Object, ByVal e As DataRowChangeEventArgs)
        Dim objcol As Object
        Dim objcolT As XDataGridTextButtonColumn
        Dim strKey As String
        Dim objValue As Object
        Dim intCol As Integer
        Try
            If e.Action = DataRowAction.Add Then

                If m_objCols Is Nothing Then
                    m_objCols = New Collection
                    If Me.TableStyles.Count > 0 Then
                        For Each objcol In Me.TableStyles(0).GridColumnStyles
                            If TypeOf objcol Is XDataGridTextButtonColumn Then
                                If objcol.hasLookup Then
                                    strKey = objcol.MappingName
                                    If Trim(strKey) = "" Then
                                        strKey = objcol.name
                                    End If
                                    m_objCols.Add(objcol, strKey)
                                End If
                            End If
                        Next
                    End If
                End If

                For Each objcolT In m_objCols
                    If Trim(objcolT.MappingName) <> "" Then
                        Try
                            objValue = e.Row(objcolT.MappingName)
                            intCol = Me.TableStyles(0).GridColumnStyles.IndexOf(objcolT)
                            objcolT.doFind(e.Row, intCol, objValue)
                        Catch
                        End Try
                    End If
                Next

                If m_blnShowFooter Then
                    Me.onFooterChanging()
                End If

            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderColumn() As Integer
        Get
            Return m_intSelectedHeaderColumn
        End Get
    End Property

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderFieldCaption() As String
        Get
            If Not Me.SelectedHeaderColumn < 0 Then
                Return Me.TableStyles(Me.CurrentView.Table.TableName).GridColumnStyles(Me.SelectedHeaderColumn).HeaderText
            End If
        End Get
    End Property

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderField() As String
        Get
            Try
                If Not Me.SelectedHeaderColumn < 0 And Me.TableStyles.Count > 0 Then
                    Return Me.TableStyles(Me.CurrentView.Table.TableName).GridColumnStyles(Me.SelectedHeaderColumn).MappingName
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property

    Private Sub InternalHeaderClick(ByVal e As XDataGridHeaderEventArgs)
        Try
            If (m_intSelectedHeaderColumn <> e.column) Then
                m_intSelectedHeaderColumn = e.column
                m_strSearchText = ""
                GenerateFilterExpression()
            End If
            Me.Focus()
            Me.CurrentCell = New DataGridCell(-1, -1)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    <Browsable(False)> _
    Public ReadOnly Property CurrentView() As DataView
        Get
            Try

                Dim ds As Object = Me.DataSource
                If TypeOf ds Is LibXConnector Then
                    ds = CType(DataSource, LibXConnector).DataSource
                End If

                Dim dv As DataView
                If (TypeOf ds Is DataSet) And (Me.DataMember.Length > 0) Then
                    '// Turn off AutoSearch if DataSource is DataSet
                    'Me.AutoSearch = False
                    dv = ds.Tables(Me.DataMember).DefaultView
                ElseIf TypeOf Me.DataSource Is DataTable Then
                    dv = ds.DefaultView
                ElseIf TypeOf Me.DataSource Is DataView Then
                    dv = ds
                End If

                Return dv
            Catch ex As Exception
                Log.Show(ex)
            End Try
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property footerGrid() As DataGrid
        Get
            Return m_objFooterGrid
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property footerTable() As DataTable
        Get
            Return m_objFooterTable
        End Get
    End Property

    Private Function IsSearchMode() As Boolean
        If Me.AutoSearch And Me.SelectedHeaderColumn > -1 Then
            Return True
        End If
        Return False
    End Function

    Public Function GetCurrentRowView() As DataRowView
        Dim objForm As Form
        Try
            objForm = Me.FindForm

            Dim cm As CurrencyManager = objForm.BindingContext(Me.DataSource, Me.DataMember)

            Return CType(cm.Current, DataRowView)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function getCurrentGridView() As DataView
        Dim objView As DataView = Me.CurrentView
        Dim objForm As Form
        Try
            objForm = Me.FindForm
            Dim cm As CurrencyManager = objForm.BindingContext(Me.DataSource, Me.DataMember)
            objView = CType(cm.List, DataView)
            Return objView
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub setFilter(ByVal p_strFilter As String)
        Dim objView As DataView = Me.CurrentView
        Dim objForm As Form
        Try
            objForm = Me.FindForm
            Dim cm As CurrencyManager = objForm.BindingContext(Me.DataSource, Me.DataMember)
            objView = CType(cm.List, DataView)
            If Not objView Is Nothing Then
                objView.RowFilter = p_strFilter
            End If
        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Sub

    Private Sub Search(ByVal p_strSearchText As String)
        Dim objView As DataView = Me.CurrentView
        Try
            If Not objView Is Nothing Then
                Dim strFilterExpr As String = ""
                If p_strSearchText <> "" Then
                    strFilterExpr = String.Format(GenerateFilterExpression(), p_strSearchText)
                End If
                DisplaySearchText()
                objView.RowFilter = strFilterExpr
            End If
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Sub OnCurrentRowChanged(ByVal e As XDataGridCurrentRowChangedEventArgs)
        RaiseEvent CurrentRowChanged(Me, e)
    End Sub


    Private Property searchText()
        Get
            Return m_strSearchText
        End Get
        Set(ByVal Value)
            m_strSearchText = Value
            If IsSearchMode() Then
                Search(m_strSearchText)
            End If
        End Set
    End Property

    Private Sub DisplaySearchText()
        Dim strText As String
        If Me.searchText.Length = 0 Then
            CaptionText = ""
        Else
            strText = Me.SelectedHeaderFieldCaption
            If Trim(strText) = "" Then
                strText = Me.SelectedHeaderField
            End If
            CaptionText = "[" & strText & "]=" & Me.searchText
        End If
    End Sub

    Public Sub ExecuteSort(ByVal p_strColumns As String)
        Dim objView As DataView = Me.CurrentView
        Dim objForm As Form
        Try
            objForm = Me.FindForm
            Dim cm As CurrencyManager = objForm.BindingContext(Me.DataSource, Me.DataMember)
            objView = CType(cm.List, DataView)
            If Not objView Is Nothing Then
                objView.Sort = p_strColumns
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Sub

    Private Function GenerateFilterExpression() As String
        Dim objTable As DataTable = Me.CurrentView.Table
        Dim strExpr As String = ""

        Try
            '//MessageBox.Show(dt.Columns[this.SelectedHeaderField].DataType.ToString());
            If Trim(SelectedHeaderField) = "" Then
                Exit Function
            End If
            Select Case objTable.Columns(Me.SelectedHeaderField).DataType.ToString()
                Case "System.String"
                    strExpr = String.Format("{0} Like {1}", Me.SelectedHeaderField, "'{0}%'")
                Case "System.Byte"
                Case "System.Decimal"
                Case "System.Double"
                Case "System.Int16"
                Case "System.Int32"
                Case "System.Int64"
                    strExpr = String.Format("{0} >= {1}", Me.SelectedHeaderField, "{0}")
                Case "System.DateTime"
                    strExpr = String.Format("{0} >= {1}", Me.SelectedHeaderField, "#{0}#")
            End Select
            Return strExpr

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Protected Overrides Sub OnKeyPress(ByVal kpe As System.Windows.Forms.KeyPressEventArgs)
        Try
            If IsSearchMode() Then

                If m_blnAutoSearchingCancel Then
                    Me.m_blnAutoSearching = False
                    m_blnAutoSearchingCancel = False
                End If

                If Char.IsLetterOrDigit(kpe.KeyChar) Then
                    '// Handle datetime search
                    If TypeOf SelectedHeaderFieldType Is DateTime Then
                        Dim dt As DateTime = DateTime.Today
                        'if (DateTimeDialog.ShowDateTimeDialog(ref dt)==DialogResult.OK)
                        '	this.SearchText = dt.ToString("M/d/yyyy");

                    ElseIf IsValidChar(kpe.KeyChar) Then
                        Me.searchText += kpe.KeyChar
                        m_blnAutoSearching = True
                    End If

                Else
                    Select Case Convert.ToByte(kpe.KeyChar)
                        '// [Backspace] key
                    Case 8
                            If searchText.Length > 0 Then
                                If (TypeOf SelectedHeaderFieldType Is DateTime) Then
                                    Me.searchText = ""
                                Else
                                    Me.searchText = Me.searchText.Substring(0, Me.searchText.Length - 1)
                                End If
                            End If
                            '// [Escape] key
                        Case 27
                            Me.searchText = ""
                            m_blnAutoSearchingCancel = True
                    End Select
                End If
            End If

            'MyBase.OnKeyPress(kpe)

        Catch ex As Exception
            Log.Show(ex)
        Finally
            MyBase.OnKeyPress(kpe)
        End Try
    End Sub

    <Browsable(False)> _
    Private ReadOnly Property SelectedHeaderFieldType() As Type
        Get
            Return Me.CurrentView.Table.Columns(Me.SelectedHeaderField).DataType
        End Get
    End Property

    Private Function IsValidChar(ByVal c As Char) As Boolean
        Dim validChar As Boolean = False

        Select Case SelectedHeaderFieldType.ToString()
            Case "System.String"
                validChar = Char.IsLetterOrDigit(c)
            Case "System.Byte"
            Case "System.Decimal"
            Case "System.Double"
            Case "System.Int16"
            Case "System.Int32"
            Case "System.Int64"
                validChar = Char.IsDigit(c)
        End Select
        Return validChar
    End Function

    Private Sub OnHeaderClick(ByVal e As XDataGridHeaderEventArgs)
        RaiseEvent HeaderClicked(e)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim hi As HitTestInfo = Me.HitTest(e.X, e.Y)
            If hi.Row = -1 And hi.Column > -1 Then
                OnHeaderClick(New XDataGridHeaderEventArgs(hi.Column, hi.Row))
            End If
        Catch ex As Exception
            Log.Show(ex)
        Finally
            MyBase.OnMouseUp(e)
        End Try
    End Sub


    Protected Overrides Sub OnVisibleChanged(ByVal e As System.EventArgs)
        Try
            '-->Me.CurrentRowIndex falla cargando cuando datagrid está bindiado a un 
            '   untypedataset. por eso puse este try
            Dim rowindex As Integer
            Try
                rowindex = Me.CurrentRowIndex
            Catch ex As Exception
                rowindex = -1
            End Try

            If Me.FullRowSelect And rowindex > -1 Then
                Me.Select(Me.CurrentRowIndex)
            End If

        Catch ex As Exception
            Log.Show(ex)
        Finally
            MyBase.OnVisibleChanged(e)
        End Try
    End Sub

    Public Property showFooterBar() As Boolean
        Get
            Return m_blnShowFooter
        End Get
        Set(ByVal Value As Boolean)
            m_blnShowFooter = Value
        End Set
    End Property


    <Browsable(False)> _
    Public ReadOnly Property verticalScrollBarVisible() As Boolean
        Get
            Return Me.VertScrollBar.Visible
        End Get
    End Property

    Public ReadOnly Property horizontalScrollBarVisible() As Boolean
        Get
            Return Me.HorizScrollBar.Visible
        End Get
    End Property

    Public Sub AdjustColumnSizeEx(ByVal p_blnInForce As Boolean, ByVal checkStringWidh As Boolean)
        Try

            If Me.DesignMode Then
                Exit Sub
            End If

            If Me.TableStyles.Count = 0 Then                Exit Sub
            End If            If Me.TableStyles(0).GridColumnStyles.Count = 0 Then                Exit Sub
            End If            'If Not p_blnIsHor And Not m_blnIsFirstTimeScroolHVisible Then            '    Exit Sub
            'End If            If Me.horizontalScrollBarVisible And m_blnIsFirstTimeScroolHVisible And p_blnInForce = False Then                Exit Sub            End If            Dim numCols As Integer
            Dim dataTable1 As DataTable

            If Me.DataSource Is Nothing Then
                Exit Sub
            End If
            Dim ds As Object = Me.DataSource
            If TypeOf ds Is LibXConnector Then
                ds = CType(DataSource, LibXConnector).DataSource
            End If

            If (TypeOf ds Is DataSet) Then
                dataTable1 = ds.Tables(Me.DataMember)
            Else
                If TypeOf ds Is DataView Then
                    dataTable1 = ds.table
                Else
                    dataTable1 = CType(ds, DataTable)
                End If
            End If
            If dataTable1 Is Nothing Then
                Exit Sub
            End If
            '-->dataTable1 = Me.NetDataSource1.Tables(0)  ' CType(Me.LibXGrid1.DataSource, DataTable)
            '-->numCols = dataTable1.Columns.Count 
            numCols = Me.TableStyles(0).GridColumnStyles.Count

            Dim scrollBarWidth As Integer
            Dim intLastVisibleCol As Integer

            scrollBarWidth = IIf(Me.verticalScrollBarVisible, SystemInformation.VerticalScrollBarWidth, 0)

            'the fudge -4 is for the grid borders
            Dim targetWidth As Integer
            targetWidth = Me.ClientSize.Width - scrollBarWidth - 4

            Dim runningWidthUsed As Integer
            runningWidthUsed = Me.TableStyles(0).RowHeaderWidth
            Dim i As Integer
            'i = 0

            ''--> Buscar la ultima visible
            'For i = 0 To numCols - 1
            '    If Trim(Me.TableStyles(0).GridColumnStyles(i).MappingName) <> "" Then
            '        If dataTable1.Columns.Contains(Me.TableStyles(0).GridColumnStyles(i).MappingName) Then
            '            intLastVisibleCol = i
            '        End If
            '    End If
            'Next

            i = 0


            If m_intLastVisibleCol = -1 Then
                For i = 0 To numCols - 1
                    '--> Si la columna del gridcol style no esta dentro de la de la tabla
                    '    es como si esa col estuviera visible false
                    If Trim(Me.TableStyles(0).GridColumnStyles(i).MappingName) <> "" Then
                        If Me.TableStyles(0).GridColumnStyles(i).Width > 0 Then
                            If dataTable1.Columns.Contains(Me.TableStyles(0).GridColumnStyles(i).MappingName) Then
                                m_intLastVisibleCol = i
                            End If
                        End If
                    End If
                Next
            End If

            intLastVisibleCol = m_intLastVisibleCol
            i = 0
            'Do While (i < (numCols - 1))
            Do While (i < intLastVisibleCol)
                '--> Si la columna del gridcol style no esta dentro de la de la tabla
                '    es como si esa col estuviera visible false
                If Trim(Me.TableStyles(0).GridColumnStyles(i).MappingName) <> "" Then
                    If Me.TableStyles(0).GridColumnStyles(i).Width > 0 Then
                        If dataTable1.Columns.Contains(Me.TableStyles(0).GridColumnStyles(i).MappingName) Then

                            If checkStringWidh Then
                                '-->Este codigo es para tratar en los lookups de ajustar las columnas
                                '   string a un size mas real que el default
                                If i > 0 Then
                                    If dataTable1.Columns(Me.TableStyles(0).GridColumnStyles(i).MappingName).DataType.Equals(GetType(System.String)) Then
                                        If Me.TableStyles(0).GridColumnStyles(i).Width = Me.TableStyles(0).PreferredColumnWidth Then
                                            Me.TableStyles(0).GridColumnStyles(i).Width = 125
                                        End If
                                    End If
                                End If
                            End If

                            runningWidthUsed = (runningWidthUsed + Me.TableStyles(0).GridColumnStyles(i).Width)
                            '-->intLastVisibleCol = i
                        End If
                    End If
                End If
                i = (i + 1)
            Loop

            'If intLastVisibleCol < (Me.TableStyles(0).GridColumnStyles.Count - 2) Then
            'intLastVisibleCol = intLastVisibleCol - 1
            'End If

            If intLastVisibleCol = -1 Then
                Exit Sub
            End If

            If (runningWidthUsed < targetWidth) Then
                Me.TableStyles(0).GridColumnStyles((intLastVisibleCol)).Width = (targetWidth - runningWidthUsed)
            End If

            m_blnIsFirstTimeScroolHVisible = False


            If Me.m_blnShowFooter Then
                If Not m_objFooterGrid Is Nothing Then

                    m_objFooterGrid.Width = Me.Width - scrollBarWidth

                    Dim objGst As DataGridColumnStyle
                    Dim objSt As New DataGridTableStyle
                    Dim objGstL As DataGridTextBoxColumn

                    If Me.TableStyles.Count > 0 Then
                        For Each objGst In Me.TableStyles(0).GridColumnStyles
                            objGstL = m_objFooterGrid.TableStyles(0).GridColumnStyles(objGst.MappingName)
                            objGstL.Width = objGst.Width
                        Next
                    End If
                    m_objFooterGrid.AdjustColumnSize(True)
                End If
            End If '-->If Me.m_blnShowFooter The

        Catch ex As Exception
            Log.Show(ex)
        Finally

        End Try
    End Sub

    ' Se incluyó la posibilidad de que se autoajuste la ultima columna en el
    ' ClientArea
    '------------------------------------------------------
    Public Sub AdjustColumnSize(Optional ByVal p_blnInForce As Boolean = False)
        AdjustColumnSizeEx(p_blnInForce, False)
    End Sub

    'Protected Overrides Sub OnKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
    '    MyBase.OnKeyUp(e)

    '    If Me.m_blnAutoSearching Then
    '        If e.KeyCode = Keys.Escape Then
    '            m_blnAutoSearching = False
    '        End If
    '    End If

    'End Sub

#Region "Helper Classes"
    Public Class XDataGridHeaderEventArgs
        Inherits EventArgs

        Private m_intColumn As Integer = -1
        Private m_intRow As Integer = -1

        Sub New(ByVal p_intCol As Integer, ByVal p_intRow As Integer)
            m_intColumn = p_intCol
            m_intRow = p_intRow
        End Sub

        Public Overloads Overrides Function ToString() As String
            Return String.Format("Column = {0}, Row = {1}", m_intColumn, m_intRow)
        End Function

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



#End Region

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If msg.WParam.ToInt32 = Keys.Enter Then
                SendKeys.Send("{Tab}")
                Return True
            End If

            Return MyBase.ProcessCmdKey(msg, keyData)
        Catch

        Finally
            If Not Me.isAutoSearching Then
                Dim objArgs As New KeyEventArgs(keyData)
                RaiseEvent CellKeyPress(Me, objArgs)
            End If
        End Try
    End Function

    Protected Overrides Sub OnDoubleClick(ByVal e As System.EventArgs)
        MyBase.OnDoubleClick(e)

        Dim pt As System.Drawing.Point = Me.PointToClient(Cursor.Position)

        Dim hti As DataGrid.HitTestInfo = Me.HitTest(pt)

        If (hti.Type = DataGrid.HitTestType.RowHeader) Then
            RaiseEvent rowHeaderDblClick(Me, e)
        End If
        If (hti.Type = DataGrid.HitTestType.Cell) Then
            RaiseEvent columnDblClick(Me, e)
        End If

    End Sub

    Protected Overrides Sub OnEnabledChanged(ByVal e As System.EventArgs)
        MyBase.OnEnabledChanged(e)
        Me.CheckBankColor()
    End Sub

    'Public Sub enforceEnableChanged(ByVal p_blnEnabled As Boolean)
    '    Dim blnChange As Boolean = True
    '    If Me.Enabled <> p_blnEnabled Then
    '        blnChange = False '*--> No tengo que llamarlo porque el evento se va a disparar
    '    End If

    '    Me.Enabled = p_blnEnabled
    '    If blnChange Then
    '        Me.checkBankColor()
    '    End If
    'End Sub

    Public Sub setIsDetail(ByVal p_blnIsDetail As Boolean)
        m_blnIsDetail = p_blnIsDetail
    End Sub

    Public Sub setWorkColors(ByVal p_objBColor As Color, ByVal p_objFColor As Color, ByVal p_objSBcolor As Color, ByVal p_objSFcolor As Color, ByVal p_blnDetail As Boolean)
        If p_blnDetail And Not m_blnSetForDetail Then
            m_blnSetForDetail = True
            m_objSavedColor = Color.Empty
        End If

        If Not m_objSavedColor.Equals(Color.Empty) Then
            Exit Sub
        End If

        Me.m_objSavedEditForeColor = p_objFColor
        Me.m_objSavedEditColor = p_objBColor
        Me.m_objSavedForeColor = p_objSFcolor
        Me.m_objSavedColor = p_objSBcolor
        Me.CheckBankColor()
    End Sub


    Public Sub CheckBankColor()

        'If Not Me.m_objDataSource Is Nothing Then
        '    Dim blnEditing As Boolean
        '    Dim objEditColor As Color

        '    If m_objSavedColor.Equals(Color.Empty) Then
        '        Exit Sub
        '    End If

        '    blnEditing = Me.m_objDataSource.isEditing

        '    If m_blnIsDetail Then
        '        blnEditing = Me.m_objDataSource.isDetailEditing
        '    End If

        '    objEditColor = m_objSavedEditColor
        '    If Not m_objDataSource.AllowGridEditing And Not m_objDataSource.AllowDetailGridEditing Then
        '        objEditColor = m_objSavedColor
        '    End If

        '    If blnEditing Then
        '        If Not Me.Enabled Then
        '            If Me.TableStyles.Count > 0 Then
        '                Me.TableStyles(0).BackColor = CAM.App.configuration.controlDisableColor()
        '                Me.TableStyles(0).ForeColor = SystemColors.InactiveBorder
        '            Else
        '                Me.BackColor = CAM.App.configuration.controlDisableColor()
        '                Me.ForeColor = SystemColors.InactiveBorder
        '            End If
        '        Else
        '            If Me.TableStyles.Count > 0 Then
        '                Me.TableStyles(0).BackColor = objEditColor
        '                Me.TableStyles(0).ForeColor = m_objSavedEditForeColor
        '            Else
        '                Me.BackColor = objEditColor
        '                Me.ForeColor = m_objSavedEditForeColor
        '            End If
        '        End If
        '    Else
        '        '--> Si cambio el forecolor debe colocarlo
        '        If Me.TableStyles.Count > 0 Then
        '            Me.TableStyles(0).BackColor = m_objSavedColor
        '            Me.TableStyles(0).ForeColor = m_objSavedForeColor
        '        Else
        '            Me.BackColor = m_objSavedColor
        '            Me.ForeColor = m_objSavedForeColor
        '        End If
        '    End If
        'End If


    End Sub

    Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
        MyBase.OnSizeChanged(e)

        AdjustColumnSize()
    End Sub

    Public Overrides Sub Refresh()
        MyBase.Refresh()
        RaiseEvent Refreshing(Me, New EventArgs)
    End Sub

    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        Dim sf As StringFormat = New StringFormat
        Dim oc As DataGridCell
        Dim r As Rectangle
        Dim w, h As Integer
        Dim y As Integer
        Dim x As Integer
        Dim ph As Integer
        Dim wh As Integer
        Dim bx As Integer
        Dim cW As New ArrayList
        Dim r2 As Rectangle
        Dim fr As Integer
        Dim gLp As Pen
        Dim rHeadC As Brush
        Dim gLpR As Pen
        Dim backBrush As Brush
        Dim nRows As Boolean
        Dim pDGray As Pen
        Dim pLGray As Pen
        Dim pWhite As Pen

        MyBase.OnPaint(pe)


        If Me.DesignMode Or Not mUseAuotFillLines Then
            Exit Sub
        End If

        If Me.VertScrollBar.Visible Then
            Exit Sub
        End If

        If Me.VisibleColumnCount = 0 Then
            Exit Sub
        End If
        If Me.TableStyles.Count = 0 Then
            Exit Sub
        End If
        If Me.TableStyles(0).GridColumnStyles.Count = 0 Then            Exit Sub
        End If
        Dim rHeadW As Integer = 0
        Dim nRowH As Boolean
        If Me.RowHeadersVisible Then
            nRowH = True
            rHeadW = Me.TableStyles(0).RowHeaderWidth
        End If


        fr = Me.VisibleRowCount
        If fr > 0 Then
            r = Me.GetCellBounds(fr - 1, 0)
            y = r.Top + r.Height
            nRows = True
        Else
            nRows = False
            r = New Rectangle(1, 0, Me.PreferredColumnWidth, Me.PreferredRowHeight)
            If Me.CaptionVisible Then
                y = r.Top + Me.PreferredRowHeight * 2 + 8
            Else
                y = r.Top + Me.PreferredRowHeight + 5
            End If
        End If

        x = r.X
        ph = Me.PreferredRowHeight
        bx = x

        Dim oG As DataGridColumnStyle
        Dim xy As Integer = rHeadW + 1

        For j As Integer = 0 To Me.VisibleColumnCount - 1
            If fr > 0 Then
                r = Me.GetCellBounds(0, j)
            Else
                oG = Me.TableStyles(0).GridColumnStyles(j)
                r = New Rectangle(xy, 0, oG.Width, Me.PreferredRowHeight)
                xy = xy + r.Width
            End If
            cW.Add(r)
        Next

        gLp = New Pen(Me.GridLineColor)
        backBrush = New SolidBrush(Me.TableStyles(0).BackColor)


        rHeadC = New SolidBrush(Me.HeaderBackColor)
        gLpR = New Pen(Me.GridLineColor)


        pDGray = New Pen(Color.DarkGray)
        pLGray = New Pen(Color.LightGray)
        pWhite = New Pen(Color.White)

        For i As Integer = fr To 100
            For j As Integer = 0 To Me.VisibleColumnCount - 1

                r = cW(j)
                x = r.X

                If nRowH Then
                    '-->EL row Header
                    pe.Graphics.FillRectangle(rHeadC, 2, y, Me.TableStyles(0).RowHeaderWidth, r.Height)
                    r2 = r.Inflate(r, 1, 1)
                    pe.Graphics.DrawRectangle(gLpR, 2, y, Me.TableStyles(0).RowHeaderWidth - 1, r2.Height)
                    '-->linea del header gris horizontal
                    pe.Graphics.DrawLine(pLGray, 2, y, Me.TableStyles(0).RowHeaderWidth, y)
                    '-->linea del header blanca horizontal
                    pe.Graphics.DrawLine(pWhite, 2, y + 1, Me.TableStyles(0).RowHeaderWidth, y + 1)
                    '-->linea del header blanca vertical
                    pe.Graphics.DrawLine(pWhite, 2, y, 2, r2.Height + 3)
                End If

                ''-->Dibujar las líneas normales de datos
                pe.Graphics.FillRectangle(backBrush, x, y, r.Width, r.Height)
                r2 = r.Inflate(r, 1, 1)
                pe.Graphics.DrawRectangle(gLp, x, y, r.Width, r2.Height)
                If nRows Then
                    If j > 0 Then
                        pe.Graphics.DrawLine(pWhite, x, y, x, r2.Height + 3)
                    Else
                        pe.Graphics.DrawLine(pDGray, x - 1, y, x - 1, r2.Height + 3)
                    End If
                End If


            Next
            y = y + ph
            x = bx
        Next

        gLp.Dispose()
        backBrush.Dispose()
        rHeadC.Dispose()
        gLpR.Dispose()

        pDGray.Dispose()
        pLGray.Dispose()
        pWhite.Dispose()


    End Sub

End Class

