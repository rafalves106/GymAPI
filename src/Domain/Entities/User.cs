using GymAPI.Domain.Exceptions;

namespace GymAPI.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsMaster { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    private User(Guid id, string username, string email, string passwordHash, bool isMaster)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsMaster = isMaster;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string username, string email, string passwordHash, bool isMaster = false)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username is required.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("A valid email is required.");

        return new User(Guid.NewGuid(), username.Trim(), email.ToLower().Trim(), passwordHash, isMaster);
    }

    public static User Restore(Guid id, string username, string email, string passwordHash, bool isMaster, DateTime createdAt, DateTime? lastLoginAt)
    {
        var user = new User(id, username, email, passwordHash, isMaster);
        user.CreatedAt = createdAt;
        user.LastLoginAt = lastLoginAt;
        return user;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
    }
}
