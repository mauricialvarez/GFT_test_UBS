using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GFT_test_UBS.Application.UseCases;

BenchmarkRunner.Run<ClassifyPortfolioUseCaseBenchmarks>();

[MemoryDiagnoser]
[ShortRunJob]
public class ClassifyPortfolioUseCaseBenchmarks
{
    private readonly ClassifyPortfolioUseCase _useCase = new();
    private string[] _inputLines = [];

    [Params(1_000, 10_000, 100_000, 1_000_000, 10_000_000)]
    public int TradeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var dataFilePath = FindDataFile($"input_{TradeCount}.txt");

        _inputLines = File.ReadAllLines(dataFilePath);
    }

    [Benchmark]
    public IReadOnlyCollection<string> ClassifyPortfolio()
    {
        return _useCase.Execute(_inputLines);
    }

    private static string FindDataFile(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "benchmarks", "data", fileName);

            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Benchmark data file '{fileName}' was not found.");
    }
}
