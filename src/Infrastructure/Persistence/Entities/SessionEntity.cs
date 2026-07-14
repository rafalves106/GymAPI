namespace GymAPI.Infrastructure.Persistence.Entities;

public class SessionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkoutId { get; set; }
    public int Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public UserEntity User { get; set; } = null!;
    public WorkoutEntity Workout { get; set; } = null!;
    public ICollection<ExerciseProgressEntity> Progress { get; set; } = new List<ExerciseProgressEntity>();
}
