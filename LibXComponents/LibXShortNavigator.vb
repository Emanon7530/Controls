Imports System.ComponentModel


Public Class LibXShortNavigator
    Inherits LibXNavigator

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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        '
        'btnFind
        '
        Me.btnFind.Location = New System.Drawing.Point(124, 0)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(24, 24)
        Me.btnFind.Text = ""
        '
        'btnNew
        '
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(24, 24)
        Me.btnNew.Text = ""
        '
        'btnEdit
        '
        Me.btnEdit.Location = New System.Drawing.Point(24, 0)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(24, 24)
        Me.btnEdit.Text = ""
        '
        'btnDel
        '
        Me.btnDel.Location = New System.Drawing.Point(48, 0)
        Me.btnDel.Name = "btnDel"
        Me.btnDel.Size = New System.Drawing.Size(24, 24)
        Me.btnDel.Text = ""
        '
        'btnPrev
        '
        Me.btnPrev.Location = New System.Drawing.Point(198, 0)
        Me.btnPrev.Name = "btnPrev"
        '
        'btnFirst
        '
        Me.btnFirst.Location = New System.Drawing.Point(174, 0)
        Me.btnFirst.Name = "btnFirst"
        '
        'lblPos
        '
        Me.lblPos.Location = New System.Drawing.Point(222, 0)
        Me.lblPos.Name = "lblPos"
        '
        'btnLast
        '
        Me.btnLast.Location = New System.Drawing.Point(326, 0)
        Me.btnLast.Name = "btnLast"
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(302, 0)
        Me.btnNext.Name = "btnNext"
        '
        'btnOk
        '
        Me.btnOk.Location = New System.Drawing.Point(74, 0)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(24, 24)
        Me.btnOk.Text = ""
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(98, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(24, 24)
        Me.btnCancel.Text = ""
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(148, 0)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(24, 24)
        Me.btnPrint.Text = ""
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(352, 0)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(24, 24)
        Me.btnExit.Tag = ""
        Me.btnExit.Text = ""
        '
        'lblSep1
        '
        Me.lblSep1.Location = New System.Drawing.Point(72, 0)
        Me.lblSep1.Name = "lblSep1"
        '
        'lblSep2
        '
        Me.lblSep2.Location = New System.Drawing.Point(122, 0)
        Me.lblSep2.Name = "lblSep2"
        '
        'lblSep3
        '
        Me.lblSep3.Location = New System.Drawing.Point(172, 0)
        Me.lblSep3.Name = "lblSep3"
        '
        'lblSep4
        '
        Me.lblSep4.Location = New System.Drawing.Point(350, 0)
        Me.lblSep4.Name = "lblSep4"
        '
        'LibXShortNavigator
        '
        Me.Name = "LibXShortNavigator"
        Me.Size = New System.Drawing.Size(744, 24)

    End Sub

#End Region

End Class
