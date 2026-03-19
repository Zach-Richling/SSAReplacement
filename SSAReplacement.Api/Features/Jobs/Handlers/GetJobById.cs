using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobById
{
    public static async Task<IResult> Handler(int id, AppDbContext db)
    {
        var job = await db.Jobs
            .AsNoTracking()
            .Include(j => j.Executable)
                .ThenInclude(ex => ex.Versions)
            .Include(j => j.Variables)
            .Include(j => j.JobSchedules)
                .ThenInclude(js => js.Schedule)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

        return job is null ? Results.NotFound() : Results.Ok(JobDetailDto.From(job));
    }
}
