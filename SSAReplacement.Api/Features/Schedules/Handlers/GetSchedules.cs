using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Handlers;

public static class GetSchedules
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var list = await db.Schedules.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
        return Results.Ok(list.Select(ScheduleDto.From));
    }
}
