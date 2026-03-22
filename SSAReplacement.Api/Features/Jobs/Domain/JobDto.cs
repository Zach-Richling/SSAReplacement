using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Features.Schedules.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Domain;

public record JobStepParameterDto(long Id, long JobStepId, string Key, string Value)
{
    public static JobStepParameterDto From(JobStepParameter p) => new(p.Id, p.JobStepId, p.Key, p.Value);
}

public record JobStepDto(long Id, long ExecutableId, string ExecutableName, int StepNumber, string Name, IReadOnlyList<JobStepParameterDto> Parameters)
{
    public static JobStepDto From(JobStep s) => new(
        s.Id,
        s.ExecutableId,
        s.Executable?.Name ?? "",
        s.StepNumber,
        s.Name,
        s.Parameters?.Select(JobStepParameterDto.From).ToList() ?? []);
}

public record JobDto(long Id, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail, DateTime? NextRunUtc)
{
    public static JobDto From(Job j) => new(
        j.Id,
        j.Name,
        j.IsEnabled,
        j.CreatedAt,
        j.NotifyEmail,
        ComputeNextRunUtc(j));

    private static DateTime? ComputeNextRunUtc(Job j)
    {
        if (!j.IsEnabled)
            return null;

        var utcNow = DateTime.UtcNow;

        DateTime? earliest = null;
        foreach (var js in j.JobSchedules ?? [])
        {
            if (js.Schedule is not Schedule schedule || !schedule.IsEnabled)
                continue;

            var next = ScheduleHelpers.TryGetNextOccurrenceUtc(schedule.CronExpression, utcNow);
            if (next is DateTime dt && (earliest is null || dt < earliest))
                earliest = dt;
        }

        return earliest;
    }
}

public record JobDetailDto(
    long Id, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail,
    IReadOnlyList<JobStepDto> Steps,
    IReadOnlyList<ScheduleDto> Schedules)
{
    public static JobDetailDto From(Job j) => new(
        j.Id, j.Name, j.IsEnabled, j.CreatedAt, j.NotifyEmail,
        j.Steps?.OrderBy(s => s.StepNumber).Select(JobStepDto.From).ToList() ?? [],
        j.JobSchedules?.Select(js => ScheduleDto.From(js.Schedule)).ToList() ?? []
    );
}
