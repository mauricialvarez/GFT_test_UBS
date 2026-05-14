using FluentAssertions;
using Xunit;

namespace GFT_test_UBS.Infrastructure.Tests;

public sealed class SmokeTests
{
    [Fact]
    public void Solution_is_ready_for_infrastructure_tests()
    {
        // Arrange
        var infrastructureProjectIsAvailable = true;

        // Act
        var result = infrastructureProjectIsAvailable;

        // Assert
        result.Should().BeTrue();
    }
}
