Option Strict Off
Option Explicit On 

Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms


Public Class XDataGridButtonColumn
    Inherits DataGridTextBoxColumn
    Public Event CellButtonClicked As XDataGridCellButtonClickEventHandler

    Private _buttonFace As Bitmap
    Private _buttonFacePressed As Bitmap
    Private _columnNum As Integer
    Private _pressedRow As Integer
    Private m_blnHandleCreated As Boolean


    Public Sub New()
        _columnNum = -1
        _pressedRow = -1

        Try
            Dim strm As System.IO.Stream = Me.GetType().Assembly.GetManifestResourceStream("fullbuttonface.bmp")
            _buttonFace = New Bitmap(strm)
            strm = Me.GetType().Assembly.GetManifestResourceStream("fullbuttonfacepressed.bmp")
            _buttonFacePressed = New Bitmap(strm)
        Catch
        End Try

    End Sub

    Public Sub New(ByVal colNum As Integer)
        _columnNum = colNum
        _pressedRow = -1

        Try
            Dim strm As System.IO.Stream = Me.GetType().Assembly.GetManifestResourceStream("fullbuttonface.bmp")
            _buttonFace = New Bitmap(strm)
            strm = Me.GetType().Assembly.GetManifestResourceStream("fullbuttonfacepressed.bmp")
            _buttonFacePressed = New Bitmap(strm)
        Catch
        End Try
    End Sub 'New

    Public ReadOnly Property Column() As Integer
        Get

            Return _columnNum

        End Get
    End Property

    Public Property CharacterCasing() As CharacterCasing
        Get
            Return Me.TextBox.CharacterCasing
        End Get
        Set(ByVal Value As CharacterCasing)
            Me.TextBox.CharacterCasing = Value
        End Set
    End Property

    Public Property MaxLength() As Integer
        Get
            Return Me.TextBox.MaxLength
        End Get
        Set(ByVal Value As Integer)
            Me.TextBox.MaxLength = Value
        End Set
    End Property

    Private Sub DrawButton(ByVal g As Graphics, ByVal bm As Bitmap, ByVal bounds As Rectangle, ByVal row As Integer)

        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid
        Dim s As String = dg(row, Me._columnNum).ToString()

        Dim sz As SizeF = g.MeasureString(s, dg.Font, bounds.Width - 4, StringFormat.GenericTypographic)

        Dim x As Integer = bounds.Left + Math.Max(0, (bounds.Width - sz.Width) / 2)
        If Not bm Is Nothing Then
            g.DrawImage(bm, bounds, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel)
        End If

        If (sz.Height < bounds.Height) Then

            Dim y As Integer = bounds.Top + (bounds.Height - sz.Height) / 2
            If (_buttonFacePressed Is bm) Then
                x = x + 1
            End If
            g.DrawString(s, dg.Font, New SolidBrush(dg.ForeColor), x, y)
        End If

    End Sub

    Protected Overloads Overrides Sub Edit(ByVal [source] As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal bounds As System.Drawing.Rectangle, ByVal [readOnly] As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)
    End Sub

    'Edit
    'dont call the baseclass so no editing done...
    'base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible); 

    Public Sub HandleMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid
        Dim hti As DataGrid.HitTestInfo = dg.HitTest(New Point(e.X, e.Y))
        Dim isClickInCell As Boolean = (hti.Column = Me._columnNum And hti.Row > -1)

        _pressedRow = -1

        Dim rect As New Rectangle(0, 0, 0, 0)

        If isClickInCell Then
            rect = dg.GetCellBounds(hti.Row, hti.Column)
            isClickInCell = e.X > rect.Right - Me._buttonFace.Width
        End If
        If isClickInCell Then
            Dim g As Graphics = Graphics.FromHwnd(dg.Handle)
            ' g.DrawImage(Me._buttonFace, rect.Right - Me._buttonFace.Width, rect.Y)
            DrawButton(g, Me._buttonFace, rect, hti.Row)
            g.Dispose()
            'If Not (CellButtonClicked Is Nothing) Then
            RaiseEvent CellButtonClicked(Me, New XDataGridCellButtonClickEventArgs(hti.Row, hti.Column))
            'End If
        End If
    End Sub 'HandleMouseUp

    Public Sub HandleMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim dg As DataGrid = Me.DataGridTableStyle.DataGrid
        Dim hti As DataGrid.HitTestInfo = dg.HitTest(New Point(e.X, e.Y))
        Dim isClickInCell As Boolean = (hti.Column = Me._columnNum And hti.Row > -1)
        Dim rect As New Rectangle(0, 0, 0, 0)
        If isClickInCell Then
            rect = dg.GetCellBounds(hti.Row, hti.Column)
            isClickInCell = e.X > rect.Right - Me._buttonFace.Width
        End If

        If isClickInCell Then
            'Console.WriteLine("HandleMouseDown " + hti.Row.ToString());
            Dim g As Graphics = Graphics.FromHwnd(dg.Handle)
            'g.DrawImage(Me._buttonFacePressed, rect.Right - Me._buttonFacePressed.Width, rect.Y)
            DrawButton(g, Me._buttonFacePressed, rect, hti.Row)
            g.Dispose()
            _pressedRow = hti.Row
        End If
    End Sub 'HandleMouseDown


    Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal [source] As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal backBrush As System.Drawing.Brush, ByVal foreBrush As System.Drawing.Brush, ByVal alignToRight As Boolean)
        'base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
        Dim parent As DataGrid = Me.DataGridTableStyle.DataGrid

        If Not m_blnHandleCreated Then
            m_blnHandleCreated = True
            AddHandler parent.MouseDown, AddressOf HandleMouseDown
            AddHandler parent.MouseUp, AddressOf HandleMouseUp
        End If

        If _columnNum < 0 Then
            _columnNum = DataGridTableStyle.GridColumnStyles.IndexOf(Me)
        End If
        Dim current As Boolean = parent.IsSelected(rowNum) Or (parent.CurrentRowIndex = rowNum And parent.CurrentCell.ColumnNumber = Me._columnNum)

        Dim BackColor As Color
        If current Then BackColor = parent.SelectionBackColor Else BackColor = parent.BackColor
        Dim ForeColor As Color
        If current Then ForeColor = parent.SelectionForeColor Else ForeColor = parent.ForeColor

        'clear the cell
        g.FillRectangle(New SolidBrush(BackColor), bounds)

        'draw the value
        Dim s As String = Me.GetColumnValueAtRow([source], rowNum).ToString() 'parent[rowNum, 0].ToString() + ((parent[rowNum, 1].ToString())+ "  ").Substring(0,2);
        'Font font = new Font("Arial", 8.25f);
        'g.DrawString(s, font, new SolidBrush(Color.Black), bounds);
        'g.DrawString(s, parent.Font, New SolidBrush(ForeColor), bounds.X, bounds.Y)

        'draw the button
        Dim bm As Bitmap
        If _pressedRow = rowNum Then bm = Me._buttonFacePressed Else bm = Me._buttonFace
        'g.DrawImage(bm, bounds.Right - bm.Width, bounds.Y)
        DrawButton(g, bm, bounds, rowNum)
    End Sub 'Paint 'font.Dispose();
End Class 'DataGridButtonColumn


