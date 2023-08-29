using Bogus;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Entities;

public class UserTests
{
    [Fact(DisplayName = "Instance New - ValidData")]
    public void InstanceNew_ValidData_MustReturnANewUserWithTheProvidedData()
    {
        // Arrange
        var userName = new Faker().Internet.UserName();
        var email = new Faker().Internet.Email();

        // Act
        var user = new User(userName, email);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(userName, user.UserName);
        Assert.Equal(userName.ToUpper(), user.NormalizedUserName);
        Assert.Equal(email, user.Email);
        Assert.Equal(email.ToUpper(), user.NormalizedEmail);
        Assert.False(user.EmailConfirmed);
        Assert.Null(user.PasswordHash);
        Assert.Null(user.SecurityStamp);
        Assert.NotEqual(Guid.Empty.ToString(), user.ConcurrencyStamp);
        Assert.NotNull(user.ConcurrencyStamp);
        Assert.NotEmpty(user.ConcurrencyStamp);
        Assert.Empty(user.PhoneNumber);
        Assert.False(user.PhoneNumberConfirmed);
        Assert.False(user.TwoFactorEnabled);
        Assert.Null(user.LockoutEnd);
        Assert.False(user.LockoutEnabled);
        Assert.Equal(0, user.AccessFailedCount);
    }

    [Fact(DisplayName = "UpdateUserName - Valid Data")]
    public void UpdateUserName_ValidData_MustUpdateUserNameToTheProvidedUserName()
    {
        // Arrange
        var user = GenerateUser();
        var userName = new Faker().Internet.UserName();

        // Act
        user.UpdateUserName(userName);

        // Assert
        Assert.Equal(userName, user.UserName);
        Assert.Equal(userName.ToUpper(), user.NormalizedUserName);
    }

    [Fact(DisplayName = "UpdateEmail - Valid Data")]
    public void UpdateEmail_ValidData_MustUpdateEmailToTheProvidedEmail()
    {
        // Arrange
        var user = GenerateUser();
        var email = new Faker().Internet.Email();

        // Act
        user.UpdateEmail(email);

        // Assert
        Assert.Equal(email, user.Email);
        Assert.Equal(email.ToUpper(), user.NormalizedEmail);
    }

    [Fact(DisplayName = "SetPhoneNumber - Valid Data")]
    public void SetPhoneNumber_ValidData_MustThrowUnusedIdentityFieldException()
    {
        // Arrange
        var user = GenerateUser();

        // Act
        var exception = Assert.Throws<UnusedIdentityFieldException>(() => user.PhoneNumber = new Faker().Phone.PhoneNumber());

        // Assert
        Assert.Equal($"Field \"{nameof(user.PhoneNumber)}\" aren't supose to be used", exception.Message);
    }

    [Fact(DisplayName = "SetPhoneNumberConfirmed - Valid Data")]
    public void SetPhoneNumberConfirmed_ValidData_MustThrowUnusedIdentityFieldException()
    {
        // Arrange
        var user = GenerateUser();

        // Act
        var exception = Assert.Throws<UnusedIdentityFieldException>(() => user.PhoneNumberConfirmed = !user.PhoneNumberConfirmed);

        // Assert
        Assert.Equal($"Field \"{nameof(user.PhoneNumberConfirmed)}\" aren't supose to be used", exception.Message);
    }

    private static User GenerateUser() =>
        new Faker<User>().CustomInstantiator(f => new User(f.Internet.UserName(), f.Internet.Email()));
}
