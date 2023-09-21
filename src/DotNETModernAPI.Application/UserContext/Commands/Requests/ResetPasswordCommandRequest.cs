using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class ResetPasswordCommandRequest : IRequest<ResultWrapper>
{
    public ResetPasswordCommandRequest() { }

    public ResetPasswordCommandRequest(string email, string newPassword, string passwordResetToken)
    {
        Email = email;
        NewPassword = newPassword;
        PasswordResetToken = passwordResetToken;
    }

    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string PasswordResetToken { get; set; }
}
