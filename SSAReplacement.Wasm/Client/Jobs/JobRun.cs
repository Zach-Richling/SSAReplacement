namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job run (matches API JobRunDto from GET /jobs/{id}/runs).
/// </summary>
public record JobRun
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int ExecutableVersionId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string Status { get; set; } = "";
    public int? ExitCode { get; set; }
    public string? Trigger { get; set; }
}
