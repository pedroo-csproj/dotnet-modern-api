using AutoMapper;
using Bogus;
using DotNETModernAPI.Application.UserContext;
using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Application.UserContext.Queries.Requests;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Domain.Views;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DotNETModernAPI.Application.Tests.UserContext;

public class UserHandlersTests
{
    public UserHandlersTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        var roleStore = new Mock<IRoleStore<Role>>();
        _userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _userRepository = new Mock<IUserRepository>();
        _emailProvider = new Mock<IEmailProvider>();
        _userValidator = new Mock<IValidator<User>>();
        _userServices = new Mock<UserServices>(_userManager.Object, _roleManager.Object, _userRepository.Object, _emailProvider.Object, _userValidator.Object);
        _mapper = new Mock<IMapper>();
        _userHandlers = new UserHandlers(_userServices.Object, _mapper.Object);
        _faker = new Faker();
    }

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IValidator<User>> _userValidator;
    private readonly Mock<UserServices> _userServices;
    private readonly Mock<IMapper> _mapper;
    private readonly UserHandlers _userHandlers;
    private readonly Faker _faker;

    [Fact(DisplayName = "List - Valid Command")]
    public async void List_ValidCommand_MustReturnNoError()
    {
        // Arrange
        var queryRequest = new ListUsersQueryRequest();

        var roles = new List<RoleVO>() { new RoleVO(Guid.NewGuid(), "Admin") };
        var listResult = new ResultWrapper<IList<UserRolesView>>(new List<UserRolesView>() { new UserRolesView(Guid.NewGuid(), _faker.Internet.UserName(), _faker.Internet.Email(), true, roles) });

        _userServices.Setup(us => us.List()).Returns(Task.FromResult(listResult));
        _mapper.Setup(m => m.Map<IEnumerable<ListUsersQueryResponse>>(listResult.Data)).Returns(new List<ListUsersQueryResponse>());

        // Act
        var handleResult = await _userHandlers.Handle(queryRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.NotNull(handleResult.Data);

        _userServices.Verify(us => us.List(), Times.Once);
        _mapper.Verify(m => m.Map<IEnumerable<ListUsersQueryResponse>>(listResult.Data), Times.Once);
    }

    [Fact(DisplayName = "Authenticate - Valid Command")]
    public async void Authenticate_ValidCommand_MustReturnNoError()
    {
        // Arrange
        AuthenticateUserCommandRequest commandRequest = new Faker<AuthenticateUserCommandRequest>().CustomInstantiator(f => new AuthenticateUserCommandRequest(f.Internet.Email(), f.Internet.Password()));
        var claims = new List<Claim>() { new Claim("aud", "http://localhost:3000") };

        _userServices.Setup(us => us.Authenticate(commandRequest.Email, commandRequest.Password)).Returns(Task.FromResult(new ResultWrapper<IList<Claim>>(claims)));

        // Act
        var handleResult = await _userHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);
        Assert.Equal(claims, handleResult.Data);

        _userServices.Verify(us => us.Authenticate(commandRequest.Email, commandRequest.Password), Times.Once);
    }

    [Fact(DisplayName = "Register - Valid Command")]
    public async void Register_ValidCommand_MustReturnNoError()
    {
        // Arrange
        RegisterUserCommandRequest commandRequest = new Faker<RegisterUserCommandRequest>().CustomInstantiator(f => new RegisterUserCommandRequest(f.Internet.UserName(), f.Internet.Email(), f.Internet.Password(), Guid.NewGuid()));

        _userServices.Setup(us => us.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password, commandRequest.RoleId.ToString())).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _userHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _userServices.Verify(us => us.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password, commandRequest.RoleId.ToString()), Times.Once);
    }

    [Fact(DisplayName = "ConfirmEmail - Valid Command")]
    public async void ConfirmEmail_ValidCommand_MustReturnNoError()
    {
        // Arrange
        ConfirmEmailCommandRequest commandRequest = new Faker<ConfirmEmailCommandRequest>().CustomInstantiator(f => new ConfirmEmailCommandRequest(f.Internet.Email(), Guid.NewGuid().ToString()));

        _userServices.Setup(us => us.ConfirmEmail(commandRequest.Email, commandRequest.EmailConfirmationToken)).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _userHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _userServices.Verify(us => us.ConfirmEmail(commandRequest.Email, commandRequest.EmailConfirmationToken), Times.Once);
    }

    [Fact(DisplayName = "RequestPasswordReset - Valid Command")]
    public async void RequestPasswordReset_ValidCommand_MustReturnNoError()
    {
        // Arrange
        RequestPasswordResetCommandRequest commandRequest = new Faker<RequestPasswordResetCommandRequest>().CustomInstantiator(f => new RequestPasswordResetCommandRequest(f.Internet.Email()));

        _userServices.Setup(us => us.RequestPasswordReset(commandRequest.Email)).Returns(Task.FromResult(new ResultWrapper()));

        // Act
        var handleResult = await _userHandlers.Handle(commandRequest, default);

        // Assert
        Assert.True(handleResult.Success);
        Assert.Equal(EErrorCode.NoError, handleResult.ErrorCode);
        Assert.Empty(handleResult.Errors);

        _userServices.Verify(us => us.RequestPasswordReset(commandRequest.Email), Times.Once);
    }
}
