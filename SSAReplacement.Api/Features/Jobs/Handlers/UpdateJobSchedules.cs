using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJobSchedules
{
    public record Request(long[]? ScheduleIds);

    public static async Task<IResult> Handler(long id, Request request, AppDbContext db)
    {
        var job = await db.Jobs
            .Include(j => j.JobSchedules)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return Results.NotFound();

        var requestedIds = (request.ScheduleIds ?? []).ToHashSet();

        var toRemove = job.JobSchedules.Where(js => !requestedIds.Contains(js.ScheduleId)).ToList();
        foreach (var js in toRemove)
            job.JobSchedules.Remove(js);

        var existingIds = job.JobSchedules.Select(js => js.ScheduleId).ToHashSet();
        foreach (var sid in requestedIds.Where(sid => !existingIds.Contains(sid)))
        {
            if (await db.Schedules.AnyAsync(s => s.Id == sid))
                job.JobSchedules.Add(new JobSchedule { ScheduleId = sid });
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
