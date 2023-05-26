namespace HabitTracker.Api.Infrastructure.Endpoints;

public interface IEndpointsDefinition
{
    public static abstract void ConfigureEndpoints(IEndpointRouteBuilder app);
}