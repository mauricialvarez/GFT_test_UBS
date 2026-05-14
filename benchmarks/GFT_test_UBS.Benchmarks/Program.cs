using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GFT_test_UBS.Application.UseCases;

BenchmarkRunner.Run<ClassifyPortfolioUseCaseBenchmarks>();

[MemoryDiagnoser]
[ShortRunJob]
public class ClassifyPortfolioUseCaseBenchmarks
{
    private readonly ClassifyPortfolioUseCase _useCase = new();
    private string _dataFilePath = string.Empty;

    [Params(1_000, 10_000, 100_000, 1_000_000, 10_000_000)]
    public int TradeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _dataFilePath = FindDataFile($"input_{TradeCount}.txt");
    }

    [Benchmark(Baseline = true)]
    public void Buffered()
    {
        var inputLines = File.ReadAllLines(_dataFilePath);
        var categories = _useCase.Execute(inputLines);

        foreach (var category in categories)
        {
            TextWriter.Null.WriteLine(category);
        }
    }

    [Benchmark]
    public async Task Streaming()
    {
        using var reader = File.OpenText(_dataFilePath);

        await _useCase.ExecuteAsync(reader, TextWriter.Null);
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
