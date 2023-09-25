using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Domain.Services;

public class RoleServices
{
    public RoleServices(RoleManager<Role> roleManager, IValidator<Role> roleValidator)
    {
        _roleManager = roleManager;
        _roleValidator = roleValidator;
    }

    private readonly RoleManager<Role> _roleManager;
    private readonly IValidator<Role> _roleValidator;

    public virtual async Task<ResultWrapper<Guid>> Create(string name)
    {
        if (await _roleManager.FindByNameAsync(name) != null)
            return new ResultWrapper<Guid>(EErrorCode.RoleAlreadyExists);

        var role = new Role(name);

        var validationResult = _roleValidator.Validate(role);

        if (!validationResult.IsValid)
            return new ResultWrapper<Guid>(validationResult.Errors);

        var createRoleResult = await _roleManager.CreateAsync(role);

        if (!createRoleResult.Succeeded)
            return new ResultWrapper<Guid>(createRoleResult.Errors);

        return new ResultWrapper<Guid>(role.Id);
    }
}
