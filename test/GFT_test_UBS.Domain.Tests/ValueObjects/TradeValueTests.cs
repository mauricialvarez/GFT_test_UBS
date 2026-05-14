using FluentAssertions;
using GFT_test_UBS.Domain.Exceptions;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Domain.Tests.ValueObjects;

public sealed class TradeValueTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1_000_000)]
    public void Constructor_should_accept_zero_and_positive_amounts(decimal amount)
    {
        // Arrange
        // Act
        var tradeValue = new TradeValue(amount);

        // Assert
        tradeValue.Amount.Should().Be(amount);
    }

    [Fact]
    public void Constructor_should_not_accept_negative_amount()
    {
        // Arrange
        // Act
        var act = () => new TradeValue(-0.01m);

        // Assert
        act.Should()
            .Throw<InvalidTradeException>()
            .WithMessage("O valor da operacao nao pode ser negativo.");
    }
}
