namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Executable version (matches API ExecutableVersionDto from GET executable detail / versions).
/// </summary>
public record ExecutableVersion(int Id, int ExecutableId, int Version, string EntryPointDll, DateTime UploadedAt, bool IsActive);
