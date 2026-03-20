namespace SSAReplacement.Api.Domain;

public class JobLog
{
    public long Id { get; set; }
    public long JobRunId { get; set; }
    public DateTime LogDate { get; set; }
    public required string LogType { get; set; }
    public required string Content { get; set; }

    public JobRun JobRun { get; set; } = null!;
}
