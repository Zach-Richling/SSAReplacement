using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Jobs.Handlers;

public static class GetJobs
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var list = await db.Jobs
            .AsNoTracking()
            .OrderBy(j => j.Id)
            .ToListAsync();

        return Results.Ok(list.Select(JobDto.From));
    }
}
