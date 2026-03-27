using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class DeleteJob
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var j = await db.Jobs.FindAsync(id);

        if (j is null)
            return Results.NotFound();

        var hasRunningRun = await db.JobRuns.AnyAsync(r => r.JobId == id && r.Status == JobRunnerService.StatusRunning);

        if (hasRunningRun)
            return Results.Problem(
                detail: "Cannot delete a job that has a running job run.",
                statusCode: StatusCodes.Status409Conflict,
                title: "JOB_RUNNING");

        db.Jobs.Remove(j);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
