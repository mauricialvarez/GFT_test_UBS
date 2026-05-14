using GFT_test_UBS.Domain.Entities;

namespace GFT_test_UBS.Application.Input;

public sealed record PortfolioInputStream(DateTime ReferenceDate, IAsyncEnumerable<ITrade> Trades);
