using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Specifications;

namespace PaymentGateway.UnitTests.Core.Specifications
{
    public class PaymentGetByIdSpecTests
    {
        [Fact]
        public void Constructor_WhenPaymentIdProvided_ShouldFilterByPaymentId()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var otherPaymentId = Guid.NewGuid();

            var payments = new List<Payment>
            {
                new Payment("4111111111111111", 12, 2025, "123", Currency.USD, 1000) { Id = paymentId },
                new Payment("4111111111112222", 11, 2026, "456", Currency.EUR, 2000) { Id = otherPaymentId }
            };

            // Act
            var spec = new PaymentGetByIdSpec(paymentId);
            var filteredPayments = spec.Evaluate(payments).ToList();

            // Assert
            Assert.Single(filteredPayments);
            Assert.Equal(paymentId, filteredPayments.First().Id);
        }

        [Fact]
        public void Constructor_WhenPaymentIdNotMatched_ShouldReturnEmpty()
        {
            // Arrange
            var paymentId = Guid.NewGuid(); // ID that doesn't match any payment
            var payments = new List<Payment>
            {
                new("4111111111111111", 12, 2025, "123", Currency.USD, 1000) { Id = Guid.NewGuid() },
                new("4111111111112222", 11, 2026, "456", Currency.EUR, 2000) { Id = Guid.NewGuid() }
            };

            // Act
            var spec = new PaymentGetByIdSpec(paymentId);
            var filteredPayments = spec.Evaluate(payments).ToList();

            // Assert
            Assert.Empty(filteredPayments);
        }
    }
}