namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable list item (matches API ExecutableDto from GET /executables).
/// </summary>
public class Executable
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int ActiveVersion { get; set; }
};
