using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Specifications;

namespace PaymentGateway.UseCases.Payments.Get;

/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetPaymentHandler(IReadRepository<Payment> repository)
    : IQueryHandler<GetPaymentQuery, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var spec = new PaymentGetByIdSpec(request.PaymentId);
        var entity = await repository.FirstOrDefaultAsync(spec, cancellationToken);
        if (entity == null) return Result.NotFound();

        return new PaymentDto(entity.Id, entity.Status.ToString(), entity.LastFourCardDigits, entity.ExpiryMonth,
            entity.ExpiryYear, entity.Currency.ToString(), entity.Amount, entity.AuthorizationCode);
    }
}