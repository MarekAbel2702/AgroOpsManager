using AgroOpsManager.Core.Entities;
using FluentAssertions;

namespace AgroOpsManager.Tests.Unit;

public class InventoryItemTests
{
    [Fact]
    public void IsLowStock_ShouldReturnTrue_WhenQuantityIsBelowMinimumQuantity()
    {
        // Arrange
        var item = new InventoryItem
        {
            Quantity = 50,
            MinimumQuantity = 100
        };

        // Act
        var result = item.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_ShouldReturnTrue_WhenQuantityEqualsMinimumQuantity()
    {
        // Arrange
        var item = new InventoryItem
        {
            Quantity = 100,
            MinimumQuantity = 100
        };

        // Act
        var result = item.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_ShouldReturnFalse_WhenQuantityIsAboveMinimumQuantity()
    {
        // Arrange
        var item = new InventoryItem
        {
            Quantity = 150,
            MinimumQuantity = 100
        };

        // Act
        var result = item.IsLowStock();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsLowStock_ShouldReturnTrue_WhenBothQuantityAndMinimumQuantityAreZero()
    {
        // Arrange
        var item = new InventoryItem
        {
            Quantity = 0,
            MinimumQuantity = 0
        };

        // Act
        var result = item.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }
}