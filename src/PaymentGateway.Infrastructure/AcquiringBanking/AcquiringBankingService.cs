using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Exceptions;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Infrastructure.AcquiringBanking;

public class AcquiringBankingService : IAcquiringBankingService
{
    private readonly ILogger<AcquiringBankingService> _logger;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string? _baseUrl;

    public AcquiringBankingService(ILogger<AcquiringBankingService> logger, IConfiguration configuration)
    {
        _logger = logger;

        // Retrieve the base URL from appsettings.json
        _baseUrl = configuration["AcquiringBank:BaseUrl"];
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentNullException(nameof(_baseUrl), "Acquiring Bank BaseUrl is not configured.");
        }
    }

    public async Task<AuthorizePaymentResponse> AuthorizePayment(Payment payment)
    {
        const string endpoint = "/payments";

        try
        {
            var month = ConvertMonthToString(payment.ExpiryMonth);

            // Prepare the request body as JSON
            var requestBody = new PaymentRequestDto
            {
                CardNumber = payment.CardNumber,
                ExpiryDate = $"{month}/{payment.ExpiryYear}",
                Currency = payment.Currency.ToString(),
                Amount = payment.Amount,
                Cvv = payment.Cvv.ToString(),
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Build the full URL
            var url = $"{_baseUrl}{endpoint}";

            _logger.LogInformation("Acquiring service {url}", url);

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
            _logger.LogError(ex, "Error calling Acquiring Banking Service");
            throw new ExternalServiceUnavailableException("Banking Service Exception");
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
        public string? AuthorizationCode { get; set; }
    }
}