Namespace Data
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class XConnection
        Implements IDbConnection

        Private mIsIntrans As Boolean
        Private mOCon As New OleDb.OleDbConnection
        Private mCConnection As OleDb.OleDbConnection
        Private mTrans As OleDb.OleDbTransaction
        Private mAutoRollback As Boolean = True
        Private mOpendWithTrans As Boolean
        Private mUseCopyConnection As Boolean


        Public ReadOnly Property ActiveTransaction() As IDbTransaction
            Get
                If mUseCopyConnection = False Then
                    Return mTrans
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property AutoRollBackWhenError() As Boolean
            Get
                Return mAutoRollback
            End Get
            Set(ByVal Value As Boolean)
                mAutoRollback = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property ConnectionObject() As IDbConnection
            Get
                If Me.mUseCopyConnection Then
                    Return Me.CopyConnection
                End If
                Return mOCon
            End Get
            Set(ByVal Value As IDbConnection)
                mOCon = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property IsIntransaction() As Boolean
            Get
                Return mIsIntrans
            End Get
            Set(ByVal Value As Boolean)
                mIsIntrans = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Sub New()
            mOCon = New OleDb.OleDbConnection
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="conStr"></param>
        Sub New(ByVal conStr As String)
            mOCon = New OleDb.OleDbConnection(conStr)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function BeginTransaction() As System.Data.IDbTransaction Implements System.Data.IDbConnection.BeginTransaction
            Try
                If mOCon.State <> ConnectionState.Open Then
                    mOpendWithTrans = True
                    mOCon.Open()
                End If

                mTrans = mOCon.BeginTransaction

                Me.mIsIntrans = True

                Return mTrans
            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="il"></param>
        ''' <returns></returns>
        Public Overloads Function BeginTransaction(ByVal il As System.Data.IsolationLevel) As System.Data.IDbTransaction Implements System.Data.IDbConnection.BeginTransaction
            Try
                mTrans = mOCon.BeginTransaction(il)

                Me.mIsIntrans = True

                Return mTrans
            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="databaseName"></param>
        Public Sub ChangeDatabase(ByVal databaseName As String) Implements System.Data.IDbConnection.ChangeDatabase
            mOCon.ChangeDatabase(databaseName)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub Close() Implements System.Data.IDbConnection.Close

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property ConnectionString() As String Implements System.Data.IDbConnection.ConnectionString
            Get
                Return mOCon.ConnectionString
            End Get
            Set(ByVal Value As String)
                mOCon.ConnectionString = Value
            End Set
        End Property

        Public ReadOnly Property ConnectionTimeout() As Integer Implements System.Data.IDbConnection.ConnectionTimeout
            Get
                Return mOCon.ConnectionTimeout
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateCommand() As System.Data.IDbCommand Implements System.Data.IDbConnection.CreateCommand
            Dim oC As IDbCommand
            Try
                If Me.mUseCopyConnection Then
                    oC = Me.mCConnection.CreateCommand
                    oC.Connection = mCConnection
                Else
                    oC = mOCon.CreateCommand
                    oC.Connection = Me.mOCon
                End If

                If Me.mIsIntrans Then
                    oC.Transaction = Me.mTrans
                End If

                Return oC

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


        Public ReadOnly Property Database() As String Implements System.Data.IDbConnection.Database
            Get
                Return mOCon.Database
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub Open() Implements System.Data.IDbConnection.Open
            Dim DateFormat As String
            Try
                If Me.mUseCopyConnection Then
                    Me.CopyConnection.Open()
                    Exit Sub
                End If

                mOCon.Open()

                ''If mOCon.State = ConnectionState.Open Then
                ''    Manager.ExecuteNonQuery("set dateformat dmy")
                ''End If

            Catch ex As Exception
                Log.Add(ex)
            Finally

            End Try

            '''

        End Sub

        Public ReadOnly Property State() As System.Data.ConnectionState Implements System.Data.IDbConnection.State
            Get
                If Me.mUseCopyConnection Then
                    Return Me.CopyConnection.State
                End If

                Return mOCon.State
            End Get
        End Property

        Public Sub Dispose() Implements System.IDisposable.Dispose
            mOCon.Dispose()
            '''

        End Sub

        Public Sub CommitTransaction()
            Try
                If mOpendWithTrans Then
                    mOpendWithTrans = False
                    mOCon.Open()
                End If

                If Me.mIsIntrans Then
                    mTrans.Commit()
                End If
                Me.mIsIntrans = False
            Catch ex As Exception
                Log.Add(ex)
            End Try
            '''

        End Sub

        Public Sub RollBackTransaction()
            Try
                If mOpendWithTrans Then
                    mOpendWithTrans = False
                    mOCon.Open()
                End If

                If Me.mIsIntrans Then
                    mTrans.Rollback()
                End If
                Me.mIsIntrans = False
            Catch ex As Exception
                Log.Add(ex)
            End Try
            '''

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property UseCopyConnection() As Boolean
            Get
                Return mUseCopyConnection
            End Get
            Set(ByVal Value As Boolean)
                mUseCopyConnection = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property CopyConnection() As IDbConnection
            Get
                If mCConnection Is Nothing Then
                    mCConnection = New OleDb.OleDbConnection(ConnectionString)
                End If
                Return mCConnection
            End Get
            Set(ByVal Value As IDbConnection)
                mCConnection = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDate() As Date
            Try

                Return Now

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function
    End Class
End Namespace