using GymAPI.Domain.Enums;

namespace GymAPI.Application.DTOs;

public record ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<MuscleGroupType> MuscleGroups { get; init; } = new();
    public List<EquipmentType> Equipments { get; init; } = new();
    public DifficultyLevel DifficultyLevel { get; init; }
    public string? VideoUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
