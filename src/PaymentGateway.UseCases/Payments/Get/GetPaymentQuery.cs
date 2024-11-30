namespace PaymentGateway.UseCases.Payments.Get;

public record GetPaymentQuery(Guid PaymentId) : IQuery<Result<AuthorizedPaymentDto>>;
