using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

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

        group.MapGet("/{id:int}/logs/stream", async (int id, [FromHeader(Name = "Last-Event-ID")] int? lastEventId, IServiceScopeFactory scopeFactory, CancellationToken cancellationToken) =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var runExists = await db.JobRuns.AsNoTracking().AnyAsync(r => r.Id == id, cancellationToken);

                if (!runExists)
                    return Results.NotFound();

                var stream = StreamJobLogsAsync(id, lastEventId ?? 0, scopeFactory, cancellationToken);

                return TypedResults.ServerSentEvents(stream);
            }
            catch (OperationCanceledException)
            {
                return Results.Ok();
            }
        });
    }

    private static async IAsyncEnumerable<SseItem<JobLogDto>> StreamJobLogsAsync(
        int id,
        int lastSeenId,
        IServiceScopeFactory scopeFactory,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var job = await db.JobRuns
            .AsNoTracking()
            .Where(jr => jr.Id == id)
            .FirstAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var logs = await db.JobLogs
                .AsNoTracking()
                .Where(l => l.JobRunId == id && l.Id > lastSeenId)
                .OrderBy(l => l.Id)
                .ToListAsync(cancellationToken);

            foreach (var log in logs)
            {
                var dto = JobLogDto.From(log);
                yield return new SseItem<JobLogDto>(dto, "job-log") { EventId = log.Id.ToString() };
                lastSeenId = log.Id;
            }

            //Keep connection open but stop querying for non-running jobs
            //All logs will be sent to the client by the above foreach loop
            if (job.Status != JobRunnerService.StatusRunning)
            {
                await Task.Delay(-1, cancellationToken);
            }

            await Task.Delay(2000, cancellationToken);
        }
    }
}
