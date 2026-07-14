using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly AppDbContext _context;

    public WorkoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Workout?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Workouts.FindAsync(id);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<Workout?> GetByIdWithExercisesAsync(Guid id)
    {
        var entity = await _context.Workouts
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id);

        return entity is null ? null : MapToDomainWithExercises(entity);
    }

    public async Task<IEnumerable<Workout>> GetByUserIdAsync(Guid userId)
    {
        var entities = await _context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return entities.Select(MapToDomainWithExercises);
    }

    public async Task<IEnumerable<Workout>> GetByUserIdAndDayAsync(Guid userId, DayOfWeek day)
    {
        var entities = await _context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.UserId == userId && w.ScheduledDay == (int)day)
            .ToListAsync();

        return entities.Select(MapToDomainWithExercises);
    }

    public async Task AddAsync(Workout workout)
    {
        var entity = MapToEntity(workout);
        await _context.Workouts.AddAsync(entity);
    }

    public void Update(Workout workout)
    {
        var existing = _context.Workouts
            .Include(w => w.Exercises)
            .FirstOrDefault(w => w.Id == workout.Id);

        if (existing is null) return;

        existing.Name = workout.Name;
        existing.ScheduledDay = workout.ScheduledDay.HasValue ? (int?)workout.ScheduledDay.Value : null;

        _context.Exercises.RemoveRange(existing.Exercises);
        foreach (var ex in workout.Exercises)
        {
            existing.Exercises.Add(new ExerciseEntity
            {
                Id = ex.Id,
                WorkoutId = ex.WorkoutId,
                Name = ex.Name,
                TargetSets = ex.TargetSets,
                TargetReps = ex.TargetReps,
                RestSeconds = ex.RestSeconds,
                Order = ex.Order
            });
        }
    }

    public void Delete(Workout workout)
    {
        var existing = _context.Workouts
            .Include(w => w.Exercises)
            .FirstOrDefault(w => w.Id == workout.Id);

        if (existing is null) return;

        _context.Exercises.RemoveRange(existing.Exercises);
        _context.Workouts.Remove(existing);
    }

    private static Workout MapToDomain(WorkoutEntity entity)
    {
        return Workout.Restore(
            entity.Id,
            entity.Name,
            entity.UserId,
            entity.ScheduledDay.HasValue ? (DayOfWeek)entity.ScheduledDay.Value : null,
            DateTime.UtcNow,
            null);
    }

    private static Workout MapToDomainWithExercises(WorkoutEntity entity)
    {
        var workout = Workout.Restore(
            entity.Id,
            entity.Name,
            entity.UserId,
            entity.ScheduledDay.HasValue ? (DayOfWeek)entity.ScheduledDay.Value : null,
            DateTime.UtcNow,
            null);

        foreach (var ex in entity.Exercises.OrderBy(e => e.Order))
        {
            workout.AddExercise(Exercise.Restore(
                ex.Id,
                ex.WorkoutId,
                ex.Name,
                ex.TargetSets,
                ex.TargetReps,
                ex.RestSeconds,
                ex.Order));
        }

        return workout;
    }

    private static WorkoutEntity MapToEntity(Workout workout)
    {
        return new WorkoutEntity
        {
            Id = workout.Id,
            Name = workout.Name,
            UserId = workout.UserId,
            ScheduledDay = workout.ScheduledDay.HasValue ? (int?)workout.ScheduledDay.Value : null,
            Exercises = workout.Exercises.Select(ex => new ExerciseEntity
            {
                Id = ex.Id,
                WorkoutId = ex.WorkoutId,
                Name = ex.Name,
                TargetSets = ex.TargetSets,
                TargetReps = ex.TargetReps,
                RestSeconds = ex.RestSeconds,
                Order = ex.Order
            }).ToList()
        };
    }
}
