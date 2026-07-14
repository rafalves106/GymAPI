using GymAPI.Application.DTOs;

namespace GymAPI.Application.Interfaces;

public interface IAuthUseCases
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
