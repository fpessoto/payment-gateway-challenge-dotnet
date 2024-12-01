using PaymentGateway.Core.Domains;

namespace PaymentGateway.Core.Interfaces;

public interface IAcquiringBankingService
{
    Task<AuthorizePaymentResponse> AuthorizePayment(Payment payment);
}

public record AuthorizePaymentResponse(bool Authorized, string? AuthorizationCode);