using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJob
{
    public record Request(int? ExecutableId, string? Name, bool? IsEnabled, string? NotifyEmail = null);

    public static async Task<IResult> Handler(int id, Request req, AppDbContext db)
    {
        var j = await db.Jobs.FindAsync(id);

        if (j is null)
            return Results.NotFound();

        if (req.ExecutableId is int eid) j.ExecutableId = eid;
        if (req.Name is not null) j.Name = req.Name;
        if (req.IsEnabled is bool en) j.IsEnabled = en;
        if (req.NotifyEmail is not null) j.NotifyEmail = req.NotifyEmail;

        await db.SaveChangesAsync();

        return Results.Ok(JobDto.From(j));
    }
}
