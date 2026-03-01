using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record ExecutableDto(int Id, string? Name, DateTime CreatedAt)
{
    public static ExecutableDto From(Executable e) => new(e.Id, e.Name, e.CreatedAt);
}

public record ExecutableDetailDto(int Id, string? Name, DateTime CreatedAt, IReadOnlyList<ExecutableVersionDto> Versions)
{
    public static ExecutableDetailDto From(Executable e) => new(
        e.Id, e.Name, e.CreatedAt,
        e.Versions.Select(v => ExecutableVersionDto.From(v)).ToList());
}
