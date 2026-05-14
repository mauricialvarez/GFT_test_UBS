using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Application.Categorization;

public sealed class HighRiskTradeCategoryRule : ITradeCategoryRule
{
    public string Category => "HIGHRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000m
            && trade.ClientSector == ClientSector.Private;
    }
}
