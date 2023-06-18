using System.Net.Http.Json;
using FluentAssertions;
using HabitTracker.Api.Payment;
using HabitTracker.Api.Tests.Integration.Infrastructure;

namespace HabitTracker.Api.Tests.Integration.Tests;

public class PaymentEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _habitsApi;

    private Func<int, bool, Task> _setUpPayment;

    public PaymentEndpointTests(ApiFactory webApplicationFactory)
    {
        _habitsApi = webApplicationFactory.HttpClient;
        _setUpPayment = webApplicationFactory.ExternalPaymentApi.SetUpPayment;
    }

    [Fact]
    public async Task WhenPaymentIsSuccessful_ReturnPaymentSucceeded()
    {
        // Arrange
        var paymentId = 1;
        var isSucceeded = true;
        await _setUpPayment(paymentId, isSucceeded);
        
        // Act
        var response = await _habitsApi.GetAsync($"api/v1/payments/{paymentId}");
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var paymentStatus = await response.Content.ReadFromJsonAsync<PaymentStatusResponse>();
        paymentStatus.Status.Should().Be("payment_succeeded");
    }
}