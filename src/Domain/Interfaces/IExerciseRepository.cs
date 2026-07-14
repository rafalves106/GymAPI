using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;

namespace GymAPI.Domain.Interfaces;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(Guid id);
    Task<IEnumerable<Exercise>> GetAllAsync();
    Task<IEnumerable<Exercise>> GetByMuscleGroupAsync(MuscleGroupType muscleGroup);
    Task<IEnumerable<Exercise>> GetByDifficultyAsync(DifficultyLevel difficultyLevel);
    Task<IEnumerable<Exercise>> SearchByNameAsync(string name);
    Task AddAsync(Exercise exercise);
    void Update(Exercise exercise);
    void Delete(Exercise exercise);
}
