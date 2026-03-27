namespace SSAReplacement.Api.Domain;

public class RefreshToken
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }
}
