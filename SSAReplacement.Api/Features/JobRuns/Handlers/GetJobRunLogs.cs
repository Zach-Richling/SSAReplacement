using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.JobRuns.Handlers;

public static class GetJobRunLogs
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var runExists = await db.JobRuns.AsNoTracking().AnyAsync(r => r.Id == id);
        if (!runExists)
            return Results.NotFound();

        var stepIds = await db.JobRunSteps
            .AsNoTracking()
            .Where(s => s.JobRunId == id)
            .Select(s => s.Id)
            .ToListAsync();

        var logs = await db.JobLogs
            .AsNoTracking()
            .Where(l => stepIds.Contains(l.JobRunStepId))
            .OrderBy(l => l.Id)
            .ToListAsync();

        return Results.Ok(logs.Select(JobLogDto.From));
    }
}
