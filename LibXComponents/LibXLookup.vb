Imports System.ComponentModel
Public Class LibXLookup
    Inherits System.Windows.Forms.UserControl

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.TabStop = False
    End Sub

    'UserControl overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents picLook As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(LibXLookup))
        Me.picLook = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'picLook
        '
        Me.picLook.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picLook.Image = CType(resources.GetObject("picLook.Image"), System.Drawing.Image)
        Me.picLook.Location = New System.Drawing.Point(0, 0)
        Me.picLook.Name = "picLook"
        Me.picLook.Size = New System.Drawing.Size(16, 16)
        Me.picLook.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picLook.TabIndex = 0
        Me.picLook.TabStop = False
        '
        'LibXLookup
        '
        Me.Controls.Add(Me.picLook)
        Me.Name = "LibXLookup"
        Me.Size = New System.Drawing.Size(16, 16)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Friend m_blnIsGrid As Boolean
    Private m_objGrid As LibXGrid
    Private mSql() As String
    Private mSQLTab() As String
    Private mWhereParam As String
    Private mTabName As String
    Private mTextChanged As Boolean = False
    Private m_objFindRow As DataRow
    Private mDataMember As String
    Private misCanceled As Boolean = False
    Private mDataSource As LibXConnector
    Private mVisParameters() As String
    Private mSrcParameters() As String
    Private mDestParameters() As String
    Private mSizeColumns() As String
    Private mSizeColumnsTab() As String
    Private m_blnIngnoreFindInBrowseMode As Boolean
    Private mShowWarning As Boolean = False
    Private mShowLookup As Boolean = True
    Private mPopupSize As Size
    Private mSrcP As LookupColumnCollection
    Private mVisP As LookupColumnCollection
    Private mDesP As LookupColumnCollection
    Private mWhereCondition As String
    Private mDataAdp As OleDb.OleDbDataAdapter
    Private mLkTable As DataTable
    Private mfmLook As fmLook = Nothing
    Private mUseTab As Boolean = False
    Private mLookCaption As String = ""
    Private mUseRowRetrieveEvents As Boolean
    Private mRow As DataRow
    Private mFound As Boolean
    Private mSearchAlternate As Boolean
    Private m_objCurrencyManager As CurrencyManager
    Private mUseCopyConnection As Boolean
    Private mShowMessageNotFound As Boolean = True

    Private m_BeginCheck As Boolean = False
    Private m_objDesBindings As Hashtable
    Private m_objDestParameters As LookupColumnCollection
    Private m_objSrcParameters As LookupColumnCollection
    Private m_objControls As Hashtable
    Private m_objDesControls As Hashtable
    Private m_objSrcControls As Hashtable
    Private m_blnHandledCreated As Boolean
    Private m_blnUsePositionChanged As Boolean
    Private m_blnRetieving As Boolean
    Private mFilterField As String
    Private mAlternateFieldSearch As String
    Private mCheckLabel As String
    Private mComboMode As Boolean
    Private mShowFilter As Boolean = True

    Public Const WM_LBUTTONDOWN = &H201
    Private Const WM_MOUSEMOVE = &H200


    Private m_objOtherControls As Collection

    Public Event BeforeExecuteQuery(ByVal sender As Object, ByVal e As BeforeExecuteQueryEventArgs)
    Public Event BeforeDBExecuteQuery(ByVal sender As Object, ByVal e As BeforeExecuteQueryEventArgs)
    Public Event RowRetrieve(ByVal sender As Object, ByVal e As RowRetrieveEventArgs)
    Public Event BeforeSetValues(ByVal sender As Object, ByVal e As LookupValuesEventArgs)
    Public Event AfterSetValues(ByVal sender As Object, ByVal e As LookupValuesEventArgs)
    Public Event BeforeFind(ByVal sender As Object, ByVal e As LookupFindEventArgs)
    Public Event BefeforeExecuteSqlFind(ByVal sender As Object, ByVal e As LookupFindEventArgs)
    Public Event CreatingGridColumns(ByVal sender As Object, ByVal e As CreatingGridColumnsEventArgs)
    Public Event CreatedGridColumns(ByVal sender As Object, ByVal e As CreatedGridColumnsEventArgs)
    Public Event CheckedChanged(ByVal sender As Object, ByVal e As CheckedChangedEventArgs)

    Public Property ShowFilter() As Boolean
        Get
            Return mShowFilter
        End Get
        Set(ByVal Value As Boolean)
            mShowFilter = Value
        End Set
    End Property

    Public Property BeginCheck() As Boolean
        Get
            Return m_BeginCheck
        End Get
        Set(ByVal Value As Boolean)
            m_BeginCheck = Value
        End Set
    End Property


    Public Property LookCaption() As String
        Get
            Return mLookCaption
        End Get
        Set(ByVal Value As String)
            mLookCaption = Value
        End Set
    End Property

    Public Property PopupSize() As Size
        Get
            Return mPopupSize
        End Get
        Set(ByVal Value As Size)
            mPopupSize = Value
        End Set
    End Property

    Public Property CheckText() As String
        Get
            Return mCheckLabel
        End Get
        Set(ByVal Value As String)
            mCheckLabel = Value
        End Set
    End Property

    Public Property FilterField() As String
        Get
            Return mFilterField
        End Get
        Set(ByVal Value As String)
            mFilterField = Value
        End Set
    End Property

    Public Property AlternateFieldSearch() As String
        Get
            Return mAlternateFieldSearch
        End Get
        Set(ByVal Value As String)
            mAlternateFieldSearch = Value
        End Set
    End Property

    Public Property ShowWarning() As Boolean
        Get
            Return mShowWarning
        End Get
        Set(ByVal Value As Boolean)
            mShowWarning = Value
        End Set
    End Property

    Public Property isCanceled() As Boolean
        Get
            Return misCanceled
        End Get
        Set(ByVal Value As Boolean)
            misCanceled = Value
        End Set
    End Property

    Public Property IgnoreFindInBrowseMode() As Boolean
        Get
            Return m_blnIngnoreFindInBrowseMode
        End Get
        Set(ByVal Value As Boolean)
            m_blnIngnoreFindInBrowseMode = Value
        End Set
    End Property

    <Browsable(True)> _
    Public Property ShowMessageNotFound() As Boolean
        Get
            Return mShowMessageNotFound
        End Get
        Set(ByVal Value As Boolean)
            mShowMessageNotFound = Value
        End Set
    End Property
    Public Property UseRowRetrieveEvents() As Boolean
        Get
            Return mUseRowRetrieveEvents
        End Get
        Set(ByVal Value As Boolean)
            mUseRowRetrieveEvents = Value
        End Set
    End Property

    Public Property WhereParameters() As String
        Get
            Return mWhereParam
        End Get
        Set(ByVal Value As String)
            mWhereParam = Value
        End Set
    End Property

    <Browsable(False)> _
    Public ReadOnly Property DataRow() As Object
        Get
            Return mRow
        End Get
    End Property

    Public Property WhereCondition() As String
        Get
            Return mWhereCondition
        End Get
        Set(ByVal Value As String)
            mWhereCondition = Value
        End Set
    End Property

    Public Property ComboMode() As Boolean
        Get
            Return mComboMode
        End Get
        Set(ByVal Value As Boolean)
            mComboMode = Value
        End Set
    End Property

    <DefaultValue(True), Browsable(True)> _
    Public Property ShowLookup() As Boolean
        Get
            Return mShowLookup
        End Get
        Set(ByVal Value As Boolean)
            mShowLookup = Value
        End Set
    End Property

    Public Sub ExecuteLookup(ByVal sql As String, ByVal where As String)
        Try
            Dim objTable As DataTable
            Dim objTableTAB As DataTable
            Dim objCol As LookupColumn
            Dim objColumn As Object
            Dim blnVisCols As Boolean = False
            Dim objVisCols As New LookupColumnCollection
            Dim griStyle As DataGridTableStyle
            Dim gridCTyle As DataGridColumnStyle
            Dim objLast As DataGridColumnStyle
            Dim gridCol As DataGridColumnStyle
            Dim objText As TextBox

            Dim blnFoundCol As Boolean
            Dim x As Double
            Dim y As Double
            Dim blnCenter As Boolean
            Dim i As Integer
            Dim intWidth As Integer
            Dim intNCols As Integer
            Dim intIndexLast As Integer
            Dim objCmd As IDbCommand
            Dim objCmdTAB As IDbCommand
            Dim strSql As String
            Dim strWhereAd As String
            Dim e3 As New BeforeExecuteQueryEventArgs

            Cursor.Current = Cursors.WaitCursor()

            If Not mDataSource Is Nothing Then
                If Not mDataSource.IsEditing Then
                    Exit Sub
                End If
            End If

            e3.SQL = sql
            RaiseEvent BeforeExecuteQuery(Me, e3)
            sql = e3.SQL

            If Trim(e3.aditionalWhere) <> "" Then
                sql = ConcatWherePart(sql, e3.aditionalWhere)
            End If

            '* --> Si hay where, se concatena
            If Trim(where) <> "" Then
                sql = ConcatWherePart(sql, where)
            Else
                If mShowWarning = True Then
                    If MessageBox.Show("Esta consulta puede tardar mucho tiempo" & vbCrLf & _
                     "Se recomienda que se especifique un criterio" & vbCrLf & _
                    "ej. las iniciales de lo que busca" & vbCrLf & vbCrLf & _
                    "Desea continuar con la consulta?", "Consultar Todos los Registros", _
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                        Exit Sub
                    End If
                End If
            End If
            '* --> Si hay where default se concatena
            If Trim(mWhereCondition) <> "" Then
                sql = ConcatWherePart(sql, mWhereCondition)
            End If

            If Trim(strWhereAd) <> "" Then
                sql = ConcatWherePart(sql, strWhereAd)
            End If

            If e3.handled Then
                Exit Sub
            End If

            objTable = New DataTable

            Me.GetVisibleColumns()
            If Not mVisP Is Nothing Then
                If mVisP.Count > 0 Then
                    blnVisCols = True
                    objVisCols = mVisP
                End If
            End If

            e3.SQL = sql
            e3.Table = objTable
            RaiseEvent BeforeDBExecuteQuery(Me, e3)

            If Not e3.customTable Is Nothing Then
                objTable = e3.customTable
            Else
                objCmd = New OleDb.OleDbCommand(e3.SQL, LibX.Data.Manager.Connection.ConnectionObject)
                objCmd.Transaction = LibX.Data.Manager.Connection.ActiveTransaction
            End If

            If mUseRowRetrieveEvents Then
                '*--> Usar un copy connection, para que el usuario, pueda 
                '*    hacer otros selects sin especificar que es copy connection
                'objCmd.Connection = New OleDb.OleDbConnection(System.Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectionStr"))  ' LibX.Data.Manager.Connection.CopyConnection
                objCmd.Connection = LibX.Data.Manager.Connection.CopyConnection
                If objCmd.Connection.State = ConnectionState.Closed Then
                    objCmd.Connection.Open()
                End If

                RemoveHandler objTable.RowChanged, AddressOf OnRowChanged
                AddHandler objTable.RowChanged, AddressOf OnRowChanged
            End If


            If mDataAdp Is Nothing Then
                mDataAdp = New OleDb.OleDbDataAdapter
            End If

            mDataAdp.SelectCommand = objCmd

            objTable.TableName = Me.mTabName

            If Not mUseRowRetrieveEvents Then
                objTable.BeginLoadData()
            End If

            If e3.customTable Is Nothing Then
                mDataAdp.Fill(objTable)
            End If

            If Not mUseRowRetrieveEvents Then
                objTable.EndLoadData()
            End If

            If mUseRowRetrieveEvents Then
                RemoveHandler objTable.RowChanged, AddressOf OnRowChanged

                objCmd.Connection.Close()
            End If

            mLkTable = objTable

            If Not mfmLook Is Nothing Then
                mfmLook = Nothing
            End If

            If mfmLook Is Nothing Then
                '* --> Si no se ha iniciado la forma
                mfmLook = New fmLook
                mfmLook.UseTab = mUseTab
                mfmLook.chk_Validation.Text = mCheckLabel
                mfmLook.ComboModo = mComboMode

                AddHandler mfmLook.Closed, AddressOf OnFormClose
                AddHandler mfmLook.chk_Validation.CheckedChanged, AddressOf OnCheckedChanged
            End If

            mfmLook.SetDataBinding(objTable)

            '// LLENAR TABLA TAB
            If mUseTab = True Then
                RefreshTab("1=2")
            End If

            '* --> Si no hay ninguna columna seleccionada como visible la pone todas 
            If Not blnVisCols Then
                For Each objColumn In objTable.Columns
                    objCol = New LookupColumn

                    objCol.DsCol = objColumn.columnname
                    If Trim(objCol.DsCol) = "" Then
                        objCol.DsCol = objColumn.caption
                    End If
                    objVisCols.add(objCol)
                Next
            End If

            '*--> Crea un gridstyle con las columnas seleccionadas
            griStyle = New DataGridTableStyle

            '* --> Se especifican las columnas visibles en el grid, tomando
            '*     en cuenta la colección. Para esto se crea un llena una gridStyleCollection
            intNCols = 0

            Dim oc As New CreatingGridColumnsEventArgs
            oc.Grid = mfmLook.Grid
            oc.TStyle = griStyle
            oc.Table = objTable

            RaiseEvent CreatingGridColumns(Me, oc)

            If Not oc.handled Then
                For Each objCol In objVisCols
                    'gridCol = New System.Windows.Forms.DataGridTextBoxColumn
                    gridCol = New XEditTextBoxColumn
                    CType(gridCol, XEditTextBoxColumn).isReadOnly = True

                    gridCol.HeaderText = objCol.DsCol
                    gridCol.MappingName = objCol.LkCol

                    '-->Si la columna es númerica se debe justicar a la izquierda

                    If (objTable.Columns(objCol.LkCol).DataType.Equals(GetType(System.Decimal)) Or _
                        objTable.Columns(objCol.LkCol).DataType.Equals(GetType(System.Single)) Or _
                        objTable.Columns(objCol.LkCol).DataType.Equals(GetType(System.Double))) Then

                        gridCol.Alignment = HorizontalAlignment.Right

                        If TypeOf gridCol Is DataGridTextBoxColumn Then
                            CType(gridCol, DataGridTextBoxColumn).Format = "#,###,###,##0.00##"
                        End If
                    End If

                    intWidth = GetSizeColumn(objCol.LkCol, mSizeColumns)
                    If intWidth > 0 Then
                        gridCol.Width = intWidth
                    End If

                    griStyle.GridColumnStyles.Add(gridCol)

                    intNCols = intNCols + 1
                Next

            End If

            Dim oc2 As New CreatedGridColumnsEventArgs
            oc2.Grid = mfmLook.Grid
            oc2.TStyle = griStyle
            oc2.Table = objTable

            RaiseEvent CreatedGridColumns(Me, oc2)


            griStyle.MappingName = mTabName


            '* --> Se agrega la configuración del grid.
            mfmLook.Grid.TableStyles.Clear()
            mfmLook.Grid.TableStyles.Add(griStyle)


            griStyle = mfmLook.DGrid.TableStyles(0)

            RemoveHandler mfmLook.Grid.rowHeaderDblClick, AddressOf mfmLook.ColumnDbClick
            AddHandler mfmLook.Grid.rowHeaderDblClick, AddressOf mfmLook.ColumnDbClick

            RemoveHandler mfmLook.Grid.columnDblClick, AddressOf mfmLook.ColumnDbClick
            AddHandler mfmLook.Grid.columnDblClick, AddressOf mfmLook.ColumnDbClick

            If UseTab = True Then
                AddHandler mfmLook.Grid.CurrentRowChanged, AddressOf CurrentRowChanged
            End If

            '* --> Se especifican las columnas visibles en el grid, tomando
            '*     en cuenta la colección. Para esto se crea un llena una gridStyleCollection
            intNCols = 0
            For Each gridCol In griStyle.GridColumnStyles

                If TypeOf gridCol Is DataGridTextBoxColumn Then
                    objText = CType(gridCol, DataGridTextBoxColumn).TextBox
                    RemoveHandler objText.DoubleClick, AddressOf mfmLook.ColumnDbClick
                    AddHandler objText.DoubleClick, AddressOf mfmLook.ColumnDbClick
                End If

                '-->gridCol.ReadOnly = True

                objLast = gridCol
                intNCols = intNCols + 1
            Next

            mfmLook.Grid.UseHandCursor = True
            mfmLook.Grid.FullRowSelect = True
            mfmLook.Grid.IsReadOnly = True
            objTable.DefaultView.AllowNew = False
            objTable.DefaultView.AllowEdit = False
            objTable.DefaultView.AllowDelete = False

            mfmLook.Grid.Focus()

            mfmLook.Grid.RowHeaderWidth = 2

            '* --> Si se especifica un caption default para el form
            If Trim(mLookCaption) <> "" Then
                mfmLook.Text = mLookCaption
            End If

            mfmLook.Grid.TabIndex = 0
            mfmLook.Grid.BackgroundColor = Color.White

            '// ajustar el size segun especificado
            If mPopupSize.Width > 0 And mPopupSize.Height > 0 Then
                mfmLook.Size = mPopupSize
            End If

            mfmLook.Grid.AdjustColumnSizeEx(True, True)

            If mLkTable.Rows.Count = 1 Then
                Me.mfmLook.ok = True
                OnFormClose(Me, New System.EventArgs)
                mfmLook.Close()
                mfmLook.Dispose() 
            Else
                mfmLook.chk_Validation.Checked = m_BeginCheck
                mfmLook.ShowDialog()

                If mfmLook.DialogResult = DialogResult.OK Then 
                Else
                    If Me.mfmLook.ok = True Then
                        Me.mfmLook.ok = True
                    Else
                        Me.mfmLook.ok = False
                    End If
                    'Me.mfmLook.ok = True
                    OnFormClose(Me, New System.EventArgs)
                    mfmLook.Close()
                End If
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        Finally
            Cursor.Current = Cursors.Default
        End Try

    End Sub

    Private Function GetSizeColumn(ByVal ColName As String, ByVal pSizeColumns As String()) As Integer
        Dim aSize As String()
        Dim mSize As String
        Dim iSize As Integer = -1

        If pSizeColumns Is Nothing Then
            Exit Function
        End If

        For Each mSize In pSizeColumns
            aSize = Split(mSize, "=")
            If aSize(0) = ColName Then
                iSize = Val(aSize(1))
                Exit For
            End If
        Next

        Return iSize
    End Function
    Private Sub CurrentRowChanged(ByVal sender As Object, ByVal e As LibXGrid.XDataGridCurrentRowChangedEventArgs)
        Dim Where As String

        Where = mWhereParam
        For Each ocol As DataColumn In mLkTable.Columns
            If Not IsNull(mLkTable.Rows(e.row).Item(ocol.ColumnName)) Then
                Where = Where.Replace("?" & ocol.ColumnName.Trim, mLkTable.Rows(e.row).Item(ocol.ColumnName))
            End If
        Next

        RefreshTab(Where)
    End Sub

    Public Sub RefreshTab(Optional ByVal WhereString As String = "")
        Dim objTableTAB As DataTable
        Dim objCmdTAB As IDbCommand
        Dim strSql As String
        Dim DataAdp As OleDb.OleDbDataAdapter
        Dim oCol As DataColumn
        Dim griStyle As DataGridTableStyle
        Dim intWidth As Integer

        '// INICIAR EL COMMAND DEL TABGRID
        strSql = GetSQLTab()

        If WhereString.Trim <> "" Then
            strSql = ConcatWherePart(strSql, WhereString)
        End If

        objCmdTAB = New OleDb.OleDbCommand(strSql, LibX.Data.Manager.Connection.CopyConnection)

        If objCmdTAB.Connection.State = ConnectionState.Closed Then
            objCmdTAB.Connection.ConnectionString = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectionStr")
            objCmdTAB.Connection.Open()
        End If

        DataAdp = New OleDb.OleDbDataAdapter
        objTableTAB = New DataTable
        objTableTAB.TableName = "tableTAB"

        DataAdp.SelectCommand = objCmdTAB
        DataAdp.Fill(objTableTAB)

        mfmLook.SetDataBindingTAB(objTableTAB)

        griStyle = New DataGridTableStyle
        griStyle.MappingName = objTableTAB.TableName

        For Each oCol In objTableTAB.Columns
            'gridCol = New System.Windows.Forms.DataGridTextBoxColumn

            Dim gridCol As New XEditTextBoxColumn
            CType(gridCol, XEditTextBoxColumn).isReadOnly = True

            gridCol.HeaderText = oCol.Caption
            gridCol.MappingName = oCol.ColumnName

            '-->Si la columna es númerica se debe justicar a la izquierda

            If (objTableTAB.Columns(oCol.ColumnName).DataType.Equals(GetType(System.Decimal)) Or _
                objTableTAB.Columns(oCol.ColumnName).DataType.Equals(GetType(System.Single)) Or _
                objTableTAB.Columns(oCol.ColumnName).DataType.Equals(GetType(System.Double))) Then

                gridCol.Alignment = HorizontalAlignment.Right

                If TypeOf gridCol Is DataGridTextBoxColumn Then
                    CType(gridCol, DataGridTextBoxColumn).Format = "#,###,###,##0.00##"
                End If
            End If

            intWidth = GetSizeColumn(oCol.ColumnName, mSizeColumnsTab)
            If intWidth > 0 Then
                gridCol.Width = intWidth
            End If

            griStyle.GridColumnStyles.Add(gridCol)

        Next
        mfmLook.LibXGridTAB.TableStyles.Clear()
        mfmLook.LibXGridTAB.TableStyles.Add(griStyle)

    End Sub
    Private Sub EditingChanging(ByVal e As Object, ByVal p_blnIsDetail As Boolean)
        Dim objcol As LookupColumn
        Dim p_blnIngoreExeFind As Boolean
        Dim blnIsEditing As Boolean

        Try

            Me.Enabled = Me.mDataSource.EnableControl
            Me.mFound = False

            If Not e Is Nothing Then
                If e.isMoving Then
                    Exit Sub
                End If
            End If

            If mDataSource.State = LibxConnectorState.Edit Then
                blnIsEditing = True
            End If
            If p_blnIsDetail Then
                blnIsEditing = False
                'If Me.m_objDataSource.getCurrentDetailAction = NetDataSource.NetDetailAction.Edit Then
                '    blnIsEditing = True
                'End If
            End If

            If Not Me.m_objDestParameters Is Nothing Then
                Me.GetDestColumns()
            End If

            PrepareOtherControls(p_blnIsDetail)


            If m_objControls Is Nothing Then
                m_objControls = mDataSource.getOwnerFormControls
            End If

            If m_objSrcControls Is Nothing Then
                Me.GetSrcColumns()
            End If


            '* --> Si hay campos fuentes, se crean asocian handlers para los eventos de leave, para la validaciones
            '* --> de find. Esto se hace si el lookup no es para calendar. 
            '* --> Tambien se valida que no se asocien más de una vez.
            If Not m_blnHandledCreated Then
                'EMR If Not mDataSource.State = LibxConnectorState.Query Then
                m_blnHandledCreated = True
                FillSourceParameters(True, p_blnIngoreExeFind)
                'EMR End If
            End If

            '-->If Me.m_objDataSource.state = NetDataSource.NetDataSourceModes.EditMode Then
            If blnIsEditing Then
                If Not mDataSource.UpdatePrimaryKeyColumns Then
                    'If Trim(m_strDataTableName) = "" Or Trim(m_strDataTableName) = "tquery" Then
                    '    getAtatchTableName()
                    'End If
                    'If Not m_objDataSource.getTable(m_strDataTableName).PrimaryKey Is Nothing Then
                    '    If m_objDataSource.getTable(m_strDataTableName).PrimaryKey.Length > 0 Then
                    '        For Each objcol In m_objDestParameters
                    '            Dim objColumn As DataColumn
                    '            If m_objDataSource.getTable(m_strDataTableName).Columns.Contains(objcol.Column) Then
                    '                objColumn = m_objDataSource.getTable(m_strDataTableName).Columns(objcol.Column)
                    '            End If
                    '            '--> Tal vez es un control
                    '            If objColumn Is Nothing Then
                    '                If m_objControls.containtsKey(objcol.Column.ToUpper) Then
                    '                    Dim objControlc As Control
                    '                    objControlc = m_objControls.Item(objcol.Column.ToUpper)
                    '                    If objControlc.DataBindings.Count > 0 Then
                    '                        objColumn = m_objDataSource.getTable(m_strDataTableName).Columns(objControlc.DataBindings(0).BindingMemberInfo.BindingField)
                    '                    End If
                    '                End If
                    '            End If
                    '            If Not objColumn Is Nothing Then
                    '                If Not Array.IndexOf(m_objDataSource.getTable(m_strDataTableName).PrimaryKey, objColumn) < 0 Then
                    '                    Me.Enabled = False
                    '                End If
                    '            End If
                    '        Next
                    '    End If
                    'End If '--> if Not m_objDataSource.Tables(m_strDataTableName).PrimaryKey Is Nothing Then
                End If '-->  If Not m_objDataSource.updatePrimaryKeyColumns Then
            End If

            If Me.Enabled Then
                'If Trim(m_strDataTableName) = "" Or Trim(m_strDataTableName) = "tquery" Then
                '    getAtatchTableName()
                'End If
                'If Trim(Me.m_objDataSource.UpdateDetailTableName) <> "" And Not m_objDataSource.AllowDetailGridEditing Then
                '    If Trim(m_strDataTableName).ToLower = Trim(Me.m_objDataSource.UpdateDetailTableName).ToLower Then
                '        Me.Enabled = m_objDataSource.enableControlDetail
                '    End If
                'End If
            End If

            Dim objControl As Object
            Dim blnAllDisable As Boolean
            Dim canHasFocus As Boolean
            blnAllDisable = True

            '-->If Not p_blnIsDetail Then '--> Para un detail lo hace luego en el afterdetailaction, luego del updateboundcontroldet
            If Not Me.m_objSrcControls Is Nothing Then
                For Each objControl In m_objSrcControls.Values
                    If TypeOf objControl Is TextBox Then
                        canHasFocus = True
                        If Not (Not canHasFocus Or objControl.ReadOnly Or Not objControl.Enabled) Then
                            blnAllDisable = False
                        End If
                    End If
                Next


                If blnAllDisable Then
                    Me.Enabled = False
                End If

            End If '-->If Not Me.m_objSrcControls Is Nothing Then


            If Not p_blnIsDetail And (mDataSource.State = LibxConnectorState.Insert And mDataSource.CurrentAction <> LibxConnectionActions.Accept) Then
                If mDataSource.CurrentAction <> LibxConnectionActions.Print Then
                    Me.Clear()
                End If
            End If


            Me.Refresh()


        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try

    End Sub


    Private Sub doExecuteFind(ByVal p_blnCheckDs As Boolean)
        Dim blnValueNull As Boolean
        Dim objArgs As LookupValuesEventArgs
        Dim p_blnIngoreExeFind As Boolean

        If Not mDataSource.CanExecuteFind Then
            Exit Sub
        End If

        If Me.m_blnIsGrid Then
            Exit Sub
        End If

        If p_blnCheckDs Then
            If Not mDataSource Is Nothing Then
                ''''If Not mDataSource.IsEditing Or mDataSource.CurrentAction = LibxConnectionActions.Find Or _
                ''''    mDataSource.CurrentAction = LibxConnectionActions.Cancel Then
                ''''    Exit Sub
                ''''End If

                If Not mDataSource.IsEditing Or mDataSource.CurrentAction = LibxConnectionActions.Cancel Then
                    Exit Sub
                End If
            End If
        End If

        blnValueNull = FillSourceParameters(True, p_blnIngoreExeFind)

        If Me.m_objDestParameters Is Nothing Then
            Me.GetDestColumns()
        End If

        '* --> Dispara el find.
        If blnValueNull Then
            If Me.m_objDestParameters.Count > 0 Then
                objArgs = New LookupValuesEventArgs
                RaiseEvent BeforeSetValues(Me, objArgs)
                If Not objArgs.handled Then
                    FillDestParams(Nothing)
                    SetValuesToControl(Nothing)
                End If
            End If
        Else
            If Not p_blnIngoreExeFind Then
                doFind()
            End If
        End If
    End Sub


    Public Sub doFind()
        Try
            Dim sSql As String

            sSql = GetSql()

            If Trim(sSql) = "" Then
                sSql = String.Concat("Select * From ", Me.mTabName)
            End If

            doFind(sSql, "")

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub doFind(ByVal p_strWhere As String)
        Try
            Dim sSql As String

            sSql = GetSql()

            If Trim(sSql) = "" Then
                sSql = String.Concat("Select * From ", Me.mTabName)
            End If

            doFind(sSql, p_strWhere)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    <Browsable(False)> _
    Public ReadOnly Property DataFound() As Boolean
        Get
                Return mFound
        End Get
    End Property

    <Browsable(False)> _
    Public Property UseCopyConnection() As Boolean
        Get
            Return mUseCopyConnection
        End Get
        Set(ByVal Value As Boolean)
            mUseCopyConnection = Value
        End Set
    End Property

    '*----------------------------------------------------
    '* Nombre        : doFind
    '* Parametros    : p_strSQL : select a ejecutar
    '*                 p_strWhere : where a concaner
    '* Descripción   : Ejecuta el find 
    '* Desarrollador : ccaraballo
    '* Modified      : 
    '*----------------------------------------------------
    Public Sub doFind(ByVal p_strSQL As String, ByVal p_strWhere As String)
        Dim strWhere As String
        Dim objRow As Object
        Dim objcol As LookupColumn
        Dim e As New LookupFindEventArgs
        Dim e2 As New LookupValuesEventArgs
        Dim e3 As New BeforeExecuteQueryEventArgs
        Dim blnDataFound As Boolean = False
        Dim strMes As String
        Dim objTable As DataTable
        Dim objCmd As OleDb.OleDbCommand
        Dim blnUse As Boolean = LibX.Data.Manager.Connection.UseCopyConnection

        Try
            blnUse = LibX.Data.Manager.Connection.UseCopyConnection

            mFound = False
            mRow = Nothing

            If mDataSource.CurrentAction <> LibxConnectionActions.Cancel And Me.m_blnIsGrid = False Then
                If mTextChanged = False Then
                    Exit Sub
                Else
                    mTextChanged = False
                End If
            End If

            If Me.m_blnUsePositionChanged Then
                Me.UseCopyConnection = True
            End If

            '* --> Si se pasa un where, se concatena
            If Trim(p_strWhere) <> "" Then
                p_strSQL = ConcatWherePart(p_strSQL, p_strWhere)
            End If

            '* --> Si hay un where default se concatena
            If Trim(mWhereCondition) <> "" Then
                p_strSQL = ConcatWherePart(p_strSQL, mWhereCondition)
            End If

            e.SQL = p_strSQL
            RaiseEvent BeforeFind(Me, e)

            p_strSQL = e.SQL

            '* --> Construye where vasado en la colleción de parámetros de campos fuentes
            If Not e.handledBuildWhere Then
                strWhere = buildParamsWhere()

                If Trim(strWhere) = "" Then
                    strWhere = " 1=3 "
                End If
            End If

            '* --> Si obtubo un where, se concatena
            If Trim(strWhere) <> "" Then
                p_strSQL = ConcatWherePart(p_strSQL, strWhere)
            End If

            '* --> Si se especifica un where adicional durante el evento, se concatena
            If Trim(e.aditionalWhere) <> "" Then
                p_strSQL = ConcatWherePart(p_strSQL, e.aditionalWhere)
            End If

            e3.SQL = p_strSQL
            e3.wherePart = strWhere
            RaiseEvent BeforeExecuteQuery(Me, e3)
            p_strSQL = e3.SQL

            If Trim(e3.aditionalWhere) <> "" Then
                p_strSQL = ConcatWherePart(p_strSQL, e3.aditionalWhere)
            End If

            If e3.handled Then
                Exit Sub
            End If

            e.SQL = p_strSQL

            '* --> Antes de ejecutar. Permite modificar el select, ya formado completo
            RaiseEvent BefeforeExecuteSqlFind(Me, e)

            If e.handled Then
                Exit Sub
            End If

            blnDataFound = True

            e3.SQL = e.SQL
            RaiseEvent BeforeDBExecuteQuery(Me, e3)

            '*Si es un grid, y quiero hacer find, mientras hago el llenado
            '*no puedo ejecutar otro datareader
            If mUseCopyConnection Then
                LibX.Data.Manager.Connection.UseCopyConnection = True
                objTable = LibX.Data.Manager.GetDataTable(e3.SQL)
            Else
                objTable = LibX.Data.Manager.GetDataTable(e3.SQL)
            End If

            If objTable Is Nothing Then
                blnDataFound = False
            End If

            If objTable.Rows.Count <= 0 Then
                blnDataFound = False
            End If

            If objTable.Rows.Count > 1 Then
                blnDataFound = False
            End If

            If objTable.Rows.Count = 1 Then
                '// tomar el primer registro
                objRow = objTable.Rows(0)
            End If

            If objRow Is Nothing Then
                blnDataFound = False
            End If

            If objRow Is Nothing Or Not blnDataFound Then
                blnDataFound = False
                strMes = ""
                If Not (mDataSource.State = LibxConnectorState.Query Or _
                        mDataSource.State = LibxConnectorState.View) Then
                    '* --> Se desplega este mensaje si está el edit mode
                    '-->strMes = CAM.Util.getPrivateMessage(89)
                    strMes = "*"
                End If

                If m_blnRetieving Then
                    strMes = ""
                End If

                If Not mFound And (mShowFilter = True OrElse objTable.Rows.Count > 0) Then
                    If Trim(mFilterField) <> "" Then
                        If mDataSource.IsEditing Then
                            strMes = ""
                            ShowFilterLook()
                            If mFound = True Then
                                Exit Sub
                            End If
                        End If
                    End If
                End If

                If mShowMessageNotFound = True And Trim(strMes) <> "" Then
                    '*--> Se requiere mensaje igual que en todos los lugares, no con msgbox
                    MessageBox.Show("No se econtró registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If


            mFound = blnDataFound
            mRow = objRow

            '* --> Llena la colección 
            FillDestParams(objRow)

            e.row = objRow
            e.dataFound = mFound

            e2.row = objRow
            e2.dataFound = mFound
            RaiseEvent BeforeSetValues(Me, e2)

            If e2.handled Then '* --> El usuario toma el row y maneja la acción de colocar los valores
                Exit Sub
            End If

            '* --> Colocar los valores resultado en los campos destinos
            SetValuesToControl(objRow)


            If Trim(strMes) <> "" And Not blnDataFound Then
                SetFocusColumn()
            End If


            RaiseEvent AfterSetValues(Me, e2)


        Catch ex As Exception
            Log.Show(ex, Me.Name)
        Finally
            LibX.Data.Manager.Connection.UseCopyConnection = blnUse
        End Try
    End Sub

    Private Sub SearchByAlternateField()
        Dim strColName As String
        Dim sSql As String
        Dim sFilter As String

        Try
            sSql = GetSql()

            If Trim(sSql) = "" Then
                sSql = String.Concat("Select * From ", Me.mTabName)
            End If

            '-->Solo tomare el valor del primer sourceparameter
            If Me.m_objSrcParameters Is Nothing Then
                Me.GetSrcColumns()
            End If

            Dim objCol As LookupColumn = m_objSrcParameters.Item(0)

            If IsNull(objCol.Value) Then
                Exit Sub
            End If

            strColName = objCol.Value.ToUpper.trim  '.Replace(" ", "%")

            sFilter = String.Concat(Me.mAlternateFieldSearch, " = '", strColName.Trim, "'")

            Me.doFind(sSql, sFilter)

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

    Private Sub ShowFilterLook()
        Dim strColName As String
        Dim sSql As String
        Dim sFilter As String

        Try
            sSql = GetSql()

            If Trim(sSql) = "" Then
                sSql = String.Concat("Select * From ", Me.mTabName)
            End If

            '-->Solo tomare el valor del primer sourceparameter
            If Me.m_objSrcParameters Is Nothing Then
                Me.GetSrcColumns()
            End If

            Dim objCol As LookupColumn = m_objSrcParameters.Item(0)

            If IsNull(objCol.Value) Then
                Exit Sub
            End If

            strColName = objCol.Value.ToUpper.Replace(" ", "%")

            sFilter = String.Concat(Me.mFilterField, " like '", strColName.Trim, "%'")

            ExecuteLookup(sSql, sFilter)


        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

                     Public Overridable Function buildParamsWhere() As String
        Dim objCol As LookupColumn
        Dim strWhere As String
        Dim objTable As DataTable
        Dim objType As Type
        Dim strTmp As String
        Dim i As Integer
        Dim strSQL As String
        Dim strColName As String
        Dim strColumn As String
        Dim objControl As Object

        Try

            If Me.m_objSrcParameters Is Nothing Then
                Me.GetSrcColumns()
            End If

            i = 0

            '* --> Determina la tabla a la que se refiere el lookup
            objTable = mDataSource.GetTable(Me.mDataMember)

            '* --> recorre los campos
            strSQL = Me.GetSql

            For Each objCol In Me.m_objSrcParameters
                If objTable.Columns.Contains(Trim(objCol.DsCol)) Then
                    strColumn = Trim(objCol.DsCol)
                    objType = objTable.Columns(strColumn).DataType
                Else
                    strColumn = Trim(objCol.LkCol)
                    objType = Nothing

                    If Not Me.m_objSrcControls Is Nothing Then
                        objControl = m_objSrcControls.Item(objCol.DsCol)
                    End If

                    If objType Is Nothing Then
                        objType = GetType(System.String)
                    End If
                End If

                If Trim(strSQL) = "" Then
                    '*--> Si es una tabla
                    strColName = Trim(objCol.LkCol)
                Else
                    '*--> Si es un sql
                    '// Se Agrego esta condicion para poder buscar por varios campos

                    strColName = GetFullColumnName(Trim(objCol.LkCol), strSQL, Me.mTabName)
                    'if i=0 then
                    ' strColName = GetFullColumnName(Trim(objCol.LkCol), strSQL, Me.mTabName)
                    'else
                    '    strColName = GetFullColumnName(Trim(objCol.DsCol), strSQL, Me.mTabName)
                    'end if                 
                End If

                strTmp = BuildCondition(strColName, objType, objCol.Value, True)

                If i = 0 Then
                    strWhere = strTmp
                Else
                    strWhere = Trim(strWhere) & " OR " & strTmp
                End If
                i = 1
            Next

            If strWhere.Trim <> "" Then
                strWhere = "(" & strWhere & ")"
            End If

            Return strWhere
        Catch ex As Exception
            Log.Add(ex, Me.Name)
        End Try
    End Function

    Private Sub SetFocusColumn()
        Dim objControl As Object
        Dim objColT As Object
        Dim strCol As String
        Dim intCol As Integer


        If m_objSrcControls Is Nothing Then
            Exit Sub
        End If
        If m_objSrcControls.Count = 0 Then
            Exit Sub
        End If

        If Not Me.m_blnIsGrid Then
            Dim skey As String
            For Each skey In m_objSrcControls.Keys
                Exit For
            Next
            objControl = m_objSrcControls(skey)
            If Not objControl Is Nothing Then
                objControl.Focus()
            End If
        Else
            '* Creo que el caso del grid hay que manejarlo en el grid

        End If

    End Sub


    Private Sub PrepareOtherControls(ByVal p_blnIsDetail As Boolean)
        Dim strColName As String
        Dim objControl As Object
        Dim objCol As LookupColumn
        Dim objParams As LookupColumnCollection
        Dim i As Integer = 0
        Dim blnIsEditing As Boolean
        Dim blnIsNew As Boolean = False
        Dim blnIsCancel As Boolean


        If Not mDataSource Is Nothing Then
            'If mDataSource.CurrentAction = LibxConnectionActions.Find Or _
            '   mDataSource.CurrentAction = LibxConnectionActions.Add Or _
            '   mDataSource.CurrentAction = LibxConnectionActions.Cancel Then
            '    blnIsNew = True
            'End If
            If mDataSource.CurrentAction = LibxConnectionActions.Find Or _
               mDataSource.CurrentAction = LibxConnectionActions.Add Or _
               mDataSource.CurrentAction = LibxConnectionActions.Cancel Then
                blnIsNew = True
            End If

        End If

        If Me.m_objDestParameters Is Nothing Then
            Me.GetDestColumns()
        End If
        objParams = m_objDestParameters

        blnIsEditing = mDataSource.IsEditing

        If p_blnIsDetail Then
            'blnIsEditing = mDataSource.isDetailEditing
            blnIsNew = False
            'If m_objDataSource.getCurrentDetailAction = NetDataSource.NetDetailAction.Add Then
            '    '--> quite la parte del cancel, porque como quiera el me hace un executefind en el
            '    '--> caso del cancel del detail
            '    '--> m_objDataSource.getCurrentDetailAction = NetDataSource.NetDetailAction.Cancel Then

            '    blnIsNew = True
            'End If
        End If


        If mDataSource.CurrentAction = LibxConnectionActions.Cancel Or _
           mDataSource.CurrentAction = LibxConnectionActions.Find Then
            blnIsNew = True
        End If

        If mDataSource.CurrentAction = LibxConnectionActions.Cancel Then
            blnIsCancel = True
        End If

        If m_objControls Is Nothing Then
            m_objControls = mDataSource.getOwnerFormControls
        End If

        If m_objOtherControls Is Nothing Then
            m_objOtherControls = New Collection

            For Each objCol In objParams
                objControl = m_objControls.Item(objCol.DsCol.ToUpper)
                If Not objControl Is Nothing Then
                    m_objOtherControls.Add(objControl, LCase(objControl.Name))
                End If
            Next

        End If

        Dim blnAdd As Boolean
        'If m_SavedControls Is Nothing Then
        '    m_SavedControls = New NetBaseCollection
        '    blnAdd = True
        'End If

        Dim blnDoit As Boolean

        For Each objControl In m_objOtherControls
            If blnIsNew Or blnAdd Then
                blnDoit = True

                '-->objControl.Text = ""
                '--> Si este control es un src Control, y fue cancelando no debe limpiarlo
                If Not m_objSrcControls Is Nothing Then
                    If blnIsCancel Then
                        If m_objSrcControls.ContainsKey(UCase(objControl.name)) Then
                            blnDoit = False
                        End If
                    End If
                End If

                If blnDoit Then
                    objControl.Text = ""
                End If

            End If
        Next

    End Sub

    Private Sub prepareOtherControls()
        Me.PrepareOtherControls(False)
    End Sub


    Public Sub Clear(Optional ByVal p_blnDisableControls As Boolean = False)
        Try
            '--> Para asegurar con los controles sean iniciado
            FillSourceParameters(True, True)

            SetValuesToControl(Nothing, p_blnDisableControls)

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

    Private Function FillSourceParameters(Optional ByVal p_blnSetHandler As Boolean = False, Optional ByRef p_blnIngoreExeFind As Boolean = False) As Boolean
        Dim objBinding As Binding
        Dim objCol As LookupColumn
        Dim objParams As LookupColumnCollection
        Dim objControl As Object
        Dim i, j As Integer
        Dim blnValueNull As Boolean
        Dim strValue As String
        Dim blnFound As Boolean
        Dim strDataMember As String
        Dim objCurr As CurrencyManager
        Dim objRow As DataRowView
        Dim intNulls As Integer

        Try

            If m_objControls Is Nothing Then
                m_objControls = mDataSource.getOwnerFormControls
            End If
            '*--> Llena el campo seleccionado con el valor indicado, para buscar
            '*    el campo del que se trata compara el bindinginfo, y va buscando dentro
            '*    de los campos de origen.

            If Me.m_objSrcParameters Is Nothing Then
                Me.GetSrcColumns()
            End If

            objParams = m_objSrcParameters


            If m_objSrcControls Is Nothing Then
                Me.FillSrcBindingControls(p_blnSetHandler)

                For Each objCol In objParams
                    For Each objControl In m_objControls.Values
                        blnFound = False
                        For Each objBinding In objControl.DataBindings
                            'If Trim(m_strDataMember) = "" Then
                            strDataMember = objBinding.BindingMemberInfo.BindingPath
                            'Else
                            '    strDataMember = m_strDataMember
                            'End If
                            If Trim(UCase(objBinding.BindingMemberInfo.BindingPath)) = Trim(UCase(strDataMember)) Then
                                '-->If InStr(UCase(objBinding.BindingMemberInfo.Bindi   ngMember), Trim(UCase(objCol.DsCol))) > 0 Then
                                If UCase(objBinding.BindingMemberInfo.BindingField) = Trim(UCase(objCol.DsCol)) Then
                                    m_objSrcControls.Add(Trim(UCase(objCol.DsCol)), objControl)

                                    If p_blnSetHandler Then
                                        setHandleToControl(objCol.DsCol, objControl)
                                    End If

                                    blnFound = True
                                    Exit For
                                End If
                            End If
                        Next
                        If blnFound Then
                            Exit For '*--> si ya encontró esa col, no bucar mas controls
                        End If
                    Next
                Next
            End If

            blnValueNull = False
            i = 0
            intNulls = 0
            If m_objSrcControls.Count > 0 Then
                For Each objCol In objParams
                    strValue = ""
                    '// EMR Se modifico para la busqueda con mas de una columna
                    '// 12/12/2007
                    objControl = m_objSrcControls.Item(UCase(objCol.DsCol))
                    '// ESTE ES el nuevo (tiene error) objControl = m_objSrcControls.Item(UCase(objCol.LkCol))

                    If Not objControl Is Nothing Then
                        If Not Me.m_blnUsePositionChanged Then
                            strValue = Trim(objControl.text)

                            If Not objControl.Visible Then
                                If Trim(strValue) = "" Then
                                    If objControl.DataBindings.Count > 0 Then
                                        If Not objControl.DataBindings(0).BindingManagerBase Is Nothing Then
                                            If objControl.DataBindings(0).BindingManagerBase.Count > 0 Then
                                                objRow = CType(objControl.DataBindings(0).BindingManagerBase.Current, DataRowView)
                                            End If
                                        End If
                                        If objRow Is Nothing Then
                                            'If objControl.DataBindings(0).BindingMemberInfo.BindingPath.ToLower = Trim(mDataSource.UpdateTableName).ToLower Then
                                            objRow = Me.mDataSource.CurrentDataRow
                                            'End If
                                            'If objControl.DataBindings(0).BindingMemberInfo.BindingPath.ToLower = Trim(m_objDataSource.UpdateDetailTableName).ToLower Then
                                            '--> objRow = Me.m_objDataSource.CurretDataRowDetail
                                            'End If
                                        End If

                                        If Not objRow Is Nothing Then
                                            strValue = objRow(objControl.DataBindings(0).BindingMemberInfo.BindingField).ToString.Trim
                                        End If

                                    End If
                                End If
                            End If '' visible

                        Else
                            If objControl.DataBindings.count > 0 Then

                                'Cuando se produce un detailpositoinchanged por un afterloaddetail en un master detail donde el detail está en 
                                'otra tab como en el ecercn07, el find no llena porque el texto de los src controls está en blanco.
                                'En este caso en el momento que el inspecciona el control el row tiene el valor pero el texto está en blanco. Lo que descubrimos es que si el control no está actualmente visible (no estamos hablando de la propiedad visible) en ese momento, la prop text no se llena. Por lo que en ese programa si damos consultar aceptar y el tab de detalle está mostrado el find lo hace bien, pero si damos consultar aceptar y el tab del detalle está tapado el find no lo hace bien.
                                If Me.mDataSource.IsEditing Then
                                    strValue = Trim(objControl.text)

                                    '-->Si el control esta visible false, o esta visible false porque esta en un tabpage
                                    '   que no es el actual, por lo que este control no ha hecho su bindingcontextchanged,
                                    '   aquí el texto que se lee da blanco aunque su valor en el rowview está correcto. Por eso
                                    '   se hizo esto.
                                    If Not objControl.Visible Then
                                        If Trim(strValue) = "" Then
                                            If objControl.DataBindings.Count > 0 Then
                                                If Not objControl.DataBindings(0).BindingManagerBase Is Nothing Then
                                                    If objControl.DataBindings(0).BindingManagerBase.Count > 0 Then
                                                        objRow = CType(objControl.DataBindings(0).BindingManagerBase.Current, DataRowView)
                                                    End If
                                                End If
                                                If objRow Is Nothing Then
                                                    'If objControl.DataBindings(0).BindingMemberInfo.BindingPath.ToLower = Trim(mDataSource.UpdateTableName).ToLower Then
                                                    objRow = Me.mDataSource.CurrentDataRow
                                                    'End If
                                                End If
                                                If Not objRow Is Nothing Then
                                                    strValue = objRow(objControl.DataBindings(0).BindingMemberInfo.BindingField).ToString.Trim
                                                End If
                                            End If
                                        End If
                                    End If '' visible

                                Else
                                    If objControl.DataBindings(0).BindingManagerBase.Count > 0 Then
                                        objRow = CType(objControl.DataBindings(0).BindingManagerBase.Current, DataRowView)

                                        '// EMR se modifico para la busqueda con mas de una columna
                                        '// 12/12/2007
                                        If objRow.Row.Table.Columns.Contains(objCol.DsCol) Then
                                            '// ESTE ES el nuevo (tiene error)If objRow.Row.Table.Columns.Contains(objCol.LkCol) Then
                                            strValue = objRow(objCol.LkCol).ToString.Trim
                                        Else
                                            '-->Cuando el lookup estaba en un cambo de detalle y el srce parameter era un control, no encontraba nada en viewmode navegando entre los registros.
                                            If objRow.Row.Table.Columns.Contains(objControl.DataBindings(0).BindingMemberInfo.BindingField) Then
                                                strValue = objRow(objControl.DataBindings(0).BindingMemberInfo.BindingField).ToString.Trim
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If Trim(strValue) = "" Then
                        blnValueNull = True
                        intNulls = intNulls + 1
                    End If
                    m_objSrcParameters.Item(i).Value = Trim(strValue)
                    i = i + 1
                Next
            End If

            '--> Para que haga el find deben estan todos los source params llenos
            If blnValueNull Then
                '--> Si hay nulos, pero no todos no hacer el find, porque eso es que estan
                '    llenando
                If mDataSource.IsEditing Then
                    '*-->Si la cantidad de campos nulos 
                    If intNulls <> m_objSrcControls.Count Then
                        blnValueNull = False
                        p_blnIngoreExeFind = True
                    End If
                End If
            End If

            Return blnValueNull

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Function

    Private Sub FillSrcBindingControls(Optional ByVal p_blnSetHandler As Boolean = False)
        Dim objBinding As Binding
        Dim strColName As String
        Dim objCol As LookupColumn
        Dim objControl As Object

        m_objSrcControls = New Hashtable

        If m_objControls Is Nothing Then
            m_objControls = mDataSource.getOwnerFormControls
        End If

        If Me.m_objSrcParameters Is Nothing Then
            Me.GetSrcColumns()
        End If

        For Each objCol In m_objSrcParameters
            strColName = objCol.DsCol
            objControl = m_objControls.Item(strColName.ToUpper)

            If Not objControl Is Nothing Then
                m_objSrcControls.Add(UCase(strColName), objControl)
                If p_blnSetHandler Then
                    Me.setHandleToControl(strColName, objControl)
                    Exit Sub
                End If
            End If '*-->If Not objControl Is Nothing Then

        Next


    End Sub

    Public Overridable Sub TextControlTextChanged(ByVal sender As Object, ByVal e As EventArgs)
        mTextChanged = True
    End Sub

    Public Overridable Sub TextControlLeave(ByVal sender As Object, ByVal e As EventArgs)
        If sender.readonly = False Then
            Me.doExecuteFind(True)
        End If

    End Sub
    Public Overridable Sub TextControlKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        If sender.readonly = False And mShowLookup = True Then
            If e.KeyCode = Keys.F4 Then
                Me.ExecuteLookup()
            End If
        End If
    End Sub

    Public Overridable Sub TextControlDoubleClick(ByVal sender As Object, ByVal e As EventArgs)
        If sender.readonly = False And mShowLookup = True Then
            Me.ExecuteLookup()
        End If
    End Sub

    Private Sub setHandleToControl(ByVal p_strColName As String, Optional ByVal p_objControl As Control = Nothing)
        Dim strSQL As String
        Dim row As DataRow
        Dim objBinding As Binding
        Dim objControl As Control
        Dim objFrm As Form


        Try
            objFrm = Me.mDataSource.OwnerForm

            If p_objControl Is Nothing Then
                For Each objControl In m_objControls.Values
                    For Each objBinding In objControl.DataBindings
                        If UCase(objBinding.BindingMemberInfo.BindingField) = UCase(p_strColName) Then
                            AddHandler objControl.Leave, AddressOf TextControlLeave
                            AddHandler objControl.KeyDown, AddressOf TextControlKeyDown
                            AddHandler objControl.DoubleClick, AddressOf TextControlDoubleClick
                            AddHandler objControl.TextChanged, AddressOf TextControlTextChanged
                            Exit For
                        End If
                    Next
                Next
            Else
                AddHandler p_objControl.Leave, AddressOf TextControlLeave
                AddHandler p_objControl.KeyDown, AddressOf TextControlKeyDown
                AddHandler p_objControl.DoubleClick, AddressOf TextControlDoubleClick
                AddHandler p_objControl.TextChanged, AddressOf TextControlTextChanged
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub


    Private Sub OnCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim e1 As New CheckedChangedEventArgs

        e1.Table = CType(mfmLook.Grid.DataSource, DataTable)
        e1.CheckStatus = mfmLook.chk_Validation.Checked

        RaiseEvent CheckedChanged(sender, e1)
    End Sub

    Private Sub OnFormClose(ByVal sender As Object, ByVal e As EventArgs)
        Dim e2 As New LookupValuesEventArgs
        Dim objRow As Object
        Try
            mRow = Nothing
            mFound = False
            misCanceled = False

            If Not Me.mfmLook.ok Then
                '* --> Si no se hizo ninguna selección en el grid del lookup
                misCanceled = True
                Exit Sub
            End If

            If mLkTable.Rows.Count = 0 Then
                Exit Sub
            End If

            mFound = True

            e2.row = mLkTable.DefaultView(mfmLook.BindingContext(mLkTable).Position).Row

            mRow = e2.row

            '* --> llenar los valores destindos
            FillDestParams(mRow)

            If e2.row Is Nothing Then
                Exit Sub
            End If

            e2.dataFound = mFound
            RaiseEvent BeforeSetValues(Me, e2)

            If e2.handled Then '* --> El usuario toma el row y maneja la acción de colocar los valores
                Exit Sub
            End If

            '* --> colocar los valores en los campos destinos
            Me.SetValuesToControl(e2.row)

            RaiseEvent AfterSetValues(Me, e2)

        Catch ex As Exception
            Log.Show(ex, Me.Name)
            End Try
    End Sub

    Private Sub SetValuesToControl(ByVal row As DataRow, Optional ByVal p_blnDisableControls As Boolean = False)
        Dim objCol As LookupColumn
        Dim i As Integer = 0
        Dim objTmp As Object

        Try

            If m_objDestParameters Is Nothing Then
                GetDestColumns()
            End If

            If m_objDestParameters.Count > 0 Then

                If m_blnIsGrid Then
                    '* --> Si es un grid
                    SetValuesToControlGrid(row)
                    'PRUEBA EMR Exit Sub
                End If

                For Each objCol In m_objDestParameters
                    If mFound Then
                        If Not row Is Nothing Then
                            SetTextToControl(objCol.DsCol, row(objCol.LkCol), p_blnDisableControls)
                        Else
                            SetTextToControl(objCol.DsCol, "", p_blnDisableControls)
                        End If
                    Else
                        '* --> Si no hay data
                        SetTextToControl(objCol.DsCol, "", p_blnDisableControls)
                    End If
                    i = i + 1
                Next
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub


    Private Sub SetTextToControl(ByVal p_strColName As String, ByVal p_objValue As Object, Optional ByVal p_blnDisableControls As Boolean = False)
        Dim strSQL As String
        Dim row As DataRow
        Dim objBinding As Binding
        Dim objControl As Object
        Dim objFrm As Form
        Try
            objFrm = Me.mDataSource.OwnerForm

            If IsNull(p_objValue) Then
                p_objValue = ""
            End If

            If m_objDesBindings Is Nothing Then
                FillDestBindingCollection()
            End If

            objControl = m_objDesBindings.Item(UCase(p_strColName))

            If Not objControl Is Nothing Then
                If TypeOf objControl Is LibxCheckBox Then
                    CType(objControl, LibxCheckBox).value = Trim(p_objValue)

                ElseIf TypeOf objControl Is CheckBox Then
                    CType(objControl, CheckBox).CheckState = Trim(p_objValue)

                ElseIf TypeOf objControl Is LibxDateTimePicker Then
                    If IsDate(p_objValue) Then
                        CType(objControl, LibxDateTimePicker).Value = CDate(p_objValue)
                    Else
                        CType(objControl, LibxDateTimePicker).Value = DBNull.Value
                    End If
                Else
                    objControl.Text = Trim(p_objValue)
                End If

                If p_blnDisableControls Then
                    objControl.Enabled = False
                End If

                Exit Sub
            End If

            If m_objControls Is Nothing Then
                m_objControls = mDataSource.getOwnerFormControls
            End If

            If m_objDesControls Is Nothing Then
                FillDestBindingControls()
            End If

            objControl = m_objDesControls.Item(UCase(p_strColName))


            If Not objControl Is Nothing Then
                If TypeOf objControl Is LibxCheckBox Then
                    CType(objControl, LibxCheckBox).value = Trim(p_objValue)

                ElseIf TypeOf objControl Is CheckBox Then
                    CType(objControl, CheckBox).CheckState = Trim(p_objValue)

                Else
                    objControl.Text = Trim(p_objValue)
                End If

                If p_blnDisableControls Then
                    objControl.Enabled = False
                End If
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub


    Private Sub FillDestParams(ByVal row As DataRow)
        Dim objcol As LookupColumn
        Dim objTmp As Object
        Dim dataFound As Boolean = True

        Try
            If m_objDestParameters Is Nothing Then
                Me.GetDestColumns()
            End If

            If m_objDestParameters.Count > 0 Then
                For Each objcol In m_objDestParameters
                    If mFound Then
                        If Not row Is Nothing And dataFound Then
                            If row(objcol.LkCol) Is DBNull.Value Then
                                m_objDestParameters.Item(objcol.DsCol).Value = ""
                            Else
                                m_objDestParameters.Item(objcol.DsCol).Value = row(objcol.LkCol)
                            End If
                        Else
                            m_objDestParameters.Item(objcol.DsCol).Value = ""
                        End If
                    Else
                        m_objDestParameters.Item(objcol.DsCol).Value = ""
                    End If
                Next
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

    Private Sub FillDestBindingControls()
        Try

            Dim objBinding As Binding
            Dim strColName As String
            Dim objCol As LookupColumn
            Dim objControl As Object

            m_objDesControls = New Hashtable

            If m_objControls Is Nothing Then
                m_objControls = mDataSource.getOwnerFormControls
            End If

            If m_objDestParameters Is Nothing Then
                Me.GetDestColumns()
            End If

            If m_objDestParameters.Count > 0 Then
                For Each objCol In m_objDestParameters
                    strColName = objCol.DsCol
                    For Each objControl In m_objControls.Values
                        If Trim(UCase(objControl.Name)) = Trim(UCase(strColName)) Then
                            m_objDesControls.Add(UCase(strColName), objControl)
                            Exit For
                        End If
                    Next
                Next
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

    Public Overridable Sub ExecuteFindGrid(Optional ByVal p_objGrid As Object = Nothing, Optional ByVal p_intColIndex As Integer = -1, Optional ByVal p_strValue As Object = Nothing)
        Try
            Dim strColName As String
            Dim i As Integer
            Dim objCol As LookupColumn
            Dim objParams As LookupColumnCollection
            Dim objColS As Object


            If p_objGrid Is Nothing Then
                p_objGrid = Me.m_objGrid
            End If

            If Me.m_objSrcParameters Is Nothing Then
                Me.GetSrcColumns()
            End If

            objColS = p_objGrid.TableStyles(0).GridColumnStyles(p_intColIndex)
            strColName = objColS.MappingName
            i = 0

            objParams = m_objSrcParameters
            For Each objCol In objParams
                '// Se modifico para la busqueda con varas columnas
                If InStr(UCase(strColName), UCase(objCol.DsCol)) > 0 Then
                    '// ESTE ES el nuevo (tiene error)If InStr(UCase(strColName), UCase(objCol.LkCol)) > 0 Then
                    m_objSrcParameters.Item(i).Value = p_strValue
                End If
                i = i + 1
            Next

            m_blnIsGrid = True
            m_objGrid = p_objGrid

            '* --> Se dispara el find
            doFind()

            m_blnIsGrid = False
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Private Sub SetValuesToControlGrid(ByVal row As DataRow)
        Dim objCol As LookupColumn
        Dim objParams As LookupColumnCollection
        Dim i As Integer = 0
        Dim intRow As Integer
        Dim intCol As Integer
        Dim objColT As Object
        Dim oValue As Object

        Try

            If m_objGrid Is Nothing Then
                Exit Sub
            End If
            If Me.m_objGrid.getCurrentGridView.Count = 0 Then
                Exit Sub
            End If
            If Me.m_objDestParameters Is Nothing Then
                Me.GetDestColumns()
            End If
            objParams = m_objDestParameters
            ''intRow = m_objGrid.currentRowNum
            intRow = m_objGrid.CurrentRowIndex
            ''//Cambiar el currentRowNum por el CurrentRowIndex porque el currentRowNum no se 
            ''//inicilializa cuando se crean los documentos y da un error con las devoluciones 
            ''//y Algunas facturas
            For Each objCol In objParams
                intCol = m_objGrid.getColByName(objCol.DsCol, objColT)
                If intCol >= 0 Then
                    If mFound Then
                        If Not row Is Nothing Then
                            intCol = m_objGrid.getColByName(objCol.DsCol, objColT)
                            If Not m_blnRetieving Then
                                oValue = row(objCol.LkCol)

                                If LibX.IsNull(oValue) Then
                                    m_objGrid(intRow, intCol) = DBNull.Value
                                Else
                                    m_objGrid(intRow, intCol) = row(objCol.LkCol).ToString.Trim
                                End If

                                If TypeOf objColT Is XDataGridComboBoxColumn Then
                                    If LibX.IsNull(oValue) Then
                                        CType(objColT, XDataGridComboBoxColumn).ComboBox.currValue = DBNull.Value
                                    Else
                                        CType(objColT, XDataGridComboBoxColumn).ComboBox.currValue = row(objCol.LkCol).ToString.Trim
                                    End If
                                Else
                                    If LibX.IsNull(oValue) Then
                                        objColT.TextBox.Text = ""
                                    Else
                                        objColT.TextBox.Text = row(objCol.LkCol).ToString.Trim
                                    End If
                                End If

                                If TypeOf objColT Is XEditTextBoxColumn Or TypeOf objColT Is XDataGridTextButtonColumn Then
                                    If LibX.IsNull(oValue) Then
                                        objColT.SetValue(DBNull.Value)
                                    Else
                                        objColT.SetValue(row(objCol.LkCol).ToString.Trim)
                                    End If
                                End If
                            Else
                                If LibX.IsNull(oValue) Then
                                    m_objFindRow(objCol.DsCol) = DBNull.Value
                                Else
                                    m_objFindRow(objCol.DsCol) = row(objCol.LkCol).ToString.Trim
                                End If
                            End If

                        Else
                            If Not m_blnRetieving Then
                                m_objGrid(intRow, intCol) = DBNull.Value
                                If TypeOf objColT Is XDataGridComboBoxColumn Then
                                    CType(objColT, XDataGridComboBoxColumn).ComboBox.currValue = ""
                                Else
                                    objColT.TextBox.Text = ""
                                End If

                                If TypeOf objColT Is XEditTextBoxColumn Or TypeOf objColT Is XDataGridTextButtonColumn Then
                                    objColT.SetValue(DBNull.Value)
                                End If

                            Else
                                m_objFindRow(objCol.DsCol) = DBNull.Value
                            End If
                        End If
                    Else
                        '* --> Si no hay data, el campo se limpia
                        If Not m_blnRetieving Then
                            m_objGrid(intRow, intCol) = DBNull.Value
                            'objColT.TextBox.Text = ""
                            If TypeOf objColT Is XDataGridComboBoxColumn Then
                                CType(objColT, XDataGridComboBoxColumn).ComboBox.currValue = ""
                            Else
                                objColT.TextBox.Text = ""
                            End If

                            If TypeOf objColT Is XEditTextBoxColumn Or TypeOf objColT Is XDataGridTextButtonColumn Then
                                objColT.SetValue(DBNull.Value)
                            End If

                        Else
                            m_objFindRow(objCol.DsCol) = DBNull.Value
                        End If
                    End If
                End If
                i = i + 1
            Next
        Catch ex As Exception
            Log.Add(ex)
        End Try


    End Sub

    Private Sub FillDestBindingCollection()
        Dim objBinding As Binding
        Dim strColName As String
        Dim objCol As LookupColumn
        Dim strDataMember As String
        Dim Cm As CurrencyManager
        Try

            Cm = mDataSource.GetCM(Trim(Me.mDataMember))

            m_objDesBindings = New Hashtable

            If Not Me.DestParameters Is Nothing Then
                If m_objDestParameters.Count > 0 Then
                    For Each objCol In m_objDestParameters
                        strColName = objCol.DsCol
                        For Each objBinding In Cm.Bindings
                            If Trim(mDataMember) = "" Then
                                strDataMember = objBinding.BindingMemberInfo.BindingPath
                            Else
                                strDataMember = mDataMember
                            End If

                            If Trim(UCase(objBinding.BindingMemberInfo.BindingPath)) = Trim(UCase(strDataMember)) Then
                                '-->If InStr(UCase(objBinding.BindingMemberInfo.BindingMember), UCase(strColName)) > 0 Then
                                If UCase(objBinding.BindingMemberInfo.BindingField) = UCase(strColName) Then
                                    m_objDesBindings.Add(UCase(strColName), objBinding.Control)
                                    Exit For
                                End If
                            End If
                        Next
                    Next
                End If
            End If

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub


    Private Sub OnRowChanged(ByVal sender As Object, ByVal e As DataRowChangeEventArgs)
        Dim objArgs As RowRetrieveEventArgs
        If e.Action = DataRowAction.Add Then
            objArgs = New RowRetrieveEventArgs
            objArgs.row = e.Row
            OnRowRetrieve(objArgs)
        End If
    End Sub

    Private Sub OnRowRetrieve(ByVal e As RowRetrieveEventArgs)
        RaiseEvent RowRetrieve(Me, e)
    End Sub

    Public Property SrcParameters() As String()
        Get
            Return mSrcParameters
        End Get
        Set(ByVal Value As String())
            mSrcParameters = Value
        End Set
    End Property

    Public Property VisParameters() As String()
        Get
            Return mVisParameters
        End Get
        Set(ByVal Value As String())
            mVisParameters = Value
        End Set
    End Property


    Public Property DestParameters() As String()
        Get
            Return mDestParameters
        End Get
        Set(ByVal Value As String())
            mDestParameters = Value
        End Set
    End Property

    <Browsable(True)> _
    Public Property SizesColumns() As String()
        Get
            Return mSizeColumns
        End Get
        Set(ByVal Value As String())
            mSizeColumns = Value
        End Set
    End Property

    <Browsable(True)> _
    Public Property SizesColumnsTab() As String()
        Get
            Return mSizeColumnsTab
        End Get
        Set(ByVal Value As String())
            mSizeColumnsTab = Value
        End Set
    End Property

    Public Function GetSrcColumns() As LookupColumnCollection
        Try
            If Not m_objSrcParameters Is Nothing Then
                Return m_objSrcParameters
            End If

            m_objSrcParameters = New LookupColumnCollection
            Dim s() As String
            Dim vs As String
            Dim lk As LookupColumn

            If Not mSrcParameters Is Nothing AndAlso mSrcParameters.Length > 0 Then
                For Each vs In Me.mSrcParameters
                    If Trim(vs) <> "" Then
                        s = Split(vs, "=")
                        lk = New LookupColumn
                        lk.DsCol = s(0)
                        lk.LkCol = s(1)

                        Try
                            m_objSrcParameters.add(lk)
                        Catch ex As Exception
                        End Try
                    End If
                Next
            End If

            Return m_objSrcParameters

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try

    End Function

    Public Function GetVisibleColumns() As LookupColumnCollection
        Try
            If Not mVisP Is Nothing Then
                Return mVisP
            End If

            mVisP = New LookupColumnCollection
            Dim s() As String
            Dim vs As String
            Dim lk As LookupColumn

            If Not mVisParameters Is Nothing AndAlso mVisParameters.Length > 0 Then
                For Each vs In mVisParameters
                    If Trim(vs) <> "" Then
                        s = Split(vs, "=")
                        lk = New LookupColumn
                        lk.DsCol = s(0)
                        lk.LkCol = s(1)
                        mVisP.add(lk)
                    End If
                Next
            End If

            Return mVisP

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Function


    Public Function GetDestColumns() As LookupColumnCollection
        Try
            If Not m_objDestParameters Is Nothing Then
                Return m_objDestParameters
            End If

            m_objDestParameters = New LookupColumnCollection
            Dim s() As String
            Dim vs As String
            Dim lk As LookupColumn

            If Not mDestParameters Is Nothing AndAlso mDestParameters.Length > 0 Then
                For Each vs In mDestParameters
                    If Trim(vs) <> "" Then
                        s = Split(vs, "=")
                        lk = New LookupColumn
                        lk.DsCol = s(0)
                        lk.LkCol = s(1)
                        m_objDestParameters.add(lk)
                    End If
                Next
            End If

            Return m_objDestParameters

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Function


    Public Sub ExecuteLookup()
        Try
            Dim sSql As String

            sSql = GetSql()

            If Trim(sSql) = "" Then
                sSql = String.Concat("Select * From ", Me.mTabName)
            End If

            ExecuteLookup(sSql, "")

        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub

    Public Sub ExecuteFind()
        Try
            Me.doExecuteFind(False)
        Catch ex As Exception
            Log.Show(ex, Me.Name)
        End Try
    End Sub


    Public Property SqlString() As String()
        Get
            Return mSql
        End Get
        Set(ByVal Value As String())
            mSql = Value
        End Set
    End Property

    Public Property SQLTab() As String()
        Get
            Return mSQLTab
        End Get
        Set(ByVal Value As String())
            mSQLTab = Value
        End Set
    End Property

    Public Property UseTab() As Boolean
        Get
            Return mUseTab
        End Get
        Set(ByVal Value As Boolean)
            mUseTab = Value
        End Set
    End Property

    Public Property TableName() As String
        Get
            Return mTabName
        End Get
        Set(ByVal Value As String)
            mTabName = Value
        End Set
    End Property

    Public Function GetSql() As String
        If Not mSql Is Nothing AndAlso mSql.Length > 0 Then
            Return Trim(Join(mSql, " "))
        End If
        Return ""
    End Function

    Public Function GetSQLTab() As String
        If Not mSQLTab Is Nothing AndAlso mSQLTab.Length > 0 Then
            Return Trim(Join(mSQLTab, " "))
        End If
        Return ""
    End Function

    Public Property DataSource() As LibXConnector
        Get
            Return mDataSource
        End Get
        Set(ByVal Value As LibXConnector)
            mDataSource = Value

            If Not Value Is Nothing Then
                RemoveHandler mDataSource.ChangeState, AddressOf OnDataSourceChange
                AddHandler mDataSource.ChangeState, AddressOf OnDataSourceChange

                RemoveHandler mDataSource.RowChange, AddressOf OnDataSourceRowChange
                AddHandler mDataSource.RowChange, AddressOf OnDataSourceRowChange


            End If

        End Set
    End Property

    Private Sub OnDataSourceRowChange(ByVal sender As Object, ByVal e As XRowChangeEventArgs)
        Try
            If m_blnUsePositionChanged Then
                If Not e.FistRowChangeAfterQuery Then
                    Exit Sub
                End If
            End If
            If m_blnIngnoreFindInBrowseMode Then
                Exit Sub
            End If

            doExecuteFind(False)

        Catch ex As Exception
            Log.Add(ex, Me.Name)
        End Try

    End Sub

    Public Property DataMember() As String
        Get
            Return mDataMember
        End Get
        Set(ByVal Value As String)
            mDataMember = Value
        End Set
    End Property

    Private Sub OnDataSourceChange(ByVal sender As Object, ByVal e As XChangeStateEventArgs)
        Me.EditingChanging(e)
    End Sub

    Private Sub EditingChanging(ByVal e As Object)
        Me.EditingChanging(e, False)
    End Sub

    Public Overridable Sub ExecuteFindGridOnRetrieve(ByRef row As DataRow, Optional ByVal grid As Object = Nothing, Optional ByVal p_intColIndex As Integer = -1, Optional ByVal p_strValue As Object = Nothing)
        Dim strColName As String
        Dim i As Integer
        Dim objCol As LookupColumn
        Dim objParams As LookupColumnCollection
        Dim objColS As Object

        Try

            m_blnRetieving = True

            m_objFindRow = row

            Me.mUseCopyConnection = True

            If grid Is Nothing Then
                grid = Me.m_objGrid
            End If


            objColS = grid.TableStyles(0).GridColumnStyles(p_intColIndex)
            strColName = objColS.MappingName
            i = 0
            objParams = Me.m_objSrcParameters
            For Each objCol In objParams
                If InStr(UCase(strColName), UCase(objCol.DsCol)) > 0 Then
                    m_objSrcParameters.Item(i).Value = p_strValue
                End If
                i = i + 1
            Next

            m_blnIsGrid = True
            m_objGrid = grid

            '* --> Se dispara el find
            If mDataSource.IsDataEditing = False Then
                doFind()
            End If

            m_blnIsGrid = False

        Catch ex As Exception
            Log.Add(ex)
        Finally
            Me.mUseCopyConnection = False
            m_blnRetieving = False
        End Try
    End Sub


    Public Sub SetContainerGrid(ByVal p_objGrid As Object)
        Try
            m_blnIsGrid = True
            m_objGrid = p_objGrid
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Sub picLook_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picLook.Click
        If mShowLookup = True Then
            Me.ExecuteLookup()
        End If
    End Sub

    Public Sub ExecuteLookupGrid()
        Try
            Me.m_blnIsGrid = True

            If mShowLookup = True Then
                Me.ExecuteLookup()
            End If

        Catch ex As Exception
            Log.Add(ex)
        Finally
            Me.m_blnIsGrid = False
        End Try
    End Sub

    Private Sub LibXLookup_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Me.Width = 16
        Me.Height = 20
    End Sub

    Private Sub LibXLookup_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.EnabledChanged
        If Me.Enabled = True Then
            Me.Cursor = Cursors.Hand
        Else
            Me.Cursor = Cursors.No
        End If
    End Sub
End Class

Public Class LookupColumn
    Public DsCol As String
    Public LkCol As String
    Public Value As Object
End Class

Public Class LookupColumnCollection
    Inherits CollectionBase

    Public mKey As New Hashtable

    Sub add(ByVal lk As LookupColumn)
        list.Add(lk)
        mKey.Add(lk.DsCol, lk)
    End Sub
    Sub add(ByVal lk As LookupColumn, ByVal key As String)
        list.Add(lk)
        mKey.Add(key, lk)
    End Sub


    Public ReadOnly Property Item(ByVal index As Integer) As LookupColumn
        Get
            Return CType(list.Item(index), LookupColumn)
        End Get
    End Property

    Public ReadOnly Property Item(ByVal index As String) As LookupColumn
        Get
            Return CType(mKey(index), LookupColumn)
        End Get
    End Property

End Class

Public Class BeforeExecuteQueryEventArgs
    Inherits EventArgs


    Private m_blnHandled As Boolean = False
    Private m_strSQL As String
    Private m_strAditionalWhere As String
    Private m_blnDataFound As Boolean = False
    Private m_strWhere As String
    Public Table As DataTable

    Public customTable As DataTable

    Sub New()

    End Sub

    Public Property wherePart() As String
        Get
            Return m_strWhere
        End Get
        Set(ByVal Value As String)
            m_strWhere = Value
        End Set
    End Property

    Public Property aditionalWhere() As String
        Get
            Return m_strAditionalWhere
        End Get
        Set(ByVal Value As String)
            m_strAditionalWhere = Value
        End Set
    End Property

    Public Property SQL() As String
        Get
            Return m_strSQL
        End Get
        Set(ByVal Value As String)
            m_strSQL = Value
        End Set
    End Property

    Public Property handled() As Boolean
        Get
            Return m_blnHandled

        End Get
        Set(ByVal Value As Boolean)
            m_blnHandled = Value
        End Set
    End Property

End Class


Public Class RowRetrieveEventArgs
    Inherits EventArgs

    Private m_blnHasError As Boolean = False
    Private mRow As Object

    Sub New()

    End Sub

    Public Property row() As Object
        Get
            Return mRow
        End Get
        Set(ByVal Value As Object)
            mRow = Value
        End Set
    End Property

    Public Property hasErrors() As Boolean
        Get
            Return m_blnHasError
        End Get
        Set(ByVal Value As Boolean)
            m_blnHasError = Value
        End Set
    End Property

End Class


Public Class LookupValuesEventArgs
    Inherits EventArgs

    Private m_blnHandled As Boolean = False
    Private mRow As Object
    Private m_strSQL As String
    Private m_strAditionalWhere As String
    Private m_blnHandleFindParamsValues As Boolean = False
    Private m_blnDataFound As Boolean = False

    Public Property dataFound() As Boolean
        Get
            Return m_blnDataFound
        End Get
        Set(ByVal Value As Boolean)
            m_blnDataFound = Value
        End Set
    End Property

    Sub New()

    End Sub

    Private Property handleFindParamsValues() As Boolean
        Get
            Return m_blnHandleFindParamsValues
        End Get
        Set(ByVal value As Boolean)
            m_blnHandleFindParamsValues = value
        End Set
    End Property

    Public Property aditionalWhere() As String
        Get
            Return m_strAditionalWhere
        End Get
        Set(ByVal Value As String)
            m_strAditionalWhere = Value
        End Set
    End Property

    Public Property SQL() As String
        Get
            Return m_strSQL
        End Get
        Set(ByVal Value As String)
            m_strSQL = Value
        End Set
    End Property
    Public Property row() As Object
        Get
            Return mRow
        End Get
        Set(ByVal Value As Object)
            mRow = Value
        End Set
    End Property


    Public Property handled() As Boolean
        Get
            Return m_blnHandled

        End Get
        Set(ByVal Value As Boolean)
            m_blnHandled = Value
        End Set
    End Property


End Class


Public Class LookupFindEventArgs
    Inherits EventArgs

    Private m_blnHandled As Boolean = False
    Private m_objRow As Object
    Private m_strSQL As String
    Private m_strAditionalWhere As String
    Private m_blnHandleFindParamsValues As Boolean = False
    Private m_blnDataFound As Boolean = False
    Private m_blnHandledBuildWhere As Boolean = False

    Public Property handledBuildWhere() As Boolean
        Get
            Return m_blnHandledBuildWhere
        End Get
        Set(ByVal Value As Boolean)
            m_blnHandledBuildWhere = Value
        End Set
    End Property

    Public Property dataFound() As Boolean
        Get
            Return m_blnDataFound
        End Get
        Set(ByVal Value As Boolean)
            m_blnDataFound = Value
        End Set
    End Property

    Sub New()

    End Sub

    Private Property handleFindParamsValues() As Boolean
        Get
            Return m_blnHandleFindParamsValues
        End Get
        Set(ByVal value As Boolean)
            m_blnHandleFindParamsValues = value
        End Set
    End Property

    Public Property aditionalWhere() As String
        Get
            Return m_strAditionalWhere
        End Get
        Set(ByVal Value As String)
            m_strAditionalWhere = Value
        End Set
    End Property

    Public Property SQL() As String
        Get
            Return m_strSQL
        End Get
        Set(ByVal Value As String)
            m_strSQL = Value
        End Set
    End Property
    Public Property row() As Object
        Get
            Return m_objRow
        End Get
        Set(ByVal Value As Object)
            m_objRow = Value
        End Set
    End Property


    Public Property handled() As Boolean
        Get
            Return m_blnHandled

        End Get
        Set(ByVal Value As Boolean)
            m_blnHandled = Value
        End Set
    End Property


End Class

Public Class CreatingGridColumnsEventArgs
    Inherits EventArgs

    Public Grid As DataGrid
    Public TStyle As DataGridTableStyle
    Public Table As DataTable
    Public handled As Boolean

End Class

Public Class CheckedChangedEventArgs
    Inherits EventArgs

    Public Table As DataTable
    Public CheckStatus As Boolean

End Class

Public Class CreatedGridColumnsEventArgs
    Inherits EventArgs

    Public Grid As DataGrid
    Public TStyle As DataGridTableStyle
    Public Table As DataTable

End Class

