using FluentAssertions;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.Exceptions;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Domain.Tests.Entities;

public sealed class TradeTests
{
    [Fact]
    public void Constructor_should_create_trade_with_required_values()
    {
        // Arrange
        var nextPaymentDate = new DateTime(2025, 12, 29);

        // Act
        var trade = new Trade(2_000_000m, ClientSector.Private, nextPaymentDate);

        // Assert
        trade.Value.Should().Be(2_000_000m);
        trade.TradeValue.Amount.Should().Be(2_000_000m);
        trade.ClientSector.Should().Be(ClientSector.Private);
        trade.NextPaymentDate.Should().Be(nextPaymentDate);
    }

    [Fact]
    public void Constructor_should_not_allow_negative_value()
    {
        // Arrange
        var nextPaymentDate = new DateTime(2025, 12, 29);

        // Act
        var act = () => new Trade(-1m, ClientSector.Private, nextPaymentDate);

        // Assert
        act.Should()
            .Throw<InvalidTradeException>()
            .WithMessage("O valor da operacao nao pode ser negativo.");
    }

    [Fact]
    public void Constructor_should_not_allow_undefined_client_sector()
    {
        // Arrange
        var undefinedClientSector = (ClientSector)999;
        var nextPaymentDate = new DateTime(2025, 12, 29);

        // Act
        var act = () => new Trade(1_000_000m, undefinedClientSector, nextPaymentDate);

        // Assert
        act.Should()
            .Throw<InvalidTradeException>()
            .WithMessage("Setor do cliente invalido.");
    }

    [Theory]
    [InlineData("07/01/2020", true)]
    [InlineData("11/11/2020", false)]
    [InlineData("11/10/2020", true)]
    public void IsExpired_should_identify_trades_expired_by_more_than_30_days(
        string nextPaymentDate,
        bool expected)
    {
        // Arrange
        var referenceDate = new DateTime(2020, 12, 11);
        var trade = CreatePublicTrade(nextPaymentDate);

        // Act
        var result = trade.IsExpired(referenceDate);

        // Assert
        result.Should().Be(expected);
    }

    private static Trade CreatePublicTrade(string nextPaymentDate)
    {
        return new Trade(
            400_000m,
            ClientSector.Public,
            DateTime.ParseExact(nextPaymentDate, "MM/dd/yyyy", null));
    }
}
