using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace GymAPI.IntegrationTests;

public class AuthIntegrationTests : IntegrationTestBase
{
    public AuthIntegrationTests(TestWebApplicationFactory factory) : base(factory) { }

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

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("email").GetString().Should().Be(request.email);
        content.GetProperty("fullName").GetString().Should().Be("John Doe");
        content.TryGetProperty("token", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var email = $"dup_{Guid.NewGuid():N}@test.com";
        var request = new { email, password = "Password123!", firstName = "John", lastName = "Doe" };

        var firstResponse = await Client.PostAsJsonAsync("/api/auth/register", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await Client.PostAsJsonAsync("/api/auth/register", request);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email, password = "Password123!", firstName = "John", lastName = "Doe"
        });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email, password = "Password123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        content.GetProperty("email").GetString().Should().Be(email);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var email = $"login_{Guid.NewGuid():N}@test.com";
        await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email, password = "Password123!", firstName = "John", lastName = "Doe"
        });

        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email, password = "WrongPassword!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = $"nonexistent_{Guid.NewGuid():N}@test.com",
            password = "Password123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
