using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Schedule> Schedules => Set<Schedule>();

    public DbSet<Executable> Executables => Set<Executable>();
    public DbSet<ExecutableVersion> ExecutableVersions => Set<ExecutableVersion>();
    public DbSet<ExecutableParameter> ExecutableParameters => Set<ExecutableParameter>();

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobStep> JobSteps => Set<JobStep>();
    public DbSet<JobStepParameter> JobStepParameters => Set<JobStepParameter>();
    public DbSet<JobSchedule> JobSchedules => Set<JobSchedule>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<JobRunStep> JobRunSteps => Set<JobRunStep>();
    public DbSet<JobLog> JobLogs => Set<JobLog>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>().ToTable("Job");
        modelBuilder.Entity<JobStep>().ToTable("JobStep");
        modelBuilder.Entity<JobStepParameter>().ToTable("JobStepParameter");
        modelBuilder.Entity<JobSchedule>().ToTable("JobSchedule");
        modelBuilder.Entity<JobRun>().ToTable("JobRun");
        modelBuilder.Entity<JobRunStep>().ToTable("JobRunStep");
        modelBuilder.Entity<JobLog>().ToTable("JobLog");

        modelBuilder.Entity<Executable>().ToTable("Executable");
        modelBuilder.Entity<ExecutableVersion>().ToTable("ExecutableVersion");
        modelBuilder.Entity<ExecutableParameter>().ToTable("ExecutableParameter");

        modelBuilder.Entity<Schedule>().ToTable("Schedule");

        modelBuilder.Entity<Job>(b =>
        {
            b.HasKey(j => j.Id);
            b.HasMany(j => j.Steps).WithOne(s => s.Job).HasForeignKey(s => s.JobId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(j => j.JobSchedules).WithOne(js => js.Job).HasForeignKey(js => js.JobId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(j => j.Runs).WithOne(jr => jr.Job).HasForeignKey(jr => jr.JobId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobStep>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasOne(s => s.Job).WithMany(j => j.Steps).HasForeignKey(s => s.JobId);
            b.HasOne(s => s.Executable).WithMany(e => e.JobSteps).HasForeignKey(s => s.ExecutableId);
            b.HasMany(s => s.Parameters).WithOne(p => p.JobStep).HasForeignKey(p => p.JobStepId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobStepParameter>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.JobStep).WithMany(s => s.Parameters).HasForeignKey(e => e.JobStepId);
        });

        modelBuilder.Entity<JobSchedule>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobSchedules).HasForeignKey(e => e.ScheduleId);
        });

        modelBuilder.Entity<JobRun>(b =>
        {
            b.HasKey(jr => jr.Id);
            b.HasOne(e => e.Job).WithMany(j => j.Runs).HasForeignKey(e => e.JobId);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobRuns).HasForeignKey(e => e.ScheduleId);
            b.HasMany(e => e.RunSteps).WithOne(s => s.JobRun).HasForeignKey(s => s.JobRunId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobRunStep>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasOne(s => s.JobRun).WithMany(r => r.RunSteps).HasForeignKey(s => s.JobRunId);
            b.HasOne(s => s.ExecutableVersion).WithMany(v => v.JobRunSteps).HasForeignKey(s => s.ExecutableVersionId);
            b.HasMany(s => s.Logs).WithOne(l => l.JobRunStep).HasForeignKey(l => l.JobRunStepId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobLog>(b =>
        {
            b.HasKey(jl => jl.Id);
            b.HasOne(e => e.JobRunStep).WithMany(s => s.Logs).HasForeignKey(e => e.JobRunStepId);
        });

        modelBuilder.Entity<Executable>(b =>
        {
            b.HasKey(e => e.Id);
        });

        modelBuilder.Entity<ExecutableVersion>(b =>
        {
            b.HasKey(ev => ev.Id);
            b.HasOne(ev => ev.Executable).WithMany(e => e.Versions).HasForeignKey(ev => ev.ExecutableId);
        });

        modelBuilder.Entity<ExecutableParameter>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Version).WithMany(x => x.Parameters).HasForeignKey(e => e.ExecutableVersionId);
        });

        modelBuilder.Entity<Schedule>(b =>
        {
            b.HasKey(s => s.Id);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshToken");
            b.HasKey(rt => rt.Id);
        });
    }
}
