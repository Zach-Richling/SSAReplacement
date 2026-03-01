using Microsoft.EntityFrameworkCore;
using Polly;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;
using System.Text.Json;

namespace SSAReplacement.Api.Services;

public interface IJobNotificationSender
{
    Task SendJobResultAsync(JobRun run, CancellationToken cancellationToken = default);
}

public sealed class WebhookNotificationSender(
    IHttpClientFactory httpClientFactory,
    AppDbContext db,
    ILogger<WebhookNotificationSender> logger) : IJobNotificationSender
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task SendJobResultAsync(JobRun run, CancellationToken cancellationToken = default)
    {
        var job = await db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == run.JobId, cancellationToken);

        if (job?.WebhookUrl is not { Length: > 0 })
            return;

        var logSnippet = await db.JobLogs
            .AsNoTracking()
            .Where(l => l.JobRunId == run.Id)
            .OrderBy(l => l.Id)
            .Select(l => l.Content)
            .ToListAsync(cancellationToken);

        var combined = string.Join("\n", logSnippet).AsSpan();

        if (combined.Length > 2000)
            combined = combined[..2000];

        var snippet = combined.ToString();

        var payload = new
        {
            jobName = job.Name,
            jobId = run.JobId,
            runId = run.Id,
            status = run.Status,
            exitCode = run.ExitCode,
            startedAt = run.StartedAt,
            finishedAt = run.FinishedAt,
            trigger = run.Trigger,
            logSnippet = snippet
        };

        var client = httpClientFactory.CreateClient();
        var policy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        await policy.ExecuteAsync(async () =>
        {
            var response = await client.PostAsJsonAsync(job.WebhookUrl, payload, JsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Webhook notification sent for JobRun {RunId} to {Url}", run.Id, job.WebhookUrl);
        });
    }
}
