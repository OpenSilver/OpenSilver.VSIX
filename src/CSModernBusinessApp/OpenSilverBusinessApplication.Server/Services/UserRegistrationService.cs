using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OpenRiaServices.Server;
using $ext_safeprojectname$.Models;

namespace $ext_safeprojectname$.Services;

[EnableClientAccess]
public class UserRegistrationService : DomainService
{
    [Invoke(HasSideEffects = true)]
    public IEnumerable<string> CreateUser(RegistrationData user,
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^.*[^a-zA-Z0-9].*$", ErrorMessage = "A password needs to contain at least one special character e.g. @ or #")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Password must be at least 7 and at most 50 characters long")]
        string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userManager = ServiceContext.GetRequiredService<UserManager<User>>();

        var appUser = new User()
        {
            UserName = user.UserName,
            Email = user.Email,
            FriendlyName = user.FriendlyName,
            PasswordQuestion = user.Question,
        };
        appUser.PasswordAnswer = userManager.PasswordHasher.HashPassword(appUser, user.Answer);

        var result = userManager.CreateAsync(appUser, password)
            .GetAwaiter().GetResult();

        if (!result.Succeeded)
        {
            return result.Errors.Select(x => x.Description);
        }

        return [];
    }
}
