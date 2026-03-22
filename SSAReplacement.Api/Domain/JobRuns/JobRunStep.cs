namespace SSAReplacement.Api.Domain;

public class JobRunStep
{
    public long Id { get; set; }
    public long JobRunId { get; set; }
    public long ExecutableVersionId { get; set; }
    public int StepNumber { get; set; }
    public string StepName { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public required string Status { get; set; }
    public int? ExitCode { get; set; }

    public JobRun JobRun { get; set; } = null!;
    public ExecutableVersion ExecutableVersion { get; set; } = null!;
    public ICollection<JobLog> Logs { get; set; } = [];
}
