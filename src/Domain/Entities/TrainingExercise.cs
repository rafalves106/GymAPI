using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.Entities;

public class TrainingExercise
{
    public const int MaxRestTime = 120;
    public const int MaxReps = 30;
    public const int MaxSeries = 15;
    public const int MaxExercisesPerTraining = 10;

    public Guid TrainingId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int OrderIndex { get; private set; }
    public int RestTime { get; private set; }
    public int Reps { get; private set; }
    public int Series { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TrainingExercise() { }

    private TrainingExercise(
        Guid trainingId,
        Guid exerciseId,
        int orderIndex,
        int restTime,
        int reps,
        int series)
    {
        TrainingId = trainingId;
        ExerciseId = exerciseId;
        OrderIndex = orderIndex;
        RestTime = restTime;
        Reps = reps;
        Series = series;
        CreatedAt = DateTime.UtcNow;
    }

    public static TrainingExercise Create(
        Guid trainingId,
        Guid exerciseId,
        int orderIndex,
        int restTime,
        int reps,
        int series)
    {
        if (restTime < 0 || restTime > MaxRestTime)
            throw new DomainException($"Rest time must be between 0 and {MaxRestTime} seconds.");

        if (reps < 1 || reps > MaxReps)
            throw new DomainException($"Reps must be between 1 and {MaxReps}.");

        if (series < 1 || series > MaxSeries)
            throw new DomainException($"Series must be between 1 and {MaxSeries}.");

        return new TrainingExercise(trainingId, exerciseId, orderIndex, restTime, reps, series);
    }

    public static TrainingExercise Restore(
        Guid trainingId,
        Guid exerciseId,
        int orderIndex,
        int restTime,
        int reps,
        int series,
        DateTime createdAt)
    {
        var trainingExercise = new TrainingExercise(trainingId, exerciseId, orderIndex, restTime, reps, series);
        trainingExercise.CreatedAt = createdAt;
        return trainingExercise;
    }

    public void Update(int restTime, int reps, int series)
    {
        if (restTime < 0 || restTime > MaxRestTime)
            throw new DomainException($"Rest time must be between 0 and {MaxRestTime} seconds.");

        if (reps < 1 || reps > MaxReps)
            throw new DomainException($"Reps must be between 1 and {MaxReps}.");

        if (series < 1 || series > MaxSeries)
            throw new DomainException($"Series must be between 1 and {MaxSeries}.");

        RestTime = restTime;
        Reps = reps;
        Series = series;
    }
}
