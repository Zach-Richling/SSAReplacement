namespace SSAReplacement.Api.Domain;

public class ExecutableVersion
{
    public int Id { get; set; }
    public int ExecutableId { get; set; }
    public int Version { get; set; }
    public string EntryPointDll { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }

    public Executable Executable { get; set; } = null!;
    public ICollection<JobRun> JobRuns { get; set; } = [];
    public ICollection<ExecutableParameter> Parameters { get; set; } = [];
}
