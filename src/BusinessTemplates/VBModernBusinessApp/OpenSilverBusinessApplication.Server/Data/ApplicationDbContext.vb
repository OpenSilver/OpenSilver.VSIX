Imports Microsoft.AspNetCore.Identity.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore
Imports $ext_safeprojectname$.Models

Public Class ApplicationDbContext
    Inherits IdentityDbContext(Of User)

    Public Sub New(options As DbContextOptions(Of ApplicationDbContext))
        MyBase.New(options)
    End Sub

End Class