Imports System.Reflection
Public Class ReportLib

    Public Enum WindowsStates
        Mormal = 0
        Maximixed = 2
        Minimixed = 1
    End Enum

    Declare Sub PECloseEngine Lib "crpe32.dll" ()
    Declare Function PEOpenEngine Lib "crpe32.dll" () As Integer
    Declare Function PEGetErrorCode Lib "CRPE32.DLL" (ByVal PrintJob%) As Integer
    Declare Function PEGetErrorText Lib "CRPE32.DLL" (ByVal PrintJob%, ByVal TextHandle As Long, ByVal TextLength%) As Integer

    '-->Para no hacer que los programas tenga que tener una referencia directa a interop_crystal.
    'Dim crystal As Crystal.CrystalReportClass
    Dim mcrystal As Object
    Dim mPrintJob As Integer
    Sub New()
        MyBase.new()

        If MdlUtil.hasOpenedEngine = True Then
            PECloseEngine()
        End If

        MdlUtil.hasOpenedEngine = True
    End Sub

    Sub New(ByVal mdl As String, ByVal ReportFileName As String)
        MyBase.New()

        Try
            Dim tmpReport As String
            Dim tmpPath As String

            With Me
                .Connect = Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectReport")
                If mdl.Trim = "" Then
                    .ReportFileName = ReportFileName
                Else
                    .ReportFileName = LibX.GetReportPath(mdl, ReportFileName)
                End If

                '// Crear una copia tempral para imprimir
                '// Crystal Report 8.5 tiene un problema que 
                '// cuando se mandan a imprimir mas de 20
                '// consecutivos da el error UNKNOWK.RPT, 
                '// access denied
                tmpPath = System.Configuration.ConfigurationSettings.AppSettings.Get("LibxTemp").ToString.Trim
                If tmpPath.Trim <> String.Empty Then
                    tmpReport = tmpPath & "\" & Replace(ReportFileName, ".rpt", "", 1) & DateTime.Now.ToString("yyyyMMddhhmmss") & ".rpt"
                    System.IO.File.Copy(.ReportFileName, tmpReport, True)

                    .ReportFileName = tmpReport
                End If

                '// Pasar el Connection String a cada subreport
                For i As Int16 = 0 To .GetNSubreports - 1
                    .SubreportToChange = .GetNthSubreportName(i)
                    .Connect = Configuration.ConfigurationSettings.AppSettings.Get("LibXConnectReport")
                Next

                '// Localizarse en el reporte principal
                .SubreportToChange = ""

                '// Recibir el SQL Query
                .WindowShowPrintSetupBtn = True
                .WindowState = WindowsStates.Maximixed
            End With

        Catch ex As Exception
            LibX.Log.Add(ex)
        End Try
    End Sub

    Public Property WindowShowPrintSetupBtn() As Boolean
        Get
            Return Crystal.WindowShowPrintSetupBtn
        End Get
        Set(ByVal Value As Boolean)
            Crystal.WindowShowPrintSetupBtn = Value
        End Set
    End Property

    Public Property Destination() As Integer
        Get
            Return Crystal.destination
        End Get
        Set(ByVal Value As Integer)
            Crystal.destination = Value
        End Set
    End Property

    Public Property Copias() As Integer
        Get
            Return Crystal.printercopies
        End Get
        Set(ByVal Value As Integer)
            Crystal.printercopies = Value
        End Set
    End Property

    Public ReadOnly Property Crystal() As Object
        Get
            Try
                Dim s As String

                If Not mcrystal Is Nothing Then
                    Return mcrystal
                End If


                '-->Los busca en System32, que es el sition def
                Dim sdef As String
                sdef = System.Environment.GetFolderPath(Environment.SpecialFolder.System)
                If Not sdef.EndsWith("\") Then
                    sdef = String.Concat(sdef, "\")
                End If

                sdef = String.Concat(sdef, "Interop.Crystal.dll")
                If System.IO.File.Exists(sdef) Then
                    s = sdef
                Else
                    '-->de lo contrario
                    '-->Lo busca del lugran de donde se cargó este dll
                    s = Me.GetType.Assembly.Location
                    s = Trim(System.IO.Path.GetDirectoryName(s))
                    If Not s.EndsWith("\") Then
                        s = String.Concat(s, "\")
                    End If
                    s = String.Concat(s, "Interop.Crystal.dll")

                    If System.IO.File.Exists(s) = False Then
                        Throw New ApplicationException("No se encontro la libreria de impresión (Interop.Crystal.dll)")
                    End If
                End If

                Dim mO As [Assembly] = [Assembly].LoadFrom(s)
                Dim oT As Type
                oT = mO.GetType("Crystal.CrystalReportClass", False, True)

                mcrystal = Activator.CreateInstance(oT)

                Return mcrystal


            Catch ex As Exception
                Log.Add(ex)
            End Try


        End Get
    End Property


    Public Property ReportFileName() As String
        Get
            Return Crystal.ReportFileName
        End Get
        Set(ByVal Value As String)
            Crystal.ReportFileName = Value
        End Set
    End Property

    Public Property Connect() As String
        Get
            Return Crystal.Connect
        End Get
        Set(ByVal Value As String)
            Crystal.Connect = Value
        End Set
    End Property

    Public Function GetNSubreports() As Integer
        Return Crystal.GetNSubreports
    End Function

    Public Property SubreportToChange() As String
        Get
            Return Crystal.SubreportToChange
        End Get
        Set(ByVal Value As String)
            Crystal.SubreportToChange = Value
        End Set
    End Property

    Public Function GetNthSubreportName(ByVal index As Integer) As String
        Return Me.Crystal.GetNthSubreportName(index)
    End Function

    Public Property ParameterFields(ByVal index As Integer) As String
        Get
            Return Me.Crystal.ParameterFields(index)
        End Get
        Set(ByVal Value As String)
            Crystal.ParameterFields(index) = Value
        End Set
    End Property

    Public Sub RetrieveSQLQuery()
        Me.Crystal.RetrieveSQLQuery()
    End Sub

    Public Property SQLQuery() As String
        Get
            Return Me.Crystal.SQLQuery
        End Get
        Set(ByVal Value As String)
            Crystal.SQLQuery = Value
        End Set
    End Property

    Public Property WindowState() As WindowsStates
        Get
            Return CInt(Crystal.WindowState)
        End Get
        Set(ByVal Value As WindowsStates)
            Crystal.WindowState = CInt(Value)
        End Set
    End Property

    Public Property Action() As Integer
        Get
            Return Crystal.Action
        End Get
        Set(ByVal Value As Integer)
            Crystal.Action = Value
        End Set
    End Property

    Public Property PrinterName() As String
        Get
            Return Crystal.PrinterName
        End Get
        Set(ByVal Value As String)
            Crystal.PrinterName = Value
        End Set
    End Property

End Class
