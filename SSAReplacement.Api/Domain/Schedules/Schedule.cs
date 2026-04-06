namespace SSAReplacement.Api.Domain;

public class Schedule : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public long? CreatedByUserId { get; set; }
    public required string CronExpression { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public ICollection<JobSchedule> JobSchedules { get; set; } = [];
    public ICollection<JobRun> JobRuns { get; set; } = [];
}
