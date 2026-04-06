namespace SSAReplacement.Api.Domain;

public class Executable : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? CreatedByUserId { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public ICollection<ExecutableVersion> Versions { get; set; } = [];
    public ICollection<JobStep> JobSteps { get; set; } = [];
}
