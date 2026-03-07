using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record JobDto(int Id, int ExecutableId, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail, string? ExecutableName)
{
    public static JobDto From(Job j, string? executableName = null) => new(
        j.Id, j.ExecutableId, j.Name, j.IsEnabled, j.CreatedAt, j.NotifyEmail,
        executableName ?? (j.Executable?.Name));
}

public record JobVariableDto(string Key, string Value);

public record JobDetailDto(
    int Id, int ExecutableId, string Name, bool IsEnabled, DateTime CreatedAt, string? NotifyEmail, string? ExecutableName,
    IReadOnlyList<JobVariableDto> Variables,
    IReadOnlyList<ScheduleDto> Schedules)
{
    public static JobDetailDto From(Job j) => new(
        j.Id, j.ExecutableId, j.Name, j.IsEnabled, j.CreatedAt, j.NotifyEmail, j.Executable?.Name,
        j.Variables?.Select(v => new JobVariableDto(v.Key, v.Value)).ToList() ?? [],
        j.JobSchedules?.Select(js => ScheduleDto.From(js.Schedule)).ToList() ?? []
    );
}
