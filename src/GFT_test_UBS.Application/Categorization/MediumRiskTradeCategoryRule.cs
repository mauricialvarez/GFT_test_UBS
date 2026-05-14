using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Application.Categorization;

public sealed class MediumRiskTradeCategoryRule : ITradeCategoryRule
{
    public string Category => "MEDIUMRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000m
            && trade.ClientSector == ClientSector.Public;
    }
}
