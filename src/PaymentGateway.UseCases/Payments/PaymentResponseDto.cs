namespace PaymentGateway.UseCases.Payments;

public record PaymentDto(
    Guid? Id,
    string Status,
    int? CardNumberLastFour,
    int? ExpiryMonth,
    int? ExpiryYear,
    string? Currency,
    long? Amount);