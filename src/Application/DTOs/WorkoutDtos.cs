namespace GymAPI.Application.DTOs;

public record WorkoutDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DayOfWeek? ScheduledDay { get; init; }
    public List<ExerciseDto> Exercises { get; init; } = new();
}

public record ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int TargetSets { get; init; }
    public int TargetReps { get; init; }
    public int RestSeconds { get; init; }
    public int Order { get; init; }
}

public record CreateWorkoutRequest
{
    public string Name { get; init; } = string.Empty;
    public List<CreateExerciseRequest> Exercises { get; init; } = new();
}

public record UpdateWorkoutRequest
{
    public string Name { get; init; } = string.Empty;
    public List<CreateExerciseRequest> Exercises { get; init; } = new();
}

public record CreateExerciseRequest
{
    public string Name { get; init; } = string.Empty;
    public int TargetSets { get; init; }
    public int TargetReps { get; init; }
    public int RestSeconds { get; init; }
}

public record AssignDayRequest
{
    public DayOfWeek? Day { get; init; }
}
