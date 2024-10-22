Imports System
Imports System.Web
Imports System.Web.Security
Imports OpenRiaServices.DomainServices.Hosting
Imports OpenRiaServices.DomainServices.Server
Imports OpenRiaServices.DomainServices.Server.ApplicationServices

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

    Private Shared ReadOnly DefaultUser As User = New User With {.Name = String.Empty}

    Private Function MapMembershipUser(user As MembershipUser) As User
        Return New User With {.Name = user.UserName}
    End Function

    Public Function ValidateUser(userName As String, password As String) As Boolean
        Return Membership.ValidateUser(userName, password)
    End Function

    <Query(IsComposable:=False)>
    Public Function GetUser() As User Implements IAuthentication(Of User).GetUser

        Dim identity = ServiceContext?.User?.Identity

        If identity Is Nothing Then
            Return DefaultUser
        End If

        If identity.IsAuthenticated Then

            Dim user = Membership.GetUser(identity.Name)

            Return MapMembershipUser(user)

        End If

        Return DefaultUser

    End Function

    Public Function Login(userName As String, password As String, isPersistent As Boolean, customData As String) As User Implements IAuthentication(Of User).Login

        If (Not ValidateUser(userName, password)) Then
            Return Nothing
        End If

        ' if IsPersistent is true, will keep logged in for up to a week (or until you logout)
        Dim ticket = New FormsAuthenticationTicket(
            version:=1,
            name:=userName,
            issueDate:=DateTime.Now,
            expiration:=DateTime.Now.AddMinutes(60),
            isPersistent,
            userData:=If(customData, String.Empty),
            cookiePath:=FormsAuthentication.FormsCookiePath)

        Dim encryptedTicket = FormsAuthentication.Encrypt(ticket)
        Dim authCookie = New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
        authCookie.HttpOnly = False
        authCookie.Expires = ticket.Expiration

        Dim httpContext = CType(ServiceContext.GetService(GetType(HttpContextBase)), HttpContextBase)
        httpContext.Response.Cookies.Add(authCookie)

        Dim user = Membership.GetUser(userName)

        If user Is Nothing Then
            Return Nothing
        End If

        Return MapMembershipUser(user)

    End Function

    Public Function Logout() As User Implements IAuthentication(Of User).Logout
        FormsAuthentication.SignOut()
        Return DefaultUser
    End Function

    Public Sub UpdateUser(user As User) Implements IAuthentication(Of User).UpdateUser
    End Sub

End Class