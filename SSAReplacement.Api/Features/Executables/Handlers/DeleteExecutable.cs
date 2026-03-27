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
            return Results.Problem(
                detail: "Cannot delete executable that is referenced by one or more job steps.",
                statusCode: StatusCodes.Status409Conflict,
                title: "EXECUTABLE_IN_USE");

        db.Executables.Remove(exe);

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
