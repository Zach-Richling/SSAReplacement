using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;

namespace SSAReplacement.Api.Endpoints;

public static class ScheduleEndpoints
{
    public static void MapScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/schedules").WithTags("Schedules");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var list = await db.Schedules.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
            return Results.Ok(list.Select(ScheduleDto.From));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var s = await db.Schedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return s is null ? Results.NotFound() : Results.Ok(ScheduleDto.From(s));
        });

        group.MapPost("/", async (CreateScheduleRequest req, AppDbContext db, IScheduleHangfireSyncService sync) =>
        {
            var s = new Schedule
            {
                Name = req.Name,
                CronExpression = req.CronExpression,
                IsEnabled = req.IsEnabled
            };
            db.Schedules.Add(s);
            await db.SaveChangesAsync();
            await sync.AddOrUpdateScheduleAsync(s.Id, s.CronExpression, s.IsEnabled);
            return Results.Created($"/schedules/{s.Id}", ScheduleDto.From(s));
        });

        group.MapPut("/{id:int}", async (int id, UpdateScheduleRequest req, AppDbContext db, IScheduleHangfireSyncService sync) =>
        {
            var s = await db.Schedules.FindAsync(id);
            if (s is null) return Results.NotFound();
            if (req.Name is not null) s.Name = req.Name;
            if (req.CronExpression is not null) s.CronExpression = req.CronExpression;
            if (req.IsEnabled is { } en) s.IsEnabled = en;
            await db.SaveChangesAsync();
            await sync.AddOrUpdateScheduleAsync(s.Id, s.CronExpression, s.IsEnabled);
            return Results.Ok(ScheduleDto.From(s));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db, IScheduleHangfireSyncService sync) =>
        {
            var s = await db.Schedules.FindAsync(id);
            if (s is null) return Results.NotFound();
            db.Schedules.Remove(s);
            await db.SaveChangesAsync();
            await sync.RemoveScheduleAsync(id);
            return Results.NoContent();
        });
    }

    public record CreateScheduleRequest(string? Name, string CronExpression, bool IsEnabled = true);
    public record UpdateScheduleRequest(string? Name, string? CronExpression, bool? IsEnabled);
}
