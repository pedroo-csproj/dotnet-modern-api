using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;

namespace DotNETModernAPI.Presentation.Policies;

internal static class PoliciesConfiguration
{
    public static void AddPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PolicyDTO>(es => configuration.GetSection("Policies").Bind(es));

        services.AddAuthorization(ao => ao.AddRolesOptions());
        services.AddAuthorization(ao => ao.AddUsersOptions());
    }
}
