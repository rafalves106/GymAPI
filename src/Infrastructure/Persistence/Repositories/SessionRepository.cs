using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;

    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkoutSession?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Sessions.FindAsync(id);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<WorkoutSession?> GetByIdWithProgressAsync(Guid id)
    {
        var entity = await _context.Sessions
            .Include(s => s.Progress)
            .FirstOrDefaultAsync(s => s.Id == id);

        return entity is null ? null : MapToDomainWithProgress(entity);
    }

    public async Task AddAsync(WorkoutSession session)
    {
        var entity = MapToEntity(session);
        await _context.Sessions.AddAsync(entity);
    }

    public void Update(WorkoutSession session)
    {
        var existing = _context.Sessions
            .Include(s => s.Progress)
            .FirstOrDefault(s => s.Id == session.Id);

        if (existing is null) return;

        existing.Status = (int)session.Status;
        existing.FinishedAt = session.FinishedAt;

        _context.ExerciseProgress.RemoveRange(existing.Progress);
        foreach (var p in session.Progress)
        {
            existing.Progress.Add(new ExerciseProgressEntity
            {
                Id = p.Id,
                SessionId = p.SessionId,
                ExerciseId = p.ExerciseId,
                CompletedSets = p.CompletedSets
            });
        }
    }

    private static WorkoutSession MapToDomain(SessionEntity entity)
    {
        return WorkoutSession.Restore(
            entity.Id,
            entity.UserId,
            entity.WorkoutId,
            (SessionStatus)entity.Status,
            entity.StartedAt,
            entity.FinishedAt);
    }

    private static WorkoutSession MapToDomainWithProgress(SessionEntity entity)
    {
        var session = WorkoutSession.Restore(
            entity.Id,
            entity.UserId,
            entity.WorkoutId,
            (SessionStatus)entity.Status,
            entity.StartedAt,
            entity.FinishedAt);

        foreach (var p in entity.Progress)
        {
            session.AddProgress(ExerciseProgress.Restore(
                p.Id,
                p.SessionId,
                p.ExerciseId,
                p.CompletedSets));
        }

        return session;
    }

    private static SessionEntity MapToEntity(WorkoutSession session)
    {
        return new SessionEntity
        {
            Id = session.Id,
            UserId = session.UserId,
            WorkoutId = session.WorkoutId,
            Status = (int)session.Status,
            StartedAt = session.StartedAt,
            FinishedAt = session.FinishedAt,
            Progress = session.Progress.Select(p => new ExerciseProgressEntity
            {
                Id = p.Id,
                SessionId = p.SessionId,
                ExerciseId = p.ExerciseId,
                CompletedSets = p.CompletedSets
            }).ToList()
        };
    }
}
