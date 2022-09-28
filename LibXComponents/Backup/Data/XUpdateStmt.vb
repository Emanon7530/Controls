Imports System.Data

Namespace Data
    Public Class XUpdateStmt
        Implements IDisposable

        Public TableName As String

        Public Command As OleDb.OleDbCommand
        Public CommandS As OleDb.OleDbCommand
        Public CommandW As OleDb.OleDbCommand
        Private mHasBinary As Boolean
        Private mAditionalFilter As String

        Sub New()
            Command = LibX.Data.Manager.Connection.CreateCommand
            CommandS = LibX.Data.Manager.Connection.CreateCommand
            CommandW = LibX.Data.Manager.Connection.CreateCommand
        End Sub

        Sub New(ByVal tabName As String)
            Me.TableName = tabName
            Command = LibX.Data.Manager.Connection.CreateCommand
            CommandS = LibX.Data.Manager.Connection.CreateCommand
            CommandW = LibX.Data.Manager.Connection.CreateCommand

        End Sub


        Sub New(ByVal cmd As IDbCommand)
            Me.Command = cmd
            Command.Connection = LibX.Data.Manager.Connection.ConnectionObject

        End Sub

        Sub New(ByVal tabName As String, ByVal cmd As IDbCommand)
            Me.TableName = tabName
            Me.Command = cmd
            Command.Connection = LibX.Data.Manager.Connection.ConnectionObject
        End Sub

        Public Property HasBinaryFields() As Boolean
            Get
                Return mHasBinary
            End Get
            Set(ByVal Value As Boolean)
                mHasBinary = Value
            End Set
        End Property

        Public ReadOnly Property Parameters() As OleDb.OleDbParameterCollection
            Get
                Return Command.Parameters
            End Get
        End Property

        Public Sub ResetText()
            If Command Is Nothing Then
                Command = LibX.Data.Manager.Connection.CreateCommand
            End If

            Command.CommandText = ""
        End Sub


        Public ReadOnly Property ParametersSet() As OleDb.OleDbParameterCollection
            Get
                If CommandS Is Nothing Then
                    CommandS = LibX.Data.Manager.Connection.CreateCommand
                End If
                Return CommandS.Parameters
            End Get
        End Property

        Public Property AditionalFilter() As String
            Get
                Return mAditionalFilter
            End Get
            Set(ByVal Value As String)
                mAditionalFilter = Value
            End Set
        End Property


        Public Function Execute() As Integer
            Dim Sql As String

            Try
                Dim intNum As Integer = 1
                Dim blnUseScopeIndentity As Boolean
                Dim oP As IDataParameter
                Dim ooP As IDataParameter

                If LibX.Data.Manager.Connection.State <> ConnectionState.Open Then
                    LibX.Data.Manager.Connection.Open()
                End If

                'CommandA = LibX.Data.Manager.Connection.CreateCommand
                'For Each oP In CommandS.Parameters
                '    ooP = Command.CreateParameter
                '    ooP.ParameterName = oP.ParameterName
                '    ooP.DbType = oP.DbType
                '    ooP.Direction = oP.Direction
                '    ooP.SourceColumn = oP.SourceColumn
                '    ooP.Value = oP.Value
                '    CommandA.Parameters.Add(ooP)
                'Next

                'For Each oP In Command.Parameters
                '    ooP = Command.CreateParameter
                '    ooP.ParameterName = oP.ParameterName
                '    ooP.DbType = oP.DbType
                '    ooP.Direction = oP.Direction
                '    ooP.SourceColumn = oP.SourceColumn
                '    ooP.Value = oP.Value
                '    CommandA.Parameters.Add(ooP)
                'Next

                If Trim(Command.CommandText) = "" Then
                    Command.CommandText = LibX.Data.Manager.BuildUpdate(TableName, CommandW.Parameters, CommandS.Parameters, Command.Parameters, mHasBinary)

                    'For Each oP In CommandA.Parameters
                    '    If not Command.Parameters.Contains(Trim(oP.ParameterName)) Then
                    '        ooP = Command.CreateParameter
                    '        ooP.ParameterName = oP.ParameterName
                    '        ooP.DbType = oP.DbType
                    '        ooP.Direction = oP.Direction
                    '        ooP.SourceColumn = oP.SourceColumn
                    '        ooP.Value = oP.Value
                    '        Command.Parameters.Add(ooP)
                    '    End If
                    'Next
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
            Finally

            End Try
        End Function


        Public Property Fields(ByVal name As String) As Object
            Get
                If Not Command.Parameters.Contains(name) Then
                    Return DBNull.Value
                End If
                Return Command.Parameters.Item(name)
            End Get
            Set(ByVal Value As Object)
                Dim oP As IDbDataParameter
                If Not Command.Parameters.Contains(name) Then
                    oP = Command.Parameters.Add(name, Value)

                    If Value.GetType.Equals(GetType(Decimal)) Then
                        oP.DbType = DbType.Decimal
                    End If
                    If Value.GetType.Equals(GetType(Int16)) Then
                        oP.DbType = DbType.Int16
                    End If
                    If Value.GetType.Equals(GetType(Int32)) Then
                        oP.DbType = DbType.Int32
                    End If
                    If Value.GetType.Equals(GetType(Int64)) Then
                        oP.DbType = DbType.Int64
                    End If
                    If Value.GetType.Equals(GetType(Date)) Then
                        oP.DbType = DbType.Date
                    End If
                    If Value.GetType.Equals(GetType(DateTime)) Then
                        oP.DbType = DbType.Date
                    End If
                    If Value.GetType.Equals(GetType(Double)) Then
                        oP.DbType = DbType.Double
                    End If


                End If
                Command.Parameters.Item(name).Value = Value

                If CommandW Is Nothing Then
                    CommandW = LibX.Data.Manager.Connection.CreateCommand
                End If

                If Not CommandW.Parameters.Contains(name) Then
                    CommandW.Parameters.Add(name, Value)
                End If
                CommandW.Parameters.Item(name).Value = Value

            End Set
        End Property

        Public Property Fields(ByVal index As Integer) As Object
            Get
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Return Command.Parameters.Item(index)
                End If
                Return DBNull.Value
            End Get
            Set(ByVal Value As Object)
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Command.Parameters.Item(index).Value = Value
                End If

                If CommandW Is Nothing Then
                    CommandW = LibX.Data.Manager.Connection.CreateCommand
                End If
                If CommandW.Parameters.Count >= index And CommandW.Parameters.Count <= index Then
                    CommandW.Parameters.Item(index).Value = Value
                End If

            End Set
        End Property

        Default Public Property FieldsSet(ByVal name As String) As Object
            Get
                If Not CommandS.Parameters.Contains(name) Then
                    Return DBNull.Value
                End If
                Return CommandS.Parameters.Item(name)
            End Get
            Set(ByVal Value As Object)
                If Not Value Is Nothing Then
                    If Value.GetType.Equals(GetType(Byte())) Or _
                       Value.GetType.Equals(GetType(System.Byte)) Then
                        Me.mHasBinary = True
                    End If
                End If

                If CommandS Is Nothing Then
                    CommandS = LibX.Data.Manager.Connection.CreateCommand
                End If


                If Not CommandS.Parameters.Contains(name) Then
                    CommandS.Parameters.Add(name, Value)
                Else
                    CommandS.Parameters.Item(name).Value = Value
                End If

                If Not Command.Parameters.Contains(name) Then
                    Dim oP As IDbDataParameter

                    oP = Command.Parameters.Add(name, Value)

                    If Value.GetType.Equals(GetType(Decimal)) Then
                        oP.DbType = DbType.Decimal
                    End If
                    If Value.GetType.Equals(GetType(Int16)) Then
                        oP.DbType = DbType.Int16
                    End If
                    If Value.GetType.Equals(GetType(Int32)) Then
                        oP.DbType = DbType.Int32
                    End If
                    If Value.GetType.Equals(GetType(Int64)) Then
                        oP.DbType = DbType.Int64
                    End If
                    If Value.GetType.Equals(GetType(Date)) Then
                        oP.DbType = DbType.Date
                    End If
                    If Value.GetType.Equals(GetType(DateTime)) Then
                        oP.DbType = DbType.Date
                    End If
                    If Value.GetType.Equals(GetType(Double)) Then
                        oP.DbType = DbType.Double
                    End If

                Else
                    Command.Parameters.Item(name).Value = Value
                End If

            End Set
        End Property

        Default Public Property FieldsSet(ByVal index As Integer) As Object
            Get
                If CommandS.Parameters.Count >= index And CommandS.Parameters.Count <= index Then
                    Return CommandS.Parameters.Item(index)
                End If
                Return DBNull.Value
            End Get
            Set(ByVal Value As Object)
                If Not Value Is Nothing Then
                    If Value.GetType.Equals(GetType(Byte())) Or _
                       Value.GetType.Equals(GetType(System.Byte)) Then
                        Me.mHasBinary = True
                    End If
                End If

                If CommandS Is Nothing Then
                    CommandS = LibX.Data.Manager.Connection.CreateCommand
                End If

                If CommandS.Parameters.Count >= index And CommandS.Parameters.Count <= index Then
                    CommandS.Parameters.Item(index).Value = Value
                End If

                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Command.Parameters.Item(index).Value = Value
                End If

            End Set
        End Property

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not Command Is Nothing Then
                Command.Dispose()
            End If
            If Not CommandS Is Nothing Then
                CommandS.Dispose()
            End If
        End Sub

    End Class
End Namespace