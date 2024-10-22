using Microsoft.EntityFrameworkCore;
using OpenRiaServices.Hosting.AspNetCore;
using $ext_safeprojectname$.Services;
using $ext_safeprojectname$.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

const string connectionStringName = "DefaultConnection";
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
    ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.$ext_usedatabase$(connectionString));

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var corsAllowedOrigin = builder.Configuration.GetValue<string>("Cors:AllowedOrigin");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                            .WithOrigins(corsAllowedOrigin)
                            .WithHeaders(HeaderNames.ContentType)
                            .AllowCredentials();
                      });
});

builder.Services.AddOpenRiaServices();
builder.Services.AddTransient<UserRegistrationService>();
builder.Services.AddTransient<AuthenticationService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenRiaServices(builder =>
{
    builder.AddDomainService<UserRegistrationService>();
    builder.AddDomainService<AuthenticationService>();
});

app.Run();