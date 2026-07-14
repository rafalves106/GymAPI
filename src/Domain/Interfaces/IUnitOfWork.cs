namespace GymAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWorkoutRepository Workouts { get; }
    ISessionRepository Sessions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
