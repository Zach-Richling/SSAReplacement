using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class GetExecutableById
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var exe = await db.Executables
            .AsNoTracking()
            .Include(e => e.Versions.OrderByDescending(v => v.UploadedAt))
            .FirstOrDefaultAsync(e => e.Id == id);

        return exe is null ? Results.NotFound() : Results.Ok(ExecutableDetailDto.From(exe));
    }
}
