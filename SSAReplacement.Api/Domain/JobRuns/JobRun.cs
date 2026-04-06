namespace SSAReplacement.Api.Domain;

public class JobRun : ISoftDeletable
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long? CreatedByUserId { get; set; }
    public long? ScheduleId { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }
    public int? CurrentStep { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public required string Status { get; set; }
    public int? ExitCode { get; set; }
    public string? Trigger { get; set; }

    public Job Job { get; set; } = null!;
    public Schedule? Schedule { get; set; }
    public ICollection<JobRunStep> RunSteps { get; set; } = [];
}
