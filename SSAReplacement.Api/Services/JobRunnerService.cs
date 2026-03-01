using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;
using System.Diagnostics;

namespace SSAReplacement.Api.Services;

public sealed class JobRunnerService(
    IServiceScopeFactory scopeFactory,
    IExecutableStorage storage,
    ILogger<JobRunnerService> logger)
{
    public const string StatusRunning = "Running";
    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";
    public const string TriggerScheduled = "Scheduled";
    public const string TriggerManual = "Manual";

    public async Task RunAsync(int jobId, int? scheduleId = null, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notifier = scope.ServiceProvider.GetRequiredService<IJobNotificationSender>();

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

        var versionDir = storage.GetVersionDirectory(job.ExecutableId, activeVersion.Id);
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

        var stdout = new List<string>();
        var stderr = new List<string>();

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
                process.StartInfo.Environment[v.Key] = v.Value;

            process.OutputDataReceived += (_, e) => { if (e.Data != null) stdout.Add(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) stderr.Add(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(cancellationToken);

            run.FinishedAt = DateTime.UtcNow;
            run.ExitCode = process.ExitCode;
            run.Status = process.ExitCode == 0 ? StatusSuccess : StatusFailed;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running job {JobId}", jobId);
            run.FinishedAt = DateTime.UtcNow;
            run.Status = StatusFailed;
            run.ExitCode = -1;
            stderr.Add(ex.ToString());
        }

        db.JobRuns.Update(run);
        await db.SaveChangesAsync(cancellationToken);

        if (stdout.Count > 0)
            db.JobLogs.Add(new JobLog { JobRunId = run.Id, LogType = "StdOut", Content = string.Join(Environment.NewLine, stdout) });

        if (stderr.Count > 0)
            db.JobLogs.Add(new JobLog { JobRunId = run.Id, LogType = "StdErr", Content = string.Join(Environment.NewLine, stderr) });

        await db.SaveChangesAsync(cancellationToken);

        await notifier.SendJobResultAsync(run, cancellationToken);
    }
}
