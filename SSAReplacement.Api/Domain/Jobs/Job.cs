namespace SSAReplacement.Api.Domain;

public class Job
{
    public int Id { get; set; }
    public int ExecutableId { get; set; }
    public required string Name { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? NotifyEmail { get; set; }

    public Executable Executable { get; set; } = null!;
    public ICollection<JobSchedule> JobSchedules { get; set; } = [];
    public ICollection<JobVariable> Variables { get; set; } = [];
    public ICollection<JobRun> Runs { get; set; } = [];
}
