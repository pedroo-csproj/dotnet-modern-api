using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext;

public class UserHandlers : IRequestHandler<RegisterUserCommandRequest, ResultWrapper>
{
    public UserHandlers(UserServices userServices) =>
        _userServices = userServices;

    private readonly UserServices _userServices;

    public async Task<ResultWrapper> Handle(RegisterUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password);
}
