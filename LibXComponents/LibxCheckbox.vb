Imports System.ComponentModel


Public Class LibxCheckBox
    Inherits CheckBox

    Implements IEditProperty

#Region "Variables"
    Private m_SettingText As Boolean
    Dim mKeepEnabled As Boolean
    Private m_blnLeaving As Boolean
    Private m_blnGotFocus As Boolean

    Dim m_objValue As Object
    Dim WithEvents m_objDataSource As LibXConnector
    Dim m_blnFromEvent As Boolean
    Dim m_blnReadOnly As Boolean


#End Region

#Region "Propiedades"

    Sub New()
        MyBase.New()
        Me.AutoCheck = True
        Me.ThreeState = True
        Me.CheckAlign = ContentAlignment.MiddleRight
    End Sub

    Public Property KeepEnabled() As Boolean
        Get
            Return mKeepEnabled
        End Get
        Set(ByVal Value As Boolean)
            mKeepEnabled = Value
        End Set
    End Property



    <Bindable(True)> _
    Public Property value() As Object
        Get
            Return m_objValue
        End Get
        Set(ByVal Value As Object)
            If m_blnLeaving Then
                m_blnLeaving = False
                If Not m_objValue Is Nothing Then
                    If m_objValue.ToString() <> "" Then
                        If m_objDataSource.isEditing Then
                            If Me.DataBindings.Count > 0 Then
                                Dim objRow As DataRowView
                                If Not Me.DataBindings(0).BindingManagerBase.Current Is Nothing Then
                                    objRow = CType(Me.DataBindings(0).BindingManagerBase.Current, DataRowView)
                                    objRow(Me.DataBindings(0).BindingMemberInfo.BindingField) = m_objValue
                                End If
                            End If
                        End If
                    End If
                End If
                Exit Property
            End If
            m_objValue = Value
            If Not m_objValue Is Nothing Then

                If m_objValue.ToString() <> "" Then
                    Me.Checked = CBool(m_objValue)
                Else
                    If Me.Checked Then
                        Me.Checked = False
                    End If

                    m_objValue = 0

                    'If Me.DataBindings.Count > 0 Then
                    '    If Not netDs Is Nothing Then
                    '        If m_objDataSource.state = NetDataSource.NetDataSourceModes.QueryMode Then
                    '            m_objValue = DBNull.Value
                    '        End If
                    '    End If
                    'End If

                    If Not m_objDataSource Is Nothing Then
                        If m_objDataSource.State = LibxConnectorState.Query Then
                            m_objValue = DBNull.Value
                        End If
                    End If

                End If


            End If
            Me.Refresh()
        End Set
    End Property

    Dim mNewState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mEditState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mFindState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled

    Dim mNewInitialValue As String
    Dim mEditInitialValue As String
    Dim mFindInitialValue As String

    Dim WithEvents mCon As LibXConnector

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

    Public Property [ReadOnly]() As Boolean
        Get
            Return m_blnReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnReadOnly = Value
        End Set
    End Property




#End Region

    Protected Overrides Sub OnBindingContextChanged(ByVal e As System.EventArgs)
        Try
            MyBase.OnBindingContextChanged(e)

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
                If Me.mRequiered Then
                    m_objDataSource.AddRequired(Me)
                End If
            End If


        Catch ex As Exception
            Log.Add(ex, Me.Name)
        End Try
    End Sub



    Protected Overrides Sub OnCheckStateChanged(ByVal e As System.EventArgs)
        If Me.m_blnReadOnly Then
            Exit Sub
        End If

        MyBase.OnCheckStateChanged(e)

        If Me.CheckState = CheckState.Indeterminate Then
            If Not m_objDataSource Is Nothing Then
                If m_objDataSource.State = LibxConnectorState.Query Then
                    m_blnFromEvent = True
                    Me.value = DBNull.Value
                Else
                    m_blnFromEvent = True
                    Me.value = 0

                    Me.CheckState = CheckState.Unchecked
                    Me.Checked = False
                End If
            End If
        End If
    End Sub

    Protected Overrides Sub OnCheckedChanged(ByVal e As System.EventArgs)

        If Me.m_blnReadOnly Then
            Exit Sub
        End If

        MyBase.OnCheckedChanged(e)

        If m_blnFromEvent Then
            m_blnFromEvent = False
            Exit Sub
        End If


        If Me.CheckState = CheckState.Indeterminate Then
            If Not m_objDataSource Is Nothing Then
                If m_objDataSource.State = LibxConnectorState.Query Then
                    m_blnFromEvent = True
                    Me.value = DBNull.Value
                    Exit Sub
                Else
                    Me.CheckState = CheckState.Unchecked
                End If
            End If
        End If


        m_blnFromEvent = True

        If Me.Checked Then
            Me.value = 1
        Else
            Me.value = 0
        End If
    End Sub

    Protected Overrides Sub OnKeyUp(ByVal kevent As System.Windows.Forms.KeyEventArgs)
        MyBase.OnKeyUp(kevent)
        If m_objDataSource Is Nothing Then
            Exit Sub
        End If
        If m_objDataSource.IsEditing And Not m_blnReadOnly Then
            If kevent.KeyCode = Keys.Escape Then
                Me.CheckState = CheckState.Indeterminate
            End If
        End If
    End Sub



    Protected Overrides Sub OnClick(ByVal e As System.EventArgs)
        Dim blnChk As Boolean = Me.Checked

        If Me.m_blnReadOnly Then
            Exit Sub
        End If


        MyBase.OnClick(e)

        If m_objDataSource Is Nothing Then
            Exit Sub
        End If
        If m_objDataSource.IsEditing Then
            If Me.m_blnReadOnly Then
                If blnChk <> Me.Checked Then
                    If Me.Checked Then
                        Me.Checked = False
                    Else
                        Me.Checked = True
                    End If
                End If
            End If
        End If
    End Sub

    Protected Overrides Sub OnEnter(ByVal e As System.EventArgs)
        m_blnGotFocus = True
        m_blnLeaving = False
        MyBase.OnEnter(e)
    End Sub
    Protected Overrides Sub OnLeave(ByVal e As System.EventArgs)
        m_blnLeaving = True
        MyBase.OnLeave(e)
        m_blnGotFocus = False
        If m_objDataSource Is Nothing Then
            Exit Sub
        End If
        '-->If m_objDataSource.isDataEditing Then
        If m_objDataSource.IsEditing Then
            If Not Me.m_blnReadOnly Then
                If Me.Checked Then
                    m_objValue = 1
                Else
                    m_objValue = 0
                End If
                If Me.DataBindings.Count > 0 Then
                    Dim objRow As DataRowView
                    If Not Me.DataBindings(0).BindingManagerBase Is Nothing Then
                        If Not Me.DataBindings(0).BindingManagerBase.Current Is Nothing Then
                            Try
                                objRow = CType(Me.DataBindings(0).BindingManagerBase.Current, DataRowView)
                                objRow(Me.DataBindings(0).BindingMemberInfo.BindingField) = m_objValue
                            Catch
                            End Try
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If msg.WParam.ToInt32 = Keys.Enter Then
            SendKeys.Send("{Tab}")
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Public Sub RefreshState(ByVal e As XChangeStateEventArgs) Implements IEditProperty.RefreshState
        Select Case e.action
            Case LibxConnectionActions.Add
                Me.ReadOnly = Not (Me.NewState = IEditProperty.InitialStateEnum.Enabled)

            Case LibxConnectionActions.Edit
                Me.ReadOnly = Not (Me.EditState = IEditProperty.InitialStateEnum.Enabled)

            Case LibxConnectionActions.Find
                Me.ReadOnly = Not (Me.FindState = IEditProperty.InitialStateEnum.Enabled)

            Case LibxConnectionActions.None
                Me.ReadOnly = True

            Case Else
                Me.ReadOnly = True

        End Select

    End Sub

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

    Private Sub m_objDataSource_ChangingState(ByVal sender As Object, ByVal e As XChangeStateEventArgs) Handles m_objDataSource.ChangingState
        RefreshState(e)
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

End Class
