using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class UpdateJobSteps
{
    public record StepRequest(long? JobStepId, long ExecutableId, int StepNumber, string Name);
    public record Request(List<StepRequest> Steps);

    public static async Task<IResult> Handler(long id, Request request, AppDbContext db)
    {
        var job = await db.Jobs
            .Include(j => j.Steps)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return Results.NotFound();

        if (request.Steps.Count == 0)
            return Results.BadRequest("At least one step is required.");

        foreach (var step in request.Steps)
        {
            if (!await db.Executables.AnyAsync(e => e.Id == step.ExecutableId))
                return Results.NotFound($"Executable {step.ExecutableId} not found");
        }

        var requestedIds = request.Steps
            .Where(s => s.JobStepId.HasValue)
            .Select(s => s.JobStepId!.Value)
            .ToHashSet();

        var stepsToRemove = job.Steps.Where(s => !requestedIds.Contains(s.Id)).ToList();
        foreach (var step in stepsToRemove)
            job.Steps.Remove(step);

        foreach (var step in request.Steps)
        {
            var existing = step.JobStepId.HasValue
                ? job.Steps.FirstOrDefault(s => s.Id == step.JobStepId.Value)
                : null;

            if (existing is not null)
            {
                existing.ExecutableId = step.ExecutableId;
                existing.StepNumber = step.StepNumber;
                existing.Name = step.Name;
            }
            else
            {
                job.Steps.Add(new JobStep
                {
                    ExecutableId = step.ExecutableId,
                    StepNumber = step.StepNumber,
                    Name = step.Name
                });
            }
        }

        await db.SaveChangesAsync();

        var reloaded = await db.Jobs
            .AsNoTracking()
            .Include(j => j.Steps)
                .ThenInclude(s => s.Executable)
            .Include(j => j.Steps)
                .ThenInclude(s => s.Parameters)
            .AsSplitQuery()
            .FirstAsync(j => j.Id == id);

        return Results.Ok(reloaded.Steps.OrderBy(s => s.StepNumber).Select(JobStepDto.From));
    }
}
