namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// A step within a job run (matches API JobRunStepDto).
/// </summary>
public class JobRunStep
{
    public long Id { get; set; }
    public long JobRunId { get; set; }
    public long ExecutableVersionId { get; set; }
    public int StepNumber { get; set; }
    public string StepName { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string Status { get; set; } = "";
    public int? ExitCode { get; set; }
}
