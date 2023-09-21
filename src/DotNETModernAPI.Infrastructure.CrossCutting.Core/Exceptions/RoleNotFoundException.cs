namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string roleName) : base($"The Role \"{roleName}\" doesn't exists") { }
}
