Imports System.Windows.Forms.ComponentModel
Imports System.ComponentModel
Imports MaskedTextBox
Public Class XMaskEdit
    Inherits MaskedTextBox.MaskedTextBox
    Implements IEditProperty

    Dim mDataType
    Dim WithEvents _BindingObject As Binding

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
#End Region

    'Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
    '    If msg.WParam.ToInt32 = Keys.Enter Then
    '        SendKeys.Send("{Tab}")
    '        Return True
    '    End If

    '    Return MyBase.ProcessCmdKey(msg, keyData)
    'End Function

    Protected Overrides Sub OnBindingContextChanged(ByVal e As System.EventArgs)
        MyBase.OnBindingContextChanged(e)

        If Me.DesignMode = True Then
            Exit Sub
        End If

        If Me.DataBindings.Count > 0 Then
            Dim ds As DataSet

            If TypeOf Me.DataBindings(0).DataSource Is LibXConnector Then
                mCon = CType(Me.DataBindings(0).DataSource, LibXConnector)
                'ds = mCon.GetDS
            End If

            If TypeOf Me.DataBindings(0).DataSource Is DataSet Then
                ds = CType(Me.DataBindings(0).DataSource, DataSet)

                If Not ds.ExtendedProperties Is Nothing AndAlso ds.ExtendedProperties.Count > 0 Then
                    mCon = ds.ExtendedProperties.Item("xcone")
                End If
            End If

            If Not ds Is Nothing Then
                If ds.Tables(Me.DataBindings(0).BindingMemberInfo.BindingPath).Columns(Me.DataBindings(0).BindingMemberInfo.BindingField).DataType Is GetType(System.String) Then
                    Me.MaxLength = ds.Tables(Me.DataBindings(0).BindingMemberInfo.BindingPath).Columns(Me.DataBindings(0).BindingMemberInfo.BindingField).MaxLength
                End If
            End If
        Else
            mCon = Nothing
        End If

        If Not mCon Is Nothing Then
            If Me.mRequiered Then
                mCon.AddRequired(Me)
            End If
        End If

    End Sub

    Private Sub mCon_Changingstate(ByVal sender As Object, ByVal e As XChangeStateEventArgs) Handles mCon.ChangingState
        RefreshState(e)
    End Sub


    Public Sub RefreshState(ByVal e As XChangeStateEventArgs) Implements IEditProperty.RefreshState
        Select Case e.action
            Case LibxConnectionActions.Add
                Me.ReadOnly = Not (Me.NewState = IEditProperty.InitialStateEnum.Enabled)

                If Me.ReadOnly = False Then
                    Me.Text = mNewInitialValue
                End If


            Case LibxConnectionActions.Edit
                Me.ReadOnly = Not (Me.EditState = IEditProperty.InitialStateEnum.Enabled)

                If Me.ReadOnly = False Then
                    If Trim(mEditInitialValue) <> "" Then
                        Me.Text = mEditInitialValue
                    End If
                End If

            Case LibxConnectionActions.Find
                Me.ReadOnly = Not (Me.FindState = IEditProperty.InitialStateEnum.Enabled)

                If Me.ReadOnly = False Then
                    Me.Text = mFindInitialValue
                End If

            Case LibxConnectionActions.None
                Me.ReadOnly = True
                Me.BackColor = System.Drawing.Color.White
            Case Else
                Me.ReadOnly = True
                Me.BackColor = System.Drawing.Color.White
        End Select

        Me.TabStop = Not Me.ReadOnly
        If e.isEditing = True Then
            If Me.ReadOnly = False Then
                Me.BackColor = System.Drawing.Color.White
            Else
                Me.BackColor = System.Drawing.SystemColors.Control
            End If
        End If
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

    Public Sub New()
        MyBase.New()
        Me.AcceptsReturn = True

        '->Me.TabStop = False
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
        Try
            If Not oControlDescription Is Nothing Then
                oControlDescription.Text = Me.mFieldDescripcion
            End If

            If Not mCon Is Nothing Then
                If mCon.IsEditing = True And Me.ReadOnly = False Then
                    Me.BackColor = System.Drawing.Color.LightYellow
                End If
            End If

        Catch ex As Exception
            LibX.Log.Add(ex, True)
        End Try
    End Sub

    Private Sub XTextBox_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Leave
        Try
            If Not mCon Is Nothing Then
                If mCon.IsEditing = True And Me.ReadOnly = False Then
                    Me.BackColor = System.Drawing.Color.White
                End If
            End If

        Catch ex As Exception
            LibX.Log.Add(ex, True)
        End Try

    End Sub

    Private Sub XMaskEdit_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then SendKeys.Send("{TAB}")
        '''If e.KeyCode = Keys.Up Then SendKeys.Send("+{TAB}")
        '''If e.KeyCode = Keys.Down Then SendKeys.Send("{TAB}")

    End Sub

    Private Sub XMaskEdit_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        If e.KeyChar = Chr(13) Then
            e.Handled = True
            Exit Sub
        End If
    End Sub

    Private Sub _BindingObject_Parse(ByVal sender As Object, ByVal e As System.Windows.Forms.ConvertEventArgs) Handles _BindingObject.Parse
        If e.Value.ToString.Trim = "" Then
            e.Value = DBNull.Value
        End If
    End Sub
End Class
