namespace GymAPI.Infrastructure.Persistence.Entities;

public class ExerciseProgressEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid ExerciseId { get; set; }
    public int CompletedSets { get; set; }

    public SessionEntity Session { get; set; } = null!;
}
