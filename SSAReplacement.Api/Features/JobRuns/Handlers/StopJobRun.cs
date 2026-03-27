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
            return Results.Problem(
                detail: "Job run is not currently running.",
                statusCode: StatusCodes.Status409Conflict,
                title: "JOB_NOT_RUNNING");

        var cancelled = cancellationService.TryCancel(id);

        if (!cancelled)
            return Results.Problem(
                detail: "Job run could not be stopped. It may have already finished.",
                statusCode: StatusCodes.Status409Conflict,
                title: "STOP_FAILED");

        return Results.Ok();
    }
}
