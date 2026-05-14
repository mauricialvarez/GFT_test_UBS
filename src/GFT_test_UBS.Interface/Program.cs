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
    var useCase = new ClassifyPortfolioUseCase();
    using var outputWriter = new StreamWriter(Console.OpenStandardOutput(), bufferSize: 1 << 20);

    await useCase.ExecuteAsync(Console.In, outputWriter);
    await outputWriter.FlushAsync();

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

static string GetUserMessage(Exception exception)
{
    return exception switch
    {
        InputValidationException => exception.Message,
        _ => "erro inesperado ao processar a entrada.",
    };
}
