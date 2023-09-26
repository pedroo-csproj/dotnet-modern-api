using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Validators;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Validators;

public class RoleValidatorTests
{
    public RoleValidatorTests() =>
        _roleValidator = new RoleValidator();

    private readonly RoleValidator _roleValidator;

    [Fact(DisplayName = "Validate Id is Guid.Empty")]
    public void Validate_IdGuidEmpty_MustReturnIdCantBeGuidEmpty()
    {
        // Arrange
        var role = GenerateRole();
        role.Id = Guid.Empty;

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("Role.Id can't be equal Guid.Empty", validationResult.Errors.First().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Name have less than 4 characters")]
    public void Validate_NameLessThan4Characters_MustReturnNameCantHaveLessThan4Characters()
    {
        // Arrange
        var role = GenerateRole();
        role.Name = role.Name[..3];

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("Role.Name can't have less than 4 characters", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("Role.NormalizedName must be equal Role.Name in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - Name can't be greater than 16 characters")]
    public void Validate_NameGreaterThan16Characters_MustReturnNameCantBeGreaterThan16Characters()
    {
        // Arrange
        var role = GenerateRole();
        role.Name = "MasterAdminFinalVersion";

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("Role.Name can't be greater than 10 characters", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("Role.NormalizedName must be equal Role.Name in UpperCase", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedName is empty")]
    public void Validate_NormalizedNameEmpty_MustReturnNormalizedNameCantBeEmpty()
    {
        // Arrange
        var role = GenerateRole();
        role.NormalizedName = string.Empty;

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Equal(3, validationResult.Errors.Count);
        Assert.Equal("Role.NormalizedName can't be empty", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("Role.NormalizedName must be equal Role.Name in UpperCase", validationResult.Errors[1].ErrorMessage);
        Assert.Equal("Role.NormalizedName can't have less than 4 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedName have less than 4 characters")]
    public void Validate_NormalizedNameLessThan4Characters_MustReturnNormalizedNameCantHaveLessThan4Characters()
    {
        // Arrange
        var role = GenerateRole();
        role.NormalizedName = role.Name[..3];

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("Role.NormalizedName must be equal Role.Name in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("Role.NormalizedName can't have less than 4 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - NormalizedName can't be greater than 16 characters")]
    public void Validate_NormalizedNameGreaterThan16Characters_MustReturnNormalizedNameCantBeGreaterThan16Characters()
    {
        // Arrange
        var role = GenerateRole();
        role.NormalizedName = "MasterAdminFinalVersion".ToUpper();

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Equal("Role.NormalizedName must be equal Role.Name in UpperCase", validationResult.Errors.First().ErrorMessage);
        Assert.Equal("Role.NormalizedName can't be greater than 10 characters", validationResult.Errors.Last().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - ConcurrencyStamp empty")]
    public void Validate_ConcurrencyStampEmpty_MustReturnConcurrencyStampCantBeEmpty()
    {
        // Arrange
        var role = GenerateRole();
        role.ConcurrencyStamp = string.Empty;

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("Role.ConcurrencyStamp can't be empty", validationResult.Errors.First().ErrorMessage);
    }

    [Fact(DisplayName = "Validate - ConcurrencyStamp is Guid.Empty")]
    public void Validate_ConcurrencyStampGuidEmpty_MustReturnConcurrencyStampCantBeGuidEmpty()
    {
        // Arrange
        var role = GenerateRole();
        role.ConcurrencyStamp = Guid.Empty.ToString();

        // Act
        var validationResult = _roleValidator.Validate(role);

        // Assert
        Assert.Single(validationResult.Errors);
        Assert.Equal("Role.ConcurrencyStamp can't be equal Guid.Empty", validationResult.Errors.First().ErrorMessage);
    }

    private static Role GenerateRole() =>
        new("Admin");
}
