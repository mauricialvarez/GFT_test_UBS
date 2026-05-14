using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Categorization;

public sealed class TradeCategorizer
{
    private readonly IReadOnlyCollection<ITradeCategoryRule> _rules;

    public TradeCategorizer(IEnumerable<ITradeCategoryRule> rules)
    {
        _rules = rules.ToArray();
    }

    public string Categorize(ITrade trade, DateTime referenceDate)
    {
        var rule = _rules.FirstOrDefault(rule => rule.IsMatch(trade, referenceDate));

        return rule?.Category ?? "UNCATEGORIZED";
    }

    public IReadOnlyCollection<string> CategorizeMany(IEnumerable<ITrade> trades, DateTime referenceDate)
    {
        return trades
            .Select(trade => Categorize(trade, referenceDate))
            .ToArray();
    }
}
