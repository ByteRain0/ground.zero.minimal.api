using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HabitTracker.Api.Habits.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

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
    public async Task GivenValidHabit_CreatesHabit()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit
        {
            Name = "First integration test"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var createdHabit = await response.Content.ReadFromJsonAsync<Habit>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdHabit.Should().NotBeNull();
        createdHabit!.Name.Should().Be(habit.Name);
        response.Headers.Location.AbsolutePath.Should().Be($"/api/v1/habits/{createdHabit.Id}");
        _habitIds.Add(createdHabit.Id);
    }

    [Fact]
    public async Task GivenInvalidHabit_ReturnsProblemDetails()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit();

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        problem.Should().NotBeNull();
        problem!.Errors.Should().NotBeEmpty();
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