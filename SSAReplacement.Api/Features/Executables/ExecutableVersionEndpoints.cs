using SSAReplacement.Api.Features.Executables.Handlers;

namespace SSAReplacement.Api.Features.Executables;

public static class ExecutableVersionEndpoints
{
    public static void MapExecutableVersionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/executables/{executableId:long}/versions")
            .WithTags("Executable Versions");

        group.MapGet("/", GetExecutableVersions.Handler);
        group.MapGet("/{versionNumber:int}/parameters", GetVersionParameters.Handler);
        group.MapPost("/", UploadExecutableVersion.Handler).DisableAntiforgery();
        group.MapPost("/{versionId:long}/activate", ActivateExecutableVersion.Handler);
    }
}
