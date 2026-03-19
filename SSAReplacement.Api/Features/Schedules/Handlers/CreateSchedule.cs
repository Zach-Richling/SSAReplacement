using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Features.Schedules.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Handlers;

public static class CreateSchedule
{
    public record Request(string Name, string CronExpression, bool IsEnabled = true);

    public static async Task<IResult> Handler(Request req, AppDbContext db, IScheduleHangfireSyncService sync)
    {
        var cronError = ScheduleHelpers.ValidateCronExpression(req.CronExpression);

        if (cronError is not null)
            return Results.BadRequest(cronError);

        var s = new Schedule
        {
            Name = req.Name,
            CronExpression = req.CronExpression,
            IsEnabled = req.IsEnabled
        };

        db.Schedules.Add(s);

        await db.SaveChangesAsync();
        await sync.AddOrUpdateScheduleAsync(s.Id, s.CronExpression, s.IsEnabled);

        return Results.Created($"/schedules/{s.Id}", ScheduleDto.From(s));
    }
}
