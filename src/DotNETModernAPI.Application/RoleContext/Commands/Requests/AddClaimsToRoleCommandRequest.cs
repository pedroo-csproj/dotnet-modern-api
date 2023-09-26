using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.RoleContext.Commands.Requests;

public class AddClaimsToRoleCommandRequest : IRequest<ResultWrapper>
{
    public AddClaimsToRoleCommandRequest() { }

    public AddClaimsToRoleCommandRequest(Guid id, IEnumerable<string> claims)
    {
        Id = id;
        Claims = claims;
    }

    public Guid Id { get; private set; }
    public IEnumerable<string> Claims { get; set; }

    public void SetId(Guid id) =>
        Id = id;
}
