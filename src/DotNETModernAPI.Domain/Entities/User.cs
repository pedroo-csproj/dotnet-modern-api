using DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User(string userName, string email)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        NormalizedUserName = userName.ToUpper();
        Email = email;
        NormalizedEmail = email.ToUpper();
        EmailConfirmed = true;
    }

    public override Guid Id { get; set; }
    public override string UserName { get; set; }
    public override string NormalizedUserName { get; set; }
    public override string Email { get; set; }
    public override string NormalizedEmail { get; set; }
    public override bool EmailConfirmed { get; set; }
    public override string PasswordHash { get; set; }
    public override string SecurityStamp { get; set; }
    public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public override string PhoneNumber { get => string.Empty; set => throw new UnusedIdentityFieldException(nameof(PhoneNumber)); }
    public override bool PhoneNumberConfirmed { get => false; set => throw new UnusedIdentityFieldException(nameof(PhoneNumberConfirmed)); }
    public override bool TwoFactorEnabled { get; set; }
    public override DateTimeOffset? LockoutEnd { get; set; }
    public override bool LockoutEnabled { get; set; }
    public override int AccessFailedCount { get; set; }

    public void UpdateUserName(string newUserName)
    {
        UserName = newUserName;
        NormalizedUserName = UserName.ToUpper();
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail;
        NormalizedEmail = Email.ToUpper();
    }
}
