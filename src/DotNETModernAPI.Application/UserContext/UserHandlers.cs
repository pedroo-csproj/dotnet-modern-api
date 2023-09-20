using AutoMapper;
using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Application.UserContext.Queries.Requests;
using DotNETModernAPI.Application.UserContext.Queries.Responses;
using DotNETModernAPI.Domain.Services;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using System.Security.Claims;

namespace DotNETModernAPI.Application.UserContext;

public class UserHandlers :
    IRequestHandler<ListUsersQueryRequest, ResultWrapper<IEnumerable<ListUsersQueryResponse>>>,
    IRequestHandler<AuthenticateUserCommandRequest, ResultWrapper<IList<Claim>>>,
    IRequestHandler<RegisterUserCommandRequest, ResultWrapper>,
    IRequestHandler<RequestPasswordResetCommandRequest, ResultWrapper>,
    IRequestHandler<ResetPasswordCommandRequest, ResultWrapper>,
    IRequestHandler<ConfirmEmailCommandRequest, ResultWrapper>
{
    public UserHandlers(UserServices userServices, IMapper mapper)
    {
        _userServices = userServices;
        _mapper = mapper;
    }

    private readonly UserServices _userServices;
    private readonly IMapper _mapper;

    public async Task<ResultWrapper<IEnumerable<ListUsersQueryResponse>>> Handle(ListUsersQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        var listResult = await _userServices.List();

        var mappedQueryResponse = _mapper.Map<IEnumerable<ListUsersQueryResponse>>(listResult.Data);

        return new ResultWrapper<IEnumerable<ListUsersQueryResponse>>(mappedQueryResponse);
    }

    public async Task<ResultWrapper<IList<Claim>>> Handle(AuthenticateUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.Authenticate(commandRequest.Email, commandRequest.Password);

    public async Task<ResultWrapper> Handle(RegisterUserCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.Register(commandRequest.UserName, commandRequest.Email, commandRequest.Password, commandRequest.RoleId.ToString());

    public async Task<ResultWrapper> Handle(ConfirmEmailCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.ConfirmEmail(commandRequest.Email, commandRequest.EmailConfirmationToken);

    public async Task<ResultWrapper> Handle(RequestPasswordResetCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.RequestPasswordReset(commandRequest.Email);

    public async Task<ResultWrapper> Handle(ResetPasswordCommandRequest commandRequest, CancellationToken cancellationToken) =>
        await _userServices.ResetPassword(commandRequest.Email, commandRequest.NewPassword, commandRequest.PasswordResetToken);
}
