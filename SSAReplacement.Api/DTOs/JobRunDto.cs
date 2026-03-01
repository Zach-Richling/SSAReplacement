using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record JobRunDto(
    int Id, int JobId, int ExecutableVersionId, int? ScheduleId,
    DateTime StartedAt, DateTime? FinishedAt, string Status, int? ExitCode, string? Trigger)
{
    public static JobRunDto From(JobRun r) => new(
        r.Id, r.JobId, r.ExecutableVersionId, r.ScheduleId,
        r.StartedAt, r.FinishedAt, r.Status, r.ExitCode, r.Trigger);
}

public record JobRunDetailDto(
    int Id, int JobId, int ExecutableVersionId, int? ScheduleId,
    DateTime StartedAt, DateTime? FinishedAt, string Status, int? ExitCode, string? Trigger,
    ExecutableVersionDto? ExecutableVersion, string? ExecutableName)
{
    public static JobRunDetailDto From(JobRun r) => new(
        r.Id, r.JobId, r.ExecutableVersionId, r.ScheduleId,
        r.StartedAt, r.FinishedAt, r.Status, r.ExitCode, r.Trigger,
        r.ExecutableVersion is not null ? ExecutableVersionDto.From(r.ExecutableVersion) : null,
        r.ExecutableVersion?.Executable?.Name);
}
