using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class DeleteExecutable
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var exe = await db.Executables.FindAsync(id);

        if (exe is null)
            return Results.NotFound();

        if (await db.JobSteps.AnyAsync(s => s.ExecutableId == id))
            return Results.Conflict("Cannot delete executable that is referenced by one or more job steps.");

        db.Executables.Remove(exe);

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
