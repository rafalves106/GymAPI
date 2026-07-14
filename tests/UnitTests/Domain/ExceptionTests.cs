using FluentAssertions;
using GymAPI.Domain.Exceptions;

namespace GymAPI.UnitTests.Domain;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var exception = new DomainException("Test error");

        exception.Message.Should().Be("Test error");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        var inner = new InvalidOperationException("Inner");
        var exception = new DomainException("Test error", inner);

        exception.Message.Should().Be("Test error");
        exception.InnerException.Should().Be(inner);
    }
}

public class ExerciseNotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithId_FormatsMessage()
    {
        var id = Guid.NewGuid();

        var exception = new ExerciseNotFoundException(id);

        exception.Message.Should().Contain(id.ToString());
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var exception = new ExerciseNotFoundException("Not found");

        exception.Message.Should().Be("Not found");
    }
}

public class TrainingNotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithId_FormatsMessage()
    {
        var id = Guid.NewGuid();

        var exception = new TrainingNotFoundException(id);

        exception.Message.Should().Contain(id.ToString());
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var exception = new TrainingNotFoundException("Not found");

        exception.Message.Should().Be("Not found");
    }
}
