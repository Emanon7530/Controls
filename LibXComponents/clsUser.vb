Public Class User
    Shared mVendCode As Int16
    Shared mUserName As String
    Shared mUserID As String
    Shared mesCajera As Boolean
    Shared VendedorName As String
    Shared mVendedorID As Integer
    Shared misLogged As Boolean = False
    Shared mWhDefault As Integer
    Shared mPassword As String
    Shared mSucursal As Integer
    Public Shared Property WhDefault() As Integer
        Get
            Return mWhDefault
        End Get
        Set(ByVal Value As Integer)
            mWhDefault = Value
        End Set
    End Property

    Public Shared Property isLogged() As Boolean
        Get
            Return misLogged
        End Get
        Set(ByVal Value As Boolean)
            misLogged = Value
        End Set
    End Property

    Public Shared Property VendedorID() As Int16
        Get
            Return mVendedorID
        End Get
        Set(ByVal Value As Int16)
            mVendedorID = Value
            VendedorName = MdlUtil.GetVendorName()
        End Set
    End Property

    Public Shared Property NombreVendedor() As String
        Get
            Return VendedorName
        End Get
        Set(ByVal Value As String)
            VendedorName = Value
        End Set
    End Property

    Public Shared Property esCajera() As Boolean
        Get
            Return mesCajera
        End Get
        Set(ByVal Value As Boolean)
            mesCajera = Value
        End Set
    End Property

    Public Shared Property UserName() As String
        Get
            Return mUserName
        End Get
        Set(ByVal Value As String)
            mUserName = Value
        End Set
    End Property

    Public Shared Property UserID() As String
        Get
            Return mUserID
        End Get
        Set(ByVal Value As String)
            mUserID = Value
        End Set
    End Property

    Public Shared Property Sucursal() As Integer
        Get
            Return mSucursal
        End Get
        Set(ByVal Value As Integer)
            mSucursal = Value
        End Set
    End Property
End Class
