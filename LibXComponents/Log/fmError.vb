Public Class fmError
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
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnDet As System.Windows.Forms.Button
    Friend WithEvents lblError As System.Windows.Forms.Label
    Friend WithEvents txtError As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(fmError))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.txtError = New System.Windows.Forms.TextBox
        Me.lblError = New System.Windows.Forms.Label
        Me.btnOk = New System.Windows.Forms.Button
        Me.btnDet = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtError)
        Me.GroupBox1.Controls.Add(Me.lblError)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(352, 80)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'txtError
        '
        Me.txtError.Location = New System.Drawing.Point(152, 56)
        Me.txtError.Multiline = True
        Me.txtError.Name = "txtError"
        Me.txtError.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtError.Size = New System.Drawing.Size(72, 8)
        Me.txtError.TabIndex = 1
        Me.txtError.Text = "TextBox1"
        Me.txtError.Visible = False
        '
        'lblError
        '
        Me.lblError.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblError.Location = New System.Drawing.Point(3, 16)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(346, 61)
        Me.lblError.TabIndex = 0
        Me.lblError.Text = "label"
        Me.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.Location = New System.Drawing.Point(160, 96)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(104, 24)
        Me.btnOk.TabIndex = 1
        Me.btnOk.Text = "Ok"
        '
        'btnDet
        '
        Me.btnDet.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDet.Location = New System.Drawing.Point(264, 96)
        Me.btnDet.Name = "btnDet"
        Me.btnDet.Size = New System.Drawing.Size(96, 24)
        Me.btnDet.TabIndex = 2
        Me.btnDet.Text = "Detalle"
        '
        'fmError
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(370, 128)
        Me.Controls.Add(Me.btnDet)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "fmError"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Error"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public Enum EnumErrorType
        [Error]
        Wirning
        Information
        Quetion
    End Enum
    Public mEx As Exception
    Public comment As String
    Public TypeError As EnumErrorType = EnumErrorType.Error

    Public Sub ShowExDet()
        lblError.Visible = False
        Me.txtError.Dock = DockStyle.Fill
        Me.txtError.Visible = True
        Me.txtError.Text = mEx.ToString
        Me.btnDet.Enabled = False
        Me.Height = Me.Height * 2
    End Sub

    Private Sub fmError_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.lblError.Dock = DockStyle.Fill
        Me.lblError.Text = mEx.Message
        If Trim(comment) <> "" Then
            Me.lblError.Text = mEx.Message & vbCrLf & comment
        End If
        Select Case Me.TypeError
            Case EnumErrorType.Wirning
                Me.Text = "Advertencia"
                Me.Icon = Nothing
                Me.btnDet.Visible = False
                Me.btnOk.Location = Me.btnDet.Location
        End Select

    End Sub

    Private Sub btnDet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDet.Click
        Me.ShowExDet()
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Close()
    End Sub
End Class
