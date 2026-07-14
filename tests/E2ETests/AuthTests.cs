using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Playwright;
using Xunit;

namespace GymAPI.Tests;

public class AuthTests : IAsyncLifetime
{
    private static readonly string BaseUrl = "http://localhost:5200";
    private readonly HttpClient _client = new();

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_ValidUser_ReturnsCreated()
    {
        var request = new
        {
            email = $"test_{Guid.NewGuid():N}@test.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(content.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrEmpty(token.GetString()));
        Assert.Equal(request.email, content.GetProperty("email").GetString());
        Assert.Contains("John", content.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var email = $"duplicate_{Guid.NewGuid():N}@test.com";
        var request = new
        {
            email,
            password = "Password123!",
            firstName = "John",
            lastName = "Doe"
        };

        await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", request);
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("already exists", content.GetProperty("error").GetProperty("message").GetString());
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        var request = new
        {
            email = "not-an-email",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsBadRequest()
    {
        var request = new
        {
            email = $"test_{Guid.NewGuid():N}@test.com",
            password = "123",
            firstName = "John",
            lastName = "Doe"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var registerRequest = new
        {
            email,
            password = "Password123!",
            firstName = "John",
            lastName = "Doe"
        };
        await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", registerRequest);

        var loginRequest = new { email, password = "Password123!" };
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(content.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrEmpty(token.GetString()));
        Assert.Equal(email, content.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var registerRequest = new
        {
            email,
            password = "Password123!",
            firstName = "John",
            lastName = "Doe"
        };
        await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", registerRequest);

        var loginRequest = new { email, password = "WrongPassword!" };
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        var loginRequest = new { email = "nonexistent@test.com", password = "Password123!" };
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
