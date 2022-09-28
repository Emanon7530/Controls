Public MustInherit Class LibxGroupBoxEventArgs
    Inherits EventArgs
    Public ItemValue As Object
    Public Handled As Boolean
End Class

Public Class LoadItemEventArgs
    Inherits LibxGroupBoxEventArgs

End Class

Public Class ChangeValueEventArgs
    Inherits LibxGroupBoxEventArgs

End Class

