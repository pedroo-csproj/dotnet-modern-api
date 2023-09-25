namespace DotNETModernAPI.Presentation.Policies;

internal static class PoliciesConfiguration
{
    public static void AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(ao => ao.AddRolesOptions());
        services.AddAuthorization(ao => ao.AddUsersOptions());
    }
}
