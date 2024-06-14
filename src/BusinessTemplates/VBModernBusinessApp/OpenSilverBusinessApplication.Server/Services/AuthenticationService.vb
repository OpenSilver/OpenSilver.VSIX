Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Identity
Imports Microsoft.Extensions.DependencyInjection
Imports OpenRiaServices.Server
Imports OpenRiaServices.Server.Authentication
Imports $ext_safeprojectname$.Models

Namespace Services

    ''' <summary>
    ''' Domain Service responsible for authenticating users when they log on to the application.
    ''' </summary>
    <EnableClientAccess>
    Public Class AuthenticationService
        Inherits DomainService
        Implements IAuthentication(Of User)

        Public Function Login(userName As String, password As String, isPersistent As Boolean, customData As String) As User Implements IAuthentication(Of User).Login

            Dim signInManager = ServiceContext.GetRequiredService(Of SignInManager(Of User))()

            Dim response = signInManager.PasswordSignInAsync(userName, password, isPersistent, False).GetAwaiter().GetResult()

            If response.Succeeded Then
                Return GetAuthenticatedUser(userName)
            End If

            Return Nothing
        End Function

        Private Function GetAnonymousUser() As User
            Return New User With {.Name = String.Empty, .Roles = Array.Empty(Of String)}
        End Function

        Private Function GetAuthenticatedUser(userName As String) As User

            Dim userManager = ServiceContext.GetRequiredService(Of UserManager(Of User))()

            Dim user = userManager.FindByNameAsync(userName).GetAwaiter().GetResult()

            If user Is Nothing Then
                Return Nothing
            End If

            Return New User() With
        {
            .Name = user.UserName,
            .Roles = user.Roles,
            .PasswordAnswer = user.PasswordAnswer,
            .PasswordQuestion = user.PasswordQuestion
        }

        End Function

        <Query(IsComposable:=False)>
        Public Function GetUser() As User Implements IAuthentication(Of User).GetUser

            Dim identity = ServiceContext.GetRequiredService(Of IHttpContextAccessor)().HttpContext.User.Identity

            If identity.IsAuthenticated Then
                Return GetAuthenticatedUser(identity.Name)
            Else
                Return GetAnonymousUser()
            End If
            
        End Function

        Public Function Logout() As User Implements IAuthentication(Of User).Logout

            Dim SignInManager = ServiceContext.GetRequiredService(Of SignInManager(Of User))()

            SignInManager.SignOutAsync().GetAwaiter().GetResult()

            Return GetAnonymousUser()

        End Function

        Public Sub UpdateUser(user As User) Implements IAuthentication(Of User).UpdateUser
        End Sub

    End Class

End Namespace