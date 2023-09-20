using Bogus;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Models;
using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DotNETModernAPI.Domain.Tests.Services;

public class UserServicesTests
{
    public UserServicesTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        var roleStore = new Mock<IRoleStore<Role>>();
        _userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
        _roleManager = new Mock<RoleManager<Role>>(roleStore.Object, null, null, null, null);
        _emailProvider = new Mock<IEmailProvider>();
        _userValidator = new Mock<IValidator<User>>();
        _userServices = new UserServices(_userManager.Object, _roleManager.Object, _emailProvider.Object, _userValidator.Object);
        _faker = new Faker();
    }

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IValidator<User>> _userValidator;
    private readonly UserServices _userServices;
    private readonly Faker _faker;

    [Fact(DisplayName = "Register - UserName already taken")]
    public async void Register_UserNameAlreadyTaken_MustReturnUserNameAlreadyTaken()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roleId = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult(GenerateUser()));

        // Act
        var result = await _userServices.Register(userName, email, password, roleId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.UserNameAlreadyTaken, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Never);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Never);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Email already taken")]
    public async void Register_EmailAlreadyTaken_MustReturnEmailAlreadyTaken()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roleId = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult(GenerateUser()));

        // Act
        var result = await _userServices.Register(userName, email, password, roleId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailAlreadyTaken, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Never);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Invalid User for FluentValidation")]
    public async void Register_InvalidUser_MustReturnInvalidEntity()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roleId = Guid.NewGuid().ToString();
        var error = "User.Email can't be empty";
        var errors = new List<string>() { error };
        var userValidationResult = new FluentValidation.Results.ValidationResult() { Errors = { new ValidationFailure("Email", error) } };

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));
        _userValidator.Setup(uv => uv.Validate(It.IsAny<User>())).Returns(userValidationResult);

        // Act
        var result = await _userServices.Register(userName, email, password, roleId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.InvalidEntity, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(errors, result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Once);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Invalid User for Identity")]
    public async void Register_InvalidUser_MustReturnIdentityError()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roleId = Guid.NewGuid().ToString();
        var error = "Invalid password pattern";
        var errors = new List<string>() { error };
        var userValidationResult = new FluentValidation.Results.ValidationResult();
        var createUserResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = error });

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));
        _userValidator.Setup(uv => uv.Validate(It.IsAny<User>())).Returns(userValidationResult);
        _userManager.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).Returns(Task.FromResult(createUserResult));

        // Act
        var result = await _userServices.Register(userName, email, password, roleId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(errors, result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Once);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
        _roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Invalid Role")]
    public async void Register_InvalidRole_MustReturnRoleNotFound()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roleId = Guid.NewGuid().ToString();
        var userValidationResult = new FluentValidation.Results.ValidationResult();
        var createUserResult = IdentityResult.Success;

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));
        _userValidator.Setup(uv => uv.Validate(It.IsAny<User>())).Returns(userValidationResult);
        _userManager.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).Returns(Task.FromResult(createUserResult));
        _roleManager.Setup(rm => rm.FindByIdAsync(roleId)).Returns(Task.FromResult((Role)null));

        // Act
        var result = await _userServices.Register(userName, email, password, roleId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.RoleNotFound, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Once);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
        _roleManager.Verify(rm => rm.FindByIdAsync(roleId), Times.Once);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Error on Assign Role")]
    public async void Register_ErrorOnAssignRole_MustReturnIdentityError()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var role = new Role("Admin");
        var userValidationResult = new FluentValidation.Results.ValidationResult();
        var createUserResult = IdentityResult.Success;
        var errors = new List<string>() { "Invalid Role" };
        var assignRoleResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = errors.First() });

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));
        _userValidator.Setup(uv => uv.Validate(It.IsAny<User>())).Returns(userValidationResult);
        _userManager.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).Returns(Task.FromResult(createUserResult));
        _roleManager.Setup(rm => rm.FindByIdAsync(role.Id.ToString())).Returns(Task.FromResult(role));
        _userManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role.Name)).Returns(Task.FromResult(assignRoleResult));

        // Act
        var result = await _userServices.Register(userName, email, password, role.Id.ToString());

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Single(result.Errors);
        Assert.Equal(errors, result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Once);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
        _roleManager.Verify(rm => rm.FindByIdAsync(role.Id.ToString()), Times.Once);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), role.Name), Times.Once);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "Register - Valid Data")]
    public async void Register_ValidData_MustReturnNoError()
    {
        // Arrange
        var userName = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var role = new Role("Admin");
        var userValidationResult = new FluentValidation.Results.ValidationResult();
        var createUserResult = IdentityResult.Success;
        var assignRoleResult = IdentityResult.Success;

        _userManager.Setup(um => um.FindByNameAsync(userName)).Returns(Task.FromResult((User)null));
        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));
        _userValidator.Setup(uv => uv.Validate(It.IsAny<User>())).Returns(userValidationResult);
        _userManager.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).Returns(Task.FromResult(createUserResult));
        _roleManager.Setup(rm => rm.FindByIdAsync(role.Id.ToString())).Returns(Task.FromResult(role));
        _userManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role.Name)).Returns(Task.FromResult(assignRoleResult));

        // Act
        var result = await _userServices.Register(userName, email, password, role.Id.ToString());

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByNameAsync(userName), Times.Once);
        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userValidator.Verify(uv => uv.Validate(It.IsAny<User>()), Times.Once);
        _userManager.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
        _roleManager.Verify(rm => rm.FindByIdAsync(role.Id.ToString()), Times.Once);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), role.Name), Times.Once);
        _userManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Once);
    }

    private static User GenerateUser() =>
        new Faker<User>().CustomInstantiator(f => new User(f.Internet.UserName(), f.Internet.Email()));
}
