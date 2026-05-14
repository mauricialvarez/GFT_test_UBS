using FluentAssertions;
using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class ExpiredTradeCategoryRuleTests
{
    [Theory]
    [InlineData("07/01/2020", true)]
    [InlineData("11/11/2020", false)]
    [InlineData("11/10/2020", true)]
    [InlineData("12/29/2025", false)]
    public void IsMatch_should_identify_trades_expired_by_more_than_30_days(
        string nextPaymentDate,
        bool expected)
    {
        // Arrange
        var referenceDate = new DateTime(2020, 12, 11);
        var trade = CreateTrade(nextPaymentDate);
        var rule = new ExpiredTradeCategoryRule();

        // Act
        var result = rule.IsMatch(trade, referenceDate);

        // Assert
        result.Should().Be(expected);
    }

    private static Trade CreateTrade(string nextPaymentDate)
    {
        return new Trade(
            400_000m,
            ClientSector.Public,
            DateTime.ParseExact(nextPaymentDate, "MM/dd/yyyy", null));
    }
}
