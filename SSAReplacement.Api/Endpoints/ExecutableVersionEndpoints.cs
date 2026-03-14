using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Loader;

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

            return Results.Ok(list.Select(v => ExecutableVersionDto.From(v)));
        });

        group.MapGet("/{versionNumber:int}/parameters", async (int executableId, int versionNumber, AppDbContext db) =>
        {
            var version = await db.ExecutableVersions
                .AsNoTracking()
                .Include(v => v.Parameters)
                .FirstOrDefaultAsync(v => v.ExecutableId == executableId && v.Version == versionNumber);

            if (version is null)
                return Results.NotFound();

            return Results.Ok(version.Parameters.Select(ExecutableParameterDto.From).ToList());
        });

        group.MapPost("/", async (int executableId, IFormFile file, [FromForm] string entryPointDll, AppDbContext db, IExecutableStorage storage) =>
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

            var isParsed = TryExtractExecutableParameters(Path.Combine(versionDir, version.EntryPointDll), out var parameters);

            if (!isParsed)
            {
                if (Directory.Exists(versionDir))
                    Directory.Delete(versionDir, true);

                return Results.InternalServerError();
            }

            version.Parameters = parameters;

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

    private static bool TryExtractExecutableParameters(string entryPointPath, out List<ExecutableParameter> parameters)
    {
        parameters = [];

        if (!File.Exists(entryPointPath))
            return false;

        var assemblyContext = new ExecutableLoadContext();

        try
        {
            var assembly = assemblyContext.LoadFromAssemblyPath(entryPointPath);
            var settingsType = assembly.DefinedTypes.Where(x => x.Name == "Settings").FirstOrDefault();

            if (settingsType is null)
                return true;

            var properties = settingsType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite);
            var settingsObject = Activator.CreateInstance(settingsType);

            foreach (var property in properties)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var isRequired = property.GetCustomAttributes().Any(attr => attr.GetType() == typeof(RequiredAttribute));
                var defaultValue = property.GetValue(settingsObject);
                var description = property.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(DescriptionAttribute));

                parameters.Add(new ExecutableParameter()
                {
                    Name = property.Name,
                    Description = description is DescriptionAttribute desc ? desc.Description : null,
                    TypeName = type.Name,
                    Required = isRequired,
                    DefaultValue = defaultValue?.ToString()
                });
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            assemblyContext.Unload();
            assemblyContext = null;
        }
    }

    private class ExecutableLoadContext : AssemblyLoadContext
    {
        public ExecutableLoadContext() : base(isCollectible: true) { }
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
