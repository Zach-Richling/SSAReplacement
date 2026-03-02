namespace SSAReplacement.Web.Models;

internal record JobSummary(
    int Id,
    int ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? WebhookUrl,
    string? NotifyEmail,
    string? ExecutableName);
