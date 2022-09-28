Public Class Login
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    Friend WithEvents txtUserName As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnAccept As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(Login))
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnAccept = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'txtUserName
        '
        Me.txtUserName.Location = New System.Drawing.Point(80, 24)
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.Size = New System.Drawing.Size(128, 20)
        Me.txtUserName.TabIndex = 0
        Me.txtUserName.Text = ""
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(80, 48)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(128, 20)
        Me.txtPassword.TabIndex = 0
        Me.txtPassword.Text = ""
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Label1.Location = New System.Drawing.Point(8, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 16)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "User Name:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Label2.Location = New System.Drawing.Point(16, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Password:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(216, 8)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 72)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Location = New System.Drawing.Point(0, 88)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(304, 8)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'btnAccept
        '
        Me.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnAccept.Location = New System.Drawing.Point(144, 104)
        Me.btnAccept.Name = "btnAccept"
        Me.btnAccept.TabIndex = 0
        Me.btnAccept.Text = "Aceptar"
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(224, 104)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancelar"
        '
        'Login
        '
        Me.AcceptButton = Me.btnAccept
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(306, 136)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtUserName)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnAccept)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Login"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Login"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub btnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAccept.Click
        User.UserID = txtUserName.Text

        If LoadUser() = True Then
            Me.Close()
        Else
            Me.DialogResult = DialogResult.Ignore
        End If
    End Sub

    Private Function LoadUser() As Boolean
        Dim SelectStmt As String
        Dim oRow As DataRow
        Dim oRow1 As DataRow
        Try
            SelectStmt = "select * from scusers " & _
                         " where username = '" & txtUserName.Text.Trim & "'" & _
                         " and passwrd = '" & txtPassword.Text.Trim & "'"

            oRow = LibX.Data.Manager.GetDataRow(SelectStmt)

            If oRow Is Nothing Then
                Throw New ApplicationException("Username o password inválido!")
            End If

            SelectStmt = "select vend_code,vend_name,vend_indcajera from ftvendm " & _
                         " where vend_code = " & oRow!vend_code

            oRow1 = LibX.Data.Manager.GetDataRow(SelectStmt)

            If oRow1 Is Nothing Then
                Throw New ApplicationException("Este usuario no esta definido como vendedor!")
            End If

            User.esCajera = Val(oRow1!vend_indcajera.ToString)
            User.UserID = txtUserName.Text
            User.VendedorID = Val(oRow1!vend_code.ToString)
            User.NombreVendedor = oRow1!vend_name.ToString
            User.isLogged = True

            Return True
        Catch ex As Exception
            LibX.Log.Show(ex)
            Return False
        End Try

    End Function
End Class
