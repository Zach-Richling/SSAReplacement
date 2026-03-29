using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Infrastructure;

public class AuditService(AppDbContext db, ICurrentUserService currentUserService)
{
    public async Task WriteAsync(string entityName, long entityId, string action, CancellationToken ct = default)
    {
        db.AuditEntries.Add(new AuditEntry
        {
            UserId = currentUserService.UserId,
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OccurredAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
    }
}
