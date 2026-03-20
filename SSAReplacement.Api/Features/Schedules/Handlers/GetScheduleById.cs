using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Schedules.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Handlers;

public static class GetScheduleById
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var s = await db.Schedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return s is null ? Results.NotFound() : Results.Ok(ScheduleDto.From(s));
    }
}
