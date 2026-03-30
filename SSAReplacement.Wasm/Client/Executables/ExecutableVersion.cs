namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable version (matches API ExecutableVersionDto from GET executable detail / versions).
/// </summary>
public record ExecutableVersion
{
    public long Id { get; set; }
    public long ExecutableId { get; set; }
    public int Version { get; set; }
    public string EntryPointExe { get; set; } = "";
    public DateTime UploadedAt { get; set; }
    public bool IsActive { get; set; }
}
