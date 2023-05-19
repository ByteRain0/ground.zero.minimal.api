using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Models;
using HabitTrackerAPI.Infrastructure;
using HabitTrackerAPI.Infrastructure.Endpoints;
using O9d.AspNet.FluentValidation;

namespace HabitTrackerAPI.Habits.Endpoints;

internal class HabitEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
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
        
        group.MapGet("/", GetAllHabitsAsync)
            .Produces<List<Habit>>(200)
            .Produces(400)
            .WithName("GetAllHabits");

        group.MapPut("/{id:int}", UpdateHabitAsync)
            .Accepts<Habit>(Constants.ContentTypes.ApplicationJson)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(404)
            .WithName("UpdateHabit");

        group.MapDelete("/{id:int}", RemoveHabitAsync)
            .Produces(404)
            .Produces(204)
            .WithName("RemoveHabit");

        group.MapGet("/{id:int}/calendar", GetMonthlyCompletionStatus)
            .Produces(404)
            .Produces<List<DayInformation>>(200)
            .WithName("MonthlyCompletionStatus");

        group.MapGet("/{id:int}/currentStreak", GetCurrentStreak)
            .Produces(404)
            .Produces<int>(200)
            .WithName("CurrentHabitStreak");
        
        group.MapGet("/{id:int}/longestStreak", GetLongestStreak)
            .Produces(404)
            .Produces<int>(200)
            .WithName("CurrentHabitLongestStreak");

        group.MapPut("/{id:int}/status/{date}", UpdateCompletionStatus)
            .Produces(404)
            .Produces(200)
            .WithName("UpdateCompletionStatus");
    }
    
    private static async Task<IResult> CreateHabitAsync(
        [Validate] Habit habit, 
        IHabitService service,
        LinkGenerator linkGenerator, 
        HttpContext context)
    {
        var habitId = await service.AddHabitAsync(habit);
        var path = linkGenerator.GetUriByName(context, "GetHabit", new { id = habitId })!;
        return Results.Created(path, habit);
    }

    private static async Task<IResult> GetHabitByIdAsync(
        int id, 
        IHabitService service,
        CancellationToken cancellationToken)
    {
        var habit = await service.GetHabitAsync(id, cancellationToken);

        if (habit is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(habit);
    }

    private static async Task<IResult> GetAllHabitsAsync(
        IHabitService service, 
        CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetHabitsAsync(cancellationToken));
    }

    private static async Task<IResult> UpdateHabitAsync(
        int id, 
        [Validate] Habit model, 
        IHabitService service)
    {
        model.Id = id;
        
        var habit = await service.UpdateHabitAsync(model);

        if (habit == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(habit);
    }

    private static async Task<IResult> RemoveHabitAsync(
        int id, 
        IHabitService service)
    {   
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }

        await service.RemoveHabitAsync(id);

        return Results.Ok();
    }

    private static async Task<IResult> GetMonthlyCompletionStatus(
        int id,
        IHabitService service,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    { 
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(
            service.GetMonthlyCompletionStatus(
                id, DateOnly.FromDateTime(dateTimeProvider.CurrentTime()), cancellationToken));
    }

    private static async Task<IResult> GetCurrentStreak(
        int id,
        IHabitService service,
        CancellationToken cancellationToken)
    {
        // Might want to move this to a separate private method to deduplicate code.
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(service.GetHabitCurrentStreakAsync(id, cancellationToken));
    }
    
    private static async Task<IResult> GetLongestStreak(
        int id, 
        IHabitService service,
        CancellationToken cancellationToken)
    {
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(service.GetHabitLongestStreakAsync(id, cancellationToken));
    }

    private static async Task<IResult> UpdateCompletionStatus(int id, DateOnly date, IHabitService service)
    {
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(service.UpdateHabitStatus(id, date));
    }
}