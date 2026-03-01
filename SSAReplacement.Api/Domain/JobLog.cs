namespace SSAReplacement.Api.Domain;

public class JobLog
{
    public int Id { get; set; }
    public int JobRunId { get; set; }
    public required string LogType { get; set; } // StdOut, StdErr
    public required string Content { get; set; }

    public JobRun JobRun { get; set; } = null!;
}
