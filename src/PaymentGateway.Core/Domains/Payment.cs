using PaymentGateway.Core.Exceptions;

using static System.Text.RegularExpressions.Regex;

namespace PaymentGateway.Core.Domains;

public class Payment(string cardNumber, int expiryMonth, int expiryYear, string cvv, Currency currency, long amount)
    : EntityBase<Guid>, IAggregateRoot
{
    public string CardNumber { get; init; } = SetCardNumber(cardNumber);

    public int LastFourCardDigits { get; init; } =
        SetLastFourCardDigits(cardNumber);

    private static int SetLastFourCardDigits(string cardNumber)
    {
        return (SetCardNumber(cardNumber).Length > 4)
            ? Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 4))
            : 0;
    }

    public int ExpiryMonth { get; init; } = SetExpiryMonth(expiryMonth);

    private static int SetExpiryMonth(int expiryMonth)
    {
        if (expiryMonth is < 1 or > 12) throw new BusinessException("Expiry month must be between 1 and 12");

        return expiryMonth;
    }

    public int ExpiryYear { get; init; } = SetExpiryYear(expiryYear);

    private static int SetExpiryYear(int expiryYear)
    {
        if (expiryYear is < 1 or > 9999) throw new BusinessException("Expiry year must be between 1 and 9999");

        return expiryYear;
    }

    public string Cvv { get; init; } = SetCvv(cvv);

    private static string SetCvv(string cvv)
    {
        if (cvv.Length is < 3 or > 4) throw new BusinessException("Cvv should contain 3 or 4 characters");

        if (!IsMatch(cvv, @"^\d+$"))
        {
            throw new BusinessException("Cvv must contain only numeric characters.");
        }

        return cvv;
    }

    public Currency Currency { get; init; } = currency;

    public long Amount { get; init; } = Guard.Against.NegativeOrZero(amount, nameof(amount),
        exceptionCreator: () => new BusinessException("Amount must be greater than zero."));

    public PaymentStatus Status { get; set; } = PaymentStatus.NotSet;
    public string? AuthorizationCode { get; set; }

    public void SetAuthorization(PaymentStatus status, string? code)
    {
        Status = status;
        AuthorizationCode = code;
    }

    private static string SetCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new BusinessException("Card Number is required");
        }

        if (cardNumber.Length is < 14 or > 19)
        {
            throw new BusinessException("Card number must be between 14 and 19 characters long.");
        }

        if (!IsMatch(cardNumber, @"^\d+$"))
        {
            throw new BusinessException("Card number must contain only numeric characters.");
        }

        return cardNumber;
    }
}