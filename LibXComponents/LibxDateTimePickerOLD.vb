Imports System.ComponentModel

Public Class LibxDateTimePicker
    Inherits System.Windows.Forms.DateTimePicker

    Private m_enmOriginalFormat As Windows.Forms.DateTimePickerFormat
    Private m_strOriginalCustomFormat As String
    Private m_blnRefreshing As Boolean = False


    Public Event CurrentValueChanged As EventHandler

#Region "Designer"


    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitializeMembers()
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

    Protected Overridable Sub OnCurrentValueChanged(ByVal e As EventArgs)
        RaiseEvent CurrentValueChanged(Me, e)
    End Sub


    <Bindable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
     Public Shadows Property Value() As Object
        Get
            If Not Me.Checked Then
                Return (DBNull.Value)
            Else
                Return (MyBase.Value)
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
                MyBase.Value = CType(newValue, Date)
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

End Class


