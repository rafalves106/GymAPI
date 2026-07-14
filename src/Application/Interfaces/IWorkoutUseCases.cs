using GymAPI.Application.DTOs;

namespace GymAPI.Application.Interfaces;

public interface IWorkoutUseCases
{
    Task<WorkoutDto> CreateAsync(CreateWorkoutRequest request, Guid userId);
    Task<WorkoutDto?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<WorkoutDto>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<WorkoutDto>> GetTodayAsync(Guid userId, DayOfWeek? day);
    Task<WorkoutDto> UpdateAsync(Guid id, UpdateWorkoutRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task AssignDayAsync(Guid id, AssignDayRequest request, Guid userId);
}
