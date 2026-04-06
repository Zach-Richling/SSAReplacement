namespace SSAReplacement.Api.Domain;

public class JobStepParameter : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public long JobStepId { get; set; }
    public long? CreatedByUserId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public JobStep JobStep { get; set; } = null!;
}
