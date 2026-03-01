using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Executable> Executables => Set<Executable>();
    public DbSet<ExecutableVersion> ExecutableVersions => Set<ExecutableVersion>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSchedule> JobSchedules => Set<JobSchedule>();
    public DbSet<JobVariable> JobVariables => Set<JobVariable>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<JobLog> JobLogs => Set<JobLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>().ToTable("Schedule");
        modelBuilder.Entity<Executable>().ToTable("Executable");
        modelBuilder.Entity<ExecutableVersion>().ToTable("ExecutableVersion");
        modelBuilder.Entity<Job>().ToTable("Job");
        modelBuilder.Entity<JobSchedule>().ToTable("JobSchedule");
        modelBuilder.Entity<JobVariable>().ToTable("JobVariable");
        modelBuilder.Entity<JobRun>().ToTable("JobRun");
        modelBuilder.Entity<JobLog>().ToTable("JobLog");

        modelBuilder.Entity<Job>(b =>
        {
            b.HasOne(e => e.Executable).WithMany(x => x.Jobs).HasForeignKey(e => e.ExecutableId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JobSchedule>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasKey(e => new { e.JobId, e.ScheduleId });
            b.HasOne(e => e.Job).WithMany(j => j.JobSchedules).HasForeignKey(e => e.JobId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobSchedules).HasForeignKey(e => e.ScheduleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExecutableVersion>(b =>
        {
            b.HasOne(e => e.Executable).WithMany(x => x.Versions).HasForeignKey(e => e.ExecutableId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobVariable>(b =>
        {
            b.HasOne(e => e.Job).WithMany(j => j.Variables).HasForeignKey(e => e.JobId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobRun>(b =>
        {
            b.HasOne(e => e.Job).WithMany(j => j.Runs).HasForeignKey(e => e.JobId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.ExecutableVersion).WithMany(v => v.JobRuns).HasForeignKey(e => e.ExecutableVersionId);
            b.HasOne(e => e.Schedule).WithMany().HasForeignKey(e => e.ScheduleId);
        });

        modelBuilder.Entity<JobLog>(b =>
        {
            b.HasOne(e => e.JobRun).WithMany(r => r.Logs).HasForeignKey(e => e.JobRunId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
