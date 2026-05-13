using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class HighRiskTradeCategoryRuleTests
{
    [Theory]
    [InlineData(1_000_001, "Private", true)]
    [InlineData(1_000_000, "Private", false)]
    [InlineData(1_000_001, "Public", false)]
    public void IsMatch_should_identify_private_trades_above_one_million(
        double value,
        string clientSector,
        bool expected)
    {
        var trade = new Trade(value, clientSector, new DateTime(2025, 12, 29));
        var rule = new HighRiskTradeCategoryRule();

        var result = rule.IsMatch(trade, new DateTime(2020, 12, 11));

        Assert.Equal(expected, result);
    }
}
