namespace GymAPI.Infrastructure.Persistence.Entities;

public class TrainingEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public UserEntity User { get; set; } = null!;
    public ICollection<TrainingExerciseEntity> TrainingExercises { get; set; } = new List<TrainingExerciseEntity>();
}
