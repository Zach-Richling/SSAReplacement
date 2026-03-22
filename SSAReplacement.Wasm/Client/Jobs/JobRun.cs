namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job run (matches API JobRunDto from GET /jobs/{id}/runs).
/// </summary>
public record JobRun
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long? ScheduleId { get; set; }
    public int? CurrentStep { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string Status { get; set; } = "";
    public int? ExitCode { get; set; }
    public string? Trigger { get; set; }
    public List<JobRunStep> RunSteps { get; set; } = [];
}
