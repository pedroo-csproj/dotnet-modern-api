using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Commands.Requests;

public class RemoveRoleFromUserCommandRequest : IRequest<ResultWrapper>
{
    public RemoveRoleFromUserCommandRequest() { }

    public RemoveRoleFromUserCommandRequest(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
