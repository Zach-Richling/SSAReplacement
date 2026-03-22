namespace SSAReplacement.Api.Domain;

public class JobLog
{
    public long Id { get; set; }
    public long JobRunStepId { get; set; }
    public DateTime LogDate { get; set; }
    public required string LogType { get; set; }
    public required string Content { get; set; }

    public JobRunStep JobRunStep { get; set; } = null!;
}
