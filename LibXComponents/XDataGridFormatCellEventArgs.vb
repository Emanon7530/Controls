


Public Delegate Sub XDataGridFormatCellEventHandler(ByVal sender As Object, ByVal e As XDataGridFormatCellEventArgs)

Public Class XDataGridFormatCellEventArgs

    Inherits EventArgs

    Public ImageIndex As Integer = -1
    Public Column As Integer
    Public RowNumber As Integer
    Public CurrentCellValue As Object
    Public TextFont As Font
    Public BackColor As Color
    Public ForeColor As Color
    'Public BackBrush As Brush
    'Public ForeBrush As Brush
    'Public BackBrushDispose As Boolean
    ' Public ForeBrushDispose As Boolean
    Public TextFontDispose As Boolean
    Public UseBaseClassDrawing As Boolean
    Public CryManager As CurrencyManager


    Public Sub New(ByVal row As Integer, ByVal col As Integer, ByVal cellValue As Object)
        RowNumber = row
        Column = col
        TextFont = Nothing
        BackColor = Color.White
        ForeColor = Color.Black
        '-->BackBrush = Nothing
        '-->ForeBrush = Nothing
        TextFontDispose = False
        '-->BackBrushDispose = False
        '-->ForeBrushDispose = False
        UseBaseClassDrawing = False
        CurrentCellValue = cellValue
    End Sub



End Class


