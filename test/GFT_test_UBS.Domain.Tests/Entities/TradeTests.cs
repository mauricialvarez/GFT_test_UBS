using GFT_test_UBS.Domain.Entities;
using Xunit;

namespace GFT_test_UBS.Domain.Tests.Entities;

public sealed class TradeTests
{
    [Fact]
    public void Constructor_should_create_trade_with_required_values()
    {
        var nextPaymentDate = new DateTime(2025, 12, 29);

        var trade = new Trade(2_000_000, "Private", nextPaymentDate);

        Assert.Equal(2_000_000, trade.Value);
        Assert.Equal("Private", trade.ClientSector);
        Assert.Equal(nextPaymentDate, trade.NextPaymentDate);
    }
}
