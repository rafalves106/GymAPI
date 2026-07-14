namespace GymAPI.Application.DTOs;

public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
}

public record RegisterRequest
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginRequest
{
    public string UsernameOrEmail { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
