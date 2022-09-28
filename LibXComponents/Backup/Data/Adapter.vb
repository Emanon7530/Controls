Namespace Data
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class Adapter
        Implements IDisposable

        Dim oADs As New Hashtable

        Public Event RowUpdating(ByVal sender As Object, ByVal e As AdpaterRowUpdatingEventArgs)
        Public Event RowUpdated(ByVal sender As Object, ByVal e As AdpaterRowUpdatedEventArgs)
        Private mSerialColName As String
        Private mTableInfo As LibXDbSourceTable
        Private mMasterRow As DataRow

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        Public Sub Save(ByVal table As DataTable)
            Try

                Save(table, "", Nothing, Nothing)

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="serialColName"></param>
        Public Sub Save(ByVal table As DataTable, ByVal serialColName As String)
            Try

                Save(table, serialColName, Nothing, Nothing)

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="serialColName"></param>
        ''' <param name="tableInfo"></param>
        Public Sub Save(ByVal table As DataTable, ByVal serialColName As String, ByVal tableInfo As LibXDbSourceTable)
            Try
                Save(table, serialColName, tableInfo, Nothing)
            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="serialColName"></param>
        ''' <param name="tableInfo"></param>
        ''' <param name="masterRow"></param>
        Public Sub Save(ByVal table As DataTable, ByVal serialColName As String, ByVal tableInfo As LibXDbSourceTable, ByVal masterRow As DataRow)
            Dim oA As OleDb.OleDbDataAdapter
            Dim oT As OleDb.OleDbDataAdapter
            Dim oB As OleDb.OleDbCommandBuilder
            Try
                mTableInfo = tableInfo
                mMasterRow = masterRow

                If Not oADs.Contains(table.TableName) Then
                    oT = New OleDb.OleDbDataAdapter
                    oA = New OleDb.OleDbDataAdapter
                    oADs.Add(table.TableName, oA)
                    oT.SelectCommand = New OleDb.OleDbCommand("Select * from " & table.TableName, LibX.Data.Manager.Connection.ConnectionObject)
                    oT.SelectCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction

                    oA.SelectCommand = New OleDb.OleDbCommand("Select * from " & table.TableName, LibX.Data.Manager.Connection.ConnectionObject)
                    oA.SelectCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction

                    oB = New OleDb.OleDbCommandBuilder(oT)

                    '-->Esto no es necesario, pero yo necesito que se queden en el adaptador
                    '   y que el commandbuilder no los vuelva a genear la proxima vez que se inserte
                    Dim Ci As OleDb.OleDbCommand '= oB.GetInsertCommand
                    Dim Cd As OleDb.OleDbCommand '= oB.GetDeleteCommand
                    Dim Cu As OleDb.OleDbCommand '= oB.GetUpdateCommand

                    Dim c As OleDb.OleDbCommand
                    Dim p2 As OleDb.OleDbParameter
                    Dim p As OleDb.OleDbParameter
                    Dim op As IDataParameter
                    Dim value As Object
                    Dim oTt As Type
                    Dim oDC As DataColumn


                    c = oB.GetInsertCommand
                    Ci = New OleDb.OleDbCommand(c.CommandText, c.Connection)
                    For Each p2 In c.Parameters
                        p = New OleDb.OleDbParameter
                        p.ParameterName = p2.ParameterName
                        p.SourceColumn = p2.SourceColumn
                        p.Direction = p2.Direction
                        p.OleDbType = p2.OleDbType
                        p.DbType = p2.DbType
                        p.Value = p2.Value
                        Ci.Parameters.Add(p)

                        op = p
                        oDC = table.Columns(op.SourceColumn)
                        If Not oDC Is Nothing Then
                            oTt = oDC.DataType
                            value = p.Value
                            If oTt.Equals(GetType(Decimal)) Then
                                op.DbType = DbType.Decimal
                            ElseIf oTt.Equals(GetType(Int16)) Then
                                op.DbType = DbType.Int16
                            ElseIf oTt.Equals(GetType(Int32)) Then
                                op.DbType = DbType.Int32
                            ElseIf oTt.Equals(GetType(Int64)) Then
                                op.DbType = DbType.Int64
                            ElseIf oTt.Equals(GetType(Date)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(DateTime)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(Double)) Then
                                op.DbType = DbType.Double
                            End If

                        End If
                    Next

                    c = oB.GetDeleteCommand
                    Cd = New OleDb.OleDbCommand(c.CommandText, c.Connection)
                    For Each p2 In c.Parameters
                        p = New OleDb.OleDbParameter
                        p.ParameterName = p2.ParameterName
                        p.SourceColumn = p2.SourceColumn
                        p.Direction = p2.Direction
                        p.OleDbType = p2.OleDbType
                        p.DbType = p2.DbType
                        p.Value = p2.Value
                        Cd.Parameters.Add(p)

                        op = p
                        oDC = table.Columns(op.SourceColumn)
                        If Not oDC Is Nothing Then
                            oTt = oDC.DataType
                            value = p.Value
                            If oTt.Equals(GetType(Decimal)) Then
                                op.DbType = DbType.Decimal
                            ElseIf oTt.Equals(GetType(Int16)) Then
                                op.DbType = DbType.Int16
                            ElseIf oTt.Equals(GetType(Int32)) Then
                                op.DbType = DbType.Int32
                            ElseIf oTt.Equals(GetType(Int64)) Then
                                op.DbType = DbType.Int64
                            ElseIf oTt.Equals(GetType(Date)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(DateTime)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(Double)) Then
                                op.DbType = DbType.Double
                            End If
                        End If
                    Next

                    '-->c = oB.GetUpdateCommand
                    Dim oDt As DataTable = LibX.Data.Manager.GetNewDataTable(table.TableName)
                    C = BUpCmd(oDt, table.TableName)

                    Cu = New OleDb.OleDbCommand(c.CommandText, c.Connection)
                     For Each p2 In c.Parameters
                        p = New OleDb.OleDbParameter
                        p.ParameterName = p2.ParameterName
                        p.SourceColumn = p2.SourceColumn
                        p.Direction = p2.Direction
                        p.OleDbType = p2.OleDbType
                        p.DbType = p2.DbType
                        p.Value = p2.Value
                        Cu.Parameters.Add(p)
                        op = p
                        oDC = table.Columns(op.SourceColumn)
                        If Not oDC Is Nothing Then
                            oTt = oDC.DataType
                            If oTt.Equals(GetType(Decimal)) Then
                                op.DbType = DbType.Decimal
                            End If
                            If oTt.Equals(GetType(Int16)) Then
                                op.DbType = DbType.Int16
                            ElseIf oTt.Equals(GetType(Int32)) Then
                                op.DbType = DbType.Int32
                            ElseIf oTt.Equals(GetType(Int64)) Then
                                op.DbType = DbType.Int64
                            ElseIf oTt.Equals(GetType(Date)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(DateTime)) Then
                                op.DbType = DbType.Date
                            ElseIf oTt.Equals(GetType(Double)) Then
                                op.DbType = DbType.Double
                            End If
                        End If

                    Next

                    oA.InsertCommand = Ci
                    oA.DeleteCommand = Cd
                    oA.UpdateCommand = Cu

                    If Trim(serialColName) <> "" Then
                        mSerialColName = serialColName
                        Dim oCmd As OleDb.OleDbCommand
                        oCmd = oA.InsertCommand
                        oCmd.CommandText = String.Concat(oCmd.CommandText, ";", "SELECT SCOPE_IDENTITY() AS ", serialColName)
                        oCmd.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord
                    End If

                    oB.Dispose()
                    oB = Nothing
                Else
                    oA = oADs(table.TableName)
                End If

                If Not oA.DeleteCommand Is Nothing Then
                    oA.DeleteCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction
                End If
                If Not oA.InsertCommand Is Nothing Then
                    oA.InsertCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction
                End If
                If Not oA.UpdateCommand Is Nothing Then
                    oA.UpdateCommand.Transaction = LibX.Data.Manager.Connection.ActiveTransaction
                End If

                RemoveHandler oA.RowUpdating, AddressOf OnRowUpdating
                AddHandler oA.RowUpdating, AddressOf OnRowUpdating

                RemoveHandler oA.RowUpdated, AddressOf OnRowUpdated
                AddHandler oA.RowUpdated, AddressOf OnRowUpdated

                oA.Update(table)

            Catch ex As Exception
                Log.Add(ex)
            End Try

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub OnRowUpdated(ByVal sender As Object, ByVal e As OleDb.OleDbRowUpdatedEventArgs)
            Dim oArgs As New AdpaterRowUpdatedEventArgs
            Try
                oArgs.UpdatingArgs = e
                oArgs.TableInfo = mTableInfo

                If Not (e.Status = UpdateStatus.SkipCurrentRow Or _
                        e.Status = UpdateStatus.ErrorsOccurred Or _
                        e.Status = UpdateStatus.SkipAllRemainingRows) Then
                    If Trim(mSerialColName) <> "" And e.StatementType = StatementType.Insert Then
                        oArgs.Serial = e.Row(mSerialColName)
                        LibX.Data.Manager.LastSerialValue = oArgs.Serial
                    End If

                End If

                RaiseEvent RowUpdated(sender, oArgs)


            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub OnRowUpdating(ByVal sender As Object, ByVal e As OleDb.OleDbRowUpdatingEventArgs)
            Dim oArgs As New AdpaterRowUpdatingEventArgs
            Try
                oArgs.UpdatingArgs = e
                oArgs.TableInfo = mTableInfo
                RaiseEvent RowUpdating(sender, oArgs)

                If oArgs.Handled Then
                    e.Status = UpdateStatus.Continue
                    Exit Sub
                End If

                If Not mTableInfo Is Nothing Then
                    If Not mMasterRow Is Nothing And Not mTableInfo.MasterDetailRelation Is Nothing And e.StatementType <> StatementType.Delete Then
                        Dim s As String
                        Dim f() As String
                        For Each s In mTableInfo.MasterDetailRelation
                            If Trim(s) <> "" Then
                                f = Split(s, "=")
                                e.Row(f(0)) = mMasterRow(f(1))
                                Dim oP As IDataParameter
                                For Each oP In e.Command.Parameters
                                    If Trim(oP.SourceColumn).ToLower = Trim(f(0)).ToLower Then
                                        oP.Value = e.Row(f(0))
                                    End If
                                Next

                            End If
                        Next
                    End If
                End If

                If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Or e.StatementType = StatementType.Update Then
                    Dim oP As IDataParameter
                    For Each oP In e.Command.Parameters
                        If TypeOf oP.Value Is String Then
                            oP.Value = e.Row(op.SourceColumn).tostring.Trim
                        End If
                        op.Value = e.Row(op.SourceColumn)

                        If Trim(op.SourceColumn).ToLower = "userid" Then
                            If IsNull(op.Value) And Trim(User.UserID) <> "" Then
                                op.Value = Trim(User.UserID)
                            End If
                        End If

                        If Trim(op.SourceColumn).ToLower = "username" Then
                            If IsNull(op.Value) And Trim(User.UserName) <> "" Then
                                op.Value = Trim(User.UserName)
                            End If
                        End If
                    Next
                End If

                'If  e.Status = UpdateStatus.Continue Or e.Status = UpdateStatus.SkipAllRemainingRows And Not e.Command Is Nothing Then
                '    Dim oP As IDataParameter
                '    For Each oP In e.Command.Parameters
                '        Try
                '            oP.Value = e.Row(oP.SourceColumn)
                '            If TypeOf oP.Value Is String Then
                '                oP.Value = Trim(oP.Value)
                '            End If
                '        Catch ex As Exception
                '        End Try
                '    Next
                'End If



            Catch ex As Exception
                Log.Add(ex)
            End Try
            '''

        End Sub

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not oADs Is Nothing Then
                Dim ad As OleDb.OleDbDataAdapter
                For Each ad In oADs
                    If Not ad.SelectCommand Is Nothing Then
                        ad.Dispose()
                    End If
                    If Not ad.InsertCommand Is Nothing Then
                        ad.Dispose()
                    End If
                    If Not ad.DeleteCommand Is Nothing Then
                        ad.Dispose()
                    End If
                    If Not ad.UpdateCommand Is Nothing Then
                        ad.Dispose()
                    End If
                Next
            End If

            '''

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p_objDataObject"></param>
        ''' <param name="p_strTable"></param>
        ''' <returns></returns>
        Private Function BUpCmd(ByVal p_objDataObject As Object, ByVal p_strTable As String) As IDbCommand
            Dim objCommand As IDbCommand
            Dim objColumn As DataColumn
            Dim strCommand As String
            Dim strTmp As String
            Dim i As Integer
            Dim strCol As String
            Dim strWherePk As String
            Dim objTable As Object
            Dim objParameter As Object
            Dim objExceptionColumnsPk As Array
            Dim objExceptionColumns() As String = {""}
            Dim objIncludedColumns() As String
            Dim intLast As Integer
            Dim blnHasCols As Boolean = False

            Try
                If TypeOf p_objDataObject Is DataSet Then
                    If Trim(p_strTable) = "" Then
                        objTable = p_objDataObject.tables(0)
                    Else
                        objTable = p_objDataObject.tables(p_strTable)
                    End If
                Else
                    objTable = p_objDataObject
                End If

                If objIncludedColumns Is Nothing Then
                    Dim objArray(objTable.Columns.Count - 1) As String
                    i = 0
                    For Each objColumn In objTable.Columns
                        objArray(i) = UCase(objColumn.ColumnName)
                        i = i + 1
                    Next
                    objIncludedColumns = objArray
                End If

                strCommand = "UPDATE " & objTable.TableName & " SET "

                objCommand = LibX.Data.Manager.Connection.CreateCommand

                strWherePk = Me.buildParamStrWherePk(objTable, objExceptionColumnsPk, False)

                If Not objExceptionColumnsPk Is Nothing Then
                    Dim objExceptionColumnsW(objExceptionColumnsPk.Length + objExceptionColumns.Length) As String
                    Dim n As Integer = 0
                    For i = 0 To objExceptionColumnsPk.Length - 1
                        objExceptionColumnsW(n) = LCase(objExceptionColumnsPk(i))
                        n = n + 1
                    Next

                    For i = 0 To objExceptionColumns.Length - 1
                        objExceptionColumnsW(n) = LCase(objExceptionColumns(i))
                        n = n + 1
                    Next
                    objExceptionColumns = objExceptionColumnsW
                End If


                i = 0
                For Each strCol In objIncludedColumns
                    If strCol.Trim <> "" Then
                        objColumn = objTable.Columns(strCol)
                        If Array.IndexOf(objExceptionColumns, LCase(objColumn.ColumnName)) < 0 Then
                            If i > 0 Then
                                strCommand = Trim(strCommand) & ","
                            End If
                            strTmp = Trim(objColumn.ColumnName) & "=?"
                            strCommand = Trim(strCommand) & " " & Trim(strTmp)



                            objParameter = objCommand.CreateParameter
                            'objParameter.DbType 
                            objParameter.ParameterName = "p" & Trim(CStr(i))
                            objParameter.SourceColumn = objColumn.ColumnName

                            If objColumn.DataType.Equals(GetType(System.Byte())) Or _
                               objColumn.DataType.Equals(GetType(System.Byte)) Then
                                '-->objParameter.DbType = DbType.Binary
                                objParameter.OleDbType = OleDb.OleDbType.LongVarBinary
                            End If

                            objCommand.Parameters.Add(objParameter)
                            i = i + 1
                            intLast = i
                            blnHasCols = True
                        End If
                    End If
                Next


                If Not blnHasCols Then
                    Return Nothing
                End If

                '* --> strWherePk = Me.buildParamStrWherePk(objTable)
                If Trim(strWherePk) <> "" Then
                    strCommand = Trim(strCommand) & " Where " & Trim(strWherePk)
                    Me.buildParamsPk(objTable, objCommand, intLast)
                End If


                objCommand.CommandText = strCommand

                'Dim op As IDbDataParameter
                'Dim value As Object
                'For Each op In objCommand.Parameters
                '    value = op.Value
                '    If Not IsNull(value) Then
                '        If value.GetType.Equals(GetType(Decimal)) Then
                '            op.DbType = DbType.Decimal
                '        End If
                '        If value.GetType.Equals(GetType(Int16)) Then
                '            op.DbType = DbType.Int16
                '        End If
                '        If value.GetType.Equals(GetType(Int32)) Then
                '            op.DbType = DbType.Int32
                '        End If
                '        If value.GetType.Equals(GetType(Int64)) Then
                '            op.DbType = DbType.Int64
                '        End If
                '        If value.GetType.Equals(GetType(Date)) Then
                '            op.DbType = DbType.Date
                '        End If
                '        If value.GetType.Equals(GetType(DateTime)) Then
                '            op.DbType = DbType.Date
                '        End If
                '        If value.GetType.Equals(GetType(Double)) Then
                '            op.DbType = DbType.Double
                '        End If

                '    End If
                'Next

                Return objCommand

            Catch ex As Exception
                Log.Add(ex)
            End Try
        End Function

        Private Sub buildParamsPk(ByVal p_objTable As Object, ByVal p_objCommand As Object, ByVal p_intLast As Integer)
            Dim objColumn As DataColumn
            Dim strCommand As String
            Dim strTmp As String
            Dim objParameter As IDbDataParameter
            Dim i As Integer


            i = 0
            strCommand = ""
            For Each objColumn In p_objTable.PrimaryKey
                If i > 0 Then
                    strCommand = Trim(strCommand) & " And "
                End If
                strTmp = Trim(objColumn.ColumnName) & "=?"
                strCommand = Trim(strCommand) & " " & Trim(strTmp)

                objParameter = p_objCommand.CreateParameter
                'objParameter.DbType 
                objParameter.ParameterName = "p" & Trim(CStr(p_intLast))
                objParameter.SourceColumn = objColumn.ColumnName
                p_objCommand.Parameters.Add(objParameter)
                p_intLast = p_intLast + 1

                i = i + 1
            Next

            '''

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p_objTable"></param>
        ''' <param name="p_objExceptionArr"></param>
        ''' <param name="joinDefaultWhere"></param>
        ''' <returns></returns>
        Private Function buildParamStrWherePk(ByVal p_objTable As Object, ByRef p_objExceptionArr As Array, ByVal joinDefaultWhere As Boolean) As String
            Dim objColumn As DataColumn
            Dim strCommand As String
            Dim strTmp As String
            Dim i As Integer
            Dim strWherePk As String


            If p_objTable.PrimaryKey Is Nothing Then
                Exit Function
            End If
            If p_objTable.PrimaryKey.Length = 0 Then
                '''

                Exit Function
            End If
            Dim objArray(p_objTable.PrimaryKey.Length-1)


            i = 0
            strCommand = ""
            For Each objColumn In p_objTable.PrimaryKey
                If i > 0 Then
                    strCommand = Trim(strCommand) & " And "
                End If
                strTmp = Trim(objColumn.ColumnName) & "=?"
                strCommand = Trim(strCommand) & " " & Trim(strTmp)

                objArray(i) = Trim(objColumn.ColumnName)

                i = i + 1
            Next


            'If Not m_blnUpdatePkCols Then
            p_objExceptionArr = objArray
            'End If

            Return strCommand

        End Function


    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class AdpaterRowUpdatingEventArgs
        Public UpdatingArgs As OleDb.OleDbRowUpdatingEventArgs
        Public TableInfo As LibXDbSourceTable
        Public Handled As Boolean
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class AdpaterRowUpdatedEventArgs
        Public UpdatingArgs As OleDb.OleDbRowUpdatedEventArgs
        Public TableInfo As LibXDbSourceTable
        Public Serial As Integer
    End Class

End Namespace
