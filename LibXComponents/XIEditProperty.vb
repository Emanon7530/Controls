Imports System.ComponentModel
Public Interface IEditProperty
    Enum InitialStateEnum
        Enabled
        Disabled
    End Enum

    Sub RefreshState(ByVal e As XChangeStateEventArgs)

    Property IgnoreRequiered() As Boolean
    Property Requiered() As Boolean

    <Browsable(True), Category("Initial State")> _
    Property NewState() As InitialStateEnum

    <Browsable(True), Category("Initial State")> _
    Property EditState() As InitialStateEnum

    <Browsable(True), Category("Initial State")> _
    Property FindState() As InitialStateEnum

    <Browsable(True), Category("Initial State")> _
        Property NewInitialValue() As String

    <Browsable(True), Category("Initial State")> _
    Property EditInitialValue() As String

    <Browsable(True), Category("Initial State")> _
    Property FindInitialValue() As String

End Interface
