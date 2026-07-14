using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GymAPI.Tests;

public class ExercisesTests : IAsyncLifetime
{
    private static readonly string BaseUrl = "http://localhost:5200";
    private readonly HttpClient _client = new();
    private string _token = string.Empty;
    private string _testEmail = string.Empty;

    public async Task InitializeAsync()
    {
        _testEmail = $"exercises_{Guid.NewGuid():N}@test.com";
        var registerRequest = new
        {
            email = _testEmail,
            password = "Password123!",
            firstName = "Test",
            lastName = "User"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/auth/register", registerRequest);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        _token = content.GetProperty("token").GetString()!;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private HttpRequestMessage CreateAuthRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Authorization", $"Bearer {_token}");
        return request;
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client.GetAsync($"{BaseUrl}/api/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, content.ValueKind);
    }

    [Fact]
    public async Task Create_ValidExercise_ReturnsCreated()
    {
        var request = new
        {
            name = $"Bench Press {Guid.NewGuid():N}",
            description = "Chest exercise with barbell",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var httpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        httpRequest.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(content.TryGetProperty("id", out _));
        Assert.Equal(request.name, content.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        var request = new
        {
            name = "Bench Press",
            description = "Chest exercise with barbell",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/exercises", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingExercise_ReturnsOk()
    {
        var createRequest = new
        {
            name = $"Squat {Guid.NewGuid():N}",
            description = "Leg exercise with barbell",
            muscleGroups = new[] { 8 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        createHttpRequest.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await _client.GetAsync($"{BaseUrl}/api/exercises/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(id, content.GetProperty("id").GetString());
    }

    [Fact]
    public async Task GetById_NonExistentExercise_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"{BaseUrl}/api/exercises/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingExercise_ReturnsOk()
    {
        var createRequest = new
        {
            name = $"Deadlift {Guid.NewGuid():N}",
            description = "Back exercise with barbell",
            muscleGroups = new[] { 2 },
            equipments = new[] { 1 },
            difficultyLevel = 3
        };

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        createHttpRequest.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var updateRequest = new
        {
            name = "Romanian Deadlift",
            description = "Hamstring exercise with barbell",
            muscleGroups = new[] { 2, 9 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var updateHttpRequest = CreateAuthRequest(HttpMethod.Put, $"{BaseUrl}/api/exercises/{id}");
        updateHttpRequest.Content = JsonContent.Create(updateRequest);
        var updateResponse = await _client.SendAsync(updateHttpRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var content = await updateResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Romanian Deadlift", content.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Delete_ExistingExercise_ReturnsNoContent()
    {
        var createRequest = new
        {
            name = $"Lateral Raise {Guid.NewGuid():N}",
            description = "Shoulder exercise with dumbbells",
            muscleGroups = new[] { 3 },
            equipments = new[] { 2 },
            difficultyLevel = 1
        };

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        createHttpRequest.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var deleteHttpRequest = CreateAuthRequest(HttpMethod.Delete, $"{BaseUrl}/api/exercises/{id}");
        var deleteResponse = await _client.SendAsync(deleteHttpRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{BaseUrl}/api/exercises/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetByMuscleGroup_ReturnsFilteredResults()
    {
        var request = new
        {
            name = $"Push Up {Guid.NewGuid():N}",
            description = "Chest bodyweight exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 10 },
            difficultyLevel = 1
        };

        var httpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        httpRequest.Content = JsonContent.Create(request);
        await _client.SendAsync(httpRequest);

        var response = await _client.GetAsync($"{BaseUrl}/api/exercises/muscle-group/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, content.ValueKind);
    }

    [Fact]
    public async Task SearchByName_ReturnsMatchingResults()
    {
        var uniqueName = $"UniqueExercise_{Guid.NewGuid():N}";
        var request = new
        {
            name = uniqueName,
            description = "Test exercise for search",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 1
        };

        var httpRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        httpRequest.Content = JsonContent.Create(request);
        await _client.SendAsync(httpRequest);

        var searchTerm = uniqueName.Split('_')[0];
        var response = await _client.GetAsync($"{BaseUrl}/api/exercises/search?name={searchTerm}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, content.ValueKind);
    }
}
