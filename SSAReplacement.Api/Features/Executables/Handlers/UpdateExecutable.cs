using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class UpdateExecutable
{
    public record Request(string Name);

    public static async Task<IResult> Handler(long id, Request req, AppDbContext db)
    {
        var exe = await db.Executables.FindAsync(id);

        if (exe is null)
            return Results.NotFound();

        if (req.Name is not null)
            exe.Name = req.Name;

        await db.SaveChangesAsync();

        return Results.Ok(ExecutableDto.From(exe));
    }
}
