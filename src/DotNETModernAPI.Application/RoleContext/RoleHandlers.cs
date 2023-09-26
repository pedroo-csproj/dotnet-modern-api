using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Application.RoleContext.Commands.Results;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using System.Security.Claims;

namespace DotNETModernAPI.Application.RoleContext;

public class RoleHandlers :
    IRequestHandler<CreateRoleCommandRequest, ResultWrapper<CreateRoleCommandResult>>,
    IRequestHandler<AddClaimsToRoleCommandRequest, ResultWrapper>
{
    public RoleHandlers(RoleServices roleServices) =>
        _roleServices = roleServices;

    private readonly RoleServices _roleServices;

    public async Task<ResultWrapper<CreateRoleCommandResult>> Handle(CreateRoleCommandRequest commandRequest, CancellationToken cancellationToken)
    {
        var createResult = await _roleServices.Create(commandRequest.Name);

        if (!createResult.Success)
            return new ResultWrapper<CreateRoleCommandResult>(createResult.ErrorCode, createResult.Errors);

        return new ResultWrapper<CreateRoleCommandResult>(new CreateRoleCommandResult(createResult.Data));
    }

    public async Task<ResultWrapper> Handle(AddClaimsToRoleCommandRequest commandRequest, CancellationToken cancellationToken)
    {
        var claims = commandRequest.Claims.Select(c => new Claim("policy", c));

        var addClaimsToRoleResult = await _roleServices.AddClaimsToRole(commandRequest.Id.ToString(), claims);

        if (!addClaimsToRoleResult.Success)
            return new ResultWrapper(addClaimsToRoleResult.ErrorCode, addClaimsToRoleResult.Errors);

        return new ResultWrapper();
    }
}
