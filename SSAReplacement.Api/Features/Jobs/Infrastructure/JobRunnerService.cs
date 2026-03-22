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
    JobCancellationService cancellationService,
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
            .Include(j => j.Steps.OrderBy(s => s.StepNumber))
                .ThenInclude(s => s.Executable)
                    .ThenInclude(e => e.Versions.Where(v => v.IsActive))
            .Include(j => j.Steps)
                .ThenInclude(s => s.Parameters)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null)
        {
            logger.LogWarning("Job {JobId} not found", jobId);
            return;
        }

        if (job.Steps.Count == 0)
        {
            logger.LogWarning("Job {JobId} has no steps defined", jobId);
            return;
        }

        var run = new JobRun
        {
            JobId = jobId,
            ScheduleId = scheduleId,
            StartedAt = DateTime.UtcNow,
            Status = StatusRunning,
            Trigger = scheduleId.HasValue ? TriggerScheduled : TriggerManual
        };

        db.JobRuns.Add(run);
        await db.SaveChangesAsync(cancellationToken);

        var runId = run.Id;
        var overallSuccess = true;
        var wasCancelled = false;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationService.Register(runId, linkedCts);

        try
        {
            foreach (var step in job.Steps.OrderBy(s => s.StepNumber))
            {
                var activeVersion = step.Executable?.Versions.FirstOrDefault();
                if (activeVersion == null)
                {
                    logger.LogWarning("Job {JobId} step {StepNumber} executable has no active version", jobId, step.StepNumber);
                    run.FinishedAt = DateTime.UtcNow;
                    run.Status = StatusFailed;
                    run.ExitCode = -1;

                    db.JobRuns.Update(run);
                    await db.SaveChangesAsync(CancellationToken.None);

                    return;
                }

                var versionDir = storage.GetVersionDirectory(step.ExecutableId, activeVersion.Version);
                var dllPath = Path.Combine(versionDir, activeVersion.EntryPointDll);

                if (!File.Exists(dllPath))
                {
                    logger.LogError("Entry point DLL not found for step {StepNumber}: {Path}", step.StepNumber, dllPath);
                    run.FinishedAt = DateTime.UtcNow;
                    run.Status = StatusFailed;
                    run.ExitCode = -1;

                    db.JobRuns.Update(run);
                    await db.SaveChangesAsync(CancellationToken.None);

                    return;
                }

                run.CurrentStep = step.StepNumber;
                db.JobRuns.Update(run);

                var runStep = new JobRunStep
                {
                    JobRunId = runId,
                    ExecutableVersionId = activeVersion.Id,
                    StepNumber = step.StepNumber,
                    StepName = step.Name,
                    StartedAt = DateTime.UtcNow,
                    Status = StatusRunning
                };

                db.JobRunSteps.Add(runStep);
                await db.SaveChangesAsync(linkedCts.Token);

                var runStepId = runStep.Id;
                var process = new Process();

                try
                {
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"\"{activeVersion.EntryPointDll}\"";
                    process.StartInfo.WorkingDirectory = versionDir;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;

                    foreach (var v in step.Parameters)
                        process.StartInfo.Environment[$"JobVariables__{v.Key}"] = v.Value;

                    process.OutputDataReceived += (_, e) =>
                    {
                        if (e.Data != null)
                            _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runStepId, "StdOut", e.Data, DateTime.UtcNow), CancellationToken.None);
                    };

                    process.ErrorDataReceived += (_, e) =>
                    {
                        if (e.Data != null)
                            _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runStepId, "StdErr", e.Data, DateTime.UtcNow), CancellationToken.None);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync(linkedCts.Token);

                    runStep.FinishedAt = DateTime.UtcNow;
                    runStep.ExitCode = process.ExitCode;
                    runStep.Status = process.ExitCode == 0 ? StatusSuccess : StatusFailed;

                    if (process.ExitCode != 0)
                        overallSuccess = false;
                }
                catch (OperationCanceledException)
                {
                    try
                    {
                        if (!process.HasExited)
                            process.Kill(entireProcessTree: true);
                    }
                    catch { }

                    runStep.FinishedAt = DateTime.UtcNow;
                    runStep.Status = StatusStopped;
                    runStep.ExitCode = -2;
                    overallSuccess = false;
                    wasCancelled = true;

                    _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runStepId, "StdErr", "The job was forcefully stopped.", DateTime.UtcNow), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error running job {JobId} step {StepNumber}", jobId, step.StepNumber);
                    runStep.FinishedAt = DateTime.UtcNow;
                    runStep.Status = StatusFailed;
                    runStep.ExitCode = -1;
                    overallSuccess = false;

                    _ = jobLogQueue.EnqueueAsync(new JobLogEntry(runStepId, "StdErr", ex.ToString(), DateTime.UtcNow), CancellationToken.None);
                }
                finally
                {
                    process?.Dispose();
                }

                db.JobRunSteps.Update(runStep);
                await db.SaveChangesAsync(CancellationToken.None);

                if (!overallSuccess)
                    break;
            }

            run.FinishedAt = DateTime.UtcNow;
            run.Status = overallSuccess ? StatusSuccess : (wasCancelled ? StatusStopped : StatusFailed);

            if (!overallSuccess)
                run.ExitCode = -1;

            db.JobRuns.Update(run);
            await db.SaveChangesAsync(CancellationToken.None);
        }
        finally
        {
            cancellationService.Unregister(runId);
        }
    }
}
