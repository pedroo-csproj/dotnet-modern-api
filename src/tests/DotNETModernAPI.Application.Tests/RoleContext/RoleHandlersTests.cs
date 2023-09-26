using DotNETModernAPI.Application.RoleContext;
using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DotNETModernAPI.Application.Tests.RoleContext;

public class RoleHandlersTests
{
    public RoleHandlersTests()
    {
        var roleStore = new Mock<IRoleStore<Role>>();
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _roleValidator = new Mock<IValidator<Role>>();
        _policies = new Mock<IOptions<PoliciesDTO>>();
        _roleServices = new Mock<RoleServices>(_roleManager.Object, _roleValidator.Object, _policies.Object);
        _roleHandlers = new RoleHandlers(_roleServices.Object);
    }

    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IValidator<Role>> _roleValidator;
    private readonly Mock<IOptions<PoliciesDTO>> _policies;
    private readonly Mock<RoleServices> _roleServices;
    private readonly RoleHandlers _roleHandlers;

    [Fact(DisplayName = "Create - Invalid Data")]
    public async void Create_InvalidData_MustReturnError()
    {
        // Arrange
        var commandRequest = GenerateCreateRoleCommandRequest();
        var errorCode = EErrorCode.RoleAlreadyExists;

        _roleServices.Setup(rs => rs.Create(commandRequest.Name)).Returns(Task.FromResult(new ResultWrapper<Guid>(errorCode)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.False(handleResult.Success);
        Assert.Equal(errorCode, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Null(handleResult.Data);

        _roleServices.Verify(rs => rs.Create(commandRequest.Name), Times.Once);
    }

    [Fact(DisplayName = "Create - Valid Data")]
    public async void Create_ValidData_MustReturnNoError()
    {
        // Arrange
        var commandRequest = GenerateCreateRoleCommandRequest();
        var roleId = Guid.NewGuid();

        _roleServices.Setup(rs => rs.Create(commandRequest.Name)).Returns(Task.FromResult(new ResultWrapper<Guid>(roleId)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Equal(roleId, handleResult.Data.Id);

        _roleServices.Verify(rs => rs.Create(commandRequest.Name), Times.Once);
    }

    [Fact(DisplayName = "AddClaimsToRole - Invalid Data")]
    public async void AddClaimsToRole_InvalidData_MustReturnInvalidPolicy()
    {
        // Arrange
        var claims = new List<string>() { "roles.listPolicies", "roles.create" };
        var commandRequest = new AddClaimsToRoleCommandRequest(Guid.NewGuid(), claims);

        _roleServices.Setup(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>())).Returns(Task.FromResult(new ResultWrapper(EErrorCode.InvalidPolicy)));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.False(handleResult.Success);
        Assert.Equal(EErrorCode.InvalidPolicy, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _roleServices.Verify(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact(DisplayName = "AddClaimsToRole - Valid Data")]
    public async void AddClaimsToRole_ValidData_MustReturnNoError()
    {
        // Arrange
        var claims = new List<string>() { "roles.listPolicies", "roles.create" };
        var commandRequest = new AddClaimsToRoleCommandRequest(Guid.NewGuid(), claims);

        _roleServices.Setup(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>())).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _roleHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _roleServices.Verify(rs => rs.AddClaimsToRole(commandRequest.Id.ToString(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    private static CreateRoleCommandRequest GenerateCreateRoleCommandRequest() =>
        new("Admin");
}
