using System.Globalization;
using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Domain.Entities;
using GFT_test_UBS.Domain.Exceptions;
using GFT_test_UBS.Domain.ValueObjects;

namespace GFT_test_UBS.Application.Input;

public sealed class PortfolioInputParser
{
    private const string DateFormat = "MM/dd/yyyy";
    private readonly PortfolioInputLinesValidator _validator;

    public PortfolioInputParser()
        : this(new PortfolioInputLinesValidator())
    {
    }

    public PortfolioInputParser(PortfolioInputLinesValidator validator)
    {
        _validator = validator;
    }

    public PortfolioInput Parse(IEnumerable<string> inputLines)
    {
        var validationResult = _validator.Validate(new PortfolioInputLines(inputLines));

        if (!validationResult.IsValid)
        {
            throw new InputValidationException(validationResult.Errors[0].ErrorMessage);
        }

        var lines = inputLines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        var referenceDate = ParseDate(lines[0]);
        var tradeCount = int.Parse(lines[1], CultureInfo.InvariantCulture);
        var tradeLines = lines.Skip(2).ToArray();

        var trades = tradeLines
            .Take(tradeCount)
            .Select(ParseTrade)
            .ToArray();

        return new PortfolioInput(referenceDate, trades);
    }

    private static ITrade ParseTrade(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            var value = decimal.Parse(parts[0], CultureInfo.InvariantCulture);
            var clientSector = Enum.Parse<ClientSector>(parts[1], ignoreCase: true);
            var nextPaymentDate = ParseDate(parts[2]);

            return new Trade(value, clientSector, nextPaymentDate);
        }
        catch (DomainException exception)
        {
            throw new InputValidationException(exception.Message);
        }
    }

    private static DateTime ParseDate(string value)
    {
        return DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
    }
}
