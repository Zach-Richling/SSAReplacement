namespace SSAReplacement.Wasm.Client.Jobs;

public record UpdateJobRequest(
    string Name,
    bool IsEnabled,
    string? NotifyEmail);
