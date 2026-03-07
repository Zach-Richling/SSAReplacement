namespace SSAReplacement.Wasm.Client.Jobs;

public class JobLog
{
    public int Id { get; set; }
    public int JobRunId { get; set; }
    public DateTime LogDate { get; set; }
    public string LogType { get; set; } = "";
    public string Content { get; set; } = "";
};
