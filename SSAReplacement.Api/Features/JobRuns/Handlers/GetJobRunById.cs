using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.JobRuns.Handlers;

public static class GetJobRunById
{
    public static async Task<IResult> Handler(int id, AppDbContext db)
    {
        var run = await db.JobRuns
            .AsNoTracking()
            .Include(r => r.Job)
            .Include(r => r.ExecutableVersion)
                .ThenInclude(v => v.Executable)
            .FirstOrDefaultAsync(r => r.Id == id);

        return run is null ? Results.NotFound() : Results.Ok(JobRunDetailDto.From(run));
    }
}
