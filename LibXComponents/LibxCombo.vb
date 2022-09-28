Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Reflection
Imports System.Drawing.Design


<ToolboxItem(True), SerializableAttribute()> Public Class LibXCombo
    Inherits ComboBox
    Implements IEditProperty

    Const WM_LBUTTONDBLCLK = &H203
    Const WM_LBUTTONDOWN = &H201
    Const WM_SETFOCUS = &H7


    <NonSerialized()> _
    Public m_HasWorkColors As Boolean

    Dim mComboBoxNullOptionText As String = "-1"
    Dim m_blnFirtChanged As Boolean
    Dim m_strTable As String
    Dim m_strTableLK As String
    Dim m_strKeyField As String
    Dim m_strDispFields As String
    Dim m_strCol As String
    Dim m_strSep As String = "-"
    Dim m_blnLoaded As Boolean = False
    Dim m_strDefValue As String
    Dim m_strDefQValue As String
    Dim m_blnBound As Boolean = True
    Dim m_strWhereDefault As String

    Dim WithEvents m_objDataSource As LibXConnector
    Dim m_orgBindingProg As String
    Dim m_intState As Integer = -1
    Dim m_blnSelValChanged As Boolean = False
    Private m_blnSetForDetail As Boolean
    Private m_blnIsDetail As Boolean
    Private m_objSavedColor As Color
    Private m_objSavedEditColor As Color
    Private m_objSavedEditForeColor As Color
    Private m_objSavedForeColor As Color

    <NonSerialized()> _
    Private m_objSavedEditColorOriginal As Color
    <NonSerialized()> _
    Private m_objOrgBkColor As System.Drawing.Color = System.Drawing.Color.Empty

    Private m_blnRequired As Boolean = False
    Private m_blnExecutingValueChanged As Boolean

    Private m_blnIsTyping As Boolean
    Private m_objValue As Object
    Private m_blnIsAttachedToGrid As Boolean
    Private m_blnWasInQuery As Boolean
    Dim m_IgnoreFocus As Boolean
    <NonSerialized()> _
    Dim m_DisabledFocus As Boolean


    Dim m_AllowDefaultSort As Boolean = True
    Private m_blnUseAppDefWhere As Boolean
    Private mSqlString As String

    Public Event BeforeLoadItems(ByVal sender As Object, ByVal e As LoadXComboItemsEventArgs)
    Public Event valueChanged(ByVal sender As Object, ByVal e As XComboSelectedEventArgs)

#Region "Property"
    Dim mNewState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mEditState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mFindState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled

    Dim mNewInitialValue As String
    Dim mEditInitialValue As String
    Dim mFindInitialValue As String

    Dim WithEvents mCon As LibXConnector
    Dim mFieldDescripcion As String = ""
    Dim oControlDescription As StatusBarPanel

    Public Property StatusBarPanelDescripcion() As StatusBarPanel
        Get
            Return oControlDescription
        End Get
        Set(ByVal Value As StatusBarPanel)
            oControlDescription = Value
        End Set
    End Property

    Public Property FieldDescription() As String
        Get
            Return mFieldDescripcion
        End Get
        Set(ByVal Value As String)
            mFieldDescripcion = Value
        End Set
    End Property

    Public Property NewState() As IEditProperty.InitialStateEnum Implements IEditProperty.NewState
        Get
            Return mNewState
        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)
            mNewState = Value
        End Set
    End Property

    Public Property FindState() As IEditProperty.InitialStateEnum Implements IEditProperty.FindState
        Get
            Return mFindState
        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)
            mFindState = Value
        End Set
    End Property

    Public Property EditState() As IEditProperty.InitialStateEnum Implements IEditProperty.EditState
        Get
            Return mEditState
        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)
            mEditState = Value
        End Set
    End Property

    Public Property EditInitialValue() As String Implements IEditProperty.EditInitialValue
        Get
            Return mEditInitialValue
        End Get
        Set(ByVal Value As String)
            mEditInitialValue = Value
        End Set
    End Property

    Public Property FindInitialValue() As String Implements IEditProperty.FindInitialValue
        Get
            Return mFindInitialValue
        End Get
        Set(ByVal Value As String)
            mFindInitialValue = Value
        End Set
    End Property

    Public Property NewInitialValue() As String Implements IEditProperty.NewInitialValue
        Get
            Return mNewInitialValue
        End Get
        Set(ByVal Value As String)
            mNewInitialValue = Value
        End Set
    End Property

#End Region

    Private mBack As Color

    Public Property AllowDefaultSort() As Boolean
        Get
            Return m_AllowDefaultSort
        End Get
        Set(ByVal Value As Boolean)
            m_AllowDefaultSort = Value
        End Set
    End Property

    Public Property currValue() As Object
        Get
            If m_blnBound Then
                Return MyBase.SelectedValue
            Else
                If Not Me.DataSource Is Nothing Then
                    If Me.DataSource.rows.count > 0 Then
                        Return Me.DataSource.rows(SelectedIndex)!colvalue
                    End If
                End If
            End If
        End Get
        Set(ByVal Value As Object)
            If m_blnBound Then

                If Value Is Nothing Then
                    Value = DBNull.Value
                    Me.SelectedValue = ""
                Else
                    Me.SelectedValue = Value
                End If

                If Not m_objDataSource Is Nothing Then


                    Dim objRow As Object
                    objRow = Me.RowOwner
                    If Not objRow Is Nothing Then
                        If Trim(Value) = "" Then
                            Value = DBNull.Value
                        End If
                        objRow(Me.DataBindings(0).BindingMemberInfo.BindingField) = Value
                    End If


                End If


            End If

        End Set
    End Property


    Public Property bound() As Boolean
        Get
            Return m_blnBound
        End Get
        Set(ByVal Value As Boolean)
            m_blnBound = Value
        End Set
    End Property

    <Browsable(False)> _
    Public ReadOnly Property RowOwner() As Object
        Get
            If Not m_objDataSource Is Nothing Then
                Dim oRow As DataRowView

                If Me.DataBindings(0).BindingManagerBase.Position < 0 Then
                    Return Nothing
                End If

                oRow = CType(Me.DataBindings(0).BindingManagerBase.Current, DataRowView)

                Return oRow

            End If
        End Get
    End Property


    <Browsable(False)> _
    Public ReadOnly Property originalBindingProperty() As String
        Get
            Return m_orgBindingProg
        End Get
    End Property


    Private Sub LibxConnector_ChangingState(ByVal sender As Object, ByVal e As XChangeStateEventArgs)
        If m_blnSelValChanged Then
            If Me.SelectedIndex <> -1 Then
                If Not m_blnIsTyping And e.state = LibxConnectorState.View Then
                    m_blnSelValChanged = False
                    Me.RefreshItem(Me.SelectedIndex)
                End If
            End If
        End If

        If e.isEditing Then
            If e.isInDataQuery Then
                If Me.DropDownStyle <> ComboBoxStyle.DropDown Then
                    Me.DropDownStyle = ComboBoxStyle.DropDown
                End If
            Else
                If DropDownStyle <> ComboBoxStyle.DropDownList Then
                    Me.DropDownStyle = ComboBoxStyle.DropDownList
                End If
            End If
        End If

        RefreshState(e)
    End Sub

    Public Property LookupKeyField() As String
        Get
            Return m_strKeyField
        End Get
        Set(ByVal Value As String)
            m_strKeyField = Value
        End Set
    End Property

    Public Property LookupKeyDisplayFields() As String
        Get
            Return m_strDispFields
        End Get
        Set(ByVal Value As String)
            m_strDispFields = Value
        End Set
    End Property

    Public Property LookupTableName() As String
        Get
            Return m_strTableLK
        End Get
        Set(ByVal Value As String)
            m_strTableLK = Value
        End Set
    End Property

    <DefaultValue("-")> _
    Public Property ColumnsSeparator() As String
        Get
            Return m_strSep
        End Get
        Set(ByVal Value As String)
            m_strSep = Value
        End Set
    End Property

    Public Property SqlString() As String
        Get
            Return mSqlString
        End Get
        Set(ByVal Value As String)
            mSqlString = Value
        End Set
    End Property

    Public Sub LoadDbItems(Optional ByVal p_blnRefresh As Boolean = False)
        Dim e As LoadXComboItemsEventArgs
        Dim objTable As DataTable
        Try
            If m_blnLoaded Then
                If Not p_blnRefresh Then
                    Exit Sub
                End If
            End If

            e = New LoadXComboItemsEventArgs
            RaiseEvent BeforeLoadItems(Me, e)

            If e.handled Then
                Exit Sub
            End If


            Dim fillItems As Boolean
            If Not Me.Items Is Nothing Then
                If Me.Items.Count > 0 Then
                    Dim oValue As Object
                    fillItems = True
                    For Each oValue In Me.Items
                        If TypeOf oValue Is DataRowView Then
                            '-->Está llenado con options de eadmatrd
                            fillItems = False
                            Exit For
                        End If
                    Next
                End If
            End If


            If fillItems Then
                If Me.Items.Count > 0 Then
                    '--> Para cuando se especifican opciones directamente en
                    '    la propiedad items
                    Dim sItem As String
                    Dim sValues() As String
                    Dim oRow As DataRow
                    Dim hasError As Boolean
                    Dim oTable As New DataTable("TCombo" & Me.Name)
                    oTable.Columns.Add("code", GetType(String))
                    oTable.Columns.Add("descr", GetType(String))

                    For Each sItem In Me.Items
                        If Trim(sItem) <> "" Then
                            sValues = Split(sItem, "-")
                            If sValues.Length <> 2 Then
                                hasError = True
                                Exit For
                            End If
                            oRow = oTable.NewRow
                            oRow!code = sValues(0)
                            oRow!descr = sValues(1)
                            oTable.Rows.Add(oRow)
                        End If
                    Next

                    If Not hasError Then

                        e.lookupTable = oTable
                        e.displayFields = "descr"
                        e.keyField = "code"


                    End If


                End If
            End If

            If Not e.lookupTable Is Nothing Then
                m_strTableLK = e.lookupTable.tableName
            End If

            If Trim(mSqlString) <> "" And Trim(e.SqlString) = "" Then
                e.SqlString = mSqlString
            End If

            If Trim(m_strTableLK) = "" And Trim(e.SqlString) = "" Then

                If Trim(m_strTable) = "" Or Trim(m_strCol) = "" Then
                    Exit Sub
                End If

                '* --> e.excludedValues es un array que permite excluir algunos elementos del llenado
                '*     Ejemplo: e.excludedValues = New String() {"1"}
                '*-->CAM.Data.Utility.fillComboOptions(Me, m_strTable, m_strCol, e.excludedValues, m_strSep)
                'ControlsUtil.fillComboOptions(Me, m_strTable, m_strCol, e.excludedValues, m_strSep, m_blnBound)
                selectNullValue()

                If m_AllowDefaultSort Then
                    Me.DataSource.DefaultView.Sort = Me.DisplayMember
                End If

                '*m_blnLoaded = True'
            Else
                If e.lookupTable Is Nothing Then

                    If Trim(e.SqlString) <> "" Then
                        fillComboWithLookup(e.SqlString)
                    Else
                        If Trim(m_strTableLK) <> "" Then
                            fillComboWithLookup()
                        End If
                    End If

                Else
                    objTable = GetFormatedLookupTable(e.lookupTable, e.displayFields, e.keyField)
                    Me.setLkTable(objTable)
                End If
            End If
            m_blnLoaded = True
        Catch ex As Exception
            Log.Show(ex)
        Finally
            'e = Nothing
        End Try
    End Sub


    Private Sub setLkTable(ByVal p_objTable As Object)
        Try
            Me.DataSource = p_objTable
        Finally
        End Try

        Me.DisplayMember = "coldescription"

        If m_AllowDefaultSort Then
            p_objTable.DefaultView.Sort = Me.DisplayMember
        End If

        If Me.m_blnBound Then
            Me.ValueMember = "colvalue"
        End If


    End Sub

    Private Sub fillComboWithLookup()
        fillComboWithLookup("Select *  From " & m_strTableLK)
    End Sub

    Private Sub fillComboWithLookup(ByVal sSql As String)
        Dim objTable As Object
        Dim strSQL As String
        Try
            strSQL = sSql

            If Trim(strSQL) = "" Then
                Exit Sub
            End If

            If Trim(m_strWhereDefault) <> "" Then
                '-->strSQL = Trim(strSQL) & " Where " & m_strWhereDefault
                strSQL = ConcatWherePart(strSQL, m_strWhereDefault)
            End If

            objTable = GetOptionsLookupTable(strSQL, m_strDispFields, m_strKeyField, m_strSep)
            setLkTable(objTable)
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Function GetOptionsLookupTable(ByVal p_strSQL As String, ByVal p_strDispFields As String, ByVal p_strKeyField As String, Optional ByVal p_strSep As String = "-") As Object
        Dim strSQL As String
        Dim objTable As Object
        Dim objTableD As DataTable
        Dim objRow As DataRow
        Dim objRowD As DataRow
        Dim strItem As String
        Dim blnAdd As Boolean
        Dim objArrFields As Array
        Dim strField As String
        Dim objCol As Object
        Dim oCol As DataColumn


        Try

            objTableD = New DataTable
            objTableD.Columns.Add("colvalue", GetType(String))
            objTableD.Columns.Add("coldescription", GetType(String))
            objTableD.Columns.Add("coldescriptiononly", GetType(String))

            strSQL = p_strSQL

            objTable = LibX.Data.Manager.GetDataTable(strSQL)

            For Each objCol In objTable.columns
                objTableD.Columns.Add(objCol.ColumnName, objCol.DataType)
            Next

            objRow = objTableD.NewRow
            objRow(0) = DBNull.Value
            If Trim(mComboBoxNullOptionText) = "-1" Then
                objRow(1) = ""
            Else
                objRow(1) = mComboBoxNullOptionText
            End If

            objRow(2) = objRow(1)

            objTableD.Rows.Add(objRow)

            For Each objCol In objTable.columns
                If Trim(strField) = "" Then
                    strField = Trim(objCol.columnname)
                Else
                    strField = Trim(strField) & "," & Trim(objCol.columnname)
                End If
            Next

            If Trim(p_strDispFields) = "" Then
                p_strDispFields = strField
            End If

            objArrFields = Split(p_strDispFields, ",")
            Dim strDisp As String = ""
            Dim strOnlyDisp As String
            Dim Used As Boolean

            For Each objRow In objTable.rows
                blnAdd = True
                strDisp = ""
                strOnlyDisp = ""
                Used = False
                For Each strItem In objArrFields
                    If Not IsNull(objRow(strItem)) Then
                        If p_strKeyField.Trim.ToLower <> strItem.Trim.ToLower Then
                            If Trim(strOnlyDisp) = "" Then
                                strOnlyDisp = Trim(objRow(strItem))
                            Else
                                strOnlyDisp = Trim(strOnlyDisp) & " " & Trim(objRow(strItem))
                            End If
                        End If
                        If Trim(strDisp) = "" Then
                            strDisp = Trim(objRow(strItem))
                        Else
                            '-->El separador se utiliza una sola vez, para separa el codigo de la descripcion, pero
                            '   para separa multiples descripciones.
                            If Not Used Then
                                strDisp = Trim(strDisp) & p_strSep & Trim(objRow(strItem))
                                Used = True
                            Else
                                strDisp = Trim(strDisp) & " " & Trim(objRow(strItem))
                            End If
                        End If
                    End If
                Next
                If blnAdd Then
                    objRowD = objTableD.NewRow
                    objRowD(0) = Trim(objRow(p_strKeyField))
                    objRowD(1) = Trim(strDisp)
                    objRowD(2) = Trim(strOnlyDisp)

                    For Each objCol In objTable.columns
                        objRowD(objCol.ColumnName) = objRow(objCol.ColumnName)
                    Next

                    '*-->objRowD(1) = Trim(objRow(p_strKeyField)) & p_strSep & Trim(strDisp)
                    objTableD.Rows.Add(objRowD)
                End If
            Next


            'objTableD.DefaultView.Sort = "coldescription"

            Return objTableD
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function


    Public Function GetCurrentRow() As DataRow
        Try

            If m_blnBound Then
                If Not Me.DataSource Is Nothing Then
                    If Me.DataSource.rows.count > 0 Then
                        If Me.SelectedIndex <> -1 Then
                            Return Me.Items(SelectedIndex).row
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub selectNullValue()
        Try
            Me.SelectedItem = DBNull.Value
        Catch
            Me.SelectedIndex = 0
        End Try
    End Sub

    Private Sub NetComboBox_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.SelectedValueChanged
        Dim objArgs As New XComboSelectedEventArgs
        Try
            m_blnIsTyping = False

            If m_blnExecutingValueChanged Then
                Exit Sub
            End If

            If m_blnLoaded Then
                m_blnSelValChanged = True
                objArgs.value = Me.currValue
                m_objValue = objArgs.value
                m_blnExecutingValueChanged = True

                If m_blnBound Then
                    If Not Me.DataSource Is Nothing Then
                        If Me.DataSource.rows.count > 0 Then
                            objArgs.Description = ""
                            objArgs.FullDescription = ""

                            If Me.SelectedIndex <> -1 Then
                                '-->objArgs.Description = Trim(Me.DataSource.rows(SelectedIndex)!coldescription)
                                objArgs.Description = Me.Items(SelectedIndex).row!coldescriptiononly
                                objArgs.FullDescription = Me.Items(SelectedIndex).row!coldescription
                                objArgs.Row = Me.Items(SelectedIndex).row

                            End If
                        End If
                    End If
                Else

                End If


                RaiseEvent valueChanged(Me, objArgs)

                '*--> Porque por alguna razon si desde un change se manda a cambiar el valor
                '*    de otro campo que tenga combo, este se hace dbnull
                If Not m_objDataSource Is Nothing Then
                    If m_objDataSource.IsEditing Then
                        if not objArgs.value is dbnull.value then
                            Me.currValue = objArgs.value
                        else
                            Me.currValue =""
                        END IF
                    End If
                End If

            End If
            '--> objArgs = Nothing
        Catch ex As Exception
            Log.Show(ex)
        Finally
            m_blnExecutingValueChanged = False
        End Try

    End Sub


    Private Sub NetComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.SelectionChangeCommitted

        m_blnSelValChanged = True

        Dim blnIsQuery As Boolean

        If Not m_objDataSource Is Nothing Then
            If m_objDataSource.State = LibxConnectorState.Query Then
                blnIsQuery = True
            End If
        End If

        If blnIsQuery Then
            If Not m_objDataSource Is Nothing Then
                If Me.SelectedIndex <> -1 Then
                    m_objValue = Me.currValue
                End If
            End If
        End If


    End Sub


    <DefaultValue("")> _
    Public Property DefaultWhereString() As String
        Get
            Return m_strWhereDefault
        End Get
        Set(ByVal Value As String)
            m_strWhereDefault = Value
        End Set
    End Property

    Public Property Required() As Boolean
        Get
            Return m_blnRequired
        End Get
        Set(ByVal Value As Boolean)
            m_blnRequired = Value
        End Set
    End Property

    Protected Overrides Sub OnBindingContextChanged(ByVal e As System.EventArgs)
        Try
            MyBase.OnBindingContextChanged(e)

            If Me.DesignMode Then
                Exit Sub
            End If
            If Me.DataBindings.Count > 0 Then
                If TypeOf DataBindings(0).DataSource Is LibXConnector Then
                    m_objDataSource = DataBindings(0).DataSource
                Else
                    Dim ds As DataSet = DataBindings(0).DataSource
                    If Not ds.ExtendedProperties Is Nothing AndAlso ds.ExtendedProperties.Count > 0 Then
                        m_objDataSource = ds.ExtendedProperties.Item("xcone")
                    End If
                End If
            End If

            If Not m_objDataSource Is Nothing Then

                If Trim(m_orgBindingProg) = "" Then

                    m_orgBindingProg = Me.DataBindings(0).PropertyName

                    If Not Me.DesignMode Then
                        Me.LoadDbItems()
                    End If

                    RemoveHandler m_objDataSource.ChangeState, AddressOf LibxConnector_ChangingState
                    'RemoveHandler m_objDataSource.beforeAcceptQueryAction, AddressOf LibxConnector_BeforeAcceptQueryAction
                    'RemoveHandler m_objDataSource.InitSettings, AddressOf OnDsInitSettings

                    AddHandler m_objDataSource.ChangeState, AddressOf LibxConnector_ChangingState
                    'AddHandler m_objDataSource.beforeAcceptQueryAction, AddressOf LibxConnector_BeforeAcceptQueryAction
                    'AddHandler m_objDataSource.InitSettings, AddressOf OnDsInitSettings

                End If

            End If

            If Not m_objDataSource Is Nothing Then
                If Me.mRequiered Then
                    m_objDataSource.AddRequired(Me)
                End If
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    'Private Sub OnDsInitSettings(ByVal sender As Object, ByVal e As EventArgs)
    '    Me.LoadDbItems()
    'End Sub

    Protected Overrides Sub OnKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)

        MyBase.OnKeyPress(e)

        m_blnIsTyping = False

        If Not m_objDataSource Is Nothing Then
            If m_objDataSource.State = LibxConnectorState.Query Then
                m_blnIsTyping = True
            End If
            If Me.DropDownStyle = ComboBoxStyle.DropDownList Then
                Exit Sub
            End If
            If Not m_blnIsTyping Then
                If m_objDataSource.IsDataEditing Then

                    If Me.DataBindings.Count > 0 Then
                        Dim objTable As DataTable
                        Dim objValue As Object = e.KeyChar.ToString
                        Dim strFld As String = Me.DataBindings(0).BindingMemberInfo.BindingField
                        objTable = Me.RowOwner.row.table
                        If objTable.Columns(strFld).DataType.Equals(GetType(System.String)) Then
                            If Not IsNumeric(objValue) Then
                                Me.currValue = e.KeyChar.ToString
                            End If
                        Else
                            If IsNumeric(objValue) Then
                                Me.currValue = e.KeyChar.ToString
                            End If
                        End If
                    End If


                    e.Handled = True
                End If
            End If
        End If

    End Sub

    Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)
        MyBase.OnDrawItem(e)
        m_blnSelValChanged = False
    End Sub

    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        If Not m_blnFirtChanged Then
            m_blnFirtChanged = True
            Me.DropDownStyle = ComboBoxStyle.DropDownList
        End If

    End Sub


    Public Sub setIsDetail(ByVal p_blnIsDetail As Boolean)
        m_blnIsDetail = p_blnIsDetail
    End Sub

    Public Sub SetWorkColors(ByVal p_objBColor As Color, ByVal p_objFColor As Color, ByVal p_objSBcolor As Color, ByVal p_objSFcolor As Color, ByVal p_blnDetail As Boolean)
        'If p_blnDetail And Not m_blnSetForDetail Then
        '    m_blnSetForDetail = True
        '    m_objSavedColor = Color.Empty
        'End If

        'If Not m_objSavedColor.Equals(Color.Empty) Then
        '    Exit Sub
        'End If

        'Me.m_objSavedEditForeColor = p_objFColor
        'Me.m_objSavedEditColor = p_objBColor
        'Me.m_objSavedForeColor = p_objSFcolor
        'Me.m_objSavedColor = p_objSBcolor
        'm_objSavedEditColorOriginal = m_objSavedEditColor

        'If m_IgnoreFocus Then
        '    Me.m_objSavedEditForeColor = m_objSavedForeColor
        '    Me.m_objSavedEditColor = m_objSavedColor

        '    If Not App.configuration.ControlIgnoreFocusColor.Equals(System.Drawing.Color.Empty) Then
        '        Me.m_objSavedEditColor = App.configuration.ControlIgnoreFocusColor
        '    End If
        'End If

        'm_HasWorkColors = True

        'Me.CheckBankColor()
    End Sub

    Public Sub CheckBankColor()
        'If Not Me.m_objDataSource Is Nothing Then
        '    Dim blnEditing As Boolean

        '    If m_objSavedColor.Equals(Color.Empty) Then
        '        Exit Sub
        '    End If

        '    blnEditing = Me.m_objDataSource.IsEditing

        '    If m_blnIsDetail Then
        '        blnEditing = Me.m_objDataSource.isDetailEditing
        '    End If

        '    If blnEditing Then

        '        If Not Me.Enabled Then
        '            Me.BackColor = CAM.App.configuration.controlDisableColor()
        '            Me.ForeColor = SystemColors.InactiveBorder
        '        Else
        '            Me.BackColor = m_objSavedEditColor
        '            Me.ForeColor = m_objSavedEditForeColor
        '        End If
        '    Else
        '        '--> Si cambio el forecolor debe colocarlo
        '        Me.BackColor = m_objSavedColor
        '        Me.ForeColor = m_objSavedForeColor
        '    End If
        'End If
    End Sub

    Public Sub EnableTakeFocus()
        'm_DisabledFocus = False

        'If m_HasWorkColors Then
        '    Me.m_objSavedEditColor = m_objSavedEditColorOriginal
        '    Me.checkBankColor()
        '    Exit Sub
        'End If

        'Me.m_objSavedEditColor = m_objSavedEditColorOriginal

        'If m_objSavedEditColorOriginal.Equals(System.Drawing.Color.Empty) Then
        '    m_objSavedEditColor = Me.m_objOrgBkColor
        'End If

        'If m_objSavedEditColor.Equals(System.Drawing.Color.Empty) Then
        '    m_objSavedEditColor = Me.BackColor
        'End If


    End Sub

    Public Sub DisableTakeFocus()
        'm_DisabledFocus = True

        'If m_HasWorkColors Then
        '    Me.m_objSavedEditColor = m_objSavedColor
        '    Me.checkBankColor()
        '    If Not App.configuration.ControlIgnoreFocusColor.Equals(System.Drawing.Color.Empty) Then
        '        Me.BackColor = App.configuration.ControlIgnoreFocusColor
        '    End If
        '    Exit Sub
        'End If

        'If m_objSavedColor.Equals(System.Drawing.Color.Empty) Then
        '    m_objSavedColor = Me.m_objOrgBkColor
        'End If
        'If m_objSavedColor.Equals(System.Drawing.Color.Empty) Then
        '    m_objSavedColor = Me.BackColor
        'End If

        'Me.BackColor = m_objSavedColor
        'Me.m_objSavedEditColor = m_objSavedColor

        'If Not App.configuration.ControlIgnoreFocusColor.Equals(System.Drawing.Color.Empty) Then
        '    Me.BackColor = App.configuration.ControlIgnoreFocusColor
        'End If

    End Sub

    Protected Overrides Sub OnEnabledChanged(ByVal e As System.EventArgs)
        MyBase.OnEnabledChanged(e)
        Me.CheckBankColor()
    End Sub


    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m_IgnoreFocus Or m_DisabledFocus Then
            If m.Msg = WM_LBUTTONDOWN Or m.Msg = WM_SETFOCUS Or m.Msg = WM_LBUTTONDBLCLK Then
                Return
            End If
        End If
        MyBase.WndProc(m)
    End Sub


    Private Function GetFormatedLookupTable(ByVal p_objTable As Object, ByVal p_strDispFields As String, ByVal p_strKeyField As String, Optional ByVal p_strSep As String = "-") As Object
        Dim strSQL As String
        Dim objTable As Object
        Dim objTableD As DataTable
        Dim objRow As DataRow
        Dim objRowD As DataRow
        Dim strItem As String
        Dim blnAdd As Boolean
        Dim objArrFields As Array
        Dim strField As String
        Dim objCol As Object


        Try

            objTableD = New DataTable
            objTableD.Columns.Add("colvalue", GetType(String))
            objTableD.Columns.Add("coldescription", GetType(String))
            objTableD.Columns.Add("coldescriptiononly", GetType(String))

            objTable = p_objTable


            For Each objCol In objTable.columns
                objTableD.Columns.Add(objCol.ColumnName, objCol.DataType)
            Next

            objRow = objTableD.NewRow
            objRow(0) = DBNull.Value
            '-->objRow(1) = "(ninguno)"
            If Trim(mComboBoxNullOptionText) = "-1" Then
                objRow(1) = ""
            Else
                objRow(1) = mComboBoxNullOptionText
            End If
            objRow(2) = objRow(1)

            objTableD.Rows.Add(objRow)

            For Each objCol In objTable.columns
                If Trim(strField) = "" Then
                    strField = Trim(objCol.columnname)
                Else
                    strField = Trim(strField) & "," & Trim(objCol.columnname)
                End If
            Next

            If Trim(p_strDispFields) = "" Then
                p_strDispFields = strField
            End If

            objArrFields = Split(p_strDispFields, ",")
            For Each objRow In objTable.rows
                blnAdd = True
                Dim strDisp As String = ""
                Dim strOnlyDisp As String
                Dim Used As Boolean

                strOnlyDisp = ""
                Used = False

                For Each strItem In objArrFields
                    'If Not CAM.Util.isNulo(objRow(strItem)) Then
                    '    If Trim(strDisp) = "" Then
                    '        strDisp = Trim(objRow(strItem))
                    '    Else
                    '        strDisp = Trim(strDisp) & p_strSep & Trim(objRow(strItem))
                    '    End If
                    'End If
                    If p_strKeyField.Trim.ToLower <> strItem.Trim.ToLower Then
                        If Trim(strOnlyDisp) = "" Then
                            strOnlyDisp = Trim(objRow(strItem))
                        Else
                            strOnlyDisp = Trim(strOnlyDisp) & " " & Trim(objRow(strItem))
                        End If
                    End If
                    If Trim(strDisp) = "" Then
                        strDisp = Trim(objRow(strItem))
                    Else
                        '-->El separador se utiliza una sola vez, para separa el codigo de la descripcion, pero
                        '   para separa multiples descripciones.
                        If Not Used Then
                            strDisp = Trim(strDisp) & p_strSep & Trim(objRow(strItem))
                            Used = True
                        Else
                            strDisp = Trim(strDisp) & " " & Trim(objRow(strItem))
                        End If
                    End If
                Next
                If blnAdd Then
                    objRowD = objTableD.NewRow
                    objRowD(0) = Trim(objRow(p_strKeyField))
                    objRowD(1) = Trim(strDisp)
                    objRowD(2) = Trim(strOnlyDisp)

                    For Each objCol In objTable.columns
                        objRowD(objCol.ColumnName) = objRow(objCol.ColumnName)
                    Next

                    '*-->objRowD(1) = Trim(objRow(p_strKeyField)) & p_strSep & Trim(strDisp)
                    objTableD.Rows.Add(objRowD)
                End If
            Next

            Return objTableD
        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function


    Public Sub RefreshState(ByVal e As XChangeStateEventArgs) Implements IEditProperty.RefreshState
        If mBack.Equals(Color.Empty) Then
            mBack = Me.BackColor
        End If


        Select Case e.action
            Case LibxConnectionActions.Add
                Me.Enabled = (Me.NewState = IEditProperty.InitialStateEnum.Enabled)
                m_objValue = Me.NewInitialValue
                If Not m_objValue Is Nothing AndAlso m_objValue.ToString.Trim <> "" Then
                    Me.currValue = m_objValue
                End If
            Case LibxConnectionActions.Edit
                Me.Enabled = (Me.EditState = IEditProperty.InitialStateEnum.Enabled)
                m_objValue = Me.EditInitialValue
                If Not m_objValue Is Nothing AndAlso m_objValue.ToString.Trim <> "" Then
                    Me.currValue = m_objValue
                End If
            Case LibxConnectionActions.Find
                Me.Enabled = (Me.FindState = IEditProperty.InitialStateEnum.Enabled)
                m_objValue = Me.FindInitialValue
                If Not m_objValue Is Nothing AndAlso m_objValue.ToString.Trim <> "" Then
                    Me.currValue = m_objValue
                End If

            Case LibxConnectionActions.None
                Me.Enabled = False
                Me.BackColor = System.Drawing.Color.White

            Case Else
                Me.Enabled = False
                Me.BackColor = System.Drawing.Color.White
        End Select

        Me.TabStop = Me.Enabled

        If e.isEditing = True Then
            If Me.Enabled = True Then
                Me.BackColor = System.Drawing.Color.White
            Else
                Me.BackColor = System.Drawing.SystemColors.Control
            End If
        End If

    End Sub

    Private mIgnoreRequiered As Boolean
    Private mRequiered As Boolean

    Public Property IgnoreRequiered() As Boolean Implements IEditProperty.IgnoreRequiered
        Get
            Return mIgnoreRequiered
        End Get
        Set(ByVal Value As Boolean)
            mIgnoreRequiered = Value
        End Set
    End Property

    Public Property Requiered() As Boolean Implements IEditProperty.Requiered
        Get
            Return mRequiered
        End Get
        Set(ByVal Value As Boolean)
            mRequiered = Value
        End Set
    End Property

    Private Sub XTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.GotFocus
        If Not oControlDescription Is Nothing Then
            oControlDescription.Text = Me.mFieldDescripcion
        End If

        If Me.Enabled = False Then
            Me.BackColor = System.Drawing.Color.White
        End If

    End Sub

    Private Sub InitializeComponent()
        '
        'LibXCombo
        '
        Me.Name = "LibXCombo"

    End Sub

    Private Sub LibXCombo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then SendKeys.Send("{TAB}")

        'If e.KeyCode = Keys.Up Then

        'End If

        'If e.KeyCode = Keys.Down Then
        '    SendKeys.Send("{TAB}")
        'End If


    End Sub
End Class


