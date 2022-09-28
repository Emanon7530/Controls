Imports System.ComponentModel

Public Class LibXNavigator
    Inherits XMsaComponents.XMsaToolBar

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'UserControl1 overrides dispose to clean up the component list.
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
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'btnFind
        '
        Me.btnFind.CausesValidation = False
        Me.btnFind.Name = "btnFind"
        Me.ToolTip1.SetToolTip(Me.btnFind, "Especificar un criterio para busqueda")
        '
        'btnNew
        '
        Me.btnNew.CausesValidation = False
        Me.btnNew.Name = "btnNew"
        Me.ToolTip1.SetToolTip(Me.btnNew, "Crear un nuevo registro")
        '
        'btnEdit
        '
        Me.btnEdit.CausesValidation = False
        Me.btnEdit.Name = "btnEdit"
        Me.ToolTip1.SetToolTip(Me.btnEdit, "Editar el registro en pantalla")
        '
        'btnDel
        '
        Me.btnDel.CausesValidation = False
        Me.btnDel.Name = "btnDel"
        Me.ToolTip1.SetToolTip(Me.btnDel, "Eliminar el registro en pantalla")
        '
        'btnPrev
        '
        Me.btnPrev.CausesValidation = False
        Me.btnPrev.Name = "btnPrev"
        Me.ToolTip1.SetToolTip(Me.btnPrev, "Ir al Registro anterior")
        '
        'btnFirst
        '
        Me.btnFirst.CausesValidation = False
        Me.btnFirst.Name = "btnFirst"
        Me.ToolTip1.SetToolTip(Me.btnFirst, "Ir al primer registro")
        '
        'lblPos
        '
        Me.lblPos.Name = "lblPos"
        '
        'btnLast
        '
        Me.btnLast.CausesValidation = False
        Me.btnLast.Name = "btnLast"
        Me.ToolTip1.SetToolTip(Me.btnLast, "Ir al último registro")
        '
        'btnNext
        '
        Me.btnNext.CausesValidation = False
        Me.btnNext.Name = "btnNext"
        Me.ToolTip1.SetToolTip(Me.btnNext, "Próximo registro")
        '
        'btnOk
        '
        Me.btnOk.Name = "btnOk"
        Me.ToolTip1.SetToolTip(Me.btnOk, "Acceptar la operación")
        '
        'btnPrint
        '
        Me.btnPrint.CausesValidation = False
        Me.btnPrint.Name = "btnPrint"
        Me.ToolTip1.SetToolTip(Me.btnPrint, "Imprimir")
        '
        'btnExit
        '
        Me.btnExit.CausesValidation = False
        Me.btnExit.Name = "btnExit"
        Me.ToolTip1.SetToolTip(Me.btnExit, "Cerrar esta pantalla")
        '
        'btnCancel
        '
        Me.btnCancel.CausesValidation = False
        Me.btnCancel.Name = "btnCancel"
        Me.ToolTip1.SetToolTip(Me.btnCancel, "Cancelar esta operación")
        '
        'lblSep1
        '
        Me.lblSep1.Name = "lblSep1"
        '
        'lblSep2
        '
        Me.lblSep2.Name = "lblSep2"
        '
        'lblSep3
        '
        Me.lblSep3.Name = "lblSep3"
        '
        'lblSep4
        '
        Me.lblSep4.Name = "lblSep4"
        '
        'LibXNavigator
        '
        Me.Name = "LibXNavigator"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Dim mCon As LibXConnector
    Dim mFromChangeState As Boolean

    Public Event ActionClick(ByVal sender As Object, ByVal e As XMsaComponents.XMsaActionClickEventArgs)
    Public Event AfterClick(ByVal sender As Object, ByVal e As XMsaComponents.XMsaActionClickEventArgs)
    Public Event ButtonsStateChanged(ByVal sender As Object, ByVal e As EventArgs)

    Dim mFirstNewControl As XTextBox
    Dim mFirstEditControl As XTextBox
    Dim mFirstFindControl As XTextBox

    Protected Overrides Sub OnActionClick(ByVal e As XMsaComponents.XMsaActionClickEventArgs)
        Try
            mCon = CType(Me.mConector, LibXConnector)

            RaiseEvent ActionClick(Me, e)

            Select Case e.Action.ButtonAction
                Case XMsaComponents.XmsaButtons.Add
                    mCon.AddNew()
                    If Not mFirstNewControl Is Nothing Then
                        mFirstNewControl.Focus()
                    End If
                Case XMsaComponents.XmsaButtons.Print
                    mCon.Print()

                Case XMsaComponents.XmsaButtons.Accept
                    mCon.Accept()
                Case XMsaComponents.XmsaButtons.Cancel
                    mCon.Cancel()
                Case XMsaComponents.XmsaButtons.Delete
                    mCon.Delete()
                Case XMsaComponents.XmsaButtons.Edit
                    mCon.Edit()
                    If Not mFirstEditControl Is Nothing Then
                        mFirstEditControl.Focus()
                    End If

                Case XMsaComponents.XmsaButtons.Find
                    mCon.Find()
                    If Not mFirstFindControl Is Nothing Then
                        mFirstFindControl.Focus()
                    End If

                Case XMsaComponents.XmsaButtons.First
                    mCon.MoveFirst()
                Case XMsaComponents.XmsaButtons.Last
                    mCon.MoveLast()
                Case XMsaComponents.XmsaButtons.Next
                    mCon.MoveNext()
                Case XMsaComponents.XmsaButtons.Prev
                    mCon.MovePrevious()
                Case XMsaComponents.XmsaButtons.Exit
                    mCon.Exit()
            End Select

            RaiseEvent AfterClick(Me, e)

        Catch ex As Exception
            LibX.Log.Show(ex)
        End Try
    End Sub

    Public Sub UpdateState()
        Try
            Dim recordCount As Long = mCon.RecordCount - 1

            '''If Not Me.HasInitiated Then
            '''    Exit Sub
            '''    'mFromChangeState = True
            '''    'Me.ExecuteInitSetttings()
            '''    'mFromChangeState = False
            '''End If

            If Not mCon.AllowEdit Then
                Me.Buttons(XMsaComponents.XmsaButtons.Edit).Enabled = False
            Else
                Me.Buttons(XMsaComponents.XmsaButtons.Edit).Enabled = Not mCon.IsEditing

            End If
            If Not mCon.AllowNew Then
                Me.Buttons(XMsaComponents.XmsaButtons.Add).Enabled = False
            Else
                Me.Buttons(XMsaComponents.XmsaButtons.Add).Enabled = Not mCon.IsEditing
            End If

            If Not mCon.AllowQuery Then
                Me.Buttons(XMsaComponents.XmsaButtons.Find).Enabled = False
            Else
                Me.Buttons(XMsaComponents.XmsaButtons.Find).Enabled = Not mCon.IsEditing
            End If

            If Not mCon.HasRecords Then
                Me.Buttons(XMsaComponents.XmsaButtons.Print).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.Next).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.Last).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.First).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.Prev).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.Delete).Enabled = False
                Me.Buttons(XMsaComponents.XmsaButtons.Edit).Enabled = False
            Else
                Me.Buttons(XMsaComponents.XmsaButtons.Print).Enabled = Not mCon.IsEditing And mCon.AllowPrint
                Me.Buttons(XMsaComponents.XmsaButtons.Delete).Enabled = Not mCon.IsEditing And mCon.AllowDelete
                Me.Buttons(XMsaComponents.XmsaButtons.First).Enabled = Not mCon.IsEditing And Not mCon.BOF
                Me.Buttons(XMsaComponents.XmsaButtons.Prev).Enabled = Not mCon.IsEditing And mCon.RecordPosition >= 1 And Not mCon.BOF
                Me.Buttons(XMsaComponents.XmsaButtons.Next).Enabled = Not mCon.IsEditing And mCon.RecordPosition < recordCount And Not mCon.EOF
                Me.Buttons(XMsaComponents.XmsaButtons.Last).Enabled = Not mCon.IsEditing And mCon.RecordPosition < recordCount And Not mCon.EOF
            End If

            '-->Botones que se mandar a coloar en un estado obligado
            Me.Buttons(XMsaComponents.XmsaButtons.Accept).Enabled = mCon.IsEditing
            Me.Buttons(XMsaComponents.XmsaButtons.Cancel).Enabled = mCon.IsEditing
            Me.Buttons(XMsaComponents.XmsaButtons.Exit).Enabled = Not mCon.IsEditing


            Dim s As String
            Select Case mCon.State
                Case LibxConnectorState.none
                    s = ""
                Case LibxConnectorState.Edit
                    s = "Editando"
                Case LibxConnectorState.Insert
                    s = "Insertando"
                Case LibxConnectorState.Query
                    s = "Consultando"
                Case LibxConnectorState.View
                    If mCon.RecordCount > 0 Then
                        s = "Visualizando"
                    Else
                        s = ""
                    End If
            End Select

            Me.lblPos.Text = "[" & s & "] [" & Trim(CStr(mCon.RecordPosition + 1)) & "/" & Trim(CStr(Me.mCon.RecordCount)) & "]:" & Trim(CStr(mCon.RecordCountDetail))

            RaiseEvent ButtonsStateChanged(Me, New System.EventArgs)
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    <Browsable(True)> _
    Public Property FirstControlInNewMode() As XTextBox
        Get
            Return mFirstNewControl
        End Get
        Set(ByVal Value As XTextBox)
            mFirstNewControl = Value
        End Set
    End Property

    <Browsable(True)> _
        Public Property FirstControlInEditMode() As XTextBox
        Get
            Return mFirstEditControl
        End Get
        Set(ByVal Value As XTextBox)
            mFirstEditControl = Value
        End Set
    End Property

    <Browsable(True)> _
        Public Property FirstControlInFindMode() As XTextBox
        Get
            Return mFirstFindControl
        End Get
        Set(ByVal Value As XTextBox)
            mFirstFindControl = Value
        End Set
    End Property

    Public Overrides Sub OnConnectorChanged()
        Try
            mCon = CType(Me.mConector, LibXConnector)
            RemoveHandler mCon.UpdatingNavState, AddressOf OnUpdatingNavState
            AddHandler mCon.UpdatingNavState, AddressOf OnUpdatingNavState

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try
    End Sub

    Private Sub OnUpdatingNavState(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Me.UpdateState()
        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try

    End Sub

    Public Overrides Sub OnInitToolSettings()
        Try
            MyBase.OnInitToolSettings()

            Me.Dock = DockStyle.Top

            If Not mCon Is Nothing Then
                mCon.DoChangeState()
            End If

        Catch ex As Exception
            Log.Add(ex) '// CAMBIE
        End Try

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click

    End Sub
End Class
