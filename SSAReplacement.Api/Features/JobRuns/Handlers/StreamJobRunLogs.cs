using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Domain;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

namespace SSAReplacement.Api.Features.JobRuns.Handlers;

public static class StreamJobRunLogs
{
    public static async Task<IResult> Handler(
        long id,
        [FromHeader(Name = "Last-Event-ID")] long? lastEventId,
        IServiceScopeFactory scopeFactory,
        CancellationToken cancellationToken)
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
    }

    private static async IAsyncEnumerable<SseItem<JobLogDto>> StreamJobLogsAsync(
        long id,
        long lastSeenId,
        IServiceScopeFactory scopeFactory,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var run = await db.JobRuns
            .AsNoTracking()
            .Where(jr => jr.Id == id)
            .FirstAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var stepIds = await db.JobRunSteps
                .AsNoTracking()
                .Where(s => s.JobRunId == id)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            var logs = await db.JobLogs
                .AsNoTracking()
                .Where(l => stepIds.Contains(l.JobRunStepId) && l.Id > lastSeenId)
                .OrderBy(l => l.Id)
                .ToListAsync(cancellationToken);

            foreach (var log in logs)
            {
                var dto = JobLogDto.From(log);
                yield return new SseItem<JobLogDto>(dto, "job-log") { EventId = log.Id.ToString() };
                lastSeenId = log.Id;
            }

            // Keep connection open but stop querying for non-running jobs.
            // All logs will have been sent to the client by the above foreach loop.
            if (run.Status != JobRunnerService.StatusRunning)
            {
                await Task.Delay(-1, cancellationToken);
            }

            await Task.Delay(2000, cancellationToken);
        }
    }
}
