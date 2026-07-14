using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GymAPI.Tests;

public class TrainingsTests : IAsyncLifetime
{
    private static readonly string BaseUrl = "http://localhost:5200";
    private readonly HttpClient _client = new();
    private string _token = string.Empty;
    private string _testEmail = string.Empty;

    public async Task InitializeAsync()
    {
        _testEmail = $"trainings_{Guid.NewGuid():N}@test.com";
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
        var httpRequest = CreateAuthRequest(HttpMethod.Get, $"{BaseUrl}/api/trainings");

        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, content.ValueKind);
    }

    [Fact]
    public async Task Create_ValidTraining_ReturnsCreated()
    {
        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = $"Bench Press {Guid.NewGuid():N}",
            description = "Chest exercise with barbell",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exerciseResponse = await _client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var trainingRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/trainings");
        trainingRequest.Content = JsonContent.Create(new
        {
            name = $"Push Day {Guid.NewGuid():N}",
            description = "Chest and shoulders workout",
            exercises = new[]
            {
                new
                {
                    exerciseId,
                    restTime = 90,
                    reps = 10,
                    series = 3
                }
            }
        });

        var response = await _client.SendAsync(trainingRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(content.TryGetProperty("id", out _));
        Assert.Equal(1, content.GetProperty("exercises").GetArrayLength());
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/trainings", new
        {
            name = "Push Day",
            exercises = new[]
            {
                new
                {
                    exerciseId = Guid.NewGuid(),
                    restTime = 90,
                    reps = 10,
                    series = 3
                }
            }
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingTraining_ReturnsOk()
    {
        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = $"Squat {Guid.NewGuid():N}",
            description = "Leg exercise",
            muscleGroups = new[] { 8 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exerciseResponse = await _client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/trainings");
        createRequest.Content = JsonContent.Create(new
        {
            name = $"Leg Day {Guid.NewGuid():N}",
            exercises = new[]
            {
                new
                {
                    exerciseId,
                    restTime = 120,
                    reps = 8,
                    series = 5
                }
            }
        });
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var getRequest = CreateAuthRequest(HttpMethod.Get, $"{BaseUrl}/api/trainings/{id}");
        var response = await _client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(id, content.GetProperty("id").GetString());
    }

    [Fact]
    public async Task GetById_NonExistentTraining_ReturnsNotFound()
    {
        var httpRequest = CreateAuthRequest(HttpMethod.Get, $"{BaseUrl}/api/trainings/{Guid.NewGuid()}");

        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingTraining_ReturnsOk()
    {
        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = $"Deadlift {Guid.NewGuid():N}",
            description = "Back exercise",
            muscleGroups = new[] { 2 },
            equipments = new[] { 1 },
            difficultyLevel = 3
        });
        var exerciseResponse = await _client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/trainings");
        createRequest.Content = JsonContent.Create(new
        {
            name = $"Back Day {Guid.NewGuid():N}",
            exercises = new[]
            {
                new
                {
                    exerciseId,
                    restTime = 90,
                    reps = 10,
                    series = 3
                }
            }
        });
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var updateRequest = CreateAuthRequest(HttpMethod.Put, $"{BaseUrl}/api/trainings/{id}");
        updateRequest.Content = JsonContent.Create(new
        {
            name = "Pull Day",
            description = "Back and biceps",
            exercises = new[]
            {
                new
                {
                    exerciseId,
                    restTime = 60,
                    reps = 12,
                    series = 4
                }
            }
        });

        var response = await _client.SendAsync(updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Pull Day", content.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Delete_ExistingTraining_ReturnsNoContent()
    {
        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = $"Plank {Guid.NewGuid():N}",
            description = "Core exercise",
            muscleGroups = new[] { 6 },
            equipments = new[] { 10 },
            difficultyLevel = 1
        });
        var exerciseResponse = await _client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/trainings");
        createRequest.Content = JsonContent.Create(new
        {
            name = $"Core Day {Guid.NewGuid():N}",
            exercises = new[]
            {
                new
                {
                    exerciseId,
                    restTime = 30,
                    reps = 15,
                    series = 3
                }
            }
        });
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var deleteRequest = CreateAuthRequest(HttpMethod.Delete, $"{BaseUrl}/api/trainings/{id}");
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getRequest = CreateAuthRequest(HttpMethod.Get, $"{BaseUrl}/api/trainings/{id}");
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Create_MultipleExercises_ReturnsAll()
    {
        var exercise1Request = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exercise1Request.Content = JsonContent.Create(new
        {
            name = $"Bench Press {Guid.NewGuid():N}",
            description = "Chest exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exercise1Response = await _client.SendAsync(exercise1Request);
        var exercise1 = await exercise1Response.Content.ReadFromJsonAsync<JsonElement>();
        var exercise1Id = exercise1.GetProperty("id").GetString();

        var exercise2Request = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/exercises");
        exercise2Request.Content = JsonContent.Create(new
        {
            name = $"Incline Press {Guid.NewGuid():N}",
            description = "Upper chest exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exercise2Response = await _client.SendAsync(exercise2Request);
        var exercise2 = await exercise2Response.Content.ReadFromJsonAsync<JsonElement>();
        var exercise2Id = exercise2.GetProperty("id").GetString();

        var trainingRequest = CreateAuthRequest(HttpMethod.Post, $"{BaseUrl}/api/trainings");
        trainingRequest.Content = JsonContent.Create(new
        {
            name = $"Push Day {Guid.NewGuid():N}",
            exercises = new[]
            {
                new { exerciseId = exercise1Id, restTime = 90, reps = 10, series = 3 },
                new { exerciseId = exercise2Id, restTime = 60, reps = 12, series = 4 }
            }
        });

        var response = await _client.SendAsync(trainingRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, content.GetProperty("exercises").GetArrayLength());
    }
}
