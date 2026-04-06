namespace SSAReplacement.Api.Domain;

public class JobStep : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long? CreatedByUserId { get; set; }
    public long ExecutableId { get; set; }
    public int StepNumber { get; set; }
    public required string Name { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Job Job { get; set; } = null!;
    public Executable Executable { get; set; } = null!;
    public ICollection<JobStepParameter> Parameters { get; set; } = [];
}
