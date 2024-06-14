Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports OpenRiaServices.DomainServices.Server.ApplicationServices

''' <summary>
''' Class containing information about the authenticated user.
''' </summary>
Partial Public Class User
    Implements IUser

    Private _roles As IEnumerable(Of String)

    ' NOTE: Profile properties can be added for use in application.
    ' To enable profiles, edit the appropriate section of web.config file.
    '
    ' public string MyProfileProperty { get; set; }

    ''' <summary>
    ''' Gets and sets the friendly name of the user.
    ''' </summary>
    Public Property FriendlyName As String

    <Key>
    Public Property Name As String Implements IUser.Name

    Public Property Roles As IEnumerable(Of String) Implements IUser.Roles
        Get
            Return _roles
        End Get
        Set(value As IEnumerable(Of String))
            _roles = value
        End Set
    End Property

End Class