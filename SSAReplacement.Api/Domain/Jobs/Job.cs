namespace SSAReplacement.Api.Domain;

public class Job : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? NotifyEmail { get; set; }
    public long? CreatedByUserId { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public ICollection<JobStep> Steps { get; set; } = [];
    public ICollection<JobSchedule> JobSchedules { get; set; } = [];
    public ICollection<JobRun> Runs { get; set; } = [];
}
