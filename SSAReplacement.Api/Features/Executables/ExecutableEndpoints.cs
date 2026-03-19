using SSAReplacement.Api.Features.Executables.Handlers;

namespace SSAReplacement.Api.Features.Executables;

public static class ExecutableEndpoints
{
    public static void MapExecutableEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/executables").WithTags("Executables");

        group.MapGet("/", GetExecutables.Handler);
        group.MapGet("/{id:int}", GetExecutableById.Handler);
        group.MapPost("/", CreateExecutable.Handler);
        group.MapPut("/{id:int}", UpdateExecutable.Handler);
        group.MapDelete("/{id:int}", DeleteExecutable.Handler);
    }
}
