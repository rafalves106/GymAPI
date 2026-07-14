using System.Security.Claims;
using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionUseCases _sessionUseCases;

    public SessionsController(ISessionUseCases sessionUseCases)
    {
        _sessionUseCases = sessionUseCases;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        return Guid.Parse(userId);
    }

    [HttpPost("start/{workoutId:guid}")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Start(Guid workoutId)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.StartAsync(workoutId, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/pause")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Pause(Guid id)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.PauseAsync(id, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/resume")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Resume(Guid id)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.ResumeAsync(id, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/stop")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Stop(Guid id)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.CompleteAsync(id, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Cancel(Guid id)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.CancelAsync(id, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/exercise/{exerciseId:guid}/increment")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Increment(Guid id, Guid exerciseId)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.IncrementAsync(id, exerciseId, userId);
        return Ok(session);
    }

    [HttpPost("{id:guid}/exercise/{exerciseId:guid}/decrement")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> Decrement(Guid id, Guid exerciseId)
    {
        var userId = GetUserId();
        var session = await _sessionUseCases.DecrementAsync(id, exerciseId, userId);
        return Ok(session);
    }
}
