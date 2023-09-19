using DotNETModernAPI.Application.UserContext.Commands.Requests;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using DotNETModernAPI.Presentation.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    public UsersController(IMediator mediator, JwtServices jwtServices)
    {
        _mediator = mediator;
        _jwtServices = jwtServices;
    }

    private readonly IMediator _mediator;
    private readonly JwtServices _jwtServices;

    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        var jwt = _jwtServices.Generate(handleResult.Data);

        return Ok(new ResultWrapper<string>(jwt));
    }

    [HttpPost("Register")]
    [Authorize(Policy = "UsersRegister")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }
}
