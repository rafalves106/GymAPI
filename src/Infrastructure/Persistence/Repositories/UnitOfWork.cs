using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _context;
    private IExerciseRepository? _exerciseRepository;
    private ITrainingRepository? _trainingRepository;

    public UnitOfWork(GymDbContext context)
    {
        _context = context;
    }

    public IExerciseRepository Exercises =>
        _exerciseRepository ??= new ExerciseRepository(_context);

    public ITrainingRepository Trainings =>
        _trainingRepository ??= new TrainingRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
