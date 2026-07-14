using GymAPI.Domain.Entities;

namespace GymAPI.Domain.Interfaces;

public interface ISessionRepository
{
    Task<WorkoutSession?> GetByIdAsync(Guid id);
    Task<WorkoutSession?> GetByIdWithProgressAsync(Guid id);
    Task AddAsync(WorkoutSession session);
    void Update(WorkoutSession session);
}
