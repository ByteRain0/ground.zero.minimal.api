namespace HabitTracker.Api.Payment;

public static class PaymentServiceInstaller
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddHttpClient<PaymentServiceClient>(opts =>
        {
            opts.BaseAddress = new Uri("https://example.com");
        });

        return services;
    }
}