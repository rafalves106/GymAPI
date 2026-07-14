namespace GymAPI.Domain.Exceptions;

public class SessionNotFoundException : Exception
{
    public SessionNotFoundException(Guid id)
        : base($"Session with ID {id} was not found.") { }

    public SessionNotFoundException(string message)
        : base(message) { }
}
