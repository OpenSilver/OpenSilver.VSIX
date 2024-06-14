Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports Microsoft.AspNetCore.Identity
Imports OpenRiaServices.Server.Authentication

Namespace Models

    ''' <summary>
    ''' Class containing information about the authenticated user.
    ''' </summary>
    Partial Public Class User
        Inherits IdentityUser
        Implements IUser

        Private _roles As IEnumerable(Of String)

        ''' <summary>
        ''' Gets and sets the friendly name of the user.
        ''' </summary>
        Public Property FriendlyName As String

        Public Property PasswordAnswer As String
        Public Property PasswordQuestion As String

        <Key>
        Public Property Name As String Implements IUser.Name

        <NotMapped>
        Public Property Roles As IEnumerable(Of String) Implements IUser.Roles
            Get
                Return _roles
            End Get
            Set(value As IEnumerable(Of String))
                _roles = value
            End Set
        End Property

    End Class

End Namespace