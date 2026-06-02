using AgroOpsManager.Core.DTOs;
using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Core.Exceptions;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Tests.Unit;

public class FieldWorkCompletionServiceTests
{
    [Fact]
    public async Task CompleteAsync_ShouldCompleteFieldWork_AndDecreaseInventoryQuantity()
    {
        // Arrange
        await using var context = CreateDbContext();

        var field = new Field
        {
            Name = "Pole Testowe",
            Location = "Sektor T",
            AreaInHectares = 10,
            SoilType = SoilType.Loamy,
            CurrentCrop = CropType.Wheat,
            Status = FieldStatus.Active
        };

        var inventoryItem = new InventoryItem
        {
            Name = "Saletra amonowa",
            Category = InventoryCategory.Fertilizer,
            Unit = "kg",
            Quantity = 1000,
            MinimumQuantity = 200,
            UnitPrice = 2.50m
        };

        var fieldWork = new FieldWork
        {
            Field = field,
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned,
            LaborCost = 300
        };

        context.Fields.Add(field);
        context.InventoryItems.Add(inventoryItem);
        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            CompletionNotes = "Praca zakończona testowo.",
            Resources = new List<ResourceUsageRequest>
            {
                new ResourceUsageRequest
                {
                    InventoryItemId = inventoryItem.Id,
                    QuantityUsed = 400
                }
            }
        };

        // Act
        await service.CompleteAsync(request);

        // Assert
        var updatedFieldWork = await context.FieldWorks
            .Include(x => x.ResourceUsages)
            .FirstAsync(x => x.Id == fieldWork.Id);

        var updatedInventoryItem = await context.InventoryItems
            .FirstAsync(x => x.Id == inventoryItem.Id);

        updatedFieldWork.Status.Should().Be(FieldWorkStatus.Completed);
        updatedFieldWork.CompletedAtUtc.Should().NotBeNull();
        updatedFieldWork.Notes.Should().Be("Praca zakończona testowo.");

        updatedInventoryItem.Quantity.Should().Be(600);

        updatedFieldWork.ResourceUsages.Should().HaveCount(1);
        updatedFieldWork.ResourceUsages.First().InventoryItemId.Should().Be(inventoryItem.Id);
        updatedFieldWork.ResourceUsages.First().QuantityUsed.Should().Be(400);
        updatedFieldWork.ResourceUsages.First().UnitPriceAtUsage.Should().Be(2.50m);
    }

    [Fact]
    public async Task CompleteAsync_ShouldCreateMultipleResourceUsages_AndDecreaseMultipleInventoryItems()
    {
        // Arrange
        await using var context = CreateDbContext();

        var field = new Field
        {
            Name = "Pole Testowe",
            Location = "Sektor T",
            AreaInHectares = 12,
            SoilType = SoilType.Loamy,
            CurrentCrop = CropType.Corn,
            Status = FieldStatus.Active
        };

        var fertilizer = new InventoryItem
        {
            Name = "Nawóz NPK",
            Category = InventoryCategory.Fertilizer,
            Unit = "kg",
            Quantity = 800,
            MinimumQuantity = 100,
            UnitPrice = 3.00m
        };

        var fuel = new InventoryItem
        {
            Name = "Olej napędowy",
            Category = InventoryCategory.Fuel,
            Unit = "L",
            Quantity = 500,
            MinimumQuantity = 100,
            UnitPrice = 6.50m
        };

        var fieldWork = new FieldWork
        {
            Field = field,
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned,
            LaborCost = 250
        };

        context.Fields.Add(field);
        context.InventoryItems.AddRange(fertilizer, fuel);
        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            Resources = new List<ResourceUsageRequest>
            {
                new ResourceUsageRequest
                {
                    InventoryItemId = fertilizer.Id,
                    QuantityUsed = 300
                },
                new ResourceUsageRequest
                {
                    InventoryItemId = fuel.Id,
                    QuantityUsed = 80
                }
            }
        };

        // Act
        await service.CompleteAsync(request);

        // Assert
        var updatedFieldWork = await context.FieldWorks
            .Include(x => x.ResourceUsages)
            .FirstAsync(x => x.Id == fieldWork.Id);

        var updatedFertilizer = await context.InventoryItems.FirstAsync(x => x.Id == fertilizer.Id);
        var updatedFuel = await context.InventoryItems.FirstAsync(x => x.Id == fuel.Id);

        updatedFieldWork.Status.Should().Be(FieldWorkStatus.Completed);
        updatedFieldWork.ResourceUsages.Should().HaveCount(2);

        updatedFertilizer.Quantity.Should().Be(500);
        updatedFuel.Quantity.Should().Be(420);
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenFieldWorkDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = 999
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Praca polowa nie istnieje.");
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenFieldWorkIsAlreadyCompleted()
    {
        // Arrange
        await using var context = CreateDbContext();

        var fieldWork = new FieldWork
        {
            Field = CreateField(),
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Completed,
            CompletedAtUtc = DateTime.UtcNow
        };

        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Ta praca jest już zakończona.");
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenFieldWorkIsCancelled()
    {
        // Arrange
        await using var context = CreateDbContext();

        var fieldWork = new FieldWork
        {
            Field = CreateField(),
            Type = FieldWorkType.Spraying,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Cancelled
        };

        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Nie można zakończyć anulowanej pracy.");
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenInventoryQuantityIsNotEnough()
    {
        // Arrange
        await using var context = CreateDbContext();

        var field = CreateField();

        var inventoryItem = new InventoryItem
        {
            Name = "Saletra amonowa",
            Category = InventoryCategory.Fertilizer,
            Unit = "kg",
            Quantity = 100,
            MinimumQuantity = 20,
            UnitPrice = 2.50m
        };

        var fieldWork = new FieldWork
        {
            Field = field,
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned,
            LaborCost = 300
        };

        context.Fields.Add(field);
        context.InventoryItems.Add(inventoryItem);
        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            Resources = new List<ResourceUsageRequest>
            {
                new ResourceUsageRequest
                {
                    InventoryItemId = inventoryItem.Id,
                    QuantityUsed = 400
                }
            }
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*Brak wystarczającej ilości materiału*");

        var unchangedInventoryItem = await context.InventoryItems
            .FirstAsync(x => x.Id == inventoryItem.Id);

        var unchangedFieldWork = await context.FieldWorks
            .FirstAsync(x => x.Id == fieldWork.Id);

        unchangedInventoryItem.Quantity.Should().Be(100);
        unchangedFieldWork.Status.Should().Be(FieldWorkStatus.Planned);
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenInventoryItemDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();

        var fieldWork = new FieldWork
        {
            Field = CreateField(),
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned
        };

        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            Resources = new List<ResourceUsageRequest>
            {
                new ResourceUsageRequest
                {
                    InventoryItemId = 999,
                    QuantityUsed = 10
                }
            }
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Wybrany materiał nie istnieje.");
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowBusinessException_WhenSameInventoryItemIsUsedMoreThanOnce()
    {
        // Arrange
        await using var context = CreateDbContext();

        var inventoryItem = new InventoryItem
        {
            Name = "Saletra amonowa",
            Category = InventoryCategory.Fertilizer,
            Unit = "kg",
            Quantity = 1000,
            MinimumQuantity = 200,
            UnitPrice = 2.50m
        };

        var fieldWork = new FieldWork
        {
            Field = CreateField(),
            Type = FieldWorkType.Fertilizing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned
        };

        context.InventoryItems.Add(inventoryItem);
        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            Resources = new List<ResourceUsageRequest>
            {
                new ResourceUsageRequest
                {
                    InventoryItemId = inventoryItem.Id,
                    QuantityUsed = 100
                },
                new ResourceUsageRequest
                {
                    InventoryItemId = inventoryItem.Id,
                    QuantityUsed = 200
                }
            }
        };

        // Act
        var act = async () => await service.CompleteAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("Ten sam materiał nie może być dodany kilka razy*");
    }

    [Fact]
    public async Task CompleteAsync_ShouldCompleteFieldWork_WhenNoResourcesAreProvided()
    {
        // Arrange
        await using var context = CreateDbContext();

        var fieldWork = new FieldWork
        {
            Field = CreateField(),
            Type = FieldWorkType.Mowing,
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned,
            LaborCost = 150
        };

        context.FieldWorks.Add(fieldWork);
        await context.SaveChangesAsync();

        var service = new FieldWorkCompletionService(context);

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = fieldWork.Id,
            Resources = new List<ResourceUsageRequest>()
        };

        // Act
        await service.CompleteAsync(request);

        // Assert
        var updatedFieldWork = await context.FieldWorks
            .Include(x => x.ResourceUsages)
            .FirstAsync(x => x.Id == fieldWork.Id);

        updatedFieldWork.Status.Should().Be(FieldWorkStatus.Completed);
        updatedFieldWork.CompletedAtUtc.Should().NotBeNull();
        updatedFieldWork.ResourceUsages.Should().BeEmpty();
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Field CreateField()
    {
        return new Field
        {
            Name = "Pole Testowe",
            Location = "Sektor T",
            AreaInHectares = 10,
            SoilType = SoilType.Loamy,
            CurrentCrop = CropType.Wheat,
            Status = FieldStatus.Active
        };
    }
}