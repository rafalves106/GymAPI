using GymAPI.Domain.Enums;

namespace GymAPI.Application.DTOs;

public record ExerciseFilterDto
{
    public MuscleGroupType? MuscleGroup { get; init; }
    public DifficultyLevel? DifficultyLevel { get; init; }
    public string? SearchTerm { get; init; }
}
