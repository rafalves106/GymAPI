using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.Entities;

public class WorkoutSession
{
    private readonly List<ExerciseProgress> _progress = new();

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid WorkoutId { get; private set; }
    public SessionStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public IReadOnlyCollection<ExerciseProgress> Progress => _progress.AsReadOnly();

    private WorkoutSession() { }

    private WorkoutSession(Guid id, Guid userId, Guid workoutId)
    {
        Id = id;
        UserId = userId;
        WorkoutId = workoutId;
        Status = SessionStatus.Running;
        StartedAt = DateTime.UtcNow;
    }

    public static WorkoutSession Create(Guid userId, Guid workoutId)
    {
        return new WorkoutSession(Guid.NewGuid(), userId, workoutId);
    }

    public static WorkoutSession Restore(Guid id, Guid userId, Guid workoutId, SessionStatus status, DateTime startedAt, DateTime? finishedAt)
    {
        var session = new WorkoutSession(id, userId, workoutId);
        session.Status = status;
        session.StartedAt = startedAt;
        session.FinishedAt = finishedAt;
        return session;
    }

    public void AddProgress(ExerciseProgress progress)
    {
        _progress.Add(progress);
    }

    public void SetProgress(IEnumerable<ExerciseProgress> progressList)
    {
        _progress.Clear();
        _progress.AddRange(progressList);
    }

    public void Pause()
    {
        if (Status != SessionStatus.Running)
            throw new DomainException("Only a running session can be paused.");

        Status = SessionStatus.Paused;
    }

    public void Resume()
    {
        if (Status != SessionStatus.Paused)
            throw new DomainException("Only a paused session can be resumed.");

        Status = SessionStatus.Running;
    }

    public void Complete()
    {
        if (Status == SessionStatus.Completed || Status == SessionStatus.Cancelled)
            throw new DomainException("Cannot complete a session that is already completed or cancelled.");

        Status = SessionStatus.Completed;
        FinishedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == SessionStatus.Completed || Status == SessionStatus.Cancelled)
            throw new DomainException("Cannot cancel a session that is already completed or cancelled.");

        Status = SessionStatus.Cancelled;
        FinishedAt = DateTime.UtcNow;
    }

    public void IncrementSet(Guid exerciseId)
    {
        var progress = _progress.FirstOrDefault(p => p.ExerciseId == exerciseId)
            ?? throw new DomainException($"No progress entry found for exercise {exerciseId}.");

        progress.Increment();
    }

    public void DecrementSet(Guid exerciseId)
    {
        var progress = _progress.FirstOrDefault(p => p.ExerciseId == exerciseId)
            ?? throw new DomainException($"No progress entry found for exercise {exerciseId}.");

        progress.Decrement();
    }
}

public enum SessionStatus
{
    Running = 0,
    Paused = 1,
    Completed = 2,
    Cancelled = 3
}
