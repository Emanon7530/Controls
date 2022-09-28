Public Class App
    Private Shared mIsRunningMenu As Boolean
    Private Shared mCParams As LibxPrgParams
    Private Shared mMainMdi As Form
    Private Shared mExecuteExit As Boolean

    Shared Sub New()
        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-DO")
    End Sub

    Public Shared Property ExecuteExit() As Boolean
        Get
            Return mExecuteExit
        End Get
        Set(ByVal Value As Boolean)
            mExecuteExit = Value
        End Set
    End Property

    Public Shared Property MainMdi() As Form
        Get
            Return mMainMdi
        End Get
        Set(ByVal Value As Form)
            mMainMdi = Value
        End Set
    End Property

    Public Shared Property RunningMenu() As Boolean
        Get
            Return mIsRunningMenu
        End Get
        Set(ByVal Value As Boolean)
            mIsRunningMenu = Value
        End Set
    End Property

    Public Shared Property CurrentPrgParams() As LibxPrgParams
        Get
            Return mCParams
        End Get
        Set(ByVal Value As LibxPrgParams)
            mCParams = Value
        End Set
    End Property

End Class
