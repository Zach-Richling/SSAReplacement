namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job variable (matches API JobVariableDto in JobDetailDto).
/// </summary>
public class JobParameter
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
}
