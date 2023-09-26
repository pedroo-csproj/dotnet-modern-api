using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using DotNETModernAPI.Presentation.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    public RolesController(IMediator mediator, IOptions<PolicyDTO> policies)
    {
        _mediator = mediator;
        _policies = policies.Value;
    }

    private readonly IMediator _mediator;
    private readonly PolicyDTO _policies;

    [HttpGet("policies")]
    [Authorize(Policy = "RolesListPolicies")]
    public IActionResult ListClaims() =>
        Ok(new ResultWrapper<PolicyDTO>(_policies));

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
