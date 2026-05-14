using GFT_test_UBS.Domain.Exceptions;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Domain.Entities;

public sealed class Trade : ITrade
{
    public Trade(decimal value, ClientSector clientSector, DateTime nextPaymentDate)
    {
        if (!Enum.IsDefined(clientSector))
        {
            throw new InvalidTradeException("Setor do cliente invalido.");
        }

        TradeValue = new TradeValue(value);
        ClientSector = clientSector;
        NextPaymentDate = nextPaymentDate;
    }

    public decimal Value => TradeValue.Amount;
    public TradeValue TradeValue { get; }
    public ClientSector ClientSector { get; }
    public DateTime NextPaymentDate { get; }

    public bool IsExpired(DateTime referenceDate)
    {
        return NextPaymentDate < referenceDate.AddDays(-30);
    }
}
