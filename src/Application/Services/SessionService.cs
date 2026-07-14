using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;

namespace GymAPI.Application.Services;

public class SessionService : ISessionUseCases
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SessionService(
        ISessionRepository sessionRepository,
        IWorkoutRepository workoutRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _workoutRepository = workoutRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SessionDto> StartAsync(Guid workoutId, Guid userId)
    {
        var workout = await _workoutRepository.GetByIdWithExercisesAsync(workoutId)
            ?? throw new WorkoutNotFoundException(workoutId);

        if (workout.UserId != userId)
            throw new UnauthorizedAccessException();

        var session = WorkoutSession.Create(userId, workoutId);

        foreach (var exercise in workout.Exercises)
        {
            session.AddProgress(ExerciseProgress.Create(session.Id, exercise.Id));
        }

        await _sessionRepository.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(session);
    }

    public async Task<SessionDto> PauseAsync(Guid sessionId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.Pause());
    }

    public async Task<SessionDto> ResumeAsync(Guid sessionId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.Resume());
    }

    public async Task<SessionDto> CompleteAsync(Guid sessionId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.Complete());
    }

    public async Task<SessionDto> CancelAsync(Guid sessionId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.Cancel());
    }

    public async Task<SessionDto> IncrementAsync(Guid sessionId, Guid exerciseId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.IncrementSet(exerciseId));
    }

    public async Task<SessionDto> DecrementAsync(Guid sessionId, Guid exerciseId, Guid userId)
    {
        return await MutateAsync(sessionId, userId, s => s.DecrementSet(exerciseId));
    }

    private async Task<SessionDto> MutateAsync(Guid sessionId, Guid userId, Action<WorkoutSession> mutate)
    {
        var session = await _sessionRepository.GetByIdWithProgressAsync(sessionId)
            ?? throw new SessionNotFoundException(sessionId);

        if (session.UserId != userId)
            throw new UnauthorizedAccessException();

        mutate(session);

        _sessionRepository.Update(session);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(session);
    }

    private static SessionDto MapToDto(WorkoutSession session)
    {
        return new SessionDto
        {
            Id = session.Id,
            WorkoutId = session.WorkoutId,
            Status = session.Status.ToString(),
            StartedAt = session.StartedAt,
            FinishedAt = session.FinishedAt,
            Progress = session.Progress.Select(p => new ExerciseProgressDto
            {
                ExerciseId = p.ExerciseId,
                CompletedSets = p.CompletedSets
            }).ToList()
        };
    }
}
