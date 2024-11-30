using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Infrastructure.AcquiringBanking;

public class AcquiringBankingService : IAcquiringBankingService
{
    private HttpClient _httpClient = new HttpClient();

    public async Task<AuthorizePaymentResponse> AuthorizePayment(Payment payment)
    {
        const string url = "http://localhost:8080/payments";

        try
        {
            var month = ConvertMonthToString(payment.ExpiryMonth);
            // Prepare the request body as JSON
            var requestBody = new PaymentRequestDto
            {
                CardNumber = payment.CardNumber,
                ExpiryDate = $"{month}/{payment.ExpiryYear}",
                Currency = payment.Currency,
                Amount = payment.Amount,
                Cvv = payment.Cvv.ToString(),
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // Send the POST request
            var response = await _httpClient.PostAsync(url, httpContent);

            // Check if the response is successful
            response.EnsureSuccessStatusCode();

            // Deserialize the response body
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponseDto>(responseBody, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Allow case-insensitive deserialization
            });

            return new AuthorizePaymentResponse(result.Authorized, result.AuthorizationCode);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., logging, rethrowing, or returning a default response)
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private string ConvertMonthToString(int month)
    {
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }

        return month.ToString("D2");
    }

    public class PaymentRequestDto
    {
        [JsonPropertyName("card_number")] public string CardNumber { get; set; }

        [JsonPropertyName("expiry_date")] public string ExpiryDate { get; set; }

        [JsonPropertyName("currency")] public string Currency { get; set; }

        [JsonPropertyName("amount")] public decimal Amount { get; set; }

        [JsonPropertyName("cvv")] public string Cvv { get; set; }
    }

    public class PaymentResponseDto
    {
        [JsonPropertyName("authorized")] public bool Authorized { get; set; }

        [JsonPropertyName("authorization_code")]
        public Guid AuthorizationCode { get; set; }
    }
}