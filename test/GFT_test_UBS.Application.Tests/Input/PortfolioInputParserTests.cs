using FluentAssertions;
using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Application.Input;
using GFT_test_UBS.Domain.ValueObjects;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Input;

public sealed class PortfolioInputParserTests
{
    [Fact]
    public void Parse_should_create_portfolio_input_from_valid_lines()
    {
        // Arrange
        var parser = new PortfolioInputParser();
        var lines = new[]
        {
            "12/11/2020",
            "2",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
        };

        // Act
        var input = parser.Parse(lines);

        // Assert
        input.ReferenceDate.Should().Be(new DateTime(2020, 12, 11));
        input.Trades.Should().HaveCount(2);
        input.Trades.First().Value.Should().Be(2_000_000m);
        input.Trades.First().ClientSector.Should().Be(ClientSector.Private);
        input.Trades.Last().ClientSector.Should().Be(ClientSector.Public);
    }

    [Fact]
    public void Parse_should_ignore_blank_lines()
    {
        // Arrange
        var parser = new PortfolioInputParser();
        var lines = new[]
        {
            "",
            "12/11/2020",
            "",
            "1",
            "",
            "2000000 Private 12/29/2025",
            "",
        };

        // Act
        var input = parser.Parse(lines);

        // Assert
        input.Trades.Should().ContainSingle();
    }

    [Theory]
    [InlineData("31/12/2020", "1", "2000000 Private 12/29/2025", "data invalida. Use o formato MM/dd/yyyy.")]
    [InlineData("12/11/2020", "abc", "2000000 Private 12/29/2025", "quantidade de operacoes invalida.")]
    [InlineData("12/11/2020", "1", "invalid Private 12/29/2025", "valor da operacao invalido.")]
    [InlineData("12/11/2020", "1", "2000000 Corporate 12/29/2025", "setor do cliente invalido.")]
    [InlineData("12/11/2020", "1", "2000000 Private 29/12/2025", "data invalida. Use o formato MM/dd/yyyy.")]
    [InlineData("12/11/2020", "1", "-1 Private 12/29/2025", "O valor da operacao nao pode ser negativo.")]
    public void Parse_should_throw_input_validation_exception_when_input_is_invalid(
        string referenceDate,
        string tradeCount,
        string tradeLine,
        string expectedMessage)
    {
        // Arrange
        var parser = new PortfolioInputParser();
        var lines = new[] { referenceDate, tradeCount, tradeLine };

        // Act
        var act = () => parser.Parse(lines);

        // Assert
        act.Should()
            .Throw<InputValidationException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void Parse_should_throw_input_validation_exception_when_declared_count_is_lower_than_trade_lines()
    {
        // Arrange
        var parser = new PortfolioInputParser();
        var lines = new[]
        {
            "12/11/2020",
            "1",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
        };

        // Act
        var act = () => parser.Parse(lines);

        // Assert
        act.Should()
            .Throw<InputValidationException>()
            .WithMessage("existem mais operacoes para processar do que o informado.");
    }

    [Fact]
    public void Parse_should_throw_input_validation_exception_when_declared_count_is_higher_than_trade_lines()
    {
        // Arrange
        var parser = new PortfolioInputParser();
        var lines = new[]
        {
            "12/11/2020",
            "2",
            "2000000 Private 12/29/2025",
        };

        // Act
        var act = () => parser.Parse(lines);

        // Assert
        act.Should()
            .Throw<InputValidationException>()
            .WithMessage("existem menos operacoes para processar do que o informado.");
    }
}
