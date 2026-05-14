namespace GFT_test_UBS.Domain.Entities;

public interface ITrade
{
    double Value { get; }
    string ClientSector { get; }
    DateTime NextPaymentDate { get; }
}
