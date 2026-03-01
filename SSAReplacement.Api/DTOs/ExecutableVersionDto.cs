using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record ExecutableVersionDto(int Id, int ExecutableId, string Version, string? Path, string EntryPointDll, DateTime UploadedAt, bool IsActive)
{
    public static ExecutableVersionDto From(ExecutableVersion v, bool includePath = true) => new(
        v.Id, v.ExecutableId, v.Version, includePath ? v.Path : null, v.EntryPointDll, v.UploadedAt, v.IsActive);
}
