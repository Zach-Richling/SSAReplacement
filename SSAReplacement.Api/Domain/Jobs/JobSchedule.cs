namespace SSAReplacement.Api.Domain;

public class JobSchedule : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long? CreatedByUserId { get; set; }
    public long ScheduleId { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Job Job { get; set; } = null!;
    public Schedule Schedule { get; set; } = null!;
}
