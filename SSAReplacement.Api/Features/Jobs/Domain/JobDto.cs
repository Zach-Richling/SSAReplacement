using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Features.Schedules.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Domain;

public record JobDto(int Id, int ExecutableId, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail, DateTime? NextRunUtc)
{
    public static JobDto From(Job j) => new(
        j.Id,
        j.ExecutableId,
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

public record JobVariableDto(string Key, string Value)
{
    public static JobVariableDto From(JobVariable jv) => new(jv.Key, jv.Value);
}

public record JobDetailDto(
    int Id, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail,
    ExecutableDto Executable,
    IReadOnlyList<JobVariableDto> Variables,
    IReadOnlyList<ScheduleDto> Schedules)
{
    public static JobDetailDto From(Job j) => new(
        j.Id, j.Name, j.IsEnabled, j.CreatedAt, j.NotifyEmail,
        ExecutableDto.From(j.Executable),
        j.Variables?.Select(v => new JobVariableDto(v.Key, v.Value)).ToList() ?? [],
        j.JobSchedules?.Select(js => ScheduleDto.From(js.Schedule)).ToList() ?? []
    );
}
