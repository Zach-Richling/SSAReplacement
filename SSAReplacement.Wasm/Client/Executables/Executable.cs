namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable list item (matches API ExecutableDto from GET /executables).
/// </summary>
public record Executable(int Id, string Name, DateTime CreatedAt, string? ActiveVersion);
