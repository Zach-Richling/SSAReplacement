namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable detail with versions (matches API ExecutableDetailDto from GET /executables/{id}).
/// </summary>
public class ExecutableDetail
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ExecutableVersion> Versions { get; set; } = [];
}
