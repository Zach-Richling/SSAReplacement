namespace SSAReplacement.Wasm.Client.Admin;

public class User
{
    public long Id { get; set; }
    public string Sid { get; set; } = "";
    public string Username { get; set; } = "";
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
}

public class UserDetail
{
    public long Id { get; set; }
    public string Sid { get; set; } = "";
    public string Username { get; set; } = "";
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public IEnumerable<AuditEntry> AuditEntries { get; set; } = [];
}
