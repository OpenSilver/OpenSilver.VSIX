using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using OpenRiaServices.Server.Authentication;

namespace $ext_safeprojectname$.Models;

public partial class User : IdentityUser, IUser
{
    [Key]
    public string Name { get; set; }

    [NotMapped]
    public IEnumerable<string> Roles { get; set; } = [];

    public string FriendlyName { get; set; }

    public string PasswordAnswer { get; set; }
    public string PasswordQuestion { get; set; }
}