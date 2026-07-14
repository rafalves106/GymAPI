namespace GymAPI.Infrastructure.Persistence.Entities;

public class WorkoutEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public int? ScheduledDay { get; set; }

    public UserEntity User { get; set; } = null!;
    public ICollection<ExerciseEntity> Exercises { get; set; } = new List<ExerciseEntity>();
}
