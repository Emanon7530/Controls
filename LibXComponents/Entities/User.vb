Imports System

Public Class User
    ' Assuming UserID is the primary key. Adjust type if necessary (e.g., Integer).
    Public Property UserID As String

    Public Property UserName As String

    ' It's highly recommended to store password hashes, not plain text passwords.
    Public Property PasswordHash As String

    ' Using more descriptive names based on potential underlying column names like suc_code
    Public Property SucursalCode As Integer

    ' Using more descriptive names based on potential underlying column names like vend_code
    Public Property VendedorCode As Integer

    ' Add any other relevant properties from the scusers table as they are identified.
    ' For example:
    ' Public Property Email As String
    ' Public Property IsActive As Boolean
    ' Public Property LastLoginDate As DateTime?

End Class
