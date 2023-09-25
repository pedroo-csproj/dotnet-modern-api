using DotNETModernAPI.Application.RoleContext.Commands.Results;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Commands.Requests;

public class CreateRoleCommandRequest : IRequest<ResultWrapper<CreateRoleCommandResult>>
{
    public CreateRoleCommandRequest() { }

    public CreateRoleCommandRequest(string name) =>
        Name = name;

    public string Name { get; set; }
}
