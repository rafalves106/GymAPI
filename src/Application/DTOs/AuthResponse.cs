namespace GymAPI.Application.DTOs;

public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
