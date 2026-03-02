namespace SSAReplacement.Web.Client;

/// <summary>
/// Executable version (matches API ExecutableVersionDto from GET executable detail / versions).
/// </summary>
public record ExecutableVersion(int Id, int ExecutableId, string Version, string? Path, string EntryPointDll, DateTime UploadedAt, bool IsActive);
