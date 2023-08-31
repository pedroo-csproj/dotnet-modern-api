using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class IdentityConfiguration
{
    public static void AddIdentity(this IServiceCollection services) =>
        services.AddIdentityCore<User>(io =>
        {
            io.User.RequireUniqueEmail = true;
            io.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnm_";
            io.Password.RequireDigit = false;
            io.Password.RequiredLength = 6;
            io.Password.RequiredUniqueChars = 0;
            io.Password.RequireLowercase = false;
            io.Password.RequireNonAlphanumeric = false;
            io.Password.RequireUppercase = false;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDataContext>();
}
