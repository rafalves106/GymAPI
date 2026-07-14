using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.Entities;

public class Exercise
{
    public Guid Id { get; private set; }
    public Guid WorkoutId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int TargetSets { get; private set; }
    public int TargetReps { get; private set; }
    public int RestSeconds { get; private set; }
    public int Order { get; private set; }

    private Exercise() { }

    private Exercise(Guid id, Guid workoutId, string name, int targetSets, int targetReps, int restSeconds, int order)
    {
        Id = id;
        WorkoutId = workoutId;
        Name = name;
        TargetSets = targetSets;
        TargetReps = targetReps;
        RestSeconds = restSeconds;
        Order = order;
    }

    public static Exercise Create(Guid workoutId, string name, int targetSets, int targetReps, int restSeconds, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Exercise name is required.");

        if (targetSets <= 0)
            throw new DomainException("Target sets must be greater than 0.");

        if (restSeconds < 0)
            throw new DomainException("Rest seconds cannot be negative.");

        return new Exercise(Guid.NewGuid(), workoutId, name.Trim(), targetSets, targetReps, restSeconds, order);
    }

    public static Exercise Restore(Guid id, Guid workoutId, string name, int targetSets, int targetReps, int restSeconds, int order)
    {
        return new Exercise(id, workoutId, name, targetSets, targetReps, restSeconds, order);
    }

    public void Update(string name, int targetSets, int targetReps, int restSeconds, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Exercise name is required.");

        if (targetSets <= 0)
            throw new DomainException("Target sets must be greater than 0.");

        if (restSeconds < 0)
            throw new DomainException("Rest seconds cannot be negative.");

        Name = name.Trim();
        TargetSets = targetSets;
        TargetReps = targetReps;
        RestSeconds = restSeconds;
        Order = order;
    }
}
