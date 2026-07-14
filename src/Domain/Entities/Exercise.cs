using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.ValueObjects;

namespace GymAPI.Domain.Entities;

public class Exercise
{
    private readonly List<MuscleGroupType> _muscleGroups = new();
    private readonly List<EquipmentType> _equipments = new();

    public Guid Id { get; private set; }
    public ExerciseName Name { get; private set; }
    public Description Description { get; private set; }
    public IReadOnlyCollection<MuscleGroupType> MuscleGroups => _muscleGroups.AsReadOnly();
    public IReadOnlyCollection<EquipmentType> Equipments => _equipments.AsReadOnly();
    public DifficultyLevel DifficultyLevel { get; private set; }
    public string? VideoUrl { get; private set; }
    public string? ExternalApiId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Exercise()
    {
        Name = null!;
        Description = null!;
    }

    private Exercise(
        Guid id,
        ExerciseName name,
        Description description,
        IEnumerable<MuscleGroupType> muscleGroups,
        IEnumerable<EquipmentType> equipments,
        DifficultyLevel difficultyLevel,
        string? videoUrl,
        string? externalApiId)
    {
        Id = id;
        Name = name;
        Description = description;
        DifficultyLevel = difficultyLevel;
        VideoUrl = videoUrl;
        ExternalApiId = externalApiId;
        CreatedAt = DateTime.UtcNow;

        foreach (var muscleGroup in muscleGroups)
            _muscleGroups.Add(muscleGroup);

        foreach (var equipment in equipments)
            _equipments.Add(equipment);
    }

    public static Exercise Create(
        ExerciseName name,
        Description description,
        IEnumerable<MuscleGroupType> muscleGroups,
        IEnumerable<EquipmentType> equipments,
        DifficultyLevel difficultyLevel,
        string? videoUrl = null,
        string? externalApiId = null)
    {
        if (muscleGroups == null || !muscleGroups.Any())
            throw new DomainException("At least one muscle group is required.");

        if (equipments == null || !equipments.Any())
            throw new DomainException("At least one equipment is required.");

        return new Exercise(
            Guid.NewGuid(),
            name,
            description,
            muscleGroups,
            equipments,
            difficultyLevel,
            videoUrl,
            externalApiId);
    }

    public static Exercise Restore(
        Guid id,
        ExerciseName name,
        Description description,
        IEnumerable<MuscleGroupType> muscleGroups,
        IEnumerable<EquipmentType> equipments,
        DifficultyLevel difficultyLevel,
        string? videoUrl,
        string? externalApiId,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        var exercise = new Exercise(
            id,
            name,
            description,
            muscleGroups,
            equipments,
            difficultyLevel,
            videoUrl,
            externalApiId);

        exercise.CreatedAt = createdAt;
        exercise.UpdatedAt = updatedAt;

        return exercise;
    }

    public void Update(
        ExerciseName name,
        Description description,
        IEnumerable<MuscleGroupType> muscleGroups,
        IEnumerable<EquipmentType> equipments,
        DifficultyLevel difficultyLevel,
        string? videoUrl = null,
        string? externalApiId = null)
    {
        if (muscleGroups == null || !muscleGroups.Any())
            throw new DomainException("At least one muscle group is required.");

        if (equipments == null || !equipments.Any())
            throw new DomainException("At least one equipment is required.");

        Name = name;
        Description = description;
        DifficultyLevel = difficultyLevel;
        VideoUrl = videoUrl;
        ExternalApiId = externalApiId;
        UpdatedAt = DateTime.UtcNow;

        _muscleGroups.Clear();
        foreach (var muscleGroup in muscleGroups)
            _muscleGroups.Add(muscleGroup);

        _equipments.Clear();
        foreach (var equipment in equipments)
            _equipments.Add(equipment);
    }
}
