using Hangfire;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class TriggerJob
{
    public static async Task<IResult> Handler(int id, AppDbContext db, IBackgroundJobClient jobClient)
    {
        var exists = await db.Jobs.AnyAsync(j => j.Id == id);

        if (!exists)
            return Results.NotFound();

        jobClient.Enqueue<JobRunnerService>(s => s.RunAsync(id));

        return Results.Accepted();
    }
}
