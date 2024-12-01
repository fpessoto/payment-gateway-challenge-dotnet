namespace PaymentGateway.Core.Domains;

public class Currency : SmartEnum<Currency>
{
    public static readonly Currency USD = new(nameof(USD), 1);
    public static readonly Currency GBP = new(nameof(GBP), 2);
    public static readonly Currency EUR = new(nameof(EUR), 3);
    protected Currency(string name, int value) : base(name, value) { }
}