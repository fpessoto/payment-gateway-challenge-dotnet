using System.Text.RegularExpressions;

namespace PaymentGateway.Core.Domains;

public class Payment(string cardNumber, int expiryMonth, int expiryYear, int cvv, string currency, long amount)
    : EntityBase<Guid>, IAggregateRoot
{
    public string EncryptedCardNumber { get; set; } = ValidCardNumber(cardNumber)
        ? cardNumber
        : throw new ArgumentException("Invalid Card Number", nameof(cardNumber));

    public string CardNumber { get; set; } = ValidCardNumber(cardNumber)
        ? cardNumber
        : throw new ArgumentException("Invalid Card Number", nameof(cardNumber));

    public int LastFourCardDigits { get; set; } =
        (cardNumber.Length > 4) ? Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 4)) : 0;

    public int ExpiryMonth { get; set; } = expiryMonth;
    public int ExpiryYear { get; set; } = expiryYear;
    public int Cvv { get; set; } = cvv;
    public string Currency { get; set; } = currency;
    public long Amount { get; set; } = amount;
    public PaymentStatus Status { get; set; } = PaymentStatus.NotSet;
    public string? AuthorizationCode { get; set; }

    public void SetAuthorization(PaymentStatus status, string? code)
    {
        Status = status;
        AuthorizationCode = code;
    }

    private static bool ValidCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new ArgumentNullException(nameof(cardNumber), "Card Number is required");
        }

        if (cardNumber.Length is < 14 or > 19)
        {
            throw new ArgumentException("Card number must be between 14 and 19 characters long.", nameof(cardNumber));
        }

        if (!Regex.IsMatch(cardNumber, @"^\d+$"))
        {
            throw new ArgumentException("Card number must contain only numeric characters.", nameof(cardNumber));
        }

        return true;
    }
}