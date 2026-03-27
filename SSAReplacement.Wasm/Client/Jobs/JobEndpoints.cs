using System.Net;
using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client.Jobs;

public class JobEndpoints(HttpClient http)
{
    /// <summary>
    /// GET /jobs. Returns the list of jobs.
    /// </summary>
    public async Task<List<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Job>>("jobs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /jobs/{id}. Returns the job detail or null if not found.
    /// </summary>
    public async Task<JobDetail?> GetJobAsync(long id, CancellationToken cancellationToken = default)
    {
        var res = await http.GetAsync($"jobs/{id}", cancellationToken);
        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<JobDetail>(cancellationToken);
    }

    /// <summary>
    /// PUT /jobs/{id}/steps. Replaces the job's steps.
    /// </summary>
    public async Task SetJobStepsAsync(long jobId, IEnumerable<JobStepRequest> steps, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/steps", new { Steps = steps }, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// PUT /jobs/{id}/schedules. Replaces the job's assigned schedules.
    /// </summary>
    public async Task SetJobSchedulesAsync(long jobId, IEnumerable<long> scheduleIds, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/schedules", new { ScheduleIds = scheduleIds }, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /jobs/{id}/trigger. Enqueues a run. Optionally starts at a specific step number.
    /// </summary>
    public async Task TriggerJobAsync(long jobId, int? startAtStep = null, CancellationToken cancellationToken = default)
    {
        var res = startAtStep.HasValue
            ? await http.PostAsJsonAsync($"jobs/{jobId}/trigger", new { StartAtStep = startAtStep.Value }, cancellationToken)
            : await http.PostAsync($"jobs/{jobId}/trigger", null, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// GET /jobs/{id}/runs. Returns the last 100 runs for the job.
    /// </summary>
    public async Task<List<JobRun>> GetJobRunsAsync(long jobId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobRun>>($"jobs/{jobId}/runs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /runs/{id}. Returns the job run detail
    /// </summary>
    public async Task<JobRun?> GetJobRunAsync(long jobRunId)
    {
        var res = await http.GetAsync($"runs/{jobRunId}");

        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<JobRun>();
    }

    /// <summary>
    /// GET /runs/{id}/logs. Returns all logs for the run.
    /// </summary>
    public async Task<List<JobLog>> GetJobLogsAsync(long jobRunId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobLog>>($"runs/{jobRunId}/logs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /jobs/{jobId}/steps/{stepId}/parameters. Returns all parameters for the given step.
    /// </summary>
    public async Task<List<JobStepParameter>> GetJobStepParametersAsync(long jobId, long stepId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobStepParameter>>($"jobs/{jobId}/steps/{stepId}/parameters", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// PUT /jobs/{jobId}/steps/{stepId}/parameters. Overwrites the step parameters.
    /// </summary>
    public async Task SetJobStepParametersAsync(long jobId, long stepId, Dictionary<string, string?> parameters, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/steps/{stepId}/parameters", new { Parameters = parameters }, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Returns the full URL for the job log SSE stream (GET /runs/{id}/logs/stream).
    /// Resume is via Last-Event-ID header (EventSource sends it automatically on reconnect).
    /// </summary>
    public string GetJobLogStreamUrl(long jobRunId)
    {
        var baseUrl = http.BaseAddress?.ToString().TrimEnd('/') ?? "";
        return $"{baseUrl}/runs/{jobRunId}/logs/stream";
    }

    /// <summary>
    /// POST /jobs. Creates a job. Returns the created job on success.
    /// </summary>
    public async Task<Job> AddJobAsync(AddJobRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("jobs", request, cancellationToken);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<Job>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }

    /// <summary>
    /// PUT /jobs/{jobId}. Updates a job
    /// </summary>
    public async Task UpdateJobAsync(long jobId, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}", request, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /runs/{id}/stop. Stops a running job run.
    /// Returns true if the stop was accepted, false if the run was not running or already finished.
    /// </summary>
    public async Task<bool> StopJobRunAsync(long jobRunId, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsync($"runs/{jobRunId}/stop", null, cancellationToken);

        if (res.StatusCode == HttpStatusCode.Conflict)
            return false;

        res.EnsureSuccessStatusCode();
        return true;
    }

    /// <summary>
    /// DELETE /jobs/{id}. Removes the job.
    /// Returns a result indicating success or failure with a reason.
    /// </summary>
    public async Task<DeleteJobResult> DeleteJobAsync(long jobId, CancellationToken cancellationToken = default)
    {
        var res = await http.DeleteAsync($"jobs/{jobId}", cancellationToken);

        if (res.StatusCode == HttpStatusCode.NotFound)
            return new DeleteJobResult(true, null, null);

        if (res.StatusCode == HttpStatusCode.Conflict)
        {
            var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
            return new DeleteJobResult(false, problem?.Title, problem?.Detail);
        }

        res.EnsureSuccessStatusCode();
        return new DeleteJobResult(true, null, null);
    }
}

public record DeleteJobResult(bool Success, string? Code, string? Message);

public record ProblemDetails
{
    public string? Type { get; init; }
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? Detail { get; init; }
}
