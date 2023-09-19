using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;

namespace DotNETModernAPI.Application.UserContext.Commands.Requests;

public class RegisterUserCommandRequest : IRequest<ResultWrapper>
{
    public RegisterUserCommandRequest() { }

    public RegisterUserCommandRequest(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }

    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid RoleId { get; set; }
}
