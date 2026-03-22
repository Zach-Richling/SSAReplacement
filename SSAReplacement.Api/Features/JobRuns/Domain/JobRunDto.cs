using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.JobRuns.Domain;

public record JobRunStepDto(
    long Id, long JobRunId, long ExecutableVersionId,
    int StepNumber, string StepName, DateTime StartedAt, DateTime? FinishedAt, string Status, int? ExitCode)
{
    public static JobRunStepDto From(JobRunStep s) => new(
        s.Id, s.JobRunId, s.ExecutableVersionId,
        s.StepNumber, s.StepName, s.StartedAt, s.FinishedAt, s.Status, s.ExitCode);
}

public record JobRunDto(
    long Id, long JobId, long? ScheduleId, int? CurrentStep,
    DateTime StartedAt, DateTime? FinishedAt, string Status, int? ExitCode, string? Trigger)
{
    public static JobRunDto From(JobRun r) => new(
        r.Id, r.JobId, r.ScheduleId, r.CurrentStep,
        r.StartedAt, r.FinishedAt, r.Status, r.ExitCode, r.Trigger);
}

public record JobRunDetailDto(
    long Id, long JobId, long? ScheduleId, int? CurrentStep,
    DateTime StartedAt, DateTime? FinishedAt, string Status, int? ExitCode, string? Trigger,
    IReadOnlyList<JobRunStepDto> RunSteps)
{
    public static JobRunDetailDto From(JobRun r) => new(
        r.Id, r.JobId, r.ScheduleId, r.CurrentStep,
        r.StartedAt, r.FinishedAt, r.Status, r.ExitCode, r.Trigger,
        r.RunSteps?.OrderBy(s => s.StepNumber).Select(JobRunStepDto.From).ToList() ?? []);
}
