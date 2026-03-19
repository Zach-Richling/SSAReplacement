using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJobParameters
{
    public record Request(Dictionary<string, string> Parameters);

    public static async Task<IResult> Handler(int id, Request request, AppDbContext db)
    {
        var job = await db.Jobs
            .Include(j => j.Variables)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return Results.NotFound();

        var executable = await db.ExecutableVersions
            .Include(ev => ev.Parameters)
            .Where(ev => ev.ExecutableId == job.ExecutableId && ev.IsActive)
            .AsNoTracking()
            .FirstAsync();

        job.Variables.Clear();
        foreach (var (key, value) in request.Parameters.Where(x => x.Value is not null))
        {
            var exeParam = executable.Parameters.FirstOrDefault(x => x.Name == key);
            if (exeParam?.DefaultValue == value)
            {
                continue;
            }

            job.Variables.Add(new JobVariable { Key = key, Value = value });
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
