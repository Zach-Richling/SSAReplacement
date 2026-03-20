using SSAReplacement.Api.Features.Executables.Handlers;

namespace SSAReplacement.Api.Features.Executables;

public static class ExecutableEndpoints
{
    public static void MapExecutableEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/executables").WithTags("Executables");

        group.MapGet("/", GetExecutables.Handler);
        group.MapGet("/{id:long}", GetExecutableById.Handler);
        group.MapPost("/", CreateExecutable.Handler);
        group.MapPut("/{id:long}", UpdateExecutable.Handler);
        group.MapDelete("/{id:long}", DeleteExecutable.Handler);
    }
}
