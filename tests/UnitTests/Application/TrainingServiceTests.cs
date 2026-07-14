using FluentAssertions;
using GymAPI.Application.DTOs;
using GymAPI.Application.Services;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.Interfaces;
using Moq;

namespace GymAPI.UnitTests.Application;

public class TrainingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<ITrainingRepository> _trainingRepositoryMock;
    private readonly TrainingService _sut;

    public TrainingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _trainingRepositoryMock = new Mock<ITrainingRepository>();
        _unitOfWorkMock.Setup(u => u.Exercises).Returns(_exerciseRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Trainings).Returns(_trainingRepositoryMock.Object);
        _sut = new TrainingService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsTrainingDto()
    {
        var userId = Guid.NewGuid();
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Intermediate);

        _exerciseRepositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new CreateTrainingRequest
        {
            Name = "Push Day",
            Description = "Chest and shoulders",
            Exercises = new List<CreateTrainingExerciseRequest>
            {
                new()
                {
                    ExerciseId = exercise.Id,
                    RestTime = 90,
                    Reps = 10,
                    Series = 3
                }
            }
        };

        var result = await _sut.CreateAsync(request, userId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Push Day");
        result.Exercises.Should().HaveCount(1);
        _trainingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Training>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NonExistentExercise_ThrowsNotFoundException()
    {
        _exerciseRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Exercise?)null);

        var request = new CreateTrainingRequest
        {
            Name = "Push Day",
            Exercises = new List<CreateTrainingExerciseRequest>
            {
                new()
                {
                    ExerciseId = Guid.NewGuid(),
                    RestTime = 90,
                    Reps = 10,
                    Series = 3
                }
            }
        };

        var act = () => _sut.CreateAsync(request, Guid.NewGuid());

        await act.Should().ThrowAsync<ExerciseNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTraining_ReturnsDto()
    {
        var userId = Guid.NewGuid();
        var training = Training.Create("Push Day", "Description", userId);
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Intermediate);

        training.AddExercise(TrainingExercise.Create(
            training.Id,
            exercise.Id,
            0,
            90,
            10,
            3));

        _trainingRepositoryMock.Setup(r => r.GetByIdWithExercisesAsync(training.Id))
            .ReturnsAsync(training);
        _exerciseRepositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);

        var result = await _sut.GetByIdAsync(training.Id, userId);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Push Day");
        result.Exercises.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_DifferentUser_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var training = Training.Create("Push Day", "Description", userId);

        _trainingRepositoryMock.Setup(r => r.GetByIdWithExercisesAsync(training.Id))
            .ReturnsAsync(training);

        var result = await _sut.GetByIdAsync(training.Id, Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentTraining_ReturnsNull()
    {
        _trainingRepositoryMock.Setup(r => r.GetByIdWithExercisesAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Training?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserTrainings()
    {
        var userId = Guid.NewGuid();
        var trainings = new List<Training>
        {
            Training.Create("Push Day", null, userId),
            Training.Create("Leg Day", null, userId)
        };

        _trainingRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(trainings);

        var result = await _sut.GetByUserIdAsync(userId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingTraining_ReturnsUpdatedDto()
    {
        var userId = Guid.NewGuid();
        var training = Training.Create("Push Day", "Old description", userId);
        var exercise = Exercise.Create(
            "Bench Press",
            "Chest exercise",
            new[] { MuscleGroupType.Chest },
            new[] { EquipmentType.Barbell },
            DifficultyLevel.Intermediate);

        training.AddExercise(TrainingExercise.Create(
            training.Id,
            exercise.Id,
            0,
            90,
            10,
            3));

        _trainingRepositoryMock.Setup(r => r.GetByIdWithExercisesAsync(training.Id))
            .ReturnsAsync(training);
        _exerciseRepositoryMock.Setup(r => r.GetByIdAsync(exercise.Id))
            .ReturnsAsync(exercise);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new UpdateTrainingRequest
        {
            Name = "Pull Day",
            Description = "New description",
            Exercises = new List<UpdateTrainingExerciseRequest>
            {
                new()
                {
                    ExerciseId = exercise.Id,
                    RestTime = 60,
                    Reps = 12,
                    Series = 4
                }
            }
        };

        var result = await _sut.UpdateAsync(training.Id, request, userId);

        result.Name.Should().Be("Pull Day");
        result.Exercises.Should().HaveCount(1);
        result.Exercises[0].RestTime.Should().Be(60);
    }

    [Fact]
    public async Task UpdateAsync_DifferentUser_ThrowsNotFoundException()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());

        _trainingRepositoryMock.Setup(r => r.GetByIdWithExercisesAsync(training.Id))
            .ReturnsAsync(training);

        var request = new UpdateTrainingRequest
        {
            Name = "Pull Day",
            Exercises = new List<UpdateTrainingExerciseRequest>()
        };

        var act = () => _sut.UpdateAsync(training.Id, request, Guid.NewGuid());

        await act.Should().ThrowAsync<TrainingNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingTraining_CallsDelete()
    {
        var userId = Guid.NewGuid();
        var training = Training.Create("Push Day", null, userId);

        _trainingRepositoryMock.Setup(r => r.GetByIdAsync(training.Id))
            .ReturnsAsync(training);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.DeleteAsync(training.Id, userId);

        _trainingRepositoryMock.Verify(r => r.Delete(training), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DifferentUser_ThrowsNotFoundException()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());

        _trainingRepositoryMock.Setup(r => r.GetByIdAsync(training.Id))
            .ReturnsAsync(training);

        var act = () => _sut.DeleteAsync(training.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<TrainingNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentTraining_ThrowsNotFoundException()
    {
        _trainingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Training?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<TrainingNotFoundException>();
    }
}
