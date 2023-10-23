using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Commands.Requests;

public class RemoveRoleFromUserCommandRequest : IRequest<ResultWrapper>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
