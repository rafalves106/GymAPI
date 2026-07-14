namespace GymAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IExerciseRepository Exercises { get; }
    ITrainingRepository Trainings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
