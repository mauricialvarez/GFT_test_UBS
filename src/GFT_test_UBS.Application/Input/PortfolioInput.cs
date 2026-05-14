using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Input;

public sealed record PortfolioInput(DateTime ReferenceDate, IReadOnlyCollection<ITrade> Trades);
