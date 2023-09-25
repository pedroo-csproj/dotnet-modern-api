using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    public RolesController(IMediator mediator) =>
        _mediator = mediator;

    private readonly IMediator _mediator;

    [HttpPost]
    [Authorize(Policy = "RolesCreate")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }
}
