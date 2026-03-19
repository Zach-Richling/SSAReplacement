using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.Executables.Domain;

public record ExecutableParameterDto(long Id, int ExecutableVersionId, string Name, string TypeName, string? Description, bool Required, string? DefaultValue)
{
    public static ExecutableParameterDto From(ExecutableParameter p) => new(
        p.Id,
        p.ExecutableVersionId,
        p.Name,
        p.TypeName,
        p.Description,
        p.Required,
        p.DefaultValue);
}
