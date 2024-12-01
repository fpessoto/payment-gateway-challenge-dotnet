using System.Net;
using System.Net.Http.Json;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.UseCases.Payments;

namespace PaymentGateway.Api.Tests;

[Collection("Sequential")]
public class PaymentsControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly Random _random = new();
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreatesPaymentAuthorized()
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
        
        var paymentResponse = await response.Content.ReadFromJsonAsync<AuthorizedPaymentDto>();
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        Assert.Equal(PaymentStatus.Authorized.ToString(), paymentResponse.Status);
        Assert.Equal(8877, paymentResponse.CardNumberLastFour);
        Assert.Equal(4, paymentResponse.ExpiryMonth);
        Assert.Equal(2025, paymentResponse.ExpiryYear);
        Assert.Equal("GBP", paymentResponse.Currency);
        Assert.Equal(100, paymentResponse.Amount);
        Assert.False(string.IsNullOrEmpty(paymentResponse.AuthorizationCode));
    }

    [Fact]
    public async Task CreatesPaymentDenied()
    {
        // Arrange
        CreatePaymentRequest paymentRequest = ValidCreatePaymentRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var paymentResponse = await response.Content.ReadFromJsonAsync<AuthorizedPaymentDto>();
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        Assert.Equal(PaymentStatus.Declined.ToString(), paymentResponse.Status);
        Assert.Equal(8112, paymentResponse.CardNumberLastFour);
        Assert.Equal(1, paymentResponse.ExpiryMonth);
        Assert.Equal(2026, paymentResponse.ExpiryYear);
        Assert.Equal("USD", paymentResponse.Currency);
        Assert.Equal(60000, paymentResponse.Amount);
        Assert.True(string.IsNullOrEmpty(paymentResponse.AuthorizationCode));
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


    [Fact]
    public async Task Returns400ForInvalidPaymentRequest()
    {
        // Arrange
        var invalidPaymentRequest = new CreatePaymentRequest
        {
            CardNumber = 123, // Invalid card number
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 100,
            Cvv = "12" // Invalid CVV
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", invalidPaymentRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Returns400ForMissingRequiredFields()
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
    public async Task RetrievesAPaymentSuccessfully()
    {
        var paymentRequest = ValidCreatePaymentRequest();
        var newPaymentResponse = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);
        var newPayment = await newPaymentResponse.Content.ReadFromJsonAsync<AuthorizedPaymentDto>();
        
        // Act
        var response = await _client.GetAsync($"/api/Payments/{newPayment?.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<AuthorizedPaymentDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}