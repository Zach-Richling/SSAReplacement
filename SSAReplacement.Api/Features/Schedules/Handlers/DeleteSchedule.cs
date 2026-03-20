using SSAReplacement.Api.Features.Schedules.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Handlers;

public static class DeleteSchedule
{
    public static async Task<IResult> Handler(long id, AppDbContext db, IScheduleHangfireSyncService sync)
    {
        var s = await db.Schedules.FindAsync(id);

        if (s is null)
            return Results.NotFound();

        db.Schedules.Remove(s);

        await db.SaveChangesAsync();
        await sync.RemoveScheduleAsync(id);

        return Results.NoContent();
    }
}
