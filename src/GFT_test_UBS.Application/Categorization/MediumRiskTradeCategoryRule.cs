using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Categorization;

public sealed class MediumRiskTradeCategoryRule : ITradeCategoryRule
{
    public string Category => "MEDIUMRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000
            && string.Equals(trade.ClientSector, "Public", StringComparison.OrdinalIgnoreCase);
    }
}
