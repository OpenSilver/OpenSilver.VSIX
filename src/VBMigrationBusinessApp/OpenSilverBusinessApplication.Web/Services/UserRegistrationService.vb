Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Web.Profile
Imports System.Web.Security
Imports OpenRiaServices.DomainServices.Hosting
Imports OpenRiaServices.DomainServices.Server

' TODO: Switch to a secure endpoint when deploying the application.
'       The user's name and password should only be passed using https.
'       To do this, set the RequiresSecureEndpoint property on EnableClientAccessAttribute to true.
'
'       <EnableClientAccess(RequiresSecureEndpoint = true)>
'
'       More information on using https with a Domain Service can be found on MSDN.

''' <summary>
''' Domain Service responsible for registering users.
''' </summary>
<EnableClientAccess>
Public Class UserRegistrationService
    Inherits DomainService

    ''' <summary>
    ''' Role to which users will be added by default.
    ''' </summary>
    Public Const DefaultRole As String = "Registered Users"

    ' NOTE: This is a sample code to get your application started.
    '       In the production code you should provide a mitigation against a denial of service attack
    '       by providing CAPTCHA control functionality or verifying user's email address.

    ''' <summary>
    ''' Adds a new user with the supplied <see cref="RegistrationData"/> and <paramref name="password"/>.
    ''' </summary>
    ''' <param name="user">The registration information for this user.</param>
    ''' <param name="password">The password for the new user.</param>
    <Invoke(HasSideEffects:=True)>
    Public Function CreateUser(user As RegistrationData,
        <Required(ErrorMessage:="This field is required")>
        <RegularExpression("^.*[^a-zA-Z0-9].*$", ErrorMessage:="A password needs to contain at least one special character e.g. @ or #")>
        <StringLength(50, MinimumLength:=7, ErrorMessage:="Password must be at least 7 and at most 50 characters long")>
        password As String) As IEnumerable(Of String)

        If user Is Nothing Then
            Throw New ArgumentNullException("user")
        End If

        ' Run this BEFORE creating the user to make sure roles are enabled and the default role is available.
        '
        ' If there is a problem with the role manager, it is better to fail now than to fail after the user is created.
        If Not Roles.RoleExists(UserRegistrationService.DefaultRole) Then
            Roles.CreateRole(UserRegistrationService.DefaultRole)
        End If

        ' NOTE: ASP.NET by default uses SQL Server Express to create the user database.
        ' CreateUser will fail if you do not have SQL Server Express installed.
        Dim createStatus As MembershipCreateStatus = Nothing
        Membership.CreateUser(user.UserName, password, user.Email, user.Question, user.Answer, True, Nothing, createStatus)

        If createStatus <> MembershipCreateStatus.Success Then
            Return New String() {createStatus.ToString()}
        End If

        ' Assign the user to the default role.
        ' This will fail if role management is disabled.
        Roles.AddUserToRole(user.UserName, UserRegistrationService.DefaultRole)

        ' Set the friendly name (profile setting).
        ' This will fail if the web.config is configured incorrectly.
        Dim profile As ProfileBase = ProfileBase.Create(user.UserName, True)
        profile.SetPropertyValue("FriendlyName", user.FriendlyName)
        profile.Save()

        Return Array.Empty(Of String)()

    End Function

End Class