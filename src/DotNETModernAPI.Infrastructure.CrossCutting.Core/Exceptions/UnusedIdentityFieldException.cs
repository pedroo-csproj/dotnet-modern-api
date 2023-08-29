namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;

public class UnusedIdentityFieldException : Exception
{
    public UnusedIdentityFieldException(string fieldName) : base($"Field \"{fieldName}\" aren't supose to be used") { }
}
