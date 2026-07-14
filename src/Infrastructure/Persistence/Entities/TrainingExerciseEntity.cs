namespace GymAPI.Infrastructure.Persistence.Entities;

public class TrainingExerciseEntity
{
    public Guid TrainingId { get; set; }
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
    public int RestTime { get; set; }
    public int Reps { get; set; }
    public int Series { get; set; }
    public DateTime CreatedAt { get; set; }

    public TrainingEntity Training { get; set; } = null!;
    public ExerciseEntity Exercise { get; set; } = null!;
}
