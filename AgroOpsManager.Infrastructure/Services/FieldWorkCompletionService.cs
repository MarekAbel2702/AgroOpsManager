using AgroOpsManager.Core.DTOs;
using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Core.Exceptions;
using AgroOpsManager.Core.Interfaces;
using AgroOpsManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Infrastructure.Services;

public class FieldWorkCompletionService : IFieldWorkCompletionService
{
    private readonly ApplicationDbContext _context;

    public FieldWorkCompletionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync(CompleteFieldWorkRequest request, CancellationToken cancellationToken = default)
    {
        var fieldWork = await _context.FieldWorks
            .Include(x => x.ResourceUsages)
            .FirstOrDefaultAsync(x => x.Id == request.FieldWorkId, cancellationToken);

        if (fieldWork is null)
        {
            throw new BusinessException("Praca polowa nie istnieje.");
        }

        if (fieldWork.Status == FieldWorkStatus.Completed)
        {
            throw new BusinessException("Ta praca jest już zakończona.");
        }

        if (fieldWork.Status == FieldWorkStatus.Cancelled)
        {
            throw new BusinessException("Nie można zakończyć anulowanej pracy.");
        }

        var selectedResources = request.Resources
            .Where(x => x.InventoryItemId > 0 && x.QuantityUsed > 0)
            .ToList();

        ValidateDuplicatedResources(selectedResources);

        foreach (var resource in selectedResources)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(x => x.Id == resource.InventoryItemId, cancellationToken);

            if (inventoryItem is null)
            {
                throw new BusinessException("Wybrany materiał nie istnieje.");
            }

            if (resource.QuantityUsed <= 0)
            {
                throw new BusinessException($"Ilość dla materiału {inventoryItem.Name} musi być większa od zera.");
            }

            if (resource.QuantityUsed > inventoryItem.Quantity)
            {
                throw new BusinessException(
                    $"Brak wystarczającej ilości materiału {inventoryItem.Name}. " +
                    $"Dostępne: {inventoryItem.Quantity:0.##} {inventoryItem.Unit}, " +
                    $"wymagane: {resource.QuantityUsed:0.##} {inventoryItem.Unit}.");
            }

            inventoryItem.Quantity -= resource.QuantityUsed;

            var usage = new WorkResourceUsage
            {
                FieldWorkId = fieldWork.Id,
                InventoryItemId = inventoryItem.Id,
                QuantityUsed = resource.QuantityUsed,
                UnitPriceAtUsage = inventoryItem.UnitPrice
            };

            _context.WorkResourceUsages.Add(usage);
        }

        fieldWork.Status = FieldWorkStatus.Completed;
        fieldWork.CompletedAtUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.CompletionNotes))
        {
            fieldWork.Notes = request.CompletionNotes;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateDuplicatedResources(List<ResourceUsageRequest> resources)
    {
        var hasDuplicates = resources
            .GroupBy(x => x.InventoryItemId)
            .Any(x => x.Count() > 1);

        if (hasDuplicates)
        {
            throw new BusinessException("Ten sam materiał nie może być dodany kilka razy. Wpisz łączną ilość w jednym wierszu.");
        }
    }
}