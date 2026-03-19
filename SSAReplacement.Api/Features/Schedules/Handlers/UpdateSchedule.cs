using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Features.Schedules.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Handlers;

public static class UpdateSchedule
{
    public record Request(string? Name, string? CronExpression, bool? IsEnabled);

    public static async Task<IResult> Handler(int id, Request req, AppDbContext db, IScheduleHangfireSyncService sync)
    {
        if (!string.IsNullOrEmpty(req.CronExpression))
        {
            var cronError = ScheduleHelpers.ValidateCronExpression(req.CronExpression);
            if (cronError is not null)
                return Results.BadRequest(cronError);
        }

        var s = await db.Schedules.FindAsync(id);

        if (s is null)
            return Results.NotFound();

        if (!string.IsNullOrWhiteSpace(req.Name)) s.Name = req.Name;
        if (req.CronExpression is not null) s.CronExpression = req.CronExpression;
        if (req.IsEnabled is bool enabled) s.IsEnabled = enabled;

        await db.SaveChangesAsync();
        await sync.AddOrUpdateScheduleAsync(s.Id, s.CronExpression, s.IsEnabled);

        return Results.Ok(ScheduleDto.From(s));
    }
}
