Public Class ExecutingActionEventArgs
    Inherits EventArgs
    Public Action As LibxConnectionActions
    Public AcceptedAction As LibxConnectionActions
    Public CanceledAction As LibxConnectionActions
    Public Handled As Boolean
End Class

Public Class ExecutedActionEventArgs
    Inherits ExecutingActionEventArgs

    Public row As DataRowView
End Class

Public Class SettingDefaultNewValues
    Inherits EventArgs
    Public row As DataRowView
End Class

Public Class SettingDefaultEditValues
    Inherits EventArgs
    Public row As DataRowView
End Class

Public Class SettingDefaultQueryValues
    Inherits EventArgs
    Public row As DataRowView
End Class

Public Class ValidatingControlsEventArgs
    Inherits EventArgs

    Public FocusControl As Control
End Class

Public Class ValidatingRequieredControlsEventArgs
    Inherits EventArgs

    Public isDetail As Boolean
    Public ValidatedControl As Control
    Public Ignore As Boolean

End Class

Public Class XRowChangeEventArgs
    Inherits EventArgs

    Private m_blnHandled As Boolean = False
    Private m_blnHandledTrans As Boolean = False
    Private m_blnHasErrors As Boolean = False
    Private m_objRow As Object
    Public FistRowChangeAfterQuery As Boolean

    Sub New()

    End Sub

    Public Property row() As Object
        Get
            Return m_objRow
        End Get
        Set(ByVal Value As Object)
            m_objRow = Value
        End Set
    End Property

End Class


Public Class XChangeStateEventArgs
    Inherits EventArgs

    Dim m_blnHideNavigationButtons As Boolean = False
    Dim m_udtState As LibxConnectorState
    Dim m_blnLoading As Boolean = False
    Dim m_blnIsDataEditing As Boolean
    Dim m_blnIsEditing As Boolean
    Dim m_blnIsInDataQuery As Boolean
    Dim m_udtAction As LibxConnectionActions
    Dim m_udtActionAc As LibxConnectionActions
    Public isMoving As Boolean

    Public Property isLoadingForm() As Boolean
        Get
            Return m_blnLoading
        End Get
        Set(ByVal Value As Boolean)
            m_blnLoading = Value
        End Set
    End Property
    Public Property hideNavigatioinButtons() As Boolean
        Get
            Return m_blnHideNavigationButtons
        End Get
        Set(ByVal Value As Boolean)
            m_blnHideNavigationButtons = Value
        End Set
    End Property

    Public Property action() As LibxConnectionActions
        Get
            Return m_udtAction
        End Get
        Set(ByVal Value As LibxConnectionActions)
            m_udtAction = Value
        End Set
    End Property

    Public Property aceptedAction() As LibxConnectionActions
        Get
            Return m_udtActionAc
        End Get
        Set(ByVal Value As LibxConnectionActions)
            m_udtActionAc = Value
        End Set
    End Property

    Public Property state() As LibxConnectorState
        Get
            Return m_udtState
        End Get
        Set(ByVal Value As LibxConnectorState)
            m_udtState = Value
        End Set
    End Property

    Public Property isEditing() As Boolean
        Get
            Return m_blnIsEditing
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsEditing = Value
        End Set
    End Property

    Public Property isInDataQuery() As Boolean
        Get
            Return Me.m_blnIsInDataQuery
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsInDataQuery = Value
        End Set
    End Property

    Public Property isDataEditing() As Boolean
        Get
            Return Me.m_blnIsDataEditing
        End Get
        Set(ByVal Value As Boolean)
            m_blnIsDataEditing = Value
        End Set
    End Property

End Class

Public Class NetComboSelectedEventArgs
    Inherits EventArgs

    Private m_objValue As Object
    Private m_objPrevValue As Object

    Public Description As String
    Public FullDescription As String
    Public Row As Object

    Public Property value() As Object
        Get
            Return m_objValue
        End Get
        Set(ByVal Value As Object)
            m_objValue = Value
        End Set
    End Property

    Public Property PreviousValue() As Object
        Get
            Return m_objPrevValue
        End Get
        Set(ByVal Value As Object)
            m_objPrevValue = Value
        End Set
    End Property
End Class

Public Class LoadXComboItemsEventArgs
    Inherits EventArgs

    Private m_blnHandled As Boolean = False
    Private m_objArr As Array
    Private m_objTable As Object
    Private m_strKeyField As String
    Private m_strDisplayField As String
    Private mSqlstring As String

    Sub New()

    End Sub

    Public Property SqlString() As String
        Get
            Return mSqlstring
        End Get
        Set(ByVal Value As String)
            mSqlstring = Value
        End Set
    End Property

    Public Property keyField() As String
        Get
            Return m_strKeyField
        End Get
        Set(ByVal Value As String)
            m_strKeyField = Value
        End Set
    End Property

    Public Property displayFields() As String
        Get
            Return m_strDisplayField
        End Get
        Set(ByVal Value As String)
            m_strDisplayField = Value
        End Set
    End Property

    Public Property lookupTable() As Object
        Get
            Return m_objTable
        End Get
        Set(ByVal Value As Object)
            m_objTable = Value
        End Set
    End Property

    Public Property excludedValues() As Array
        Get
            Return m_objArr
        End Get
        Set(ByVal Value As Array)
            m_objArr = Value
        End Set
    End Property

    Public Property handled() As Boolean
        Get
            Return m_blnHandled

        End Get
        Set(ByVal Value As Boolean)
            m_blnHandled = Value
        End Set
    End Property



End Class


Public Class XComboSelectedEventArgs
    Inherits EventArgs

    Private m_objValue As Object
    Private m_objPrevValue As Object

    Public Description As String
    Public FullDescription As String
    Public Row As Object

    Public Property value() As Object
        Get
            Return m_objValue
        End Get
        Set(ByVal Value As Object)
            m_objValue = Value
        End Set
    End Property

    Public Property PreviousValue() As Object
        Get
            Return m_objPrevValue
        End Get
        Set(ByVal Value As Object)
            m_objPrevValue = Value
        End Set
    End Property

End Class



Public Class XBeforeExecuteQueryEventArgs
    Inherits EventArgs

    Public row As DataRow
    Public Sql As String
    Public Where As String
    Public AditionalWhere As String
    Public DoFill As Boolean = True
End Class

Public Class XBeforeExecuteFillEventArgs
    Inherits EventArgs

    Public Sql As String
    Public DoFill As Boolean = True
End Class

Public Class XBeforeSaveDetailEventArgs
    Inherits EventArgs

    Public TableInfo As LibXDbSourceTable
    Public MasterRow As DataRow
    Public Handled As Boolean

End Class

Public Class XbeforeLoadDetailEventArgs
    Inherits EventArgs

    Public TableInfo As LibXDbSourceTable
    Public MasterRow As DataRow
    Public Sql As String
    Public WhereJoin As String
    Public AditionalWhere As String
    Public Handled As Boolean
    Public ConcatenateWhereJoin As Boolean = True
End Class

