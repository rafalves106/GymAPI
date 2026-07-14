using GymAPI.Domain.Entities;

namespace GymAPI.Domain.Interfaces;

public interface IWorkoutRepository
{
    Task<Workout?> GetByIdAsync(Guid id);
    Task<Workout?> GetByIdWithExercisesAsync(Guid id);
    Task<IEnumerable<Workout>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Workout>> GetByUserIdAndDayAsync(Guid userId, DayOfWeek day);
    Task AddAsync(Workout workout);
    void Update(Workout workout);
    void Delete(Workout workout);
}
