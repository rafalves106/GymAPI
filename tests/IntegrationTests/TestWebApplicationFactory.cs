using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAPI.Api;
using GymAPI.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymAPI.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var toRemove = services.Where(s =>
                s.ServiceType == typeof(DbContextOptions<GymDbContext>) ||
                s.ServiceType == typeof(GymDbContext) ||
                (s.ServiceType != null && s.ServiceType.FullName != null &&
                 s.ServiceType.FullName.Contains("DbContext")) ||
                (s.ImplementationType != null && s.ImplementationType.FullName != null &&
                 (s.ImplementationType.FullName.Contains("Npgsql") ||
                  s.ImplementationType.FullName.Contains("EntityFrameworkCore"))))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<GymDbContext>(options =>
                options.UseInMemoryDatabase(DatabaseName));
        });
    }
}

public class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly TestWebApplicationFactory Factory;

    public IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<string> RegisterAndGetTokenAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"test_{Guid.NewGuid():N}@test.com",
            password = "Password123!",
            firstName = "Test",
            lastName = "User"
        });

        var content = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        return content.GetProperty("token").GetString()!;
    }

    protected async Task<string> RegisterUserAsync(string email, string password = "Password123!")
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password,
            firstName = "Test",
            lastName = "User"
        });

        var content = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        return content.GetProperty("token").GetString()!;
    }

    protected HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string? token = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (token is not null)
            request.Headers.Add("Authorization", $"Bearer {token}");
        return request;
    }
}
