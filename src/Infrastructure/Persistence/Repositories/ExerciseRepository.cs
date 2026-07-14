using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly GymDbContext _context;

    public ExerciseRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<IEnumerable<Exercise>> GetAllAsync()
    {
        var entities = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<IEnumerable<Exercise>> GetByMuscleGroupAsync(MuscleGroupType muscleGroup)
    {
        var entities = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .Where(e => e.ExerciseMuscleGroups.Any(mg => mg.MuscleGroup == muscleGroup))
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<IEnumerable<Exercise>> GetByDifficultyAsync(DifficultyLevel difficultyLevel)
    {
        var entities = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .Where(e => e.DifficultyLevel == difficultyLevel)
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<IEnumerable<Exercise>> SearchByNameAsync(string name)
    {
        var entities = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .Where(e => e.Name.Contains(name))
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task AddAsync(Exercise exercise)
    {
        var entity = MapToEntity(exercise);
        await _context.Exercises.AddAsync(entity);
    }

    public void Update(Exercise exercise)
    {
        var existing = _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .FirstOrDefault(e => e.Id == exercise.Id);

        if (existing is null) return;

        existing.Name = exercise.Name;
        existing.Description = exercise.Description;
        existing.DifficultyLevel = exercise.DifficultyLevel;
        existing.VideoUrl = exercise.VideoUrl;
        existing.UpdatedAt = exercise.UpdatedAt;

        _context.ExerciseMuscleGroups.RemoveRange(existing.ExerciseMuscleGroups);
        foreach (var mg in exercise.MuscleGroups)
        {
            existing.ExerciseMuscleGroups.Add(new ExerciseMuscleGroupEntity
            {
                ExerciseId = exercise.Id,
                MuscleGroup = mg
            });
        }

        _context.ExerciseEquipments.RemoveRange(existing.ExerciseEquipments);
        foreach (var eq in exercise.Equipments)
        {
            existing.ExerciseEquipments.Add(new ExerciseEquipmentEntity
            {
                ExerciseId = exercise.Id,
                EquipmentType = eq
            });
        }
    }

    public void Delete(Exercise exercise)
    {
        var existing = _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseEquipments)
            .FirstOrDefault(e => e.Id == exercise.Id);

        if (existing is null) return;

        _context.ExerciseMuscleGroups.RemoveRange(existing.ExerciseMuscleGroups);
        _context.ExerciseEquipments.RemoveRange(existing.ExerciseEquipments);
        _context.Exercises.Remove(existing);
    }

    private static Exercise MapToDomain(ExerciseEntity entity)
    {
        return Exercise.Restore(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.ExerciseMuscleGroups.Select(mg => mg.MuscleGroup),
            entity.ExerciseEquipments.Select(eq => eq.EquipmentType),
            entity.DifficultyLevel,
            entity.VideoUrl,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    private static ExerciseEntity MapToEntity(Exercise exercise)
    {
        return new ExerciseEntity
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            DifficultyLevel = exercise.DifficultyLevel,
            VideoUrl = exercise.VideoUrl,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            ExerciseMuscleGroups = exercise.MuscleGroups
                .Select(mg => new ExerciseMuscleGroupEntity
                {
                    ExerciseId = exercise.Id,
                    MuscleGroup = mg
                }).ToList(),
            ExerciseEquipments = exercise.Equipments
                .Select(eq => new ExerciseEquipmentEntity
                {
                    ExerciseId = exercise.Id,
                    EquipmentType = eq
                }).ToList()
        };
    }
}
