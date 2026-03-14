using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record JobDto(int Id, int ExecutableId, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail)
{
    public static JobDto From(Job j) => new(j.Id, j.ExecutableId, j.Name, j.IsEnabled, j.CreatedAt, j.NotifyEmail);
}

public record JobVariableDto(string Key, string Value)
{
    public static JobVariableDto From(JobVariable jv) => new(jv.Key, jv.Value);
};

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
