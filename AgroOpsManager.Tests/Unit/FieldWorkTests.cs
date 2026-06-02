using AgroOpsManager.Core.Entities;
using FluentAssertions;

namespace AgroOpsManager.Tests.Unit;

public class FieldWorkTests
{
    [Fact]
    public void CalculateResourcesCost_ShouldReturnZero_WhenThereAreNoResourceUsages()
    {
        // Arrange
        var fieldWork = new FieldWork();

        // Act
        var result = fieldWork.CalculateResourcesCost();

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateResourcesCost_ShouldReturnSumOfResourceUsageCosts()
    {
        // Arrange
        var fieldWork = new FieldWork
        {
            ResourceUsages = new List<WorkResourceUsage>
            {
                new WorkResourceUsage
                {
                    QuantityUsed = 100,
                    UnitPriceAtUsage = 2.50m
                },
                new WorkResourceUsage
                {
                    QuantityUsed = 50,
                    UnitPriceAtUsage = 4.00m
                }
            }
        };

        // Act
        var result = fieldWork.CalculateResourcesCost();

        // Assert
        result.Should().Be(450m);
    }

    [Fact]
    public void CalculateTotalCost_ShouldReturnLaborCost_WhenThereAreNoResourceUsages()
    {
        // Arrange
        var fieldWork = new FieldWork
        {
            LaborCost = 350m
        };

        // Act
        var result = fieldWork.CalculateTotalCost();

        // Assert
        result.Should().Be(350m);
    }

    [Fact]
    public void CalculateTotalCost_ShouldReturnLaborCostPlusResourcesCost()
    {
        // Arrange
        var fieldWork = new FieldWork
        {
            LaborCost = 350m,
            ResourceUsages = new List<WorkResourceUsage>
            {
                new WorkResourceUsage
                {
                    QuantityUsed = 100,
                    UnitPriceAtUsage = 2.50m
                },
                new WorkResourceUsage
                {
                    QuantityUsed = 50,
                    UnitPriceAtUsage = 4.00m
                }
            }
        };

        // Act
        var result = fieldWork.CalculateTotalCost();

        // Assert
        result.Should().Be(800m);
    }

    [Fact]
    public void CalculateTotalCost_ShouldHandleDecimalValues()
    {
        // Arrange
        var fieldWork = new FieldWork
        {
            LaborCost = 99.99m,
            ResourceUsages = new List<WorkResourceUsage>
            {
                new WorkResourceUsage
                {
                    QuantityUsed = 12.5m,
                    UnitPriceAtUsage = 3.20m
                }
            }
        };

        // Act
        var result = fieldWork.CalculateTotalCost();

        // Assert
        result.Should().Be(139.99m);
    }
}