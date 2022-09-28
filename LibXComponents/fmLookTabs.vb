Public Class fmLookTabs
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
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents DGrid As LibX.LibXGrid
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents LibXGrid1 As LibX.LibXGrid
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(fmLookTabs))
        Me.DGrid = New LibX.LibXGrid
        Me.btnOk = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.LibXGrid1 = New LibX.LibXGrid
        CType(Me.DGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.LibXGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DGrid
        '
        Me.DGrid.AutoAdjustLastColumn = True
        Me.DGrid.AutoSearch = True
        Me.DGrid.BackgroundColor = System.Drawing.Color.White
        Me.DGrid.DataMember = ""
        Me.DGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGrid.FullRowSelect = False
        Me.DGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.DGrid.IsReadOnly = False
        Me.DGrid.Location = New System.Drawing.Point(0, 0)
        Me.DGrid.Name = "DGrid"
        Me.DGrid.ReadOnly = True
        Me.DGrid.searchText = ""
        Me.DGrid.showFooterBar = False
        Me.DGrid.Size = New System.Drawing.Size(440, 198)
        Me.DGrid.TabIndex = 0
        Me.DGrid.UseAutoFillLines = True
        Me.DGrid.UseHandCursor = False
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOk.Location = New System.Drawing.Point(288, 244)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(80, 24)
        Me.btnOk.TabIndex = 1
        Me.btnOk.Text = "Ok"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancel.Location = New System.Drawing.Point(376, 244)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 24)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancelar"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Location = New System.Drawing.Point(-16, 232)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(480, 8)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(8, 8)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(448, 224)
        Me.TabControl1.TabIndex = 4
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.DGrid)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(440, 198)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.LibXGrid1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Size = New System.Drawing.Size(440, 198)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        '
        'LibXGrid1
        '
        Me.LibXGrid1.AutoAdjustLastColumn = True
        Me.LibXGrid1.AutoSearch = True
        Me.LibXGrid1.BackgroundColor = System.Drawing.Color.White
        Me.LibXGrid1.DataMember = ""
        Me.LibXGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LibXGrid1.FullRowSelect = False
        Me.LibXGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.LibXGrid1.IsReadOnly = False
        Me.LibXGrid1.Location = New System.Drawing.Point(0, 0)
        Me.LibXGrid1.Name = "LibXGrid1"
        Me.LibXGrid1.ReadOnly = True
        Me.LibXGrid1.searchText = ""
        Me.LibXGrid1.showFooterBar = False
        Me.LibXGrid1.Size = New System.Drawing.Size(440, 198)
        Me.LibXGrid1.TabIndex = 1
        Me.LibXGrid1.UseAutoFillLines = True
        Me.LibXGrid1.UseHandCursor = False
        '
        'fmLookTabs
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(464, 272)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOk)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "fmLookTabs"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Detalle"
        CType(Me.DGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        CType(Me.LibXGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public ok As Boolean

    Public Sub SetDataBinding(ByVal data As DataTable)
        Me.DGrid.SetDataBinding(data, "")
    End Sub

    Public Function Grid() As LibXGrid
        Return Me.DGrid
    End Function

    Public Sub ColumnDbClick(ByVal sender As Object, ByVal e As EventArgs)
        DoOk()
    End Sub

    Private Sub DoOk()
        ok = True
        Close()
    End Sub

    Private Sub fmLook_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ok = False
    End Sub

    Private Sub dgrid_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles DGrid.DoubleClick
        DoOk()
    End Sub

    Private Sub dgrid_cellKeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DGrid.cellKeyPress
        If Not DGrid.isAutoSearching Then

            If e.KeyCode = Keys.Return Then
                DoOk()
            End If
            If e.KeyCode = Keys.Escape And Not DGrid.isAutoSearching Then
                Me.Close()
            End If
        End If
    End Sub


    Private Sub DGrid_Navigate(ByVal sender As System.Object, ByVal ne As System.Windows.Forms.NavigateEventArgs) Handles DGrid.Navigate

    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Me.DoOk()
    End Sub

    Private Sub fmLook_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        DGrid.ResetautoSearch()
    End Sub
End Class
