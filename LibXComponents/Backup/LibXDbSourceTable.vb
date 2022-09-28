Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization

'<Serializable(), TypeConverter(GetType(LibXDbSourceTableConverter))> _
<DesignTimeVisible(False), TypeConverter(GetType(LibXDbSourceTableConverter))> _
Public Class LibXDbSourceTable
    Inherits Component
    Private mTableName As String
    Private mSource() As String
    Private mFillOnQuery As Boolean = True
    Private mHeaderIsOnGrid As Boolean = False
    Private mFillOnRowChange As Boolean
    Private mAllowEdit As Boolean = True
    Private mAllowDelete As Boolean = True
    Private mAllowNew As Boolean = True
    Private mInnerJoin As Boolean = True
    Private mSerialColName As String
    Private mLineColName As String
    Private mKeyString As String
    Private mCustomDbUpdate As Boolean
    Private mDeleteOrder As Integer
    Private mInsertOrder As Integer
    Private mUpdateOrder As Integer
    Private mIsDetail As Boolean
    Private mMasterTable As String
    Private mMasterDetailRelation() As String
    Private mUseRowRetrieveEvents As Boolean
    Private mSortCol As String
    Private mIncSer As Boolean

    Public Sub New()

    End Sub

    Public Property AutoIncrementSerial() As Boolean
        Get
            Return mIncSer
        End Get
        Set(ByVal Value As Boolean)
            mIncSer = Value
        End Set
    End Property

    Public Property Sort() As String
        Get
            Return mSortCol
        End Get
        Set(ByVal Value As String)
            mSortCol = Value
        End Set
    End Property

    Public Property UseRowRetrieve() As Boolean
        Get
            Return mUseRowRetrieveEvents
        End Get
        Set(ByVal Value As Boolean)
            mUseRowRetrieveEvents = Value
        End Set
    End Property

    Public Property IsDetail() As Boolean
        Get
            Return mIsDetail
        End Get
        Set(ByVal Value As Boolean)
            mIsDetail = Value
        End Set
    End Property

    Public Property MasterDetailRelation() As String()
        Get
            Return mMasterDetailRelation
        End Get
        Set(ByVal Value As String())
            mMasterDetailRelation = Value
        End Set
    End Property

    Public Property MasterTableName() As String
        Get
            Return mMasterTable
        End Get
        Set(ByVal Value As String)
            mMasterTable = Value
        End Set
    End Property

    Public Property CustomDbUpdate() As Boolean
        Get
            Return mCustomDbUpdate
        End Get
        Set(ByVal Value As Boolean)
            mCustomDbUpdate = Value
        End Set
    End Property

    Public Property KeyFields() As String
        Get
            Return mKeyString
        End Get
        Set(ByVal Value As String)
            mKeyString = Value
        End Set
    End Property

    Public Property SerialColumnName() As String
        Get
            Return mSerialColName
        End Get
        Set(ByVal Value As String)
            mSerialColName = Value
        End Set
    End Property
    Public Property LineColName() As String
        Get
            Return mLineColName
        End Get
        Set(ByVal Value As String)
            mLineColName = Value
        End Set
    End Property

    Public Property AllowNew() As Boolean
        Get
            Return mAllowNew
        End Get
        Set(ByVal Value As Boolean)
            mAllowNew = Value
        End Set
    End Property

    Public Property InnerJon() As Boolean
        Get
            Return mInnerJoin
        End Get
        Set(ByVal Value As Boolean)
            mInnerJoin = Value
        End Set
    End Property

    Public Property HeaderIsOnGrid() As Boolean
        Get
            Return mHeaderIsOnGrid
        End Get
        Set(ByVal Value As Boolean)
            mHeaderIsOnGrid = Value
        End Set
    End Property

    Public Property AllowEdit() As Boolean
        Get
            Return Me.mAllowEdit
        End Get
        Set(ByVal Value As Boolean)
            mAllowEdit = Value
        End Set
    End Property

    Public Property AllowDelete() As Boolean
        Get
            Return Me.mAllowDelete
        End Get
        Set(ByVal Value As Boolean)
            mAllowDelete = Value
        End Set
    End Property

    Public Property TableName() As String
        Get
            Return mTableName
        End Get
        Set(ByVal Value As String)
            mTableName = Value
        End Set
    End Property

    Public Property Source() As String()
        Get
            Return mSource
        End Get
        Set(ByVal Value As String())
            If Not value Is Nothing Then
                If Value.Length <= 0 Then
                    value = Nothing
                End If
            End If
            mSource = Value

        End Set
    End Property

    <Browsable(False)> _
    Public Property FillOnQuery() As Boolean
        Get
            Return mFillOnQuery
        End Get
        Set(ByVal Value As Boolean)
            mFillOnQuery = Value
        End Set
    End Property

    Public Property FillOnRowChange() As Boolean
        Get
            Return Me.mFillOnRowChange
        End Get
        Set(ByVal Value As Boolean)
            Me.mFillOnRowChange = Value
        End Set
    End Property

    Public Property DeleteOrder() As Integer
        Get

            Return mDeleteOrder
        End Get
        Set(ByVal Value As Integer)
            mDeleteOrder = Value
        End Set
    End Property

    Public Property UpdateOrder() As Integer
        Get
            Return mUpdateOrder
        End Get
        Set(ByVal Value As Integer)
            mUpdateOrder = Value
        End Set
    End Property

    Public Property InsertOrder() As Integer
        Get
            Return mInsertOrder
        End Get
        Set(ByVal Value As Integer)
            mInsertOrder = Value
        End Set
    End Property

End Class
