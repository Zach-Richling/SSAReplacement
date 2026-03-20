using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Executable> Executables => Set<Executable>();
    public DbSet<ExecutableVersion> ExecutableVersions => Set<ExecutableVersion>();
    public DbSet<ExecutableParameter> ExecutableParameters => Set<ExecutableParameter>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSchedule> JobSchedules => Set<JobSchedule>();
    public DbSet<JobVariable> JobVariables => Set<JobVariable>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<JobLog> JobLogs => Set<JobLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>().ToTable("Job");
        modelBuilder.Entity<JobSchedule>().ToTable("JobSchedule");
        modelBuilder.Entity<JobRun>().ToTable("JobRun");
        modelBuilder.Entity<JobVariable>().ToTable("JobVariable");
        modelBuilder.Entity<JobLog>().ToTable("JobLog");

        modelBuilder.Entity<Executable>().ToTable("Executable");
        modelBuilder.Entity<ExecutableVersion>().ToTable("ExecutableVersion");
        modelBuilder.Entity<ExecutableParameter>().ToTable("ExecutableParameter");

        modelBuilder.Entity<Schedule>().ToTable("Schedule");

        modelBuilder.Entity<Job>(b =>
        {
            b.HasKey(j => j.Id);
            b.HasOne(j => j.Executable).WithMany(e => e.Jobs).HasForeignKey(e => e.ExecutableId);
            b.HasMany(j => j.JobSchedules).WithOne(js => js.Job).HasForeignKey(js => js.JobId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(j => j.Variables).WithOne(v => v.Job).HasForeignKey(v => v.JobId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(j => j.Runs).WithOne(jr => jr.Job).HasForeignKey(jr => jr.JobId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
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
            b.HasOne(e => e.ExecutableVersion).WithMany(v => v.JobRuns).HasForeignKey(e => e.ExecutableVersionId);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobRuns).HasForeignKey(e => e.ScheduleId);
        });

        modelBuilder.Entity<JobVariable>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Job).WithMany(j => j.Variables).HasForeignKey(e => e.JobId);
        });

        modelBuilder.Entity<JobLog>(b =>
        {
            b.HasKey(jl => jl.Id);
            b.HasOne(e => e.JobRun).WithMany(r => r.Logs).HasForeignKey(e => e.JobRunId);
        });


        modelBuilder.Entity<ExecutableVersion>(b =>
        {
            b.HasOne(e => e.Executable).WithMany(x => x.Versions).HasForeignKey(e => e.ExecutableId);
        });

        modelBuilder.Entity<ExecutableParameter>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Version).WithMany(x => x.Parameters).HasForeignKey(e => e.ExecutableVersionId);
        });

        modelBuilder.Entity<Schedule>(b => b.HasKey(s => s.Id));
    }
}
