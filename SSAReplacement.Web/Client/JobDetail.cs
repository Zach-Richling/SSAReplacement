namespace SSAReplacement.Web.Client;

/// <summary>
/// Job detail (matches API JobDetailDto from GET /jobs/{id}).
/// </summary>
public record JobDetail(
    int Id,
    int ExecutableId,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    string? WebhookUrl,
    string? NotifyEmail,
    string? ExecutableName,
    IReadOnlyList<JobVariableDto> Variables,
    IReadOnlyList<Schedule> Schedules);
