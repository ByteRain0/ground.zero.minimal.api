using HabitTracker.Api.Habits.Contracts;
using HabitTracker.Api.Habits.Contracts.Models;
using HabitTracker.Api.Infrastructure.DateTime;
using HabitTracker.Api.Infrastructure.Endpoints;
using O9d.AspNet.FluentValidation;

namespace HabitTracker.Api.Habits.Endpoints;

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

        group.MapGet("/{id:int}/currentStreak", GetCurrentStreakAsync)
            .Produces(404)
            .Produces<int>(200)
            .WithName("GetCurrentStreak");
        
        group.MapGet("/{id:int}/longestStreak", GetLongestStreakAsync)
            .Produces(404)
            .Produces<int>(200)
            .WithName("GetLongestStreak");

        group.MapDelete("/{id:int}/status/{date}", RemoveCompletionStatusAsync)
            .Produces(404)
            .Produces(204)
            .WithName("RemoveCompletionStatus");
        
        group.MapPost("/{id:int}/status/{date}", AddCompletionStatusAsync)
            .Produces(404)
            .Produces(201)
            .WithName("AddCompletionStatus");
    }
    
    private static async Task<IResult> CreateHabitAsync(
        [Validate] Habit habit, 
        IHabitService service,
        LinkGenerator linkGenerator, 
        HttpContext context)
    {
        var habitId = await service.AddHabitAsync(habit);
        habit.Id = (uint)habitId;
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
        model.Id = (uint)id;
        var updated = await service.UpdateHabitAsync(model);
        return updated ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> RemoveHabitAsync(
        int id, 
        IHabitService service)
    {
        var removed = await service.RemoveHabitAsync(id);
        return removed ? Results.NoContent() : Results.NotFound();
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

        return Results.Ok(await 
            service.GetMonthlyCompletionStatus(
                id, DateOnly.FromDateTime(dateTimeProvider.CurrentTime()), cancellationToken));
    }

    private static async Task<IResult> GetCurrentStreakAsync(
        int id,
        IHabitService service,
        IDateTimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Might want to move this to a separate private method to deduplicate code.
        var habit = await service.GetHabitAsync(id, CancellationToken.None);

        if (habit is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(await service.GetHabitCurrentStreakAsync(id, DateOnly.FromDateTime(timeProvider.CurrentTime()), cancellationToken));
    }
    
    private static async Task<IResult> GetLongestStreakAsync(
        int id, 
        IHabitService service,
        CancellationToken cancellationToken)
    {
        var habit = await service.GetHabitAsync(id, CancellationToken.None);
        return habit == null ? 
            Results.NotFound() 
            : Results.Ok(await service.GetHabitLongestStreakAsync(id, cancellationToken)) ;
    }

    private static async Task<IResult> AddCompletionStatusAsync(
        int id, 
        DateOnly date, 
        IHabitService service,
        LinkGenerator linkGenerator, 
        HttpContext context)
    {
        var created = await service.AddCompletedDay(id, date);
        var path = linkGenerator.GetUriByName(context, "MonthlyCompletionStatus", new { id = id })!;
        return created ? Results.Created(path, default) : Results.NotFound();
    }
    
    private static async Task<IResult> RemoveCompletionStatusAsync(int id, DateOnly date, IHabitService service)
    {
        var removed = await service.RemoveCompletedDay(id, date);
        return removed ? Results.NoContent() : Results.NotFound();
    }
}