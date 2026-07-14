using GymAPI.Domain.Exceptions;
using GymAPI.Domain.ValueObjects;

namespace GymAPI.Domain.Entities;

public class Training
{
    private readonly List<TrainingExercise> _exercises = new();

    public Guid Id { get; private set; }
    public TrainingName Name { get; private set; }
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public IReadOnlyCollection<TrainingExercise> Exercises => _exercises.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Training()
    {
        Name = null!;
    }

    private Training(
        Guid id,
        TrainingName name,
        string? description,
        Guid userId)
    {
        Id = id;
        Name = name;
        Description = description;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Training Create(
        TrainingName name,
        string? description,
        Guid userId)
    {
        return new Training(Guid.NewGuid(), name, description, userId);
    }

    public static Training Restore(
        Guid id,
        TrainingName name,
        string? description,
        Guid userId,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        var training = new Training(id, name, description, userId);
        training.CreatedAt = createdAt;
        training.UpdatedAt = updatedAt;
        return training;
    }

    public void Update(
        TrainingName name,
        string? description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddExercise(TrainingExercise exercise)
    {
        if (_exercises.Count >= 10)
            throw new DomainException("A training can have at most 10 exercises.");

        _exercises.Add(exercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveExercise(Guid exerciseId)
    {
        var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);
        if (exercise != null)
        {
            _exercises.Remove(exercise);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearExercises()
    {
        _exercises.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExercises(IEnumerable<TrainingExercise> exercises)
    {
        var exerciseList = exercises.ToList();
        if (exerciseList.Count > 10)
            throw new DomainException("A training can have at most 10 exercises.");

        _exercises.Clear();
        _exercises.AddRange(exerciseList);
        UpdatedAt = DateTime.UtcNow;
    }
}
