using System.Net;
using System.Net.Http.Json;

namespace HabitTracker.Api.Tests.Integration;

public class CreateHabitEndpointTests :
    IClassFixture<WebApplicationFactory<IApiMarker>>,
    IAsyncLifetime
{
    private readonly WebApplicationFactory<IApiMarker> _webApplicationFactory;

    private List<int> _habitIds;

    public CreateHabitEndpointTests(WebApplicationFactory<IApiMarker> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _habitIds = new List<int>();
    }

    [Fact]
    public async Task GivenValidPayload_CreatesHabit()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit
        {
            Name = "First Integration Test"
        };

        // Act
        var result = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var createdHabit = await result.Content.ReadFromJsonAsync<Habit>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdHabit.Should().NotBeNull();
        createdHabit!.Name.Should().Be(habit.Name);
        result.Headers.Location!.AbsolutePath.Should().Be($"/api/v1/habits/{createdHabit.Id}");
        _habitIds.Add(createdHabit.Id);
    }

    [Fact]
    public async Task GivenInvalidPayload_ReturnsProblemDetails()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit();

        // Act
        var result = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().NotBeNullOrEmpty();
        problem.Errors.Any(x =>
                x.Key == "Name"
                && x.Value.Any(y => y == "'Name' must not be empty."))
            .Should()
            .BeTrue();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = _webApplicationFactory.CreateClient();

        foreach (var habitId in _habitIds)
        {
            await httpClient.DeleteAsync($"api/v1/habits/{habitId}");
        }
    }
}