Imports System.Configuration.ConfigurationSettings
Imports LibXComponents.DataAccess ' For AppDbContext
Imports LibXComponents.Entities   ' For User entity
Imports Microsoft.EntityFrameworkCore ' For FirstOrDefault and other LINQ methods

Namespace Data
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class Manager

        Private Shared mConnection As XConnection
        Private Shared mHasConnection As Boolean
        Private Shared mIsInTransaction As Boolean
        Private Shared mLastSerial As Integer
        Private Shared mSaveDate As String
        Private Shared mIsAuthenticated As Boolean
        Private Shared mIsCancel As Boolean
        Private Shared mValidateUser As Boolean = True
        Private Shared msValidateUser As String
        Private Shared mIsDesign As Boolean



        Public Shared Property IsDesignMode() As Boolean
            Get
                Return mIsDesign
            End Get
            Set(ByVal Value As Boolean)
                mIsDesign = Value
            End Set
        End Property

        Public Shared Property ValidateUser() As Boolean
            Get
                If mIsDesign Then
                    Return False
                End If

                Return mValidateUser
            End Get
            Set(ByVal Value As Boolean)
                mValidateUser = Value
            End Set
        End Property

        Public Shared Property IsAuthenticated() As Boolean
            Get
                Return mIsAuthenticated
            End Get
            Set(ByVal Value As Boolean)
                mIsAuthenticated = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Shared Sub New()
            System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-DO")

            If Trim(msValidateUser) = "" Then
                msValidateUser = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXValidateUser")
                If Trim(msValidateUser) = "" Then
                    msValidateUser = "True"
                End If
                mValidateUser = CBool(msValidateUser)
            End If

        End Sub

        Public Shared Property LastSerialValue() As Integer
            Get
                Return mLastSerial
            End Get
            Set(ByVal Value As Integer)
                mLastSerial = Value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="numero"></param>
        ''' <returns></returns>
        Public Shared Function GetMessage(ByVal numero As Integer) As String
            Return GetMessage(numero, "", Nothing)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="numero"></param>
        ''' <param name="comment"></param>
        ''' <returns></returns>
        Public Shared Function GetMessage(ByVal numero As Integer, ByVal comment As String) As String
            Return GetMessage(numero, comment, Nothing)
        End Function

        Public Shared Function GetMessageWithNoFound(ByVal numero As Integer, ByVal comment As String, ByVal replaceValues() As Object) As String
            Try
                Dim s As String
                Dim oRow As DataRow

                s = String.Concat("Select * from libxmessages where msg_numero =", numero)

                oRow = LibX.Data.Manager.GetDataRow(s)

                If oRow Is Nothing Then
                    Return "A ocurrido un error inesperado, no se completó la acción!!"
                End If

                s = oRow!msg_nombre

                If Trim(comment) <> "" Then
                    s = String.Concat(s, vbCrLf, comment)
                End If


                If Not replaceValues Is Nothing Then
                    s = String.Format(s, replaceValues)
                End If

                Return s

            Catch ex As Exception
                Log.Show(ex)
            End Try

        End Function

        Public Shared Function GetMessage(ByVal numero As Integer, ByVal comment As String, ByVal replaceValues() As Object) As String
            Try
                Dim s As String
                Dim oRow As DataRow

                s = String.Concat("Select * from libxmessages where msg_numero =", numero)

                oRow = LibX.Data.Manager.GetDataRow(s)

                If oRow Is Nothing Then
                    Return String.Concat("A ocurrido un error inesperado, no se completó la acción!!")
                End If

                s = oRow!msg_nombre

                If Trim(comment) <> "" Then
                    s = String.Concat(s, vbCrLf, comment)
                End If


                If Not replaceValues Is Nothing Then
                    s = String.Format(s, replaceValues)
                End If

                Return s

            Catch ex As Exception
                Log.Show(ex)
            End Try
        End Function

        Public Shared Property HasConnection() As Boolean
            Get
                Return mHasConnection
            End Get
            Set(ByVal Value As Boolean)
                mHasConnection = Value
            End Set
        End Property

        Public Shared Property Connection() As XConnection
            Get
                Return mConnection
            End Get
            Set(ByVal Value As XConnection)
                mConnection = Value
            End Set
        End Property

        Public Shared Function GetSaveDateFormat() As String
            If Trim(mSaveDate) <> "" Then
                Return mSaveDate
            End If
            Dim s As String
            s = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXSaveDateFormat")
            If Trim(s) = "" Then
                s = "MM/dd/yyyy"
            End If
            mSaveDate = s
            Return s
        End Function


        Public Shared Function ExecuteLogin() As Boolean
            Try
                OpenConnection()

                If App.ExecuteExit Or Not mIsAuthenticated Then
                    Return False
                End If

                Return True

            Catch ex As Exception
                Log.Show(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        Public Shared Sub OpenConnection()
            Dim mConStr As String
            Dim Intentos As Integer
            Try

                If Not ValidateUser Then
                    mConStr = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectionStr")
                    mConnection = New XConnection(mConStr)
                    mConnection.Open()
                    mHasConnection = True
                    mIsAuthenticated = True
                    Exit Sub
                End If

                If Not mIsAuthenticated Then
                    mIsCancel = False
                    If Not Login() Then
                        If Not mIsCancel Then
                            Throw New Exception("Usuario o Clave inválidos")
                        End If
                    End If
                    If Not mIsAuthenticated Then

                        '-->Como el application.exit no funciona desde un control hosted
                        '   el load de la forma como quiera se haria, por lo que itengo dejar la conexión hecha
                        '   para no tener que validar en todos los loads

                        mConStr = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectionStr")
                        mConnection = New XConnection(mConStr)
                        mConnection.Open()

                        App.ExecuteExit = True
                        System.Windows.Forms.Application.Exit()
                    End If
                End If

            Catch ex As Exception
                Log.UseConnection = False
                If Not mValidateUser Then
                    Log.Show(ex)
                End If
                App.ExecuteExit = True
                System.Windows.Forms.Application.Exit()
            End Try

        End Sub

        Public Shared Function Login() As Boolean
            Dim sUsr As String
            Dim sPass As String
            Dim strConnectionString As String
            Dim mConStr As String
            Dim mSuCursal As Integer
            Dim NoIntentos As Integer
            Try

                Do While NoIntentos <= 2
                    NoIntentos += 1
                    If DoValidLogin(sUsr, sPass) Then
                        If Trim(sUsr) = "-l" Then
                            mIsCancel = True
                            NoIntentos = 3
                            Return False
                        End If

                        mConStr = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectionStr")
                        mSuCursal = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings.Get("LibxScDefault")) ' Ensure mSuCursal is Integer for EF Core query
                        ' mConnection = New XConnection(mConStr) ' Temporarily commented out for EF Core transition
                        ' mConnection.Open() ' This will eventually be removed or refactored too. Old connection not needed for this EF Core path.

                        Dim sSql As String = String.Concat("Select * from scusers where username ='", Trim(sUsr), "' and passwrd='", Trim(sPass), "' and suc_code = " & mSuCursal) ' This line will also be removed

                        ' --- EF Core user query ---
                        Using context As New AppDbContext() ' AppDbContext now uses temporary hardcoded connection string
                            Dim trimmedUser = Trim(sUsr)
                            Dim appUser = context.Users.FirstOrDefault(Function(u) u.UserName = trimmedUser AndAlso u.SucursalCode = mSuCursal)

                            If appUser IsNot Nothing Then
                                ' Logic for successful user find will go here
                            Else
                                System.Windows.Forms.MessageBox.Show("Usuario o ContraseÃ±a invÃ¡lida.", "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                            ' Further logic will be inserted here in next steps (checking appUser, password, etc.)

                        End Using
                        ' --- End EF Core user query ---

                        ' Dim oRow As DataRow = LibX.Data.Manager.GetDataRow(sSql) ' Refactoring to EF Core

                        If Not oRow Is Nothing Then ' This condition will be replaced by EF Core appUser check
                            User.UserID = oRow!userid.ToString
                            User.UserName = oRow!username.ToString
                            User.VendedorID = Val(oRow!vend_code.ToString)
                            User.isLogged = True
                            ''User.WhDefault = AppSettings.Get("LibXWhDefault")
                            User.WhDefault = LibX.Data.Manager.GetScalar("select whse_code from cgsucursal where suc_code = " & Val(oRow!suc_code.ToString))
                            User.Sucursal = Val(oRow!suc_code.ToString)
                            mIsAuthenticated = True
                            NoIntentos = 3
                            Return True
                        Else
                            System.Windows.Forms.MessageBox.Show("Usuario o Contraseña inválida", "ACCESO DENEGADO", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If

                        mHasConnection = True
                    End If

                    If Trim(sUsr) = "-l" Then
                        mIsCancel = True
                        NoIntentos = 3
                        Return False
                    End If
                Loop

                Return False
            Catch ex As Exception
                Log.UseConnection = False
                Log.Show(ex)
            Finally

            End Try

        End Function

        Private Shared Function DoValidLogin(ByRef usr As String, ByRef password As String) As Boolean
            Dim ofm As New fLogin

            Do While True
                ofm.ShowDialog()
                usr = Trim(ofm.mUsr)
                password = Trim(ofm.mPass)
                If Trim(usr) = "-l" Then
                    Return False
                End If
                If Trim(usr) <> "" Then
                    Exit Do
                End If
            Loop

            Return True

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <returns></returns>
        Public Shared Function GetNewDataTable(ByVal tabName As String) As DataTable
            Try
                Dim oT As New DataTable

                FillSchema(oT, tabName, "", False)

                Return oT

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        Public Shared Sub Save(ByVal table As DataTable)
            Try

                Dim oA As New Adapter
                oA.Save(table)

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="serialColName"></param>
        Public Shared Sub Save(ByVal table As DataTable, ByVal serialColName As String)
            Try

                Dim oA As New Adapter
                oA.Save(table, serialColName)

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function GetDataTable(ByVal sql As String) As DataTable
            Try
                Return GetDataTable(sql, False)
            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal tableName As String) As DataTable
            Try
                Return GetDataTable(sql, tableName, True)
            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="withkey"></param>
        ''' <returns></returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal withkey As Boolean) As DataTable
            Try

                Return GetDataTable(sql, "", withkey)

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="tableName"></param>
        ''' <param name="withkey"></param>
        ''' <returns></returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal tableName As String, ByVal withkey As Boolean) As DataTable
            Try

                Dim dt As New DataTable
                Dim oAd As New OleDb.OleDbDataAdapter(sql, mConnection.ConnectionObject)
                oAd.SelectCommand.Transaction = mConnection.ActiveTransaction
                If withkey Then
                    oAd.MissingSchemaAction = MissingSchemaAction.AddWithKey
                End If
                oAd.Fill(dt)
                dt.TableName = Trim(tableName)

                Return dt

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function ExecuteNonQuery(ByVal sql As String) As Integer
            Dim oResult As Integer
            Try
                If mConnection.State <> ConnectionState.Open Then
                    mConnection.Open()
                End If
                Dim dt As New DataTable
                Dim oCmd As New OleDb.OleDbCommand(sql, mConnection.ConnectionObject)
                oCmd.Transaction = mConnection.ActiveTransaction

                oResult = oCmd.ExecuteNonQuery
                oCmd.Dispose()

                Return oResult

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function GetDataRow(ByVal sql As String) As DataRow
            Try

                Dim dt As New DataTable
                Dim oAd As New OleDb.OleDbDataAdapter(sql, mConnection.ConnectionObject)
                oAd.SelectCommand.Transaction = mConnection.ActiveTransaction
                oAd.Fill(dt)

                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0)
                End If

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function GetScalar(ByVal sql As String) As Object
            Dim oResult As Object
            Try

                    If mConnection.State <> ConnectionState.Open Then
                    mConnection.Open()
                End If
                Dim dt As New DataTable
                Dim oCmd As New OleDb.OleDbCommand(sql, mConnection.ConnectionObject)
                oCmd.Transaction = mConnection.ActiveTransaction

                oResult = oCmd.ExecuteScalar
                oCmd.Dispose()

                Return oResult

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function ExecuteSql(ByVal sql As String) As Integer
            Dim oResult As Integer
            Try

                If mConnection.State <> ConnectionState.Open Then
                    mConnection.Open()
                End If
                Dim dt As New DataTable
                Dim oCmd As New OleDb.OleDbCommand(sql, mConnection.ConnectionObject)
                oCmd.Transaction = mConnection.ActiveTransaction

                oResult = oCmd.ExecuteNonQuery
                oCmd.Dispose()

                Return oResult

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dt"></param>
        ''' <param name="tabName"></param>
        ''' <param name="sSql"></param>
        ''' <param name="withKey"></param>
        Public Shared Sub FillSchema(ByVal dt As DataTable, ByVal tabName As String, ByVal sSql As String, ByVal withKey As Boolean)
            Try
                If Trim(sSql) = "" Then
                    sSql = String.Concat("Select * from ", tabName)
                End If

                Dim oAd As New OleDb.OleDbDataAdapter(sSql, mConnection.ConnectionObject)
                oAd.SelectCommand.Transaction = mConnection.ActiveTransaction

                If withKey Then
                    oAd.MissingSchemaAction = MissingSchemaAction.AddWithKey
                End If
                oAd.FillSchema(dt, SchemaType.Source)
                dt.TableName = tabName

            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="oTable"></param>
        ''' <param name="key"></param>
        Public Shared Sub ApplayKeyString(ByVal oTable As DataTable, ByVal key As String)
            Try
                Dim k() As String = Split(key, ",")
                Dim cols(k.Length - 1) As DataColumn
                Dim sCol As String
                Dim i As Integer = 0

                For Each sCol In k
                    cols(i) = oTable.Columns(sCol)
                    i = i + 1
                Next

                oTable.PrimaryKey = cols

            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dt"></param>
        ''' <param name="tableName"></param>
        Public Shared Sub FillSchema(ByVal dt As DataTable, ByVal tableName As String)
            Try
                FillSchema(dt, tableName, "", False)
            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dt"></param>
        ''' <param name="tableName"></param>
        ''' <param name="withkey"></param>
        Public Shared Sub FillSchema(ByVal dt As DataTable, ByVal tableName As String, ByVal withkey As Boolean)
            Try
                FillSchema(dt, tableName, "", withkey)
            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <param name="withKey"></param>
        ''' <returns></returns>
        Public Shared Function GetNewDataTable(ByVal tabName As String, ByVal withKey As Boolean) As DataTable
            Try
                Dim oT As New DataTable

                FillSchema(oT, tabName, "", True)

                Return oT

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="usr"></param>
        ''' <param name="pass"></param>
        Public Shared Sub OpenConnection(ByVal usr As String, ByVal pass As String)

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <param name="params"></param>
        ''' <param name="serialColName"></param>
        ''' <param name="serialParamName"></param>
        ''' <param name="hasBinaryFields"></param>
        ''' <returns></returns>
        Public Shared Function BuildInsert(ByVal tabName As String, ByVal params As IDataParameterCollection, ByVal serialColName As String, ByRef serialParamName As String, ByVal hasBinaryFields As Boolean) As String
            Try
                Dim Sql As String
                Dim sWhere As String
                Dim oParam As Object
                Dim sUcols As String
                Dim sWCols As String
                Dim strTmp As String
                Dim i As Integer
                Dim j As Integer
                Dim sC As String

                serialParamName = serialColName

                For Each oParam In params
                    sC = oParam.SourceColumn

                    If Trim(sC) = "" Then
                        sC = oParam.ParameterName
                        oParam.SourceColumn = sC
                    End If
                    If Trim(sUcols) = "" Then
                        sUcols = sC
                    Else
                        sUcols = Trim(sUcols) & "," & sC
                    End If
                Next

                For Each oParam In params
                    If Trim(sWhere) = "" Then
                        sWhere = "?"
                    Else
                        sWhere = Trim(sWhere) & ",?"
                    End If


                    If hasBinaryFields Then
                        If oParam.DbType = DbType.Binary Or oParam.DbType = DbType.Byte Then
                            oParam.OleDbType = OleDb.OleDbType.LongVarBinary
                        Else
                            If Not oParam.Value Is Nothing Then
                                If oParam.Value.GetType.Equals(GetType(System.Byte())) Or _
                                   oParam.Value.GetType.Equals(GetType(System.Byte)) Then
                                    oParam.OleDbType = OleDb.OleDbType.LongVarBinary
                                End If
                            End If
                        End If
                    End If

                Next

                Sql = "Insert Into  " & tabName & _
                      " (  " & sUcols & ") Values " & _
                      " (  " & sWhere & ")"

                Return Sql

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <param name="wParams"></param>
        ''' <param name="sParams"></param>
        ''' <param name="allParams"></param>
        ''' <param name="hasBinaryFields"></param>
        ''' <returns></returns>
        Public Shared Function BuildUpdate(ByVal tabName As String, ByVal wParams As IDataParameterCollection, ByVal sParams As IDataParameterCollection, ByVal allParams As IDataParameterCollection, ByVal hasBinaryFields As Boolean) As String
            Try
                Dim Sql As String
                Dim sSet As String
                Dim sWhere As String
                Dim oParam As Object
                Dim sUcols As String
                Dim sWCols As String
                Dim strTmp As String
                Dim sC As String


                For Each oParam In allParams

                    If hasBinaryFields Then
                        If oParam.DbType = DbType.Binary Or oParam.DbType = DbType.Byte Then
                            oParam.OleDbType = OleDb.OleDbType.LongVarBinary
                        Else
                            If Not oParam.Value Is Nothing Then
                                If oParam.Value.GetType.Equals(GetType(System.Byte())) Or _
                                   oParam.Value.GetType.Equals(GetType(System.Byte)) Then
                                    oParam.OleDbType = OleDb.OleDbType.LongVarBinary
                                End If
                            End If
                        End If
                    End If

                    sC = oParam.SourceColumn
                    If Trim(sC) = "" Then
                        sC = oParam.ParameterName
                        oParam.SourceColumn = sC
                    End If
                    strTmp = Trim(sC) & "=?"

                    If sParams.Contains(oParam.ParameterName.ToString) Then

                        If Trim(sSet) = "" Then
                            sSet = Trim(strTmp)
                        Else
                            sSet = Trim(sSet) & "," & Trim(strTmp)
                        End If
                    End If

                    If wParams.Contains(oParam.ParameterName.ToString) Then
                        If Trim(sWhere) = "" Then
                            sWhere = Trim(strTmp)
                        Else
                            sWhere = Trim(sWhere) & " and " & Trim(strTmp)
                        End If
                    End If
                Next


                Sql = "Update  " & tabName & _
                      "   Set  " & sSet & _
                      "  Where " & sWhere

                Return Sql

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <param name="params"></param>
        ''' <returns></returns>
        Public Shared Function BuildDelete(ByVal tabName As String, ByVal params As IDataParameterCollection) As String
            Try
                Dim Sql As String
                Dim sSet As String
                Dim sWhere As String
                Dim oParam As Object
                Dim sUcols As String
                Dim sWCols As String
                Dim strTmp As String
                Dim sC As String


                For Each oParam In params
                    sC = oParam.SourceColumn
                    If Trim(sC) = "" Then
                        sC = oParam.ParameterName
                        oParam.SourceColumn = sC
                    End If

                    strTmp = Trim(sC) & "=?"

                    If Trim(sWhere) = "" Then
                        sWhere = Trim(strTmp)
                    Else
                        sWhere = Trim(sWhere) & " and " & Trim(strTmp)
                    End If
                Next


                Sql = "Delete From " & tabName & _
                      "  Where " & sWhere

                Return Sql

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tabName"></param>
        ''' <param name="params"></param>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        Public Shared Function BuildSelect(ByVal tabName As String, ByVal params As IDataParameterCollection, ByVal sql As String) As String
            Try
                Dim sSql As String
                Dim sSet As String
                Dim sWhere As String
                Dim oParam As Object
                Dim sUcols As String
                Dim sWCols As String
                Dim strTmp As String
                Dim sC As String

                If Trim(sql) = "" Then
                    sql = String.Concat("Select * from ", tabName)
                End If


                For Each oParam In params
                    sC = oParam.SourceColumn
                    If Trim(sC) = "" Then
                        sC = oParam.ParameterName
                        oParam.SourceColumn = sC
                    End If

                    strTmp = Trim(sC) & "=?"

                    If Trim(sWhere) = "" Then
                        sWhere = Trim(strTmp)
                    Else
                        sWhere = Trim(sWhere) & " and " & Trim(strTmp)
                    End If
                Next

                sSql = sql
                If Trim(sWhere) <> "" Then
                    If sql.ToLower.IndexOf("where") < 0 Then
                        sSql = String.Concat(sql, "  Where ", sWhere)
                    Else
                        sSql = String.Concat(sql, "  and ", sWhere)
                    End If
                End If

                Return sSql

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function


    End Class
End Namespace