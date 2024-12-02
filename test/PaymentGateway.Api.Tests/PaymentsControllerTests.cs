using System.Net;
using System.Net.Http.Json;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Core.Domains;
using PaymentGateway.UseCases.Payments;

namespace PaymentGateway.Api.Tests;

[Collection("Sequential")]
public class PaymentsControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly Random _random = new();
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidData_ReturnPaymentAuthorized()
    {
        // Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            CardNumber = 2222405343248877,
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentDto>();
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        Assert.Equal(PaymentStatus.Authorized.ToString(), paymentResponse.Status);
        Assert.Equal(8877, paymentResponse.CardNumberLastFour);
        Assert.Equal(4, paymentResponse.ExpiryMonth);
        Assert.Equal(2025, paymentResponse.ExpiryYear);
        Assert.Equal("GBP", paymentResponse.Currency);
        Assert.Equal(100, paymentResponse.Amount);
    }

    [Fact]
    public async Task Create_WithInvalidCardData_ReturnPaymentDenied()
    {
        // Arrange
        CreatePaymentRequest paymentRequest = ValidCreatePaymentRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentDto>();
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        Assert.Equal(PaymentStatus.Declined.ToString(), paymentResponse.Status);
        Assert.Equal(8112, paymentResponse.CardNumberLastFour);
        Assert.Equal(1, paymentResponse.ExpiryMonth);
        Assert.Equal(2026, paymentResponse.ExpiryYear);
        Assert.Equal("USD", paymentResponse.Currency);
        Assert.Equal(60000, paymentResponse.Amount);
    }

    [Theory]
    [InlineData(0, 4, 2025, "GBP", 100, "12")] // Invalid card number, invalid CVV
    [InlineData(123456789012345, 13, 2025, "GBP", 100, "123")] // Invalid expiry month
    [InlineData(123456789012345, 4, 0, "GBP", 100, "123")] // Invalid expiry year
    [InlineData(123456789012345, 4, 2025, "", 100, "123")] // Empty currency
    [InlineData(123456789012345, 4, 2025, "ABC", 100, "123")] // Invalid currency
    [InlineData(123456789012345, 4, 2025, "GBP", -1, "123")] // Negative amount
    [InlineData(123456789012345, 4, 2025, "GBP", 100, "")] // Empty CVV
    public async Task Create_WithInvalidData_Returns400(
        long cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv)
    {
        // Arrange
        var invalidPaymentRequest = new CreatePaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = cvv
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", invalidPaymentRequest);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentDto>();
        
        Assert.NotNull(paymentResponse);
        Assert.Null(paymentResponse.Id);
        Assert.Equal(PaymentStatus.Rejected.ToString(), paymentResponse.Status);
    }

    [Fact]
    public async Task Create_WithMissingRequiredFields_Returns400()
    {
        // Arrange
        var incompletePaymentRequest = new CreatePaymentRequest
        {
            // Missing card number
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", incompletePaymentRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WithExistentPayment_RetrievesAPaymentSuccessfully()
    {
        var paymentRequest = ValidCreatePaymentRequest();
        var newPaymentResponse = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);
        var newPayment = await newPaymentResponse.Content.ReadFromJsonAsync<PaymentDto>();

        // Act
        var response = await _client.GetAsync($"/api/Payments/{newPayment?.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404d()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static CreatePaymentRequest ValidCreatePaymentRequest()
    {
        var paymentRequest = new CreatePaymentRequest
        {
            CardNumber = 2222405343248112,
            ExpiryMonth = 1,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 60000,
            Cvv = "456"
        };
        return paymentRequest;
    }
}