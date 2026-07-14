using System.ComponentModel.DataAnnotations;

namespace GymAPI.Application.DTOs;

public record UpdateTrainingRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required]
    [MinLength(1)]
    [MaxLength(10)]
    public List<UpdateTrainingExerciseRequest> Exercises { get; init; } = new();
}

public record UpdateTrainingExerciseRequest
{
    [Required]
    public Guid ExerciseId { get; init; }

    [Range(0, 120)]
    public int RestTime { get; init; }

    [Range(1, 30)]
    public int Reps { get; init; }

    [Range(1, 15)]
    public int Series { get; init; }
}
