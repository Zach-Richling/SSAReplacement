using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.JobRuns.Handlers;

public static class StopJobRun
{
    public static async Task<IResult> Handler(long id, AppDbContext db, JobCancellationService cancellationService)
    {
        var run = await db.JobRuns.FirstOrDefaultAsync(r => r.Id == id);

        if (run is null)
            return Results.NotFound();

        if (run.Status != JobRunnerService.StatusRunning)
            return Results.Conflict(new { message = "Job run is not currently running." });

        var cancelled = cancellationService.TryCancel(id);

        if (!cancelled)
            return Results.Conflict(new { message = "Job run could not be stopped. It may have already finished." });

        return Results.Ok();
    }
}
