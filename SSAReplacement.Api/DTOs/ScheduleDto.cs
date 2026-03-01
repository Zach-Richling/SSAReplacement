using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record ScheduleDto(int Id, string? Name, string CronExpression, bool IsEnabled, DateTime CreatedAt)
{
    public static ScheduleDto From(Schedule s) => new(s.Id, s.Name, s.CronExpression, s.IsEnabled, s.CreatedAt);
}
