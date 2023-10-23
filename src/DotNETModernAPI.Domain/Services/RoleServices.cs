using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Repositories;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DotNETModernAPI.Domain.Services;

public class RoleServices
{
    public RoleServices(RoleManager<Role> roleManager, UserManager<User> userManager, IValidator<Role> roleValidator, IOptions<PoliciesDTO> policies, IRoleRepository roleRepository)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _roleValidator = roleValidator;
        _policies = policies.Value;
        _roleRepository = roleRepository;
    }

    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<Role> _roleValidator;
    private readonly PoliciesDTO _policies;
    private readonly IRoleRepository _roleRepository;

    public virtual async Task<ResultWrapper<IList<Role>>> List()
    {
        var roles = await _roleRepository.List();

        return new ResultWrapper<IList<Role>>(roles);
    }

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

    public virtual async Task<ResultWrapper> Update(string id, string name)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return new ResultWrapper(EErrorCode.RoleNotFound);

        role.UpdateName(name);

        var validationResult = _roleValidator.Validate(role);

        if (!validationResult.IsValid)
            return new ResultWrapper(validationResult.Errors);

        var updateRoleResult = await _roleManager.UpdateAsync(role);

        if (!updateRoleResult.Succeeded)
            return new ResultWrapper(updateRoleResult.Errors);

        return new ResultWrapper();
    }

    public virtual async Task<ResultWrapper> AddClaimsToRole(string id, IEnumerable<Claim> claims)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return new ResultWrapper(EErrorCode.RoleNotFound);

        var roleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in claims)
        {
            if (!_policies.Users.Contains(claim.Value) && !_policies.Roles.Contains(claim.Value))
                return new ResultWrapper(EErrorCode.InvalidPolicy, new List<string>() { claim.Value });

            if (roleClaims.Any(c => c.Value == claim.Value))
                return new ResultWrapper(EErrorCode.PolicyAlreadyAssignedToRole, new List<string>() { claim.Value });
        }

        foreach (var claim in claims)
        {
            var addRoleResult = await _roleManager.AddClaimAsync(role, claim);

            if (!addRoleResult.Succeeded)
                return new ResultWrapper(addRoleResult.Errors);
        }

        return new ResultWrapper();
    }

    public virtual async Task<ResultWrapper> RemoveRoleFromUser(string id, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return new ResultWrapper(EErrorCode.UserNotFound);

        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return new ResultWrapper(EErrorCode.RoleNotFound);

        var userIsInRole = await _userManager.IsInRoleAsync(user, role.Name);

        if (!userIsInRole)
            return new ResultWrapper(EErrorCode.UserDoesntHaveRole);

        var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, role.Name);

        if (!removeFromRoleResult.Succeeded)
            return new ResultWrapper(removeFromRoleResult.Errors);

        return new ResultWrapper();
    }
}
