using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.JobRuns.Handlers;

public static class GetJobRunLogs
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var logs = await db.JobLogs.AsNoTracking().Where(l => l.JobRunId == id).OrderBy(l => l.Id).ToListAsync();
        return Results.Ok(logs.Select(JobLogDto.From));
    }
}
