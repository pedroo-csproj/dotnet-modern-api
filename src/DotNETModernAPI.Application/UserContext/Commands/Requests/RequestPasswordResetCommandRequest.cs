using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class RequestPasswordResetCommandRequest : IRequest<ResultWrapper>
{
    public RequestPasswordResetCommandRequest() { }

    public RequestPasswordResetCommandRequest(string email) =>
        Email = email;

    public string Email { get; set; }
}
