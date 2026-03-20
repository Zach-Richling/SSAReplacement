using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Executables.Domain;
using SSAReplacement.Api.Features.Executables.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Executables.Handlers;

public static class UploadExecutableVersion
{
    public static async Task<IResult> Handler(
        long executableId,
        IFormFile file,
        [FromForm] string entryPointDll,
        AppDbContext db,
        IExecutableStorage storage)
    {
        if (await db.Executables.FindAsync(executableId) is null)
            return Results.NotFound("Executable not found");

        var versionNumber = await db.ExecutableVersions.CountAsync(v => v.ExecutableId == executableId) + 1;
        var version = new ExecutableVersion
        {
            ExecutableId = executableId,
            Version = versionNumber,
            EntryPointDll = entryPointDll.Trim(),
            IsActive = false
        };

        db.ExecutableVersions.Add(version);

        await using var stream = file.OpenReadStream();
        var versionDir = await storage.SaveVersionAsync(executableId, versionNumber, stream);
        var entryPointPath = Path.Combine(versionDir, version.EntryPointDll);

        if (!File.Exists(entryPointPath))
        {
            return Results.BadRequest("Entrypoint Dll is not in the uploaded zip.");
        }

        var isParsed = ExecutableLoadContext.TryExtractExecutableParameters(entryPointPath, out var parameters);

        if (!isParsed)
        {
            if (Directory.Exists(versionDir))
                Directory.Delete(versionDir, true);

            return Results.InternalServerError();
        }

        version.Parameters = parameters;

        await db.SaveChangesAsync();

        return Results.Created($"/executables/{executableId}/versions/{version.Id}", ExecutableVersionDto.From(version));
    }
}
