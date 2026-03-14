using Hangfire;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.DTOs;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;

namespace SSAReplacement.Api.Endpoints;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/jobs").WithTags("Jobs");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var list = await db.Jobs
                .AsNoTracking()
                .OrderBy(j => j.Id)
                .ToListAsync();

            return Results.Ok(list.Select(j => JobDto.From(j)));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var job = await db.Jobs
                .AsNoTracking()
                .Include(j => j.Executable)
                    .ThenInclude(ex => ex.Versions)
                .Include(j => j.Variables)
                .Include(j => j.JobSchedules)
                    .ThenInclude(js => js.Schedule)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            return job is null ? Results.NotFound() : Results.Ok(JobDetailDto.From(job));
        });

        group.MapPost("/", async (CreateJobRequest req, AppDbContext db) =>
        {
            if (await db.Executables.FindAsync(req.ExecutableId) is null)
                return Results.NotFound("Executable not found");

            var job = new Job
            {
                ExecutableId = req.ExecutableId,
                Name = req.Name,
                IsEnabled = req.IsEnabled,
                NotifyEmail = req.NotifyEmail
            };

            db.Jobs.Add(job);

            await db.SaveChangesAsync();
            return Results.Created($"/jobs/{job.Id}", JobDto.From(job));
        });

        group.MapPut("/{id:int}", async (int id, UpdateJobRequest req, AppDbContext db) =>
        {
            var j = await db.Jobs.FindAsync(id);

            if (j is null)
                return Results.NotFound();

            if (req.ExecutableId is int eid) j.ExecutableId = eid;
            if (req.Name is not null) j.Name = req.Name;
            if (req.IsEnabled is bool en) j.IsEnabled = en;
            if (req.NotifyEmail is not null) j.NotifyEmail = req.NotifyEmail;

            await db.SaveChangesAsync();

            return Results.Ok(JobDto.From(j));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var j = await db.Jobs.FindAsync(id);

            if (j is null)
                return Results.NotFound();

            db.Jobs.Remove(j);

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapPut("/{id:int}/schedules", async (int id, CreateJobSchedulesRequest request, AppDbContext db) =>
        {
            var job = await db.Jobs
                .Include(j => j.JobSchedules)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job is null)
                return Results.NotFound();

            job.JobSchedules.Clear();
            foreach (var sid in request.ScheduleIds ?? [])
            {
                if (await db.Schedules.AnyAsync(s => s.Id == sid))
                    job.JobSchedules.Add(new JobSchedule { ScheduleId = sid });
            }

            await db.SaveChangesAsync();

            return Results.Ok();
        });

        group.MapGet("/{id:int}/parameters", async (int id, AppDbContext db) =>
        {
            var job = await db.Jobs
                .Include(j => j.Variables)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job is null)
                return Results.NotFound();

            return Results.Ok(job.Variables.Select(v => JobVariableDto.From(v)));
        });

        group.MapPut("/{id:int}/parameters", async (int id, CreateJobParametersRequest request, AppDbContext db) =>
        {
            var job = await db.Jobs
                .Include(j => j.Variables)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job is null)
                return Results.NotFound();

            var executable = await db.ExecutableVersions
                .Include(ev => ev.Parameters)
                .Where(ev => ev.ExecutableId == job.ExecutableId && ev.IsActive)
                .FirstAsync();

            job.Variables.Clear();
            foreach (var (key, value) in request.Parameters.Where(x => x.Value is not null))
            {
                var exeParam = executable.Parameters.FirstOrDefault(x => x.Name == key);
                if (exeParam?.DefaultValue == value)
                {
                    continue;
                }

                job.Variables.Add(new JobVariable { Key = key, Value = value });
            }

            await db.SaveChangesAsync();

            return Results.Ok();
        });

        group.MapPost("/{id:int}/trigger", async (int id, AppDbContext db, JobRunnerService runner, IBackgroundJobClient jobClient) =>
        {
            var exists = await db.Jobs.AnyAsync(j => j.Id == id);

            if (!exists)
                return Results.NotFound();

            jobClient.Enqueue<JobRunnerService>(s => s.RunAsync(id, null, CancellationToken.None));

            return Results.Accepted();
        });

        group.MapGet("/{id:int}/runs", async (int id, AppDbContext db) =>
        {
            var runs = await db.JobRuns
                .AsNoTracking()
                .Where(r => r.JobId == id)
                .OrderByDescending(r => r.StartedAt)
                .ToListAsync();

            return Results.Ok(runs.Select(JobRunDto.From));
        });
    }

    public record CreateJobRequest(int ExecutableId, string Name, bool IsEnabled = true, string? NotifyEmail = null);
    public record UpdateJobRequest(int? ExecutableId, string? Name, bool? IsEnabled, string? NotifyEmail = null);
    public record CreateJobSchedulesRequest(int[]? ScheduleIds);
    public record CreateJobParametersRequest(Dictionary<string, string> Parameters);
}
