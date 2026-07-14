using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Repositories;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IWorkoutRepository? _workoutRepository;
    private ISessionRepository? _sessionRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IWorkoutRepository Workouts =>
        _workoutRepository ??= new WorkoutRepository(_context);

    public ISessionRepository Sessions =>
        _sessionRepository ??= new SessionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
