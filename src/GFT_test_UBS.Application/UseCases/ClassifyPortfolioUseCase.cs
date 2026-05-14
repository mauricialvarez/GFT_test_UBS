using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Application.Input;

namespace GFT_test_UBS.Application.UseCases;

public sealed class ClassifyPortfolioUseCase
{
    private readonly PortfolioInputParser _inputParser;
    private readonly PortfolioInputStreamReader _inputStreamReader;
    private readonly TradeCategorizer _tradeCategorizer;

    public ClassifyPortfolioUseCase()
        : this(
            new PortfolioInputParser(),
            new PortfolioInputStreamReader(),
            new TradeCategorizer(new ITradeCategoryRule[]
            {
                new ExpiredTradeCategoryRule(),
                new HighRiskTradeCategoryRule(),
                new MediumRiskTradeCategoryRule(),
            }))
    {
    }

    public ClassifyPortfolioUseCase(
        PortfolioInputParser inputParser,
        PortfolioInputStreamReader inputStreamReader,
        TradeCategorizer tradeCategorizer)
    {
        _inputParser = inputParser;
        _inputStreamReader = inputStreamReader;
        _tradeCategorizer = tradeCategorizer;
    }

    public ClassifyPortfolioUseCase(PortfolioInputParser inputParser, TradeCategorizer tradeCategorizer)
        : this(inputParser, new PortfolioInputStreamReader(), tradeCategorizer)
    {
    }

    public IReadOnlyCollection<string> Execute(IEnumerable<string> inputLines)
    {
        var input = _inputParser.Parse(inputLines);

        return _tradeCategorizer.CategorizeMany(input.Trades, input.ReferenceDate);
    }

    public async Task ExecuteAsync(
        TextReader inputReader,
        TextWriter outputWriter,
        CancellationToken cancellationToken = default)
    {
        var input = await _inputStreamReader.ReadAsync(inputReader, cancellationToken);

        await foreach (var trade in input.Trades.WithCancellation(cancellationToken))
        {
            var category = _tradeCategorizer.Categorize(trade, input.ReferenceDate);

            await outputWriter.WriteLineAsync(category.AsMemory(), cancellationToken);
        }
    }
}
