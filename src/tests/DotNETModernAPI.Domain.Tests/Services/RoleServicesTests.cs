using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Services;

public class RoleServicesTests
{
    public RoleServicesTests()
    {
        var roleStore = new Mock<IRoleStore<Role>>();
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _roleValidator = new Mock<IValidator<Role>>();
        _roleServices = new RoleServices(_roleManager.Object, _roleValidator.Object);
    }

    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IValidator<Role>> _roleValidator;
    private readonly RoleServices _roleServices;

    #region Create

    [Fact(DisplayName = "Create - Role Already Exists")]
    public async void Create_RoleAlreadyExists_MustReturnRoleAlreadyExists()
    {
        // Arrange
        var name = "Admin";
        var role = new Role(name);

        _roleManager.Setup(rm => rm.FindByNameAsync(name)).Returns(Task.FromResult(role));

        // Act
        var result = await _roleServices.Create(name);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.RoleAlreadyExists, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Equal(Guid.Empty, result.Data);

        _roleManager.Verify(rm => rm.FindByNameAsync(name), Times.Once);
        _roleValidator.Verify(rv => rv.Validate(It.IsAny<Role>()), Times.Never);
        _roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Create - Invalid Role for FluentValidation")]
    public async void Create_InvalidRole_MustReturnInvalidEntity()
    {
        // Arrange
        var name = "Admin";
        var role = new Role(name);
        var error = "Role.Name can't be empty";
        var errors = new List<string>() { error };
        var roleValidationResult = new FluentValidation.Results.ValidationResult() { Errors = { new ValidationFailure("Name", error) } };

        _roleManager.Setup(rm => rm.FindByNameAsync(name)).Returns(Task.FromResult((Role)null));
        _roleValidator.Setup(rv => rv.Validate(It.IsAny<Role>())).Returns(roleValidationResult);

        // Act
        var result = await _roleServices.Create(name);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.InvalidEntity, result.ErrorCode);
        Assert.Equal(errors, result.Errors);
        Assert.Equal(Guid.Empty, result.Data);

        _roleManager.Verify(rm => rm.FindByNameAsync(name), Times.Once);
        _roleValidator.Verify(rv => rv.Validate(It.IsAny<Role>()), Times.Once);
        _roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Create - Invalid Role for RoleManager")]
    public async void Create_InvalidRole_MustReturnIdentityError()
    {
        // Arrange
        var name = "Admin";
        var role = new Role(name);
        var roleValidationResult = new ValidationResult();
        var errors = new List<string>() { "Invalid Name" };
        var createRoleResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = errors.First() });

        _roleManager.Setup(rm => rm.FindByNameAsync(name)).Returns(Task.FromResult((Role)null));
        _roleValidator.Setup(rv => rv.Validate(It.IsAny<Role>())).Returns(roleValidationResult);
        _roleManager.Setup(rm => rm.CreateAsync(It.IsAny<Role>())).Returns(Task.FromResult(createRoleResult));

        // Act
        var result = await _roleServices.Create(name);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Equal(errors, result.Errors);
        Assert.Equal(Guid.Empty, result.Data);

        _roleManager.Verify(rm => rm.FindByNameAsync(name), Times.Once);
        _roleValidator.Verify(rv => rv.Validate(It.IsAny<Role>()), Times.Once);
        _roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Once);
    }

    [Fact(DisplayName = "Create - ValidData")]
    public async void Create_ValidData_MustReturnNoError()
    {
        // Arrange
        var name = "Admin";
        var role = new Role(name);
        var roleValidationResult = new ValidationResult();
        var createRoleResult = IdentityResult.Success;

        _roleManager.Setup(rm => rm.FindByNameAsync(name)).Returns(Task.FromResult((Role)null));
        _roleValidator.Setup(rv => rv.Validate(It.IsAny<Role>())).Returns(roleValidationResult);
        _roleManager.Setup(rm => rm.CreateAsync(It.IsAny<Role>())).Returns(Task.FromResult(createRoleResult));

        // Act
        var result = await _roleServices.Create(name);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.NotEqual(Guid.Empty, result.Data);

        _roleManager.Verify(rm => rm.FindByNameAsync(name), Times.Once);
        _roleValidator.Verify(rv => rv.Validate(It.IsAny<Role>()), Times.Once);
        _roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Once);
    }

    #endregion
}
