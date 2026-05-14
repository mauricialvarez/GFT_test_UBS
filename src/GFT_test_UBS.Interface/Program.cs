using System.Globalization;
using GFT_test_UBS.Application.Categorization;
using GFT_test_UBS.Domain.Entities;

var referenceDate = ParseDate(Console.ReadLine());
var tradeCount = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);
var trades = new List<ITrade>();

for (var i = 0; i < tradeCount; i++)
{
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    trades.Add(ParseTrade(line));
}

var categorizer = new TradeCategorizer(new ITradeCategoryRule[]
{
    new ExpiredTradeCategoryRule(),
    new HighRiskTradeCategoryRule(),
    new MediumRiskTradeCategoryRule(),
});

foreach (var category in categorizer.CategorizeMany(trades, referenceDate))
{
    Console.WriteLine(category);
}

static ITrade ParseTrade(string line)
{
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var value = double.Parse(parts[0], CultureInfo.InvariantCulture);
    var clientSector = parts[1];
    var nextPaymentDate = ParseDate(parts[2]);

    return new Trade(value, clientSector, nextPaymentDate);
}

static DateTime ParseDate(string? value)
{
    return DateTime.ParseExact(value ?? string.Empty, "MM/dd/yyyy", CultureInfo.InvariantCulture);
}
