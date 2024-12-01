using System.Text.RegularExpressions;

namespace PaymentGateway.Core.Domains;

public class Payment(string cardNumber, int expiryMonth, int expiryYear, string cvv, Currency currency, long amount)
    : EntityBase<Guid>, IAggregateRoot
{
    public string EncryptedCardNumber { get; set; } = SetCardNumber(cardNumber);
    
    public string CardNumber { get; set; } = SetCardNumber(cardNumber);

    public int LastFourCardDigits { get; set; } =
        (SetCardNumber(cardNumber).Length > 4) ? Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 4)) : 0;

    public int ExpiryMonth { get; set; } = SetExpiryMonth(expiryMonth);

    private static int SetExpiryMonth(int expiryMonth)
    {
        if(expiryMonth is < 1 or > 12) throw new ArgumentOutOfRangeException( nameof(expiryMonth));
        
        return expiryMonth;
    }

    public int ExpiryYear { get; set; } = SetExpiryYear(expiryYear);

    private static int SetExpiryYear(int expiryYear)
    {
        if(expiryYear is < 1 or > 9999) throw new ArgumentOutOfRangeException( nameof(expiryYear));
        
        return expiryYear;
    }

    public string Cvv { get; set; } = SetCvv(cvv);

    private static string SetCvv(string cvv)
    {
        if(cvv.Length is < 3 or > 4 ) throw new ArgumentOutOfRangeException( nameof(cvv));
        
        if (!Regex.IsMatch(cvv, @"^\d+$"))
        {
            throw new ArgumentException("Cvv must contain only numeric characters.", nameof(cardNumber));
        }
        
        return cvv;
    }

    public Currency Currency { get; set; } = currency;
    public long Amount { get; set; } = amount;
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

        return cardNumber;
    }
   
}