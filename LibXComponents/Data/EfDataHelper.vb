Imports Microsoft.EntityFrameworkCore
Imports LibXComponents.DataAccess
Imports LibXComponents.Entities
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Diagnostics ' For Debug.WriteLine

Public Class EfDataHelper

    Public Shared Function AuthenticateUserWithEFCore(sUsr As String, sPass As String, mSuCursal As Integer) As User
        Try
            Using context As New AppDbContext() ' Assumes AppDbContext has temporary connection string
                Dim trimmedUserName = Trim(sUsr)
                Dim appUser = context.Users.FirstOrDefault(Function(u) u.UserName = trimmedUserName AndAlso u.SucursalCode = mSuCursal)

                If appUser IsNot Nothing Then
                    ' IMPORTANT SECURITY WARNING:
                    ' The line below compares the input password (sPass) directly with appUser.PasswordHash.
                    ' This is ONLY secure if appUser.PasswordHash ALREADY CONTAINS a properly hashed version
                    ' of the password and sPass is hashed before comparison using the SAME hashing algorithm.
                    ' The original code ("passwrd='", Trim(sPass), "'") implies PLAIN TEXT password storage,
                    ' which is a SEVERE security vulnerability.
                    '
                    ' TODO:
                    ' 1. Ensure passwords in the 'scusers' table (mapped to User.PasswordHash) are securely hashed (e.g., using ASP.NET Core Identity hasher).
                    ' 2. Implement a secure password verification function, e.g., VerifyPasswordHash(submittedPassword, storedHash).
                    ' 3. Replace the direct string comparison below with:
                    '    If VerifyPasswordHash(Trim(sPass), appUser.PasswordHash) Then ...

                    If appUser.PasswordHash = Trim(sPass) Then ' <<< !!! INSECURE IF PasswordHash IS PLAIN TEXT OR HASHING MISMATCH !!!
                        Return appUser ' Authentication successful
                    Else
                        Return Nothing ' Password incorrect
                    End If
                Else
                    Return Nothing ' User not found
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error in AuthenticateUserWithEFCore: " & ex.ToString())
            Return Nothing
        End Try
    End Function

    ' Placeholder for fetching WhseCode if needed, to be implemented with EF Core
    ' Public Shared Function GetWhseCodeForSucursal(sucursalCode As Integer) As String
    '     Try
    '         Using context As New AppDbContext()
    '             ' Assuming a Sucursales entity exists that maps cgsucursal table
    '             ' Dim sucInfo = context.Sucursales.FirstOrDefault(Function(s) s.SucursalCode = sucursalCode)
    '             ' If sucInfo IsNot Nothing Then
    '             '     Return sucInfo.WhseCode
    '             ' End If
    '             Return String.Empty ' Or a default value
    '         End Using
    '     Catch ex As Exception
    '         Debug.WriteLine("Error in GetWhseCodeForSucursal: " & ex.ToString())
    '         Return String.Empty ' Or a default value
    '     End Try
    ' End Function

    Public Shared Async Function GetEntityListFromSqlAsync(Of TEntity As Class)(sqlQuery As String, ParamArray parameters As Object()) As Task(Of List(Of TEntity))
        Try
            Using context As New AppDbContext() ' Assumes AppDbContext has temp connection string
                Return Await context.Set(Of TEntity)().FromSqlRaw(sqlQuery, parameters).ToListAsync()
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error in GetEntityListFromSqlAsync(Of " & GetType(TEntity).Name & "): " & ex.ToString())
            Return New List(Of TEntity)() ' Return empty list on error
        End Try
    End Function

    Public Shared Async Function GetScalarAsync(Of T)(sqlQuery As String, ParamArray parameters As Object()) As Task(Of T)
        Try
            Using context As New AppDbContext()
                ' Using RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync for non-tracking queries or specific needs.
                ' For scalar, often best to use a specific query that returns one row, one column.
                ' EF Core's FromSqlRaw or SqlQuery is typically used to map to types.
                ' A more direct ADO.NET approach might be simpler for pure scalar from arbitrary SQL if not mapping to an entity.
                ' This implementation uses SqlQueryRaw for flexibility, assuming the query returns a single value convertible to T.

                ' This is a simplified way to get a scalar. It materializes a list of single objects and takes the first.
                ' For performance critical scalar, specific ADO.NET or a more targeted EF query might be better.
                Dim resultList = Await context.Database.SqlQueryRaw(Of Object)(sqlQuery, parameters).ToListAsync()
                Dim result = resultList.FirstOrDefault()

                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    Return CType(Convert.ChangeType(result, GetType(T)), T)
                End If
                Return Nothing ' Or default(T) depending on desired behavior for value types
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error in GetScalarAsync(Of " & GetType(T).Name & "): " & ex.ToString())
            Return Nothing ' Or default(T) or throw
        End Try
    End Function

    Public Shared Async Function ExecuteNonQueryAsync(sqlCommand As String, ParamArray parameters As Object()) As Task(Of Integer)
        Try
            Using context As New AppDbContext()
                Return Await context.Database.ExecuteSqlRawAsync(sqlCommand, parameters)
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error in ExecuteNonQueryAsync: " & ex.ToString())
            Return -1 ' Return -1 to indicate error, similar to some ADO.NET patterns
        End Try
    End Function

End Class
