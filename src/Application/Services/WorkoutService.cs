using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;

namespace GymAPI.Application.Services;

public class WorkoutService : IWorkoutUseCases
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkoutService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkoutDto> CreateAsync(CreateWorkoutRequest request, Guid userId)
    {
        var workout = Workout.Create(request.Name, userId);

        var order = 0;
        foreach (var ex in request.Exercises)
        {
            var exercise = Exercise.Create(
                workout.Id,
                ex.Name,
                ex.TargetSets,
                ex.TargetReps,
                ex.RestSeconds,
                order++);

            workout.AddExercise(exercise);
        }

        await _unitOfWork.Workouts.AddAsync(workout);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(workout);
    }

    public async Task<WorkoutDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdWithExercisesAsync(id);
        if (workout == null || workout.UserId != userId)
            return null;

        return MapToDto(workout);
    }

    public async Task<IEnumerable<WorkoutDto>> GetByUserIdAsync(Guid userId)
    {
        var workouts = await _unitOfWork.Workouts.GetByUserIdAsync(userId);
        return workouts.Select(MapToDto);
    }

    public async Task<IEnumerable<WorkoutDto>> GetTodayAsync(Guid userId, DayOfWeek? day)
    {
        var targetDay = day ?? DateTime.UtcNow.DayOfWeek;
        var workouts = await _unitOfWork.Workouts.GetByUserIdAndDayAsync(userId, targetDay);
        return workouts.Select(MapToDto);
    }

    public async Task<WorkoutDto> UpdateAsync(Guid id, UpdateWorkoutRequest request, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdWithExercisesAsync(id)
            ?? throw new WorkoutNotFoundException(id);

        if (workout.UserId != userId)
            throw new WorkoutNotFoundException(id);

        workout.Rename(request.Name);

        var exercises = new List<Exercise>();
        var order = 0;
        foreach (var ex in request.Exercises)
        {
            exercises.Add(Exercise.Create(
                workout.Id,
                ex.Name,
                ex.TargetSets,
                ex.TargetReps,
                ex.RestSeconds,
                order++));
        }
        workout.ReplaceExercises(exercises);

        _unitOfWork.Workouts.Update(workout);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(workout);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdAsync(id)
            ?? throw new WorkoutNotFoundException(id);

        if (workout.UserId != userId)
            throw new WorkoutNotFoundException(id);

        _unitOfWork.Workouts.Delete(workout);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignDayAsync(Guid id, AssignDayRequest request, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdAsync(id)
            ?? throw new WorkoutNotFoundException(id);

        if (workout.UserId != userId)
            throw new WorkoutNotFoundException(id);

        workout.AssignToDay(request.Day);

        _unitOfWork.Workouts.Update(workout);
        await _unitOfWork.SaveChangesAsync();
    }

    private static WorkoutDto MapToDto(Workout workout)
    {
        return new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            ScheduledDay = workout.ScheduledDay,
            Exercises = workout.Exercises
                .OrderBy(e => e.Order)
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    TargetSets = e.TargetSets,
                    TargetReps = e.TargetReps,
                    RestSeconds = e.RestSeconds,
                    Order = e.Order
                }).ToList()
        };
    }
}
