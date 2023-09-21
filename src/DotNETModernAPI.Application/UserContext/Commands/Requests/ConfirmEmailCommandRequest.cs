using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class ConfirmEmailCommandRequest : IRequest<ResultWrapper>
{
    public ConfirmEmailCommandRequest() { }

    public ConfirmEmailCommandRequest(string email, string emailConfirmationToken)
    {
        Email = email;
        EmailConfirmationToken = emailConfirmationToken;
    }

    public string Email { get; set; }
    public string EmailConfirmationToken { get; set; }
}
