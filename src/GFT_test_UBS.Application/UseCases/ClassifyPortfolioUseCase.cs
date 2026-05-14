using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Application.Input;

namespace GFT_test_UBS.Application.UseCases;

public sealed class ClassifyPortfolioUseCase
{
    private readonly PortfolioInputParser _inputParser;
    private readonly TradeCategorizer _tradeCategorizer;

    public ClassifyPortfolioUseCase()
        : this(
            new PortfolioInputParser(),
            new TradeCategorizer(new ITradeCategoryRule[]
            {
                new ExpiredTradeCategoryRule(),
                new HighRiskTradeCategoryRule(),
                new MediumRiskTradeCategoryRule(),
            }))
    {
    }

    public ClassifyPortfolioUseCase(PortfolioInputParser inputParser, TradeCategorizer tradeCategorizer)
    {
        _inputParser = inputParser;
        _tradeCategorizer = tradeCategorizer;
    }

    public IReadOnlyCollection<string> Execute(IEnumerable<string> inputLines)
    {
        var input = _inputParser.Parse(inputLines);

        return _tradeCategorizer.CategorizeMany(input.Trades, input.ReferenceDate);
    }
}
