using System.Net;
using System.Net.Http.Json;
using AutoBogus;
using FluentAssertions;
using HabitTracker.Api.Habits.Contracts.Models;
using HabitTracker.Api.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Api.Tests.Integration.Tests;

public class CreateHabitEndpointTests : 
    IClassFixture<ApiFactory>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;

    private Func<Task> _resetDatabase;

    public CreateHabitEndpointTests(ApiFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.HttpClient;
        _resetDatabase = webApplicationFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task GivenValidHabit_CreatesHabit()
    {
        // Arrange

        var actions = new[] {"Annoy", "Bug", "Disturb"};
        var habitDataGenerator = new AutoFaker<Habit>()
            .RuleFor(x => x.Name, faker => $"{faker.PickRandom(actions)} {faker.Name.FullName()} with some repetitive requests.");

        var habit = habitDataGenerator.Generate();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var createdHabit = await response.Content.ReadFromJsonAsync<Habit>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdHabit.Should().NotBeNull();
        
        // In case you want to validate multiple fields at the same time you might want to match against a snapshot
        // Snapshot.Match(createdHabit, options => options.IgnoreField("Id"));
        
        createdHabit!.Name.Should().Be(habit.Name);
        response.Headers.Location.AbsolutePath.Should().Be($"/api/v1/habits/{createdHabit.Id}");
    }

    [Fact]
    public async Task GivenInvalidHabit_ReturnsProblemDetails()
    {
        // Arrange
        var habit = new Habit();

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/v1/habits", habit);
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

    public Task DisposeAsync() => _resetDatabase();
}