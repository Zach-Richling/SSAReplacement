using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobRuns
{
    public static async Task<IResult> Handler(int id, AppDbContext db)
    {
        var runs = await db.JobRuns
            .AsNoTracking()
            .Where(r => r.JobId == id)
            .OrderByDescending(r => r.StartedAt)
            .ToListAsync();

        return Results.Ok(runs.Select(JobRunDto.From));
    }
}
