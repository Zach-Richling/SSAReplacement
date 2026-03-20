using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class CreateJob
{
    public record Request(long ExecutableId, string Name, bool IsEnabled = true, string? NotifyEmail = null);

    public static async Task<IResult> Handler(Request req, AppDbContext db)
    {
        if (await db.Executables.FindAsync(req.ExecutableId) is null)
            return Results.NotFound("Executable not found");

        var job = new Job
        {
            ExecutableId = req.ExecutableId,
            Name = req.Name,
            IsEnabled = req.IsEnabled,
            NotifyEmail = req.NotifyEmail
        };

        db.Jobs.Add(job);

        await db.SaveChangesAsync();

        return Results.Created($"/jobs/{job.Id}", JobDto.From(job));
    }
}
