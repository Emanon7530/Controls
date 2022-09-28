Imports System.Data

Namespace Data

    Public Class XInsertStmt
        Implements IDisposable

        Private m_blnUseSecuence As Boolean
        Private m_blnUseTriggerForSequence As Boolean

        Public TableName As String
        Public SerialColName As String
        Public SequenceObject As String

        Private mHasBinary As Boolean
        Private mExcludeSerialColForInsert As Boolean = True

        Public Command As OleDb.OleDbCommand

        Sub New()
            Me.Command = LibX.Data.Manager.Connection.CreateCommand
            init()
        End Sub

        Sub New(ByVal tabName As String)
            Me.TableName = tabName
            Me.Command = LibX.Data.Manager.Connection.CreateCommand
            init()
        End Sub

        Sub New(ByVal tabName As String, ByVal serialCol As String)
            Me.TableName = tabName
            Me.SerialColName = serialCol
            Me.Command = LibX.Data.Manager.Connection.CreateCommand
            init()
        End Sub

        Sub New(ByVal tabName As String, ByVal serialCol As String, ByVal sequenceDbObject As String)
            Me.TableName = tabName
            Me.SerialColName = serialCol
            Me.SequenceObject = sequenceDbObject
            Me.Command = LibX.Data.Manager.Connection.CreateCommand
            init()
        End Sub


        Sub New(ByVal cmd As IDbCommand)
            Me.Command = cmd
            init()
        End Sub

        Sub New(ByVal tabName As String, ByVal cmd As IDbCommand)
            Me.TableName = tabName
            Me.Command = cmd
            init()
        End Sub

        Private Sub init()
            m_blnUseTriggerForSequence = False
        End Sub

        Public Property ExcludeSerialColForInsert() As Boolean
            Get
                Return mExcludeSerialColForInsert
            End Get
            Set(ByVal Value As Boolean)
                mExcludeSerialColForInsert = Value
            End Set
        End Property

        Public Property HasBinaryFields() As Boolean
            Get
                Return mHasBinary
            End Get
            Set(ByVal Value As Boolean)
                mHasBinary = Value
            End Set
        End Property

        Public Property UseTriggerForSequence() As Boolean
            Get
                Return m_blnUseTriggerForSequence
            End Get
            Set(ByVal Value As Boolean)
                m_blnUseTriggerForSequence = Value
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
                If Not Value Is Nothing Then
                    If Value.GetType.Equals(GetType(Byte())) Or _
                       Value.GetType.Equals(GetType(System.Byte)) Then
                        Me.mHasBinary = True
                    End If
                End If

                If Not Command.Parameters.Contains(name) Then
                    Command.Parameters.Add(name, Value)
                Else
                    Command.Parameters.Item(name) = Value
                End If
            End Set
        End Property

        Default Public Property Fields(ByVal index As Integer) As Object
            Get
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Return Command.Parameters.Item(index)
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
                If Command.Parameters.Count >= index And Command.Parameters.Count <= index Then
                    Command.Parameters.Item(index) = Value
                End If
            End Set
        End Property

        Public Sub ResetText()
            If Command Is Nothing Then
                Command = New OleDb.OleDbCommand
                Command.Connection = LibX.Data.Manager.Connection.ConnectionObject
            End If

            Command.CommandText = ""
        End Sub


        Public Function Execute() As Integer
            Dim Sql As String
            Dim serialPName As String
            Dim pName As String

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
                    Command.CommandText = LibX.Data.Manager.BuildInsert(TableName, Command.Parameters, SerialColName, serialPName, mHasBinary)

                    If Trim(SerialColName) <> "" Then
                        If mExcludeSerialColForInsert Then
                            If Command.Parameters.IndexOf(serialPName) >= 0 Then
                                Command.Parameters.RemoveAt(Command.Parameters.IndexOf(serialPName))
                                Command.CommandText = LibX.Data.Manager.BuildInsert(TableName, Command.Parameters, SerialColName, serialPName, mHasBinary)
                            End If
                        Else
                            If Command.Parameters.IndexOf(serialPName) < 0 Then
                                Dim oParam As IDataParameter = Command.CreateParameter
                                oparam.ParameterName = SerialColName
                                oparam.Value = 0
                                Command.CommandText = LibX.Data.Manager.BuildInsert(TableName, Command.Parameters, SerialColName, serialPName, Me.mHasBinary)
                            End If
                        End If
                    End If
                End If

                Sql = Command.CommandText

                If Trim(SerialColName) <> "" Then

                    blnUseScopeIndentity = True

                    pName = "pSerial_" & SerialColName

                    If blnUseScopeIndentity Then
                        If Me.Command.Parameters.IndexOf(pName) < 0 Then
                            Sql = Command.CommandText & ";" & "SELECT ? = SCOPE_IDENTITY();"
                            Command.CommandText = Sql
                            Command.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord
                            Dim oParam As IDataParameter
                            'oParam = Command.CreateParameter
                            'oParam.ParameterName = pName
                            'oParam.SourceColumn = pName
                            'oParam.DbType = DbType.Int32
                            'oParam.Direction = ParameterDirection.Output
                            oParam = Me.Parameters.Add(pName, DBNull.Value)
                            'oparam.SourceColumn = "SerialCol"
                            oparam.Direction = ParameterDirection.Output
                            oparam.DbType = DbType.Int32
                        End If
                    End If

                End If

                intNum = Command.ExecuteNonQuery

                If Trim(SerialColName) <> "" Then
                    If blnUseScopeIndentity Then
                        intNum = CType(Command.Parameters(pName).Value, Integer)

                        If intNum < 0 Then
                            Throw New Exception("No se pudo obtener el valor serial")
                        End If
                    Else
                        'intNum = App.dbConnection.getSerialValue()
                        'If intNum < 0 Then
                        '    Throw New LastException
                        'End If
                    End If
                End If

                Return intNum
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