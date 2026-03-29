namespace SSAReplacement.Api.Domain;

public class User
{
    public long Id { get; set; }
    public required string Sid { get; set; }
    public required string Username { get; set; }
    public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;

    public ICollection<AuditEntry> AuditEntries { get; set; } = [];
}
