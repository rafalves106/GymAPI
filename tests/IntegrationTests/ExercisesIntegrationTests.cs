using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace GymAPI.IntegrationTests;

public class ExercisesIntegrationTests : IntegrationTestBase
{
    public ExercisesIntegrationTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/exercises");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        var request = new
        {
            name = "Bench Press",
            description = "Chest exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var response = await Client.PostAsJsonAsync("/api/exercises", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithAuth_ReturnsCreated()
    {
        var token = await RegisterAndGetTokenAsync();
        var request = new
        {
            name = "Bench Press",
            description = "Chest exercise with barbell",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        };

        var httpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        httpRequest.Headers.Add("Authorization", $"Bearer {token}");
        httpRequest.Content = JsonContent.Create(request);

        var response = await Client.SendAsync(httpRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("name").GetString().Should().Be("Bench Press");
    }

    [Fact]
    public async Task GetById_ExistingExercise_ReturnsOk()
    {
        var token = await RegisterAndGetTokenAsync();

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        createHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        createHttpRequest.Content = JsonContent.Create(new
        {
            name = "Squat",
            description = "Leg exercise",
            muscleGroups = new[] { 8 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });
        var createResponse = await Client.SendAsync(createHttpRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var getResponse = await Client.GetAsync($"/api/exercises/{id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/exercises/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingExercise_ReturnsOk()
    {
        var token = await RegisterAndGetTokenAsync();

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        createHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        createHttpRequest.Content = JsonContent.Create(new
        {
            name = "Deadlift",
            description = "Back exercise",
            muscleGroups = new[] { 2 },
            equipments = new[] { 1 },
            difficultyLevel = 3
        });
        var createResponse = await Client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var updateHttpRequest = CreateAuthRequest(HttpMethod.Put, $"/api/exercises/{id}");
        updateHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        updateHttpRequest.Content = JsonContent.Create(new
        {
            name = "Romanian Deadlift",
            description = "Hamstring exercise",
            muscleGroups = new[] { 2, 9 },
            equipments = new[] { 1 },
            difficultyLevel = 2
        });

        var response = await Client.SendAsync(updateHttpRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("name").GetString().Should().Be("Romanian Deadlift");
    }

    [Fact]
    public async Task Delete_ExistingExercise_ReturnsNoContent()
    {
        var token = await RegisterAndGetTokenAsync();

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        createHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        createHttpRequest.Content = JsonContent.Create(new
        {
            name = "Lateral Raise",
            description = "Shoulder exercise",
            muscleGroups = new[] { 3 },
            equipments = new[] { 2 },
            difficultyLevel = 1
        });
        var createResponse = await Client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var deleteHttpRequest = CreateAuthRequest(HttpMethod.Delete, $"/api/exercises/{id}");
        deleteHttpRequest.Headers.Add("Authorization", $"Bearer {token}");

        var deleteResponse = await Client.SendAsync(deleteHttpRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/exercises/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByMuscleGroup_ReturnsFilteredResults()
    {
        var token = await RegisterAndGetTokenAsync();

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        createHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        createHttpRequest.Content = JsonContent.Create(new
        {
            name = $"Push Up {Guid.NewGuid():N}",
            description = "Chest exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 10 },
            difficultyLevel = 1
        });
        await Client.SendAsync(createHttpRequest);

        var response = await Client.GetAsync("/api/exercises/muscle-group/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchByName_ReturnsMatchingResults()
    {
        var token = await RegisterAndGetTokenAsync();
        var uniqueName = $"UniqueSearch_{Guid.NewGuid():N}";

        var createHttpRequest = CreateAuthRequest(HttpMethod.Post, "/api/exercises");
        createHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
        createHttpRequest.Content = JsonContent.Create(new
        {
            name = uniqueName,
            description = "Test exercise",
            muscleGroups = new[] { 1 },
            equipments = new[] { 1 },
            difficultyLevel = 1
        });
        await Client.SendAsync(createHttpRequest);

        var searchTerm = uniqueName.Split('_')[0];
        var response = await Client.GetAsync($"/api/exercises/search?name={searchTerm}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.ValueKind.Should().Be(JsonValueKind.Array);
    }
}
