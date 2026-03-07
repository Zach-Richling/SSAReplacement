namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable detail with versions (matches API ExecutableDetailDto from GET /executables/{id}).
/// </summary>
public record ExecutableDetail(int Id, string Name, DateTime CreatedAt, IReadOnlyList<ExecutableVersion> Versions);
