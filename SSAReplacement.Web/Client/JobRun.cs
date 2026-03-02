namespace SSAReplacement.Web.Client;

/// <summary>
/// Job run (matches API JobRunDto from GET /jobs/{id}/runs).
/// </summary>
public record JobRun(
    int Id,
    int JobId,
    int ExecutableVersionId,
    int? ScheduleId,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string Status,
    int? ExitCode,
    string? Trigger);
