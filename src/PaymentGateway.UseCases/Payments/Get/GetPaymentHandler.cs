using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Specifications;

namespace PaymentGateway.UseCases.Payments.Get;

/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetPaymentHandler(IReadRepository<Payment> repository)
    : IQueryHandler<GetPaymentQuery, Result<AuthorizedPaymentDto>>
{
    public async Task<Result<AuthorizedPaymentDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var spec = new PaymentGetByIdSpec(request.PaymentId);
        var entity = await repository.FirstOrDefaultAsync(spec, cancellationToken);
        if (entity == null) return Result.NotFound();

        return new AuthorizedPaymentDto(entity.Id, entity.Status.ToString(), entity.LastFourCardDigits, entity.ExpiryMonth,
            entity.ExpiryYear, entity.Currency, entity.Amount, entity.AuthorizationCode);
    }
}