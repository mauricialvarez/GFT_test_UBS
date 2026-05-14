using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Application.UseCases;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "{Message:lj}{NewLine}",
        standardErrorFromLevel: LogEventLevel.Error)
    .CreateLogger();

try
{
    var inputLines = ReadInputLines();
    var useCase = new ClassifyPortfolioUseCase();

    foreach (var category in useCase.Execute(inputLines))
    {
        Log.Information("{Category}", category);
    }

    return 0;
}
catch (Exception exception)
{
    Log.Error("Erro: {Message}", GetUserMessage(exception));

    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static string[] ReadInputLines()
{
    var lines = new List<string>();
    string? line;

    while ((line = Console.ReadLine()) is not null)
    {
        lines.Add(line);
    }

    return lines.ToArray();
}

static string GetUserMessage(Exception exception)
{
    return exception switch
    {
        InputValidationException => exception.Message,
        _ => "erro inesperado ao processar a entrada.",
    };
}
