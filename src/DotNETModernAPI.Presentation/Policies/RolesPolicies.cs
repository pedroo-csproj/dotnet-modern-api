using Microsoft.AspNetCore.Authorization;

namespace DotNETModernAPI.Presentation.Policies;

internal static class RolesPolicies
{
    public static void AddRolesOptions(this AuthorizationOptions options)
    {
        options.AddPolicy("RolesListRoles", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "roles.listRoles")));
        options.AddPolicy("RolesListPolicies", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "roles.listPolicies")));
        options.AddPolicy("RolesCreate", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "roles.create")));
        options.AddPolicy("RolesAddClaimsToRole", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "roles.addClaimsToRole")));
    }
}
