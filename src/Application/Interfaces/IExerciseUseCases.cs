using GymAPI.Application.DTOs;
using GymAPI.Domain.Enums;

namespace GymAPI.Application.Interfaces;

public interface IExerciseUseCases
{
    Task<ExerciseDto> CreateAsync(CreateExerciseRequest request);
    Task<ExerciseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ExerciseDto>> GetAllAsync();
    Task<IEnumerable<ExerciseDto>> GetByMuscleGroupAsync(MuscleGroupType muscleGroup);
    Task<IEnumerable<ExerciseDto>> GetByDifficultyAsync(DifficultyLevel difficultyLevel);
    Task<IEnumerable<ExerciseDto>> SearchByNameAsync(string name);
    Task<ExerciseDto> UpdateAsync(Guid id, UpdateExerciseRequest request);
    Task DeleteAsync(Guid id);
}
