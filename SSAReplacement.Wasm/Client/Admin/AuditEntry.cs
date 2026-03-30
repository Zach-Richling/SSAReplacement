namespace SSAReplacement.Wasm.Client.Admin;

public class AuditEntry
{
    public long Id { get; set; }
    public string EntityName { get; set; } = "";
    public long EntityId { get; set; }
    public string Action { get; set; } = "";
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime OccurredAt { get; set; }
    public long? UserId { get; set; }
    public User? User { get; set; }
}
