using FluentAssertions;
using GFT_test_UBS.Application.Exceptions;
using GFT_test_UBS.Application.UseCases;
using Xunit;

namespace GFT_test_UBS.Application.Tests.UseCases;

public sealed class ClassifyPortfolioUseCaseTests
{
    [Fact]
    public void Execute_should_classify_trades_from_input_lines()
    {
        // Arrange
        var input = new[]
        {
            "12/11/2020",
            "4",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
            "5000000 Public 01/02/2024",
            "3000000 Public 10/26/2023",
        };
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        var categories = useCase.Execute(input);

        // Assert
        categories.Should().Equal("HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK");
    }

    [Fact]
    public void Execute_should_ignore_blank_lines_before_between_and_after_input()
    {
        // Arrange
        var input = new[]
        {
            "",
            "12/11/2020",
            "",
            "2",
            "",
            "2000000 Private 12/29/2025",
            "",
            "400000 Public 07/01/2020",
            "",
        };
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        var categories = useCase.Execute(input);

        // Assert
        categories.Should().Equal("HIGHRISK", "EXPIRED");
    }

    [Fact]
    public void Execute_should_return_error_when_input_contains_more_trades_than_declared()
    {
        // Arrange
        var input = new[]
        {
            "12/11/2020",
            "2",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
            "5000000 Public 01/02/2024",
        };
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        var act = () => useCase.Execute(input);

        // Assert
        act.Should()
            .Throw<InputValidationException>()
            .WithMessage("linha 5: existem mais operacoes para processar do que o informado.");
    }

    [Fact]
    public async Task ExecuteAsync_should_classify_trades_from_input_stream()
    {
        // Arrange
        using var input = new StringReader(string.Join(Environment.NewLine, new[]
        {
            "12/11/2020",
            "4",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
            "5000000 Public 01/02/2024",
            "3000000 Public 10/26/2023",
        }));
        using var output = new StringWriter();
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        await useCase.ExecuteAsync(input, output);

        // Assert
        NormalizeLineEndings(output.ToString())
            .Trim()
            .Should()
            .Be(string.Join(Environment.NewLine, new[] { "HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK" }));
    }

    [Fact]
    public async Task ExecuteAsync_should_ignore_blank_lines_before_between_and_after_input()
    {
        // Arrange
        using var input = new StringReader(string.Join(Environment.NewLine, new[]
        {
            "",
            "12/11/2020",
            "",
            "2",
            "",
            "2000000 Private 12/29/2025",
            "",
            "400000 Public 07/01/2020",
            "",
        }));
        using var output = new StringWriter();
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        await useCase.ExecuteAsync(input, output);

        // Assert
        NormalizeLineEndings(output.ToString())
            .Trim()
            .Should()
            .Be(string.Join(Environment.NewLine, new[] { "HIGHRISK", "EXPIRED" }));
    }

    [Fact]
    public async Task ExecuteAsync_should_return_error_when_input_contains_more_trades_than_declared()
    {
        // Arrange
        using var input = new StringReader(string.Join(Environment.NewLine, new[]
        {
            "",
            "12/11/2020",
            "",
            "2",
            "",
            "2000000 Private 12/29/2025",
            "",
            "400000 Public 07/01/2020",
            "",
            "5000000 Public 01/02/2024",
        }));
        using var output = new StringWriter();
        var useCase = new ClassifyPortfolioUseCase();

        // Act
        var act = () => useCase.ExecuteAsync(input, output);

        // Assert
        await act.Should()
            .ThrowAsync<InputValidationException>()
            .WithMessage("linha 10: existem mais operacoes para processar do que o informado.");
    }

    private static string NormalizeLineEndings(string value)
    {
        return value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    }
}
