using AgroOpsManager.Core.DTOs;

namespace AgroOpsManager.Core.Interfaces;

public interface IFieldWorkCompletionService
{
    Task CompleteAsync(CompleteFieldWorkRequest request, CancellationToken cancellationToken = default);
}