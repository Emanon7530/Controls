
Public Module MdlUtil

    Private mHasOpenedEngine As Boolean
    Private mUseEditingColors As String



    Public Property hasOpenedEngine() As Boolean
        Get
            Return mHasOpenedEngine
        End Get
        Set(ByVal Value As Boolean)
            mHasOpenedEngine = Value
        End Set
    End Property

    Public Function GetAppPath() As String
        Return System.AppDomain.CurrentDomain.BaseDirectory()
    End Function

    Friend Function GetVendorName() As String
        Dim Nombre As String

        Try

            Nombre = LibX.Data.Manager.GetScalar("select vend_name from ftvendm where vend_code = " & User.VendedorID.ToString)

            Return Nombre
        Catch ex As Exception
            Log.Add(ex, True)
        End Try
    End Function

    Public Function IsNull(ByVal value As Object) As Boolean
        Dim objType As Type
        Dim strTmp As String

        Try
            If value Is Nothing Then
                Return True
            End If

            If IsDBNull(value) Then
                Return True
            End If

            objType = value.GetType()
            strTmp = value.ToString.Trim

            If objType Is Type.GetType("System.String") Or _
               objType Is Type.GetType("System.Date") Then

                If strTmp.Length = 0 Then
                    Return True
                End If

                Return False

            End If

            If strTmp.Length = 0 Or strTmp = "-Infinity" Or strTmp = "Infinity" Then
                Return True
            End If

            If value = Nothing Then
                If IsNumeric(value) Then
                    If value = 0 Then
                        Return False
                    End If
                End If
                Return True
            End If

            Return False
        Catch
            Return True
        End Try

    End Function

    Public Function GetReportPath(ByVal sMod As String, ByVal sname As String) As String
        Try
            Dim s As String
            Dim apppath As String

            s = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXReportPth")
            apppath = System.Configuration.ConfigurationSettings.AppSettings.Get("apppath")

            Dim oA() As String = New String() {apppath, sMod, sname}

            If Trim(sMod) <> "" Then
                s = String.Format(s, oA)
            End If

            Return s
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Property UseEditingColors() As Boolean
        Get
            If Trim(mUseEditingColors) = "" Then
                mUseEditingColors = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXUseEditingColors")
                If Trim(mUseEditingColors) = "" Then
                    mUseEditingColors = "false"
                End If
            End If
            Return CBool(mUseEditingColors)
        End Get
        Set(ByVal Value As Boolean)
            mUseEditingColors = Value.ToString
        End Set
    End Property


    Public Function ConcatWherePart(ByVal p_strSQL As String, ByVal p_strWhere As String) As String
        Dim strSQL As String
        Dim intPos As Integer
        Dim strWhere As String
        Try
            If Trim(p_strWhere) = "" Then
                Return p_strSQL
            End If
            strWhere = getWherePart(p_strSQL)
            If Trim(strWhere) <> "" Then
                strWhere = Trim(strWhere) & " and " & p_strWhere
            Else
                strWhere = p_strWhere
            End If

            strSQL = ReplaceWherePart(p_strSQL, strWhere)

            Return strSQL
        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Function

    Public Function ReplaceWherePart(ByVal p_strSql As String, ByVal p_strWherePart As String) As String
        Dim strTmp As String
        Dim strSQL As String
        Dim intPos As Integer
        Dim intPosWhere As Integer
        Dim intPos2 As Integer
        Dim intPos3 As Integer
        Dim intLast As Integer
        Dim strSel As String
        Dim strAd As String
        Dim strjoin As String

        Try
            If Trim(p_strWherePart) = "" Then
                Return p_strSql
            End If
            strSQL = p_strSql
            intPosWhere = InStr(LCase(strSQL), "where")
            If intPosWhere > 0 Then
                strSel = Mid(strSQL, 1, intPosWhere - 1)
                strSQL = strSQL.Remove(0, intPosWhere - 1)
            Else
                strSel = strSQL
            End If
            intPos = InStr(LCase(strSQL), "order by")
            intPos2 = InStr(LCase(strSQL), "group by")
            intPos3 = InStr(LCase(strSQL), " having ")
            intLast = 0
            If intPos > 0 Then
                intLast = intPos
            End If
            If intPos2 > intLast Then
                intLast = intPos2
            End If
            If intPos3 > intLast Then
                intLast = intPos3
            End If
            strAd = ""
            If intLast > 0 Then
                strAd = strSQL.Remove(0, intLast - 1)
            End If

            strjoin = " where "
            If InStr(LCase(p_strWherePart), "where") > 0 Then
                strjoin = " "
            End If
            If intPosWhere = 0 Then
                If intLast = 0 Then
                    intLast = Len(strSQL) + 1
                End If
                strSel = Mid(strSQL, 1, intLast - 1)
            End If

            strSQL = strSel & strjoin & p_strWherePart & " " & strAd

            Return strSQL
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function


    Public Function getWherePart(ByVal p_strSQL As String) As String
        Dim strTmp As String
        Dim strSQL As String
        Dim intPos As Integer
        Dim intPos2 As Integer
        Dim intPos3 As Integer
        Dim intLast As Integer
        Dim strSel As String
        Dim strAd As String

        Try
            strSQL = p_strSQL
            If InStr(LCase(strSQL), "where") = 0 Then
                Return ""
            End If
            intPos = InStr(LCase(strSQL), "where")
            If intPos > 0 Then
                strSel = Mid(strSQL, 1, intPos)
                strSQL = strSQL.Remove(0, intPos - 1)
            Else
                strSel = strSQL
            End If
            intPos = InStr(LCase(strSQL), "order by")
            intPos2 = InStr(LCase(strSQL), "group by")
            intPos3 = InStr(LCase(strSQL), " having ")
            intLast = 0
            If intPos > 0 Then
                intLast = intPos
            End If
            If intPos2 > intLast Then
                intLast = intPos2
            End If
            If intPos3 > intLast Then
                intLast = intPos3
            End If
            If intLast = 0 Then
                intLast = strSQL.Length
            Else
                intLast = intLast - 1
            End If
            strAd = ""

            If intLast > 0 Then
                strAd = Mid(strSQL, 1, intLast)
                strAd = Mid(strAd, 7, Len(strAd))
            End If

            strSQL = strAd

            Return strSQL
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function


    Public Function GetFullColumnName(ByVal FieldName As String, ByVal p_strSQL As String, ByVal p_strMainTable As String) As String
        Dim pos As Integer, S As String, i As Integer, rs As String, IsCharacter As Boolean
        Dim fpos As Integer
        Dim rs0 As String

        p_strSQL = p_strSQL.Replace(ControlChars.NewLine, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.FormFeed, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.Cr, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.CrLf, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.Tab, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.Lf, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.NullChar, " ")
        p_strSQL = p_strSQL.Replace(ControlChars.VerticalTab, " ")

        '-->Si se encuentra un OUTER sacar dividir hasta el From
        '-->El getFullColumnName falla si el campo es econtrado en un LEFT OUTER JOIN. Lo mismo puede pasar con un Where.
        '   El problema es que como al principio se busca ".campo, si ese criterio lo encentra en el where o outer porque está tablax.campo retorna ese, 
        '   pero debe ser solo lo del fromPart.
        If p_strSQL.ToLower.IndexOf(" outer ") >= 0 Or p_strSQL.ToLower.IndexOf(" where ") Then
            If p_strSQL.ToLower.IndexOf(" from ") >= 0 Then
                p_strSQL = p_strSQL.Substring(0, p_strSQL.ToLower.IndexOf(" from "))
            End If
        End If

        pos = InStr(LCase(p_strSQL), "." & LCase(FieldName))
        S = p_strSQL

        If pos > 0 Then
            fpos = pos
            For i = pos To 1 Step -1
                If Mid(S, i, 1) = "," Or Mid(S, i, 1) = " " Or Mid(S, i, 1) = Chr(13) Then
                    rs = Mid(S, i + 1, pos - i)
                    Exit For
                ElseIf i = 1 Then
                    rs = Mid(S, 1, pos)
                    Exit For
                End If
            Next i
            rs = rs & FieldName
            rs0 = rs

            '--> Si existe otra combinacion igual luego de donde se econtro esta hay que buscar
            '    exactamente el campo que es.
            '    digamos " eprecom.forma1, eprecom.forma2,eprecod.forma
            '    tomaria eprecom.forma, y ese campo no existe
            pos = InStr(fpos + FieldName.Length, LCase(p_strSQL), "." & LCase(FieldName))
            If pos > 0 Then
                pos = InStr(LCase(p_strSQL), "." & LCase(FieldName) & ",")
                If pos = 0 Then
                    pos = InStr(LCase(p_strSQL), "." & LCase(FieldName) & " ")
                End If
                If pos = 0 Then
                    pos = InStr(LCase(p_strSQL), "." & LCase(FieldName) & Chr(13))
                End If
                rs = ""

                For i = pos To 1 Step -1
                    If Mid(S, i, 1) = "," Or Mid(S, i, 1) = " " Or Mid(S, i, 1) = Chr(13) Then
                        rs = Mid(S, i + 1, pos - i)
                        Exit For
                    ElseIf i = 1 Then
                        rs = Mid(S, 1, pos)
                        Exit For
                    End If
                Next i
                rs = rs & FieldName
            End If

        Else
            '--> pos = InStr(LCase(p_strSQL), " as " & LCase(FieldName))
            Dim tmps As String
            Dim tmps2 As String
            Dim pos2 As Integer
            Dim strSql As String

            strSql = p_strSQL

            pos = InStr(LCase(strSql), LCase(FieldName))
            If pos = 0 Then
                '--> el mantenimiento solo tiene el updatetablename
                rs = Trim(p_strMainTable) & "." & FieldName
            Else
                tmps = strSql.Remove(pos - 1, strSql.Length - (pos - 1))
                If Trim(tmps.ToLower).EndsWith(" as") Then

                    'cuando se agregan columnas sinónimas estas deben ser unicas
                    'por lo que no se agrega el tablename. Ademas al ser ficticias 
                    'no pertenecen a esta tablename

                    'rs = FieldName


                    '--> Tengo " tabla.col As micol"

                    pos2 = tmps.ToLower.LastIndexOf("as")
                    tmps2 = tmps.Remove(pos2 - 1, tmps.Length - pos2)
                    pos2 = Trim(tmps2.LastIndexOf(","))
                    If pos2 = -1 Then
                        pos2 = Trim(tmps2).LastIndexOf(" ")
                    End If
                    tmps = tmps2.Remove(0, pos2 + 1)
                    rs = tmps
                    '-->End If
                    '-->If pos > 0 Then

                    '    For i = pos To 1 Step -1
                    '        If Mid(S, i, 1) <> " " Then IsCharacter = True
                    '        If IsCharacter And Mid(S, i, 1) = " " Then
                    '            rs = Mid(S, i + 1, pos - i)
                    '            Exit For
                    '        End If
                    '    Next i
                Else
                    rs = Trim(p_strMainTable) & "." & FieldName
                End If '-->If Trim(tmps.ToLower).EndsWith(" as") Then
            End If '-->If pos = 0 Then
        End If
        GetFullColumnName = rs
    End Function


    Public Function GetSqlWhere(ByVal row As DataRow, ByVal sql As String, ByVal tabName As String) As String
        Try
            Dim oC As DataColumn
            Dim oV As Object
            Dim sWhere As String
            Dim sW As String
            Dim sFCol As String
            For Each oC In row.Table.Columns
                oV = row(oC.ColumnName)

                sW = ""
                If Not IsNull(oV) And oC.ReadOnly = False Then
                    sFCol = GetFullColumnName(oC.ColumnName, sql, tabName)

                    sW = BuildCondition(sFCol, oC.DataType, oV, False)

                End If

                If Trim(sW) <> "" Then
                    If Trim(sWhere) = "" Then
                        sWhere = sW
                    Else
                        sWhere = String.Concat(sWhere, " and ", sW)
                    End If
                End If
            Next

            Return sWhere
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Function GetSqlWhereByPk(ByVal oRow As DataRow, ByVal sql As String) As String
        Try
            Dim oC As DataColumn
            Dim oV As Object
            Dim sWhere As String
            Dim sW As String
            Dim sFCol As String

            If oRow.Table.PrimaryKey Is Nothing Then
                Exit Function
            End If

            If oRow.Table.PrimaryKey.Length <= 0 Then
                Exit Function
            End If

            For Each oC In orow.Table.PrimaryKey
                oV = oRow(oC.ColumnName)

                sW = ""
                If Not IsNull(oV) Then
                    sFCol = GetFullColumnName(oC.ColumnName, sql, oRow.Table.TableName)

                    sW = BuildCondition(sFCol, oC.DataType, oV, False)
                End If

                If Trim(sW) <> "" Then
                    If Trim(sWhere) = "" Then
                        sWhere = sW
                    Else
                        sWhere = String.Concat(sWhere, " and ", sW)
                    End If
                End If
            Next

            Return sWhere
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Function BuildCondition(ByVal colName As String, ByVal typ As Type, ByVal value As Object, ByVal iqualstr As Boolean) As String
        Try
            Dim sW As String

            If typ.Equals(GetType(String)) Then
                If iqualstr Then
                    sW = String.Concat(colName, " = '", Trim(value), "'")
                Else
                    sW = String.Concat(colName, " like '", Trim(value).Replace(" ", "%"), "%'")
                End If
            End If
            If typ.Equals(GetType(Date)) Then
                sW = String.Concat(colName, " = '", CDate(value).ToString(LibX.Data.Manager.GetSaveDateFormat), "'")
            End If
            If typ.Equals(GetType(Decimal)) Then
                If IsNumeric(value) = False Then
                    sW = "1=2"
                Else
                    sW = String.Concat(colName, " = ", Trim(value))
                End If
            End If
            If typ.Equals(GetType(Int32)) Or typ.Equals(GetType(Int16)) Then
                If IsNumeric(value) = False Then
                    sW = "1=2"
                Else
                    sW = String.Concat(colName, " = ", Trim(value))
                End If
            End If

            Return sW
        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Function buildParamStrWherePk(ByVal p_objTable As Object, ByRef p_objExceptionArr As Array, ByVal joinDefaultWhere As Boolean) As String
        Dim objColumn As DataColumn
        Dim strCommand As String
        Dim strTmp As String
        Dim i As Integer
        Dim strWherePk As String


        If p_objTable.PrimaryKey Is Nothing Then
            Exit Function
        End If
        If p_objTable.PrimaryKey.Length = 0 Then
            Exit Function
        End If

        Dim objArray(p_objTable.PrimaryKey.Length - 1)

        i = 0
        strCommand = ""

        For Each objColumn In p_objTable.PrimaryKey

            If strCommand.Trim = "" Then
                strCommand = ""
            End If
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

End Module
