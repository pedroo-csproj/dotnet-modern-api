namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;

public class UserDoesntHaveRolesException : Exception
{
    public UserDoesntHaveRolesException() : base($"An User must have at least one Role") { }
}
