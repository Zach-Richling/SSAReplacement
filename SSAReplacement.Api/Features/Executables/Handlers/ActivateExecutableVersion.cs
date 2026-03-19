using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class ActivateExecutableVersion
{
    public static async Task<IResult> Handler(int executableId, int versionId, AppDbContext db)
    {
        var version = await db.ExecutableVersions.FirstOrDefaultAsync(v => v.ExecutableId == executableId && v.Id == versionId);

        if (version is null)
            return Results.NotFound();

        await db.ExecutableVersions
            .Where(v => v.ExecutableId == executableId)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.IsActive, false));

        version.IsActive = true;

        await db.SaveChangesAsync();

        return Results.Ok(ExecutableVersionDto.From(version));
    }
}
