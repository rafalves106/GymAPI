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
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutUseCases _workoutUseCases;

    public WorkoutsController(IWorkoutUseCases workoutUseCases)
    {
        _workoutUseCases = workoutUseCases;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        return Guid.Parse(userId);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetAll()
    {
        var userId = GetUserId();
        var workouts = await _workoutUseCases.GetByUserIdAsync(userId);
        return Ok(workouts);
    }

    [HttpGet("today")]
    [ProducesResponseType(typeof(IEnumerable<WorkoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetToday([FromQuery] DayOfWeek? day)
    {
        var userId = GetUserId();
        var workouts = await _workoutUseCases.GetTodayAsync(userId, day);
        return Ok(workouts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDto>> GetById(Guid id)
    {
        var userId = GetUserId();
        var workout = await _workoutUseCases.GetByIdAsync(id, userId);
        if (workout is null)
            return NotFound();

        return Ok(workout);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkoutDto>> Create([FromBody] CreateWorkoutRequest request)
    {
        var userId = GetUserId();
        var workout = await _workoutUseCases.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkoutDto>> Update(Guid id, [FromBody] UpdateWorkoutRequest request)
    {
        var userId = GetUserId();
        var workout = await _workoutUseCases.UpdateAsync(id, request, userId);
        return Ok(workout);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await _workoutUseCases.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPut("{id:guid}/day")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignDay(Guid id, [FromBody] AssignDayRequest request)
    {
        var userId = GetUserId();
        await _workoutUseCases.AssignDayAsync(id, request, userId);
        return NoContent();
    }
}
