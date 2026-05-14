namespace GFT_test_UBS.Domain.Exceptions;

public sealed class InvalidTradeException : DomainException
{
    public InvalidTradeException(string message)
        : base(message)
    {
    }
}
