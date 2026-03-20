namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job list item (matches API JobDto from GET /jobs).
/// </summary>
public record Job(
    long Id,
    long ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? NotifyEmail,
    DateTime? NextRunUtc)
{
    public DateTime? NextRunLocal => NextRunUtc?.ToLocalTime();
};
