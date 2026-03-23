using Hangfire;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class TriggerJob
{
    public record TriggerJobRequest(int? StartAtStep = null);

    public static async Task<IResult> Handler(long id, TriggerJobRequest? request, AppDbContext db, IBackgroundJobClient jobClient)
    {
        var exists = await db.Jobs.AnyAsync(j => j.Id == id);

        if (!exists)
            return Results.NotFound();

        var startAtStep = request?.StartAtStep;
        jobClient.Enqueue<JobRunnerService>(s => s.RunAsync(id, null, startAtStep));

        return Results.Accepted();
    }
}
