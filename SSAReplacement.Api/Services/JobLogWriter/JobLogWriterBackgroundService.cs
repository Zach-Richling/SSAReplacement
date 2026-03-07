using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services.JobLogWriter;

namespace SSAReplacement.Api.Services;

public sealed class JobLogWriterBackgroundService(
    JobLogQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<JobLogWriterBackgroundService> logger) : BackgroundService
{
    private const int BatchSize = 50;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<JobLogEntry>(BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            batch.Clear();
            using var flushCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            flushCts.CancelAfter(FlushInterval);

            try
            {
                await foreach (var entry in queue.Reader.ReadAllAsync(flushCts.Token).WithCancellation(stoppingToken))
                {
                    batch.Add(entry);
                    if (batch.Count >= BatchSize)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
            }

            if (batch.Count > 0)
                await FlushBatchAsync(batch);
        }

        if (batch.Count > 0)
        {
            await FlushBatchAsync(batch);
            batch.Clear();
        }

        while (queue.Reader.TryRead(out var remaining))
        {
            batch.Add(remaining);
            if (batch.Count >= BatchSize)
            {
                await FlushBatchAsync(batch);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await FlushBatchAsync(batch);
    }

    private async Task FlushBatchAsync(List<JobLogEntry> batch)
    {
        if (batch.Count == 0)
            return;

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var entities = batch.Select(e => new JobLog
            {
                JobRunId = e.JobRunId,
                LogType = e.LogType,
                Content = e.Content,
                LogDate = e.LogDate
            }).ToList();

            db.JobLogs.AddRange(entities);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to flush {Count} job log entries", batch.Count);
        }
    }
}
