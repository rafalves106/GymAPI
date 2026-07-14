using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;

namespace GymAPI.Application.Services;

public class TrainingService : ITrainingUseCases
{
    private readonly IUnitOfWork _unitOfWork;

    public TrainingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TrainingDto> CreateAsync(CreateTrainingRequest request, Guid userId)
    {
        var training = Training.Create(
            request.Name,
            request.Description,
            userId);

        foreach (var exerciseRequest in request.Exercises)
        {
            var exercise = await _unitOfWork.Exercises.GetByIdAsync(exerciseRequest.ExerciseId)
                ?? throw new ExerciseNotFoundException(exerciseRequest.ExerciseId);

            var trainingExercise = TrainingExercise.Create(
                training.Id,
                exercise.Id,
                request.Exercises.IndexOf(exerciseRequest),
                exerciseRequest.RestTime,
                exerciseRequest.Reps,
                exerciseRequest.Series);

            training.AddExercise(trainingExercise);
        }

        await _unitOfWork.Trainings.AddAsync(training);
        await _unitOfWork.SaveChangesAsync();

        return await MapToDtoAsync(training);
    }

    public async Task<TrainingDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var training = await _unitOfWork.Trainings.GetByIdWithExercisesAsync(id);
        if (training == null || training.UserId != userId)
            return null;

        return await MapToDtoAsync(training);
    }

    public async Task<IEnumerable<TrainingDto>> GetByUserIdAsync(Guid userId)
    {
        var trainings = await _unitOfWork.Trainings.GetByUserIdAsync(userId);
        var dtos = new List<TrainingDto>();
        foreach (var training in trainings)
        {
            dtos.Add(await MapToDtoAsync(training));
        }
        return dtos;
    }

    public async Task<TrainingDto> UpdateAsync(Guid id, UpdateTrainingRequest request, Guid userId)
    {
        var training = await _unitOfWork.Trainings.GetByIdWithExercisesAsync(id)
            ?? throw new TrainingNotFoundException(id);

        if (training.UserId != userId)
            throw new TrainingNotFoundException(id);

        training.Update(request.Name, request.Description);
        training.ClearExercises();

        foreach (var exerciseRequest in request.Exercises)
        {
            var exercise = await _unitOfWork.Exercises.GetByIdAsync(exerciseRequest.ExerciseId)
                ?? throw new ExerciseNotFoundException(exerciseRequest.ExerciseId);

            var trainingExercise = TrainingExercise.Create(
                training.Id,
                exercise.Id,
                request.Exercises.IndexOf(exerciseRequest),
                exerciseRequest.RestTime,
                exerciseRequest.Reps,
                exerciseRequest.Series);

            training.AddExercise(trainingExercise);
        }

        _unitOfWork.Trainings.Update(training);
        await _unitOfWork.SaveChangesAsync();

        return await MapToDtoAsync(training);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var training = await _unitOfWork.Trainings.GetByIdAsync(id)
            ?? throw new TrainingNotFoundException(id);

        if (training.UserId != userId)
            throw new TrainingNotFoundException(id);

        _unitOfWork.Trainings.Delete(training);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<TrainingDto> MapToDtoAsync(Training training)
    {
        var exercises = new List<TrainingExerciseDto>();
        foreach (var te in training.Exercises)
        {
            var exercise = await _unitOfWork.Exercises.GetByIdAsync(te.ExerciseId);
            exercises.Add(new TrainingExerciseDto
            {
                ExerciseId = te.ExerciseId,
                ExerciseName = exercise?.Name ?? "Unknown",
                OrderIndex = te.OrderIndex,
                RestTime = te.RestTime,
                Reps = te.Reps,
                Series = te.Series
            });
        }

        return new TrainingDto
        {
            Id = training.Id,
            Name = training.Name,
            Description = training.Description,
            Exercises = exercises.OrderBy(e => e.OrderIndex).ToList(),
            CreatedAt = training.CreatedAt,
            UpdatedAt = training.UpdatedAt
        };
    }
}
