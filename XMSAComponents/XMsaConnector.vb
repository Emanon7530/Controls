Imports System.ComponentModel

<ProvideProperty("DataGenerator", GetType(DataSet)), _
 ProvideProperty("DefineAll", GetType(DataSet))> _
Public Class XMsaConnector
    Inherits System.ComponentModel.Component
    Implements IExtenderProvider, IListSource, ISupportInitialize


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

    Protected mDats As New Hashtable
    Protected mDs As New DataSet
    Protected mDt As IList
    Protected mDm As String
    Protected mOnIni As Boolean
    Protected mRealDs As DataSet
    Private WithEvents mOwner As Form
    Event ChangeDetail()

    Public Property OwnerForm() As Form
        Get
            Return mOwner
        End Get
        Set(ByVal Value As Form)
            mOwner = Value
        End Set
    End Property

    Public Overridable Sub RefreshSources()

    End Sub


    Public Overridable Function ResetDataGenerator(ByVal ds As DataSet) As DataSet
        Me.SetDataGenerator(ds, Nothing)
        Return Nothing
    End Function


    Public Overridable Sub SetDefineAll(ByVal ds As DataSet, ByVal value As Boolean)
        Try
            If Not mDats.Contains(ds) Then
                Dim oP As New XConnectorProperties
                oP.Connector = Nothing
                oP.DefineAll = value
                mDats.Add(ds, oP)
            Else
                CType(mDats(ds), XConnectorProperties).DefineAll = value
                'If CType(mDats(ds), XConnectorProperties).Connector Is Nothing Then
                '    CType(mDats(ds), XConnectorProperties).Connector = Me
                'End If
            End If

            RefreshSources()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Public Overridable Function GetDefineAll(ByVal ds As DataSet) As Boolean
        Try
            If mDats.Contains(ds) Then
                Return CType(mDats(ds), XConnectorProperties).DefineAll
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Function

    Public Overridable Sub SetDataGenerator(ByVal ds As DataSet, ByVal value As XMsaConnector)
        Try

            If Not mDats.Contains(ds) Then
                Dim oP As New XConnectorProperties
                oP.Connector = value
                mDats.Add(ds, oP)
            Else
                CType(mDats(ds), XConnectorProperties).Connector = value
            End If

            RefreshSources()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Public Overridable Function GetDataGenerator(ByVal ds As DataSet) As XMsaConnector
        Try
            If mDats.Contains(ds) Then
                Return CType(mDats(ds), XConnectorProperties).Connector
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Function

    Public Function CanExtend(ByVal comp As Object) As Boolean Implements System.ComponentModel.IExtenderProvider.CanExtend
        If TypeOf comp Is DataSet Then
            Return True
        End If
    End Function

    Public ReadOnly Property ContainsListCollection() As Boolean Implements System.ComponentModel.IListSource.ContainsListCollection
        Get
            If TypeOf mDt Is DataSet Then
                Return True
            End If

        End Get
    End Property

    Protected Function GetOrgDs() As DataSet
        If mDs Is Nothing Then
            mDs = New DataSet
        End If
        Return mDs
    End Function

    Public Function GetList() As System.Collections.IList Implements System.ComponentModel.IListSource.GetList
        If mDt Is Nothing Then
            mDt = CType(mDs, IListSource).GetList
        End If

        Return mDt
    End Function

    <Category("Data"), _
    RefreshProperties(RefreshProperties.Repaint), _
    TypeConverter("System.Windows.Forms.Design.DataSourceConverter," & _
    "System.Design")> _
    Public Property DataSource() As Object
        Get
            Return mDs
        End Get
        Set(ByVal Value As Object)
            mDs = Value
            mRealDs = Value
            If Not mRealDs Is Nothing Then
                mRealDs.ExtendedProperties.Add("xcone", Me)
            End If
        End Set
    End Property

    <Category("Data"), _
    Editor("System.Windows.Forms.Design.DataMemberListEditor," & _
    "System.Design", GetType(System.Drawing.Design.UITypeEditor))> _
    Public Property DataMember() As String
        Get
            Return mDm
        End Get
        Set(ByVal Value As String)
            mDm = Value
        End Set
    End Property

    Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
        mOnIni = True
    End Sub

    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        mOnIni = False
        OnInitSettings()

    End Sub

    Protected Overridable Sub OnInitSettings()
        RefreshSources()
    End Sub


    Protected Overridable Sub OnLoadForm()


    End Sub


    Private Sub mOwner_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles mOwner.Load

        OnLoadForm()
    End Sub
End Class

Public Class XConnectorProperties
    Public Connector As XMsaConnector
    Public DefineAll As Boolean
End Class