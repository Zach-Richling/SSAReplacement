namespace SSAReplacement.Wasm.Client.Jobs;

public class JobLog
{
    public long Id { get; set; }
    public long JobRunStepId { get; set; }
    public DateTime LogDate { get; set; }
    public string LogType { get; set; } = "";
    public string Content { get; set; } = "";
};
