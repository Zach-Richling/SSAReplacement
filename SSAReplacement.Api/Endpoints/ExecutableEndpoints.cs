using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Endpoints;

public static class ExecutableEndpoints
{
    public static void MapExecutableEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/executables").WithTags("Executables");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var list = await db.Executables.AsNoTracking().OrderBy(e => e.Id).ToListAsync();

            return Results.Ok(list.Select(ExecutableDto.From));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var exe = await db.Executables
                .AsNoTracking()
                .Include(e => e.Versions.OrderByDescending(v => v.UploadedAt))
                .FirstOrDefaultAsync(e => e.Id == id);

            return exe is null ? Results.NotFound() : Results.Ok(ExecutableDetailDto.From(exe));
        });

        group.MapPost("/", async (CreateExecutableRequest req, AppDbContext db) =>
        {
            var exe = new Executable { Name = req.Name };
            db.Executables.Add(exe);

            await db.SaveChangesAsync();

            return Results.Created($"/executables/{exe.Id}", ExecutableDto.From(exe));
        });

        group.MapPut("/{id:int}", async (int id, UpdateExecutableRequest req, AppDbContext db) =>
        {
            var exe = await db.Executables.FindAsync(id);

            if (exe is null)
                return Results.NotFound();

            if (req.Name is not null)
                exe.Name = req.Name;

            await db.SaveChangesAsync();

            return Results.Ok(ExecutableDto.From(exe));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var exe = await db.Executables.FindAsync(id);

            if (exe is null)
                return Results.NotFound();

            if (await db.Jobs.AnyAsync(j => j.ExecutableId == id))
                return Results.Conflict("Cannot delete executable that is referenced by one or more jobs.");

            db.Executables.Remove(exe);

            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    public record CreateExecutableRequest(string? Name);
    public record UpdateExecutableRequest(string? Name);
}
