using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using System.Security.Claims;

namespace DotNETModernAPI.Application.UserContext;

public class UserHandlers :
    IRequestHandler<AuthenticateUserCommandRequest, ResultWrapper<IList<Claim>>>,
    IRequestHandler<RegisterUserCommandRequest, ResultWrapper>,
    IRequestHandler<RequestPasswordResetCommandRequest, ResultWrapper>,
    IRequestHandler<ResetPasswordCommandRequest, ResultWrapper>,
    IRequestHandler<ConfirmEmailCommandRequest, ResultWrapper>
{
    public UserHandlers(UserServices userServices) =>
        _userServices = userServices;

    private readonly UserServices _userServices;

    public async Task<ResultWrapper<IList<Claim>>> Handle(AuthenticateUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.Authenticate(commandRequest.Email, commandRequest.Password);

    public async Task<ResultWrapper> Handle(RegisterUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password, commandRequest.RoleId.ToString());

    public async Task<ResultWrapper> Handle(RequestPasswordResetCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.RequestPasswordReset(commandRequest.Email);

    public async Task<ResultWrapper> Handle(ResetPasswordCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.ResetPassword(commandRequest.Email, commandRequest.NewPassword, commandRequest.PasswordResetToken);

    public async Task<ResultWrapper> Handle(ConfirmEmailCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.ConfirmEmail(commandRequest.Email, commandRequest.EmailConfirmationToken);
}
