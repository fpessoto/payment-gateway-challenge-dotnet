namespace PaymentGateway.Core.Exceptions;

public class ExternalServiceUnavailableException : Exception
{
    public ExternalServiceUnavailableException()
    {
    }

    public ExternalServiceUnavailableException(string message) : base() { }

    public ExternalServiceUnavailableException(string message, Exception inner) : base(message, inner) { }
}