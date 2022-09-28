Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Reflection
Imports System.Drawing.Design

<ToolboxItem(True), SerializableAttribute()> Public Class LibXRadioGroupBox
    Inherits GroupBox

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

    Dim WithEvents m_objDataSource As DataSet
    Dim m_Sorted As Boolean
    Dim m_SelectedValue As Object
    Dim m_DataMemberName As String
    Dim m_DisplayMemberName As String
    Dim m_ValuemMemberName As String
    Dim m_Orientation As Orientation
    Dim m_Items() As String
    Dim m_strWhereDefault As String
    Dim m_blnRequired As Boolean

    Public Event LoadingItem(ByVal e As LoadItemEventArgs)
    Public Event LoadedItem(ByVal e As LoadItemEventArgs)

    Public Event ChangingItem(ByVal e As ChangeValueEventArgs)
    Public Event ChangedItem(ByVal e As ChangeValueEventArgs)

    Public Sub OnChangingItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim e1 As New ChangeValueEventArgs
        RaiseEvent ChangingItem(e1)

    End Sub

    Public Sub OnChangedItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim e1 As New ChangeValueEventArgs
        RaiseEvent ChangedItem(e1)

    End Sub


    Public Property Items() As String()
        Get
            Return m_Items
        End Get
        Set(ByVal Value As String())
            m_Items = Value
            CreateRadioItem()
        End Set
    End Property

    Public Property DataSource() As DataSet
        Get
            Return m_objDataSource
        End Get
        Set(ByVal Value As DataSet)
            m_objDataSource = Value
        End Set
    End Property

    Public Property DataMemberName() As String
        Get
            Return m_DataMemberName
        End Get
        Set(ByVal Value As String)
            m_DataMemberName = Value
        End Set
    End Property

    Public Property ValueMemberName() As String
        Get
            Return m_ValuemMemberName
        End Get
        Set(ByVal Value As String)
            m_ValuemMemberName = Value
        End Set
    End Property

    Public Property Orientation() As Orientation
        Get
            Return m_Orientation
        End Get
        Set(ByVal Value As Orientation)
            m_Orientation = Value
        End Set
    End Property

    <Bindable(True)> _
    Public Property SelectedValue() As Object
        Get
            Return m_SelectedValue
        End Get
        Set(ByVal Value As Object)
            m_SelectedValue = Value
        End Set
    End Property

    Public Property Sorted() As Boolean
        Get
            Return m_Sorted
        End Get
        Set(ByVal Value As Boolean)
            m_Sorted = Value
        End Set
    End Property

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

    Private Sub CreateRadioItem()
        Dim Radio As RadioButton
        Dim i As Int16
        Try

            Me.SuspendLayout()
            Me.Controls.Clear()

            For i = 0 To m_Items.Length - 1
                Radio = New RadioButton
                Radio.Text = Split(m_Items(i), "-")(1)
                Radio.Visible = True

                AddHandler Radio.Click, AddressOf OnChangedItem
                AddHandler Radio.CheckedChanged, AddressOf OnChangingItem

                Me.Controls.Add(Radio)

                Radio.Top = (20 * i)
            Next

            Me.ResumeLayout()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
