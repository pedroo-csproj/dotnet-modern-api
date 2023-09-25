using Bogus;
using DotNETModernAPI.Application.RoleContext;
using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DotNETModernAPI.Application.Tests.RoleContext;

public class RoleHandlersTests
{
    public RoleHandlersTests()
    {
        var roleStore = new Mock<IRoleStore<Role>>();
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _roleValidator = new Mock<IValidator<Role>>();
        _roleServices = new Mock<RoleServices>(_roleManager.Object, _roleValidator.Object);
        _roleHandlers = new RoleHandlers(_roleServices.Object);
    }

    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IValidator<Role>> _roleValidator;
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

    private static CreateRoleCommandRequest GenerateCreateRoleCommandRequest() =>
        new("Admin");
}
