namespace GymAPI.Domain.Exceptions;

public class ExerciseNotFoundException : Exception
{
    public ExerciseNotFoundException(Guid id)
        : base($"Exercise with ID {id} was not found.") { }

    public ExerciseNotFoundException(string message)
        : base(message) { }
}
