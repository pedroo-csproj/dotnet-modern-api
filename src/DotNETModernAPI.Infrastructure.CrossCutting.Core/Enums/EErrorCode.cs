namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;

public enum EErrorCode
{
    NoError = 0,
    UserNameAlreadyTaken = 1,
    EmailAlreadyTaken = 2,
    InvalidEntity = 3,
    IdentityError = 4
}
