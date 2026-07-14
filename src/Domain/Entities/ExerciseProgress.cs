namespace GymAPI.Domain.Entities;

public class ExerciseProgress
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int CompletedSets { get; private set; }

    private ExerciseProgress() { }

    private ExerciseProgress(Guid id, Guid sessionId, Guid exerciseId)
    {
        Id = id;
        SessionId = sessionId;
        ExerciseId = exerciseId;
        CompletedSets = 0;
    }

    public static ExerciseProgress Create(Guid sessionId, Guid exerciseId)
    {
        return new ExerciseProgress(Guid.NewGuid(), sessionId, exerciseId);
    }

    public static ExerciseProgress Restore(Guid id, Guid sessionId, Guid exerciseId, int completedSets)
    {
        var progress = new ExerciseProgress(id, sessionId, exerciseId);
        progress.CompletedSets = completedSets;
        return progress;
    }

    public void Increment()
    {
        CompletedSets++;
    }

    public void Decrement()
    {
        if (CompletedSets > 0)
            CompletedSets--;
    }
}
