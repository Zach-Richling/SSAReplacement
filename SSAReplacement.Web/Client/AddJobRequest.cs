namespace SSAReplacement.Web.Client;

/// <summary>
/// Request body for POST /jobs (create job).
/// </summary>
public record AddJobRequest(
    int ExecutableId,
    string Name,
    bool IsEnabled = true,
    string? WebhookUrl = null,
    string? NotifyEmail = null);
