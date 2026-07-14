using FluentAssertions;
using GymAPI.Domain.Exceptions;
using GymAPI.Domain.ValueObjects;

namespace GymAPI.UnitTests.Domain;

public class ExerciseNameTests
{
    [Fact]
    public void Create_ValidName_ReturnsName()
    {
        var name = new ExerciseName("Bench Press");
        name.Value.Should().Be("Bench Press");
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var name = new ExerciseName("  Bench Press  ");
        name.Value.Should().Be("Bench Press");
    }

    [Fact]
    public void Create_EmptyString_ThrowsDomainException()
    {
        var act = () => new ExerciseName("");
        act.Should().Throw<DomainException>()
            .WithMessage("*required*");
    }

    [Fact]
    public void Create_NullString_ThrowsDomainException()
    {
        var act = () => new ExerciseName(null!);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WhitespaceOnly_ThrowsDomainException()
    {
        var act = () => new ExerciseName("   ");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_TooShort_ThrowsDomainException()
    {
        var act = () => new ExerciseName("A");
        act.Should().Throw<DomainException>()
            .WithMessage("*2 characters*");
    }

    [Fact]
    public void Create_TooLong_ThrowsDomainException()
    {
        var act = () => new ExerciseName(new string('A', 101));
        act.Should().Throw<DomainException>()
            .WithMessage("*100 characters*");
    }

    [Fact]
    public void Create_MaxLength_Succeeds()
    {
        var name = new ExerciseName(new string('A', 100));
        name.Value.Should().HaveLength(100);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var name = new ExerciseName("Squat");
        string value = name;
        value.Should().Be("Squat");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesName()
    {
        ExerciseName name = "Deadlift";
        name.Value.Should().Be("Deadlift");
    }
}

public class DescriptionTests
{
    [Fact]
    public void Create_ValidDescription_ReturnsDescription()
    {
        var description = new Description("A great chest exercise");
        description.Value.Should().Be("A great chest exercise");
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var description = new Description("  A great exercise  ");
        description.Value.Should().Be("A great exercise");
    }

    [Fact]
    public void Create_EmptyString_ThrowsDomainException()
    {
        var act = () => new Description("");
        act.Should().Throw<DomainException>()
            .WithMessage("*required*");
    }

    [Fact]
    public void Create_NullString_ThrowsDomainException()
    {
        var act = () => new Description(null!);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_TooLong_ThrowsDomainException()
    {
        var act = () => new Description(new string('A', 501));
        act.Should().Throw<DomainException>()
            .WithMessage("*500 characters*");
    }

    [Fact]
    public void Create_MaxLength_Succeeds()
    {
        var description = new Description(new string('A', 500));
        description.Value.Should().HaveLength(500);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var description = new Description("Test description");
        string value = description;
        value.Should().Be("Test description");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesDescription()
    {
        Description description = "New description";
        description.Value.Should().Be("New description");
    }
}
