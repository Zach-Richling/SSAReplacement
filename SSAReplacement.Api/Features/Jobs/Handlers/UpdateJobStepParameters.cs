using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJobStepParameters
{
    public record Request(Dictionary<string, string> Parameters);

    public static async Task<IResult> Handler(long id, long stepId, Request request, AppDbContext db)
    {
        var step = await db.JobSteps
            .Include(s => s.Parameters)
            .FirstOrDefaultAsync(s => s.Id == stepId && s.JobId == id);

        if (step is null)
            return Results.NotFound();

        step.Parameters.Clear();
        foreach (var (key, value) in request.Parameters.Where(x => x.Value is not null))
        {
            step.Parameters.Add(new JobStepParameter { Key = key, Value = value });
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
