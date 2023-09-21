namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;

public class RoleWithoutClaimsException : Exception
{
    public RoleWithoutClaimsException(string roleName) : base($"An attempt to retrieve claims of a Role named \"{roleName}\" happened") { }
}
