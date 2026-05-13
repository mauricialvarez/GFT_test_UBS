using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
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
        var referenceDate = new DateTime(2020, 12, 11);
        var trade = new Trade(400_000, "Public", DateTime.ParseExact(nextPaymentDate, "MM/dd/yyyy", null));
        var rule = new ExpiredTradeCategoryRule();

        var result = rule.IsMatch(trade, referenceDate);

        Assert.Equal(expected, result);
    }
}
