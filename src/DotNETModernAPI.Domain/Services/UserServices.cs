using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Models;
using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DotNETModernAPI.Domain.Services;

public class UserServices
{
    public UserServices(UserManager<User> userManager, RoleManager<Role> roleManager, IEmailProvider emailProvider, IValidator<User> userValidator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailProvider = emailProvider;
        _userValidator = userValidator;
    }

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IEmailProvider _emailProvider;
    private readonly IValidator<User> _userValidator;

    //TODO: apply the single responsability principle
    public virtual async Task<ResultWrapper<IList<Claim>>> Authenticate(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailOrPasswordIncorrect);

        if (!user.EmailConfirmed)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailNotConfirmed);

        var correctPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!correctPassword)
            return new ResultWrapper<IList<Claim>>(EErrorCode.EmailOrPasswordIncorrect);

        var rolesNames = await _userManager.GetRolesAsync(user);

        IList<Claim> claims = new List<Claim>();

        if (rolesNames.Any())
        {
            foreach (var roleName in rolesNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                claims = await _roleManager.GetClaimsAsync(role);
            }
        }

        return new ResultWrapper<IList<Claim>>(claims);
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
}
