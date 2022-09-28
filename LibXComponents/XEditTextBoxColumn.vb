Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing


Public Class XEditTextBoxColumn
    Inherits DataGridTextBoxColumn

    Private m_blnIsReadOnly As Boolean

    Private m_ImageList As ImageList
    Private m_intColumn As Integer = -1
    Private m_TabStop As Boolean = True

    Public Event SetCellFormat As XDataGridFormatCellEventHandler
    Private m_blnUseCustomCellFormat As Boolean

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

    Public Property TabStop() As Boolean
        Get
            Return m_TabStop
        End Get
        Set(ByVal Value As Boolean)
            m_TabStop = Value
        End Set
    End Property

    Public Property UseCustomCellFormat() As Boolean
        Get
            Return m_blnUseCustomCellFormat
        End Get
        Set(ByVal Value As Boolean)
            m_blnUseCustomCellFormat = Value
        End Set
    End Property

    Public Sub New()
    End Sub

    '''Protected Overloads Overrides Sub Edit(ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal bounds As System.Drawing.Rectangle, ByVal p_readOnly As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)
    '''    If m_blnIsReadOnly Then
    '''        Exit Sub
    '''    End If
    '''    MyBase.Edit(source, rowNum, bounds, p_readOnly, instantText, cellIsVisible)
    '''End Sub

    Public ReadOnly Property Column() As Integer
        Get

            Return Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me)

        End Get
    End Property


    Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal backBrush As System.Drawing.Brush, ByVal foreBrush As System.Drawing.Brush, ByVal alignToRight As Boolean)
        Dim objFont As Font
        Try

            If Not m_blnUseCustomCellFormat Then
                MyBase.Paint(g, bounds, [source], rowNum, backBrush, foreBrush, alignToRight)
                Exit Sub
            End If

            Dim e As XDataGridFormatCellEventArgs = Nothing
            objFont = Me.TextBox.Font

            'fire the formatting event
            Dim col As Integer = Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me)
            e = New XDataGridFormatCellEventArgs(rowNum, col, Me.GetColumnValueAtRow([source], rowNum))
            RaiseEvent SetCellFormat(Me, e)

            If e.ImageIndex <> -1 And Not Me.m_ImageList Is Nothing Then
                g.FillRectangle(backBrush, bounds)
                Dim [oF] As RectangleF
                Dim iH As Integer
                iH = bounds.Width
                [oF] = Me.m_ImageList.Images(e.ImageIndex).GetBounds(g.PageUnit)
                bounds.Height = [oF].Height
                bounds.Width = [oF].Width
                'bounds.X = (iH - bounds.Width) / 2
                g.DrawImage(Me.m_ImageList.Images(e.ImageIndex), bounds)
                Return
            End If

            If e.UseBaseClassDrawing Then
                m_blnUseCustomCellFormat = False
            End If

            If m_blnUseCustomCellFormat Then
                backBrush = New SolidBrush(e.BackColor)
                foreBrush = New SolidBrush(e.ForeColor)

                If Not e.TextFont Is Nothing Then
                    objFont = e.TextFont
                End If

                '    'if TextFont set, then must call drawstring
                '    If m_blnUseCustomCellFormat Then
                '        g.FillRectangle(backBrush, bounds)
                '        'Try


                '        Dim charWidth As Integer = Fix(Math.Ceiling(g.MeasureString("c", objFont, 20, StringFormat.GenericTypographic).Width))

                '        Dim s As String = Me.GetColumnValueAtRow([source], rowNum).ToString()

                '        If Trim(Me.Format) <> "" And Trim(s) <> "" Then
                '            If IsDate(s) Then
                '                s = CDate(s).ToString(Me.Format)
                '            Else
                '                s = CDbl(s).ToString(Me.Format)
                '            End If
                '        End If

                '        Dim maxChars As Integer = Math.Min(s.Length, bounds.Width / charWidth)


                '        g.DrawString(s.Substring(0, maxChars), objFont, foreBrush, bounds.X, bounds.Y + 2)

                '    End If
                'End If


                'If Not m_blnUseCustomCellFormat Then
                '*--> De esta forma hace lo mismo que el codigo anterior, y no tengo problemas
                '*    con el format el alignment default, lo unico es que no puedo cambiar el textfont. Aunque de
                '*    la parte de arriba solo tube problemas con el alignment
                MyBase.Paint(g, bounds, [source], rowNum, backBrush, foreBrush, alignToRight)
                'End If
            End If

            'clean up
            If Not (e Is Nothing) Then
                'If e.BackBrushDispose Then
                '    e.BackBrush.Dispose()
                'End If
                'If e.ForeBrushDispose Then
                '    e.ForeBrush.Dispose()
                'End If
                If e.TextFontDispose Then
                    e.TextFont.Dispose()
                End If
            End If

        Catch ex As System.Exception
            Log.Show(ex)
        Finally
            If Not objFont Is Nothing Then
                'objFont.Dispose()
            End If
            'If Not backBrush Is Nothing Then
            '    backBrush.Dispose()
            'End If
            'If Not foreBrush Is Nothing Then
            '    foreBrush.Dispose()
            'End If

        End Try

    End Sub

    Public Property ImageList() As ImageList
        Get
            Return Me.m_ImageList
        End Get
        Set(ByVal Value As ImageList)
            Me.m_ImageList = Value
        End Set
    End Property

    Public Function getColumnNumber() As Integer
        If m_intColumn < 0 Then
            m_intColumn = DataGridTableStyle.GridColumnStyles.IndexOf(Me)
        End If
        Return m_intColumn
    End Function


    Public Property isReadOnly() As Boolean
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
        End Set
    End Property

    Public Function GetValue() As String
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid
            Dim oValue As Object = Grid(Grid.CurrentRowIndex, Me.getColumnNumber)

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub SetValue(ByVal value As Object)
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid

            SetValue(Grid.CurrentRowIndex, value)

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub

    Public Function GetValue(ByVal row As Integer) As String
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid
            Dim oValue As Object = Grid(Grid.CurrentRowIndex, Me.getColumnNumber)

            Return oValue.ToString.Trim

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Function

    Public Sub SetValue(ByVal row As Integer, ByVal value As Object)
        Try
            Dim Grid As DataGrid = Me.DataGridTableStyle.DataGrid

            If Not value Is DBNull.Value Then
                Grid(row, Me.getColumnNumber) = value
            Else
                Grid(row, Me.getColumnNumber) = DBNull.Value
            End If

            Dim cm As CurrencyManager
            cm = CType(Grid, LibXGrid).GetCM
            If cm Is Nothing OrElse cm.Position < 0 Then
                Exit Sub
            End If

            Dim cellRect As Rectangle = Grid.GetCellBounds(row, Me.getColumnNumber)
            Dim isVis As Boolean = True

            If Me.Width = 0 Then
                isVis = False
            End If

            If Not value Is DBNull.Value Then
                MyBase.Edit(cm, row, cellRect, Me.ReadOnly, value, isVis)
            Else
                MyBase.Edit(cm, row, cellRect, Me.ReadOnly, String.Empty, isVis)
            End If

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub
End Class
