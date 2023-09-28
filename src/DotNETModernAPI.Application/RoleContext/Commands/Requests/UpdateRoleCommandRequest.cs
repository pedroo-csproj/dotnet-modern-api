using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Commands.Requests;

public class UpdateRoleCommandRequest : IRequest<ResultWrapper>
{
    public UpdateRoleCommandRequest() { }

    public UpdateRoleCommandRequest(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; set; }

    public void SetId(Guid id) =>
        Id = id;
}
