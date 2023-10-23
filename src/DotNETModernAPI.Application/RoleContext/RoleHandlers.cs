using AutoMapper;
using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Application.RoleContext.Commands.Results;
using DotNETModernAPI.Application.RoleContext.Queries.Requests;
using DotNETModernAPI.Application.RoleContext.Queries.Results;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using System.Security.Claims;

namespace DotNETModernAPI.Application.RoleContext;

public class RoleHandlers :
    IRequestHandler<ListRolesQueryRequest, ResultWrapper<IList<ListRolesQueryResult>>>,
    IRequestHandler<CreateRoleCommandRequest, ResultWrapper<CreateRoleCommandResult>>,
    IRequestHandler<UpdateRoleCommandRequest, ResultWrapper>,
    IRequestHandler<AddClaimsToRoleCommandRequest, ResultWrapper>,
    IRequestHandler<RemoveRoleFromUserCommandRequest, ResultWrapper>
{
    public RoleHandlers(RoleServices roleServices, IMapper mapper)
    {
        _roleServices = roleServices;
        _mapper = mapper;
    }

    private readonly RoleServices _roleServices;
    private readonly IMapper _mapper;

    public async Task<ResultWrapper<IList<ListRolesQueryResult>>> Handle(ListRolesQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        var listResult = await _roleServices.List();

        var mappedQueryResult = _mapper.Map<IList<ListRolesQueryResult>>(listResult.Data);

        return new ResultWrapper<IList<ListRolesQueryResult>>(mappedQueryResult);
    }

    public async Task<ResultWrapper<CreateRoleCommandResult>> Handle(CreateRoleCommandRequest commandRequest, CancellationToken cancellationToken)
    {
        var createResult = await _roleServices.Create(commandRequest.Name);

        if (!createResult.Success)
            return new ResultWrapper<CreateRoleCommandResult>(createResult.ErrorCode, createResult.Errors);

        return new ResultWrapper<CreateRoleCommandResult>(new CreateRoleCommandResult(createResult.Data));
    }

    public async Task<ResultWrapper> Handle(UpdateRoleCommandRequest commandRequest, CancellationToken cancellationToken)
    {
        var updateResult = await _roleServices.Update(commandRequest.Id.ToString(), commandRequest.Name);

        if (!updateResult.Success)
            return new ResultWrapper(updateResult.ErrorCode, updateResult.Errors);

        return new ResultWrapper();
    }

    public async Task<ResultWrapper> Handle(AddClaimsToRoleCommandRequest commandRequest, CancellationToken cancellationToken)
    {
        var claims = commandRequest.Claims.Select(c => new Claim("policy", c));

        var addClaimsToRoleResult = await _roleServices.AddClaimsToRole(commandRequest.Id.ToString(), claims);

        if (!addClaimsToRoleResult.Success)
            return new ResultWrapper(addClaimsToRoleResult.ErrorCode, addClaimsToRoleResult.Errors);

        return new ResultWrapper();
    }

    public async Task<ResultWrapper> Handle(RemoveRoleFromUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _roleServices.RemoveRoleFromUser(commandRequest.Id.ToString(), commandRequest.UserId.ToString());
}
