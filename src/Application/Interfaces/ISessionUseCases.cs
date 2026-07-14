using GymAPI.Application.DTOs;

namespace GymAPI.Application.Interfaces;

public interface ISessionUseCases
{
    Task<SessionDto> StartAsync(Guid workoutId, Guid userId);
    Task<SessionDto> PauseAsync(Guid sessionId, Guid userId);
    Task<SessionDto> ResumeAsync(Guid sessionId, Guid userId);
    Task<SessionDto> CompleteAsync(Guid sessionId, Guid userId);
    Task<SessionDto> CancelAsync(Guid sessionId, Guid userId);
    Task<SessionDto> IncrementAsync(Guid sessionId, Guid exerciseId, Guid userId);
    Task<SessionDto> DecrementAsync(Guid sessionId, Guid exerciseId, Guid userId);
}
