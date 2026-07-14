using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class TrainingRepository : ITrainingRepository
{
    private readonly GymDbContext _context;

    public TrainingRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<Training?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Trainings
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<Training?> GetByIdWithExercisesAsync(Guid id)
    {
        var entity = await _context.Trainings
            .Include(t => t.TrainingExercises)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDomainWithExercises(entity);
    }

    public async Task<IEnumerable<Training>> GetByUserIdAsync(Guid userId)
    {
        var entities = await _context.Trainings
            .Include(t => t.TrainingExercises)
            .Where(e => e.UserId == userId)
            .ToListAsync();

        return entities.Select(MapToDomainWithExercises);
    }

    public async Task AddAsync(Training training)
    {
        var entity = MapToEntity(training);
        await _context.Trainings.AddAsync(entity);
    }

    public void Update(Training training)
    {
        var existing = _context.Trainings
            .Include(t => t.TrainingExercises)
            .FirstOrDefault(e => e.Id == training.Id);

        if (existing is null) return;

        existing.Name = training.Name;
        existing.Description = training.Description;
        existing.UpdatedAt = training.UpdatedAt;

        _context.TrainingExercises.RemoveRange(existing.TrainingExercises);
        foreach (var te in training.Exercises)
        {
            existing.TrainingExercises.Add(new TrainingExerciseEntity
            {
                TrainingId = te.TrainingId,
                ExerciseId = te.ExerciseId,
                OrderIndex = te.OrderIndex,
                RestTime = te.RestTime,
                Reps = te.Reps,
                Series = te.Series,
                CreatedAt = te.CreatedAt
            });
        }
    }

    public void Delete(Training training)
    {
        var existing = _context.Trainings
            .Include(t => t.TrainingExercises)
            .FirstOrDefault(e => e.Id == training.Id);

        if (existing is null) return;

        _context.TrainingExercises.RemoveRange(existing.TrainingExercises);
        _context.Trainings.Remove(existing);
    }

    private static Training MapToDomain(TrainingEntity entity)
    {
        return Training.Restore(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.UserId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    private static Training MapToDomainWithExercises(TrainingEntity entity)
    {
        var training = Training.Restore(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.UserId,
            entity.CreatedAt,
            entity.UpdatedAt);

        foreach (var te in entity.TrainingExercises.OrderBy(e => e.OrderIndex))
        {
            var trainingExercise = TrainingExercise.Restore(
                te.TrainingId,
                te.ExerciseId,
                te.OrderIndex,
                te.RestTime,
                te.Reps,
                te.Series,
                te.CreatedAt);

            training.AddExercise(trainingExercise);
        }

        return training;
    }

    private static TrainingEntity MapToEntity(Training training)
    {
        return new TrainingEntity
        {
            Id = training.Id,
            Name = training.Name,
            Description = training.Description,
            UserId = training.UserId,
            CreatedAt = training.CreatedAt,
            UpdatedAt = training.UpdatedAt,
            TrainingExercises = training.Exercises
                .Select(te => new TrainingExerciseEntity
                {
                    TrainingId = te.TrainingId,
                    ExerciseId = te.ExerciseId,
                    OrderIndex = te.OrderIndex,
                    RestTime = te.RestTime,
                    Reps = te.Reps,
                    Series = te.Series,
                    CreatedAt = te.CreatedAt
                }).ToList()
        };
    }
}
