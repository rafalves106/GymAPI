using System.ComponentModel.DataAnnotations;
using GymAPI.Domain.Enums;

namespace GymAPI.Application.DTOs;

public record CreateExerciseRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<MuscleGroupType> MuscleGroups { get; init; } = new();

    [Required]
    [MinLength(1)]
    public List<EquipmentType> Equipments { get; init; } = new();

    [Required]
    public DifficultyLevel DifficultyLevel { get; init; }

    [Url]
    [StringLength(500)]
    public string? VideoUrl { get; init; }
}
