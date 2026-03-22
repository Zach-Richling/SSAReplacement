namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// A step within a job definition (matches API JobStepDto).
/// </summary>
public class JobStep
{
    public long Id { get; set; }
    public long ExecutableId { get; set; }
    public string ExecutableName { get; set; } = "";
    public int StepNumber { get; set; }
    public string Name { get; set; } = "";
    public List<JobStepParameter> Parameters { get; set; } = [];
}
