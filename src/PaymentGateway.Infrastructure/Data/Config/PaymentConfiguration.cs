using PaymentGateway.Core.Domains;

namespace PaymentGateway.Infrastructure.Data.Config;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.CardNumber)
            .IsRequired();
        builder.Property(p => p.ExpiryMonth).IsRequired();
        builder.Property(p => p.ExpiryYear).IsRequired();
        builder.Property(p => p.Cvv).IsRequired();
        builder.Property(p => p.Currency).HasConversion(
            x => x.Value,
            x => Currency.FromValue(x)).IsRequired();
        builder.Property(p => p.Amount).IsRequired();
        builder.Property(p => p.LastFourCardDigits).IsRequired();
        builder.Property(p => p.Status).HasConversion(
            x => x.Value,
            x => PaymentStatus.FromValue(x)).IsRequired();
        builder.Property(p => p.AuthorizationCode).IsRequired();
    }
}