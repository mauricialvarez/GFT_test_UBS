using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Categorization;

public sealed class ExpiredTradeCategoryRule : ITradeCategoryRule
{
    public string Category => "EXPIRED";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.IsExpired(referenceDate);
    }
}
