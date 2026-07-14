using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace GymAPI.IntegrationTests;

public class TrainingsIntegrationTests : IntegrationTestBase
{
    public TrainingsIntegrationTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var token = await RegisterAndGetTokenAsync();

        var httpRequest = CreateAuthRequest(HttpMethod.Get, "/api/trainings");
        httpRequest.Headers.Add("Authorization", $"Bearer {token}");

        var response = await Client.SendAsync(httpRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/trainings");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithAuth_ReturnsCreated()
    {
        var token = await RegisterAndGetTokenAsync();

        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        exerciseRequest.Headers.Add("Authorization", $"Bearer {token}");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = "Bench Press",
            description = "Chest exercise with barbell",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exerciseResponse = await Client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var trainingRequest = CreateAuthRequest(HttpMethod.Post, "/api/trainings");
        trainingRequest.Headers.Add("Authorization", $"Bearer {token}");
        trainingRequest.Content = JsonContent.Create(new
        {
            name = "Push Day",
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

        var response = await Client.SendAsync(trainingRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("name").GetString().Should().Be("Push Day");
        content.GetProperty("exercises").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetById_ExistingTraining_ReturnsOk()
    {
        var token = await RegisterAndGetTokenAsync();

        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        exerciseRequest.Headers.Add("Authorization", $"Bearer {token}");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = "Squat",
            description = "Leg exercise",
            muscleGroups = new[] { 8 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exerciseResponse = await Client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, "/api/trainings");
        createRequest.Headers.Add("Authorization", $"Bearer {token}");
        createRequest.Content = JsonContent.Create(new
        {
            name = "Leg Day",
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
        var createResponse = await Client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var getResponse = await Client.SendAsync(CreateAuthRequest(HttpMethod.Get, $"/api/trainings/{id}", token));

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNotFound()
    {
        var token = await RegisterAndGetTokenAsync();

        var response = await Client.SendAsync(CreateAuthRequest(HttpMethod.Get, $"/api/trainings/{Guid.NewGuid()}", token));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingTraining_ReturnsOk()
    {
        var token = await RegisterAndGetTokenAsync();

        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        exerciseRequest.Headers.Add("Authorization", $"Bearer {token}");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = "Deadlift",
            description = "Back exercise",
            muscleGroups = new[] { 2 },
            equipments = new[] { 1 },
            difficultyLevel = 3
        });
        var exerciseResponse = await Client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, "/api/trainings");
        createRequest.Headers.Add("Authorization", $"Bearer {token}");
        createRequest.Content = JsonContent.Create(new
        {
            name = "Back Day",
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
        var createResponse = await Client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var updateRequest = CreateAuthRequest(HttpMethod.Put, $"/api/trainings/{id}");
        updateRequest.Headers.Add("Authorization", $"Bearer {token}");
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

        var response = await Client.SendAsync(updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("name").GetString().Should().Be("Pull Day");
    }

    [Fact]
    public async Task Delete_ExistingTraining_ReturnsNoContent()
    {
        var token = await RegisterAndGetTokenAsync();

        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        exerciseRequest.Headers.Add("Authorization", $"Bearer {token}");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = "Plank",
            description = "Core exercise",
            muscleGroups = new[] { 6 },
            equipments = new[] { 10 },
            difficultyLevel = 1
        });
        var exerciseResponse = await Client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, "/api/trainings");
        createRequest.Headers.Add("Authorization", $"Bearer {token}");
        createRequest.Content = JsonContent.Create(new
        {
            name = "Core Day",
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
        var createResponse = await Client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var deleteRequest = CreateAuthRequest(HttpMethod.Delete, $"/api/trainings/{id}");
        deleteRequest.Headers.Add("Authorization", $"Bearer {token}");

        var deleteResponse = await Client.SendAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.SendAsync(CreateAuthRequest(HttpMethod.Get, $"/api/trainings/{id}", token));
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DifferentUsers_CannotSeeEachOthersTrainings()
    {
        var token1 = await RegisterAndGetTokenAsync();
        var token2 = await RegisterAndGetTokenAsync();

        var exerciseRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        exerciseRequest.Headers.Add("Authorization", $"Bearer {token1}");
        exerciseRequest.Content = JsonContent.Create(new
        {
            name = "Bench Press",
            description = "Chest exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var exerciseResponse = await Client.SendAsync(exerciseRequest);
        var exercise = await exerciseResponse.Content.ReadFromJsonAsync<JsonElement>();
        var exerciseId = exercise.GetProperty("id").GetString();

        var createRequest = CreateAuthRequest(HttpMethod.Post, "/api/trainings");
        createRequest.Headers.Add("Authorization", $"Bearer {token1}");
        createRequest.Content = JsonContent.Create(new
        {
            name = "User 1 Training",
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
        var createResponse = await Client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var getRequest = CreateAuthRequest(HttpMethod.Get, $"/api/trainings/{id}");
        getRequest.Headers.Add("Authorization", $"Bearer {token2}");

        var getResponse = await Client.SendAsync(getRequest);

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
