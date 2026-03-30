namespace SSAReplacement.Api.Domain;

public class AuditEntry
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public required string EntityName { get; set; }
    public long EntityId { get; set; }
    public required string Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
