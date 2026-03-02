namespace SSAReplacement.Web.Client;

/// <summary>
/// Response body from POST /jobs (created job).
/// </summary>
public record AddJobResponse(
    int Id,
    int ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? WebhookUrl,
    string? NotifyEmail,
    string? ExecutableName);
