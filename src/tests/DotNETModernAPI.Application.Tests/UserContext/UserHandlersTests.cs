using Bogus;
using DotNETModernAPI.Application.UserContext;
using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DotNETModernAPI.Application.Tests.UserContext;

public class UserHandlersTests
{
    public UserHandlersTests()
    {
        var store = new Mock<IUserStore<User>>();
        _userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        _userValidator = new Mock<IValidator<User>>();
        _userServices = new Mock<UserServices>(_userManager.Object, _userValidator.Object);
        _userHandlers = new UserHandlers(_userServices.Object);
    }

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IValidator<User>> _userValidator;
    private readonly Mock<UserServices> _userServices;
    private readonly UserHandlers _userHandlers;

    [Fact(DisplayName = "Register - Valid Command")]
    public async void Register_ValidCommand_MustReturnNoError()
    {
        // Arrange
        RegisterUserCommandRequest commandRequest = new Faker<RegisterUserCommandRequest>().CustomInstantiator(f => new RegisterUserCommandRequest(f.Internet.UserName(), f.Internet.Email(), f.Internet.Password()));

        _userServices.Setup(us => us.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password)).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _userHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _userServices.Verify(us => us.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password), Times.Once);
    }
}
