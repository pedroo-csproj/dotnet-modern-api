using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class RequestPasswordResetCommandRequest : IRequest<ResultWrapper>
{
    public string Email { get; set; }
}
