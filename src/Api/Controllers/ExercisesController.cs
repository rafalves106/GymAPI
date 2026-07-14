using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseUseCases _exerciseUseCases;

    public ExercisesController(IExerciseUseCases exerciseUseCases)
    {
        _exerciseUseCases = exerciseUseCases;
    }

    /// <summary>
    /// Get all exercises
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
    {
        var exercises = await _exerciseUseCases.GetAllAsync();
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercise by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseDto>> GetById(Guid id)
    {
        var exercise = await _exerciseUseCases.GetByIdAsync(id);
        if (exercise is null)
            return NotFound();

        return Ok(exercise);
    }

    /// <summary>
    /// Get exercises by muscle group
    /// </summary>
    [HttpGet("muscle-group/{muscleGroup}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByMuscleGroup(MuscleGroupType muscleGroup)
    {
        var exercises = await _exerciseUseCases.GetByMuscleGroupAsync(muscleGroup);
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercises by difficulty level
    /// </summary>
    [HttpGet("difficulty/{difficultyLevel}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByDifficulty(DifficultyLevel difficultyLevel)
    {
        var exercises = await _exerciseUseCases.GetByDifficultyAsync(difficultyLevel);
        return Ok(exercises);
    }

    /// <summary>
    /// Search exercises by name
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term is required.");

        var exercises = await _exerciseUseCases.SearchByNameAsync(name);
        return Ok(exercises);
    }

    /// <summary>
    /// Create a new exercise
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExerciseDto>> Create([FromBody] CreateExerciseRequest request)
    {
        var exercise = await _exerciseUseCases.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, exercise);
    }

    /// <summary>
    /// Update an existing exercise
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExerciseDto>> Update(Guid id, [FromBody] UpdateExerciseRequest request)
    {
        var exercise = await _exerciseUseCases.UpdateAsync(id, request);
        return Ok(exercise);
    }

    /// <summary>
    /// Delete an exercise
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _exerciseUseCases.DeleteAsync(id);
        return NoContent();
    }
}
