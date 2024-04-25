using Microsoft.AspNetCore.Identity;
using OpenRiaServices.Server;
using OpenRiaServices.Server.Authentication;
using $ext_safeprojectname$.Models;

namespace $ext_safeprojectname$.Services;

[EnableClientAccess]
public class AuthenticationService : DomainService, IAuthentication<User>
{
    public User Login(string userName, string password, bool isPersistent, string customData)
    {
        var signInManager = ServiceContext.GetRequiredService<SignInManager<User>>();

        var response = signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure: false)
            .GetAwaiter().GetResult();

        if (response.Succeeded)
        {
            return GetAuthenticatedUser(userName);
        }
        else if (response.IsLockedOut)
        {
            throw new NotImplementedException();
        }
        else if (response.RequiresTwoFactor)
        {
            // customdata can be used to pass 2fa token
            throw new NotImplementedException();
        }

        return null;
    }

    private User GetAnonymousUser() => new() { Name = string.Empty, Roles = [] };

    private User GetAuthenticatedUser(string userName)
    {
        var userManager = ServiceContext.GetRequiredService<UserManager<User>>();

        var user = userManager.FindByNameAsync(userName)
            .GetAwaiter().GetResult();

        if (user == null) return null;

        return new User()
        {
            Name = user.UserName,
            Roles = user.Roles,
            PasswordAnswer = user.PasswordAnswer,
            PasswordQuestion = user.PasswordQuestion
        };
    }

    public User Logout()
    {
        var signInManager = ServiceContext.GetRequiredService<SignInManager<User>>();

        signInManager.SignOutAsync()
            .GetAwaiter().GetResult();

        return GetAnonymousUser();
    }

    public User GetUser()
    {
        var identity = ServiceContext.GetRequiredService<IHttpContextAccessor>().HttpContext.User.Identity;

        return identity?.IsAuthenticated == true
            ? GetAuthenticatedUser(identity?.Name)
            : GetAnonymousUser();
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }
}