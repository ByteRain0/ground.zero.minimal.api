using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Models;

namespace HabitTrackerAPI.Habits.Endpoints;

/// <summary>
/// Alternative way you cloud structure your Minimal API based around use cases..
/// </summary>
internal class GetAllHabitsEndpoint
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/habits/", GetAllHabitsAsync)
            .Produces<List<Habit>>(200)
            .Produces(400)
            .WithTags("Habits")
            .WithName("GetAllHabits");
    }

    private static async Task<IResult> GetAllHabitsAsync(IHabitService service, CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetHabitsAsync(cancellationToken));
    }
}