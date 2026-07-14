using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.ValueObjects;

public record TrainingName
{
    public string Value { get; }
    private const int MinLength = 1;
    private const int MaxLength = 100;

    public TrainingName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Training name is required.");

        var trimmed = value.Trim();
        if (trimmed.Length < MinLength)
            throw new DomainException($"Training name must be at least {MinLength} character.");
        if (trimmed.Length > MaxLength)
            throw new DomainException($"Training name must not exceed {MaxLength} characters.");

        Value = trimmed;
    }

    public static implicit operator string(TrainingName name) => name.Value;
    public static implicit operator TrainingName(string value) => new(value);

    public override string ToString() => Value;
}
