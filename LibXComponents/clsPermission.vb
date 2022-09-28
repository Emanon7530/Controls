Public Class Permission
    Public Shared ID As Integer
    Public Shared ProgramName As String = ""
    Public Shared UserID As Integer
    Public Shared AllowAdd As Boolean = False
    Public Shared AllowEdit As Boolean = False
    Public Shared AllowDelete As Boolean = False
    Public Shared AllowQuery As Boolean = False
    Public Shared AllowPrint As Boolean = False

    Public Shared Function Load() As Boolean
        Dim SelectStmt As String
        Dim oRow As DataRow

        SelectStmt = "select * from scpermsm " & _
                     " where userid = " & ID.ToString & _
                     "   and progname = '" & ProgramName.Trim & "'"

        oRow = LibX.Data.Manager.GetDataRow(SelectStmt)

        If oRow Is Nothing Then
            Return False
        End If

        ID = Val(oRow!id.ToString.Trim)
        ProgramName = oRow!progname.ToString.Trim
        UserID = Val(oRow!userid.ToString.Trim)

        If LibX.IsNull(oRow!allowadd) OrElse oRow!allowadd.ToString.Trim = "0" Then
            AllowAdd = False
        Else
            AllowAdd = True
        End If

        If LibX.IsNull(oRow!allowedit) OrElse oRow!allowedit.ToString.Trim = "0" Then
            AllowEdit = False
        Else
            AllowEdit = True
        End If

        If LibX.IsNull(oRow!allowdelete) OrElse oRow!allowdelete.ToString.Trim = "0" Then
            AllowDelete = False
        Else
            AllowDelete = True
        End If

        If LibX.IsNull(oRow!allowquery) OrElse oRow!allowquery.ToString.Trim = "0" Then
            AllowQuery = False
        Else
            AllowQuery = True
        End If

        If LibX.IsNull(oRow!allowprint) OrElse oRow!allowprint.ToString.Trim = "0" Then
            AllowPrint = False
        Else
            AllowPrint = True
        End If

        Return True
    End Function
End Class
