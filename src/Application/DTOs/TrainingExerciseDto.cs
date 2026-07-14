namespace GymAPI.Application.DTOs;

public record TrainingExerciseDto
{
    public Guid ExerciseId { get; init; }
    public string ExerciseName { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public int RestTime { get; init; }
    public int Reps { get; init; }
    public int Series { get; init; }
}
