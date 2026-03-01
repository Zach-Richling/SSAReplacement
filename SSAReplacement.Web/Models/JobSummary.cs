namespace SSAReplacement.Web.Models;

public record JobSummary(
    int Id,
    int ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? WebhookUrl,
    string? NotifyEmail,
    string? ExecutableName);
