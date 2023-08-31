using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Domain.Services;

public class UserServices
{
    public UserServices(UserManager<User> userManager, IValidator<User> userValidator)
    {
        _userManager = userManager;
        _userValidator = userValidator;
    }

    private readonly UserManager<User> _userManager;
    private readonly IValidator<User> _userValidator;

    public virtual async Task<ResultWrapper> Register(string userName, string email, string password)
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

        return new ResultWrapper();
    }
}
