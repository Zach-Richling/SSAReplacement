namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable version (matches API ExecutableVersionDto from GET executable detail / versions).
/// </summary>
public record ExecutableVersion
{
    public int Id { get; set; }
    public int ExecutableId { get; set; }
    public int Version { get; set; }
    public string EntryPointDll { get; set; } = "";
    public DateTime UploadedAt { get; set; }
    public bool IsActive { get; set; }
}
