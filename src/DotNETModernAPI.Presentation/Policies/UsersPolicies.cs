using Microsoft.AspNetCore.Authorization;

namespace DotNETModernAPI.Presentation.Policies;

internal static class UsersPolicies
{
    public static void AddUsersOptions(this AuthorizationOptions options)
    {
        options.AddPolicy("UsersList", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "users.list")));
        options.AddPolicy("UsersRegister", apb => apb.RequireAssertion(ahc => ahc.User.HasClaim(c => c.Value == "users.register")));
    }
}
