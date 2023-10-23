using DotNETModernAPI.Application.RoleContext.Commands.Requests;
using DotNETModernAPI.Application.RoleContext.Queries.Requests;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.DTOs;
using DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    public RolesController(IMediator mediator, IOptions<PoliciesDTO> policies)
    {
        _mediator = mediator;
        _policies = policies.Value;
    }

    private readonly IMediator _mediator;
    private readonly PoliciesDTO _policies;

    [HttpGet]
    [Authorize(Policy = "RolesListRoles")]
    public async Task<IActionResult> List([FromRoute] ListRolesQueryRequest queryRequest)
    {
        var handleResult = await _mediator.Send(queryRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }

    [HttpGet("policies")]
    [Authorize(Policy = "RolesListPolicies")]
    public IActionResult ListClaims() =>
        Ok(new ResultWrapper<PoliciesDTO>(_policies));

    [HttpPost]
    [Authorize(Policy = "RolesCreate")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "RolesUpdate")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRoleCommandRequest commandRequest)
    {
        commandRequest.SetId(id);

        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }

    [HttpPost("{id}/claims/add")]
    [Authorize(Policy = "RolesAddClaimsToRole")]
    public async Task<IActionResult> AddClaimsToRole([FromRoute] Guid id, [FromBody] AddClaimsToRoleCommandRequest commandRequest)
    {
        commandRequest.SetId(id);

        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }

    [HttpPost("{Id}/users/{UserId}")]
    [Authorize(Policy = "RemoveRoleFromUser")]
    public async Task<IActionResult> RemoveRoleFromUser([FromRoute] RemoveRoleFromUserCommandRequest commandRequest)
    {
        var handleResult = await _mediator.Send(commandRequest);

        if (!handleResult.Success)
            return BadRequest(handleResult);

        return Ok(handleResult);
    }
}
