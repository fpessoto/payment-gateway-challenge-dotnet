namespace PaymentGateway.Core.Domains;

public class PaymentStatus : SmartEnum<PaymentStatus>
{
    public static readonly PaymentStatus NotSet = new(nameof(NotSet), 1);
    public static readonly PaymentStatus Authorized = new(nameof(Authorized), 2);
    public static readonly PaymentStatus Declined = new(nameof(Declined), 3);
    public static readonly PaymentStatus Rejected = new(nameof(Rejected), 4);
    protected PaymentStatus(string name, int value) : base(name, value) { }
}