using FluentAssertions;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;

namespace GymAPI.UnitTests.Domain;

public class ExerciseTests
{
    [Fact]
    public void Create_ValidExercise_ReturnsNewExercise()
    {
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise with barbell",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Intermediate);

        exercise.Id.Should().NotBe(Guid.Empty);
        exercise.Name.Value.Should().Be("Bench Press");
        exercise.Description.Value.Should().Be("Chest exercise with barbell");
        exercise.MuscleGroups.Should().Contain(MuscleGroupType.Chest);
        exercise.Equipments.Should().Contain(EquipmentType.Barbell);
        exercise.DifficultyLevel.Should().Be(DifficultyLevel.Intermediate);
        exercise.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        exercise.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithVideoUrl_SetsVideoUrl()
    {
        var exercise = Exercise.Create(
            "Squat",
            "Leg exercise",
            new[] { MuscleGroupType.Quadriceps },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Advanced,
            "https://youtube.com/watch?v=123");

        exercise.VideoUrl.Should().Be("https://youtube.com/watch?v=123");
    }

    [Fact]
    public void Create_NoMuscleGroups_ThrowsDomainException()
    {
        var act = () => Exercise.Create(
            "Test",
            "Description",
            Array.Empty<MuscleGroupType>(),
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Beginner);

        act.Should().Throw<DomainException>()
            .WithMessage("*muscle group*");
    }

    [Fact]
    public void Create_NullMuscleGroups_ThrowsDomainException()
    {
        var act = () => Exercise.Create(
            "Test",
            "Description",
            null!,
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Beginner);

        act.Should().Throw<DomainException>()
            .WithMessage("*muscle group*");
    }

    [Fact]
    public void Create_NoEquipments_ThrowsDomainException()
    {
        var act = () => Exercise.Create(
            "Test",
            "Description",
            new[] { MuscleGroupType.Chest },
            Array.Empty<EquipmentType>(),
            DifficultyLevel.Beginner);

        act.Should().Throw<DomainException>()
            .WithMessage("*equipment*");
    }

    [Fact]
    public void Update_ValidData_UpdatesAllFields()
    {
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Beginner);

        exercise.Update(
            "Incline Bench Press",
            "Upper chest exercise",
            new[] { MuscleGroupType.Chest, MuscleGroupType.Shoulders },
            new[] { EquipmentType.Barbell, EquipmentType.Bench },
            DifficultyLevel.Intermediate,
            "https://youtube.com/watch?v=456");

        exercise.Name.Value.Should().Be("Incline Bench Press");
        exercise.Description.Value.Should().Be("Upper chest exercise");
        exercise.MuscleGroups.Should().HaveCount(2);
        exercise.Equipments.Should().HaveCount(2);
        exercise.DifficultyLevel.Should().Be(DifficultyLevel.Intermediate);
        exercise.VideoUrl.Should().Be("https://youtube.com/watch?v=456");
        exercise.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Restore_PreservesAllData()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var exercise = Exercise.Restore(
            Guid.NewGuid(),
            "Deadlift",
            "Back exercise",
            new[] { MuscleGroupType.Back },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Advanced,
            "https://youtube.com/watch?v=789",
            "ext-123",
            createdAt,
            updatedAt);

        exercise.Id.Should().NotBe(Guid.Empty);
        exercise.Name.Value.Should().Be("Deadlift");
        exercise.CreatedAt.Should().Be(createdAt);
        exercise.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Create_MultipleMuscleGroups_AllAdded()
    {
        var muscleGroups = new[] { MuscleGroupType.Chest, MuscleGroupType.Triceps, MuscleGroupType.Shoulders };

        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            muscleGroups,
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Intermediate);

        exercise.MuscleGroups.Should().HaveCount(3);
        exercise.MuscleGroups.Should().Contain(muscleGroups);
    }
}
