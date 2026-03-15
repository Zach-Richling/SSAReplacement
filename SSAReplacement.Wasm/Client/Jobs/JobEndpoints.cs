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
    public async Task<JobDetail?> GetJobAsync(int id, CancellationToken cancellationToken = default)
    {
        var res = await http.GetAsync($"jobs/{id}", cancellationToken);
        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<JobDetail>(cancellationToken);
    }

    /// <summary>
    /// PUT /jobs/{id}/schedules. Replaces the job's assigned schedules.
    /// </summary>
    public async Task SetJobSchedulesAsync(int jobId, IEnumerable<int> scheduleIds, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/schedules", new { ScheduleIds = scheduleIds }, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /jobs/{id}/trigger. Enqueues a run.
    /// </summary>
    public async Task TriggerJobAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsync($"jobs/{jobId}/trigger", null, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// GET /jobs/{id}/runs. Returns the last 100 runs for the job.
    /// </summary>
    public async Task<List<JobRun>> GetJobRunsAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobRun>>($"jobs/{jobId}/runs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /runs/{id}. Returns the job run detail
    /// </summary>
    public async Task<JobRun?> GetJobRunAsync(int jobRunId)
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
    public async Task<List<JobLog>> GetJobLogsAsync(int jobRunId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobLog>>($"runs/{jobRunId}/logs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /job/{jobId}/parameters. Returns all parameters for the job.
    /// </summary>
    public async Task<List<JobParameter>> GetJobParametersAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<JobParameter>>($"jobs/{jobId}/parameters", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// PUT /job/{jobId}/parameters. Overwrites the job parameters with the input parameters
    /// </summary>
    public async Task SetJobParametersAsync(int jobId, Dictionary<string, string?> parameters, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/parameters", new { Parameters = parameters }, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Returns the full URL for the job log SSE stream (GET /runs/{id}/logs/stream).
    /// Resume is via Last-Event-ID header (EventSource sends it automatically on reconnect).
    /// </summary>
    public string GetJobLogStreamUrl(int jobRunId)
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
    public async Task UpdateJobAsync(int jobId, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"jobs/{jobId}", request, cancellationToken);
        res.EnsureSuccessStatusCode();
    }
}
