using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.Entities;

public class Workout
{
    private readonly List<Exercise> _exercises = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DayOfWeek? ScheduledDay { get; private set; }
    public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Workout() { }

    private Workout(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = name;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Workout Create(string name, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Workout name is required.");

        return new Workout(Guid.NewGuid(), name.Trim(), userId);
    }

    public static Workout Restore(Guid id, string name, Guid userId, DayOfWeek? scheduledDay, DateTime createdAt, DateTime? updatedAt)
    {
        var workout = new Workout(id, name, userId);
        workout.ScheduledDay = scheduledDay;
        workout.CreatedAt = createdAt;
        workout.UpdatedAt = updatedAt;
        return workout;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Workout name is required.");

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToDay(DayOfWeek? day)
    {
        ScheduledDay = day;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddExercise(Exercise exercise)
    {
        _exercises.Add(exercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveExercise(Guid exerciseId)
    {
        var exercise = _exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise != null)
        {
            _exercises.Remove(exercise);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ReplaceExercises(IEnumerable<Exercise> exercises)
    {
        _exercises.Clear();
        _exercises.AddRange(exercises);
        UpdatedAt = DateTime.UtcNow;
    }
}
