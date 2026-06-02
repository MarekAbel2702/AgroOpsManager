using AgroOpsManager.Core.Entities;
using FluentAssertions;

namespace AgroOpsManager.Tests.Unit;

public class MachineTests
{
    [Fact]
    public void RequiresService_ShouldReturnTrue_WhenHoursSinceLastServiceEqualsServiceInterval()
    {
        // Arrange
        var machine = new Machine
        {
            CurrentWorkingHours = 1000,
            WorkingHoursAtLastService = 750,
            ServiceIntervalHours = 250
        };

        // Act
        var result = machine.RequiresService();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void RequiresService_ShouldReturnTrue_WhenHoursSinceLastServiceExceedsServiceInterval()
    {
        // Arrange
        var machine = new Machine
        {
            CurrentWorkingHours = 1100,
            WorkingHoursAtLastService = 750,
            ServiceIntervalHours = 250
        };

        // Act
        var result = machine.RequiresService();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void RequiresService_ShouldReturnFalse_WhenHoursSinceLastServiceIsBelowServiceInterval()
    {
        // Arrange
        var machine = new Machine
        {
            CurrentWorkingHours = 900,
            WorkingHoursAtLastService = 750,
            ServiceIntervalHours = 250
        };

        // Act
        var result = machine.RequiresService();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RequiresService_ShouldReturnFalse_WhenMachineWasJustServiced()
    {
        // Arrange
        var machine = new Machine
        {
            CurrentWorkingHours = 1000,
            WorkingHoursAtLastService = 1000,
            ServiceIntervalHours = 250
        };

        // Act
        var result = machine.RequiresService();

        // Assert
        result.Should().BeFalse();
    }
}