Option Strict On
Option Explicit On 
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data

<ToolboxItem(False)> _
   Public Class XDataGridComboBox
    Inherits LibXCombo


    Public Sub New()
        MyBase.New()
    End Sub
    Public isInEditOrNavigateMode As Boolean = True

    Protected Overrides Sub RefreshItem(ByVal index As Integer)

    End Sub

    Protected Overrides Sub SetItemsCore(ByVal items As System.Collections.IList)

    End Sub

    Private Sub XDataGridComboBox_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.EnabledChanged
    End Sub
End Class

Public Class XDataGridComboBoxColumn
    Inherits DataGridTextBoxColumn

    Private _ColumnComboBox As LibXCombo
    Private _Source As System.Windows.Forms.CurrencyManager
    Private _RowNum As Integer
    Private _IsEditing As Boolean
    Private _FirstTimeFlag As Boolean
    Private xMargin As Integer = 2
    Private yMargin As Integer = 1


    Private m_blnIsReadOnly As Boolean
    '

    Sub New()

        _Source = Nothing
        _IsEditing = False

        _ColumnComboBox = New LibXCombo
        With _ColumnComboBox
            .DropDownStyle = ComboBoxStyle.DropDownList
            .DataSource = Nothing
            .DisplayMember = ""
            .ValueMember = Nothing
        End With

    End Sub


    ' This constructor accepts DisplayMember and 
    ' ValueMember as strings
    '
    Public Sub New(ByVal DataSource As DataTable, _
                   ByVal DisplayMember As String, _
                   ByVal ValueMember As String)

        _Source = Nothing
        _IsEditing = False

        _ColumnComboBox = New LibXCombo
        With _ColumnComboBox
            .DropDownStyle = ComboBoxStyle.DropDownList
            .DataSource = DataSource.DefaultView
            .DisplayMember = DisplayMember
            .ValueMember = ValueMember
        End With
        '
        ' Wire handlers
        '
        AddHandler _ColumnComboBox.Leave, AddressOf LeaveComboBox
        AddHandler _ColumnComboBox.Enter, AddressOf EnterComboBox
        AddHandler _ColumnComboBox.SelectionChangeCommitted, AddressOf ComboStartEditing

    End Sub
    '
    ' This constructor accepts DisplayMember and 
    ' ValueMember as Integers
    '
    Public Sub New(ByVal DataSource As DataTable, _
                   ByVal DisplayMember As Integer, _
                   ByVal ValueMember As Integer)

        _Source = Nothing
        _IsEditing = False

        _ColumnComboBox = New LibXCombo
        With _ColumnComboBox
            .DropDownStyle = ComboBoxStyle.DropDownList
            .DataSource = DataSource.DefaultView
            .DisplayMember = DataSource.Columns(DisplayMember).ToString
            .ValueMember = DataSource.Columns(ValueMember).ToString
        End With
        '
        ' Wire handlers
        '
        AddHandler _ColumnComboBox.Leave, AddressOf LeaveComboBox
        AddHandler _ColumnComboBox.Enter, AddressOf EnterComboBox
        AddHandler _ColumnComboBox.SelectionChangeCommitted, AddressOf ComboStartEditing

    End Sub

    Public Property isReadOnly() As Boolean
        Get
            Return m_blnIsReadOnly
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsReadOnly = Value
        End Set
    End Property

    Private Sub HandleScroll(ByVal sender As Object, ByVal e As EventArgs)
        If _ColumnComboBox.Visible Then
            _ColumnComboBox.Hide()
        End If
    End Sub

    Private Sub ComboStartEditing(ByVal sender As Object, ByVal e As EventArgs)
        _IsEditing = True
        _FirstTimeFlag = False
        MyBase.ColumnStartedEditing(DirectCast(sender, System.Windows.Forms.Control))
    End Sub

    Private Sub EnterComboBox(ByVal sender As Object, ByVal e As EventArgs)

        _FirstTimeFlag = True
    End Sub

    Private Sub LeaveComboBox(ByVal sender As Object, ByVal e As EventArgs)
        If _IsEditing Then
            SetColumnValueAtRow(_Source, _RowNum, _ColumnComboBox.Text)
            _IsEditing = False
            Invalidate()
        End If
        _ColumnComboBox.Hide()
        AddHandler Me.DataGridTableStyle.DataGrid.Scroll, New EventHandler(AddressOf HandleScroll)
    End Sub

    Protected Overloads Overrides Sub Edit(ByVal [Source] As System.Windows.Forms.CurrencyManager, _
                                           ByVal RowNum As Integer, _
                                           ByVal Bounds As System.Drawing.Rectangle, _
                                           ByVal [ReadOnly] As Boolean, _
                                           ByVal InstantText As String, _
                                           ByVal CellIsVisible As Boolean)

        MyBase.Edit(Source:=[Source], _
                    RowNum:=RowNum, _
                    Bounds:=Bounds, _
                    ReadOnly:=[ReadOnly], _
                    InstantText:=InstantText, _
                    CellIsVisible:=CellIsVisible)

        _RowNum = RowNum
        _Source = [Source]

        With _ColumnComboBox
            .Parent = Me.TextBox.Parent
            .BackColor = Me.TextBox.BackColor

            '.Location = Me.TextBox.Location
            .Size = New Size(Me.TextBox.Size.Width, _ColumnComboBox.Size.Height)

            Bounds.Offset(xMargin, yMargin)
            Bounds.Width -= xMargin * 2

            .Height -= yMargin
            .Bounds = Bounds


            If .Items.Count > 0 Then
                '
                ' If ComboBox has items and text is 
                ' empty set to first item in the list
                '
                If Me.TextBox.Text = String.Empty Then
                    .SelectedIndex = 0
                    If _FirstTimeFlag Then
                        ComboStartEditing(_ColumnComboBox, New EventArgs)
                    End If
                    '
                    ' Else set ComboBox text to TextBox text
                    '
                Else
                    .SelectedIndex = _ColumnComboBox.FindStringExact(Me.TextBox.Text)
                End If
            End If

            'If Me._IsEditing Then
            Me.TextBox.Visible = False
            .Visible = True
            .BringToFront()
            .Focus()
            'Else
            'Me.TextBox.Visible = True
            'End If

            'Else
            '.Visible = False
            'End If
            '
            ' Handle the Scroll event
            '
            AddHandler Me.DataGridTableStyle.DataGrid.Scroll, AddressOf HandleScroll

        End With
    End Sub

    Protected Overrides Function Commit(ByVal dataSource As System.Windows.Forms.CurrencyManager, _
                                        ByVal rowNum As Integer) As Boolean

        Me._ColumnComboBox.Hide()
        If _IsEditing Then
            _IsEditing = False
            SetColumnValueAtRow(dataSource, rowNum, _ColumnComboBox.Text)
        End If
        Return True
    End Function

    Protected Overrides Sub ConcedeFocus()
        Console.WriteLine("ConcedeFocus")
        MyBase.ConcedeFocus()
    End Sub

    Public Property ComboBox() As LibXCombo
        Get
            Return _ColumnComboBox

        End Get
        Set(ByVal value As LibXCombo)
            _ColumnComboBox = value

            _ColumnComboBox.Hide()

            _Source = Nothing
            _IsEditing = False

            AddHandler _ColumnComboBox.Leave, AddressOf LeaveComboBox
            AddHandler _ColumnComboBox.Enter, AddressOf EnterComboBox
            AddHandler _ColumnComboBox.SelectionChangeCommitted, AddressOf ComboStartEditing

        End Set
    End Property

    Protected Overrides Function GetColumnValueAtRow(ByVal [Source] As System.Windows.Forms.CurrencyManager, _
                                                     ByVal RowNum As Integer) As Object

        Dim ColValue As Object = MyBase.GetColumnValueAtRow([Source], RowNum)
        Dim View As DataView
        If TypeOf Me._ColumnComboBox.DataSource Is DataView Then
            View = DirectCast(Me._ColumnComboBox.DataSource, DataView)
        Else
            View = DirectCast(CType(Me._ColumnComboBox.DataSource, DataTable).DefaultView, DataView)
        End If
        Dim RowCount As Integer = View.Count
        Dim IntInx As Integer = 0
        Dim ValueMem As Object
        '
        ' Sort the DataView and use a binary search if this 
        ' method proves too slow
        '
        While IntInx < RowCount
            ValueMem = View(IntInx)(Me._ColumnComboBox.ValueMember)
            If (Not ValueMem Is DBNull.Value) AndAlso _
               (Not ColValue Is DBNull.Value) AndAlso _
               CType(ColValue, String).Trim.ToUpper.Equals(CType(ValueMem, String).Trim.ToUpper) Then
                Exit While
            End If
            IntInx += 1
        End While
        'If View.Table.PrimaryKey.Length = 0 Then

        'Else
        '    Dim r As DataRow
        '    r = View.Table.Rows.Find(New Object() {ColValue})
        '    If Not r Is Nothing Then
        '        Return r(Me._ColumnComboBox.DisplayMember)
        '    End If
        'End If

        If IntInx < RowCount Then
            Return View(IntInx)(Me._ColumnComboBox.DisplayMember)
        End If
        Return DBNull.Value
    End Function

    Protected Overrides Sub SetColumnValueAtRow(ByVal [Source] As System.Windows.Forms.CurrencyManager, _
                                                ByVal RowNum As Integer, _
                                                ByVal Value As Object)
        Dim ValueToSet As Object = Value
        Dim View As DataView
        If TypeOf Me._ColumnComboBox.DataSource Is DataView Then
            View = DirectCast(Me._ColumnComboBox.DataSource, DataView)
        Else
            View = DirectCast(CType(Me._ColumnComboBox.DataSource, DataTable).DefaultView, DataView)
        End If
        Dim RowCount As Integer = View.Count
        Dim IntInx As Integer = 0
        Dim DisplayMem As Object
        '
        ' Sort the DataView and use a binary search if this 
        ' method proves too slow
        '

        While IntInx < RowCount
            DisplayMem = View(IntInx)(Me._ColumnComboBox.DisplayMember)
            If (Not DisplayMem Is DBNull.Value) AndAlso _
               ValueToSet.Equals(DisplayMem) Then
                Exit While
            End If
            IntInx += 1
        End While
        If IntInx < RowCount Then
            ValueToSet = View(IntInx)(Me._ColumnComboBox.ValueMember)
        Else
            ValueToSet = DBNull.Value
        End If
        MyBase.SetColumnValueAtRow([Source], RowNum, ValueToSet)
    End Sub

End Class

'Public Class DataGridCombo
'    Inherits libxcombo
'    Private WM_KEYUP As Integer = &H101

'    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
'        If m.Msg = WM_KEYUP Then
'            '
'            ' Ignore WM_KEYUP to prevent tabbing through the ComboBox
'            '
'            Return
'        End If
'        MyBase.WndProc(m)
'    End Sub
'End Class


'Inherits DataGridColumnStyle

' UI constants    

'Private xMargin As Integer = 2
'Private yMargin As Integer = 1
'Private Combo As XDataGridComboBox
'Private combo As LibXCombo
'Private _DisplayMember As String
'Private _ValueMember As String

' Used to track editing state

'Private OldVal As String = String.Empty
'Private InEdit As Boolean = False

'Private m_objlibxcomboProps As CAM.Controls.Win.BaseClasses.libxcomboProps
'Private m_comboCreated As Boolean

'Public Event setComboBox(ByVal sender As Object, ByVal e As SetComboBoxEventArgs)


'Sub New()
'    combo = New XDataGridComboBox
'    combo = New LibXCombo

'    With combo
'        .Visible = False
'        .DataSource = Nothing
'        .DisplayMember = ""
'        .ValueMember = Nothing
'    End With

'End Sub

' Create a new column - DisplayMember, ValueMember
' Passed by ordinal 

'Public Sub New(ByRef DataSource As DataTable, _
'               ByVal DisplayMember As Integer, _
'               ByVal ValueMember As Integer)

'    combo = New XDataGridComboBox
'    combo = New LibXCombo
'    _DisplayMember = DataSource.Columns.Item(index:=DisplayMember).ToString
'    _ValueMember = DataSource.Columns.Item(index:=ValueMember).ToString

'    With combo
'        .Visible = False
'        .DataSource = DataSource
'        .DisplayMember = _DisplayMember
'        .ValueMember = _ValueMember
'    End With
'End Sub

' Create a new column - DisplayMember, ValueMember 
' passed by string

'Public Sub New(ByRef DataSource As DataTable, _
'               ByVal DisplayMember As String, _
'               ByVal ValueMember As String)

'    combo = New XDataGridComboBox
'    combo = New LibXCombo
'    With combo
'        .Visible = False
'        .DataSource = DataSource
'        .DisplayMember = DisplayMember
'        .ValueMember = ValueMember
'    End With

'End Sub

'Private Sub getCombo()
'    Dim objArgs As New SetComboBoxEventArgs
'    m_comboCreated = True
'    RaiseEvent setComboBox(Me, objArgs)
'    If Not objArgs.Combo Is Nothing Then
'        combo = objArgs.Combo
'        combo.Visible = False
'    End If

'End Sub

'Public Property ComboBox() As LibXCombo
'    Get
'        Return combo
'    End Get
'    Set(ByVal Value As LibXCombo)
'        combo = Value
'        combo.Visible = False
'    End Set
'End Property

'<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
'Public Property ComboBoxProperties() As libxcomboProps
'    Get
'        If m_objlibxcomboProps Is Nothing Then
'            m_objlibxcomboProps = New CAM.Controls.Win.BaseClasses.libxcomboProps
'        End If

'        Return m_objlibxcomboProps
'    End Get
'    Set(ByVal Value As libxcomboProps)
'        m_objlibxcomboProps = Value
'    End Set
'End Property

'------------------------------------------------------
' Methods overridden from DataGridColumnStyle
'------------------------------------------------------

' Abort Changes

'Protected Overloads Overrides Sub Abort(ByVal RowNum As Integer)
'    Debug.WriteLine("Abort()")
'    RollBack()
'    HideComboBox()
'    EndEdit()
'End Sub

' Commit Changes

'Protected Overloads Overrides Function Commit(ByVal DataSource As CurrencyManager, _
'                                              ByVal RowNum As Integer) As Boolean
'    HideComboBox()
'    If Not InEdit Then
'        Return True
'    End If

'    Try
'        Dim Value As Object = combo.currValue
'        If NullText.Equals(Value) Then
'            Value = Convert.DBNull
'        End If
'        If Value Is Nothing Then
'            Value = Convert.DBNull
'        End If
'        SetColumnValueAtRow(DataSource, RowNum, Value)
'    Catch e As Exception
'        RollBack()
'        Return False
'    End Try
'    EndEdit()
'    Return True
'End Function

' Remove focus

'Protected Overloads Overrides Sub ConcedeFocus()
'    combo.Visible = False
'End Sub

' Edit Grid

'Protected Overloads Overrides Sub Edit(ByVal Source As CurrencyManager, _
'                                       ByVal Rownum As Integer, _
'                                       ByVal Bounds As Rectangle, _
'                                       ByVal [ReadOnly] As Boolean, _
'                                       ByVal InstantText As String, _
'                                       ByVal CellIsVisible As Boolean)

'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    combo.Text = String.Empty

'    Dim OriginalBounds As Rectangle = Bounds

'    OldVal = combo.Text

'    If CellIsVisible Then
'        Bounds.Offset(xMargin, yMargin)
'        Bounds.Width -= xMargin * 2
'        Bounds.Height -= yMargin
'        combo.Bounds = Bounds
'        combo.Visible = True
'    Else
'        combo.Bounds = OriginalBounds
'        combo.Visible = False
'    End If

'    combo.currValue = GetText(GetColumnValueAtRow(Source, Rownum))

'    If Not InstantText Is Nothing Then
'        combo.currValue = InstantText
'    End If

'    combo.RightToLeft = Me.DataGridTableStyle.DataGrid.RightToLeft
'    combo.Focus()

'    If InstantText Is Nothing Then
'        combo.SelectAll()
'    Else
'        Dim [End] As Integer = combo.Text.Length
'        combo.Select([End], 0)
'    End If

'    If combo.Visible Then
'        DataGridTableStyle.DataGrid.Invalidate(OriginalBounds)
'    End If

'    InEdit = True

'End Sub

'Protected Overloads Overrides Function GetMinimumHeight() As Integer
'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If


'     Set the minimum height to the height of the combobox

'    Return combo.PreferredHeight + yMargin
'End Function

'Protected Overloads Overrides Function GetPreferredHeight(ByVal g As Graphics, _
'                                                          ByVal Value As Object) As Integer
'    Debug.WriteLine("GetPreferredHeight()")
'    Dim NewLineIndex As Integer = 0
'    Dim NewLines As Integer = 0
'    Dim ValueString As String = Me.GetText(Value)
'    Do
'        While NewLineIndex <> -1
'            NewLineIndex = ValueString.IndexOf("r\n", NewLineIndex + 1)
'            NewLines += 1
'        End While
'    Loop

'    Return FontHeight * NewLines + yMargin
'End Function

'Protected Overloads Overrides Function GetPreferredSize(ByVal g As Graphics, _
'                                                        ByVal Value As Object) As Size
'    Dim Extents As Size = Size.Ceiling(g.MeasureString(GetText(Value), _
'                                       Me.DataGridTableStyle.DataGrid.Font))
'    Extents.Width += xMargin * 2 + DataGridTableGridLineWidth
'    Extents.Height += yMargin
'    Return Extents
'End Function


'Protected Overloads Overrides Sub Paint(ByVal g As Graphics, _
'                                        ByVal Bounds As Rectangle, _
'                                        ByVal Source As CurrencyManager, _
'                                        ByVal RowNum As Integer)

'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    Paint(g, Bounds, Source, RowNum, False)
'End Sub

'Protected Overloads Overrides Sub Paint(ByVal g As Graphics, _
'                                        ByVal Bounds As Rectangle, _
'                                        ByVal Source As CurrencyManager, _
'                                        ByVal RowNum As Integer, _
'                                        ByVal AlignToRight As Boolean)

'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    Dim Text As String = GetText(GetColumnValueAtRow(Source, RowNum))
'    PaintText(g, Bounds, Text, AlignToRight)
'End Sub

'Protected Overloads Sub Paint(ByVal g As Graphics, _
'                              ByVal Bounds As Rectangle, _
'                              ByVal Source As CurrencyManager, _
'                              ByVal RowNum As Integer, _
'                              ByVal BackBrush As Brush, _
'                              ByVal ForeBrush As Brush, _
'                              ByVal AlignToRight As Boolean)
'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    Dim Text As String = GetText(GetColumnValueAtRow(Source, RowNum))
'    PaintText(g, Bounds, Text, BackBrush, ForeBrush, AlignToRight)
'End Sub

'Protected Overloads Overrides Sub SetDataGridInColumn(ByVal Value As DataGrid)
'    MyBase.SetDataGridInColumn(Value)

'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    If Not (combo.Parent Is Value) Then
'        If Not (combo.Parent Is Nothing) Then
'            combo.Parent.Controls.Remove(combo)
'        End If
'    End If

'    If Not (Value Is Nothing) Then Value.Controls.Add(combo)
'End Sub

'Protected Overloads Overrides Sub UpdateUI(ByVal Source As CurrencyManager, _
'                                           ByVal RowNum As Integer, ByVal InstantText As String)

'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    combo.Text = GetText(GetColumnValueAtRow(Source, RowNum))
'    If Not (InstantText Is Nothing) Then
'        combo.Text = InstantText
'    End If
'End Sub

'----------------------------------------------------------------------
' Helper Methods 
'----------------------------------------------------------------------

'Private ReadOnly Property DataGridTableGridLineWidth() As Integer
'    Get
'        If Me.DataGridTableStyle.GridLineStyle = DataGridLineStyle.Solid Then
'            Return 1
'        Else
'            Return 0
'        End If
'    End Get
'End Property

'Private Sub EndEdit()
'    InEdit = False
'    Invalidate()
'End Sub

'Private Function GetText(ByVal Value As Object) As String
'    If Value Is System.DBNull.Value Then Return NullText

'    If Not Value Is Nothing Then
'        Return Value.ToString
'    Else
'        Return String.Empty
'    End If

'End Function

'Private Sub HideComboBox()
'    If Not m_comboCreated Then
'        Me.getCombo()
'    End If

'    If combo.Focused Then
'        Me.DataGridTableStyle.DataGrid.Focus()
'    End If
'    combo.Visible = False
'End Sub

'Private Sub RollBack()
'    combo.Text = OldVal
'End Sub

'Public Function GetValue() As String
'    Try


'    Catch ex As Exception
'        Log.Show(ex)
'    End Try
'End Function

'Public Sub SetValue(ByVal value As Object)
'    Try

'    Catch ex As Exception
'        Log.Show(ex)
'    End Try
'End Sub

'Public Function GetValue(ByVal row As Integer) As String
'    Try


'    Catch ex As Exception
'        Log.Show(ex)
'    End Try
'End Function

'Public Sub SetValue(ByVal row As Integer, ByVal value As Object)
'    Try


'    Catch ex As Exception
'        Log.Show(ex)
'    End Try
'End Sub


'Private Sub PaintText(ByVal g As Graphics, _
'                      ByVal Bounds As Rectangle, _
'                      ByVal Text As String, _
'                      ByVal AlignToRight As Boolean)


'    Dim BackBrush As Brush = New SolidBrush(Me.DataGridTableStyle.BackColor)
'    Dim ForeBrush As Brush = New SolidBrush(Me.DataGridTableStyle.ForeColor)
'    PaintText(g, Bounds, Text, BackBrush, ForeBrush, AlignToRight)
'End Sub

'Private Sub PaintText(ByVal g As Graphics, _
'                      ByVal TextBounds As Rectangle, _
'                      ByVal Text As String, _
'                      ByVal BackBrush As Brush, _
'                      ByVal ForeBrush As Brush, _
'                      ByVal AlignToRight As Boolean)

'    Dim Rect As Rectangle = TextBounds
'    Dim RectF As RectangleF = RectF.op_Implicit(Rect) ' Convert to RectangleF
'    Dim Format As StringFormat = New StringFormat

'    If AlignToRight Then
'        Format.FormatFlags = StringFormatFlags.DirectionRightToLeft
'    End If

'    Select Case Me.Alignment
'        Case Is = HorizontalAlignment.Left
'            Format.Alignment = StringAlignment.Near
'        Case Is = HorizontalAlignment.Right
'            Format.Alignment = StringAlignment.Far
'        Case Is = HorizontalAlignment.Center
'            Format.Alignment = StringAlignment.Center
'    End Select

'    Format.FormatFlags = Format.FormatFlags Or StringFormatFlags.NoWrap
'    g.FillRectangle(Brush:=BackBrush, Rect:=Rect)

'    Rect.Offset(0, yMargin)
'    Rect.Height -= yMargin
'    g.DrawString(Text, Me.DataGridTableStyle.DataGrid.Font, ForeBrush, RectF, Format)
'    Format.Dispose()

'End Sub

'Public Class SetComboBoxEventArgs
'    Public Combo As XDataGridComboBox
'End Class

'End Class
