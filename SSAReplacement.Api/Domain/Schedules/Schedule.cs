namespace SSAReplacement.Api.Domain;

public class Schedule
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public required string CronExpression { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobSchedule> JobSchedules { get; set; } = [];
    public ICollection<JobRun> JobRuns { get; set; } = [];
}
