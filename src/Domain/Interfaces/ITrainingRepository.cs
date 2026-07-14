using GymAPI.Domain.Entities;

namespace GymAPI.Domain.Interfaces;

public interface ITrainingRepository
{
    Task<Training?> GetByIdAsync(Guid id);
    Task<Training?> GetByIdWithExercisesAsync(Guid id);
    Task<IEnumerable<Training>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Training training);
    void Update(Training training);
    void Delete(Training training);
}
