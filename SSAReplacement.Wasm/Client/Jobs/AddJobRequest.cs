namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Request body for POST /jobs (create job).
/// </summary>
public record AddJobRequest(
    int ExecutableId,
    string Name,
    bool IsEnabled = true,
    string? NotifyEmail = null);
