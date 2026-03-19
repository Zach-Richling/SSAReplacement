using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.Schedules.Domain;

public record ScheduleDto(int Id, string? Name, string CronExpression, bool IsEnabled, DateTime CreatedAt)
{
    public static ScheduleDto From(Schedule s) => new(s.Id, s.Name, s.CronExpression, s.IsEnabled, s.CreatedAt);
}
