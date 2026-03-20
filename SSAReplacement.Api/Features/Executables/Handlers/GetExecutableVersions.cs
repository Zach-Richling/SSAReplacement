using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class GetExecutableVersions
{
    public static async Task<IResult> Handler(long executableId, AppDbContext db)
    {
        var list = await db.ExecutableVersions
            .AsNoTracking()
            .Where(v => v.ExecutableId == executableId)
            .OrderByDescending(v => v.UploadedAt)
            .ToListAsync();

        return Results.Ok(list.Select(v => ExecutableVersionDto.From(v)));
    }
}
