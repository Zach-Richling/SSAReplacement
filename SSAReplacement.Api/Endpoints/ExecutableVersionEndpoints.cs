using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;

namespace SSAReplacement.Api.Endpoints;

public static class ExecutableVersionEndpoints
{
    public static void MapExecutableVersionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/executables/{executableId:int}/versions")
            .WithTags("Executable Versions");

        group.MapGet("/", async (int executableId, AppDbContext db) =>
        {
            var list = await db.ExecutableVersions
                .AsNoTracking()
                .Where(v => v.ExecutableId == executableId)
                .OrderByDescending(v => v.UploadedAt)
                .ToListAsync();

            return Results.Ok(list.Select(v => ExecutableVersionDto.From(v, includePath: false)));
        });

        group.MapPost("/", async (int executableId, IFormFile file, [FromForm] string entryPointDll, AppDbContext db, IExecutableStorage storage) =>
        {
            if (await db.Executables.FindAsync(executableId) is null)
                return Results.NotFound("Executable not found");

            var versionNumber = await db.ExecutableVersions.CountAsync(v => v.ExecutableId == executableId) + 1;
            var version = new ExecutableVersion
            {
                ExecutableId = executableId,
                Version = versionNumber.ToString(),
                Path = "",
                EntryPointDll = entryPointDll.Trim(),
                IsActive = false
            };

            db.ExecutableVersions.Add(version);

            await using var stream = file.OpenReadStream();
            var versionDir = await storage.SaveVersionAsync(executableId, versionNumber, stream);
            version.Path = versionDir;

            await db.SaveChangesAsync();

            return Results.Created($"/executables/{executableId}/versions/{version.Id}", ExecutableVersionDto.From(version));
        }).DisableAntiforgery();

        group.MapPost("/{versionId:int}/activate", async (int executableId, int versionId, AppDbContext db) =>
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
        });
    }
}
