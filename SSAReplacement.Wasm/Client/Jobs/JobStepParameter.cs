namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// A step-level parameter (matches API JobStepParameterDto).
/// </summary>
public class JobStepParameter
{
    public long Id { get; set; }
    public long JobStepId { get; set; }
    public string Key { get; set; } = "";
    public string? Value { get; set; }
}
