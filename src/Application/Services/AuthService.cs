using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;

namespace GymAPI.Application.Services;

public class AuthService : IAuthUseCases
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, new[] { "User" });

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        user.UpdateLastLogin();
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, new[] { "User" });

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}
