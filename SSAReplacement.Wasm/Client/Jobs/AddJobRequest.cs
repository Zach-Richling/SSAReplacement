namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// A step in a job creation/update request.
/// </summary>
public record JobStepRequest(long ExecutableId, int StepNumber, string Name);

/// <summary>
/// Request body for POST /jobs (create job).
/// </summary>
public record AddJobRequest(
    string Name,
    List<JobStepRequest> Steps,
    bool IsEnabled = true,
    string? NotifyEmail = null);
