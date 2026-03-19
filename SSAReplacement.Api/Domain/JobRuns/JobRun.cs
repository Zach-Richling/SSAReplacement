namespace SSAReplacement.Api.Domain;

public class JobRun
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int ExecutableVersionId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public required string Status { get; set; } // Running, Success, Failed
    public int? ExitCode { get; set; }
    public string? Trigger { get; set; } // Scheduled, Manual

    public Job Job { get; set; } = null!;
    public ExecutableVersion ExecutableVersion { get; set; } = null!;
    public Schedule? Schedule { get; set; }
    public ICollection<JobLog> Logs { get; set; } = [];
}
