using GymAPI.Domain.Enums;

namespace GymAPI.Infrastructure.Persistence.Entities;

public class ExerciseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel DifficultyLevel { get; set; }
    public string? VideoUrl { get; set; }
    public string? ExternalApiId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ExerciseMuscleGroupEntity> ExerciseMuscleGroups { get; set; } = new List<ExerciseMuscleGroupEntity>();
    public ICollection<ExerciseEquipmentEntity> ExerciseEquipments { get; set; } = new List<ExerciseEquipmentEntity>();
}

public class ExerciseMuscleGroupEntity
{
    public Guid ExerciseId { get; set; }
    public MuscleGroupType MuscleGroup { get; set; }

    public ExerciseEntity Exercise { get; set; } = null!;
}

public class ExerciseEquipmentEntity
{
    public Guid ExerciseId { get; set; }
    public EquipmentType EquipmentType { get; set; }

    public ExerciseEntity Exercise { get; set; } = null!;
}
