using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.ValueObjects;

public record ExerciseName
{
    public string Value { get; }
    private const int MaxLength = 100;
    private const int MinLength = 2;

    public ExerciseName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Exercise name is required.");

        if (value.Length < MinLength)
            throw new DomainException($"Exercise name must be at least {MinLength} characters.");

        if (value.Length > MaxLength)
            throw new DomainException($"Exercise name must not exceed {MaxLength} characters.");

        Value = value.Trim();
    }

    public static implicit operator string(ExerciseName name) => name.Value;
    public static implicit operator ExerciseName(string value) => new(value);

    public override string ToString() => Value;
}
