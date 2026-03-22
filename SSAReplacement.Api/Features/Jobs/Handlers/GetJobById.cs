using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobById
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var job = await db.Jobs
            .AsNoTracking()
            .Include(j => j.Steps.OrderBy(s => s.StepNumber))
                .ThenInclude(s => s.Executable)
            .Include(j => j.Steps)
                .ThenInclude(s => s.Parameters)
            .Include(j => j.JobSchedules)
                .ThenInclude(js => js.Schedule)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

        return job is null ? Results.NotFound() : Results.Ok(JobDetailDto.From(job));
    }
}
