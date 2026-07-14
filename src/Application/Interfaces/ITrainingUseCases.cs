using GymAPI.Application.DTOs;

namespace GymAPI.Application.Interfaces;

public interface ITrainingUseCases
{
    Task<TrainingDto> CreateAsync(CreateTrainingRequest request, Guid userId);
    Task<TrainingDto?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<TrainingDto>> GetByUserIdAsync(Guid userId);
    Task<TrainingDto> UpdateAsync(Guid id, UpdateTrainingRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
