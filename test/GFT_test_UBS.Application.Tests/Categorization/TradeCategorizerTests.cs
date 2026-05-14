using FluentAssertions;
using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Categorization;

public sealed class TradeCategorizerTests
{
    [Fact]
    public void Categorize_should_use_rules_in_precedence_order()
    {
        // Arrange
        var referenceDate = new DateTime(2020, 12, 11);
        var expiredAndHighRiskTrade = new Trade(2_000_000m, ClientSector.Private, new DateTime(2020, 7, 1));
        var categorizer = CreateCategorizer();

        // Act
        var category = categorizer.Categorize(expiredAndHighRiskTrade, referenceDate);

        // Assert
        category.Should().Be("EXPIRED");
    }

    [Fact]
    public void CategorizeMany_should_return_categories_in_input_order()
    {
        // Arrange
        var referenceDate = new DateTime(2020, 12, 11);
        var trades = new[]
        {
            new Trade(2_000_000m, ClientSector.Private, new DateTime(2025, 12, 29)),
            new Trade(400_000m, ClientSector.Public, new DateTime(2020, 7, 1)),
            new Trade(5_000_000m, ClientSector.Public, new DateTime(2024, 1, 2)),
            new Trade(3_000_000m, ClientSector.Public, new DateTime(2023, 10, 26)),
        };
        var categorizer = CreateCategorizer();

        // Act
        var categories = categorizer.CategorizeMany(trades, referenceDate);

        // Assert
        categories.Should().Equal("HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK");
    }

    [Fact]
    public void Categorize_should_return_uncategorized_when_no_rule_matches()
    {
        // Arrange
        var referenceDate = new DateTime(2020, 12, 11);
        var lowValueTrade = new Trade(400_000m, ClientSector.Private, new DateTime(2025, 12, 29));
        var categorizer = CreateCategorizer();

        // Act
        var category = categorizer.Categorize(lowValueTrade, referenceDate);

        // Assert
        category.Should().Be("UNCATEGORIZED");
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
