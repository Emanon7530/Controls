Imports System.ComponentModel

    Public Class XMsaMenuItem
        Inherits MenuItem

#Region " Código generado por el Diseñador de componentes "

        Private pImagen As Image
        Private pFuente As Font
        Private pWidth As Integer
        Private pHeight As Integer

        Public Sub New(ByVal text As String, ByVal image As Image, ByVal onClick As EventHandler, ByVal shortcut As Shortcut)
            MyBase.New(text, onClick, shortcut)
            OwnerDraw = True
            pImagen = image
            pFuente = SystemInformation.MenuFont
        End Sub

        Public Sub New()
            MyClass.New("", Nothing, Nothing, System.Windows.Forms.Shortcut.None)
        End Sub

        'Component reemplaza a Dispose para limpiar la lista de componentes.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Requerido por el Diseñador de componentes
        Private components As System.ComponentModel.IContainer

        'NOTA: el Diseñador de componentes requiere el siguiente procedimiento
        'Se puede modificar utilizando el Diseñador de componentes.
        'No lo modifique con el editor de código.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

#End Region


    Public tag As Object

    'Agrega dos propiedades nuevas al MenuItem, Imagen, que es la imagen o icono a mostrar, y fuente, para poder cambiarla
    <Category("Appearance"), DefaultValue(GetType(Image), "Ninguna"), Description("Imagen a desplegar en el menu")> _
   Public Property Imagen() As Image
        Get
            Imagen = pImagen
        End Get
        Set(ByVal new_Value As Image)
            pImagen = new_Value
        End Set
    End Property

    <Category("Appearance"), Description("Fuente a desplegar en el Menú.")> _
    Public Property Fuente() As Font
        Get
            Fuente = pFuente

        End Get
        Set(ByVal new_Value As Font)
            pFuente = new_Value
        End Set
    End Property
    Private ReadOnly Property esMenuPrincipal() As Boolean
        Get
            esMenuPrincipal = (TypeOf (Me.Parent) Is MainMenu)
        End Get
    End Property
    Public ReadOnly Property Width() As Integer
        Get
            Return pWidth
        End Get
    End Property
    Public ReadOnly Property Height() As Integer
        Get
            Return pHeight
        End Get
    End Property

    ' Sobreescribe la los eventos MeasureItem y DrawItem del objeto MenuItem
    Protected Overrides Sub OnMeasureItem(ByVal e As System.Windows.Forms.MeasureItemEventArgs)

        MyBase.OnMeasureItem(e)

        Dim sTexto As String
        Dim SF As New StringFormat
        SF.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide
        sTexto = CalculaTexto().Replace(Chr(5), "  ")

        Select Case sTexto
            Case "-"
                e.ItemHeight = 3
            Case Else
                Dim tamano As SizeF
                e.ItemHeight = CInt(e.Graphics.MeasureString(sTexto, pFuente, tamano, SF).Height)
                If e.ItemHeight < 22 Then e.ItemHeight = 22
                e.ItemWidth = CInt(e.Graphics.MeasureString(sTexto, pFuente, tamano, SF).Width) + 2
                If Not esMenuPrincipal Then
                    e.ItemWidth += 45
                End If
        End Select
        SF.Dispose()
        pHeight = e.ItemHeight
        pWidth = e.ItemWidth
    End Sub

    Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)

        MyBase.OnDrawItem(e)


        Dim colorx As Color = Color.FromArgb(100, SystemColors.GrayText.R, SystemColors.GrayText.G, SystemColors.GrayText.B)
        Dim uPen As Pen = New Pen(colorx)


        LimpiarFondo(e)
        If esMenuPrincipal Then
            DibujarResaltado(e)
            DibujarTexto(e)
        ElseIf Me.Text = "-" Then
            Barra(e)
            e.Graphics.DrawLine(uPen, e.Bounds.X + 32, e.Bounds.Y + 1, e.Bounds.Right, e.Bounds.Y + 1)
        Else
            Barra(e)
            DibujarResaltado(e)
            DibujarTexto(e)
            If Not (pImagen Is Nothing) Then
                Dim bmp As New Bitmap(pImagen, 16, 16)
                Dim TransColor As Color = bmp.GetPixel(0, bmp.Height - 1)
                bmp.MakeTransparent(TransColor)

                If CBool(e.State And DrawItemState.Disabled) Then
                    ControlPaint.DrawImageDisabled(e.Graphics, bmp, e.Bounds.X + 4, e.Bounds.Y + (e.Bounds.Height \ 2) - 8, SystemColors.GrayText)
                Else
                    If CBool(e.State And DrawItemState.Selected) Then
                        Dim bmpShadow As New Bitmap(bmp)
                        Dim x As Integer
                        Dim y As Integer
                        For x = 0 To 15
                            For y = 0 To 15
                                If bmpShadow.GetPixel(x, y).A <> 0 Then
                                    bmpShadow.SetPixel(x, y, SystemColors.ControlDark)
                                End If
                            Next
                        Next
                        e.Graphics.DrawImage(bmpShadow, New Rectangle(e.Bounds.X + 5, e.Bounds.Y + (e.Bounds.Height \ 2) - 7, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel)
                        e.Graphics.DrawImage(bmp, New Rectangle(e.Bounds.X + 3, e.Bounds.Y + (e.Bounds.Height \ 2) - 9, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel)
                        bmpShadow.Dispose()
                    Else
                        e.Graphics.DrawImage(bmp, New Rectangle(e.Bounds.X + 4, e.Bounds.Y + (e.Bounds.Height \ 2) - 8, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel)
                    End If
                End If
                bmp.Dispose()
            End If

        End If

        If Not uPen Is Nothing Then
            uPen.Dispose()
        End If


    End Sub

    'Procedimientos y funciones requeridas para el dibujado del menu
    Private Sub LimpiarFondo(ByVal e As System.Windows.Forms.DrawItemEventArgs)

        Dim Brocha As Brush = SystemBrushes.ControlLightLight
        Dim Superficie As New Rectangle(e.Bounds.X + 24, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height)

        If esMenuPrincipal Then
            Brocha = SystemBrushes.Control
            Superficie.X -= 24
            Superficie.Width += 24
        End If

        e.Graphics.FillRectangle(Brocha, Superficie)

    End Sub

    Private Sub Barra(ByVal e As System.Windows.Forms.DrawItemEventArgs)
        Dim Brocha As Brush = SystemBrushes.ControlLight
        e.Graphics.FillRectangle(Brocha, New Rectangle(e.Bounds.X, e.Bounds.Y, 24, e.Bounds.Height))
    End Sub

    Private Sub DibujarResaltado(ByVal e As System.Windows.Forms.DrawItemEventArgs)

        Dim Brocha As SolidBrush = New SolidBrush(Color.FromArgb(182, 189, 210))
        Dim Pluma As Pen = New Pen(Color.FromArgb(10, 36, 106), 1)

        If Not CBool(e.State And DrawItemState.Disabled) Then
            If CBool(e.State And DrawItemState.Selected) Or CBool(e.State And DrawItemState.HotLight) Then
                e.Graphics.FillRectangle(Brocha, New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))
                e.Graphics.DrawRectangle(Pluma, New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1))
            End If
        End If
        Brocha.Dispose()
        Pluma.Dispose()
    End Sub
    Private Sub DibujarTexto(ByVal e As System.Windows.Forms.DrawItemEventArgs)

        Dim EspacioTexto As New RectangleF(e.Bounds.X + 30, e.Bounds.Top, e.Bounds.Width - 45, e.Bounds.Height)
        Dim Brocha As Brush = New SolidBrush(Color.Black) ' Brushes.Black '  = New SolidBrush(Color.Black)
        Dim SF As New System.Drawing.StringFormat(StringFormatFlags.NoWrap)

        SF.LineAlignment = StringAlignment.Center
        SF.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show

        If esMenuPrincipal Then
            EspacioTexto = New RectangleF(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height)
        End If

        If CBool(e.State And DrawItemState.Selected) Or CBool(e.State And DrawItemState.HotLight) Then
            Brocha = New SolidBrush(Color.Black)    'Brushes.Black
        End If

        If CBool(e.State And DrawItemState.Disabled) Then
            Brocha = New SolidBrush(SystemColors.GrayText)  ' SystemBrushes.FromSystemColor(SystemColors.GrayText)
        End If

        Dim Partes() As String = CalculaTexto.Split(Chr(5))
        e.Graphics.DrawString(Partes(0), pFuente, Brocha, EspacioTexto, SF)
        If Partes.Length = 2 Then
            SF.Alignment = StringAlignment.Far
            e.Graphics.DrawString(Partes(1), pFuente, Brocha, EspacioTexto, SF)
        End If
        SF.Dispose()
        Brocha.Dispose()
        Erase Partes

    End Sub

    Private Function CalculaTexto() As String
        If Me.ShowShortcut And Me.Shortcut <> Shortcut.None Then
            Return Me.Text & Chr(5) & CType(Me.Shortcut, Keys).ToString
        Else
            Return Me.Text
        End If
    End Function
End Class
