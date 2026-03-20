using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.Executables.Domain;

public record ExecutableVersionDto(long Id, long ExecutableId, int Version, string EntryPointDll, DateTime UploadedAt, bool IsActive)
{
    public static ExecutableVersionDto From(ExecutableVersion v) => new(
        v.Id, v.ExecutableId, v.Version, v.EntryPointDll, v.UploadedAt, v.IsActive);
}
