using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Jobs;
using SSAReplacement.Wasm.Client.Schedules;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client;

/// <summary>
/// Typed HTTP client for the SSA Replacement API. Lives in the Web project; no reference to the Api project.
/// </summary>
public class SsaApiClient(HttpClient http)
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
        var res = await http.PutAsJsonAsync($"jobs/{jobId}/schedules", new PutJobSchedulesRequest(scheduleIds), cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /jobs/{id}/trigger. Enqueues a run. Returns when accepted (202).
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
    /// GET /executables. Returns the list of executables.
    /// </summary>
    public async Task<List<Executable>> GetExecutablesAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Executable>>("executables", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /executables/{id}. Returns the executable detail or null if not found.
    /// </summary>
    public async Task<ExecutableDetail?> GetExecutableAsync(int id, CancellationToken cancellationToken = default)
    {
        var res = await http.GetAsync($"executables/{id}", cancellationToken);
        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ExecutableDetail>(cancellationToken);
    }

    /// <summary>
    /// POST /executables/{executableId}/versions. Uploads a new version. Requires multipart form: file, entryPointDll. Returns the created version.
    /// </summary>
    public async Task<ExecutableVersion> UploadExecutableVersionAsync(int executableId, Stream fileStream, string fileName, string entryPointDll, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(entryPointDll), "entryPointDll");

        var res = await http.PostAsync($"executables/{executableId}/versions", content, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ExecutableVersion>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }

    /// <summary>
    /// GET /executables/{executableId}/versions/{versionId}/parameters. Returns the parameters for the given version.
    /// </summary>
    public async Task<List<ExecutableParameter>> GetExecutableVersionParametersAsync(int executableId, int versionId, CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<ExecutableParameter>>($"executables/{executableId}/versions/{versionId}/parameters", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// POST /executables/{executableId}/versions/{versionId}/activate. Sets the given version as active.
    /// </summary>
    public async Task ActivateExecutableVersionAsync(int executableId, int versionId, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsync($"executables/{executableId}/versions/{versionId}/activate", null, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /executables. Creates an executable. Returns the created executable on success.
    /// </summary>
    public async Task<Executable> CreateExecutableAsync(CreateExecutableRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("executables", request, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Executable>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }

    /// <summary>
    /// POST /jobs. Creates a job. Returns the created job on success.
    /// </summary>
    public async Task<AddJobResponse> AddJobAsync(AddJobRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("jobs", request, cancellationToken);

        res.EnsureSuccessStatusCode();

        var job = await res.Content.ReadFromJsonAsync<AddJobResponse>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");

        return job;
    }

    /// <summary>
    /// GET /schedules. Returns the list of schedules.
    /// </summary>
    public async Task<List<Schedule>> GetSchedulesAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Schedule>>("schedules", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// POST /schedules. Creates a schedule. Returns the created schedule on success.
    /// Throws <see cref="HttpRequestException"/> with the response body message on 400 (e.g. invalid cron).
    /// </summary>
    public async Task<Schedule> AddScheduleAsync(CreateScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("schedules", request, cancellationToken);

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = await res.Content.ReadFromJsonAsync<string>(cancellationToken)
                ?? await res.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(string.IsNullOrWhiteSpace(message) ? "Invalid request." : message);
        }

        res.EnsureSuccessStatusCode();

        var schedule = await res.Content.ReadFromJsonAsync<Schedule>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");

        return schedule;
    }
}
