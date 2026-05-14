using System.Globalization;
using FluentValidation;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Application.Input;

public sealed class PortfolioInputLinesValidator : AbstractValidator<PortfolioInputLines>
{
    private const string DateFormat = "MM/dd/yyyy";

    public PortfolioInputLinesValidator()
    {
        RuleFor(input => input.Lines)
            .NotNull()
            .WithMessage("entrada incompleta.");

        RuleFor(input => input)
            .Custom(ValidateInput);
    }

    private static void ValidateInput(PortfolioInputLines input, ValidationContext<PortfolioInputLines> context)
    {
        var lines = Normalize(input.Lines);

        if (lines.Length < 2)
        {
            context.AddFailure("entrada incompleta.");
            return;
        }

        ValidateDate(lines[0], context);

        if (!int.TryParse(lines[1], NumberStyles.None, CultureInfo.InvariantCulture, out var tradeCount))
        {
            context.AddFailure("quantidade de operacoes invalida.");
            return;
        }

        var tradeLines = lines.Skip(2).ToArray();

        if (tradeLines.Length > tradeCount)
        {
            context.AddFailure("existem mais operacoes para processar do que o informado.");
            return;
        }

        if (tradeLines.Length < tradeCount)
        {
            context.AddFailure("existem menos operacoes para processar do que o informado.");
            return;
        }

        foreach (var tradeLine in tradeLines)
        {
            if (!ValidateTradeLine(tradeLine, context))
            {
                return;
            }
        }
    }

    private static bool ValidateTradeLine(string tradeLine, ValidationContext<PortfolioInputLines> context)
    {
        var parts = tradeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            context.AddFailure("operacao invalida.");
            return false;
        }

        if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var tradeValue))
        {
            context.AddFailure("valor da operacao invalido.");
            return false;
        }

        if (tradeValue < 0)
        {
            context.AddFailure("O valor da operacao nao pode ser negativo.");
            return false;
        }

        if (!Enum.TryParse<ClientSector>(parts[1], ignoreCase: true, out var clientSector)
            || !Enum.IsDefined(clientSector))
        {
            context.AddFailure("setor do cliente invalido.");
            return false;
        }

        return ValidateDate(parts[2], context);
    }

    private static bool ValidateDate(string value, ValidationContext<PortfolioInputLines> context)
    {
        if (!DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            context.AddFailure("data invalida. Use o formato MM/dd/yyyy.");
            return false;
        }

        return true;
    }

    private static string[] Normalize(IEnumerable<string>? inputLines)
    {
        return (inputLines ?? Array.Empty<string>())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }
}
