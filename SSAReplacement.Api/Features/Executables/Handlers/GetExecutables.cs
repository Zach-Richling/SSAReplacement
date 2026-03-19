using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class GetExecutables
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var list = await db.Executables
            .Include(e => e.Versions)
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync();

        return Results.Ok(list.Select(ExecutableDto.From));
    }
}
