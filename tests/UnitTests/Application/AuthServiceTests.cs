using FluentAssertions;
using GymAPI.Application.DTOs;
using GymAPI.Application.Services;
using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using Moq;

namespace GymAPI.UnitTests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _sut = new AuthService(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsAuthResponse()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _jwtTokenServiceMock.Setup(j => j.GenerateToken(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns("jwt-token");

        var result = await _sut.RegisterAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Email.Should().Be("test@test.com");
        result.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var existingUser = User.Create("test@test.com", "hash", "John", "Doe");
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        var act = () => _sut.RegisterAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var password = "Password123!";
        var user = User.Create("test@test.com", BCrypt.Net.BCrypt.HashPassword(password), "John", "Doe");

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _jwtTokenServiceMock.Setup(j => j.GenerateToken(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns("jwt-token");

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = password
        };

        var result = await _sut.LoginAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = User.Create("test@test.com", BCrypt.Net.BCrypt.HashPassword("Password123!"), "John", "Doe");

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "WrongPassword!"
        };

        var act = () => _sut.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task LoginAsync_NonExistentUser_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "Password123!"
        };

        var act = () => _sut.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid email or password*");
    }
}
