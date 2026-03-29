namespace SSAReplacement.Api.Domain;

public class ExecutableVersion : IAuditable
{
    public long Id { get; set; }
    public long ExecutableId { get; set; }
    public long? CreatedByUserId { get; set; }
    public int Version { get; set; }
    public string EntryPointDll { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }

    public Executable Executable { get; set; } = null!;
    public ICollection<JobRunStep> JobRunSteps { get; set; } = [];
    public ICollection<ExecutableParameter> Parameters { get; set; } = [];
}
