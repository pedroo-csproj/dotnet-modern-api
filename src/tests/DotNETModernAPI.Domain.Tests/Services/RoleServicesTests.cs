using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Services;

public class RoleServicesTests
{
    public RoleServicesTests()
    {
        var roleStore = new Mock<IRoleStore<Role>>();
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _roleValidator = new Mock<IValidator<Role>>();
        _policies = Options.Create<PoliciesDTO>(new PoliciesDTO()
        {
            Users = new List<string>() { "users.list", "users.register" },
            Roles = new List<string>() { "roles.insertPolicy", "roles.listPolicies", "roles.create", "roles.retrievePolicy" }
        });
        _roleServices = new RoleServices(_roleManager.Object, _roleValidator.Object, _policies);
    }

    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IValidator<Role>> _roleValidator;
    private readonly IOptions<PoliciesDTO> _policies;
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
        var roleValidationResult = new ValidationResult() { Errors = { new ValidationFailure("Name", error) } };

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

    #region AddClaimsToRole

    [Fact(DisplayName = "AddClaimsToRole - Role Doesn't Exists")]
    public async void AddClaimsToRole_RoleDoesntExists_MustReturnRoleNotFound()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var claims = new List<Claim>() { new Claim("policy", "roles.listPolicies"), new Claim("policy", "roles.create") };

        _roleManager.Setup(rm => rm.FindByIdAsync(id)).Returns(Task.FromResult((Role)null));

        // Act
        var result = await _roleServices.AddClaimsToRole(id, claims);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.RoleNotFound, result.ErrorCode);
        Assert.Empty(result.Errors);

        _roleManager.Verify(rm => rm.FindByIdAsync(id), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
        _roleManager.Verify(rm => rm.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>()), Times.Never);
    }

    [Fact(DisplayName = "AddClaimsToRole - Policy Not Allowed")]
    public async void AddClaimsToRole_PolicyNotAllowed_MustReturnInvalidPolicy()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var toAddClaims = new List<Claim>() { new Claim("policy", "roles.generatePolicy"), new Claim("policy", "roles.retrievePolicy") };
        var claims = GenerateClaims();
        var role = new Role("Admin");

        _roleManager.Setup(rm => rm.FindByIdAsync(id)).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult(claims));

        // Act
        var result = await _roleServices.AddClaimsToRole(id, toAddClaims);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.InvalidPolicy, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(result.Errors.First(), toAddClaims.First().Value);

        _roleManager.Verify(rm => rm.FindByIdAsync(id), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(role), Times.Once);
        _roleManager.Verify(rm => rm.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>()), Times.Never);
    }

    [Fact(DisplayName = "AddClaimsToRole - Claim Already Assigned")]
    public async void AddClaimsToRole_ClaimAlreadyAssigned_MustReturnPolicyAlreadyAssignedToRole()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var toAddClaims = new List<Claim>() { new Claim("policy", "roles.listPolicies"), new Claim("policy", "roles.create") };
        var claims = GenerateClaims();
        var role = new Role("Admin");

        _roleManager.Setup(rm => rm.FindByIdAsync(id)).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult((IList<Claim>)claims));

        // Act
        var result = await _roleServices.AddClaimsToRole(id, toAddClaims);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.PolicyAlreadyAssignedToRole, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(result.Errors.First(), claims.First().Value);

        _roleManager.Verify(rm => rm.FindByIdAsync(id), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(role), Times.Once);
        _roleManager.Verify(rm => rm.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>()), Times.Never);
    }

    [Fact(DisplayName = "AddClaimsToRole - Invalid Role for RoleManager")]
    public async void AddClaimsToRole_InvalidRole_MustReturnPolicyAlreadyAssignedToRole()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var toAddClaims = new List<Claim>() { new Claim("policy", "roles.insertPolicy"), new Claim("policy", "roles.retrievePolicy") };
        var claims = GenerateClaims();
        var role = new Role("Admin");
        var errors = new List<string>() { "Invalid Name" };
        var addClaimResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = errors.First() });

        _roleManager.Setup(rm => rm.FindByIdAsync(id)).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult((IList<Claim>)claims));
        _roleManager.Setup(rm => rm.AddClaimAsync(role, toAddClaims.First())).Returns(Task.FromResult(addClaimResult));

        // Act
        var result = await _roleServices.AddClaimsToRole(id, toAddClaims);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(result.Errors.First(), errors.First());

        _roleManager.Verify(rm => rm.FindByIdAsync(id), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(role), Times.Once);
        _roleManager.Verify(rm => rm.AddClaimAsync(role, toAddClaims.First()), Times.Once);
    }

    [Fact(DisplayName = "AddClaimsToRole - Valid Data")]
    public async void AddClaimsToRole_ValidData_MustReturnNoError()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var toAddClaims = new List<Claim>() { new Claim("policy", "roles.insertPolicy"), new Claim("policy", "roles.retrievePolicy") };
        var claims = GenerateClaims();
        var role = new Role("Admin");
        var addClaimResult = IdentityResult.Success;

        _roleManager.Setup(rm => rm.FindByIdAsync(id)).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult((IList<Claim>)claims));
        _roleManager.Setup(rm => rm.AddClaimAsync(role, toAddClaims.First())).Returns(Task.FromResult(addClaimResult));
        _roleManager.Setup(rm => rm.AddClaimAsync(role, toAddClaims.Last())).Returns(Task.FromResult(addClaimResult));

        // Act
        var result = await _roleServices.AddClaimsToRole(id, toAddClaims);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);

        _roleManager.Verify(rm => rm.FindByIdAsync(id), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(role), Times.Once);
        _roleManager.Verify(rm => rm.AddClaimAsync(role, toAddClaims.First()), Times.Once);
    }

    #endregion

    private IList<Claim> GenerateClaims() =>
        new List<Claim>() { new Claim("policy", "roles.listPolicies"), new Claim("policy", "roles.create") };
}
