Option Strict Off
Option Explicit On 
Imports System.ComponentModel
Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms


Public Class XDataGridLinkColumn
    Inherits XEditTextBoxColumn


    Public mLinkCreated As Boolean
    '<NonSerialized()> _
    Public mLink As LinkLabel

    Public Event SettingLinkLabel(ByVal sender As Object, ByVal e As EventArgs)
    Public Event LinkeClicked(ByVal sender As Object, ByVal e As EventArgs)



    <Browsable(False)> _
    Public ReadOnly Property [LinkLabel]() As LinkLabel
        Get
            Return mLink
        End Get
    End Property

    Protected Overrides Sub Abort(ByVal rowNum As Integer)
        MyBase.Abort(rowNum)
    End Sub

    Protected Overrides Function Commit(ByVal dataSource As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer) As Boolean
        Return MyBase.Commit(dataSource, rowNum)
    End Function

    Protected Overrides Function GetMinimumHeight() As Integer
        Return MyBase.GetMinimumHeight
    End Function

    Protected Overrides Function GetPreferredHeight(ByVal g As System.Drawing.Graphics, ByVal value As Object) As Integer
        Return MyBase.GetPreferredHeight(g, value)
    End Function

    Protected Overrides Function GetPreferredSize(ByVal g As System.Drawing.Graphics, ByVal value As Object) As System.Drawing.Size
        Return MyBase.GetPreferredSize(g, value)
    End Function


    Private Sub OnMouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs)
        If Not mLink Is Nothing Then
            mLink.Visible = False
        End If
    End Sub

    Private Sub OnCurrentRowChanged(ByVal sender As Object, ByVal e As LibXGrid.XDataGridCurrentRowChangedEventArgs)
        Dim bounds As System.Drawing.Rectangle
        Dim Grid As LibXGrid = Me.DataGridTableStyle.DataGrid
        bounds = Grid.GetCellBounds(e.row, e.column)
        Me.SetLink(e.column, e.row, bounds, True)
    End Sub

    Private Sub onGridScrolling(ByVal sender As Object, ByVal e As EventArgs)
        If Not mLink Is Nothing Then
            mLink.Visible = False
        End If
    End Sub

    Private Sub DoFirstSet(ByVal sender As Object)
        Dim ds As LibXConnector
        ds = sender
        If Not ds.IsEditing Then
            Dim bounds As System.Drawing.Rectangle
            Dim Grid As LibXGrid = Me.DataGridTableStyle.DataGrid
            Dim rownum As Integer
            rownum = Grid.CurrentCell.RowNumber
            Dim col As Integer = Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me)
            If rownum < 0 Or col < 0 Then
                Exit Sub
            End If
            Try
                bounds = Grid.GetCellBounds(rownum, col)
                Me.SetLink(col, rownum, bounds, False)
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub onAfterRowChange(ByVal sender As Object, ByVal e As XRowChangeEventArgs)
        Me.DoFirstSet(sender)
    End Sub


    Private Sub InitLink()
        RaiseEvent SettingLinkLabel(Me, New EventArgs)

        If Me.mLink Is Nothing Then
            mLink = New LinkLabel
            Select Case Me.Alignment
                Case HorizontalAlignment.Center
                    mLink.TextAlign = ContentAlignment.MiddleCenter
                Case HorizontalAlignment.Left
                    mLink.TextAlign = ContentAlignment.MiddleLeft
                Case HorizontalAlignment.Right
                    mLink.TextAlign = ContentAlignment.MiddleRight
            End Select

        End If
        Dim Grid As LibXGrid = Me.DataGridTableStyle.DataGrid

        RemoveHandler mLink.LinkClicked, AddressOf OnLinkClicked
        AddHandler mLink.LinkClicked, AddressOf OnLinkClicked

        RemoveHandler Grid.MouseWheel, AddressOf OnMouseWheel
        AddHandler Grid.MouseWheel, AddressOf OnMouseWheel

        RemoveHandler Grid.GridScrolling, AddressOf onGridScrolling
        AddHandler Grid.GridScrolling, AddressOf onGridScrolling

        RemoveHandler Grid.CurrentRowChanged, AddressOf OnCurrentRowChanged
        AddHandler Grid.CurrentRowChanged, AddressOf OnCurrentRowChanged

        RemoveHandler Grid.Refreshing, AddressOf OnRefreshing
        AddHandler Grid.Refreshing, AddressOf OnRefreshing


        If Not Grid.DataSource Is Nothing Then
            If TypeOf Grid.DataSource Is LibXConnector Then
                RemoveHandler CType(Grid.DataSource, LibXConnector).AfterRowChange, AddressOf onAfterRowChange
                AddHandler CType(Grid.DataSource, LibXConnector).AfterRowChange, AddressOf onAfterRowChange
            End If
        End If



        mLinkCreated = True
    End Sub

    Private Sub OnRefreshing(ByVal sender As Object, ByVal e As EventArgs)
        If Not mLink Is Nothing Then

            mLink.Visible = False
            Dim Grid As LibXGrid = Me.DataGridTableStyle.DataGrid
            If Grid.CurrentRowIndex = -1 Then
                Me.mLink.Visible = False
            Else
                Dim bounds As System.Drawing.Rectangle
                bounds = Grid.GetCellBounds(0, Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me))
                Me.SetLink(Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me), 0, bounds, True)
            End If
        End If
    End Sub

    Private Sub OnLinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs)
        RaiseEvent LinkeClicked(sender, e)
    End Sub

    Private Sub SetLink(ByVal col As Integer, ByVal rowNum As Integer, ByVal bounds As System.Drawing.Rectangle, ByVal checkState As Boolean)
        Try
            If col < 0 Then
                Exit Sub
            End If

            mLink.Visible = False

            If col <> Me.DataGridTableStyle.GridColumnStyles.IndexOf(Me) Then
                Exit Sub
            End If

            Dim Grid As LibXGrid = Me.DataGridTableStyle.DataGrid

            If checkState Then
                If Not Grid.DataSource Is Nothing Then
                    If TypeOf Grid.DataSource Is LibXConnector Then
                        If CType(Grid.DataSource, LibXConnector).State = LibxConnectorState.Query Then
                            Exit Sub
                        End If
                    End If
                End If
            End If

            mLink.Top = bounds.Top
            mLink.Left = bounds.Left
            mLink.Width = bounds.Width
            mLink.Height = bounds.Height


            If IsNull(Grid.Item(rowNum, col)) Then
                mLink.Text = ""
            Else
                mLink.Text = Grid.Item(rowNum, col)
            End If

            mLink.Visible = True
            mLink.BringToFront()


        Catch ex As System.Exception
            Log.Show(ex)
        End Try

    End Sub

    Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer)
        MyBase.Paint(g, bounds, source, rowNum)
    End Sub

    Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal alignToRight As Boolean)
        MyBase.Paint(g, bounds, source, rowNum, alignToRight)
    End Sub

    Protected Overrides Sub SetDataGridInColumn(ByVal value As System.Windows.Forms.DataGrid)
        MyBase.SetDataGridInColumn(value)
        Try
            If Me.DesignMode Then
                Exit Sub
            End If

            If Not Me.mLinkCreated Then
                InitLink()
            End If

            If Not (mLink.Parent Is value) Then
                If Not (mLink.Parent Is Nothing) Then
                    mLink.Parent.Controls.Remove(mLink)
                End If
            End If

            If Not (value Is Nothing) Then value.Controls.Add(mLink)

            mLink.Visible = False

        Catch ex As Exception
            Log.Show(ex)
        End Try
    End Sub


End Class
