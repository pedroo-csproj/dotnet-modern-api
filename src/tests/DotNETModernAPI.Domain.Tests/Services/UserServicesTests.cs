using Bogus;
using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Models;
using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Domain.Views;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
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
        _userRepository = new Mock<IUserRepository>();
        _emailProvider = new Mock<IEmailProvider>();
        _userValidator = new Mock<IValidator<User>>();
        _userServices = new UserServices(_userManager.Object, _roleManager.Object, _userRepository.Object, _emailProvider.Object, _userValidator.Object);
        _faker = new Faker();
    }

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<RoleManager<Role>> _roleManager;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IValidator<User>> _userValidator;
    private readonly UserServices _userServices;
    private readonly Faker _faker;

    #region List

    [Fact(DisplayName = "List - Valid Data")]
    public async void List_ValidData_MustReturnNoError()
    {
        // Arrange
        var role = new List<RoleVO>() { new RoleVO(Guid.NewGuid(), "Admin") };
        var userRoles = new List<UserRolesView>() { new UserRolesView(Guid.NewGuid(), _faker.Internet.UserName(), _faker.Internet.Email(), true, role) };

        _userRepository.Setup(ur => ur.List()).Returns(Task.FromResult((IList<UserRolesView>)userRoles));

        // Act
        var result = await _userServices.List();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Equal(userRoles, result.Data);

        _userRepository.Verify(ur => ur.List(), Times.Once);
    }

    #endregion

    #region Authenticate

    [Fact(DisplayName = "Authenticate - Invalid Email")]
    public async void Authenticate_InvalidEmail_MustReturnEmailOrPasswordIncorrect()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();

        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));

        // Act
        var result = await _userServices.Authenticate(email, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailOrPasswordIncorrect, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Null(result.Data);

        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Authenticate - Email not Confirmed")]
    public async void Authenticate_EmailNotConfirmed_MustReturnEmailNotConfirmed()
    {
        // Arrange
        var user = GenerateUser(false);
        var password = _faker.Internet.Password();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));

        // Act
        var result = await _userServices.Authenticate(user.Email, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotConfirmed, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Null(result.Data);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Authenticate - Incorrect Password")]
    public async void Authenticate_IncorrectPassword_MustReturnEmailOrPasswordIncorrect()
    {
        // Arrange
        var user = GenerateUser();
        var password = _faker.Internet.Password();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.CheckPasswordAsync(user, password)).Returns(Task.FromResult(false));

        // Act
        var result = await _userServices.Authenticate(user.Email, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailOrPasswordIncorrect, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Null(result.Data);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _roleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Authenticate - User doesn't have Roles")]
    public async void Authenticate_UserDoesntHaveRoles_MustThrowUserDoesntHaveRolesException()
    {
        // Arrange
        var user = GenerateUser();
        var password = _faker.Internet.Password();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.CheckPasswordAsync(user, password)).Returns(Task.FromResult(true));
        _userManager.Setup(um => um.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)new List<string>()));

        // Act
        var exception = await Assert.ThrowsAsync<UserDoesntHaveRolesException>(async () => await _userServices.Authenticate(user.Email, password));

        // Assert
        Assert.Equal("An User must have at least one Role", exception.Message);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(user), Times.Once);
        _roleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Authenticate - Invalid Role")]
    public async void Authenticate_RoleNotFound_MustThrowRoleNotFoundException()
    {
        // Arrange
        var user = GenerateUser();
        var password = _faker.Internet.Password();
        var roles = new List<string>() { "Admin", "Instructor" };

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.CheckPasswordAsync(user, password)).Returns(Task.FromResult(true));
        _userManager.Setup(um => um.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)roles));
        _roleManager.Setup(rm => rm.FindByNameAsync(roles.First())).Returns(Task.FromResult((Role)null));

        // Act
        var exception = await Assert.ThrowsAsync<RoleNotFoundException>(async () => await _userServices.Authenticate(user.Email, password));

        // Assert
        Assert.Equal($"The Role \"{roles.First()}\" doesn't exists", exception.Message);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(user), Times.Once);
        _roleManager.Verify(rm => rm.FindByNameAsync(roles.First()), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Never);
    }

    [Fact(DisplayName = "Authenticate - Role without Claims")]
    public async void Authenticate_RoleWithoutClaims_MustThrowRoleWithoutClaimsException()
    {
        // Arrange
        var user = GenerateUser();
        var password = _faker.Internet.Password();
        var roles = new List<string>() { "Admin", "Instructor" };
        var role = new Role(roles.First());

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.CheckPasswordAsync(user, password)).Returns(Task.FromResult(true));
        _userManager.Setup(um => um.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)roles));
        _roleManager.Setup(rm => rm.FindByNameAsync(roles.First())).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult((IList<Claim>)new List<Claim>()));

        // Act
        var exception = await Assert.ThrowsAsync<RoleWithoutClaimsException>(async () => await _userServices.Authenticate(user.Email, password));

        // Assert
        Assert.Equal($"An attempt to retrieve claims of a Role named \"{role.Name}\" happened", exception.Message);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(user), Times.Once);
        _roleManager.Verify(rm => rm.FindByNameAsync(roles.First()), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Once);
    }

    [Fact(DisplayName = "Authenticate - Valid Data")]
    public async void Authenticate_ValidData_MustReturnNoError()
    {
        // Arrange
        var user = GenerateUser();
        var password = _faker.Internet.Password();
        var roles = new List<string>() { "Admin" };
        var role = new Role(roles.First());
        var claims = new List<Claim>() { new Claim("aud", "http://localhost:3000") };

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.CheckPasswordAsync(user, password)).Returns(Task.FromResult(true));
        _userManager.Setup(um => um.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)roles));
        _roleManager.Setup(rm => rm.FindByNameAsync(roles.First())).Returns(Task.FromResult(role));
        _roleManager.Setup(rm => rm.GetClaimsAsync(role)).Returns(Task.FromResult((IList<Claim>)claims));

        // Act
        var result = await _userServices.Authenticate(user.Email, password);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);
        Assert.Equal(claims, result.Data);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(user), Times.Once);
        _roleManager.Verify(rm => rm.FindByNameAsync(roles.First()), Times.Once);
        _roleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<Role>()), Times.Once);
    }

    #endregion

    #region Register

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

    #endregion

    #region ConfirmEmail

    [Fact(DisplayName = "ConfirmEmail - Invalid Email")]
    public async void ConfirmEmail_InvalidEmail_MustReturnEmailNotFound()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var emailConfirmationToken = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));

        // Act
        var result = await _userServices.ConfirmEmail(email, emailConfirmationToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotFound, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ConfirmEmail - Email already Confirmed")]
    public async void ConfirmEmail_EmailAlreadyConfirmed_MustReturnEmailAlreadyConfirmed()
    {
        // Arrange
        var user = GenerateUser();
        var emailConfirmationToken = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));

        // Act
        var result = await _userServices.ConfirmEmail(user.Email, emailConfirmationToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailAlreadyConfirmed, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ConfirmEmail - Invalid Token")]
    public async void ConfirmEmail_InvalidToken_MustReturnIdentityError()
    {
        // Arrange
        var user = GenerateUser(false);
        var emailConfirmationToken = Guid.NewGuid().ToString();
        var error = "Invalid Token";
        var errors = new List<string>() { error };
        var confirmEmailResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = error });

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.ConfirmEmailAsync(user, emailConfirmationToken)).Returns(Task.FromResult(confirmEmailResult));

        // Act
        var result = await _userServices.ConfirmEmail(user.Email, emailConfirmationToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Equal(errors, result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ConfirmEmailAsync(user, emailConfirmationToken), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ConfirmEmail - Valid Data")]
    public async void ConfirmEmail_ValidData_MustReturnNoError()
    {
        // Arrange
        var user = GenerateUser(false);
        var emailConfirmationToken = Guid.NewGuid().ToString();
        var confirmEmailResult = IdentityResult.Success;
        var emailRequest = new EmailRequestModel(user.Email, "Email Confirmed", $"Email confirmed.");

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.ConfirmEmailAsync(user, emailConfirmationToken)).Returns(Task.FromResult(confirmEmailResult));

        // Act
        var result = await _userServices.ConfirmEmail(user.Email, emailConfirmationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ConfirmEmailAsync(user, emailConfirmationToken), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Once);
    }

    #endregion

    #region RequestPasswordReset

    [Fact(DisplayName = "RequestPasswordReset - Invalid Email")]
    public async void RequestPasswordReset_InvalidEmail_MustReturnEmailNotFound()
    {
        // Arrange
        var email = _faker.Internet.Email();

        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));

        // Act
        var result = await _userServices.RequestPasswordReset(email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotFound, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userManager.Verify(um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "RequestPasswordReset - Email not Confirmed")]
    public async void RequestPasswordReset_EmailNotConfirmed_MustReturnEmailNotConfirmed()
    {
        // Arrange
        var user = GenerateUser(false);

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));

        // Act
        var result = await _userServices.RequestPasswordReset(user.Email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotConfirmed, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "RequestPasswordReset - Valid Data")]
    public async void RequestPasswordReset_ValidData_MustReturnNoError()
    {
        // Arrange
        var user = GenerateUser();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.GeneratePasswordResetTokenAsync(user)).Returns(Task.FromResult(Guid.NewGuid().ToString()));

        // Act
        var result = await _userServices.RequestPasswordReset(user.Email);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.GeneratePasswordResetTokenAsync(user), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Once);
    }

    #endregion

    #region ResetPassword

    [Fact(DisplayName = "ResetPassword - Invalid Email")]
    public async void ResetPassword_InvalidEmail_MustReturnEmailNotFound()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var newPassword = _faker.Internet.Password();
        var passwordResetToken = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByEmailAsync(email)).Returns(Task.FromResult((User)null));

        // Act
        var result = await _userServices.ResetPassword(email, newPassword, passwordResetToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotFound, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        _userManager.Verify(um => um.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ResetPassword - Email not Confirmed")]
    public async void ResetPassword_EmailNotConfirmed_MustReturnEmailNotConfirmed()
    {
        // Arrange
        var user = GenerateUser(false);
        var newPassword = _faker.Internet.Password();
        var passwordResetToken = Guid.NewGuid().ToString();

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));

        // Act
        var result = await _userServices.ResetPassword(user.Email, newPassword, passwordResetToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.EmailNotConfirmed, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ResetPassword - Invalid Password ResetToken")]
    public async void ResetPassword_InvalidPasswordResetToken_MustReturnIdentityError()
    {
        // Arrange
        var user = GenerateUser();
        var newPassword = _faker.Internet.Password();
        var passwordResetToken = Guid.NewGuid().ToString();
        var error = "Invalid Password Reset Token";
        var errors = new List<string>() { error };
        var resetPasswordResult = IdentityResult.Failed(new IdentityError() { Code = Guid.NewGuid().ToString(), Description = error });

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.ResetPasswordAsync(user, passwordResetToken, newPassword)).Returns(Task.FromResult(resetPasswordResult));

        // Act
        var result = await _userServices.ResetPassword(user.Email, newPassword, passwordResetToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(EErrorCode.IdentityError, result.ErrorCode);
        Assert.Equal(errors, result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ResetPasswordAsync(user, passwordResetToken, newPassword), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Never);
    }

    [Fact(DisplayName = "ResetPassword - Valid Data")]
    public async void ResetPassword_ValidData_MustReturnNoError()
    {
        // Arrange
        var user = GenerateUser();
        var newPassword = _faker.Internet.Password();
        var passwordResetToken = Guid.NewGuid().ToString();
        var resetPasswordResult = IdentityResult.Success;

        _userManager.Setup(um => um.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
        _userManager.Setup(um => um.ResetPasswordAsync(user, passwordResetToken, newPassword)).Returns(Task.FromResult(resetPasswordResult));

        // Act
        var result = await _userServices.ResetPassword(user.Email, newPassword, passwordResetToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(EErrorCode.NoError, result.ErrorCode);
        Assert.Empty(result.Errors);

        _userManager.Verify(um => um.FindByEmailAsync(user.Email), Times.Once);
        _userManager.Verify(um => um.ResetPasswordAsync(user, passwordResetToken, newPassword), Times.Once);
        _emailProvider.Verify(ep => ep.SendAsync(It.IsAny<EmailRequestModel>()), Times.Once);
    }

    #endregion

    private static User GenerateUser(bool emailConfirmed = true) =>
        new Faker<User>().CustomInstantiator(f => new User(f.Internet.UserName(), f.Internet.Email()) { EmailConfirmed = emailConfirmed });
}
