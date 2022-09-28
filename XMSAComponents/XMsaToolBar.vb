Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel


'''Public Enum XmsaButtons
'''    Add = 1
'''    Delete = 2
'''    Find = 3
'''    Edit = 4
'''    [Next] = 5
'''    Prev = 6
'''    First = 7
'''    Last = 8
'''    Print = 9
'''    Accept = 10
'''    Cancel = 11
'''    [Exit] = 12
'''End Enum

''Public Enum XmsaButtons
''    Add = 1
''    Edit = 2
''    Delete = 3
''    Accept = 4
''    Cancel = 5
''    Find = 6
''    Print = 7
''    First = 8
''    Prev = 9
''    [Next] = 10
''    Last = 11
''    [Exit] = 12
''End Enum

Public Enum XmsaButtons
    Add = 1
    Edit = 2
    Delete = 3
    Accept = 5
    Cancel = 6
    Find = 8
    Print = 9
    First = 11
    Prev = 12
    [Next] = 13
    Last = 14
    [Exit] = 16
End Enum

''' <summary>
'''  Representa un ToolBar estático. Este ToolBar es un navegador de cualquier datasource. Pero 
'''  que a su vez es un datasource. El proceso es que los controles se enlazan a este como datasource
'''  y un dataset a éste.
''' </summary>
Public Class XMsaToolBar
    Inherits System.Windows.Forms.UserControl
    '    Implements IListSource

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
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Protected WithEvents btnFind As System.Windows.Forms.Button
    Protected WithEvents btnNew As System.Windows.Forms.Button
    Protected WithEvents btnEdit As System.Windows.Forms.Button
    Protected WithEvents btnDel As System.Windows.Forms.Button
    Protected WithEvents btnPrev As System.Windows.Forms.Button
    Protected WithEvents btnFirst As System.Windows.Forms.Button
    Protected WithEvents lblPos As System.Windows.Forms.Label
    Protected WithEvents btnLast As System.Windows.Forms.Button
    Protected WithEvents btnNext As System.Windows.Forms.Button
    Protected WithEvents btnOk As System.Windows.Forms.Button
    Protected WithEvents btnCancel As System.Windows.Forms.Button
    Protected WithEvents btnPrint As System.Windows.Forms.Button
    Protected WithEvents btnExit As System.Windows.Forms.Button
    Protected WithEvents lblSep1 As System.Windows.Forms.Label
    Protected WithEvents lblSep2 As System.Windows.Forms.Label
    Protected WithEvents lblSep3 As System.Windows.Forms.Label
    Protected WithEvents lblSep4 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(XMsaToolBar))
        Me.btnFind = New System.Windows.Forms.Button
        Me.btnNew = New System.Windows.Forms.Button
        Me.btnEdit = New System.Windows.Forms.Button
        Me.btnDel = New System.Windows.Forms.Button
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnFirst = New System.Windows.Forms.Button
        Me.lblPos = New System.Windows.Forms.Label
        Me.btnLast = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.btnOk = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnPrint = New System.Windows.Forms.Button
        Me.btnExit = New System.Windows.Forms.Button
        Me.lblSep1 = New System.Windows.Forms.Label
        Me.lblSep2 = New System.Windows.Forms.Label
        Me.lblSep3 = New System.Windows.Forms.Label
        Me.lblSep4 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btnFind
        '
        Me.btnFind.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnFind.CausesValidation = False
        Me.btnFind.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFind.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFind.Image = CType(resources.GetObject("btnFind.Image"), System.Drawing.Image)
        Me.btnFind.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFind.Location = New System.Drawing.Point(316, 0)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(64, 24)
        Me.btnFind.TabIndex = 5
        Me.btnFind.Text = "Buscar"
        Me.btnFind.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnNew.CausesValidation = False
        Me.btnNew.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNew.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNew.Image = CType(resources.GetObject("btnNew.Image"), System.Drawing.Image)
        Me.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNew.Location = New System.Drawing.Point(0, 0)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(64, 24)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "Agregar"
        Me.btnNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnEdit
        '
        Me.btnEdit.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnEdit.CausesValidation = False
        Me.btnEdit.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEdit.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEdit.Image = CType(resources.GetObject("btnEdit.Image"), System.Drawing.Image)
        Me.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnEdit.Location = New System.Drawing.Point(64, 0)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(56, 24)
        Me.btnEdit.TabIndex = 1
        Me.btnEdit.Text = "Editar"
        Me.btnEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnDel
        '
        Me.btnDel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnDel.CausesValidation = False
        Me.btnDel.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDel.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDel.Image = CType(resources.GetObject("btnDel.Image"), System.Drawing.Image)
        Me.btnDel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDel.Location = New System.Drawing.Point(120, 0)
        Me.btnDel.Name = "btnDel"
        Me.btnDel.Size = New System.Drawing.Size(56, 24)
        Me.btnDel.TabIndex = 2
        Me.btnDel.Text = "Borrar"
        Me.btnDel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ImageList1
        '
        Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'btnPrev
        '
        Me.btnPrev.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnPrev.CausesValidation = False
        Me.btnPrev.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrev.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrev.Location = New System.Drawing.Point(470, 0)
        Me.btnPrev.Name = "btnPrev"
        Me.btnPrev.Size = New System.Drawing.Size(24, 24)
        Me.btnPrev.TabIndex = 8
        Me.btnPrev.Text = "<"
        '
        'btnFirst
        '
        Me.btnFirst.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnFirst.CausesValidation = False
        Me.btnFirst.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFirst.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFirst.Location = New System.Drawing.Point(446, 0)
        Me.btnFirst.Name = "btnFirst"
        Me.btnFirst.Size = New System.Drawing.Size(24, 24)
        Me.btnFirst.TabIndex = 7
        Me.btnFirst.Text = "<<"
        '
        'lblPos
        '
        Me.lblPos.BackColor = System.Drawing.Color.CornflowerBlue
        Me.lblPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPos.Dock = System.Windows.Forms.DockStyle.Left
        Me.lblPos.Font = New System.Drawing.Font("Arial", 8.0!)
        Me.lblPos.ForeColor = System.Drawing.Color.White
        Me.lblPos.Location = New System.Drawing.Point(494, 0)
        Me.lblPos.Name = "lblPos"
        Me.lblPos.Size = New System.Drawing.Size(80, 24)
        Me.lblPos.TabIndex = 14
        Me.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnLast
        '
        Me.btnLast.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnLast.CausesValidation = False
        Me.btnLast.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLast.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLast.Location = New System.Drawing.Point(598, 0)
        Me.btnLast.Name = "btnLast"
        Me.btnLast.Size = New System.Drawing.Size(24, 24)
        Me.btnLast.TabIndex = 10
        Me.btnLast.Text = ">>"
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnNext.CausesValidation = False
        Me.btnNext.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNext.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNext.Location = New System.Drawing.Point(574, 0)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(24, 24)
        Me.btnNext.TabIndex = 9
        Me.btnNext.Text = ">"
        '
        'btnOk
        '
        Me.btnOk.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnOk.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOk.Location = New System.Drawing.Point(178, 0)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(64, 24)
        Me.btnOk.TabIndex = 3
        Me.btnOk.Text = "Aceptar"
        Me.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnCancel.CausesValidation = False
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.Image = CType(resources.GetObject("btnCancel.Image"), System.Drawing.Image)
        Me.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancel.Location = New System.Drawing.Point(242, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(72, 24)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancelar"
        Me.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnPrint.CausesValidation = False
        Me.btnPrint.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.Image = CType(resources.GetObject("btnPrint.Image"), System.Drawing.Image)
        Me.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPrint.Location = New System.Drawing.Point(380, 0)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(64, 24)
        Me.btnPrint.TabIndex = 6
        Me.btnPrint.Text = "Imprimir"
        Me.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnExit
        '
        Me.btnExit.BackColor = System.Drawing.Color.WhiteSmoke
        Me.btnExit.CausesValidation = False
        Me.btnExit.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExit.Image = CType(resources.GetObject("btnExit.Image"), System.Drawing.Image)
        Me.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnExit.Location = New System.Drawing.Point(624, 0)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(64, 24)
        Me.btnExit.TabIndex = 11
        Me.btnExit.Text = "Cerrar"
        Me.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSep1
        '
        Me.lblSep1.BackColor = System.Drawing.Color.CornflowerBlue
        Me.lblSep1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSep1.Dock = System.Windows.Forms.DockStyle.Left
        Me.lblSep1.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSep1.ForeColor = System.Drawing.Color.White
        Me.lblSep1.Location = New System.Drawing.Point(176, 0)
        Me.lblSep1.Name = "lblSep1"
        Me.lblSep1.Size = New System.Drawing.Size(2, 24)
        Me.lblSep1.TabIndex = 21
        Me.lblSep1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSep2
        '
        Me.lblSep2.BackColor = System.Drawing.Color.CornflowerBlue
        Me.lblSep2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSep2.Dock = System.Windows.Forms.DockStyle.Left
        Me.lblSep2.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSep2.ForeColor = System.Drawing.Color.White
        Me.lblSep2.Location = New System.Drawing.Point(314, 0)
        Me.lblSep2.Name = "lblSep2"
        Me.lblSep2.Size = New System.Drawing.Size(2, 24)
        Me.lblSep2.TabIndex = 22
        Me.lblSep2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSep3
        '
        Me.lblSep3.BackColor = System.Drawing.Color.CornflowerBlue
        Me.lblSep3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSep3.Dock = System.Windows.Forms.DockStyle.Left
        Me.lblSep3.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSep3.ForeColor = System.Drawing.Color.White
        Me.lblSep3.Location = New System.Drawing.Point(444, 0)
        Me.lblSep3.Name = "lblSep3"
        Me.lblSep3.Size = New System.Drawing.Size(2, 24)
        Me.lblSep3.TabIndex = 23
        Me.lblSep3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSep4
        '
        Me.lblSep4.BackColor = System.Drawing.Color.CornflowerBlue
        Me.lblSep4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSep4.Dock = System.Windows.Forms.DockStyle.Left
        Me.lblSep4.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSep4.ForeColor = System.Drawing.Color.White
        Me.lblSep4.Location = New System.Drawing.Point(622, 0)
        Me.lblSep4.Name = "lblSep4"
        Me.lblSep4.Size = New System.Drawing.Size(2, 24)
        Me.lblSep4.TabIndex = 24
        Me.lblSep4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'XMsaToolBar
        '
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.lblSep4)
        Me.Controls.Add(Me.btnLast)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.lblPos)
        Me.Controls.Add(Me.btnPrev)
        Me.Controls.Add(Me.btnFirst)
        Me.Controls.Add(Me.lblSep3)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.btnFind)
        Me.Controls.Add(Me.lblSep2)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblSep1)
        Me.Controls.Add(Me.btnDel)
        Me.Controls.Add(Me.btnEdit)
        Me.Controls.Add(Me.btnNew)
        Me.Name = "XMsaToolBar"
        Me.Size = New System.Drawing.Size(808, 24)
        Me.ResumeLayout(False)

    End Sub

#End Region


    Private mBts As New Hashtable
    Private mBCts As New Hashtable
    Public Const LeftMargin As Integer = 2
    Public Const TextMargin As Integer = 2

    Private mFrmOwn As Form
    Private _NormalBorderColor As Color = Color.WhiteSmoke
    Private _NormalColorA As Color = Color.WhiteSmoke
    'Private _NormalColorB As Color = Color.DarkSlateGray
    Private _NormalColorB As Color = Color.Gray

    Private dt As IList
    Private ds As DataSet
    Dim mdatamember As String
    Protected mConector As XMsaConnector

    Public Event InitToolSettings(ByVal sender As Object, ByVal e As EventArgs)


    Private mUseMenu As Boolean = True

    Protected WithEvents mnuAdd As XMsaMenuItem
    Protected WithEvents mnuEdit As XMsaMenuItem
    Protected WithEvents mnuDelete As XMsaMenuItem
    Protected WithEvents mnuSep1 As XMsaMenuItem
    Protected WithEvents mnuAccept As XMsaMenuItem
    Protected WithEvents mnuCancel As XMsaMenuItem
    Protected WithEvents mnuSep2 As XMsaMenuItem
    Protected WithEvents mnufind As XMsaMenuItem
    Protected WithEvents mnuPrint As XMsaMenuItem
    Protected WithEvents mnuSep3 As XMsaMenuItem
    Protected WithEvents mnuNext As XMsaMenuItem
    Protected WithEvents mnuPrev As XMsaMenuItem
    Protected WithEvents mnuFirst As XMsaMenuItem
    Protected WithEvents mnuLast As XMsaMenuItem
    Protected WithEvents mnuBrow As XMsaMenuItem
    Protected WithEvents mnuSep4 As XMsaMenuItem
    Protected WithEvents mnuExit As XMsaMenuItem

    Private mM As New MainMenu
    Private mHasInitiated As Boolean


    Public Property BuildMenu() As Boolean
        Get
            Return mUseMenu
        End Get
        Set(ByVal Value As Boolean)
            mUseMenu = Value
        End Set
    End Property

    Private Sub Button1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles btnFind.Paint, btnNew.Paint, btnEdit.Paint, btnDel.Paint, btnNext.Paint, btnFirst.Paint, btnLast.Paint, btnPrev.Paint, btnOk.Paint, btnPrint.Paint, btnCancel.Paint, btnExit.Paint
        'Try
        Dim brush As LinearGradientBrush
        Dim mode As LinearGradientMode
        Dim newRect As Rectangle
        Dim iPoint As Point
        Dim tPoint As Point
        Dim tf As StringFormat

        Dim ob As Button = CType(sender, Button)

        GetPoints(ob, iPoint, tPoint, tf, e.Graphics)

        Dim mPt As Point = Me.PointToClient(Control.MousePosition)
        Dim pressed As Boolean = (Control.MouseButtons = MouseButtons.Left And ob.ClientRectangle.Contains(mPt))
        Dim bMouseInButton As Boolean = ob.Bounds.Contains(mPt)


        If (pressed) Then
            _NormalColorB = Color.DarkGray
        Else
            If bMouseInButton Then
                _NormalColorB = Color.DarkGray
            Else
                _NormalColorB = Color.Gray
            End If
        End If

        e.Graphics.SmoothingMode = SmoothingMode.HighQuality

        'Dim p As Point

        newRect = ob.ClientRectangle  ' New Rectangle(p.X, p.Y, ob.Size.Width, ob.Size.Height)

        mode = LinearGradientMode.Vertical
        brush = New LinearGradientBrush(newRect, _NormalColorA, _NormalColorB, mode)

        Dim pW As New Pen(Color.White)
        Dim pG As New Pen(Color.DarkGray)


        e.Graphics.FillRectangle(brush, newRect)


        e.Graphics.DrawLine(pG, 0, 0, 0, newRect.Height + 1)

        'e.Graphics.DrawLine(pW, 1, 1, 1, newRect.Height + 1)
        'e.Graphics.DrawLine(pW, newRect.Width - 1, 1, newRect.Width - 1, newRect.Height + 1)
        If sender Is btnExit Then
            e.Graphics.DrawLine(pG, newRect.Width - 1, 0, newRect.Width - 1, newRect.Height + 1)
        End If


        If Not ob.Image Is Nothing Then
            If ob.Enabled Then
                'e.Graphics.DrawImage(CType(sender, Button).Image, newRect)
                e.Graphics.DrawImage(ob.Image, iPoint)
            Else
                ControlPaint.DrawImageDisabled(e.Graphics, ob.Image, iPoint.X, iPoint.Y, Color.Transparent)
            End If
        End If


        DrawText(e.Graphics, sender, tPoint, tf)

        pW.Dispose()
        pG.Dispose()
        brush.Dispose()
        pW = Nothing
        pG = Nothing
        brush = Nothing


        'Catch
        '    MyBase.OnPaint(e)
        'End Try

    End Sub



    Dim ButtonShadowOffset As Integer = 5

    Private Sub GetPoints(ByVal but As Button, ByRef iPoint As Point, ByRef tPoint As Point, ByRef tf As StringFormat, ByVal g As Graphics)
        Try
            Dim x As Integer = but.Width
            Dim y As Integer = but.Height

            tf = New StringFormat
            tf.FormatFlags = StringFormatFlags.FitBlackBox
            'tf.Alignment = StringAlignment.Center
            'tf.LineAlignment = StringAlignment.Center

            If Not but.Image Is Nothing Then
                If but.Text.Length = 0 Then
                    iPoint = New Point((x - but.Image.Width) / 2, (y - but.Image.Height) / 2)
                Else
                    iPoint = New Point(LeftMargin, (y - but.Image.Height) / 2)
                End If

                tPoint = New Point(LeftMargin + but.Image.Width + TextMargin, (y - but.Font.Height) / 2)
            Else
                'Dim size As Size = GetTextSize(but, but.CreateGraphics(), but.Text.Replace("&", ""), but.Font, New Size(x, y), tf)
                Dim size As Size = GetTextSize(but, g, but.Text, but.Font, New Size(x, y))
                tPoint = New Point((x - size.Width - 2) / 2, (y - but.Font.Height) / 2)
            End If

            tf.Dispose()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Function GetTextSize(ByVal but As Button, ByVal g As Graphics, ByVal text As String, ByVal font As Font, ByVal sz As Size) As Size
        Try
            If but.Text.Length = 0 Then
                Return Size.Empty
            End If

            Dim Format As StringFormat = New StringFormat
            Format.FormatFlags = StringFormatFlags.FitBlackBox

            Dim layoutRect As RectangleF = New System.Drawing.RectangleF(0, 0, sz.Width, sz.Height)
            Dim chRange() As CharacterRange = New CharacterRange() {New CharacterRange(0, but.Text.Length)}
            Dim regs() As Region '= New Region() {}

            Format.SetMeasurableCharacterRanges(chRange)

            regs = g.MeasureCharacterRanges(but.Text, but.Font, layoutRect, Format)
            Dim rect As Rectangle = Rectangle.Round(regs(0).GetBounds(g))

            Return New Size(rect.Width, rect.Height)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Function


    Private Sub DrawText(ByVal g As Graphics, ByVal button As Button, ByVal tPoint As Point, ByVal tf As StringFormat)
        'Try
        If button.Text Is Nothing Or button.Text.Trim.Length = 0 Then
            Exit Sub
        End If

        Dim layoutRect As RectangleF = _
        New RectangleF(0, 0, button.Width, _
         button.Height - ButtonShadowOffset)

        Dim LabelShadowOffset As Integer = 1

        Dim fmt As StringFormat = New StringFormat
        fmt.Alignment = StringAlignment.Center
        fmt.LineAlignment = StringAlignment.Center

        layoutRect.Offset(0, -LabelShadowOffset)
        Dim brushFiller As SolidBrush
        'If button.Enabled Then
        brushFiller = New SolidBrush(Color.Black)
        'Else
        '    brushFiller = New SolidBrush(SystemColors.InactiveCaptionText)
        'End If
        'g.DrawString(button.Text, button.Font, brushFiller, layoutRect, fmt)
        'tf.Alignment = StringAlignment.Center

        If button.Enabled Then
            g.DrawString(button.Text, button.Font, brushFiller, tPoint.X, tPoint.Y, tf)
        Else

            'layoutRect = New RectangleF(0, 0, button.Width, button.Height - ButtonShadowOffset)
            '--ControlPaint.DrawStringDisabled(g, button.Text, button.Font, Color.Black, layoutRect, tf)

            brushFiller = New SolidBrush(SystemColors.InactiveCaptionText)
            g.DrawString(button.Text, button.Font, brushFiller, tPoint.X, tPoint.Y, tf)
        End If
        brushFiller.Dispose()
        fmt.Dispose()

        brushFiller = Nothing
        fmt = Nothing

        'Catch ex As Exception
        '    MsgBox(ex.ToString)
        'End Try
    End Sub


    Private Sub CsgToolBar_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Me.Height = 24
    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)
        'Try
        'MyBase.OnPaintBackground(e)

        'Exit Sub

        Dim brush As LinearGradientBrush
        Dim mode As LinearGradientMode
        Dim newRect As Rectangle



        e.Graphics.SmoothingMode = SmoothingMode.HighQuality

        newRect = ClientRectangle ' pevent.ClipRectangle toda el area, no solo la tocada

        If newRect.Width <= 0 Or newRect.Height <= 0 Then
            Exit Sub
        End If

        mode = LinearGradientMode.Vertical
        brush = New LinearGradientBrush(newRect, _NormalColorA, _NormalColorB, mode)

        Dim pW As New Pen(Color.White)
        Dim pG As New Pen(Color.DarkGray)


        e.Graphics.FillRectangle(brush, newRect)

        e.Graphics.DrawLine(pG, 0, 0, newRect.Width + 1, 0)
        e.Graphics.DrawLine(pG, 0, newRect.Height - 1, newRect.Width + 1, newRect.Height - 1)

        pG.Dispose()
        pW.Dispose()

        pG = Nothing
        pW = Nothing
        brush.Dispose()
        brush = Nothing


        'Catch ex As Exception

        'End Try
    End Sub

    Protected Overridable Sub OnActionClick(ByVal e As XMsaActionClickEventArgs)

    End Sub

    Private Sub MnuItemClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim o As New XMsaActionClickEventArgs
        o.Action = sender.tag
        o.ButtonAction = CType(sender.tag, XMsaButton).ButtonAction
        OnActionClick(o)
    End Sub

    Public Sub ButtonClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim o As New XMsaActionClickEventArgs
        o.Action = sender.tag
        o.ButtonAction = CType(sender.tag, XMsaButton).ButtonAction
        OnActionClick(o)
        End Sub

    Public Overridable Sub OnInitToolSettings()
        Try
            mBts = New Hashtable
            Dim oX As New XMsaButton

            '// Main Menu
            Dim oI As New XMsaMenuItem
            oI.Text = "Acciones"
            mM.MenuItems.Add(oI)

            '// Agregar
            mnuAdd = New XMsaMenuItem
            mnuAdd.Text = Me.btnNew.Text
            mnuAdd.Imagen = btnNew.Image
            mnuAdd.Shortcut = Shortcut.F2 '//F2 - Agregar

            oX = New XMsaButton
            oX.MnuItem = mnuAdd
            oX.Button = btnNew
            oX.ButtonAction = XmsaButtons.Add
            mBts.Add(XmsaButtons.Add, oX)

            mnuAdd.tag = oX
            btnNew.Tag = oX
            RemoveHandler mnuAdd.Click, AddressOf MnuItemClick
            RemoveHandler btnNew.Click, AddressOf ButtonClick
            AddHandler mnuAdd.Click, AddressOf MnuItemClick
            AddHandler btnNew.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuAdd)

            '// Editar
            mnuEdit = New XMsaMenuItem
            mnuEdit.Text = Me.btnEdit.Text
            mnuEdit.Imagen = btnEdit.Image
            mnuEdit.Shortcut = Shortcut.F3 '// F3 = Editar

            oX = New XMsaButton
            oX.MnuItem = mnuEdit
            oX.Button = btnEdit
            oX.ButtonAction = XmsaButtons.Edit
            mBts.Add(XmsaButtons.Edit, oX)

            mnuEdit.tag = oX
            btnEdit.Tag = oX
            RemoveHandler mnuEdit.Click, AddressOf MnuItemClick
            RemoveHandler btnEdit.Click, AddressOf ButtonClick
            AddHandler mnuEdit.Click, AddressOf MnuItemClick
            AddHandler btnEdit.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuEdit)

            '// Borrar
            mnuDelete = New XMsaMenuItem
            mnuDelete.Text = Me.btnDel.Text
            mnuDelete.Imagen = btnDel.Image
            mnuDelete.Shortcut = Shortcut.CtrlDel '// CTRL + DEL = Borrar

            oX = New XMsaButton
            oX.MnuItem = mnuDelete
            oX.Button = btnDel
            oX.ButtonAction = XmsaButtons.Delete
            mBts.Add(XmsaButtons.Delete, oX)

            mnuDelete.tag = oX
            btnDel.Tag = oX
            RemoveHandler mnuDelete.Click, AddressOf MnuItemClick
            RemoveHandler btnDel.Click, AddressOf ButtonClick

            AddHandler mnuDelete.Click, AddressOf MnuItemClick
            AddHandler btnDel.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuDelete)

            '// Separador 1
            mnuSep1 = New XMsaMenuItem
            mnuSep1.Text = "-"
            oI.MenuItems.Add(mnuSep1)

            '// Aceptar
            mnuAccept = New XMsaMenuItem
            mnuAccept.Text = Me.btnOk.Text
            mnuAccept.Imagen = btnOk.Image
            mnuAccept.Shortcut = Shortcut.F5 '// F5 = Aceptar

            oX = New XMsaButton
            oX.MnuItem = mnuAccept
            oX.Button = btnOk
            oX.ButtonAction = XmsaButtons.Accept
            mBts.Add(XmsaButtons.Accept, oX)

            mnuAccept.tag = oX
            btnOk.Tag = oX
            RemoveHandler mnuAccept.Click, AddressOf MnuItemClick
            RemoveHandler btnOk.Click, AddressOf ButtonClick
            AddHandler mnuAccept.Click, AddressOf MnuItemClick
            AddHandler btnOk.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuAccept)

            '// Cancelar
            mnuCancel = New XMsaMenuItem
            mnuCancel.Text = Me.btnCancel.Text
            mnuCancel.Imagen = Me.btnCancel.Image
            mnuCancel.Shortcut = Shortcut.F6 '// F6 = Cancelar

            oX = New XMsaButton
            oX.MnuItem = mnuCancel
            oX.Button = btnCancel
            oX.ButtonAction = XmsaButtons.Cancel
            mBts.Add(XmsaButtons.Cancel, oX)

            mnuCancel.tag = oX
            btnCancel.Tag = oX
            RemoveHandler mnuCancel.Click, AddressOf MnuItemClick
            RemoveHandler btnCancel.Click, AddressOf ButtonClick
            AddHandler mnuCancel.Click, AddressOf MnuItemClick
            AddHandler btnCancel.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuCancel)

            '// Separador 2
            mnuSep2 = New XMsaMenuItem
            mnuSep2.Text = "-"
            oI.MenuItems.Add(mnuSep2)

            '// Buscar
            mnufind = New XMsaMenuItem
            mnufind.Text = btnFind.Text
            mnufind.Imagen = btnFind.Image
            mnufind.Shortcut = Shortcut.CtrlB  '// CTRL + B = Buscar

            oX = New XMsaButton
            oX.MnuItem = mnufind
            oX.Button = btnFind
            oX.ButtonAction = XmsaButtons.Find
            mBts.Add(XmsaButtons.Find, oX)
            mnufind.tag = oX
            btnFind.Tag = oX
            RemoveHandler mnufind.Click, AddressOf MnuItemClick
            RemoveHandler btnFind.Click, AddressOf ButtonClick
            AddHandler mnufind.Click, AddressOf MnuItemClick
            AddHandler btnFind.Click, AddressOf ButtonClick
            oI.MenuItems.Add(mnufind)

            '// Print
            mnuPrint = New XMsaMenuItem
            mnuPrint.Text = Me.btnPrint.Text
            mnuPrint.Imagen = btnPrint.Image
            mnuPrint.Shortcut = Shortcut.CtrlI '// CTRL + I

            oX = New XMsaButton
            oX.MnuItem = mnuPrint
            oX.Button = btnPrint
            oX.ButtonAction = XmsaButtons.Print
            mBts.Add(XmsaButtons.Print, oX)

            mnuPrint.tag = oX
            btnPrint.Tag = oX
            RemoveHandler mnuPrint.Click, AddressOf MnuItemClick
            RemoveHandler btnPrint.Click, AddressOf ButtonClick

            AddHandler mnuPrint.Click, AddressOf MnuItemClick
            AddHandler btnPrint.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuPrint)

            '// Separador 3
            mnuSep3 = New XMsaMenuItem
            mnuSep3.Text = "-"
            oI.MenuItems.Add(mnuSep3)

            '// Proximo
            mnuNext = New XMsaMenuItem
            mnuNext.Text = "Siguiente"
            mnuNext.Imagen = btnNext.Image

            oX = New XMsaButton
            oX.MnuItem = mnuNext
            oX.Button = btnNext
            oX.ButtonAction = XmsaButtons.Next
            mBts.Add(XmsaButtons.Next, oX)

            mnuNext.tag = oX
            btnNext.Tag = oX
            RemoveHandler mnuNext.Click, AddressOf MnuItemClick
            RemoveHandler btnNext.Click, AddressOf ButtonClick

            AddHandler mnuNext.Click, AddressOf MnuItemClick
            AddHandler btnNext.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuNext)

            '// Anterior
            mnuPrev = New XMsaMenuItem
            mnuPrev.Text = "Anterior"
            mnuPrev.Imagen = btnPrev.Image

            oX = New XMsaButton
            oX.MnuItem = mnuPrev
            oX.Button = btnPrev
            oX.ButtonAction = XmsaButtons.Prev
            mBts.Add(XmsaButtons.Prev, oX)

            mnuPrev.tag = oX
            btnPrev.Tag = oX
            RemoveHandler mnuPrev.Click, AddressOf MnuItemClick
            RemoveHandler btnPrev.Click, AddressOf ButtonClick

            AddHandler mnuPrev.Click, AddressOf MnuItemClick
            AddHandler btnPrev.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuPrev)

            '// Primero
            mnuFirst = New XMsaMenuItem
            mnuFirst.Text = "Primero"
            mnuFirst.Imagen = btnFirst.Image

            oX = New XMsaButton
            oX.MnuItem = mnuFirst
            oX.Button = btnFirst
            oX.ButtonAction = XmsaButtons.First
            mBts.Add(XmsaButtons.First, oX)

            mnuFirst.tag = oX
            btnFirst.Tag = oX
            RemoveHandler mnuFirst.Click, AddressOf MnuItemClick
            RemoveHandler btnFirst.Click, AddressOf ButtonClick

            AddHandler mnuFirst.Click, AddressOf MnuItemClick
            AddHandler btnFirst.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuFirst)


            '// Ultimo
            mnuLast = New XMsaMenuItem
            mnuLast.Text = "Ultimo"
            mnuLast.Imagen = btnLast.Image

            oX = New XMsaButton
            oX.MnuItem = mnuLast
            oX.Button = btnLast
            oX.ButtonAction = XmsaButtons.Last
            mBts.Add(XmsaButtons.Last, oX)

            mnuLast.tag = oX
            btnLast.Tag = oX
            RemoveHandler mnuLast.Click, AddressOf MnuItemClick
            RemoveHandler btnLast.Click, AddressOf ButtonClick

            AddHandler mnuLast.Click, AddressOf MnuItemClick
            AddHandler btnLast.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuLast)

            '// Separador 4
            mnuSep4 = New XMsaMenuItem
            mnuSep4.Text = "-"
            oI.MenuItems.Add(mnuSep4)

            ''''// Hojear
            '''mnuBrow = New XMsaMenuItem
            '''mnuBrow.Text = "Hojear"
            ''''mnuBrow.Imagen = btnNew.Image
            '''mnuBrow.Shortcut = Shortcut.F2 '//F2 - Agregar

            ''''''oX = New XMsaButton
            ''''''oX.MnuItem = mnuBrow
            ''''''oX.Button = btnNew
            ''''''oX.ButtonAction = XmsaButtons.Add
            ''''''mBts.Add(XmsaButtons.Add, oX)

            ''''''mnuAdd.tag = oX
            ''''''btnNew.Tag = oX
            '''RemoveHandler mnuBrow.Click, AddressOf MnuItemClick
            ''''// RemoveHandler btnNew.Click, AddressOf ButtonClick
            '''AddHandler mnuBrow.Click, AddressOf MnuItemClick
            ''''// AddHandler btnNew.Click, AddressOf ButtonClick

            '''oI.MenuItems.Add(mnuAdd)


            '// Salir
            mnuExit = New XMsaMenuItem
            mnuExit.Text = Me.btnExit.Text
            mnuExit.Imagen = btnExit.Image
            mnuExit.Shortcut = Shortcut.F12 '// F12 = Salir

            oX = New XMsaButton
            oX.MnuItem = mnuExit
            oX.Button = btnExit
            oX.ButtonAction = XmsaButtons.Exit
            mBts.Add(XmsaButtons.Exit, oX)

            mnuExit.tag = oX
            btnExit.Tag = oX
            RemoveHandler mnuExit.Click, AddressOf MnuItemClick
            RemoveHandler btnExit.Click, AddressOf ButtonClick
            AddHandler mnuExit.Click, AddressOf MnuItemClick
            AddHandler btnExit.Click, AddressOf ButtonClick

            oI.MenuItems.Add(mnuExit)


            RaiseEvent InitToolSettings(Me, New EventArgs)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Overridable Sub SetTextMode(ByVal sText As String)
    End Sub

    Public Overridable Sub SetTextRecordPosition(ByVal sText As String)
        Me.lblPos.Text = sText
    End Sub


    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

    End Sub

    'Private Sub mFrmOwn_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles mFrmOwn.Load
    '    Me.OnOwnerLoad()


    '    If mUseMenu Then
    '        Me.mFrmOwn.Menu = mM
    '    End If

    'End Sub



    ' <Category("Data"), _
    'RefreshProperties(RefreshProperties.Repaint), _
    'TypeConverter("System.Windows.Forms.Design.DataSourceConverter," & _
    '"System.Design")> _
    ' Public Property DataSource() As Object
    '     Get
    '         Return ds
    '     End Get
    '     Set(ByVal Value As Object)
    '         ds = Value
    '     End Set
    ' End Property

    ' <Category("Data"), _
    ' Editor("System.Windows.Forms.Design.DataMemberListEditor," & _
    ' "System.Design", GetType(System.Drawing.Design.UITypeEditor))> _
    ' Public Property DataMember() As String
    '     Get
    '         Return mdatamember
    '     End Get
    '     Set(ByVal Value As String)
    '         mdatamember = Value
    '     End Set
    ' End Property


    'Public ReadOnly Property ContainsListCollection() As Boolean Implements System.ComponentModel.IListSource.ContainsListCollection
    '    Get
    '        If TypeOf dt Is DataSet Then
    '            Return True
    '        Else
    '            Return False
    '        End If
    '    End Get
    'End Property

    'Public Overridable Function GetCurrentCyManager() As CurrencyManager
    '    If TypeOf dt Is DataTable Then
    '        Return Me.mFrmOwn.BindingContext(Me)
    '    Else
    '        Return mFrmOwn.BindingContext(Me, Me.mdatamember)
    '    End If
    'End Function

    'Public Function GetList() As System.Collections.IList Implements System.ComponentModel.IListSource.GetList
    '    If dt Is Nothing Then
    '        If ds Is Nothing Then
    '            ds = New DataSet
    '        End If
    '        dt = CType(ds, IListSource).GetList
    '    End If

    '    Return CType(dt, IList)
    'End Function

    Protected Overrides Sub OnMouseHover(ByVal e As System.EventArgs)
        MyBase.OnMouseHover(e)
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)
        Me.Invalidate()
    End Sub

    Public Property Buttons(ByVal index As XmsaButtons) As XMsaButton
        Get
            Return mBts(index)
        End Get
        Set(ByVal Value As XMsaButton)
            mBts(index) = Value
        End Set
    End Property

    Public Overridable Sub OnConnectorChanged()

    End Sub

    Public Property Connector() As XMsaConnector
        Get
            Return mConector
        End Get
        Set(ByVal Value As XMsaConnector)
            mConector = Value

            If Not Value Is Nothing Then
                If mConector.OwnerForm Is Nothing Then
                    mConector.OwnerForm = Me.FindForm
                    mFrmOwn = mConector.OwnerForm
                End If

            End If
            OnConnectorChanged()
        End Set
    End Property

    Public ReadOnly Property HasInitiated() As Boolean
        Get
            Return mHasInitiated
        End Get
    End Property

    Public Sub ExecuteInitSetttings()
        Try
            Me.mFrmOwn = Me.FindForm

            OnInitToolSettings()

            If mUseMenu And Not Me.mFrmOwn Is Nothing Then
                Me.mFrmOwn.Menu = mM
            End If

            Me.lblPos.Text = ""

            mHasInitiated = True
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub


    Private Sub XMsaToolBar_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Not mHasInitiated Then
                Me.ExecuteInitSetttings()
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    'Private Sub lblSep1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles lblSep1.Paint
    '    Dim brush As LinearGradientBrush
    '    Dim mode As LinearGradientMode
    '    Dim newRect As Rectangle

    '    Dim ol As Label = CType(sender, Label)


    '    e.Graphics.SmoothingMode = SmoothingMode.HighQuality

    '    newRect = ol.ClientRectangle ' pevent.ClipRectangle toda el area, no solo la tocada


    '    mode = LinearGradientMode.Vertical
    '    brush = New LinearGradientBrush(newRect, _NormalColorA, _NormalColorB, mode)

    '    Dim pW As New Pen(Color.White)
    '    Dim pG As New Pen(Color.DarkGray)


    '    'e.Graphics.FillRectangle(brush, newRect)

    '    e.Graphics.DrawLine(pW, 1, 1, 1, newRect.Height - 1)
    '    e.Graphics.DrawLine(pG, 2, 1, 2, newRect.Height - 1)
    '    'e.Graphics.DrawLine(pw, 0, newRect.Height - 1, newRect.Width + 1, newRect.Height - 1)

    '    pG.Dispose()
    '    pW.Dispose()

    '    brush.Dispose()
    '    brush = Nothing

    'End Sub

    Private Sub XMsaToolBar_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress

    End Sub
End Class

Public Class XMsaButton
    Public Button As Button
    Public MnuItem As XMsaMenuItem
    Public ButtonAction As XmsaButtons

    Public Property Enabled() As Boolean
        Get
            Return Button.Enabled
        End Get
        Set(ByVal Value As Boolean)
            Button.Enabled = Value
            MnuItem.Enabled = Value
        End Set
    End Property
End Class

Public Class XMsaActionClickEventArgs
    Public Action As XMsaButton
    Public ButtonAction As XmsaButtons
End Class

