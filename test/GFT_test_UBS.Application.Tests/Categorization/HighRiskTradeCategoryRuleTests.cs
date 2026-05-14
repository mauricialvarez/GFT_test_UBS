using FluentAssertions;
using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class HighRiskTradeCategoryRuleTests
{
    [Theory]
    [InlineData(1_000_001, ClientSector.Private, true)]
    [InlineData(1_000_000, ClientSector.Private, false)]
    [InlineData(1_000_001, ClientSector.Public, false)]
    public void IsMatch_should_identify_private_trades_above_one_million(
        decimal value,
        ClientSector clientSector,
        bool expected)
    {
        // Arrange
        var trade = new Trade(value, clientSector, new DateTime(2025, 12, 29));
        var rule = new HighRiskTradeCategoryRule();

        // Act
        var result = rule.IsMatch(trade, new DateTime(2020, 12, 11));

        // Assert
        result.Should().Be(expected);
    }
}
