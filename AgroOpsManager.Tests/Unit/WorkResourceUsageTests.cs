using AgroOpsManager.Core.Entities;
using FluentAssertions;

namespace AgroOpsManager.Tests.Unit;

public class WorkResourceUsageTests
{
    [Fact]
    public void TotalCost_ShouldReturnQuantityUsedMultipliedByUnitPriceAtUsage()
    {
        // Arrange
        var usage = new WorkResourceUsage
        {
            QuantityUsed = 400,
            UnitPriceAtUsage = 2.10m
        };

        // Act
        var result = usage.TotalCost;

        // Assert
        result.Should().Be(840m);
    }

    [Fact]
    public void TotalCost_ShouldReturnZero_WhenQuantityUsedIsZero()
    {
        // Arrange
        var usage = new WorkResourceUsage
        {
            QuantityUsed = 0,
            UnitPriceAtUsage = 2.10m
        };

        // Act
        var result = usage.TotalCost;

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void TotalCost_ShouldReturnZero_WhenUnitPriceAtUsageIsZero()
    {
        // Arrange
        var usage = new WorkResourceUsage
        {
            QuantityUsed = 100,
            UnitPriceAtUsage = 0
        };

        // Act
        var result = usage.TotalCost;

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void TotalCost_ShouldHandleDecimalValues()
    {
        // Arrange
        var usage = new WorkResourceUsage
        {
            QuantityUsed = 12.5m,
            UnitPriceAtUsage = 3.20m
        };

        // Act
        var result = usage.TotalCost;

        // Assert
        result.Should().Be(40.00m);
    }
}