using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Endpoints;

public static class JobRunEndpoints
{
    public static void MapJobRunEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/runs").WithTags("Job Runs");

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var run = await db.JobRuns
                .AsNoTracking()
                .Include(r => r.Job)
                .Include(r => r.ExecutableVersion)
                    .ThenInclude(v => v.Executable)
                .FirstOrDefaultAsync(r => r.Id == id);

            return run is null ? Results.NotFound() : Results.Ok(JobRunDetailDto.From(run));
        });

        group.MapGet("/{id:int}/logs", async (int id, AppDbContext db) =>
        {
            var logs = await db.JobLogs.AsNoTracking().Where(l => l.JobRunId == id).OrderBy(l => l.Id).ToListAsync();
            return Results.Ok(logs.Select(JobLogDto.From));
        });
    }
}
