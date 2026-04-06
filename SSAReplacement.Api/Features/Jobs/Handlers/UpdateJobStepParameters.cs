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

        var requestedKeys = request.Parameters
            .Where(x => x.Value is not null)
            .Select(x => x.Key)
            .ToHashSet();

        var paramsToRemove = step.Parameters.Where(p => !requestedKeys.Contains(p.Key)).ToList();
        foreach (var p in paramsToRemove)
            step.Parameters.Remove(p);

        foreach (var (key, value) in request.Parameters.Where(x => x.Value is not null))
        {
            var existing = step.Parameters.FirstOrDefault(p => p.Key == key);
            if (existing is not null)
                existing.Value = value;
            else
                step.Parameters.Add(new JobStepParameter { Key = key, Value = value });
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
