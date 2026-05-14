using System.Globalization;
using System.Runtime.CompilerServices;
using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.Exceptions;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Application.Input;

public sealed class PortfolioInputStreamReader
{
    private const string DateFormat = "MM/dd/yyyy";

    public async Task<PortfolioInputStream> ReadAsync(
        TextReader reader,
        CancellationToken cancellationToken = default)
    {
        var referenceDateLine = await ReadNextNonBlankLineAsync(reader, 0, cancellationToken);

        if (referenceDateLine is null)
        {
            throw new InputValidationException("entrada incompleta.");
        }

        var tradeCountLine = await ReadNextNonBlankLineAsync(reader, referenceDateLine.LineNumber, cancellationToken);

        if (tradeCountLine is null)
        {
            throw new InputValidationException("entrada incompleta.");
        }

        var referenceDate = ParseDate(referenceDateLine.Value, referenceDateLine.LineNumber);

        if (!int.TryParse(
            tradeCountLine.Value,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var tradeCount))
        {
            throw new InputValidationException(
                WithLine(tradeCountLine.LineNumber, "quantidade de operacoes invalida."));
        }

        return new PortfolioInputStream(
            referenceDate,
            ReadTradesAsync(reader, tradeCount, tradeCountLine.LineNumber, cancellationToken));
    }

    private static async IAsyncEnumerable<ITrade> ReadTradesAsync(
        TextReader reader,
        int tradeCount,
        int lastReadLineNumber,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var currentLineNumber = lastReadLineNumber;

        for (var index = 0; index < tradeCount; index++)
        {
            var tradeLine = await ReadNextNonBlankLineAsync(reader, currentLineNumber, cancellationToken);

            if (tradeLine is null)
            {
                throw new InputValidationException("existem menos operacoes para processar do que o informado.");
            }

            currentLineNumber = tradeLine.LineNumber;

            yield return ParseTrade(tradeLine.Value, tradeLine.LineNumber);
        }

        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            currentLineNumber++;

            if (!string.IsNullOrWhiteSpace(line))
            {
                throw new InputValidationException(
                    WithLine(currentLineNumber, "existem mais operacoes para processar do que o informado."));
            }
        }
    }

    private static async Task<InputLine?> ReadNextNonBlankLineAsync(
        TextReader reader,
        int lastReadLineNumber,
        CancellationToken cancellationToken)
    {
        var lineNumber = lastReadLineNumber;
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            lineNumber++;

            if (!string.IsNullOrWhiteSpace(line))
            {
                return new InputLine(lineNumber, line);
            }
        }

        return null;
    }

    private static ITrade ParseTrade(string line, int lineNumber)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            throw new InputValidationException(WithLine(lineNumber, "operacao invalida."));
        }

        if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var tradeValue))
        {
            throw new InputValidationException(WithLine(lineNumber, "valor da operacao invalido."));
        }

        if (tradeValue < 0)
        {
            throw new InputValidationException(
                WithLine(lineNumber, "O valor da operacao nao pode ser negativo."));
        }

        if (!Enum.TryParse<ClientSector>(parts[1], ignoreCase: true, out var clientSector)
            || !Enum.IsDefined(clientSector))
        {
            throw new InputValidationException(WithLine(lineNumber, "setor do cliente invalido."));
        }

        var nextPaymentDate = ParseDate(parts[2], lineNumber);

        try
        {
            return new Trade(tradeValue, clientSector, nextPaymentDate);
        }
        catch (DomainException exception)
        {
            throw new InputValidationException(exception.Message);
        }
    }

    private static DateTime ParseDate(string value, int lineNumber)
    {
        if (!DateTime.TryParseExact(
            value,
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date))
        {
            throw new InputValidationException(WithLine(lineNumber, "data invalida. Use o formato MM/dd/yyyy."));
        }

        return date;
    }

    private static string WithLine(int lineNumber, string message)
    {
        return $"linha {lineNumber}: {message}";
    }

    private sealed record InputLine(int LineNumber, string Value);
}
