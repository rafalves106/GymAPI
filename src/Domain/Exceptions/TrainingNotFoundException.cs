namespace GymAPI.Domain.Exceptions;

public class TrainingNotFoundException : Exception
{
    public TrainingNotFoundException(Guid id)
        : base($"Training with ID {id} was not found.") { }

    public TrainingNotFoundException(string message)
        : base(message) { }
}
