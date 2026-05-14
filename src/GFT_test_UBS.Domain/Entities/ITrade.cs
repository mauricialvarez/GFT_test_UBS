using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Domain.Entities;

public interface ITrade
{
    decimal Value { get; }
    ClientSector ClientSector { get; }
    DateTime NextPaymentDate { get; }

    bool IsExpired(DateTime referenceDate);
}
