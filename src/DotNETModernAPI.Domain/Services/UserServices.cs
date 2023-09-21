using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Models;
using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Domain.Views;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Exceptions;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DotNETModernAPI.Domain.Services;

public class UserServices
{
    public UserServices(UserManager<User> userManager, RoleManager<Role> roleManager, IUserRepository userRepository, IEmailProvider emailProvider, IValidator<User> userValidator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _emailProvider = emailProvider;
        _userValidator = userValidator;
    }

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserRepository _userRepository;
    private readonly IEmailProvider _emailProvider;
    private readonly IValidator<User> _userValidator;

    public virtual async Task<ResultWrapper<IList<UserRolesView>>> List() =>
        new ResultWrapper<IList<UserRolesView>>(await _userRepository.List());

    //TODO: apply the single responsability principle
    public virtual async Task<ResultWrapper<IList<Claim>>> Authenticate(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailOrPasswordIncorrect);

        if (!user.EmailConfirmed)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailNotConfirmed);

        var correctPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!correctPassword)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailOrPasswordIncorrect);

        var rolesNames = await _userManager.GetRolesAsync(user);

        IList<Claim> claims = new List<Claim>();

        if (!rolesNames.Any())
            throw new UserDoesntHaveRolesException();

        foreach (var roleName in rolesNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName) ?? throw new RoleNotFoundException(roleName);

            claims = await _roleManager.GetClaimsAsync(role);

            if (!claims.Any())
                throw new RoleWithoutClaimsException(roleName);
        }

        return new ResultWrapper<IList<Claim>>(claims);
    }

    //TODO: apply the single responsability principle
    public virtual async Task<ResultWrapper> Register(string userName, string email, string password, string roleId)
    {
        if (await _userManager.FindByNameAsync(userName) != null)
            return new ResultWrapper(EErrorCode.UserNameAlreadyTaken);

        if (await _userManager.FindByEmailAsync(email) != null)
            return new ResultWrapper(EErrorCode.EmailAlreadyTaken);

        var user = new User(userName, email);

        var userValidationResult = _userValidator.Validate(user);

        if (!userValidationResult.IsValid)
            return new ResultWrapper(userValidationResult.Errors);

        var createUserResult = await _userManager.CreateAsync(user, password);

        if (!createUserResult.Succeeded)
            return new ResultWrapper(createUserResult.Errors);

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
            return new ResultWrapper(EErrorCode.RoleNotFound);

        var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);

        if (!addToRoleResult.Succeeded)
            return new ResultWrapper(addToRoleResult.Errors);

        var emailConfirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailRequest = new EmailRequestModel(user.Email, "Confirm Email", $"<h1>{emailConfirmationToken}</h1>");

        await _emailProvider.SendAsync(emailRequest);

        return new ResultWrapper();
    }

    public virtual async Task<ResultWrapper> ConfirmEmail(string email, string emailConfirmationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return new ResultWrapper(EErrorCode.EmailNotFound);

        if (user.EmailConfirmed)
            return new ResultWrapper(EErrorCode.EmailAlreadyConfirmed);

        var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

        if (!emailConfirmationResult.Succeeded)
            return new ResultWrapper(emailConfirmationResult.Errors);

        var emailRequest = new EmailRequestModel(user.Email, "Email Confirmed", $"Email confirmed.");

        await _emailProvider.SendAsync(emailRequest);

        return new ResultWrapper();
    }

    public virtual async Task<ResultWrapper> RequestPasswordReset(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return new ResultWrapper(EErrorCode.EmailNotFound);

        if (!user.EmailConfirmed)
            return new ResultWrapper(EErrorCode.EmailNotConfirmed);

        var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var emailRequest = new EmailRequestModel(email, "Reset your Password", $"<h1>{passwordResetToken}</h1>");

        await _emailProvider.SendAsync(emailRequest);

        return new ResultWrapper();
    }

    public virtual async Task<ResultWrapper> ResetPassword(string email, string newPassword, string passwordResetToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return new ResultWrapper(EErrorCode.EmailNotFound);

        if (!user.EmailConfirmed)
            return new ResultWrapper(EErrorCode.EmailNotConfirmed);

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);

        if (!resetPasswordResult.Succeeded)
            return new ResultWrapper(resetPasswordResult.Errors);

        var emailRequest = new EmailRequestModel(user.Email, "Password Changed", $"Hello {user.UserName}, your password was changed successfully");

        await _emailProvider.SendAsync(emailRequest);

        return new ResultWrapper();
    }
}
