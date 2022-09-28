Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel

Public Class XMsaButtonBase
    Inherits Button

    Private _NormalBorderColor As Color = Color.WhiteSmoke
    Private _NormalColorA As Color = Color.WhiteSmoke
    Private _NormalColorB As Color = Color.Gray
    Dim ButtonShadowOffset As Integer = 5
    Public Const LeftMargin As Integer = 2
    Public Const TextMargin As Integer = 2


    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Try
            Dim brush As LinearGradientBrush
            Dim mode As LinearGradientMode
            Dim newRect As Rectangle
            Dim iPoint As Point
            Dim tPoint As Point
            Dim tf As StringFormat

            Dim ob As Button = Me

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

            If Not ob.Image Is Nothing Then
                If ob.Enabled Then
                    'e.Graphics.DrawImage(CType(sender, Button).Image, newRect)
                    e.Graphics.DrawImage(ob.Image, iPoint)
                Else
                    ControlPaint.DrawImageDisabled(e.Graphics, ob.Image, iPoint.X, iPoint.Y, Color.Transparent)
                End If
            End If


            DrawText(e.Graphics, ob, tPoint, tf)

            pW.Dispose()
            pG.Dispose()
            brush.Dispose()
            brush = Nothing

        Catch
            MyBase.OnPaint(e)
        End Try

    End Sub


    Private Sub GetPoints(ByVal but As Button, ByRef iPoint As Point, ByRef tPoint As Point, ByRef tf As StringFormat, ByVal g As Graphics)
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

    End Sub

    Public Function GetTextSize(ByVal but As Button, ByVal g As Graphics, ByVal text As String, ByVal font As Font, ByVal sz As Size) As Size
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
    End Function


    Private Sub DrawText(ByVal g As Graphics, ByVal button As Button, ByVal tPoint As Point, ByVal tf As StringFormat)

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

    End Sub


End Class


Public Class XMsaButtonBasePrint
    Inherits Button

    Private _NormalBorderColor As Color = Color.WhiteSmoke
    Private _NormalColorA As Color = Color.WhiteSmoke
    Private _NormalColorB As Color = Color.Gray
    Dim ButtonShadowOffset As Integer = 5
    Public Const LeftMargin As Integer = 2
    Public Const TextMargin As Integer = 2


    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Try
            Dim brush As LinearGradientBrush
            Dim mode As LinearGradientMode
            Dim newRect As Rectangle
            Dim iPoint As Point
            Dim tPoint As Point
            Dim tf As StringFormat

            Dim ob As Button = Me

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

            ''e.Graphics.DrawLine(pW, 1, 1, 1, newRect.Height + 1)
            ''e.Graphics.DrawLine(pW, newRect.Width - 1, 1, newRect.Width - 1, newRect.Height + 1)
            'If sender Is btnPrint Then
                e.Graphics.DrawLine(pG, newRect.Width - 1, 0, newRect.Width - 1, newRect.Height + 1)
            'End If


            If Not ob.Image Is Nothing Then
                If ob.Enabled Then
                    'e.Graphics.DrawImage(CType(sender, Button).Image, newRect)
                    e.Graphics.DrawImage(ob.Image, iPoint)
                Else
                    ControlPaint.DrawImageDisabled(e.Graphics, ob.Image, iPoint.X, iPoint.Y, Color.Transparent)
                End If
            End If


            DrawText(e.Graphics, ob, tPoint, tf)

            pW.Dispose()
            pG.Dispose()
            brush.Dispose()
            brush = Nothing

        Catch
            MyBase.OnPaint(e)
        End Try

    End Sub


    Private Sub GetPoints(ByVal but As Button, ByRef iPoint As Point, ByRef tPoint As Point, ByRef tf As StringFormat, ByVal g As Graphics)
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

    End Sub

    Public Function GetTextSize(ByVal but As Button, ByVal g As Graphics, ByVal text As String, ByVal font As Font, ByVal sz As Size) As Size
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
    End Function


    Private Sub DrawText(ByVal g As Graphics, ByVal button As Button, ByVal tPoint As Point, ByVal tf As StringFormat)

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

    End Sub


End Class
