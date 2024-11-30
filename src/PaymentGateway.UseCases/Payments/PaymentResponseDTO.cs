namespace PaymentGateway.UseCases.Payments;

public record AuthorizedPaymentDto(
    Guid Id,
    String Status,
    int CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    long Amount,
    Guid AuthorizationCode);