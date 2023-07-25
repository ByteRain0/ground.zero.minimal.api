using System.Text.Json;
using HabitTracker.Api.Payment;
using WireMock.Client.Extensions;
using WireMock.Net.Testcontainers;

namespace HabitTracker.Api.Tests.Integration.Infrastructure;

public class PaymentApiServer : IAsyncDisposable
{

    private readonly WireMockContainer _paymentApi = new WireMockContainerBuilder()
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .Build();

    public string Url => _paymentApi.GetPublicUrl();

    public async Task Start()
    {
        await _paymentApi.StartAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _paymentApi.StopAsync();
        await _paymentApi.DisposeAsync();
    }

    public async Task SetUpPayment(int id, bool isSuccessful)
    {
        var status = isSuccessful ? "payment_succeeded" : "payment_failed";

        var response = new PaymentStatusResponse
        {
            Status = status
        };

        var mappingBuilder = _paymentApi
            .CreateWireMockAdminClient()
            .GetMappingBuilder();
        
        mappingBuilder.Given(m => m
            .WithRequest(req => req
                .UsingGet()
                .WithPath($"/api/v1/payments/{id}/status")
            )
            .WithResponse(rsp => rsp
                .WithBody(JsonSerializer.Serialize(response))
                .WithStatusCode(200)
            )
        );

        await mappingBuilder.BuildAndPostAsync();
    }
}