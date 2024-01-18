Imports System
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Security
Imports OpenRiaServices.DomainServices.Hosting
Imports OpenRiaServices.DomainServices.Server
Imports OpenRiaServices.DomainServices.Server.Authentication

' TODO: Switch to a secure endpoint when deploying the application.
'       The user's name and password should only be passed using https.
'       To do this, set the RequiresSecureEndpoint property on EnableClientAccessAttribute to true.
'
'       [EnableClientAccess(RequiresSecureEndpoint = true)]
'
'       More information on using https with a Domain Service can be found on MSDN.

''' <summary>
''' Domain Service responsible for authenticating users when they log on to the application.
'''
''' Most of the functionality is already provided by the AuthenticationBase class.
''' </summary>
<EnableClientAccess>
Public Class AuthenticationService
    Inherits DomainService
    Implements IAuthentication(Of User)

    Private Shared DefaultUser As User = New User With {.Name = String.Empty}

    ''' <summary>
    ''' Use this method to fill your User object with additional data (from database, for example)
    ''' </summary>
    ''' <param name="user"></param>
    ''' <returns></returns>
    Private Function MapMembershipUser(user As MembershipUser) As User
        Return New User With {.Name = user.UserName}
    End Function

    Public Function ValidateUser(userName As String, password As String) As Boolean
        Return Membership.ValidateUser(userName, password)
    End Function

    Public Sub UpdateUser(user As User) Implements IAuthentication(Of User).UpdateUser
    End Sub

    Public Function Login(userName As String, password As String, isPersistent As Boolean, customData As String) As User Implements IAuthentication(Of User).Login

        If Not ValidateUser(userName, password) Then Return Nothing

        ' if IsPersistent Is true, will keep logged in for up to a week (Or until you logout)
        Dim ticket As New FormsAuthenticationTicket(
            version:=1,
            name:=userName,
            issueDate:=DateTime.Now,
            expiration:=DateTime.Now.AddMinutes(60),
            isPersistent,
            userData:=If(customData, String.Empty),
            FormsAuthentication.FormsCookiePath)

        Dim encryptedTicket As String = FormsAuthentication.Encrypt(ticket)

        Dim authCookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) With
        {
            .HttpOnly = False,
            .Expires = ticket.Expiration
        }

        Dim httpContext As HttpContextBase = CType(ServiceContext.GetService(GetType(HttpContextBase)), HttpContextBase)
        httpContext.Response.Cookies.Add(authCookie)

        Dim user As MembershipUser = Membership.GetUser(userName)

        If user Is Nothing Then Return Nothing

        Return MapMembershipUser(user)
    End Function

    Public Function Logout() As User Implements IAuthentication(Of User).Logout

        FormsAuthentication.SignOut()

        Return DefaultUser

    End Function

    Public Function GetUser() As User Implements IAuthentication(Of User).GetUser

        Dim identity As IIdentity = ServiceContext?.User?.Identity

        If identity Is Nothing Then Return DefaultUser

        If identity.IsAuthenticated Then

            Dim user As MembershipUser = Membership.GetUser(identity.Name)

            Return MapMembershipUser(user)

        End If

        Return DefaultUser

    End Function

End Class