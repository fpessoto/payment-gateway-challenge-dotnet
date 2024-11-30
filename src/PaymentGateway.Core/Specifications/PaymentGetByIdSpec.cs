using PaymentGateway.Core.Domains;

namespace PaymentGateway.Core.Specifications;

public class PaymentGetByIdSpec : Specification<Payment>
{
    public PaymentGetByIdSpec(Guid paymentId) =>
        Query
            .Where(payment => payment.Id == paymentId);
}