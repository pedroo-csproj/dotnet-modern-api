using Bogus;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Validators;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Validators;

public class UserValidatorTests
{
    public UserValidatorTests()
    {
        _userValidator = new UserValidator();
        _faker = new Faker();
    }

    private readonly UserValidator _userValidator;
    private readonly Faker _faker;

    [Fact(DisplayName = "Validate - Id is Guid.Empty")]
    public void Validate_IdGuidEmpty_MustReturnIdCantBeGuidEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.Id = Guid.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("User.Id can't be equal Guid.Empty", validationResult.Errors.First().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - UserName is empty")]
    public void Validate_UserNameEmpty_MustReturnUserNameCantBeEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.UserName = string.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(3, validationResult.Errors.Count);
        Assert.Equal("User.UserName can't be empty", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.UserName can't have less than 4 characters", validationResult.Errors[1].ErrorMessage);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - UserName have less than 4 characters")]
    public void Validate_UserNameLessThan4Characters_MustReturnUserNameCantHaveLessThan4Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.UserName = _faker.Internet.UserName()[..3];

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.UserName can't have less than 4 characters", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - UserName can't be greater than 16 characters")]
    public void Validate_UserNameGreaterThan16Characters_MustReturnUserNameCantBeGreaterThan16Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.UserName = _faker.Internet.UserName() + _faker.Internet.UserName() + _faker.Internet.UserName();

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.UserName can't be greater than 16 characters", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedUserName is empty")]
    public void Validate_NormalizedUserNameEmpty_MustReturnNormalizedUserNameCantBeEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedUserName = string.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(3, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedUserName can't be empty", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors[1].ErrorMessage);
        Assert.Equal("User.NormalizedUserName can't have less than 4 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedUserName have less than 4 characters")]
    public void Validate_NormalizedUserNameLessThan4Characters_MustReturnNormalizedUserNameCantHaveLessThan4Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedUserName = _faker.Internet.UserName()[..3];

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedUserName can't have less than 4 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedUserName can't be greater than 16 characters")]
    public void Validate_NormalizedUserNameGreaterThan16Characters_MustReturnNormalizedUserNameCantBeGreaterThan16Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedUserName = _faker.Internet.UserName() + _faker.Internet.UserName() + _faker.Internet.UserName();

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedUserName must be equal User.UserName in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedUserName can't be greater than 16 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Email is empty")]
    public void Validate_EmailEmpty_MustReturnEmailCantBeEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.Email = string.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(3, validationResult.Errors.Count);
        Assert.Equal("User.Email can't be empty", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.Email must be a valid email", validationResult.Errors[1].ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Invalid Email")]
    public void Validate_InvalidEmail_MustReturnEmailMustBeValidEmail()
    {
        // Arrange
        var user = GenerateUser();
        user.Email = user.Email.Replace("@", string.Empty);

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.Email must be a valid email", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Email greater than 320 characters")]
    public void AValidate_EmailGreaterThan320Characters_MustReturnEmailCantHaveLessThan4Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.Email = $"{string.Concat(Enumerable.Repeat("a", 321))}@email.com";

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.Email can't be greater than 320 characters", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedEmail is empty")]
    public void Validate_NormalizedEmailEmpty_MustReturnNormalizedEmailCantBeEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedEmail = string.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(3, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedEmail can't be empty", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors[1].ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be a valid email", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Invalid NormalizedEmail")]
    public void Validate_InvalidNormalizedEmail_MustReturnNormalizedEmailMustBeValidEmail()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedEmail = user.NormalizedEmail.Replace("@", string.Empty);

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedEmail must be a valid email", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedEmail different from Email")]
    public void Validate_NormalizedEmailDifferentFromEmail_MustReturnNormalizedEmailDifferentFromEmail()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedEmail = _faker.Internet.Email();

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.First().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedEmail greater than 320 characters")]
    public void Validate_NormalizedEmailGreaterThan320Characters_MustReturnNormalizedEmailCantHaveLessThan4Characters()
    {
        // Arrange
        var user = GenerateUser();
        user.NormalizedEmail = $"{string.Concat(Enumerable.Repeat("a", 321))}@email.com";

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("User.NormalizedEmail can't be greater than 320 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - ConcurrencyStamp empty")]
    public void Validate_ConcurrencyStampEmpty_MustReturnConcurrencyStampCantBeEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.ConcurrencyStamp = string.Empty;

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("User.ConcurrencyStamp can't be empty", validationResult.Errors.First().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - ConcurrencyStamp is Guid.Empty")]
    public void Validate_ConcurrencyStampGuidEmpty_MustReturnConcurrencyStampCantBeGuidEmpty()
    {
        // Arrange
        var user = GenerateUser();
        user.ConcurrencyStamp = Guid.Empty.ToString();

        // Act
        var validationResult = _userValidator.Validate(user);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("User.ConcurrencyStamp can't be equal Guid.Empty", validationResult.Errors.First().ErrorMessage);
    }

    private User GenerateUser()
    {
        var userName = _faker.Internet.UserName();

        if (userName.Length >= 16)
            userName = userName[..16];

        return new Faker<User>().CustomInstantiator(f => new User(userName, f.Internet.Email()));
    }
}
