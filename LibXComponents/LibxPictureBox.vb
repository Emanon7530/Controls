Imports System.ComponentModel
Imports System.IO
Public Class LibxPictureBox
    Inherits System.Windows.Forms.PictureBox
    Implements LibX.IEditProperty


    Dim _EditState As IEditProperty.InitialStateEnum
    Dim _InsertState As IEditProperty.InitialStateEnum

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
    <Bindable(True)> _
    Public Property value() As Object
        Get
            Dim oImageByte As Byte()
            Dim oImageMS As MemoryStream

            oImageMS = New MemoryStream
            Me.Image.Save(oImageMS, Me.Image.RawFormat)

            oImageByte = oImageMS.GetBuffer
            m_objValue = oImageByte

            Return m_objValue
        End Get

        Set(ByVal Value As Object)
            If m_blnLeaving Then
                m_blnLeaving = False
                If Not m_objValue Is Nothing Then
                    If m_objValue.ToString() <> "" Then
                        If m_objDataSource.IsDataEditing Then
                            If Me.DataBindings.Count > 0 Then
                                Dim objRow As DataRowView
                                Dim oImageByte As Byte()
                                Dim oImageMS As MemoryStream

                                If Not Me.DataBindings(0).BindingManagerBase.Current Is Nothing Then
                                    objRow = CType(Me.DataBindings(0).BindingManagerBase.Current, DataRowView)
                                    oImageMS = New MemoryStream
                                    Me.Image.Save(oImageMS, Me.Image.RawFormat)

                                    oImageByte = oImageMS.GetBuffer
                                    m_objValue = oImageByte
                                    objRow(Me.DataBindings(0).BindingMemberInfo.BindingField) = oImageByte
                                End If
                            End If
                        End If
                    End If
                End If
                Exit Property
            End If

            Me.Refresh()
        End Set
    End Property
#End Region
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


    Public Property EditState() As IEditProperty.InitialStateEnum Implements IEditProperty.EditState
        Get
            Return _EditState
        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)
            _EditState = Value
        End Set
    End Property

    Public Property FindInitialValue() As String Implements IEditProperty.FindInitialValue
        Get

        End Get
        Set(ByVal Value As String)

        End Set
    End Property

    Public Property FindState() As IEditProperty.InitialStateEnum Implements IEditProperty.FindState
        Get

        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)

        End Set
    End Property

    Public Property IgnoreRequiered() As Boolean Implements IEditProperty.IgnoreRequiered
        Get

        End Get
        Set(ByVal Value As Boolean)

        End Set
    End Property

    Public Property NewInitialValue() As String Implements IEditProperty.NewInitialValue
        Get

        End Get
        Set(ByVal Value As String)

        End Set
    End Property

    Public Property NewState() As IEditProperty.InitialStateEnum Implements IEditProperty.NewState
        Get
            Return _InsertState
        End Get
        Set(ByVal Value As IEditProperty.InitialStateEnum)
            _InsertState = Value
        End Set
    End Property

    Public Sub RefreshState(ByVal e As XChangeStateEventArgs) Implements IEditProperty.RefreshState
        Select Case e.action
            Case LibxConnectionActions.Add
                Me.Enabled = (Me.NewState = IEditProperty.InitialStateEnum.Enabled)

            Case LibxConnectionActions.Edit
                Me.Enabled = (Me.EditState = IEditProperty.InitialStateEnum.Enabled)

            Case LibxConnectionActions.Find
                Me.Enabled = False

            Case LibxConnectionActions.None
                Me.Enabled = False

            Case Else
                Me.Enabled = False

        End Select

    End Sub

    Public Property Requiered() As Boolean Implements IEditProperty.Requiered
        Get

        End Get
        Set(ByVal Value As Boolean)

        End Set
    End Property

    Public Property EditInitialValue() As String Implements IEditProperty.EditInitialValue
        Get

        End Get
        Set(ByVal Value As String)

        End Set
    End Property

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

        Catch ex As Exception
            Log.Add(ex, Me.Name)
        End Try
    End Sub

End Class
