using FluentAssertions;
using GymAPI.Application.DTOs;
using GymAPI.Application.Services;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;
using Moq;

namespace GymAPI.UnitTests.Application;

public class ExerciseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExerciseRepository> _repositoryMock;
    private readonly ExerciseService _sut;

    public ExerciseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IExerciseRepository>();
        _unitOfWorkMock.Setup(u => u.Exercises).Returns(_repositoryMock.Object);
        _sut = new ExerciseService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsExerciseDto()
    {
        var request = new CreateExerciseRequest
        {
            Name = "Bench Press",
            Description = "Chest exercise with barbell",
            MuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Chest },
            Equipments = new List<EquipmentType> { EquipmentType.Barbell },
            DifficultyLevel = DifficultyLevel.Intermediate
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Exercise>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _sut.CreateAsync(request);

        result.Should().NotBeNull();
        result.Name.Should().Be("Bench Press");
        result.MuscleGroups.Should().Contain(MuscleGroupType.Chest);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Exercise>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingExercise_ReturnsDto()
    {
        var exercise = Exercise.Create(
            "Squat",
            "Leg exercise",
            new[] { MuscleGroupType.Quadriceps },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Advanced);

        _repositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);

        var result = await _sut.GetByIdAsync(exercise.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Squat");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentExercise_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Exercise?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllExercises()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Create("Bench Press", "Chest", new[] { MuscleGroupType.Chest }, new[] { EquipmentType.Barbell }, DifficultyLevel.Beginner),
            Exercise.Create("Squat", "Legs", new[] { MuscleGroupType.Quadriceps }, new[] { EquipmentType.Barbell }, DifficultyLevel.Intermediate)
        };

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(exercises);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingExercise_ReturnsUpdatedDto()
    {
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Beginner);

        _repositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new UpdateExerciseRequest
        {
            Name = "Incline Bench Press",
            Description = "Upper chest exercise",
            MuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Chest, MuscleGroupType.Shoulders },
            Equipments = new List<EquipmentType> { EquipmentType.Barbell },
            DifficultyLevel = DifficultyLevel.Intermediate
        };

        var result = await _sut.UpdateAsync(exercise.Id, request);

        result.Name.Should().Be("Incline Bench Press");
        result.MuscleGroups.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_NonExistentExercise_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Exercise?)null);

        var request = new UpdateExerciseRequest
        {
            Name = "Test",
            Description = "Test description",
            MuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Chest },
            Equipments = new List<EquipmentType> { EquipmentType.Barbell },
            DifficultyLevel = DifficultyLevel.Beginner
        };

        var act = () => _sut.UpdateAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ExerciseNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingExercise_CallsDelete()
    {
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Beginner);

        _repositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.DeleteAsync(exercise.Id);

        _repositoryMock.Verify(r => r.Delete(exercise), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentExercise_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Exercise?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<ExerciseNotFoundException>();
    }
}
