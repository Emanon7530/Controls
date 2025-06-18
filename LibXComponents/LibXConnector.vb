Imports System.ComponentModel
Imports LibXComponents.Data ' For EfDataHelper
Imports LibXComponents.Entities ' For User, GenericItem
Imports System.Collections.Generic ' For List(Of T)
Imports System.Threading.Tasks ' For Task

Public Enum LibxConnectionActions
    None = 0
    Add = 1
    Delete = 2
    Find = 3
    Edit = 4
    MoveNext = 5
    MovePrev = 6
    MoveFirst = 7
    MoveLast = 8
    Print = 9
    Accept = 10
    Cancel = 11
    [Exit] = 12
End Enum

Public Enum LibxConnectorState
    none = 0
    View = 1
    Insert = 2
    Edit = 3
    Query = 4
End Enum

Public Enum LibxInitModes
    none = 0
    Insert = 1
    Edit = 2
    Query = 4
    Print = 5
End Enum

Public Class LibXConnector
    Inherits XMsaComponents.XMsaConnector

#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

    Private mCloseButton As Boolean = False
    Private mSources As LibXDbSourceTableCollection
    Private mRequired As Hashtable
    Private mFillRequieredControls As Boolean

    Private mCurrAction As LibxConnectionActions
    Private mIsEditing As Boolean
    Private mRecordCount As Integer = 0
    Private mRequery As Boolean = False

    Dim mReportMode As Boolean = False

    Dim mAllowNew As Boolean = True
    Dim mAllowEdit As Boolean = True
    Dim mAllowDelete As Boolean = True
    Dim mAllowPrint As Boolean = True
    Dim mHasRecords As Boolean
    Dim mAllowQuery As Boolean = True
    Dim mRecordPosition As Integer
    Dim mRecordPositionOld As Integer
    Dim mRecordCountDetail As Integer
    Dim mAtBof As Boolean
    Dim mAtEof As Boolean
    Dim mIsHeaderOnGrid As Boolean
    Dim m_objFormControls As Hashtable
    Dim mUpdatePrimaryKeyColumns As Boolean = False
    Dim m_blnCanExecuteFind As Boolean = True
    Dim mShowWarningCancel As Boolean = True
    Dim mCheck As Boolean
    Dim mParams As LibxPrgParams
    Dim mReportName As String = ""
    Dim mModuleName As String = ""
    Dim mReport As LibX.ReportLib

    Private mUseTransactions As Boolean = True
    Private mHandledUpdates As Boolean
    Private mHandledRowsFill As Boolean
    Private mRequiredControllFailed As Boolean
    Private mRequiredControllFailedDet As Boolean
    Private mErroProv As ErrorProvider
    Private mErroProvDet As ErrorProvider
    Private mFirstBChanged As Boolean
    Private mLastQuery As String

    Private WithEvents mSaver As New LibX.Data.Adapter
    Private mState As LibxConnectorState

    Private mHasInitMode As Boolean
    Public Event InitingComboBoxes(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ExecutedAction(ByVal sender As Object, ByVal e As ExecutingActionEventArgs)
    Public Event ExecutingAction(ByVal sender As Object, ByVal e As ExecutingActionEventArgs)
    Public Event SettingDefaultNewValues(ByVal sender As Object, ByVal e As SettingDefaultNewValues)
    Public Event SettingDefaulteditValues(ByVal sender As Object, ByVal e As SettingDefaulteditValues)
    Public Event SettingDefaultqueryValues(ByVal sender As Object, ByVal e As SettingDefaultqueryValues)
    Public Event ValidatingControls(ByVal sender As Object, ByVal e As ValidatingControlsEventArgs)
    Public Event ValidatingRequieredControls(ByVal sender As Object, ByVal e As ValidatingRequieredControlsEventArgs)
    Public Event ValidatingRequieredControlsDetail(ByVal sender As Object, ByVal e As ValidatingRequieredControlsEventArgs)

    Public Event InsertingRow(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatingEventArgs)
    Public Event InsertedRow(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatedEventArgs)
    Public Event InsertingDetailRow(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatingEventArgs)
    Public Event InsertedDetailRow(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatedEventArgs)

    Public Event RowChange(ByVal sender As Object, ByVal e As XRowChangeEventArgs)
    Public Event AfterRowChange(ByVal sender As Object, ByVal e As XRowChangeEventArgs)
    Public Event ChangingState(ByVal sender As Object, ByVal e As XChangeStateEventArgs)
    Public Event ChangeState(ByVal sender As Object, ByVal e As XChangeStateEventArgs)
    Public Event InitSettings(ByVal sender As Object, ByVal e As EventArgs)
    Public Event UpdatingNavState(ByVal sender As Object, ByVal e As EventArgs)
    Public Event BeforeExecuteQuery(ByVal sender As Object, ByVal e As XBeforeExecuteQueryEventArgs)
    Public Event BeforeExecuteFill(ByVal sender As Object, ByVal e As XBeforeExecuteFillEventArgs)
    Public Event BeforeSaveDetail(ByVal sender As Object, ByVal e As XBeforeSaveDetailEventArgs)
    Public Event AfterSaveDetail(ByVal sender As Object, ByVal e As XBeforeSaveDetailEventArgs)
    Public Event BeforeLoadDetail(ByVal sender As Object, ByVal e As XbeforeLoadDetailEventArgs)
    Public Event AfterLoadDetail(ByVal sender As Object, ByVal e As XbeforeLoadDetailEventArgs)
    Public Event TransactionStarted(ByVal sender As Object, ByVal e As EventArgs)


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Property Sources() As LibXDbSourceTableCollection
        Get
            If mSources Is Nothing Then
                mSources = New LibXDbSourceTableCollection
                RemoveHandler mSources.ListChanged, AddressOf OnSourcesListChanged
                AddHandler mSources.ListChanged, AddressOf OnSourcesListChanged
            End If
            Return mSources
        End Get
        Set(ByVal Value As LibXDbSourceTableCollection)
            mSources = Value
        End Set
    End Property

    <Browsable(False)> _
    Public ReadOnly Property IsDataEditing() As Boolean
            Get
            If Me.mState = LibxConnectorState.Edit Or _
               Me.mState = LibxConnectorState.Insert Then
                    Return True
            End If
            Return False
        End Get
    End Property

    <Browsable(False)> _
    Public Property ReportObject() As LibX.ReportLib
        Get
            If mReport Is Nothing Then
                If mModuleName Is Nothing OrElse mModuleName.Trim = "" Then
                    Return Nothing
                End If

                If mReportName Is Nothing OrElse mReportName.Trim = "" Then
                    Return Nothing
                End If

                mReport = New LibX.ReportLib(mModuleName, mReportName)
            End If

            Return mReport
        End Get

        Set(ByVal value As LibX.ReportLib)
            mReport = value
        End Set
    End Property

    <Browsable(True)> _
    Public Property RequeryData() As Boolean
        Get
            Return mRequery
        End Get
        Set(ByVal Value As Boolean)
            mRequery = Value
        End Set
    End Property

    <Browsable(False)> _
    Public Property ShowWarningCancel() As Boolean
        Get
            Return mShowWarningCancel
        End Get
        Set(ByVal value As Boolean)
            mShowWarningCancel = value
        End Set
    End Property

    <Browsable(False)> _
    Public Property State() As LibxConnectorState
        Get
            Return mState
        End Get
        Set(ByVal Value As LibxConnectorState)
            mState = Value
        End Set
    End Property

    <Browsable(True)> _
    Public Property ReportName() As String
        Get
            Return mReportName
        End Get
        Set(ByVal value As String)
            mReportName = value
        End Set
    End Property

    <Browsable(True)> _
    Public Property ModuleName() As String
        Get
            Return mModuleName
        End Get
        Set(ByVal value As String)
            mModuleName = value
        End Set
    End Property

    <Browsable(True)> _
Public Property ReportMode() As Boolean
        Get
            Return mReportMode
        End Get
        Set(ByVal value As Boolean)
            mReportMode = value
        End Set
    End Property

    Public Overrides Sub RefreshSources()
        Dim oP As XMsaComponents.XConnectorProperties
        Dim Fd As Boolean
        Dim oT As LibXDbSourceTable
        Dim dt As DataTable
        Dim oTmp As Object

        Try

            If Me.mOnIni Then
                Exit Sub
            End If

            CheckConnection()

            Dim sSql As String
            If mSources Is Nothing Then
                Exit Sub
            End If

            Me.GetOrgDs()

            Dim oDs As DataSet = mDs

            For Each oT In Me.mSources

                If Trim(oT.TableName) <> "" Then
                    sSql = ""
                    If Not oT.Source Is Nothing AndAlso oT.Source.Length > 0 Then
                        sSql = String.Join(" ", oT.Source)
                    End If
                    dt = New DataTable(oT.TableName)


                    '-->Buscar en los dataSources
                    Fd = False
                    For Each oDs In mDats.Keys
                        oP = Nothing
                        oP = mDats(oDs)

                        If Not oP.Connector Is Nothing Then
                            If oDs.Tables.Contains(oT.TableName) Then
                                dt = oDs.Tables(oT.TableName)
                                Fd = True
                            Else
                                If oP.DefineAll Then '-->Si esto esta false, solo se agregan
                                    '   las tablas que no esten predefinidas
                                    If Me.DesignMode Then
                                        'If Me.Container.Components.Item(dt.TableName) Is Nothing Then
                                        Me.Container.Add(dt)
                                        'End If

                                        oDs.Tables.Add(dt)

                                    End If

                                    Fd = True
                                End If
                            End If
                            If Fd Then
                                LibX.Data.Manager.FillSchema(dt, oT.TableName, sSql, True)
                                If Trim(oT.KeyFields) <> "" Then
                                    LibX.Data.Manager.ApplayKeyString(dt, oT.KeyFields)
                                End If

                                If Me.DesignMode Then
                                    Dim oC As DataColumn
                                    For Each oC In dt.Columns
                                        'If Me.Container.Components.Item(oC.ColumnNam) Is Nothing Then
                                        Me.Container.Add(oC)
                                        'End If
                                    Next
                                End If
                            End If
                        End If
                    Next

                    '-->Siempre se agregan al dataSet interno si no existe.
                    oDs = mds
                    Dim ig As Boolean
                    If Not Me.mRealDs Is Nothing Then
                        If mRealDs.Equals(oDs) Then
                            '-->Ese dataset está como datasource,por lo que ya fue trabajao
                            ig = True
                        End If
                    End If
                    If Not ig Then
                        dt = New DataTable(oT.TableName)
                        If Not oDs.Tables.Contains(oT.TableName) Then
                            oDs.Tables.Add(dt)
                        End If
                        LibX.Data.Manager.FillSchema(dt, oT.TableName, sSql, True)
                        If Trim(oT.KeyFields) <> "" Then
                            LibX.Data.Manager.ApplayKeyString(dt, oT.KeyFields)
                        End If
                    End If

                End If '--

            Next
        Catch ex As Exception
            LibX.Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Private Sub OnOwnerBindingContextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If Not mFirstBChanged Then

                If Not Me.DesignMode Then

                    mFirstBChanged = True

                End If


            End If
        Catch ex As Exception
            LibX.Log.Show(ex)
        End Try
    End Sub

    Private Sub OnSourcesListChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Me.RefreshSources()

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Sub CheckConnection()
        Try
            LibX.Data.Manager.IsDesignMode = Me.DesignMode

            If Not LibX.Data.Manager.HasConnection Then
                LibX.Data.Manager.OpenConnection()

                If Not LibX.Data.Manager.IsAuthenticated Or App.ExecuteExit Then
                    Application.Exit()
                    Me.OwnerForm.Close()
                End If

            End If

            If Not Me.DesignMode Then
                RemoveHandler Me.OwnerForm.BindingContextChanged, AddressOf OnOwnerBindingContextChanged
                AddHandler Me.OwnerForm.BindingContextChanged, AddressOf OnOwnerBindingContextChanged

                RemoveHandler Me.OwnerForm.Closing, AddressOf OwnerFormClose
                AddHandler Me.OwnerForm.Closing, AddressOf OwnerFormClose
            End If


        Catch ex As Exception
            Log.Add(ex)

        End Try
    End Sub

    Protected Overrides Sub OnLoadForm()
        MyBase.OnLoadForm()

        If Not LibX.Data.Manager.IsAuthenticated Or App.ExecuteExit Then
            Application.Exit()
            Me.OwnerForm.Close()
        End If

        mCloseButton = False

        If Not Me.DesignMode Then
            If mReportMode = True Then
                Me.Find()
                Exit Sub
            End If

            If Not Me.Parameters Is Nothing Then
                If Me.Parameters.initMode = LibxInitModes.Insert Then
                    mHasInitMode = True
                    Me.AddNew()
                ElseIf Me.Parameters.initMode = LibxInitModes.Query Then
                    mHasInitMode = True
                    Me.Find()
                End If

                If Trim(Me.Parameters.WhereToExecute) <> "" Then
                    Me.ExecuteFind(Me.Parameters.WhereToExecute)
                End If

                If Me.Parameters.initMode = LibxInitModes.Edit Then
                    If Me.RecordCount > 0 Then
                        Me.Edit()
                    End If
                End If
            End If
        End If

    End Sub

    Public Function GetCM() As CurrencyManager
        Try
            Return GetCM("")

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Function GetDS() As Object
        If mRealDs Is Nothing Then
            Return Me
        Else
            Return mRealDs
        End If
    End Function

    Public Function GetCM(ByVal table As String) As CurrencyManager
        Try
            Dim tabName As String
            If Trim(table) = "" Then
                table = mdm
            End If
            tabName = table
            If Trim(tabName) = "" Then
                tabName = Me.mSources.Item(0).TableName
            End If
            Dim cm As CurrencyManager

            cm = Me.OwnerForm.BindingContext(GetDS, tabName)

            Return cm
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    <Browsable(False)> _
    Public Property IsEditing() As Boolean
        Get
            Return mIsEditing
        End Get
        Set(ByVal Value As Boolean)
            mIsEditing = Value
        End Set
    End Property

    Public Sub Edit()
        Dim CurrPos As Integer
        Try
            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            '// Refrescar los datos en pantalla
            CurrPos = mRecordPosition
            oArgs.Action = LibxConnectionActions.Edit
            mCurrAction = LibxConnectionActions.Edit
            mState = LibxConnectorState.Edit

            If Me.mRequery = True Then
                Me.ReQuery()
            End If

            '// Volver al registro actual
            MoveAbsolute(CurrPos)

            CheckDs()

            If Me.mDs.HasChanges Then
                mds.AcceptChanges()
            End If

            mRecordPositionOld = mRecordPosition

            oArgs.Action = LibxConnectionActions.Edit
            mCurrAction = LibxConnectionActions.Edit
            mState = LibxConnectorState.Edit

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            If Me.OwnerForm Is Nothing Then
                Exit Sub
            End If

            Me.mDs.EnforceConstraints = False

            Dim cm As CurrencyManager

            cm = GetCM()
            '-->cm.AddNew()

            mIsEditing = True

            oArgs2.row = CType(cm.Current, DataRowView)
            oArgs2.Action = LibxConnectionActions.Edit

            OnSettingDefaultEditValues(oArgs2.row)

            mCurrAction = LibxConnectionActions.Edit

            DoChangeState()

            RaiseEvent ExecutedAction(Me, oArgs2)

        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Sub

    Public Sub AcceptEdit()
        Dim oV As New ValidatingControlsEventArgs
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean

        Try

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Accept
            oArgs.AcceptedAction = LibxConnectionActions.Edit

            Dim cm As CurrencyManager = GetCM()
            cm.EndCurrentEdit()

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If


            ValidateControls(oV)

            If mRequiredControllFailed Then
                Exit Sub
            End If

            If mUseTransactions Then
                LibX.Data.Manager.Connection.BeginTransaction()

                RaiseEvent TransactionStarted(Me, New EventArgs)
            End If

            tabName = Me.GetMainTableName

            oDs = mds.GetChanges

            If Not oDs Is Nothing Then
                oTable = oDs.Tables(tabName)
            End If

            blnChanges = True
            If oTable Is Nothing Then
                blnChanges = False
            Else
                If oTable.Rows.Count = 0 Then
                    blnChanges = False
                End If
            End If

            If mHandledUpdates Then
                blnChanges = False
            End If

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            If blnChanges AndAlso Not oT.CustomDbUpdate Then
                Dim entitySavedSuccessfully As Boolean = False
                Dim recordsAffected As Integer = 0
                Dim pkValue As Object = Nothing ' To store PK for InsertedRow event
                Dim entityIsUser As Boolean = False

                If oTable IsNot Nothing AndAlso oTable.Rows.Count > 0 Then
                    Dim modifiedRow As DataRow = oTable.Rows(0)

                    ' Determine PK value from the original row version for finding the entity
                    Dim originalPkValue As Object = Nothing
                    If Not String.IsNullOrEmpty(sCol) AndAlso modifiedRow.Table.Columns.Contains(sCol) Then
                        originalPkValue = modifiedRow(sCol, DataRowVersion.Original)
                    ElseIf modifiedRow.Table.PrimaryKey.Length > 0 Then
                        originalPkValue = modifiedRow(modifiedRow.Table.PrimaryKey(0).ColumnName, DataRowVersion.Original)
                    Else
                        Log.Add("AcceptEdit: Cannot identify Primary Key for table " & tabName & " to update record.")
                    End If

                    pkValue = originalPkValue ' Store for InsertedRow event, assuming PK doesn't change

                    If originalPkValue IsNot Nothing AndAlso originalPkValue IsNot DBNull.Value Then
                        ' --- Simulate InsertingRow Event ---
                        Dim insertingArgs As New LibX.Data.AdpaterRowUpdatingEventArgs()
                        Dim fakeOleDbUpdatingArgs As System.Data.Common.RowUpdatingEventArgs = Nothing
                        Try
                            fakeOleDbUpdatingArgs = New System.Data.Common.RowUpdatingEventArgs(modifiedRow, Nothing, StatementType.Update, Nothing)
                        Catch exMockArgs As Exception
                            Log.Add("AcceptEdit: Could not create fake RowUpdatingEventArgs for InsertingRow. " & exMockArgs.Message)
                        End Try
                        insertingArgs.TableInfo = oT
                        ' insertingArgs.UpdatingArgs = fakeOleDbUpdatingArgs ' Problematic

                        RaiseEvent InsertingRow(Me, insertingArgs)

                        Dim skipSave As Boolean = insertingArgs.Handled
                        If Not skipSave AndAlso fakeOleDbUpdatingArgs IsNot Nothing AndAlso fakeOleDbUpdatingArgs.Status = UpdateStatus.SkipCurrentRow Then
                            skipSave = True
                        End If

                        If skipSave Then
                            If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                                LibX.Data.Manager.Connection.RollBackTransaction()
                            End If
                            mds.RejectChanges()
                            Return ' Exit Sub
                        End If

                        ' --- EF Core Save Operation ---
                        Try
                            Using context As New AppDbContext()
                                Dim foundEntity As Object = Nothing
                                If tabName.ToLower() = "scusers" Then
                                    entityIsUser = True
                                    Dim userIdToFind As String = Convert.ToString(originalPkValue)
                                    Dim userToUpdate = context.Users.Find(userIdToFind)
                                    If userToUpdate IsNot Nothing Then
                                        If modifiedRow.Table.Columns.Contains("UserName") Then userToUpdate.UserName = modifiedRow.Field(Of String)("UserName")
                                        If modifiedRow.Table.Columns.Contains("PasswordHash") Then userToUpdate.PasswordHash = modifiedRow.Field(Of String)("PasswordHash")
                                        If modifiedRow.Table.Columns.Contains("SucursalCode") Then userToUpdate.SucursalCode = modifiedRow.Field(Of Integer)("SucursalCode")
                                        If modifiedRow.Table.Columns.Contains("VendedorCode") Then userToUpdate.VendedorCode = modifiedRow.Field(Of Integer)("VendedorCode")
                                        ' TODO: Map other User properties
                                        foundEntity = userToUpdate
                                    Else
                                        Log.Add("AcceptEdit: User with ID " & userIdToFind & " not found for update.")
                                    End If
                                Else ' Default to GenericItem
                                    entityIsUser = False
                                    Dim itemIdToFind As Integer = Convert.ToInt32(originalPkValue)
                                    Dim itemToUpdate = context.GenericItems.Find(itemIdToFind)
                                    If itemToUpdate IsNot Nothing Then
                                        If modifiedRow.Table.Columns.Contains("Name") Then itemToUpdate.Name = modifiedRow.Field(Of String)("Name")
                                        If modifiedRow.Table.Columns.Contains("Description") Then itemToUpdate.Description = modifiedRow.Field(Of String)("Description")
                                        If modifiedRow.Table.Columns.Contains("ModifiedDate") Then itemToUpdate.ModifiedDate = DateTime.Now
                                        ' TODO: Map other GenericItem properties
                                        foundEntity = itemToUpdate
                                    Else
                                        Log.Add("AcceptEdit: GenericItem with ID " & itemIdToFind & " not found for update.")
                                    End If
                                End If

                                If foundEntity IsNot Nothing Then
                                    recordsAffected = context.SaveChanges().GetAwaiter().GetResult() ' TODO: Async
                                    entitySavedSuccessfully = (recordsAffected > 0)
                                End If
                            End Using
                        Catch exDb As Exception
                            Log.Add(exDb)
                            If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                                LibX.Data.Manager.Connection.RollBackTransaction()
                            End If
                            Throw
                        End Try
                    End If
                End If

                If entitySavedSuccessfully Then
                    ' --- Simulate InsertedRow Event ---
                    Dim updatedArgs As New LibX.Data.AdpaterRowUpdatedEventArgs()
                    Dim fakeOleDbUpdatedArgs As System.Data.Common.RowUpdatedEventArgs = Nothing
                    Try
                        fakeOleDbUpdatedArgs = New System.Data.Common.RowUpdatedEventArgs(modifiedRow, Nothing, StatementType.Update, recordsAffected)
                    Catch exMockArgs As Exception
                        Log.Add("AcceptEdit: Could not create fake RowUpdatedEventArgs for InsertedRow. " & exMockArgs.Message)
                    End Try
                    updatedArgs.TableInfo = oT
                    ' updatedArgs.UpdatingArgs = fakeOleDbUpdatedArgs ' If possible
                    If pkValue IsNot Nothing Then ' pkValue here is the original PK, used for identification
                       If TypeOf pkValue Is Integer Then updatedArgs.Serial = CInt(pkValue)
                       ElseIf TypeOf pkValue Is String Then Integer.TryParse(pkValue.ToString(), updatedArgs.Serial)
                       End If
                    End If
                    RaiseEvent InsertedRow(Me, updatedArgs)
                    blnChanges = False
                End If
            End If

            If Not blnChanges AndAlso Not mHandledUpdates Then
                ExecSaveDetail(CType(cm.Current, DataRowView).Row)

            End If

            If mUseTransactions And Not mHandledUpdates Then
                LibX.Data.Manager.Connection.CommitTransaction()
            End If

            If Not oTable Is Nothing And Not mHandledUpdates Then

                If mds.Tables(tabName).PrimaryKey.Length = 0 Then
                    Dim objrow As DataRow
                    Dim i As Integer = 0
                    mds.Tables(tabName).BeginLoadData()

                    For Each objrow In mds.Tables(tabName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To oTable.Columns.Count - 1
                                mds.Tables(tabName).Columns(j).ReadOnly = False
                                objrow(j) = oTable.Rows(i)(j)
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(tabName).EndLoadData()
                Else
                    Me.mDs.Merge(oTable)

                    Dim i As Integer
                    oTable = mDs.Tables(tabName)
                    For i = oTable.Rows.Count - 1 To 0 Step -1
                        If oTable.Rows(i).RowState = DataRowState.Added Then
                            oTable.Rows.RemoveAt(i)
                        End If
                    Next
                End If
            End If

            mds.AcceptChanges()

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View

            DoChangeState()

            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Edit

            RaiseEvent ExecutedAction(Me, oArgs2)

            mCurrAction = LibxConnectionActions.None
        Catch ex As Exception
            'Log.Add(ex, True)
            If Not oV Is Nothing Then
                If Not oV.FocusControl Is Nothing Then
                    oV.FocusControl.Focus()
                End If
            End If
            LibX.Log.Add(ex) '// CAMBIE

        End Try
    End Sub

    Public Sub Delete()
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean
        Dim drv As DataRowView

        Try

            '// Si esta editando or no hay registro en pantalla no hacer nada
            If mIsEditing Or mRecordCount = 0 Then
                Exit Sub
            End If

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Delete
            mCurrAction = LibxConnectionActions.Delete
            mState = LibxConnectorState.View

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If


            Dim cm As CurrencyManager
            cm = GetCM()

            '// Bloquear el registro antes de borrar
            If mUseTransactions Then
                LibX.Data.Manager.Connection.BeginTransaction()

                RaiseEvent TransactionStarted(Me, New EventArgs)
            End If

            If MessageBox.Show("Desea borrar el registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                mState = LibxConnectorState.View
                Exit Sub
            End If

            cm.RemoveAt(cm.Position)

            tabName = Me.GetMainTableName

            oDs = mds.GetChanges

            If Not oDs Is Nothing Then
                oTable = oDs.Tables(tabName)
            End If

            blnChanges = True
            If oTable Is Nothing Then
                blnChanges = False
            Else
                If oTable.Rows.Count = 0 Then
                    blnChanges = False
                End If
            End If

            If mHandledUpdates Then
                blnChanges = False
            End If

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName ' sCol might be used for PK identification if TableInfo is not fully detailed
            End If

            Dim entityDeletedSuccessfully As Boolean = False
            Dim pkValue As Object = Nothing
            Dim originalRowForDetailDelete As DataRow = drvToDelete.Row ' drvToDelete captured before cm.RemoveAt

            ' Determine PK value from the original row (before it was removed from CurrencyManager)
            If originalRowForDetailDelete IsNot Nothing Then
                If tabName.ToLower() = "scusers" Then
                    If originalRowForDetailDelete.Table.Columns.Contains("UserID") Then
                        pkValue = originalRowForDetailDelete.Field(Of String)("UserID")
                    End If
                Else ' Assuming GenericItem or other tables with "Id" as PK
                    If originalRowForDetailDelete.Table.Columns.Contains("Id") Then
                        pkValue = originalRowForDetailDelete.Field(Of Integer)("Id")
                    End If
                End If
                ' TODO: Need a more robust way to get PK column name and type, perhaps from oT.KeyFields
                If pkValue Is Nothing AndNot String.IsNullOrEmpty(sCol) AndAlso originalRowForDetailDelete.Table.Columns.Contains(sCol) Then
                     pkValue = originalRowForDetailDelete(sCol) ' Fallback to sCol if specific PK logic above fails
                End If
            End If

            ' The original code determined blnChanges AFTER cm.RemoveAt by looking at mds.GetChanges.
            ' If oTable (from mds.GetChanges) is not Nothing and has rows, it means the row was marked as Deleted.
            If blnChanges AndAlso Not oT.CustomDbUpdate Then
                If pkValue IsNot Nothing AndAlso pkValue IsNot DBNull.Value Then
                    Dim recordsAffected As Integer = 0
                    Dim entityToDeleteForEvent As Object = Nothing ' Used to hold a temporary entity for event args if needed

                    ' --- Simulate InsertingRow Event ---
                    Dim insertingArgs As New LibX.Data.AdpaterRowUpdatingEventArgs()
                    Dim fakeOleDbUpdatingArgs As System.Data.Common.RowUpdatingEventArgs = Nothing
                    Try
                        ' originalRowForDetailDelete is the DataRow before cm.RemoveAt
                        fakeOleDbUpdatingArgs = New System.Data.Common.RowUpdatingEventArgs(originalRowForDetailDelete, Nothing, StatementType.Delete, Nothing)
                    Catch exMockArgs As Exception
                        Log.Add("Delete: Could not create fake RowUpdatingEventArgs for InsertingRow. " & exMockArgs.Message)
                    End Try
                    insertingArgs.TableInfo = oT
                    ' insertingArgs.UpdatingArgs = fakeOleDbUpdatingArgs ' Problematic due to type

                    RaiseEvent InsertingRow(Me, insertingArgs)

                    Dim skipSave As Boolean = insertingArgs.Handled
                    If Not skipSave AndAlso fakeOleDbUpdatingArgs IsNot Nothing AndAlso fakeOleDbUpdatingArgs.Status = UpdateStatus.SkipCurrentRow Then
                        skipSave = True
                    End If

                    If skipSave Then
                        If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                            LibX.Data.Manager.Connection.RollBackTransaction()
                        End If
                        mds.RejectChanges() ' Revert cm.RemoveAt
                        Return ' Exit Sub
                    End If

                    ' --- EF Core Delete Operation ---
                    Try
                        Using context As New AppDbContext()
                            If tabName.ToLower() = "scusers" Then
                                Dim userToDelete = context.Users.Find(Convert.ToString(pkValue))
                                If userToDelete IsNot Nothing Then
                                    context.Users.Remove(userToDelete)
                                    recordsAffected = context.SaveChanges().GetAwaiter().GetResult() ' TODO: Convert Delete to Async
                                    entityDeletedSuccessfully = (recordsAffected > 0)
                                End If
                            Else ' Default to GenericItem
                                Dim itemToDelete = context.GenericItems.Find(Convert.ToInt32(pkValue))
                                If itemToDelete IsNot Nothing Then
                                    context.GenericItems.Remove(itemToDelete)
                                    recordsAffected = context.SaveChanges().GetAwaiter().GetResult() ' TODO: Convert Delete to Async
                                    entityDeletedSuccessfully = (recordsAffected > 0)
                                End If
                            End If
                        End Using

                        If Not entityDeletedSuccessfully Then
                             Log.Add("Delete: Entity with PK " & pkValue.ToString() & " for table " & tabName & " not found or DB save failed.")
                        Else
                             blnChanges = False ' EF Core handled the delete
                        End If
                    Catch exDb As Exception
                        Log.Add(exDb)
                        If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                            LibX.Data.Manager.Connection.RollBackTransaction()
                        End If
                        mds.RejectChanges()
                        Throw
                    End Try
                Else
                    Log.Add("Delete: Primary Key not found for table " & tabName & ". Cannot delete via EF Core.")
                    If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                         LibX.Data.Manager.Connection.RollBackTransaction()
                    End If
                    mds.RejectChanges()
                    Return
                End If
            End If

            ' --- Simulate InsertedRow Event (if EF Core delete was successful) ---
            If entityDeletedSuccessfully Then
                Dim insertedArgs As New LibX.Data.AdpaterRowUpdatedEventArgs()
                Dim fakeOleDbUpdatedArgs As System.Data.Common.RowUpdatedEventArgs = Nothing
                Try
                    fakeOleDbUpdatedArgs = New System.Data.Common.RowUpdatedEventArgs(originalRowForDetailDelete, Nothing, StatementType.Delete, recordsAffected)
                Catch exMockArgs As Exception
                    Log.Add("Delete: Could not create fake RowUpdatedEventArgs for InsertedRow. " & exMockArgs.Message)
                End Try
                insertedArgs.TableInfo = oT
                ' insertedArgs.UpdatingArgs = fakeOleDbUpdatedArgs ' If possible
                If pkValue IsNot Nothing Then
                   If TypeOf pkValue Is Integer Then insertedArgs.Serial = CInt(pkValue)
                   ElseIf TypeOf pkValue Is String Then Integer.TryParse(pkValue.ToString(), insertedArgs.Serial)
                   End If
                End If
                RaiseEvent InsertedRow(Me, insertedArgs)
            End If

            If originalRowForDetailDelete IsNot Nothing Then
                 ExecDeleteDetail(originalRowForDetailDelete)
            End If

            If mUseTransactions AndAlso Not mHandledUpdates Then
                If entityDeletedSuccessfully OrElse oT.CustomDbUpdate Then ' If EF delete or custom update happened
                    LibX.Data.Manager.Connection.CommitTransaction()
                Else
                    If LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                        LibX.Data.Manager.Connection.RollBackTransaction()
                    End If
                    mds.RejectChanges()
                    Return
                End If
            End If

            If entityDeletedSuccessfully OrElse oT.CustomDbUpdate Then
                 mds.AcceptChanges() ' Finalize UI change if DB operation was successful
            Else
                 mds.RejectChanges()
            End If
            ' The following block for merging was specific to how updates/inserts might affect a separate 'oTable'.
            ' For deletes, mds.AcceptChanges() or mds.RejectChanges() above should handle the state of the main DataTable.
            ' If Not oTable Is Nothing And Not mHandledUpdates Then

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View
            mRecordPosition = cm.Position
            mRecordCount = mDS.Tables(tabName).Rows.Count

            DoChangeState()

            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Delete

            OnRowChange(True)

            RaiseEvent ExecutedAction(Me, oArgs2)
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Public Sub MoveAbsolute(ByVal Position As Integer)
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            cm.Position = Position

            RefreshStates()

            OnRowChange()

            NavChangeState()

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub


    Public Sub MoveNext()
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            cm.Position += 1

            RefreshStates()

            OnRowChange()

            NavChangeState()

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Public Property IsHeaderOnGrid() As Boolean
        Get
            Return mIsHeaderOnGrid
        End Get
        Set(ByVal Value As Boolean)
            mIsHeaderOnGrid = Value
        End Set
    End Property

    Private Sub OnRowChange(Optional ByVal afterQuery As Boolean = False)
        Try
            Dim oa As New XRowChangeEventArgs

            oa.FistRowChangeAfterQuery = afterQuery

            Dim cm As CurrencyManager
            cm = GetCM()

            Dim oRow As DataRow
            If cm.Position >= 0 AndAlso Not cm.Current Is Nothing Then
                If TypeOf cm.Current Is DataRowView Then
                    oRow = CType(cm.Current, DataRowView).Row
                End If
                oa.row = oRow
            End If

            RaiseEvent RowChange(Me, oa)

            LoadDetail()

            RaiseEvent AfterRowChange(Me, oa)

        Catch ex As Exception
            LibX.Log.Add(ex)
        End Try
    End Sub

    Private Sub RefreshStates()
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            If cm Is Nothing Then
                mRecordPosition = -1
                mRecordCount = 0

            Else
                If mState <> LibxConnectorState.Query Then

                    If cm.Count > 0 Then
                        mRecordPosition = cm.Position
                    End If

                End If

            End If

            mHasRecords = False
            If mRecordCount > 0 Then
                mHasRecords = True
            End If

            mAtEof = True
            If mHasRecords And mRecordPosition < mRecordCount Then
                mAtEof = False
            End If

            mAtBof = True
            If mRecordPosition > 0 Then
                mAtBof = False
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub


    Public Sub MovePrevious()
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            cm.Position -= 1

            RefreshStates()

            OnRowChange()

            NavChangeState()

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try

    End Sub

    Public Sub MoveLast()
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            cm.Position = cm.Count - 1

            RefreshStates()

            OnRowChange()

            NavChangeState()

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try

    End Sub

    Public Sub MoveFirst()
        Try
            Dim cm As CurrencyManager

            cm = GetCM()

            cm.Position = 0

            RefreshStates()

            OnRowChange()

            NavChangeState()

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub


    Private Sub CheckDs()
        Try
            If mCheck Then
                Exit Sub
            End If
            Dim t As LibXDbSourceTable
            For Each t In Me.mSources
                '''if mds.Tables(t.TableName).Columns.contains(t.SerialColumnName)=false then
                '''    throw new applicationException("No existe la columna serial " & t.SerialColumnName & " en la tabla " & t.TableName)
                '''end if

                If Trim(t.SerialColumnName) <> "" And Not t.AutoIncrementSerial Then
                    mds.Tables(t.TableName).Columns(t.SerialColumnName).ReadOnly = False
                    mds.Tables(t.TableName).Columns(t.SerialColumnName).AutoIncrement = False
                End If
            Next
            mCheck = True
        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Sub

    Public Sub Find()
        Try
            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            CheckDs()

            mRecordPositionOld = mRecordPosition

            oArgs.Action = LibxConnectionActions.Find
            mCurrAction = LibxConnectionActions.Find
            mState = LibxConnectorState.Query

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            If Me.OwnerForm Is Nothing Then
                Exit Sub
            End If

            For Each oT As LibXDbSourceTable In mSources
                Me.mDs.Tables(oT.TableName).Rows.Clear()
            Next

            Me.mDs.EnforceConstraints = False
            Dim cm As CurrencyManager

            cm = GetCM()

            Dim sTable As String = Me.GetMainTableName
            Dim blnBeforeRowIsChanged As Boolean
            Dim intPos As Integer = cm.List.Count
            If intPos > 0 Then
                intPos = cm.Position
                If mds.Tables(sTable).Rows.Count > 0 Then
                    If mds.Tables(sTable).Rows(intPos).RowState <> DataRowState.Unchanged Then
                        blnBeforeRowIsChanged = True
                    End If
                End If
            End If

            cm.AddNew()

            If cm.List.Count > 1 And Not blnBeforeRowIsChanged Then
                If mds.Tables(sTable).Rows.Count > 0 Then
                    If mds.Tables(sTable).Rows(intPos).RowState <> DataRowState.Unchanged Then
                        mds.Tables(sTable).Rows(intPos).RejectChanges()
                    End If
                End If
            End If

            mIsEditing = True

            oArgs2.row = CType(cm.Current, DataRowView)
            oArgs2.Action = LibxConnectionActions.Find

            ClearDetail()

            Me.OnSettingDefaultQueryBalues(oArgs2.row)

            DoChangeState()

            RaiseEvent ExecutedAction(Me, oArgs2)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub Cancel()
        Dim cm As CurrencyManager

        Try
            Dim oArgs As New ExecutingActionEventArgs

            If mShowWarningCancel = True Then
                If MessageBox.Show("Seguro desea cancelar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                    mState = LibxConnectorState.View
                    Exit Sub
                End If
            End If

            oArgs.AcceptedAction = Me.mCurrAction
            mCurrAction = LibxConnectionActions.Cancel
            oArgs.Action = LibxConnectionActions.Cancel

            RaiseEvent ExecutingAction(Me, oArgs)

            cm = GetCM()
            cm.CancelCurrentEdit()

            Me.IsEditing = False

            mds.RejectChanges()

            If mState = LibxConnectorState.Insert Then
                If cm.Count > 0 Then
                    cm.Position = mRecordPositionOld
                End If
            End If

            mState = LibxConnectorState.View

            DoChangeState()

            OnRowChange()

            If Not mRequired Is Nothing And Not mErroProv Is Nothing Then
                Dim oc As Control
                For Each oc In Me.mRequired.Values
                    mErroProv.SetError(oc, "")
                Next
            End If

        Catch ex As Exception
            Log.Add(ex)     '// CAMBIE
        End Try

    End Sub

    Public ReadOnly Property CurrentAction() As LibxConnectionActions
        Get
            Return mCurrAction
        End Get
    End Property

    <Browsable(False)> _
    Public Property CanExecuteFind() As Boolean
        Get
            Return m_blnCanExecuteFind
        End Get
        Set(ByVal Value As Boolean)
            m_blnCanExecuteFind = Value
        End Set
    End Property

    Public Function GetTable(ByVal tabname As String)
        If Trim(tabname) = "" Then
            Return mds.Tables(0)
        Else
            Return mds.Tables(tabname)
        End If
    End Function

    Public Sub AddNew()
        Dim cm As CurrencyManager

        Try
            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            ''//Limpiar del dataset para para evitar que trate de Update cuando se esta addnew
            CType(Me.GetDS, DataSet).Clear()
         
            CheckDs()

            If Me.mDs.HasChanges Then
                mds.AcceptChanges()
            End If

            mRecordPositionOld = mRecordPosition

            oArgs.Action = LibxConnectionActions.Add
            mCurrAction = LibxConnectionActions.Add
            mState = LibxConnectorState.Insert

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            If Me.OwnerForm Is Nothing Then
                Exit Sub
            End If

            Me.mDs.EnforceConstraints = False

            cm = GetCM()

            Dim sTable As String = Me.GetMainTableName
            Dim blnBeforeRowIsChanged As Boolean
            Dim intPos As Integer = cm.List.Count
            If intPos > 0 Then
                intPos = cm.Position
                If mds.Tables(sTable).Rows.Count > 0 Then
                    If mds.Tables(sTable).Rows(intPos).RowState <> DataRowState.Unchanged Then
                        blnBeforeRowIsChanged = True
                    End If
                End If
            End If

            cm.AddNew()

            If cm.List.Count > 1 And Not blnBeforeRowIsChanged Then
                If mds.Tables(sTable).Rows.Count > 0 Then
                    If mds.Tables(sTable).Rows(intPos).RowState <> DataRowState.Unchanged Then
                        mds.Tables(sTable).Rows(intPos).RejectChanges()
                    End If
                End If
            End If


            mIsEditing = True

            oArgs2.row = CType(cm.Current, DataRowView)
            oArgs2.Action = LibxConnectionActions.Add

            ClearDetail()

            DoChangeState()

            OnSettingDefaultNewValues(oArgs2.row)

            RaiseEvent ExecutedAction(Me, oArgs2)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub ClearDetail(ByVal TableName As String)
        Try
            '-->Iniciar los details
            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            For Each oT In mSources
                If oT.IsDetail And oT.TableName.ToLower = TableName.ToLower Then
                    mds.Tables(oT.TableName).Rows.Clear()
                    Exit For
                End If
            Next


        Catch ex As Exception
            Log.Show(ex)
        End Try

    End Sub
    Public Sub ClearDetail()
        Try
            '-->Iniciar los details
            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            For Each oT In mSources
                If oT.IsDetail Then
                    mds.Tables(oT.TableName).Rows.Clear()
                End If
            Next

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub
    Public Sub [Exit]()
        If mParams Is Nothing OrElse mParams.IsFromOther = False Then
            If MessageBox.Show("Seguro desea cerrar esta aplicación?", "Cerrar Aplicación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If
        End If

        mCloseButton = True
        Me.OwnerForm.Close()
    End Sub

    <Browsable(False)> _
    Public Property RecordCount() As Integer
        Get
            Return mRecordCount
        End Get
        Set(ByVal Value As Integer)
            mRecordCount = Value
        End Set
    End Property

    Public Property AllowNew() As Boolean
        Get
            Return mAllowNew
        End Get
        Set(ByVal Value As Boolean)
            mAllowNew = Value
        End Set
    End Property


    Public Property AllowEdit() As Boolean
        Get
            Return mAllowEdit
        End Get
        Set(ByVal Value As Boolean)
            mAllowEdit = Value
        End Set
    End Property


    Public Property AllowDelete() As Boolean
        Get
            Return mAllowDelete
        End Get
        Set(ByVal Value As Boolean)
            mAllowDelete = Value
        End Set
    End Property


    Public Property AllowPrint() As Boolean
        Get
            Return mAllowPrint
        End Get
        Set(ByVal Value As Boolean)
            mAllowPrint = Value
        End Set
    End Property


    <Browsable(False)> _
    Public Property HasRecords() As Boolean
        Get
            Return mHasRecords
        End Get
        Set(ByVal Value As Boolean)
            mHasRecords = Value
        End Set
    End Property

    <Browsable(False)> _
    Public Property Parameters() As LibxPrgParams
        Get
            If Not LibX.App.CurrentPrgParams Is Nothing Then
                Me.mParams = LibX.App.CurrentPrgParams.Clone
                LibX.App.CurrentPrgParams = Nothing
            End If
            Return mParams
        End Get
        Set(ByVal Value As LibxPrgParams)
            mParams = Value
        End Set
    End Property



    Public Property AllowQuery() As Boolean
        Get
            Return mAllowQuery
        End Get
        Set(ByVal Value As Boolean)
            mAllowQuery = Value
        End Set
    End Property

    <Browsable(False)> _
   Public Property EOF() As Boolean
        Get
            Return mAtEof
        End Get
        Set(ByVal Value As Boolean)
            mAtEof = Value
        End Set
    End Property

    <Browsable(False)> _
    Public Property BOF() As Boolean
        Get
            Return mAtBof
        End Get
        Set(ByVal Value As Boolean)
            mAtBof = Value
        End Set
    End Property

    Public Function RecordPosition() As Integer
        Return mRecordPosition
    End Function

    Public Function RecordCountDetail() As Integer
        Return mRecordCountDetail
    End Function

    Public Sub NavChangeState()
        RaiseEvent UpdatingNavState(Me, New EventArgs)
    End Sub

    Public Sub DoChangeState()
        Try
            If Me.DesignMode Then
                Exit Sub
            End If

            Me.RefreshStates()

            NavChangeState()

            Dim oA As New XChangeStateEventArgs
            oA.action = CurrentAction
            oA.isEditing = mIsEditing
            oA.isDataEditing = IsDataEditing
            oA.state = mState

            RaiseEvent ChangingState(Me, oA)

            RaiseEvent ChangeState(Me, oA)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Property HandledRowsFill() As Boolean
        Get
            Return mHandledRowsFill
        End Get
        Set(ByVal Value As Boolean)
            mHandledRowsFill = Value
        End Set
    End Property

    Public Property HandledUpdates() As Boolean
        Get
            Return mHandledUpdates
        End Get
        Set(ByVal Value As Boolean)
            mHandledUpdates = Value
        End Set
    End Property

    Public Sub ValidateControls(ByVal o As ValidatingControlsEventArgs)
        Try

            RaiseEvent ValidatingControls(Me, o)

            ValidatingRequierdControls(o)


        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub AddRequired(ByVal ctrl As Control)

        If mRequired Is Nothing Then
            mRequired = New Hashtable
        End If

        If Not mRequired.ContainsKey(ctrl.Name.ToLower) Then
            mRequired.Add(ctrl.Name.ToLower, ctrl)
        End If


    End Sub

    Private Sub FillRequieredControls()
        Try
            If mFillRequieredControls Then
                Exit Sub
            End If

            Dim cm As CurrencyManager
            cm = GetCM()

            Dim oBind As Binding
            Dim sField As String
            Dim ds As DataSet = mds

            For Each oBind In cm.Bindings
                sField = oBind.BindingMemberInfo.BindingField
                If Not ds.Tables(oBind.BindingMemberInfo.BindingPath).Columns(sField).AllowDBNull Then
                    Me.AddRequired(oBind.Control)
                End If
            Next


            mFillRequieredControls = True
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try

    End Sub
    Private Sub _BindingObject_Parse(ByVal sender As Object, ByVal e As System.Windows.Forms.ConvertEventArgs)
        If e.Value.ToString.Trim = "" Then
            e.Value = DBNull.Value
            Return
        End If

    End Sub

    Private Sub _BindingObject_Format(ByVal sender As Object, ByVal e As System.Windows.Forms.ConvertEventArgs)
        If e.Value.ToString.Trim = "" Then
            e.Value = DBNull.Value
            Return
        End If

        If e.Value.GetType Is GetType(Decimal) Then
            e.Value = CType(e.Value, Decimal).ToString("###,###,##0.00")
        End If

    End Sub

    Public Sub ValidatingRequierdControls(ByVal o As ValidatingControlsEventArgs)
        Try
            Dim oA As New ValidatingRequieredControlsEventArgs

            mRequiredControllFailed = False
            mRequiredControllFailedDet = False

            If mErroProv Is Nothing Then
                mErroProv = New ErrorProvider
                mErroProv.ContainerControl = Me.OwnerForm
            End If


            FillRequieredControls()

            Dim oc As Control
            Dim nulo As Boolean
            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String

            If Me.mRequired Is Nothing Then
                Exit Sub
            End If

            For Each oc In Me.mRequired.Values
                mErroProv.SetError(oc, "")

                oA = New ValidatingRequieredControlsEventArgs
                oA.ValidatedControl = oc
                RaiseEvent ValidatingRequieredControls(Me, oA)


                If oc.DataBindings.Count > 0 Then
                    oT = Me.mSources.Item(oc.DataBindings(0).BindingMemberInfo.BindingPath)
                    If Not oT Is Nothing Then
                        sCol = oT.SerialColumnName
                        '-->No validar si es un serial
                        If Trim(sCol).ToLower = oc.DataBindings(0).BindingMemberInfo.BindingField.ToLower Then
                            oA.Ignore = True
                        End If
                    End If
                End If

                If TypeOf oc Is IEditProperty Then
                    If CType(oc, IEditProperty).IgnoreRequiered Then
                        oA.Ignore = True
                    End If
                End If

                If Not oA.Ignore Then
                    nulo = False

                    If TypeOf oc Is TextBox Then
                        If Trim(oc.Text) = "" Then
                            nulo = True
                        End If
                    End If
                    If TypeOf oc Is LibxDateTimePicker Then
                        If IsNull(CType(oc, LibxDateTimePicker).Value) Then
                            nulo = True
                        End If
                    End If
                    If TypeOf oc Is LibXCombo Then
                        If IsNull(CType(oc, LibXCombo).currValue) Then
                            nulo = True
                        End If
                    End If
                    If nulo Then
                        mErroProv.SetError(oc, "Este control necesita la entra de un valor")
                        mRequiredControllFailed = True
                    End If
                End If
            Next


            DoValidatingRequierdControlsDetail()

            If mRequiredControllFailedDet Then
                mRequiredControllFailed = True
            End If

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub


    Public Sub DoValidatingRequierdControlsDetail()
        Try
            Dim ot As LibXDbSourceTable
            Dim oRow As DataRow
            Dim cm As CurrencyManager
            Dim oTable As DataTable
            Dim oDs As DataSet

            '-->Por ahora **********************************
            Exit Sub


            oDs = mds.GetChanges

            For Each ot In Me.Sources
                If ot.IsDetail Then

                    cm = Me.GetCM(ot.TableName)
                    cm.EndCurrentEdit()

                    If Not oDs Is Nothing Then
                        oTable = oDs.Tables(ot.TableName)
                    End If

                    If Not oTable Is Nothing AndAlso oTable.Rows.Count > 0 Then
                        ValidatingRequierdControlsDetail(ot.TableName, oTable, ot)
                    End If

                    If Me.mRequiredControllFailedDet Then
                        Exit Sub
                    End If


                End If
            Next
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Private Sub ValidatingRequierdControlsDetail(ByVal table As String, ByVal dtable As DataTable, ByVal oT As LibX.LibXDbSourceTable)
        Try
            Dim oA As New ValidatingRequieredControlsEventArgs

            oA.isDetail = True


            If mErroProvDet Is Nothing Then
                mErroProvDet = New ErrorProvider
                mErroProvDet.ContainerControl = Me.OwnerForm
            End If

            mErroProvDet.BindToDataAndErrors(mds, table)

            Dim oRow As DataRow
            Dim sCol As String
            Dim nulo As Boolean
            Dim oC As DataColumn

            For Each oRow In dtable.Rows

                oA = New ValidatingRequieredControlsEventArgs
                oA.isDetail = True
                RaiseEvent ValidatingRequieredControlsDetail(Me, oA)


                For Each oC In dtable.Columns

                    If Not oT Is Nothing Then
                        sCol = oT.SerialColumnName
                        '-->No validar si es un serial
                        If Trim(sCol).ToLower = oC.ColumnName.ToLower Then
                            oA.Ignore = True
                        End If
                    End If

                    If oC.AllowDBNull Then
                        oA.Ignore = True
                    End If

                    If Not oA.Ignore Then
                        nulo = False

                        Dim oValue As Object

                        oValue = ""
                        oValue = oRow(oC.ColumnName)

                        If IsNull(oValue) Then
                            nulo = True
                        End If

                        If nulo Then
                            If Not oRow Is Nothing Then
                                oRow.SetColumnError(oC.ColumnName, "Este campo necesita la entrada de un valor")
                                mRequiredControllFailedDet = True
                                oRow.RowError = "Campos nulos"
                            End If
                        End If

                    End If
                Next
            Next


        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Sub

    Public Sub LoadDetail()
        Try

            Dim cm As CurrencyManager
            cm = GetCM()

            Dim oRow As DataRow
            If cm.Position >= 0 AndAlso Not cm.Current Is Nothing Then
                If TypeOf cm.Current Is DataRowView Then
                    oRow = CType(cm.Current, DataRowView).Row
                End If
            End If

            Dim oInfo As LibXDbSourceTable
            For Each oInfo In Me.mSources
                If oInfo.FillOnRowChange And oInfo.IsDetail Then
                    If oInfo.MasterTableName <> "" Then
                        cm = GetCM(oInfo.MasterTableName)
                        If cm.Position >= 0 AndAlso Not cm.Current Is Nothing Then
                            If TypeOf cm.Current Is DataRowView Then
                                oRow = CType(cm.Current, DataRowView).Row
                            End If
                            LoadDetail(oRow, oInfo)
                        End If

                    Else
                        LoadDetail(oRow, oInfo)
                    End If

                End If
            Next


        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub


    Public Sub LoadDetail(ByVal masterRow As DataRow, ByVal table As LibXDbSourceTable)
        Dim objTable As DataTable
        Dim sSql As String
        Dim strWhereJoin As String
        Dim args As New XbeforeLoadDetailEventArgs
        Dim strTmp As String
        Dim strWhere As String

        Try
            m_blnCanExecuteFind = False

            objTable = mds.Tables(table.TableName)

            objTable.Rows.Clear()

            If masterRow Is Nothing Then
                m_blnCanExecuteFind = True
                Exit Sub
            End If

            sSql = String.Concat("Select * From ", table.TableName)
            If Not table.Source Is Nothing AndAlso table.Source.Length > 0 Then
                sSql = Join(table.Source, " ")
            End If

            If Not table.MasterDetailRelation Is Nothing Then
                Dim s As String
                Dim f() As String
                Dim sColName As String
                Dim oType As Type
                For Each s In table.MasterDetailRelation
                    If Trim(s) <> "" Then
                        f = Split(s, "=")
                        sColName = GetFullColumnName(f(0), sSql, table.TableName)
                        oType = objTable.Columns(f(0)).DataType
                        strTmp = BuildCondition(sColName, oType, masterRow(f(1)), True)
                        If Trim(strTmp) <> "" Then
                            If Trim(strWhereJoin) = "" Then
                                strWhereJoin = strTmp
                            Else
                                strWhereJoin = Trim(strWhereJoin) & " and " & strTmp
                            End If
                        End If
                    End If
                Next
            End If


            args.Sql = sSql
            args.WhereJoin = strWhereJoin
            args.MasterRow = masterRow

            RaiseEvent BeforeLoadDetail(Me, args)

            If args.Handled Then
                Exit Sub
            End If

            '*--> Si no se usa el load normal se puede agregar un where adicional
            '*    o modificar el que hay
            If args.ConcatenateWhereJoin Then
                strTmp = args.WhereJoin
                strWhere = getWherePart(args.Sql)
                If Trim(strWhere) <> "" And Trim(strTmp) <> "" Then
                    strTmp = strWhere & " and " & strTmp
                End If
                If Trim(args.AditionalWhere) <> "" Then
                    strTmp = strTmp & " and " & args.AditionalWhere
                End If
                args.Sql = ReplaceWherePart(args.Sql, strTmp)
            End If

            Dim oAd As New OleDb.OleDbDataAdapter(args.Sql, LibX.Data.Manager.Connection.ConnectionObject)


            If table.UseRowRetrieve Then
                LibX.Data.Manager.Connection.UseCopyConnection = True
                RemoveHandler objTable.RowChanged, AddressOf OnDetailRowChanged
                AddHandler objTable.RowChanged, AddressOf OnDetailRowChanged
            End If

            If LibX.Data.Manager.Connection.IsIntransaction = True Then
                oAd.SelectCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction
            End If

            oAd.Fill(objTable)


            If Trim(table.Sort) <> "" Then
                objTable.DefaultView.Sort = table.Sort
            End If

            If table.UseRowRetrieve Then
                LibX.Data.Manager.Connection.UseCopyConnection = False
                RemoveHandler objTable.RowChanged, AddressOf OnDetailRowChanged
            End If

            m_blnCanExecuteFind = True

            If Not Me.OwnerForm Is Nothing Then
                'Dim objCurr As CurrencyManager
                'objCurr =  
                'If Not objCurr Is Nothing Then
                '    RemoveHandler objCurr.PositionChanged, AddressOf DetailPositionChanged
                '    AddHandler objCurr.PositionChanged, AddressOf DetailPositionChanged
                'End If
            End If


            RaiseEvent AfterLoadDetail(Me, args)

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Private Sub OnDetailRowChanged(ByVal sender As Object, ByVal e As DataRowChangeEventArgs)

    End Sub

    Public Function GetMainTableName() As String
        Dim tabName As String
        tabName = Me.mDm
        If Trim(tabName) = "" Then
            tabName = Me.mSources.Item(0).TableName
        End If
        Return tabName
    End Function

    Public Property UseTransactions() As Boolean
        Get
            Return mUseTransactions
        End Get
        Set(ByVal Value As Boolean)
            mUseTransactions = Value
        End Set
    End Property

    Public Sub AcceptNew()
        Dim oV As New ValidatingControlsEventArgs
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean

        Try

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Accept
            oArgs.AcceptedAction = LibxConnectionActions.Add

            Dim cm As CurrencyManager = GetCM()
            cm.EndCurrentEdit()

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If


            ValidateControls(oV)

            If mRequiredControllFailed Then
                Exit Sub
            End If

            If mUseTransactions Then
                LibX.Data.Manager.Connection.BeginTransaction()

                RaiseEvent TransactionStarted(Me, New EventArgs)
            End If

            tabName = Me.GetMainTableName

            oDs = mds.GetChanges

            If Not oDs Is Nothing Then
                oTable = oDs.Tables(tabName)
            End If

            blnChanges = True
            If oTable Is Nothing Then
                blnChanges = False
            Else
                If oTable.Rows.Count = 0 Then
                    blnChanges = False
                End If
            End If

            If mHandledUpdates Then
                blnChanges = False
            End If

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            If blnChanges = True AndAlso oT.CustomDbUpdate = False Then
                Dim entitySavedSuccessfully As Boolean = False
                Dim newEntityPrimaryKey As Object = Nothing
                Dim recordsAffected As Integer = 0

                If oTable IsNot Nothing AndAlso oTable.Rows.Count > 0 Then
                    Dim newRowToSave As DataRow = oTable.Rows(0)
                    Dim entityToSave As Object = Nothing
                    Dim newEntityIsUser As Boolean = False

                    ' --- Create and Populate Entity ---
                    If tabName.ToLower() = "scusers" Then
                        Dim newUser As New User()
                        If newRowToSave.Table.Columns.Contains("UserID") AndAlso Not newRowToSave.IsNull("UserID") Then newUser.UserID = newRowToSave.Field(Of String)("UserID")
                        If newRowToSave.Table.Columns.Contains("UserName") AndAlso Not newRowToSave.IsNull("UserName") Then newUser.UserName = newRowToSave.Field(Of String)("UserName")
                        If newRowToSave.Table.Columns.Contains("PasswordHash") AndAlso Not newRowToSave.IsNull("PasswordHash") Then newUser.PasswordHash = newRowToSave.Field(Of String)("PasswordHash")
                        If newRowToSave.Table.Columns.Contains("SucursalCode") AndAlso Not newRowToSave.IsNull("SucursalCode") Then newUser.SucursalCode = newRowToSave.Field(Of Integer)("SucursalCode")
                        If newRowToSave.Table.Columns.Contains("VendedorCode") AndAlso Not newRowToSave.IsNull("VendedorCode") Then newUser.VendedorCode = newRowToSave.Field(Of Integer)("VendedorCode")
                        entityToSave = newUser
                        newEntityIsUser = True
                    Else
                        Dim newItem As New GenericItem()
                        If newRowToSave.Table.Columns.Contains("Name") AndAlso Not newRowToSave.IsNull("Name") Then newItem.Name = newRowToSave.Field(Of String)("Name")
                        If newRowToSave.Table.Columns.Contains("Description") AndAlso Not newRowToSave.IsNull("Description") Then newItem.Description = newRowToSave.Field(Of String)("Description")
                        If newRowToSave.Table.Columns.Contains("CreatedDate") AndAlso Not newRowToSave.IsNull("CreatedDate") Then newItem.CreatedDate = newRowToSave.Field(Of DateTime)("CreatedDate") Else newItem.CreatedDate = DateTime.Now
                        entityToSave = newItem
                        newEntityIsUser = False
                    End If

                    ' --- Simulate InsertingRow Event ---
                    Dim insertingArgs As New LibX.Data.AdpaterRowUpdatingEventArgs()
                    Dim fakeOleDbUpdatingArgs As System.Data.Common.RowUpdatingEventArgs = Nothing
                    Try
                        fakeOleDbUpdatingArgs = New System.Data.Common.RowUpdatingEventArgs(newRowToSave, Nothing, StatementType.Insert, Nothing)
                        ' TODO: If AdpaterRowUpdatingEventArgs has a constructor or property to accept fakeOleDbUpdatingArgs, use it.
                        ' For now, we assume TableInfo is the primary part used by handlers, or Handled property.
                    Catch exMockArgs As Exception
                        Log.Add("AcceptNew: Could not create fake RowUpdatingEventArgs for InsertingRow. " & exMockArgs.Message)
                    End Try
                    insertingArgs.TableInfo = oT
                    ' If insertingArgs.UpdatingArgs can be set and is typed to the base System.Data.Common.RowUpdatingEventArgs:
                    ' insertingArgs.UpdatingArgs = fakeOleDbUpdatingArgs

                    RaiseEvent InsertingRow(Me, insertingArgs)

                    Dim skipSave As Boolean = insertingArgs.Handled
                    If Not skipSave AndAlso fakeOleDbUpdatingArgs IsNot Nothing AndAlso fakeOleDbUpdatingArgs.Status = UpdateStatus.SkipCurrentRow Then
                        skipSave = True
                    End If

                    If skipSave Then
                        If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                            LibX.Data.Manager.Connection.RollBackTransaction() ' Rollback if event handler cancelled
                        End If
                        Return ' Exit Sub as operation was cancelled
                    End If

                    ' --- EF Core Save Operation ---
                    Try
                        If entityToSave IsNot Nothing Then
                            Using context As New AppDbContext()
                                If newEntityIsUser Then
                                    context.Users.Add(CType(entityToSave, User))
                                Else
                                    context.GenericItems.Add(CType(entityToSave, GenericItem))
                                End If
                                recordsAffected = context.SaveChanges().GetAwaiter().GetResult() ' TODO: Convert AcceptNew to Async
                                entitySavedSuccessfully = (recordsAffected > 0)

                                If entitySavedSuccessfully Then
                                    If newEntityIsUser Then
                                        newEntityPrimaryKey = CType(entityToSave, User).UserID
                                    Else
                                        newEntityPrimaryKey = CType(entityToSave, GenericItem).Id
                                    End If
                                    If Not String.IsNullOrEmpty(sCol) AndAlso newRowToSave.Table.Columns.Contains(sCol) AndAlso newEntityPrimaryKey IsNot Nothing Then
                                        newRowToSave(sCol) = newEntityPrimaryKey
                                    End If
                                End If
                            End Using
                        End If
                    Catch exDb As Exception
                        Log.Add(exDb)
                        If mUseTransactions AndAlso LibX.Data.Manager.Connection IsNot Nothing AndAlso LibX.Data.Manager.Connection.IsIntransaction Then
                            LibX.Data.Manager.Connection.RollBackTransaction()
                        End If
                        Throw
                    End Try

                    ' --- Simulate InsertedRow Event ---
                    If entitySavedSuccessfully Then
                        Dim insertedArgs As New LibX.Data.AdpaterRowUpdatedEventArgs()
                        Dim fakeOleDbUpdatedArgs As System.Data.Common.RowUpdatedEventArgs = Nothing
                        Try
                             fakeOleDbUpdatedArgs = New System.Data.Common.RowUpdatedEventArgs(newRowToSave, Nothing, StatementType.Insert, recordsAffected)
                        Catch exMockArgs As Exception
                            Log.Add("AcceptNew: Could not create fake RowUpdatedEventArgs for InsertedRow. " & exMockArgs.Message)
                        End Try
                        insertedArgs.TableInfo = oT
                        ' insertedArgs.UpdatingArgs = fakeOleDbUpdatedArgs ' If possible
                        If newEntityPrimaryKey IsNot Nothing Then
                            If TypeOf newEntityPrimaryKey Is Integer Then
                                insertedArgs.Serial = CInt(newEntityPrimaryKey)
                            ElseIf TypeOf newEntityPrimaryKey Is String Then
                                Integer.TryParse(newEntityPrimaryKey.ToString(), insertedArgs.Serial)
                            End If
                        End If
                        RaiseEvent InsertedRow(Me, insertedArgs)
                        blnChanges = False
                    End If
                End If
            End If

            If blnChanges = True AndAlso mHandledUpdates = False Then
                ExecSaveDetail(CType(cm.Current, DataRowView).Row)
            End If

            If mUseTransactions And Not mHandledUpdates Then
                LibX.Data.Manager.Connection.CommitTransaction()
            End If

            If Not oTable Is Nothing And Not mHandledUpdates Then

                If mds.Tables(tabName).PrimaryKey.Length = 0 Then
                    Dim objrow As DataRow
                    Dim i As Integer = 0
                    mds.Tables(tabName).BeginLoadData()

                    For Each objrow In mds.Tables(tabName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To oTable.Columns.Count - 1
                                mds.Tables(tabName).Columns(j).ReadOnly = False
                                objrow(j) = oTable.Rows(i)(j)
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(tabName).EndLoadData()
                Else
                    Me.mDs.Merge(oTable)

                    Dim i As Integer
                    oTable = mDs.Tables(tabName)
                    For i = oTable.Rows.Count - 1 To 0 Step -1
                        If oTable.Rows(i).RowState = DataRowState.Added Then
                            oTable.Rows.RemoveAt(i)
                        End If
                    Next
                End If
            End If

            mds.AcceptChanges()

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View


            mRecordPosition = cm.Position
            mRecordCount = mds.Tables(tabName).Rows.Count


            DoChangeState()

            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Add

            RaiseEvent ExecutedAction(Me, oArgs2)

            mCurrAction = LibxConnectionActions.None

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
            If Not oV Is Nothing Then
                If Not oV.FocusControl Is Nothing Then
                    oV.FocusControl.Focus()
                End If
            End If
        End Try
    End Sub



    Public Sub ExecuteFind()
        Try
            ExecuteFind("")
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub ExecuteFind(ByVal where As String)
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean

        Try

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs
            Dim oRow As DataRow

            Dim cm As CurrencyManager = GetCM()

            oArgs.Action = LibxConnectionActions.Accept
            oArgs.AcceptedAction = LibxConnectionActions.Find

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If


            tabName = Me.GetMainTableName

            oTable = mDs.Tables(tabName).Clone
            oRow = oTable.NewRow
            For Each oc As DataColumn In oTable.Columns
                If oc.Expression = "" Then
                    oc.ReadOnly = False
                    oc.AllowDBNull = True
                End If
            Next
            oTable.Rows.Add(oRow)

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            Dim sSql As String
            Dim sWhere As String

            If Not oT.Source Is Nothing AndAlso oT.Source.Length > 0 Then
                sSql = Trim(Join(oT.Source, " "))
            Else
                sSql = "Select * from " & tabName
            End If

            If blnChanges Then
                sWhere = GetSqlWhere(oTable.Rows(0), sSql, tabName)
            End If

            If Trim(where) <> "" Then
                If Trim(sWhere) <> "" Then
                    sWhere = String.Concat(sWhere, " and ", where)
                Else
                    sWhere = String.Concat(where)
                End If
            End If


            Dim oAeq As New XBeforeExecuteQueryEventArgs
            Dim oAeq2 As New XBeforeExecuteFillEventArgs
            oAeq.Sql = sSql
            oAeq.Where = sWhere

            RaiseEvent BeforeExecuteQuery(Me, oAeq)


            oAeq2.DoFill = oAeq.DoFill
            oAeq2.Sql = ReplaceWherePart(oAeq.Sql, oAeq.Where)

            If Trim(oAeq.AditionalWhere) <> "" Then
                oAeq2.Sql = ConcatWherePart(oAeq2.Sql, oAeq.AditionalWhere)
            End If

            RaiseEvent BeforeExecuteFill(Me, oAeq2)

            mLastQuery = oAeq2.Sql

            mds.Tables(tabName).Rows.Clear()

            If oAeq2.DoFill Then
                ' --- EF Core Data Fetching ---
                ' TODO: Convert ExecuteFind to an Async method to properly use Await.
                ' Calling .GetAwaiter().GetResult() blocks the current thread and can lead to deadlocks in some contexts.
                ' TODO: Implement dynamic entity type determination based on tabName or a mapping instead of If/ElseIf.

                Dim fetchedData As System.Collections.IList
                Try
                    If tabName.Equals("scusers", StringComparison.OrdinalIgnoreCase) Then
                        fetchedData = EfDataHelper.GetEntityListFromSqlAsync(Of User)(oAeq2.Sql, Nothing).GetAwaiter().GetResult()
                    Else
                        fetchedData = EfDataHelper.GetEntityListFromSqlAsync(Of GenericItem)(oAeq2.Sql, Nothing).GetAwaiter().GetResult()
                    End If
                Catch ex As Exception
                    Log.Add(ex) ' Log the exception
                    fetchedData = New List(Of Object)() ' Ensure fetchedData is not null and is of a type that can be iterated if empty
                End Try

                If fetchedData IsNot Nothing Then
                    For Each item As Object In fetchedData
                        Dim newRow As DataRow = mds.Tables(tabName).NewRow()
                        ' --- Map properties from item to newRow ---
                        If TypeOf item Is User Then
                            Dim userItem = CType(item, User)
                            If mds.Tables(tabName).Columns.Contains("UserID") Then newRow("UserID") = userItem.UserID
                            If mds.Tables(tabName).Columns.Contains("UserName") AndAlso userItem.UserName IsNot Nothing Then newRow("UserName") = userItem.UserName Else If mds.Tables(tabName).Columns.Contains("UserName") Then newRow("UserName") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("PasswordHash") AndAlso userItem.PasswordHash IsNot Nothing Then newRow("PasswordHash") = userItem.PasswordHash Else If mds.Tables(tabName).Columns.Contains("PasswordHash") Then newRow("PasswordHash") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("SucursalCode") Then newRow("SucursalCode") = userItem.SucursalCode
                            If mds.Tables(tabName).Columns.Contains("VendedorCode") Then newRow("VendedorCode") = userItem.VendedorCode
                            ' TODO: Map other User properties as needed.
                        ElseIf TypeOf item Is GenericItem Then
                            Dim genericItem = CType(item, GenericItem)
                            If mds.Tables(tabName).Columns.Contains("Id") Then newRow("Id") = genericItem.Id
                            If mds.Tables(tabName).Columns.Contains("Name") AndAlso genericItem.Name IsNot Nothing Then newRow("Name") = genericItem.Name Else If mds.Tables(tabName).Columns.Contains("Name") Then newRow("Name") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("Description") AndAlso genericItem.Description IsNot Nothing Then newRow("Description") = genericItem.Description Else If mds.Tables(tabName).Columns.Contains("Description") Then newRow("Description") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("CreatedDate") Then newRow("CreatedDate") = genericItem.CreatedDate
                            ' TODO: Map other GenericItem properties as needed.
                        End If
                        mds.Tables(tabName).Rows.Add(newRow)
                    Next
                    mds.Tables(tabName).AcceptChanges() ' Commit changes to the DataTable
                End If
                ' --- End EF Core Data Fetching ---
            End If

            Me.mRecordCount = mds.Tables(tabName).Rows.Count
            Me.mRecordPosition = 0

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View

            DoChangeState()

            OnRowChange(True)

            If Me.mIsHeaderOnGrid Then
                RemoveHandler cm.PositionChanged, AddressOf HeaderPositionChanged
                AddHandler cm.PositionChanged, AddressOf HeaderPositionChanged
            End If


            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Find

            RaiseEvent ExecutedAction(Me, oArgs2)


        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub ReQuery()
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean

        Try
            tabName = Me.mDm
            If Trim(tabName) = "" Then
                tabName = Me.mSources.Item(0).TableName
            End If

            '''oDs = mds.GetChanges

            '''If Not oDs Is Nothing Then
            '''    oTable = oDs.Tables(tabName)

            '''    If oTable.Rows.Count <= 0 Then
            '''        Exit Sub
            '''    End If
            '''Else
            '''    Exit Sub
            '''End If

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Accept
            oArgs.AcceptedAction = LibxConnectionActions.Find

            Dim cm As CurrencyManager = GetCM()
            cm.EndCurrentEdit()

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            blnChanges = False

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            Dim sSql As String
            Dim sWhere As String

            If Me.CurrentDataRow Is Nothing Then
                Exit Sub
            End If

            If mCurrAction = LibxConnectionActions.Edit Then
                If Not oT.Source Is Nothing AndAlso oT.Source.Length > 0 Then
                    sSql = Trim(Join(oT.Source, " "))
                Else
                    sSql = "Select * from " & tabName
                End If

                sWhere = GetSqlWhereByPk(Me.CurrentDataRow.Row, sSql)
            Else
                If mLastQuery Is Nothing Then
                    If Not oT.Source Is Nothing AndAlso oT.Source.Length > 0 Then
                        sSql = Trim(Join(oT.Source, " "))
                    Else
                        sSql = "Select * from " & tabName
                    End If
                Else
                    sSql = mLastQuery
                End If

                If blnChanges Then
                    sWhere = GetSqlWhere(oTable.Rows(0), sSql, tabName)
                End If
            End If

            Dim oAeq As New XBeforeExecuteQueryEventArgs
            Dim oAeq2 As New XBeforeExecuteFillEventArgs
            oAeq.Sql = sSql
            oAeq.Where = sWhere
            If blnChanges Then
                oAeq.row = oTable.Rows(0)
            End If

            RaiseEvent BeforeExecuteQuery(Me, oAeq)

            mds.RejectChanges() ' Preserving this call

            oAeq2.DoFill = oAeq.DoFill
            oAeq2.Sql = ConcatWherePart(oAeq.Sql, oAeq.Where)

            If Trim(oAeq.AditionalWhere) <> "" Then
                oAeq2.Sql = ConcatWherePart(oAeq2.Sql, oAeq.AditionalWhere)
            End If

            RaiseEvent BeforeExecuteFill(Me, oAeq2)

            mds.Tables(tabName).Rows.Clear()

            If oAeq2.DoFill And Not Me.mHandledRowsFill Then
                ' --- EF Core Data Fetching for ReQuery ---
                ' TODO: Convert ReQuery to an Async method to properly use Await.
                ' Calling .GetAwaiter().GetResult() blocks the current thread.
                ' TODO: Implement dynamic entity type determination based on tabName.

                Dim fetchedData As System.Collections.IList
                Try
                    If tabName.Equals("scusers", StringComparison.OrdinalIgnoreCase) Then
                        fetchedData = EfDataHelper.GetEntityListFromSqlAsync(Of User)(oAeq2.Sql, Nothing).GetAwaiter().GetResult()
                    Else
                        fetchedData = EfDataHelper.GetEntityListFromSqlAsync(Of GenericItem)(oAeq2.Sql, Nothing).GetAwaiter().GetResult()
                    End If
                Catch ex As Exception
                    Log.Add(ex) ' Log the exception
                    fetchedData = New List(Of Object)() ' Ensure fetchedData is not null
                End Try

                ' mds.RejectChanges() was called before this block, preserving its original position.
                ' Now, clear and populate with new data.
                mds.Tables(tabName).Rows.Clear()

                If fetchedData IsNot Nothing Then
                    For Each item As Object In fetchedData
                        Dim newRow As DataRow = mds.Tables(tabName).NewRow()
                        ' --- Map properties from item to newRow ---
                        If TypeOf item Is User Then
                            Dim userItem = CType(item, User)
                            If mds.Tables(tabName).Columns.Contains("UserID") Then newRow("UserID") = userItem.UserID
                            If mds.Tables(tabName).Columns.Contains("UserName") AndAlso userItem.UserName IsNot Nothing Then newRow("UserName") = userItem.UserName Else If mds.Tables(tabName).Columns.Contains("UserName") Then newRow("UserName") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("PasswordHash") AndAlso userItem.PasswordHash IsNot Nothing Then newRow("PasswordHash") = userItem.PasswordHash Else If mds.Tables(tabName).Columns.Contains("PasswordHash") Then newRow("PasswordHash") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("SucursalCode") Then newRow("SucursalCode") = userItem.SucursalCode
                            If mds.Tables(tabName).Columns.Contains("VendedorCode") Then newRow("VendedorCode") = userItem.VendedorCode
                            ' TODO: Map other User properties as needed.
                        ElseIf TypeOf item Is GenericItem Then
                            Dim genericItem = CType(item, GenericItem)
                            If mds.Tables(tabName).Columns.Contains("Id") Then newRow("Id") = genericItem.Id
                            If mds.Tables(tabName).Columns.Contains("Name") AndAlso genericItem.Name IsNot Nothing Then newRow("Name") = genericItem.Name Else If mds.Tables(tabName).Columns.Contains("Name") Then newRow("Name") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("Description") AndAlso genericItem.Description IsNot Nothing Then newRow("Description") = genericItem.Description Else If mds.Tables(tabName).Columns.Contains("Description") Then newRow("Description") = DBNull.Value
                            If mds.Tables(tabName).Columns.Contains("CreatedDate") Then newRow("CreatedDate") = genericItem.CreatedDate
                            ' TODO: Map other GenericItem properties as needed.
                        End If
                        mds.Tables(tabName).Rows.Add(newRow)
                    Next
                    mds.Tables(tabName).AcceptChanges() ' Commit changes to the DataTable
                End If
                ' --- End EF Core Data Fetching for ReQuery ---
            End If

            Me.mRecordCount = 0
            Me.mRecordPosition = 0
            If Not Me.mHandledRowsFill Then
                Me.mRecordCount = mds.Tables(tabName).Rows.Count
            End If


            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View

            DoChangeState()

            OnRowChange(True)

            If Me.mIsHeaderOnGrid Then
                RemoveHandler cm.PositionChanged, AddressOf HeaderPositionChanged
                AddHandler cm.PositionChanged, AddressOf HeaderPositionChanged
            End If


            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Find

            RaiseEvent ExecutedAction(Me, oArgs2)

            mCurrAction = LibxConnectionActions.None
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Public Sub AcceptFind()
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean

        Try

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Accept
            oArgs.AcceptedAction = LibxConnectionActions.Find

            Dim cm As CurrencyManager = GetCM()
            cm.EndCurrentEdit()


            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            tabName = Me.mDm
            If Trim(tabName) = "" Then
                tabName = Me.mSources.Item(0).TableName
            End If

            oDs = mds.GetChanges

            If Not oDs Is Nothing Then
                oTable = oDs.Tables(tabName)
            End If

            blnChanges = True
            If oTable Is Nothing Then
                blnChanges = False
            Else
                If oTable.Rows.Count = 0 Then
                    blnChanges = False
                End If
            End If

            Dim oT As LibX.LibXDbSourceTable
            Dim sCol As String
            oT = Me.mSources.Item(tabName)
            If Not oT Is Nothing Then
                sCol = oT.SerialColumnName
            End If

            Dim sSql As String
            Dim sWhere As String

            If Not oT.Source Is Nothing AndAlso oT.Source.Length > 0 Then
                sSql = Trim(Join(oT.Source, " "))
            Else
                sSql = "Select * from " & tabName
            End If

            If blnChanges Then
                sWhere = GetSqlWhere(oTable.Rows(0), sSql, tabName)
            End If

            Dim oAeq As New XBeforeExecuteQueryEventArgs
            Dim oAeq2 As New XBeforeExecuteFillEventArgs
            oAeq.Sql = sSql
            oAeq.Where = sWhere
            If blnChanges Then
                oAeq.row = oTable.Rows(0)
            End If

            RaiseEvent BeforeExecuteQuery(Me, oAeq)

            mds.RejectChanges()

            oAeq2.DoFill = oAeq.DoFill
            oAeq2.Sql = ConcatWherePart(oAeq.Sql, oAeq.Where)

            If Trim(oAeq.AditionalWhere) <> "" Then
                oAeq2.Sql = ConcatWherePart(oAeq2.Sql, oAeq.AditionalWhere)
            End If

            RaiseEvent BeforeExecuteFill(Me, oAeq2)

            mds.Tables(tabName).Rows.Clear()

            '// Guardar el ultimo query ejecutado
            mLastQuery = oAeq2.Sql

            If oAeq2.DoFill And Not Me.mHandledRowsFill Then
                ' --- EF Core Data Fetching ---
                ' TODO: Convert AcceptFind to an Async method to properly use Await.
                ' Calling .GetAwaiter().GetResult() blocks the current thread and can lead to deadlocks in some contexts.
                ' TODO: Implement dynamic entity type determination based on tabName instead of hardcoding GenericItem.
                '       For example, by using a dictionary map or conditional logic:
                ' Dim currentEntityType As Type
                ' If tabName.Equals("scusers", StringComparison.OrdinalIgnoreCase) Then currentEntityType = GetType(User) Else currentEntityType = GetType(GenericItem) End If
                ' And then call a modified EfDataHelper method that accepts a Type parameter, or use reflection.

                Dim fetchedData As List(Of GenericItem)
                Try
                    fetchedData = EfDataHelper.GetEntityListFromSqlAsync(Of GenericItem)(oAeq2.Sql, Nothing).GetAwaiter().GetResult()
                Catch ex As Exception
                    Log.Add(ex) ' Log the exception
                    fetchedData = New List(Of GenericItem)() ' Ensure fetchedData is not null
                End Try

                mds.Tables(tabName).Rows.Clear() ' Clear existing rows before populating

                If fetchedData IsNot Nothing Then
                    For Each item As GenericItem In fetchedData
                        Dim newRow As DataRow = mds.Tables(tabName).NewRow()
                        ' --- Map properties from item to newRow ---
                        ' This is a simplified mapping. A more robust solution would:
                        ' 1. Check if mds.Tables(tabName).Columns.Contains("ColumnName") before assigning.
                        ' 2. Handle potential DBNull.Value for nullable properties in entities if the DataTable doesn't allow nulls.
                        ' 3. Match entity properties to DataTable column names, which might require a mapping dictionary if names differ.

                        If mds.Tables(tabName).Columns.Contains("Id") Then newRow("Id") = item.Id
                        If mds.Tables(tabName).Columns.Contains("Name") AndAlso item.Name IsNot Nothing Then newRow("Name") = item.Name Else If mds.Tables(tabName).Columns.Contains("Name") Then newRow("Name") = DBNull.Value
                        If mds.Tables(tabName).Columns.Contains("Description") AndAlso item.Description IsNot Nothing Then newRow("Description") = item.Description Else If mds.Tables(tabName).Columns.Contains("Description") Then newRow("Description") = DBNull.Value
                        If mds.Tables(tabName).Columns.Contains("CreatedDate") Then newRow("CreatedDate") = item.CreatedDate
                        ' TODO: Add mapping for other relevant properties based on GenericItem and actual table schema.
                        ' Example for other potential GenericItem properties (ensure they exist in GenericItem and the DataTable):
                        ' If mds.Tables(tabName).Columns.Contains("ModifiedDate") AndAlso item.ModifiedDate.HasValue Then newRow("ModifiedDate") = item.ModifiedDate.Value Else If mds.Tables(tabName).Columns.Contains("ModifiedDate") Then newRow("ModifiedDate") = DBNull.Value
                        ' If mds.Tables(tabName).Columns.Contains("IsEnabled") Then newRow("IsEnabled") = item.IsEnabled

                        mds.Tables(tabName).Rows.Add(newRow)
                    Next
                    mds.Tables(tabName).AcceptChanges() ' Commit changes to the DataTable
                End If
                ' --- End EF Core Data Fetching ---
            End If

            Me.mRecordCount = 0
            Me.mRecordPosition = 0
            If Not Me.mHandledRowsFill Then
                Me.mRecordCount = mds.Tables(tabName).Rows.Count
            End If

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Accept
            mState = LibxConnectorState.View

            DoChangeState()

            OnRowChange(True)

            If Me.mIsHeaderOnGrid Then
                RemoveHandler cm.PositionChanged, AddressOf HeaderPositionChanged
                AddHandler cm.PositionChanged, AddressOf HeaderPositionChanged
            End If


            oArgs2.Action = LibxConnectionActions.Accept
            oArgs2.AcceptedAction = LibxConnectionActions.Find

            RaiseEvent ExecutedAction(Me, oArgs2)

            mCurrAction = LibxConnectionActions.None
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Public Sub Print()
        Dim tabName As String
        Dim oDs As DataSet
        Dim oTable As DataTable
        Dim blnChanges As Boolean
        Dim WherePk As String
        Try

            Dim oArgs As New ExecutingActionEventArgs
            Dim oArgs2 As New ExecutedActionEventArgs

            oArgs.Action = LibxConnectionActions.Print
            oArgs.AcceptedAction = LibxConnectionActions.Print

            Dim cm As CurrencyManager = GetCM()
            cm.EndCurrentEdit()

            RaiseEvent ExecutingAction(Me, oArgs)

            If oArgs.Handled Then
                Exit Sub
            End If

            tabName = Me.mDm
            If Trim(tabName) = "" Then
                tabName = Me.mSources.Item(0).TableName
            End If

            If mReportMode = True Then
                oDs = mds.GetChanges

                If Not oDs Is Nothing Then
                    oTable = oDs.Tables(tabName)
                End If

                blnChanges = True
                If oTable Is Nothing Then
                    blnChanges = False
                Else
                    If oTable.Rows.Count = 0 Then
                        blnChanges = False
                    End If
                End If
            Else
                oTable = mds.Tables(tabName)
                blnChanges = True
            End If


            If mReportName Is Nothing OrElse mReportName.Trim = "" Then
                Exit Sub
            End If

            'If mModuleName Is Nothing OrElse mModuleName.Trim = "" Then
            '    Throw New ApplicationException("Especifique el modulo del reporte")
            'End If

            ReportObject.RetrieveSQLQuery()

            Dim oAeq As New XBeforeExecuteQueryEventArgs
            Dim oAeq2 As New XBeforeExecuteFillEventArgs

            oAeq.Sql = mReport.SQLQuery

            If blnChanges = True Then
                If Me.mReportMode = False Then
                    oAeq.Where = GetSqlWhereByPk(oTable.Rows(cm.Position), oAeq.Sql)
                Else
                    oAeq.Where = GetSqlWhere(oTable.Rows(0), oAeq.Sql, tabName)
                End If
            End If

            RaiseEvent BeforeExecuteQuery(Me, oAeq)

            mReport.SQLQuery = ConcatWherePart(oAeq.Sql, oAeq.Where)
            mReport.Action = 1

            Me.mIsEditing = False

            Me.mCurrAction = LibxConnectionActions.Print
            mState = LibxConnectorState.View

            DoChangeState()

            If Me.mIsHeaderOnGrid Then
                RemoveHandler cm.PositionChanged, AddressOf HeaderPositionChanged
                AddHandler cm.PositionChanged, AddressOf HeaderPositionChanged
            End If


            oArgs2.Action = LibxConnectionActions.Print
            oArgs2.AcceptedAction = LibxConnectionActions.Print

            RaiseEvent ExecutedAction(Me, oArgs2)

            mCurrAction = LibxConnectionActions.None
        Catch ex As Exception
            Log.Show(ex) '// CAMBIE
        Finally
            mReport = Nothing
        End Try
    End Sub

    Public Sub Accept()
        Try
            LibX.Data.Manager.Connection.UseCopyConnection = False

            Select Case mCurrAction
                Case LibxConnectionActions.Add
                    AcceptNew()
                Case LibxConnectionActions.Find
                    AcceptFind()
                Case LibxConnectionActions.Edit
                    AcceptEdit()

            End Select

        Catch ex As Exception
            LibX.Log.Add(ex)
        End Try
    End Sub

    Public Overridable Sub OnSettingDefaultQueryBalues(ByVal row As DataRowView)
        Dim oS As New SettingDefaultQueryValues
        oS.row = row
        RaiseEvent SettingDefaultqueryValues(Me, oS)
    End Sub

    Public Overridable Sub OnSettingDefaultNewValues(ByVal row As DataRowView)
        Dim oS As New SettingDefaultNewValues
        oS.row = row
        RaiseEvent SettingDefaultNewValues(Me, oS)

    End Sub

    Public Overridable Sub OnSettingDefaultEditValues(ByVal row As DataRowView)
        Dim oS As New SettingDefaultEditValues
        oS.row = row
        RaiseEvent SettingDefaulteditValues(Me, oS)

    End Sub


    Private Sub HeaderPositionChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.RefreshStates()
        NavChangeState()
    End Sub


    Private Sub mSaver_RowUpdating(ByVal sender As Object, ByVal e As Data.AdpaterRowUpdatingEventArgs) Handles mSaver.RowUpdating
        Try
            RaiseEvent InsertingRow(sender, e)

            If e.UpdatingArgs.Status = UpdateStatus.[Continue] Then
                If e.UpdatingArgs.StatementType = StatementType.Delete Then
                    ExecDeleteDetail(e.UpdatingArgs.Row)
                End If
            End If


        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Function CurrentDataRow() As DataRowView
        Try

            Return Me.CurrentDataRow("")

        Catch ex As Exception

            Log.Add(ex)

        End Try


    End Function

    Public Function CurrentDataRow(ByVal tabName As String) As DataRowView
        Dim cm As CurrencyManager
        Try
            cm = GetCM(tabName)

            If cm.Position > -1 AndAlso Not cm.Current Is Nothing Then
                Return CType(cm.Current, DataRowView)
            End If

        Catch ex As Exception
            Log.Add(ex)

        End Try
    End Function

    Protected Overrides Sub OnInitSettings()
        Try
            MyBase.OnInitSettings()

            RaiseEvent InitSettings(Me, New EventArgs)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Sub mSaver_RowUpdated(ByVal sender As Object, ByVal e As Data.AdpaterRowUpdatedEventArgs) Handles mSaver.RowUpdated
        Try
            RaiseEvent InsertedRow(sender, e)

            If e.UpdatingArgs.Status = UpdateStatus.[Continue] Then
                If e.UpdatingArgs.StatementType = StatementType.Delete Then
                    '-->Siempre es antes en el caso de los details
                    '-->ExecDeleteDetail(e.UpdatingArgs.Row)
                Else
                    ExecSaveDetail(e.UpdatingArgs.Row)

                End If
            End If

        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Sub

    Private Sub ExecDeleteDetail(ByVal masterrow As DataRow)
        Try
            Dim ot As LibXDbSourceTable
            Dim cm As CurrencyManager

            For Each ot In Me.Sources
                If ot.IsDetail AndAlso ot.MasterTableName = "" Then
                    DeleteDetail(masterrow, ot)
                End If
            Next

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Public Sub ExecSaveDetail(ByVal masterRow As DataRow)
        Try
            Dim ot As LibXDbSourceTable
            Dim oRow As DataRow
            Dim cm As CurrencyManager

            For Each ot In Me.Sources
                If ot.IsDetail Then
                    If ot.MasterTableName = "" Then
                        SaveDetail(masterRow, ot)
                    Else
                        cm = Me.GetCM(ot.MasterTableName)
                        If cm.Position >= 0 AndAlso Not cm.Current Is Nothing Then
                            oRow = CType(cm.Current, DataRowView).Row
                            SaveDetail(oRow, ot)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Private Sub DeleteDetail(ByVal masterRow As DataRow, ByVal table As LibXDbSourceTable)
        Try
            Dim oTable As DataTable = mds.Tables(table.TableName)
            Dim sRel As String
            Dim cm As CurrencyManager
            Dim oA As New XBeforeSaveDetailEventArgs
            Dim objTable As Object
            Dim objDs As DataSet

            cm = GetCM(table.TableName)

            cm.EndCurrentEdit()

            oA.MasterRow = masterRow
            oA.TableInfo = table
            oA.Handled = False

            RaiseEvent BeforeSaveDetail(Me, oA)

            If oA.Handled Then
                Exit Sub
            End If

            For i As Integer = cm.Count - 1 To 0 Step -1
                cm.RemoveAt(i)
            Next

            objDs = mds.GetChanges

            If Not objDs Is Nothing Then
                objTable = objDs.Tables(table.TableName)
            End If

            If objTable Is Nothing Then
                '--> no ha habido ningun row nuevo
                Exit Sub
            End If

            Dim oAd As New LibX.Data.Adapter

            RemoveHandler oAd.RowUpdating, AddressOf OnDetailRowUpdating
            AddHandler oAd.RowUpdating, AddressOf OnDetailRowUpdating

            RemoveHandler oAd.RowUpdated, AddressOf OnDetailRowUpdated
            AddHandler oAd.RowUpdated, AddressOf OnDetailRowUpdated

            oAd.Save(objTable, table.SerialColumnName, table, masterRow)

            RaiseEvent AfterSaveDetail(Me, oA)

            If oA.Handled = True Then
                Exit Sub
            End If

            '*--> Hacer merge
            If Not objTable Is Nothing Then

                If objTable.PrimaryKey.Length = 0 Then
                    Dim objrow As DataRow
                    Dim i As Integer = 0


                    mds.Tables(table.TableName).BeginLoadData()

                    For Each objrow In mds.Tables(table.TableName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To objTable.Columns.Count - 1
                                mds.Tables(table.TableName).Columns(j).ReadOnly = False
                                objrow(j) = objTable.rows(i)(j)
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(table.TableName).EndLoadData()
                Else
                    '-->Me.Merge(objTable)
                    '--> Se me daba el siguiente caso: luego del merge en el detail
                    '    aparecen mas rows que los insertados, eso es porque como el registro
                    '    orignal tenia los datos del key sin reemplazar, cuando hace este merge son
                    '    registros diferentes porque ya el nuevo tiene los valores porque el replace 
                    '    se hace en el grabado

                    Dim objrow As DataRow
                    Dim i As Integer = 0

                    mds.Tables(table.TableName).BeginLoadData()

                    For Each objrow In mds.Tables(table.TableName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To objTable.Columns.Count - 1
                                mds.Tables(table.TableName).Columns(j).ReadOnly = False
                                objrow(j) = objTable.rows(i)(j)
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(table.TableName).EndLoadData()

                End If

            End If

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub



    Private Sub SaveDetail(ByVal masterRow As DataRow, ByVal table As LibXDbSourceTable)
        Try
            Dim oTable As DataTable = mds.Tables(table.TableName)
            Dim sRel As String
            Dim cm As CurrencyManager
            Dim oA As New XBeforeSaveDetailEventArgs
            Dim objTable As Object
            Dim objDs As DataSet

            cm = GetCM(table.TableName)

            cm.EndCurrentEdit()

            oA.MasterRow = masterRow
            oA.TableInfo = table
            oA.Handled = False

            RaiseEvent BeforeSaveDetail(Me, oA)

            If oA.Handled Then
                Exit Sub
            End If

            If Not table.LineColName Is Nothing AndAlso table.LineColName.Trim <> "" Then
                For Each objrow As DataRow In mds.Tables(table.TableName).Rows
                    If objrow.RowState = DataRowState.Added Then
                        objrow.Item(table.LineColName) = _
                                    Val(mds.Tables(table.TableName).Compute("max(" & table.LineColName.Trim & ")", _
                                                                                   table.LineColName.Trim & _
                                                                                   " is not null").ToString.Trim) + 1
                    End If
                Next
            End If

            objDs = mds.GetChanges

            If Not objDs Is Nothing Then
                objTable = objDs.Tables(table.TableName)
            End If

            If objTable Is Nothing Then
                '--> no ha habido ningun row nuevo
                Exit Sub
            End If

            If mds.Tables(table.TableName).Select(Nothing, Nothing, DataViewRowState.CurrentRows).Length <= 0 _
            AndAlso table.InnerJon = True Then
                Throw New ApplicationException("No puede guardar este registro sin detalle!")
            End If

            Dim oAd As New LibX.Data.Adapter

            RemoveHandler oAd.RowUpdating, AddressOf OnDetailRowUpdating
            AddHandler oAd.RowUpdating, AddressOf OnDetailRowUpdating

            RemoveHandler oAd.RowUpdated, AddressOf OnDetailRowUpdated
            AddHandler oAd.RowUpdated, AddressOf OnDetailRowUpdated

            If table.CustomDbUpdate = False Then
                oAd.Save(objTable, table.SerialColumnName, table, masterRow)
            End If

            RaiseEvent AfterSaveDetail(Me, oA)

            If oA.Handled = True Then
                Exit Sub
            End If

            '*--> Hacer merge
            If Not objTable Is Nothing Then

                If objTable.PrimaryKey.Length = 0 Then
                    Dim objrow As DataRow
                    Dim i As Integer = 0

                    mds.Tables(table.TableName).BeginLoadData()

                    For Each objrow In mds.Tables(table.TableName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To objTable.Columns.Count - 1
                                If mds.Tables(table.TableName).Columns(j).Expression = "" Then
                                    mds.Tables(table.TableName).Columns(j).ReadOnly = False
                                    objrow(j) = objTable.rows(i)(j)
                                End If
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(table.TableName).EndLoadData()
                Else
                    '-->Me.Merge(objTable)
                    '--> Se me daba el siguiente caso: luego del merge en el detail
                    '    aparecen mas rows que los insertados, eso es porque como el registro
                    '    orignal tenia los datos del key sin reemplazar, cuando hace este merge son
                    '    registros diferentes porque ya el nuevo tiene los valores porque el replace 
                    '    se hace en el grabado

                    Dim objrow As DataRow
                    Dim i As Integer = 0


                    mds.Tables(table.TableName).BeginLoadData()

                    For Each objrow In mds.Tables(table.TableName).Rows
                        If objrow.RowState = DataRowState.Added Or objrow.RowState = DataRowState.Modified Then
                            For j As Integer = 0 To objTable.Columns.Count - 1
                                If mds.Tables(table.TableName).Columns(j).Expression = "" Then
                                    mds.Tables(table.TableName).Columns(j).ReadOnly = False
                                    objrow(j) = objTable.rows(i)(j)
                                End If
                            Next j
                            i += 1
                        End If
                    Next

                    mds.Tables(table.TableName).EndLoadData()
                End If

            End If

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Private Sub OnDetailRowUpdating(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatingEventArgs)
        Try

            RaiseEvent InsertingDetailRow(sender, e)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub

    Private Sub OnDetailRowUpdated(ByVal sender As Object, ByVal e As LibX.Data.AdpaterRowUpdatedEventArgs)
        Try
            RaiseEvent InsertedDetailRow(sender, e)
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Sub


    Private Sub FillOwnerFormControlsRecur(ByVal p_objcontrol As Object)
        Dim objDS As Object
        Dim objControl As Control
        Dim objBinding As Binding
        Dim blnHasBinding As Boolean
        Dim strDataMember As String

        Try
            If TypeOf p_objcontrol Is TextBox Or TypeOf p_objcontrol Is Label Then
                blnHasBinding = False
                For Each objBinding In p_objcontrol.DataBindings
                    If TypeOf objBinding.Control Is TextBox Then
                        AddHandler objBinding.Format, AddressOf _BindingObject_Format
                        AddHandler objBinding.Parse, AddressOf _BindingObject_Parse
                    End If

                    If TypeOf objBinding.Control Is Label Then
                        AddHandler objBinding.Format, AddressOf _BindingObject_Format
                    End If


                    blnHasBinding = True
                    '*--> Master
                    If Trim(mdm) = "" Then
                        strDataMember = objBinding.BindingMemberInfo.BindingPath
                    Else
                        strDataMember = mdm
                    End If

                    If Trim(UCase(objBinding.BindingMemberInfo.BindingPath)) = Trim(UCase(strDataMember)) Then
                        m_objFormControls.Add(p_objcontrol.Name.ToUpper, p_objcontrol)
                        Exit Sub
                    End If

                    '*--> Detail
                    'If Trim(Me.m_strDataMemberDetail) = "" Then
                    '    strDataMember = objBinding.BindingMemberInfo.BindingPath
                    'Else
                    '    strDataMember = m_strDataMemberDetail
                    'End If

                    If Trim(UCase(objBinding.BindingMemberInfo.BindingPath)) = Trim(UCase(strDataMember)) Then
                        m_objFormControls.Add(p_objcontrol.Name.ToUpper, p_objcontrol)
                        Exit Sub
                    End If
                Next

                If Not blnHasBinding Then
                    m_objFormControls.Add(p_objcontrol.Name.ToUpper, p_objcontrol)
                End If

                '*Else
                '*-->m_objFormControls.Add(p_objcontrol)
                '*End If
                Exit Sub
            End If

            If TypeOf p_objcontrol Is Panel Or TypeOf p_objcontrol Is TabPage Or _
               TypeOf p_objcontrol Is GroupBox Or TypeOf p_objcontrol Is TabControl Then
                For Each objControl In p_objcontrol.Controls
                    FillOwnerFormControlsRecur(objControl)
                Next
            End If

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub


    Public Function EnableControl() As Boolean
        EnableControl = False
        If Not mIsEditing Then
            Exit Function
        End If

        If mState = LibxConnectorState.Insert And mAllowNew Then
            EnableControl = True
        End If
        If mState = LibxConnectorState.Edit And mAllowEdit Then
            EnableControl = True
        End If
        If mState = LibxConnectorState.Query And Me.mAllowQuery Then
            EnableControl = True
        End If
    End Function

    Public Property UpdatePrimaryKeyColumns() As Boolean
        Get
            Return mUpdatePrimaryKeyColumns
        End Get
        Set(ByVal Value As Boolean)
            mUpdatePrimaryKeyColumns = Value
        End Set
    End Property


    Public Function getOwnerFormControls() As Hashtable
        Dim objControl As Control
        Dim objFrm As Form


        If Not m_objFormControls Is Nothing Then
            Return m_objFormControls
        End If
        objFrm = Me.OwnerForm
        m_objFormControls = New Hashtable
        For Each objControl In objFrm.Controls
            FillOwnerFormControlsRecur(objControl)
        Next
        Return m_objFormControls

    End Function

    Public Sub OwnerFormClose(ByVal sender As Object, ByVal e As CancelEventArgs)
        If mCloseButton = False AndAlso Me.IsEditing = True Then
            If MessageBox.Show("Desea Cancelar la acción actual y cerrar este programa?", "Cerrar Programa", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub

End Class

