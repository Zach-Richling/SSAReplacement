using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.Executables.Domain;

public record ExecutableDto(int Id, string Name, DateTime CreatedAt, int ActiveVersion)
{
    public static ExecutableDto From(Executable e) => new(
        e.Id, e.Name, e.CreatedAt,
        e.Versions.FirstOrDefault(v => v.IsActive)?.Version ?? 0);
}

public record ExecutableDetailDto(int Id, string? Name, DateTime CreatedAt, IReadOnlyList<ExecutableVersionDto> Versions)
{
    public static ExecutableDetailDto From(Executable e) => new(
        e.Id, e.Name, e.CreatedAt,
        e.Versions?.Select(ExecutableVersionDto.From).ToList() ?? []);
}
