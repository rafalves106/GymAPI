namespace GymAPI.Application.DTOs;

public record SessionDto
{
    public Guid Id { get; init; }
    public Guid WorkoutId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime StartedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
    public List<ExerciseProgressDto> Progress { get; init; } = new();
}

public record ExerciseProgressDto
{
    public Guid ExerciseId { get; init; }
    public int CompletedSets { get; init; }
}
