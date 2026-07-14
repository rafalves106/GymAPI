using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;

namespace GymAPI.Application.Services;

public class ExerciseService : IExerciseUseCases
{
    private readonly IUnitOfWork _unitOfWork;

    public ExerciseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ExerciseDto> CreateAsync(CreateExerciseRequest request)
    {
        var exercise = Exercise.Create(
            request.Name,
            request.Description,
            request.MuscleGroups,
            request.Equipments,
            request.DifficultyLevel,
            request.VideoUrl,
            request.ExternalApiId);

        await _unitOfWork.Exercises.AddAsync(exercise);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(exercise);
    }

    public async Task<ExerciseDto?> GetByIdAsync(Guid id)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);
        return exercise is null ? null : MapToDto(exercise);
    }

    public async Task<IEnumerable<ExerciseDto>> GetAllAsync()
    {
        var exercises = await _unitOfWork.Exercises.GetAllAsync();
        return exercises.Select(MapToDto);
    }

    public async Task<IEnumerable<ExerciseDto>> GetByMuscleGroupAsync(MuscleGroupType muscleGroup)
    {
        var exercises = await _unitOfWork.Exercises.GetByMuscleGroupAsync(muscleGroup);
        return exercises.Select(MapToDto);
    }

    public async Task<IEnumerable<ExerciseDto>> GetByDifficultyAsync(DifficultyLevel difficultyLevel)
    {
        var exercises = await _unitOfWork.Exercises.GetByDifficultyAsync(difficultyLevel);
        return exercises.Select(MapToDto);
    }

    public async Task<IEnumerable<ExerciseDto>> SearchByNameAsync(string name)
    {
        var exercises = await _unitOfWork.Exercises.SearchByNameAsync(name);
        return exercises.Select(MapToDto);
    }

    public async Task<ExerciseDto> UpdateAsync(Guid id, UpdateExerciseRequest request)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id)
            ?? throw new ExerciseNotFoundException(id);

        exercise.Update(
            request.Name,
            request.Description,
            request.MuscleGroups,
            request.Equipments,
            request.DifficultyLevel,
            request.VideoUrl,
            request.ExternalApiId);

        _unitOfWork.Exercises.Update(exercise);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(exercise);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id)
            ?? throw new ExerciseNotFoundException(id);

        _unitOfWork.Exercises.Delete(exercise);
        await _unitOfWork.SaveChangesAsync();
    }

    private static ExerciseDto MapToDto(Exercise exercise) => new()
    {
        Id = exercise.Id,
        Name = exercise.Name,
        Description = exercise.Description,
        MuscleGroups = exercise.MuscleGroups.ToList(),
        Equipments = exercise.Equipments.ToList(),
        DifficultyLevel = exercise.DifficultyLevel,
        VideoUrl = exercise.VideoUrl,
        ExternalApiId = exercise.ExternalApiId,
        CreatedAt = exercise.CreatedAt,
        UpdatedAt = exercise.UpdatedAt
    };
}
