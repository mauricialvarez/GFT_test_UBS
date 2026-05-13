using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class TradeCategorizerTests
{
    [Fact]
    public void Categorize_should_use_rules_in_precedence_order()
    {
        var referenceDate = new DateTime(2020, 12, 11);
        var expiredAndHighRiskTrade = new Trade(2_000_000, "Private", new DateTime(2020, 7, 1));
        var categorizer = CreateCategorizer();

        var category = categorizer.Categorize(expiredAndHighRiskTrade, referenceDate);

        Assert.Equal("EXPIRED", category);
    }

    [Fact]
    public void CategorizeMany_should_return_categories_in_input_order()
    {
        var referenceDate = new DateTime(2020, 12, 11);
        var trades = new[]
        {
            new Trade(2_000_000, "Private", new DateTime(2025, 12, 29)),
            new Trade(400_000, "Public", new DateTime(2020, 7, 1)),
            new Trade(5_000_000, "Public", new DateTime(2024, 1, 2)),
            new Trade(3_000_000, "Public", new DateTime(2023, 10, 26)),
        };
        var categorizer = CreateCategorizer();

        var categories = categorizer.CategorizeMany(trades, referenceDate);

        Assert.Equal(new[] { "HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK" }, categories);
    }

    private static TradeCategorizer CreateCategorizer()
    {
        return new TradeCategorizer(new ITradeCategoryRule[]
        {
            new ExpiredTradeCategoryRule(),
            new HighRiskTradeCategoryRule(),
            new MediumRiskTradeCategoryRule(),
        });
    }
}
