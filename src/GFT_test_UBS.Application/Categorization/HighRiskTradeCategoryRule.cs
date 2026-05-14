using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Categorization;

public sealed class HighRiskTradeCategoryRule : ITradeCategoryRule
{
    public string Category => "HIGHRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000
            && string.Equals(trade.ClientSector, "Private", StringComparison.OrdinalIgnoreCase);
    }
}
