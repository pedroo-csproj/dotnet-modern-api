using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using System.Security.Claims;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class AuthenticateUserCommandRequest : IRequest<ResultWrapper<IList<Claim>>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
