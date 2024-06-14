Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Identity
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Net.Http.Headers
Imports OpenRiaServices.Hosting.AspNetCore
Imports $ext_safeprojectname$.Models
Imports $ext_safeprojectname$.Services

Module Program
    Sub Main(args As String())

        Const connectionStringName As String = "DefaultConnection"
        Const MyAllowSpecificOrigins As String = "_myAllowSpecificOrigins"

        Dim builder = WebApplication.CreateBuilder(args)

        Dim connectionString As String = builder.Configuration.GetConnectionString(connectionStringName)
        If connectionString Is Nothing Then
            Throw New InvalidOperationException($"Connection string '{connectionStringName}' not found.")
        End If

        builder.Services.AddDbContext(Of ApplicationDbContext)(Sub(options) options.$ext_usedatabase$(connectionString))

        builder.Services.AddIdentity(Of User, IdentityRole)().AddEntityFrameworkStores(Of ApplicationDbContext)()

        Dim corsAllowedOrigin = builder.Configuration.GetValue(Of String)("Cors:AllowedOrigin")

        builder.Services.AddCors(Sub(options) options.AddPolicy(MyAllowSpecificOrigins, Sub(policy) policy.WithOrigins(corsAllowedOrigin).WithHeaders(HeaderNames.ContentType).AllowCredentials()))

        builder.Services.AddOpenRiaServices()
        builder.Services.AddTransient(Of UserRegistrationService)()
        builder.Services.AddTransient(Of AuthenticationService)()

        builder.Services.AddHttpContextAccessor()

        builder.Services.AddAuthentication().AddCookie()
        builder.Services.AddAuthorization()

        Dim app = builder.Build()

        app.UseCors(MyAllowSpecificOrigins)

        app.UseAuthentication()
        app.UseAuthorization()

        app.MapOpenRiaServices(Sub(b)
                                   b.AddDomainService(Of UserRegistrationService)()
                                   b.AddDomainService(Of AuthenticationService)()
                               End Sub)

        app.Run()

    End Sub
End Module