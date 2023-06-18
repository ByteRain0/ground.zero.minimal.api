namespace HabitTracker.Api.Payment;

public class PaymentServiceClient
{
    private readonly HttpClient _httpClient;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentStatusResponse> RetrievePaymentStatusAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/v1/payments/{id}/status");

        if (response.IsSuccessStatusCode)
        {
            var paymentStatus = await response.Content.ReadFromJsonAsync<PaymentStatusResponse>();
            return paymentStatus;
        }

        return default;
    }
}