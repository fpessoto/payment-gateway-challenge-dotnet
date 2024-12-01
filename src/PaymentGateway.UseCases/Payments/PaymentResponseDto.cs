namespace PaymentGateway.UseCases.Payments;

public record PaymentDto(
    Guid Id,
    String Status,
    int CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    long Amount,
    string? AuthorizationCode);