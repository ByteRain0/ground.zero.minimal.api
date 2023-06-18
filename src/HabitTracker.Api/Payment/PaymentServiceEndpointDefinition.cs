using HabitTracker.Api.Infrastructure.Endpoints;

namespace HabitTracker.Api.Payment;

public class PaymentServiceEndpointDefinition : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/payments/{id:int}", GetPaymentStatus);
    }
    
    private static async Task<IResult> GetPaymentStatus(int id, PaymentServiceClient httpClient) => Results.Ok(await httpClient.RetrievePaymentStatusAsync(id));
}