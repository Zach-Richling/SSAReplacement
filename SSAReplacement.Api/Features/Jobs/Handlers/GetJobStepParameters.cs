using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobStepParameters
{
    public static async Task<IResult> Handler(long id, long stepId, AppDbContext db)
    {
        var step = await db.JobSteps
            .Include(s => s.Parameters)
            .FirstOrDefaultAsync(s => s.Id == stepId && s.JobId == id);

        if (step is null)
            return Results.NotFound();

        return Results.Ok(step.Parameters.Select(JobStepParameterDto.From));
    }
}
