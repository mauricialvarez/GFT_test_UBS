using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Application.UseCases;
using Serilog;
using Serilog.Events;

var tracingId = GetTracingId();
var inputFile = Environment.GetEnvironmentVariable("INPUT_FILE") ?? "stdin";
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.WithProperty("TracingId", tracingId)
    .Enrich.WithProperty("InputFile", inputFile)
    .WriteTo.Console(
        outputTemplate: "{Message:lj}{NewLine}",
        restrictedToMinimumLevel: LogEventLevel.Error,
        standardErrorFromLevel: LogEventLevel.Error);

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");

if (!string.IsNullOrWhiteSpace(seqUrl))
{
    loggerConfiguration = loggerConfiguration.WriteTo.Seq(seqUrl);
}

Log.Logger = loggerConfiguration.CreateLogger();

try
{
    Log.Information(
        "Inicio da execucao. TracingId={TracingId}; Arquivo={InputFile}",
        tracingId,
        inputFile);

    var useCase = new ClassifyPortfolioUseCase();
    using var outputWriter = new StreamWriter(Console.OpenStandardOutput(), bufferSize: 1 << 20);

    await useCase.ExecuteAsync(Console.In, outputWriter);
    await outputWriter.FlushAsync();

    Log.Information(
        "Arquivo processado com sucesso. TracingId={TracingId}; Arquivo={InputFile}",
        tracingId,
        inputFile);
    Log.Information(
        "Fim da execucao. TracingId={TracingId}; Arquivo={InputFile}; Status=Success",
        tracingId,
        inputFile);

    return 0;
}
catch (Exception exception)
{
    var userMessage = GetUserMessage(exception);

    Log.Warning(
        exception,
        "Falha ao processar arquivo. TracingId={TracingId}; Arquivo={InputFile}; Erro={Message}",
        tracingId,
        inputFile,
        userMessage);
    Log.Error("Erro: {Message}", userMessage);
    Log.Information(
        "Fim da execucao. TracingId={TracingId}; Arquivo={InputFile}; Status=Failure",
        tracingId,
        inputFile);

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

static string GetTracingId()
{
    var tracingId = Environment.GetEnvironmentVariable("TRACING_ID");

    return string.IsNullOrWhiteSpace(tracingId)
        ? Guid.NewGuid().ToString("N")
        : tracingId;
}
