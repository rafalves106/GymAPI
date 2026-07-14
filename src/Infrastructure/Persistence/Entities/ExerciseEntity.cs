namespace GymAPI.Infrastructure.Persistence.Entities;

public class ExerciseEntity
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public int RestSeconds { get; set; }
    public int Order { get; set; }

    public WorkoutEntity Workout { get; set; } = null!;
}
