namespace SSAReplacement.Api.Domain;

public class ExecutableVersion : IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public long ExecutableId { get; set; }
    public long? CreatedByUserId { get; set; }
    public int Version { get; set; }
    public string EntryPointExe { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public byte RecStatus { get; set; } = 1;
    public long? DeletedByUserId { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Executable Executable { get; set; } = null!;
    public ICollection<JobRunStep> JobRunSteps { get; set; } = [];
    public ICollection<ExecutableParameter> Parameters { get; set; } = [];
}
