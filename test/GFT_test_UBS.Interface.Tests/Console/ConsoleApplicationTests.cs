using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace GFT_test_UBS.Interface.Tests.Console;

public sealed class ConsoleApplicationTests
{
    [Fact]
    public async Task Console_should_classify_trades_from_standard_input()
    {
        // Arrange
        var input = string.Join(Environment.NewLine, new[]
        {
            "12/11/2020",
            "4",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
            "5000000 Public 01/02/2024",
            "3000000 Public 10/26/2023",
        });

        // Act
        var result = await RunConsoleAsync(input);

        // Assert
        result.ExitCode.Should().Be(0);
        NormalizeLineEndings(result.StandardOutput)
            .Trim()
            .Should()
            .Be(string.Join(Environment.NewLine, new[] { "HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK" }));
        result.StandardError.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Console_should_return_error_when_reference_date_is_invalid()
    {
        // Arrange
        var input = string.Join(Environment.NewLine, new[]
        {
            "31/12/2020",
            "1",
            "2000000 Private 12/29/2025",
        });

        // Act
        var result = await RunConsoleAsync(input);

        // Assert
        result.ExitCode.Should().NotBe(0);
        result.StandardError.Should().Contain("Erro: linha 1: data invalida. Use o formato MM/dd/yyyy.");
    }

    [Fact]
    public async Task Console_should_return_error_when_next_payment_date_is_invalid()
    {
        // Arrange
        var input = string.Join(Environment.NewLine, new[]
        {
            "12/11/2020",
            "1",
            "2000000 Private 29/12/2025",
        });

        // Act
        var result = await RunConsoleAsync(input);

        // Assert
        result.ExitCode.Should().NotBe(0);
        result.StandardError.Should().Contain("Erro: linha 3: data invalida. Use o formato MM/dd/yyyy.");
    }

    [Fact]
    public async Task Console_should_ignore_blank_lines_before_between_and_after_input()
    {
        // Arrange
        var input = string.Join(Environment.NewLine, new[]
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
        });

        // Act
        var result = await RunConsoleAsync(input);

        // Assert
        result.ExitCode.Should().Be(0);
        NormalizeLineEndings(result.StandardOutput)
            .Trim()
            .Should()
            .Be(string.Join(Environment.NewLine, new[] { "HIGHRISK", "EXPIRED" }));
        result.StandardError.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Console_should_return_error_in_portuguese_when_input_contains_more_trades_than_declared()
    {
        // Arrange
        var input = string.Join(Environment.NewLine, new[]
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
            "",
        });

        // Act
        var result = await RunConsoleAsync(input);

        // Assert
        result.ExitCode.Should().NotBe(0);
        result.StandardError.Should().Contain("Erro: linha 10: existem mais operacoes para processar do que o informado.");
    }

    private static async Task<ConsoleResult> RunConsoleAsync(string input)
    {
        var applicationPath = Path.Combine(AppContext.BaseDirectory, "GFT_test_UBS.Interface.dll");
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{applicationPath}\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        });

        process.Should().NotBeNull();

        await process!.StandardInput.WriteAsync(input);
        process.StandardInput.Close();

        var standardOutput = await process.StandardOutput.ReadToEndAsync();
        var standardError = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new ConsoleResult(process.ExitCode, standardOutput, standardError);
    }

    private static string NormalizeLineEndings(string value)
    {
        return value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    }

    private sealed record ConsoleResult(int ExitCode, string StandardOutput, string StandardError);
}
