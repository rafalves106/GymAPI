using FluentAssertions;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Enums;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.ValueObjects;

namespace GymAPI.UnitTests.Domain;

public class TrainingTests
{
    [Fact]
    public void Create_ValidTraining_ReturnsNewTraining()
    {
        var userId = Guid.NewGuid();

        var training = Training.Create(
            "Push Day",
            "Chest and shoulders workout",
            userId);

        training.Id.Should().NotBe(Guid.Empty);
        training.Name.Value.Should().Be("Push Day");
        training.Description.Should().Be("Chest and shoulders workout");
        training.UserId.Should().Be(userId);
        training.Exercises.Should().BeEmpty();
        training.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        training.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithoutDescription_ReturnsTraining()
    {
        var training = Training.Create(
            "Leg Day",
            null,
            Guid.NewGuid());

        training.Description.Should().BeNull();
    }

    [Fact]
    public void Update_UpdatesAllFields()
    {
        var training = Training.Create(
            "Push Day",
            "Old description",
            Guid.NewGuid());

        training.Update("Pull Day", "New description");

        training.Name.Value.Should().Be("Pull Day");
        training.Description.Should().Be("New description");
        training.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddExercise_ValidExercise_AddsToCollection()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());
        var exercise = TrainingExercise.Create(
            training.Id,
            Guid.NewGuid(),
            0,
            90,
            10,
            3);

        training.AddExercise(exercise);

        training.Exercises.Should().HaveCount(1);
        training.Exercises.Should().Contain(exercise);
    }

    [Fact]
    public void AddExercise_MaxExercises_ThrowsDomainException()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());

        for (int i = 0; i < 10; i++)
        {
            training.AddExercise(TrainingExercise.Create(
                training.Id,
                Guid.NewGuid(),
                i,
                90,
                10,
                3));
        }

        var act = () => training.AddExercise(TrainingExercise.Create(
            training.Id,
            Guid.NewGuid(),
            10,
            90,
            10,
            3));

        act.Should().Throw<DomainException>()
            .WithMessage("*10 exercises*");
    }

    [Fact]
    public void RemoveExercise_ExistingExercise_RemovesFromCollection()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());
        var exerciseId = Guid.NewGuid();

        training.AddExercise(TrainingExercise.Create(training.Id, exerciseId, 0, 90, 10, 3));

        training.RemoveExercise(exerciseId);

        training.Exercises.Should().BeEmpty();
    }

    [Fact]
    public void ClearExercises_RemovesAll()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());

        training.AddExercise(TrainingExercise.Create(training.Id, Guid.NewGuid(), 0, 90, 10, 3));
        training.AddExercise(TrainingExercise.Create(training.Id, Guid.NewGuid(), 1, 60, 12, 4));

        training.ClearExercises();

        training.Exercises.Should().BeEmpty();
    }

    [Fact]
    public void Restore_PreservesAllData()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var userId = Guid.NewGuid();

        var training = Training.Restore(
            Guid.NewGuid(),
            "Push Day",
            "Description",
            userId,
            createdAt,
            updatedAt);

        training.CreatedAt.Should().Be(createdAt);
        training.UpdatedAt.Should().Be(updatedAt);
        training.UserId.Should().Be(userId);
    }

    [Fact]
    public void RemoveExercise_NonExistentExercise_DoesNothing()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());
        training.AddExercise(TrainingExercise.Create(training.Id, Guid.NewGuid(), 0, 90, 10, 3));

        training.RemoveExercise(Guid.NewGuid());

        training.Exercises.Should().HaveCount(1);
    }

    [Fact]
    public void SetExercises_ReplacesAllExercises()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());
        training.AddExercise(TrainingExercise.Create(training.Id, Guid.NewGuid(), 0, 90, 10, 3));

        var newExercises = new List<TrainingExercise>
        {
            TrainingExercise.Create(training.Id, Guid.NewGuid(), 0, 60, 12, 4),
            TrainingExercise.Create(training.Id, Guid.NewGuid(), 1, 30, 15, 5)
        };

        training.SetExercises(newExercises);

        training.Exercises.Should().HaveCount(2);
    }

    [Fact]
    public void SetExercises_TooManyExercises_ThrowsDomainException()
    {
        var training = Training.Create("Push Day", null, Guid.NewGuid());
        var exercises = new List<TrainingExercise>();
        for (int i = 0; i < 11; i++)
        {
            exercises.Add(TrainingExercise.Create(training.Id, Guid.NewGuid(), i, 90, 10, 3));
        }

        var act = () => training.SetExercises(exercises);

        act.Should().Throw<DomainException>()
            .WithMessage("*10 exercises*");
    }

    [Fact]
    public void Update_WithNullDescription_SetsNull()
    {
        var training = Training.Create("Push Day", "Description", Guid.NewGuid());

        training.Update("Pull Day", null);

        training.Description.Should().BeNull();
    }
}

public class TrainingNameTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_EmptyOrWhitespace_ThrowsDomainException(string name)
    {
        var act = () => new TrainingName(name);

        act.Should().Throw<DomainException>()
            .WithMessage("*required*");
    }

    [Fact]
    public void Create_TooLong_ThrowsDomainException()
    {
        var longName = new string('A', 101);

        var act = () => new TrainingName(longName);

        act.Should().Throw<DomainException>()
            .WithMessage("*100*");
    }

    [Fact]
    public void Create_ValidName_ReturnsTrimmedName()
    {
        var name = new TrainingName("  Push Day  ");

        name.Value.Should().Be("Push Day");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesName()
    {
        TrainingName name = "Push Day";

        name.Value.Should().Be("Push Day");
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var name = new TrainingName("Push Day");
        string value = name;

        value.Should().Be("Push Day");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var name = new TrainingName("Push Day");

        name.ToString().Should().Be("Push Day");
    }
}

public class TrainingExerciseTests
{
    [Fact]
    public void Create_ValidExercise_ReturnsNewTrainingExercise()
    {
        var trainingId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();

        var trainingExercise = TrainingExercise.Create(
            trainingId,
            exerciseId,
            0,
            90,
            10,
            3);

        trainingExercise.TrainingId.Should().Be(trainingId);
        trainingExercise.ExerciseId.Should().Be(exerciseId);
        trainingExercise.OrderIndex.Should().Be(0);
        trainingExercise.RestTime.Should().Be(90);
        trainingExercise.Reps.Should().Be(10);
        trainingExercise.Series.Should().Be(3);
    }

    [Theory]
    [InlineData(-1, 10, 3)]
    [InlineData(121, 10, 3)]
    public void Create_InvalidRestTime_ThrowsDomainException(int restTime, int reps, int series)
    {
        var act = () => TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            restTime,
            reps,
            series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Rest time*");
    }

    [Theory]
    [InlineData(90, 0, 3)]
    [InlineData(90, 31, 3)]
    public void Create_InvalidReps_ThrowsDomainException(int restTime, int reps, int series)
    {
        var act = () => TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            restTime,
            reps,
            series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Reps*");
    }

    [Theory]
    [InlineData(90, 10, 0)]
    [InlineData(90, 10, 16)]
    public void Create_InvalidSeries_ThrowsDomainException(int restTime, int reps, int series)
    {
        var act = () => TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            restTime,
            reps,
            series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Series*");
    }

    [Fact]
    public void Update_UpdatesAllFields()
    {
        var trainingExercise = TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            90,
            10,
            3);

        trainingExercise.Update(60, 12, 4);

        trainingExercise.RestTime.Should().Be(60);
        trainingExercise.Reps.Should().Be(12);
        trainingExercise.Series.Should().Be(4);
    }

    [Fact]
    public void Restore_PreservesAllData()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var trainingId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();

        var trainingExercise = TrainingExercise.Restore(
            trainingId,
            exerciseId,
            2,
            120,
            15,
            5,
            createdAt);

        trainingExercise.TrainingId.Should().Be(trainingId);
        trainingExercise.ExerciseId.Should().Be(exerciseId);
        trainingExercise.OrderIndex.Should().Be(2);
        trainingExercise.RestTime.Should().Be(120);
        trainingExercise.Reps.Should().Be(15);
        trainingExercise.Series.Should().Be(5);
        trainingExercise.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(-1, 10, 3)]
    [InlineData(121, 10, 3)]
    public void Update_InvalidRestTime_ThrowsDomainException(int restTime, int reps, int series)
    {
        var trainingExercise = TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            90,
            10,
            3);

        var act = () => trainingExercise.Update(restTime, reps, series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Rest time*");
    }

    [Theory]
    [InlineData(90, 0, 3)]
    [InlineData(90, 31, 3)]
    public void Update_InvalidReps_ThrowsDomainException(int restTime, int reps, int series)
    {
        var trainingExercise = TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            90,
            10,
            3);

        var act = () => trainingExercise.Update(restTime, reps, series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Reps*");
    }

    [Theory]
    [InlineData(90, 10, 0)]
    [InlineData(90, 10, 16)]
    public void Update_InvalidSeries_ThrowsDomainException(int restTime, int reps, int series)
    {
        var trainingExercise = TrainingExercise.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            90,
            10,
            3);

        var act = () => trainingExercise.Update(restTime, reps, series);

        act.Should().Throw<DomainException>()
            .WithMessage("*Series*");
    }
}
