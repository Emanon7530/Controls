Public Enum LogWay
    none = 0
    EventLog = 1
    File = 2
End Enum
Public Enum EnumErrorLevel
    [Error]
    Warning
    Information
End Enum

Public Class LibXException
    Inherits ApplicationException

    Dim _ErrorLevel As EnumErrorLevel

    Public Property Errorlevel() As EnumErrorLevel
        Get
            Return _ErrorLevel
        End Get
        Set(ByVal Value As EnumErrorLevel)
            _ErrorLevel = Value
        End Set
    End Property

    Public Sub New(ByVal message As String, ByVal errorlevel As EnumErrorLevel)
        MyBase.New(message)

        _ErrorLevel = errorlevel

    End Sub
End Class

Public Class Log
    Private Shared mUseConnection As Boolean = True
    Private Shared mLogFile As String
    Private Shared mLogWay As LogWay
    Private Shared mWriter As System.IO.StreamWriter
    Private Shared mLog As EventLog

    Private Shared Sub GetLogWay()
        Try
            Dim s2 As String
            s2 = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXLogType")
            If Trim(s2) = "" Then
                s2 = "2"
            End If

            mLogWay = LogWay.none
            If Trim(s2) = "1" Then
                mLogWay = LogWay.File
            ElseIf Trim(s2) = "2" Then
                mLogWay = LogWay.EventLog
            End If

            If mLogWay = LogWay.none Then
                mLogFile = "none"

            ElseIf mLogWay = LogWay.EventLog Then
                mLogFile = "none"
                mLog = New EventLog("LibxErrors")
                mLog.Source = "SrcLibx"

            ElseIf mLogWay = LogWay.File Then
                Dim s As String
                s = System.Configuration.ConfigurationSettings.AppSettings.Get("apppath")
                If Not Trim(s).EndsWith("\") Then
                    s = String.Concat(s, "\")
                End If

                s = String.Concat(s, "log")
                If Not System.IO.Directory.Exists(s) Then
                    System.IO.Directory.CreateDirectory(s)
                End If

                Dim sDate As String = String.Concat("F", Now.ToString("dd-MM-yyyy"), ".txt")
                s = String.Concat(s, "\", sDate)
                mLogFile = s

                mWriter = New System.IO.StreamWriter(New System.IO.FileStream(s, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite))
                mWriter.AutoFlush = True
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Shared Sub SaveToLog(ByVal ex As Exception)
        Try
            If Trim(mLogFile) = "" Then
                GetLogWay()
            End If

            Dim s As String
            s = String.Concat("**********************", vbCrLf, ex.ToString, vbCrLf)

            If Not mWriter Is Nothing Then
                mWriter.WriteLine(s)
            End If

            If Not mLog Is Nothing Then
                mLog.WriteEntry(s)
            End If

        Catch oEx As Exception
            MsgBox(oEx.ToString)
        End Try
    End Sub


    <System.Diagnostics.DebuggerStepThrough()> _
    Public Shared Sub Add(ByVal ex As Exception)
        Dim f As New fmError

        If App.ExecuteExit Then
            Exit Sub
        End If

        '// ERROR DE ACCESO A BASE DE DATOS, NO BUSCAR DESCRIPCION DE ERROR
        If TypeOf ex Is OleDb.OleDbException Then
            If CType(ex, OleDb.OleDbException).ErrorCode = -2147217843 _
            Or CType(ex, OleDb.OleDbException).ErrorCode = -2147467259 Then '// SQL Server No exist or access denied
                GoTo showException
            End If
        End If

        If LibX.Data.Manager.Connection.AutoRollBackWhenError Then
            LibX.Data.Manager.Connection.RollBackTransaction()
        End If

        Dim doShow As Boolean
        If ex.InnerException Is Nothing Then
            doShow = True
        End If
        If TypeOf ex Is OleDb.OleDbException Then
            Dim oDx As Exception
            Dim s As String
            s = GetDataException(ex)
            oDx = New Exception(s, ex)
            ex = oDx
        End If

showException:
        If Not TypeOf ex Is LibXException OrElse CType(ex, LibXException).Errorlevel = EnumErrorLevel.Error Then
            SaveToLog(ex)
        End If

        If doShow Then
            '-->MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error)
            f.mEx = ex
            f.ShowDialog()
        End If

        Dim oEx As New Exception(ex.Message, ex)

        Throw oEx

    End Sub

    Private Shared Function GetDataException(ByVal ex As Exception) As String
        Dim ox As OleDb.OleDbException = ex
        Dim oE As OleDb.OleDbError
        Dim s As String
        Dim se As String
        Dim j As Integer
        Dim i As Integer
        Dim eNumber As String
        Dim sComment As String


        For i = ox.Errors.Count - 1 To 0 Step -1
            oE = ox.Errors.Item(i)
            se = ""
            If oE.NativeError <> 0 Then
                eNumber = oE.NativeError
            End If

            sComment = LibX.Data.Manager.GetMessageWithNoFound(eNumber, "", Nothing)
            If Trim(sComment) = "" Then
                sComment = oE.Message
            End If

            sComment = String.Concat("(", oE.NativeError, ") ", sComment)

            Return sComment
        Next


    End Function

    Public Shared Property UseConnection() As Boolean
        Get
            Return mUseConnection
        End Get
        Set(ByVal Value As Boolean)
            mUseConnection = Value
        End Set
    End Property


    <System.Diagnostics.DebuggerStepThrough()> _
    Public Shared Sub Add(ByVal ex As Exception, ByVal shown As Boolean)
        Dim f As New fmError

        If App.ExecuteExit Then
            Exit Sub
        End If

        '// ERROR DE ACCESO A BASE DE DATOS, NO BUSCAR DESCRIPCION DE ERROR
        If TypeOf ex Is OleDb.OleDbException Then
            If CType(ex, OleDb.OleDbException).ErrorCode = -2147217843 _
            Or CType(ex, OleDb.OleDbException).ErrorCode = -2147467259 Then '// SQL Server No exist or access denied
                GoTo showException
            End If
        End If

        If LibX.Data.Manager.Connection.AutoRollBackWhenError Then
            LibX.Data.Manager.Connection.RollBackTransaction()
        End If

        If TypeOf ex Is OleDb.OleDbException Then
            Dim oDx As Exception
            Dim s As String
            s = GetDataException(ex)
            oDx = New Exception(s, ex)
            ex = oDx
        End If

showException:
        If Not TypeOf ex Is LibXException OrElse CType(ex, LibXException).Errorlevel = EnumErrorLevel.Error Then
            SaveToLog(ex)
        End If

        Dim doShow As Boolean
        If ex.InnerException Is Nothing Then
            doShow = True
        End If

        If doShow Then
            '-->Solo se muestra si es el primero
            f.mEx = ex
            f.ShowDialog()
        End If

        Dim oEx As New Exception(ex.Message, ex)

        Throw oEx

    End Sub


    <System.Diagnostics.DebuggerStepThrough()> _
    Public Shared Sub Add(ByVal ex As Exception, ByVal comment As String)
        Dim f As New fmError
        Dim s As String

        If App.ExecuteExit Then
            Exit Sub
        End If

        '// ERROR DE ACCESO A BASE DE DATOS, NO BUSCAR DESCRIPCION DE ERROR
        If TypeOf ex Is OleDb.OleDbException Then
            If CType(ex, OleDb.OleDbException).ErrorCode = -2147217843 _
            Or CType(ex, OleDb.OleDbException).ErrorCode = -2147467259 Then '// SQL Server No exist or access denied
                GoTo showException
            End If
        End If

        If LibX.Data.Manager.Connection.AutoRollBackWhenError Then
            LibX.Data.Manager.Connection.RollBackTransaction()
        End If

        If TypeOf ex Is OleDb.OleDbException Then
            Dim oDx As Exception
            s = GetDataException(ex)
            oDx = New Exception(s, ex)
            ex = oDx
        End If

showException:
        If Not TypeOf ex Is LibXException OrElse CType(ex, LibXException).Errorlevel = EnumErrorLevel.Error Then
            SaveToLog(ex)
        End If

        Dim doShow As Boolean
        If ex.InnerException Is Nothing Then
            doShow = True
        End If

        If doShow Then
            '-->MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error)
            f.mEx = ex
            f.ShowDialog()
        End If

        Dim oEx As New Exception(ex.Message & vbCrLf & Trim(comment))

        Throw New Exception(oEx.Message, ex)

    End Sub


    Public Shared Sub Show(ByVal ex As Exception)
        Dim f As New fmError

        '// ERROR DE ACCESO A BASE DE DATOS, NO BUSCAR DESCRIPCION DE ERROR
        If TypeOf ex Is OleDb.OleDbException Then
            If CType(ex, OleDb.OleDbException).ErrorCode = -2147217843 _
            Or CType(ex, OleDb.OleDbException).ErrorCode = -2147467259 Then '// SQL Server No exist or access denied
                GoTo showException
            End If
        End If

        If mUseConnection Then
            If LibX.Data.Manager.Connection.AutoRollBackWhenError Then
                LibX.Data.Manager.Connection.RollBackTransaction()
            End If
        End If
        If TypeOf ex Is OleDb.OleDbException Then
            Dim oDx As Exception
            Dim s As String
            s = GetDataException(ex)
            oDx = New Exception(s, ex)
            ex = oDx
        End If
showException:


        If Not TypeOf ex Is LibXException OrElse CType(ex, LibXException).Errorlevel = EnumErrorLevel.Error Then
            SaveToLog(ex)
        End If

        f.mEx = ex
        f.ShowDialog()

    End Sub

    Public Shared Sub Show(ByVal ex As Exception, ByVal comment As String)
        Dim f As New fmError

        If App.ExecuteExit Then
            Exit Sub
        End If
        '// ERROR DE ACCESO A BASE DE DATOS, NO BUSCAR DESCRIPCION DE ERROR
        If TypeOf ex Is OleDb.OleDbException Then
            If CType(ex, OleDb.OleDbException).ErrorCode = -2147217843 _
            Or CType(ex, OleDb.OleDbException).ErrorCode = -2147467259 Then '// SQL Server No exist or access denied
                GoTo showException
            End If
        End If

        If LibX.Data.Manager.Connection.AutoRollBackWhenError Then
            LibX.Data.Manager.Connection.RollBackTransaction()
        End If
        If TypeOf ex Is OleDb.OleDbException Then
            Dim oDx As Exception
            Dim s As String
            s = GetDataException(ex)
            oDx = New Exception(s, ex)
            ex = oDx
        End If
showException:


        If Not TypeOf ex Is LibXException OrElse CType(ex, LibXException).Errorlevel = EnumErrorLevel.Error Then
            SaveToLog(ex)
        End If

        f.mEx = ex
        f.comment = comment
        f.ShowDialog()

    End Sub


End Class

