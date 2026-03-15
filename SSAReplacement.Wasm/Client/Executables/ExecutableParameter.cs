namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable parameter (matches API ExecutableParameterDto from GET executable version parameters).
/// </summary>
public class ExecutableParameter
{
    public long Id { get; set; }
    public int ExecutableVersionId { get; set; }
    public string Name { get; set; } = "";
    public string TypeName { get; set; } = "";
    public string? Description { get; set; }
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
}
