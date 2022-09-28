Imports System.Data

Namespace Data
    Public Class XDeleteStmt
        Implements IDisposable

        Public TableName As String

        Public Command As OleDb.OleDbCommand
        Private mAditionalFilter As String

        Sub New()
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub

        Sub New(ByVal tabName As String)
            Me.TableName = tabName
            Command = LibX.Data.Manager.Connection.CreateCommand
        End Sub


        Sub New(ByVal cmd As IDbCommand)
            Me.Command = cmd
        End Sub

        Sub New(ByVal tabName As String, ByVal cmd As IDbCommand)
            Me.TableName = tabName
            Me.Command = cmd
        End Sub

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


        Public Function Execute() As Integer
            Dim Sql As String
            Try
                Dim intNum As Integer = 1
                Dim blnUseScopeIndentity As Boolean

                If LibX.Data.Manager.Connection.State <> ConnectionState.Open Then
                    LibX.Data.Manager.Connection.Open()
                End If

                If Command Is Nothing Then
                    Command = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildDelete(TableName, Command.Parameters)

                    If Trim(mAditionalFilter) <> "" Then
                        Command.CommandText = String.Concat(Command.CommandText, " and ", mAditionalFilter)
                    End If
                End If

                Sql = Command.CommandText

                intNum = Command.ExecuteNonQuery

                Return 1
            Catch ex As Exception
                Log.Add(ex, Sql)
                Return -1
            End Try
        End Function

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not Command Is Nothing Then
                Command.Dispose()
            End If
        End Sub
    End Class

End Namespace