Imports System.Data

Namespace Data
    Public Class XSelecStmt
        Implements IDisposable

        Public TableName As String

        Public Command As OleDb.OleDbCommand
        Private mAditionalFilter As String
        Private mSql As String

        Sub New()
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub

        Sub New(ByVal tabName As String)
            Me.TableName = tabName
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub

        Sub New(ByVal tabName As String, ByVal sqlstring As String)
            mSql = sqlstring
            TableName = tabName
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub


        Sub New(ByVal cmd As IDbCommand)
            Me.Command = cmd
        End Sub

        Sub New(ByVal tabName As String, ByVal cmd As IDbCommand)
            Me.TableName = tabName
            Me.Command = cmd
        End Sub

        Public Property Sql() As String
            Get
                Return mSql
            End Get
            Set(ByVal Value As String)
                mSql = Value
            End Set
        End Property

        Public ReadOnly Property Parameters() As OleDb.OleDbParameterCollection
            Get
                Return Command.Parameters
            End Get
        End Property

        Default Public Property Fields(ByVal name As String) As Object
            Get
                If Not Command.Parameters.Contains(name) Then
                    Return DBNull.Value
                End If
                Return Command.Parameters.Item(name)
            End Get
            Set(ByVal Value As Object)
                If Not Command.Parameters.Contains(name) Then
                    Command.Parameters.Add(name, Value)
                Else
                    Command.Parameters.Item(name) = Value
                End If
            End Set
        End Property

        Public Property AditionalFilter() As String
            Get
                Return mAditionalFilter
            End Get
            Set(ByVal Value As String)
                mAditionalFilter = Value
            End Set
        End Property

        Public Sub ResetText()
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub

        Default Public Property Fields(ByVal index As Integer) As Object
            Get
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Return Command.Parameters.Item(index)
                End If
                Return DBNull.Value
            End Get
            Set(ByVal Value As Object)
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Command.Parameters.Item(index) = Value
                End If
            End Set
        End Property


        Public Function GetTable() As DataTable
            Dim Sql As String
            Try
                Dim intNum As Integer = 1
                Dim blnUseScopeIndentity As Boolean

                If Command Is Nothing Then
                    Command = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildSelect(TableName, Command.Parameters, mSql)

                    If Trim(mAditionalFilter) <> "" Then
                        Command.CommandText = String.Concat(Command.CommandText, " and ", mAditionalFilter)
                    End If


                End If

                Sql = Command.CommandText
                Dim oad As New OleDb.OleDbDataAdapter(Command)
                Dim oTable As New DataTable(TableName)
                oad.Fill(oTable)


                Return oTable
            Catch ex As Exception
                Log.Add(ex, Sql)
            End Try
        End Function

        Public Function GetTable(ByVal withKey As Boolean) As DataTable
            Dim Sql As String
            Try
                Dim intNum As Integer = 1
                Dim blnUseScopeIndentity As Boolean

                If Command Is Nothing Then
                    Command = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildSelect(TableName, Command.Parameters, mSql)

                    If Trim(mAditionalFilter) <> "" Then
                        Command.CommandText = String.Concat(Command.CommandText, " and ", mAditionalFilter)
                    End If


                End If

                Sql = Command.CommandText
                Dim oad As New OleDb.OleDbDataAdapter(Command)
                If withKey Then
                    oad.MissingSchemaAction = MissingSchemaAction.AddWithKey
                End If
                Dim oTable As New DataTable(TableName)
                oad.Fill(oTable)


                Return oTable
            Catch ex As Exception
                Log.Add(ex, Sql)
            End Try
        End Function

        Public Function GetDataRow() As DataRow
            Dim Sql As String
            Try
                Dim intNum As Integer = 1
                Dim blnUseScopeIndentity As Boolean

                If Command Is Nothing Then
                    Command = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildSelect(TableName, Command.Parameters, mSql)

                    If Trim(mAditionalFilter) <> "" Then
                        Command.CommandText = String.Concat(Command.CommandText, " and ", mAditionalFilter)
                    End If

                End If

                Sql = Command.CommandText
                Dim oad As New OleDb.OleDbDataAdapter(Command)
                Dim oTable As New DataTable(TableName)

                oad.Fill(oTable)

                If oTable.Rows.Count > 0 Then
                    Return oTable.Rows(0)
                End If

            Catch ex As Exception
                Log.Add(ex, Sql)
            End Try
        End Function


        Public Function GetScalar() As Object
            Dim Sql As String
            Try
                Dim intNum As Integer = 1

                If Command Is Nothing Then
                    Command = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildSelect(TableName, Command.Parameters, mSql)

                    If Trim(mAditionalFilter) <> "" Then
                        Command.CommandText = String.Concat(Command.CommandText, " and ", mAditionalFilter)
                    End If

                End If

                Sql = Command.CommandText

                Return Command.ExecuteScalar

            Catch ex As Exception
                Log.Add(ex, Sql)
            End Try
        End Function

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not Command Is Nothing Then
                Command.Dispose()
            End If
        End Sub
    End Class

End Namespace