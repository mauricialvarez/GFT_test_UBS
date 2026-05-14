using FluentAssertions;
using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class MediumRiskTradeCategoryRuleTests
{
    [Theory]
    [InlineData(1_000_001, ClientSector.Public, true)]
    [InlineData(1_000_000, ClientSector.Public, false)]
    [InlineData(1_000_001, ClientSector.Private, false)]
    public void IsMatch_should_identify_public_trades_above_one_million(
        decimal value,
        ClientSector clientSector,
        bool expected)
    {
        // Arrange
        var trade = new Trade(value, clientSector, new DateTime(2025, 12, 29));
        var rule = new MediumRiskTradeCategoryRule();

        // Act
        var result = rule.IsMatch(trade, new DateTime(2020, 12, 11));

        // Assert
        result.Should().Be(expected);
    }
}
