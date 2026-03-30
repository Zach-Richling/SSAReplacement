using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client.Executables;

public class ExecutableEndpoints(HttpClient http)
{
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
    public async Task<ExecutableDetail?> GetExecutableAsync(long id, CancellationToken cancellationToken = default)
    {
        var res = await http.GetAsync($"executables/{id}", cancellationToken);

        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<ExecutableDetail>(cancellationToken);
    }

    /// <summary>
    /// POST /executables/{executableId}/versions. Uploads a new version. Requires multipart form: file, entryPointExe. Returns the created version.
    /// </summary>
    public async Task<ExecutableVersion> UploadExecutableVersionAsync(long executableId, Stream fileStream, string fileName, string entryPointExe, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(entryPointExe), "entryPointExe");

        var res = await http.PostAsync($"executables/{executableId}/versions", content, cancellationToken);
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<ExecutableVersion>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }

    /// <summary>
    /// GET /executables/{executableId}/versions/{versionId}/parameters. Returns the parameters for the given version.
    /// </summary>
    public async Task<List<ExecutableParameter>> GetExecutableVersionParametersAsync(long executableId, long versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await http.GetFromJsonAsync<List<ExecutableParameter>>($"executables/{executableId}/versions/{versionId}/parameters", cancellationToken);
            return list ?? [];
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }
    }

    /// <summary>
    /// POST /executables/{executableId}/versions/{versionId}/activate. Sets the given version as active.
    /// </summary>
    public async Task ActivateExecutableVersionAsync(long executableId, long versionId, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsync($"executables/{executableId}/versions/{versionId}/activate", null, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// POST /executables. Creates an executable. Returns the created executable on success.
    /// </summary>
    public async Task<Executable> CreateExecutableAsync(string name, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("executables", new { Name = name }, cancellationToken);
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<Executable>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }
}
