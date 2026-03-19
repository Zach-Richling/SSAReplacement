using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class DeleteJob
{
    public static async Task<IResult> Handler(int id, AppDbContext db)
    {
        var j = await db.Jobs.FindAsync(id);

        if (j is null)
            return Results.NotFound();

        db.Jobs.Remove(j);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
