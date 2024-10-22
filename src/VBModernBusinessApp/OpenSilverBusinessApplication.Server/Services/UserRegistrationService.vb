Imports System.ComponentModel.DataAnnotations
Imports Microsoft.AspNetCore.Identity
Imports Microsoft.Extensions.DependencyInjection
Imports OpenRiaServices.Server
Imports $ext_safeprojectname$.Models

Namespace Services

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

            Dim userManager = ServiceContext.GetRequiredService(Of UserManager(Of User))()

            Dim appUser = New User() With
            {
                .UserName = user.UserName,
                .Email = user.Email,
                .FriendlyName = user.FriendlyName,
                .PasswordQuestion = user.Question
            }
            appUser.PasswordAnswer = userManager.PasswordHasher.HashPassword(appUser, user.Answer)

            Dim result = userManager.CreateAsync(appUser, password).GetAwaiter().GetResult()

            If Not result.Succeeded Then
                Return result.Errors.Select(Function(x) x.Description)
            End If

            Return Array.Empty(Of String)()

        End Function

    End Class

End Namespace