Public Class XException
    Inherits System.ApplicationException

    Sub New()
        MyBase.New()
    End Sub

    Sub New(ByVal sMessage As String)
        MyBase.New(sMessage)
    End Sub

    Sub New(ByVal errnum As Integer)
        MyBase.New(LibX.Data.Manager.GetMessage(errnum))
    End Sub

    Sub New(ByVal errnum As Integer, ByVal comment As String)
        MyBase.New(LibX.Data.Manager.GetMessage(errnum, comment))
    End Sub

    Sub New(ByVal errnum As Integer, ByVal replaceValues() As Object)
        MyBase.New(LibX.Data.Manager.GetMessage(errnum, "", replaceValues))
    End Sub

    Sub New(ByVal errnum As Integer, ByVal comment As String, ByVal replaceValues() As Object)
        MyBase.New(LibX.Data.Manager.GetMessage(errnum, comment, replaceValues))
    End Sub

End Class
