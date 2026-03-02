namespace SSAReplacement.Web.Client;

/// <summary>
/// Job list item (matches API JobDto from GET /jobs).
/// </summary>
public record Job(
    int Id,
    int ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? WebhookUrl,
    string? NotifyEmail,
    string? ExecutableName);
