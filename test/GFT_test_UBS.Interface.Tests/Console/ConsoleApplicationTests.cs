using System.Diagnostics;
using Xunit;

namespace GFT_test_UBS.Interface.Tests.Console;

public sealed class ConsoleApplicationTests
{
    [Fact]
    public async Task Console_should_classify_trades_from_standard_input()
    {
        var input = string.Join(Environment.NewLine, new[]
        {
            "12/11/2020",
            "4",
            "2000000 Private 12/29/2025",
            "400000 Public 07/01/2020",
            "5000000 Public 01/02/2024",
            "3000000 Public 10/26/2023",
        });

        var result = await RunConsoleAsync(input);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(
            string.Join(Environment.NewLine, new[] { "HIGHRISK", "EXPIRED", "MEDIUMRISK", "MEDIUMRISK" }),
            NormalizeLineEndings(result.StandardOutput).Trim());
        Assert.True(string.IsNullOrWhiteSpace(result.StandardError));
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

        Assert.NotNull(process);

        await process.StandardInput.WriteAsync(input);
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
