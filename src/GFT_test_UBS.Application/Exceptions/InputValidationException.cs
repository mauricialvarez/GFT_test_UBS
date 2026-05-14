namespace GFT_test_UBS.Application.Exceptions;

public sealed class InputValidationException : Exception
{
    public InputValidationException(string message)
        : base(message)
    {
    }
}
