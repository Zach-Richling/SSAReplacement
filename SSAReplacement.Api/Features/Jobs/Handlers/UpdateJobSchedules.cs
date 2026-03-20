using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJobSchedules
{
    public record Request(int[]? ScheduleIds);

    public static async Task<IResult> Handler(long id, Request request, AppDbContext db)
    {
        var job = await db.Jobs
            .Include(j => j.JobSchedules)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return Results.NotFound();

        job.JobSchedules.Clear();
        foreach (var sid in request.ScheduleIds ?? [])
        {
            if (await db.Schedules.AnyAsync(s => s.Id == sid))
                job.JobSchedules.Add(new JobSchedule { ScheduleId = sid });
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
