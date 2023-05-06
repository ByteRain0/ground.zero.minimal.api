using HabitTrackerAPI.Contracts.Models;
using HabitTrackerAPI.Service;
using O9d.AspNet.FluentValidation;

namespace HabitTrackerAPI.Habits;

internal static class HabitEndpoints
{
    internal static void MapHabitEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/habits")
            .WithTags("Habits")
            .WithOpenApi()
            .WithValidationFilter();

        group.MapPost("/", CreateHabitAsync)
            .Accepts<Habit>(Constants.ContentTypes.ApplicationJson)
            .Produces(201)
            .ProducesValidationProblem()
            .WithName("CreateHabit");
        
        group.MapGet("/{id:int}", GetHabitByIdAsync)
            .Produces(404)
            .Produces<Habit>()
            .WithName("GetHabit");
    }

    internal static async Task<IResult> CreateHabitAsync([Validate] Habit habit, IHabitService service,
        LinkGenerator linkGenerator, HttpContext context)
    {
        var habitId = await service.AddHabitAsync(habit);
        var path = linkGenerator.GetUriByName(context, "GetHabit", new { id = habitId })!;
        return Results.Created(path, habit);
    }

    internal static async Task<IResult> GetHabitByIdAsync(int id, IHabitService service,
        CancellationToken cancellationToken)
    {
        var habit = await service.GetHabitAsync(id, cancellationToken);

        if (habit is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(habit);
    }
}