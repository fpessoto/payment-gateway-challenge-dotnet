namespace PaymentGateway.UseCases.Payments.Create;

/// <summary>
/// Create a new Payment.
/// </summary>
/// <param name="CardNumber"></param>
/// <param name="ExpiryMonth"></param>
/// <param name="ExpiryYear"></param>
/// <param name="Cvv"></param>
/// <param name="Currency"></param>
/// <param name="Amount"></param>
public record CreatePaymentCommand(
    long CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Cvv,
    string Currency,
    long Amount) : ICommand<Result<AuthorizedPaymentDto>>;