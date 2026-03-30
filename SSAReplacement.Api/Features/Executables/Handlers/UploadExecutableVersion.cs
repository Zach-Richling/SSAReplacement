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
        [FromForm] string entryPointExe,
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
            EntryPointExe = entryPointExe.Trim(),
            IsActive = false
        };

        db.ExecutableVersions.Add(version);

        await using var stream = file.OpenReadStream();
        var versionDir = await storage.SaveVersionAsync(executableId, versionNumber, stream);
        var exePath = Path.Combine(versionDir, version.EntryPointExe);

        if (!File.Exists(exePath))
        {
            if (Directory.Exists(versionDir))
                Directory.Delete(versionDir, true);

            return Results.BadRequest("Entrypoint exe is not in the uploaded zip.");
        }

        var dllName = Path.ChangeExtension(version.EntryPointExe, ".dll");
        var dllPath = Path.Combine(versionDir, dllName);

        if (!File.Exists(dllPath))
        {
            if (Directory.Exists(versionDir))
                Directory.Delete(versionDir, true);

            return Results.BadRequest($"A DLL with the same name as the executable ({dllName}) must be present in the zip for parameter scanning.");
        }

        var isParsed = ExecutableLoadContext.TryExtractExecutableParameters(dllPath, out var parameters);

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
