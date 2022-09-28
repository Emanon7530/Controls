Imports System.Reflection
Public Class LibXRunner

    Private Shared mPath As String
    Private Shared mHome As String

    '-->Asume que el tipo principal es el nombre del programa
    Public Shared Function Run(ByVal fullPrgName As String, ByVal mdl As String) As Object
        Try
            Return Run(fullPrgName, "", mdl, False)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Shared Function Run(ByVal fullPrgName As String, ByVal mdl As String, ByVal modal As Boolean) As Object
        Try

            Return Run(fullPrgName, "", mdl, modal)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Shared Function Run(ByVal fullPrgName As String, ByVal typeName As String, ByVal mdl As String) As Object
        Try

            Return Run(fullPrgName, typeName, mdl, False)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Shared Function Run(ByVal fullPrgName As String, ByVal typeName As String, ByVal mdl As String, ByVal modal As Boolean) As Object
        Try
            If Trim(mPath) = "" Then
                Dim s As String
                Dim sh As String
                sh = System.Configuration.ConfigurationSettings.AppSettings.Get("apppath")
                s = System.Configuration.ConfigurationSettings.AppSettings.Get("LibXProgramPath")

                mPath = s
                mHome = sh

            End If

            Dim oA() As String = New String() {mHome, mdl}
            Dim sP As String

            sP = String.Format(mPath, oA).ToUpper

            sP = sP.Replace("$PRG", fullPrgName)
            sP = sP.Replace("$EXE", String.Concat(fullPrgName, ".exe"))

            'Ejemplos de las variables
            '<add key="LibXReportPth" value="C:\SGT\Fuente\SGT\{0}\RPT\{1}" />
            '<add key="apppath" value="C:\SGT\Fuente\SGT\" />
            '<add key="LibXProgramPath" value="{0}{1}\PRG\$PRG\bin\$EXE" />


            Return RunWithPath(sP, typeName, modal)


        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    '''

    Public Shared Function RunWithPath(ByVal fullPrgName As String) As Object
        Try
            Return RunWithPath(fullPrgName, "", False)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Shared Function RunWithPath(ByVal fullPrgName As String, ByVal modal As Boolean) As Object
        Try

            Return RunWithPath(fullPrgName, "", modal)

        Catch ex As Exception
            Log.Add(ex)

        End Try
    End Function

    Public Shared Function RunWithPath(ByVal fullPrgName As String, ByVal typeName As String) As Object
        Try

            Return RunWithPath(fullPrgName, typeName, False)

        Catch ex As Exception
            Log.Add(ex)
        End Try
    End Function

    Public Shared Function RunWithPath(ByVal fullPrgName As String, ByVal typeName As String, ByVal modal As Boolean) As Object
        Try
            If Trim(typeName) = "" Then
                typeName = System.IO.Path.GetFileNameWithoutExtension(fullPrgName)
            End If

            Dim oA As [Assembly] = [Assembly].LoadFrom(fullPrgName)

            If oA Is Nothing Then
                Throw New Exception(String.Concat("El programa ", fullPrgName, " No pudo ser cargado"))
            End If

            Dim oT As Type = oA.GetType(String.Concat(typeName, ".", typeName), False, True)
            If oT Is Nothing Then
                Dim obt As Type
                Dim oTs() As Type = oA.GetTypes
                For Each obt In oTs
                    If obt.Name.ToLower = typeName.Trim.ToLower Then
                        oT = obt
                    End If
                Next
            End If

            If oT Is Nothing Then
                Throw New Exception(String.Concat("El Tipo ", typeName, " no se puede carga del programa ", fullPrgName))
            End If

            Dim ob As Object

            ob = Activator.CreateInstance(oT)
            If ob Is Nothing Then
                Throw New Exception(String.Concat("No se puedo crear el objeto del tipo ", typeName, " no se puede carga del programa ", fullPrgName))
            End If

            If Not LibX.App.MainMdi Is Nothing And Not modal Then
                Dim f As Form
                ob.MdiParent = LibX.App.MainMdi
            End If

            If Not LibX.App.CurrentPrgParams Is Nothing Then
                For Each oc As Object In ob.controls
                    If TypeOf oc Is LibXNavigator Then
                        Dim libxc As LibXConnector = CType(oc.Connector, LibXConnector)
                        With LibX.App.CurrentPrgParams
                            libxc.AllowDelete = .AllowDelete
                            libxc.AllowEdit = .AllowEdit
                            libxc.AllowNew = .AllowNew
                            libxc.AllowPrint = .AllowPrint
                            libxc.AllowQuery = .AllowQuery
                        End With
                    End If
                Next
            End If

            If modal Then
                ob.ShowDialog()
            Else
                ob.show()
            End If

            Return ob
        Catch ex As Exception
            Log.Add(ex, "Error cargando el programa " & fullPrgName)
            Return Nothing

        End Try
    End Function



End Class

Public Class LibxPrgParams
    Public IsFromOther As Boolean
    Public Datos As New Hashtable
    Public Value As Object
    Public isFromMenu As Boolean
    Public AllowNew As Boolean = True
    Public AllowDelete As Boolean = True
    Public AllowEdit As Boolean = True
    Public AllowPrint As Boolean = True
    Public AllowQuery As Boolean = True
    Public initMode As LibxInitModes = LibxInitModes.none
    Public WhereToExecute As String

    Public Function Clone() As LibxPrgParams
        Try
            Dim o As New LibxPrgParams
            o.IsFromOther = Me.IsFromOther
            o.Value = Me.Value
            o.isFromMenu = Me.isFromMenu
            o.initMode = Me.initMode
            o.WhereToExecute = WhereToExecute

            If Not Datos Is Nothing Then
                Dim x As Object
                Dim ox As New Hashtable
                For Each ex As Object In Datos.Keys
                    ox.Add(ex, Datos(ex))
                Next
                o.Datos = ox
            End If

            Return o
        Catch ex As Exception
            Log.Add(ex)
        End Try

    End Function
End Class
