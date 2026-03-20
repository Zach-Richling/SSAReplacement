using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobParameters
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var job = await db.Jobs
            .Include(j => j.Variables)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return Results.NotFound();

        return Results.Ok(job.Variables.Select(JobVariableDto.From));
    }
}
