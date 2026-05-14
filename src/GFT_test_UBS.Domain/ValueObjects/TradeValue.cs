using GFT_test_UBS.Domain.Exceptions;

namespace GFT_test_UBS.Domain.ValueObjects;

public sealed record TradeValue
{
    public TradeValue(decimal amount)
    {
        if (amount < 0)
        {
            throw new InvalidTradeException("O valor da operacao nao pode ser negativo.");
        }

        Amount = amount;
    }

    public decimal Amount { get; }
}
