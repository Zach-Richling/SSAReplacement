using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class CreateExecutable
{
    public record Request(string Name);

    public static async Task<IResult> Handler(Request req, AppDbContext db)
    {
        var exe = new Executable { Name = req.Name };
        db.Executables.Add(exe);

        await db.SaveChangesAsync();

        return Results.Created($"/executables/{exe.Id}", ExecutableDto.From(exe));
    }
}
