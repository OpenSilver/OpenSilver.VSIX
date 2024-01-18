Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports OpenRiaServices.DomainServices.Server.Authentication

''' <summary>
''' Class containing information about the authenticated user.
''' </summary>
Partial Public Class User
    Implements IUser

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

    Public Property Roles As IEnumerable(Of String) = New List(Of String)() Implements IUser.Roles

End Class