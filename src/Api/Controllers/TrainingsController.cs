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
public class TrainingsController : ControllerBase
{
    private readonly ITrainingUseCases _trainingUseCases;

    public TrainingsController(ITrainingUseCases trainingUseCases)
    {
        _trainingUseCases = trainingUseCases;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        return Guid.Parse(userId);
    }

    /// <summary>
    /// Get all trainings for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TrainingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TrainingDto>>> GetAll()
    {
        var userId = GetUserId();
        var trainings = await _trainingUseCases.GetByUserIdAsync(userId);
        return Ok(trainings);
    }

    /// <summary>
    /// Get training by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TrainingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingDto>> GetById(Guid id)
    {
        var userId = GetUserId();
        var training = await _trainingUseCases.GetByIdAsync(id, userId);
        if (training is null)
            return NotFound();

        return Ok(training);
    }

    /// <summary>
    /// Create a new training
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TrainingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TrainingDto>> Create([FromBody] CreateTrainingRequest request)
    {
        var userId = GetUserId();
        var training = await _trainingUseCases.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = training.Id }, training);
    }

    /// <summary>
    /// Update an existing training
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TrainingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TrainingDto>> Update(Guid id, [FromBody] UpdateTrainingRequest request)
    {
        var userId = GetUserId();
        var training = await _trainingUseCases.UpdateAsync(id, request, userId);
        return Ok(training);
    }

    /// <summary>
    /// Delete a training
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await _trainingUseCases.DeleteAsync(id, userId);
        return NoContent();
    }
}
