namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable parameter (matches API ExecutableParameterDto from GET executable version parameters).
/// </summary>
public record ExecutableParameter(long Id, int ExecutableVersionId, string Name, string TypeName, string? Description, bool Required, string? DefaultValue);
