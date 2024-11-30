using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.UseCases.Payments.Create;

public class CreatePaymentHandler(IRepository<Payment> repository, IAcquiringBankingService acquiringBankingService)
    : ICommandHandler<CreatePaymentCommand, Result<AuthorizedPaymentDto>>
{
    public async Task<Result<AuthorizedPaymentDto>> Handle(CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var newPayment = new Payment(request.CardNumber.ToString(), request.ExpiryMonth, request.ExpiryYear,
            request.Cvv, request.Currency, request.Amount);

        var authorizePaymentResponse = await acquiringBankingService.AuthorizePayment(newPayment);

        if (authorizePaymentResponse.Authorized is false)
        {
            throw new Exception("Payment authorization failed");
        }

        newPayment.SetAuthorization("Success", authorizePaymentResponse.AuthorizationCode);

        await repository.AddAsync(newPayment, cancellationToken);

        var createdPaymentResponse = new AuthorizedPaymentDto(newPayment.Id, "Authorized",
            newPayment.LastFourCardDigits,
            newPayment.ExpiryMonth, newPayment.ExpiryYear, newPayment.Currency, newPayment.Amount,
            authorizePaymentResponse.AuthorizationCode);

        return createdPaymentResponse;
    }
}