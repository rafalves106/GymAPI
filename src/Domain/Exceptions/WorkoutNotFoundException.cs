namespace GymAPI.Domain.Exceptions;

public class WorkoutNotFoundException : Exception
{
    public WorkoutNotFoundException(Guid id)
        : base($"Workout with ID {id} was not found.") { }

    public WorkoutNotFoundException(string message)
        : base(message) { }
}
