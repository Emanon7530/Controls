Imports System.ComponentModel

Public Class LibxDateTimePicker
    Inherits System.Windows.Forms.DateTimePicker
    Implements IEditProperty

    Dim mNewState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mEditState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled
    Dim mFindState As IEditProperty.InitialStateEnum = IEditProperty.InitialStateEnum.Enabled

    Dim mNewInitialValue As String
    Dim mEditInitialValue As String
    Dim mFindInitialValue As String

    Private m_enmOriginalFormat As Windows.Forms.DateTimePickerFormat
    Private m_strOriginalCustomFormat As String
    Private m_blnRefreshing As Boolean = False
    Private mOValue As Object


    Public Event CurrentValueChanged As EventHandler
    Private WithEvents mCon As LibXConnector

#Region "Designer"


    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitializeMembers()

        '// size por defecto
        Me.Size = New Size(121, 21)
    End Sub

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

    'NOTE: The following procedure is required by the Windows FormDesigner
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub
#End Region


    Private mIgnoreRequiered As Boolean
    Private mRequiered As Boolean
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

    Protected Overridable Sub OnCurrentValueChanged(ByVal e As EventArgs)
        RaiseEvent CurrentValueChanged(Me, e)
    End Sub


    <Bindable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
     Public Shadows Property Value() As Object
        Get
            If Not Me.Checked Then
                Return (DBNull.Value)
            Else
                Return MyBase.Value.ToString("d")
            End If
        End Get
        Set(ByVal newValue As Object)

            Dim blnRaiseEvent As Boolean = False

            If newValue Is DBNull.Value Then
                If Me.Checked Then

                    Me.Checked = False
                    Me.RefreshText()
                    blnRaiseEvent = True
                End If

            ElseIf IsDate(newValue) Then

                blnRaiseEvent = (Not Me.Checked) And (CType(newValue, Date).Equals(Me.Value))

                Me.Checked = True
                MyBase.Value = CType(newValue, Date).ToString("d")
                Me.RefreshText()

            Else
                Throw New ArgumentException
            End If

            If blnRaiseEvent Then
                Me.OnCurrentValueChanged(New EventArgs)
            End If

        End Set
    End Property


    Protected Overrides Sub OnValueChanged(ByVal e As System.EventArgs)

        Me.RefreshText()

        Me.OnCurrentValueChanged(e)

        MyBase.OnValueChanged(e)
    End Sub

    Protected Overrides Sub OnFormatChanged(ByVal e As System.EventArgs)

        If Not m_blnRefreshing Then
            Me.SaveOriginalFormats()
        End If

        MyBase.OnFormatChanged(e)
    End Sub

    Public Overrides Sub Refresh()
        Me.RefreshText()

        MyBase.Refresh()
    End Sub

    Private Sub InitializeMembers()
        Me.SaveOriginalFormats()

        Me.Format = DateTimePickerFormat.Custom
        Me.CustomFormat = "dd/MM/yyyy"
        MyBase.ShowCheckBox = False
    End Sub

    Private Sub SaveOriginalFormats()
        m_enmOriginalFormat = Me.Format
        '-->m_strOriginalCustomFormat = Me.CustomFormat
        m_strOriginalCustomFormat = "dd/MM/yyyy"
    End Sub

    Private Sub RestoreOriginalFormats()
        Me.CustomFormat = m_strOriginalCustomFormat
        Me.Format = m_enmOriginalFormat
    End Sub

    Private Sub RefreshText()

        m_blnRefreshing = True

        If Me.Checked Then
            Me.RestoreOriginalFormats()
        Else
            Me.Format = Windows.Forms.DateTimePickerFormat.Custom
            Me.CustomFormat = " "
        End If

        m_blnRefreshing = False

    End Sub

    '''

    Protected Overrides Sub OnBindingContextChanged(ByVal e As System.EventArgs)
        Try
            MyBase.OnBindingContextChanged(e)

            If Me.DesignMode Then
                Exit Sub
            End If
            If Me.DataBindings.Count > 0 Then
                If TypeOf DataBindings(0).DataSource Is LibXConnector Then
                    mCon = DataBindings(0).DataSource
                Else
                    Dim ds As DataSet = DataBindings(0).DataSource
                    If Not ds.ExtendedProperties Is Nothing AndAlso ds.ExtendedProperties.Count > 0 Then
                        mCon = ds.ExtendedProperties.Item("xcone")
                    End If
                End If
            End If

            If Not mCon Is Nothing Then
                If Me.mRequiered Then
                    mCon.AddRequired(Me)
                End If
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Private Sub mCon_Changingstate(ByVal sender As Object, ByVal e As XChangeStateEventArgs) Handles mCon.ChangingState
        RefreshState(e)
    End Sub


    Public Sub RefreshState(ByVal e As XChangeStateEventArgs) Implements IEditProperty.RefreshState
        Select Case e.action
            Case LibxConnectionActions.Add
                Me.Enabled = (Me.NewState = IEditProperty.InitialStateEnum.Enabled)
                If Me.Enabled = True Then
                    If Trim(NewInitialValue) <> "" Then
                        If Trim(Me.NewInitialValue).ToLower = "now" Or Trim(Me.NewInitialValue).ToLower = "today" Then
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                        Else
                            Me.Value = Me.NewInitialValue
                            Me.Value = Me.NewInitialValue
                        End If
                    End If
                    Me.BackColor = System.Drawing.Color.LightYellow
                Else
                    Me.BackColor = System.Drawing.Color.White
                End If

            Case LibxConnectionActions.Edit
                Me.Enabled = (Me.EditState = IEditProperty.InitialStateEnum.Enabled)
                If Me.Enabled = True Then
                    If Trim(EditInitialValue) <> "" Then
                        If Trim(EditInitialValue).ToLower = "now" Or Trim(EditInitialValue).ToLower = "today" Then
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                        Else
                            Me.Value = EditInitialValue
                        End If
                    End If
                    Me.BackColor = System.Drawing.Color.LightYellow
                Else
                    Me.BackColor = System.Drawing.Color.White
                End If



            Case LibxConnectionActions.Find
                Me.Enabled = (Me.FindState = IEditProperty.InitialStateEnum.Enabled)
                If Me.Enabled = True Then
                    If Trim(FindInitialValue) <> "" Then
                        If Trim(FindInitialValue).ToLower = "now" Or Trim(FindInitialValue).ToLower = "today" Then
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                            Me.Value = LibX.Data.Manager.Connection.GetDate.ToString("d")
                        Else
                            Me.Value = FindInitialValue
                            Me.Value = FindInitialValue
                        End If
                    End If
                    Me.BackColor = System.Drawing.Color.LightYellow
                Else
                    Me.BackColor = System.Drawing.Color.White
                End If

            Case LibxConnectionActions.None
                Me.Enabled = False
                Me.BackColor = System.Drawing.Color.White

            Case Else
                Me.Enabled = False
                Me.BackColor = System.Drawing.Color.White

        End Select
        Me.TabStop = Me.Enabled

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

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If msg.WParam.ToInt32 = Keys.Enter Then
            SendKeys.Send("{Tab}")
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub XTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.GotFocus
        Try

            If Not oControlDescription Is Nothing Then
                oControlDescription.Text = Me.mFieldDescripcion
            End If

            If Not mCon Is Nothing Then
                If mCon.IsEditing = True Then
                    If Me.TabStop = False Then
                        SendKeys.Send("{TAB}")
                    End If
                End If

                If mCon.IsEditing = True And Me.Enabled = True Then
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
                If mCon.IsEditing = True And Me.Enabled = True Then
                    Me.BackColor = System.Drawing.Color.White
                End If
            End If

        Catch ex As Exception
            LibX.Log.Add(ex, True)
        End Try

    End Sub

End Class


