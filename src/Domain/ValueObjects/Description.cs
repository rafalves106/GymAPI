using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.ValueObjects;

public record Description
{
    public string Value { get; }
    private const int MaxLength = 500;

    public Description(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Description is required.");

        if (value.Length > MaxLength)
            throw new DomainException($"Description must not exceed {MaxLength} characters.");

        Value = value.Trim();
    }

    public static implicit operator string(Description description) => description.Value;
    public static implicit operator Description(string value) => new(value);

    public override string ToString() => Value;
}
