using FluentAssertions;
using GFT_test_UBS.Application.Input;
using Xunit;

namespace GFT_test_UBS.Application.Tests.Input;

public sealed class PortfolioInputLinesValidatorTests
{
    [Fact]
    public void Validate_should_accept_valid_input_lines()
    {
        // Arrange
        var validator = new PortfolioInputLinesValidator();
        var input = new PortfolioInputLines(new[]
        {
            "12/11/2020",
            "2",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
        });

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("31/12/2020", "1", "2000000 Private 12/29/2025", "linha 1: data invalida. Use o formato MM/dd/yyyy.")]
    [InlineData("12/11/2020", "abc", "2000000 Private 12/29/2025", "linha 2: quantidade de operacoes invalida.")]
    [InlineData("12/11/2020", "1", "invalid Private 12/29/2025", "linha 3: valor da operacao invalido.")]
    [InlineData("12/11/2020", "1", "2000000 Corporate 12/29/2025", "linha 3: setor do cliente invalido.")]
    [InlineData("12/11/2020", "1", "2000000 Private 29/12/2025", "linha 3: data invalida. Use o formato MM/dd/yyyy.")]
    [InlineData("12/11/2020", "1", "-1 Private 12/29/2025", "linha 3: O valor da operacao nao pode ser negativo.")]
    public void Validate_should_return_expected_message_when_input_is_invalid(
        string referenceDate,
        string tradeCount,
        string tradeLine,
        string expectedMessage)
    {
        // Arrange
        var validator = new PortfolioInputLinesValidator();
        var input = new PortfolioInputLines(new[] { referenceDate, tradeCount, tradeLine });

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be(expectedMessage);
    }
}
