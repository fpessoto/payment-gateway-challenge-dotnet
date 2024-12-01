using PaymentGateway.Core.Domains;
using PaymentGateway.Core.Exceptions;

namespace PaymentGateway.UnitTests.Core.Domains
{
    public class PaymentTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties_WhenValidDataProvided()
        {
            // Arrange
            string cardNumber = "4111111111111111";
            int expiryMonth = 12;
            int expiryYear = 2025;
            string cvv = "123";
            Currency currency = Currency.USD;
            long amount = 1000;

            // Act
            var payment = new Payment(cardNumber, expiryMonth, expiryYear, cvv, currency, amount);

            // Assert
            Assert.Equal("4111111111111111", payment.CardNumber);
            Assert.Equal(1111, payment.LastFourCardDigits);
            Assert.Equal(12, payment.ExpiryMonth);
            Assert.Equal(2025, payment.ExpiryYear);
            Assert.Equal("123", payment.Cvv);
            Assert.Equal(Currency.USD, payment.Currency);
            Assert.Equal(1000, payment.Amount);
            Assert.Equal(PaymentStatus.NotSet, payment.Status);
        }

        [Theory]
        [InlineData("", 12, 2025, "123", "USD", 1000)]
        [InlineData(null, 12, 2025, "123", "USD", 1000)]
        public void Constructor_ShouldThrowBusinessExceptionException_WhenCardNumberIsNullOrEmpty(
            string cardNumber, int expiryMonth, int expiryYear, string cvv, string currency, long amount)
        {
            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, Currency.FromName(currency), amount));
        }

        [Theory]
        [InlineData("411111111111", 12, 2025, "123", "USD", 1000)]
        [InlineData("41111111111111111111", 12, 2025, "123", "USD", 1000)]
        public void Constructor_ShouldThrowBusinessException_WhenCardNumberLengthInvalid(
            string cardNumber, int expiryMonth, int expiryYear, string cvv, string currency, long amount)
        {
            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, Currency.FromName(currency), amount));
        }

        [Theory]
        [InlineData("4111abcd1111", 12, 2025, "123", "USD", 1000)]
        public void Constructor_ShouldThrowBusinessException_WhenCardNumberNotNumeric(
            string cardNumber, int expiryMonth, int expiryYear, string cvv, string currency, long amount)
        {
            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, Currency.FromName(currency), amount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void Constructor_ShouldThrowBusinessExceptionException_WhenExpiryMonthInvalid(int expiryMonth)
        {
            // Arrange
            string cardNumber = "4111111111111111";
            int expiryYear = 2025;
            string cvv = "123";
            Currency currency = Currency.USD;
            long amount = 1000;

            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, currency, amount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public void Constructor_ShouldThrowBusinessException_WhenExpiryYearInvalid(int expiryYear)
        {
            // Arrange
            string cardNumber = "4111111111111111";
            int expiryMonth = 12;
            string cvv = "123";
            Currency currency = Currency.USD;
            long amount = 1000;

            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, currency, amount));
        }

        [Theory]
        [InlineData("12")]
        [InlineData("12345")]
        public void Constructor_ShouldThrowBusinessException_WhenCvvInvalid(string cvv)
        {
            // Arrange
            string cardNumber = "4111111111111111";
            int expiryMonth = 12;
            int expiryYear = 2025;
            Currency currency = Currency.USD;
            long amount = 1000;

            // Act & Assert
            Assert.Throws<BusinessException>(() =>
                new Payment(cardNumber, expiryMonth, expiryYear, cvv, currency, amount));
        }

        [Fact]
        public void SetAuthorization_ShouldSetStatusAndAuthorizationCode()
        {
            // Arrange
            string cardNumber = "4111111111111111";
            int expiryMonth = 12;
            int expiryYear = 2025;
            string cvv = "123";
            Currency currency = Currency.USD;
            long amount = 1000;
            var payment = new Payment(cardNumber, expiryMonth, expiryYear, cvv, currency, amount);

            // Act
            payment.SetAuthorization(PaymentStatus.Authorized, "AUTH12345");

            // Assert
            Assert.Equal(PaymentStatus.Authorized, payment.Status);
            Assert.Equal("AUTH12345", payment.AuthorizationCode);
        }
    }
}
