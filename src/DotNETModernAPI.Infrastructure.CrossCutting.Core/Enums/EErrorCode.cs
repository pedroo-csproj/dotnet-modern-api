namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;

public enum EErrorCode
{
    NoError = 0,
    UserNameAlreadyTaken = 1,
    EmailAlreadyTaken = 2,
    InvalidEntity = 3,
    IdentityError = 4,
    EmailOrPasswordIncorrect = 5,
    EmailNotFound = 6,
    EmailNotConfirmed = 7,
    RoleNotFound = 8
}
