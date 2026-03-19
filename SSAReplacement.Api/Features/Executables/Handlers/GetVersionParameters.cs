using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class GetVersionParameters
{
    public static async Task<IResult> Handler(int executableId, int versionNumber, AppDbContext db)
    {
        var version = await db.ExecutableVersions
            .AsNoTracking()
            .Include(v => v.Parameters)
            .FirstOrDefaultAsync(v => v.ExecutableId == executableId && v.Version == versionNumber);

        if (version is null)
            return Results.NotFound();

        return Results.Ok(version.Parameters.Select(ExecutableParameterDto.From).ToList());
    }
}
