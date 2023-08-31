using DotNETModernAPI.Application.UserContext.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    public UsersController(IMediator mediator) =>
        _mediator = mediator;

    private readonly IMediator _mediator;

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }
}
