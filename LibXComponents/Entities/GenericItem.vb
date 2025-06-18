Imports System

Public Class GenericItem
    ' Assuming a common Integer primary key.
    Public Property Id As Integer

    Public Property Name As String

    Public Property Description As String

    Public Property CreatedDate As DateTime

    Public Property ModifiedDate As DateTime?

    ' Properties for self-referencing relationship
    Public Property ParentId As Integer?
    Public Overridable Property Parent As GenericItem ' Overridable for EF Core lazy loading
    Public Overridable Property Children As ICollection(Of GenericItem) = New List(Of GenericItem) ' Overridable for EF Core

    ' Add other common properties that might be found in various tables
    ' that LibXGrid might display.
    ' For example:
    ' Public Property IsEnabled As Boolean
    ' Public Property Value As Decimal
    ' Public Property Category As String

End Class
