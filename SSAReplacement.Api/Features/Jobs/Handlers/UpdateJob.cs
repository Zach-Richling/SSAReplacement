using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJob
{
    public record Request(string? Name, bool? IsEnabled, string? NotifyEmail = null);

    public static async Task<IResult> Handler(long id, Request req, AppDbContext db)
    {
        var j = await db.Jobs.FindAsync(id);

        if (j is null)
            return Results.NotFound();

        if (req.Name is not null) j.Name = req.Name;
        if (req.IsEnabled is bool en) j.IsEnabled = en;
        if (req.NotifyEmail is not null) j.NotifyEmail = req.NotifyEmail;

        await db.SaveChangesAsync();

        var reloaded = await db.Jobs
            .AsNoTracking()
            .Include(x => x.JobSchedules)
                .ThenInclude(js => js.Schedule)
            .FirstAsync(x => x.Id == id);

        return Results.Ok(JobDto.From(reloaded));
    }
}
