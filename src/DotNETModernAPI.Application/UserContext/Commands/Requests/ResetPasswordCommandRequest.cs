using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class ResetPasswordCommandRequest : IRequest<ResultWrapper>
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string PasswordResetToken { get; set; }
}
