using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Common.JobLogWriter;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Executables.Infrastructure;
using SSAReplacement.Api.Infrastructure;
using System.Diagnostics;

namespace SSAReplacement.Api.Features.Jobs.Infrastructure;

public sealed class JobRunnerService(
    IServiceScopeFactory scopeFactory,
    IExecutableStorage storage,
    IJobLogQueue jobLogQueue,
    ILogger<JobRunnerService> logger)
{
    public const string StatusRunning = "Running";
    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";
    public const string StatusStopped = "Stopped";

    public const string TriggerScheduled = "Scheduled";
    public const string TriggerManual = "Manual";

    public async Task RunAsync(long jobId, long? scheduleId = null, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var job = await db.Jobs
            .Include(j => j.Executable)
                .ThenInclude(e => e.Versions.Where(v => v.IsActive))
            .Include(j => j.Variables)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null)
        {
            logger.LogWarning("Job {JobId} not found", jobId);
            return;
        }

        var activeVersion = job.Executable?.Versions.FirstOrDefault();
        if (activeVersion == null)
        {
            logger.LogWarning("Job {JobId} executable has no active version", jobId);
            return;
        }

        var versionDir = storage.GetVersionDirectory(job.ExecutableId, activeVersion.Version);
        var dllPath = Path.Combine(versionDir, activeVersion.EntryPointDll);

        if (!File.Exists(dllPath))
        {
            logger.LogError("Entry point DLL not found: {Path}", dllPath);
            return;
        }

        var run = new JobRun
        {
            JobId = jobId,
            ExecutableVersionId = activeVersion.Id,
            ScheduleId = scheduleId,
            StartedAt = DateTime.UtcNow,
            Status = StatusRunning,
            Trigger = scheduleId.HasValue ? TriggerScheduled : TriggerManual
        };

        db.JobRuns.Add(run);
        await db.SaveChangesAsync(cancellationToken);

        var runId = run.Id;

        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"\"{activeVersion.EntryPointDll}\"";
            process.StartInfo.WorkingDirectory = versionDir;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            foreach (var v in job.Variables)
                process.StartInfo.Environment[$"JobVariables__{v.Key}"] = v.Value;

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null)
                    _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runId, "StdOut", e.Data, DateTime.UtcNow), CancellationToken.None);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                    _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runId, "StdErr", e.Data, DateTime.UtcNow), CancellationToken.None);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);

            run.FinishedAt = DateTime.UtcNow;
            run.ExitCode = process.ExitCode;
            run.Status = process.ExitCode == 0 ? StatusSuccess : StatusFailed;
        }
        catch (TaskCanceledException)
        {
            run.FinishedAt = DateTime.UtcNow;
            run.Status = StatusStopped;
            run.ExitCode = -2;

            _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runId, "StdErr", "The job was forcefully stopped.", DateTime.UtcNow), CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running job {JobId}", jobId);
            run.FinishedAt = DateTime.UtcNow;
            run.Status = StatusFailed;
            run.ExitCode = -1;

            _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runId, "StdErr", ex.ToString(), DateTime.UtcNow), CancellationToken.None);
        }

        db.JobRuns.Update(run);

        await db.SaveChangesAsync(CancellationToken.None);
    }
}
