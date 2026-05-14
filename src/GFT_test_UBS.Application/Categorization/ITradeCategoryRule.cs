using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Categorization;

public interface ITradeCategoryRule
{
    string Category { get; }

    bool IsMatch(ITrade trade, DateTime referenceDate);
}
