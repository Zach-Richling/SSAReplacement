using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : DbContext(options)
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

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

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
            b.HasOne<User>().WithMany().HasForeignKey(j => j.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(j => j.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(j => j.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobStep>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasOne(s => s.Job).WithMany(j => j.Steps).HasForeignKey(s => s.JobId);
            b.HasOne(s => s.Executable).WithMany(e => e.JobSteps).HasForeignKey(s => s.ExecutableId);
            b.HasMany(s => s.Parameters).WithOne(p => p.JobStep).HasForeignKey(p => p.JobStepId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<User>().WithMany().HasForeignKey(s => s.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(s => s.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(s => s.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobStepParameter>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.JobStep).WithMany(s => s.Parameters).HasForeignKey(e => e.JobStepId);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(e => e.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobSchedule>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobSchedules).HasForeignKey(e => e.ScheduleId);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(e => e.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobRun>(b =>
        {
            b.HasKey(jr => jr.Id);
            b.HasOne(e => e.Job).WithMany(j => j.Runs).HasForeignKey(e => e.JobId);
            b.HasOne(e => e.Schedule).WithMany(s => s.JobRuns).HasForeignKey(e => e.ScheduleId);
            b.HasMany(e => e.RunSteps).WithOne(s => s.JobRun).HasForeignKey(s => s.JobRunId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(e => e.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobRunStep>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasOne(s => s.JobRun).WithMany(r => r.RunSteps).HasForeignKey(s => s.JobRunId);
            b.HasOne(s => s.ExecutableVersion).WithMany(v => v.JobRunSteps).HasForeignKey(s => s.ExecutableVersionId);
            b.HasMany(s => s.Logs).WithOne(l => l.JobRunStep).HasForeignKey(l => l.JobRunStepId).OnDelete(DeleteBehavior.Cascade);
            b.HasQueryFilter(s => s.ExecutableVersion.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<JobLog>(b =>
        {
            b.HasKey(jl => jl.Id);
            b.HasOne(e => e.JobRunStep).WithMany(s => s.Logs).HasForeignKey(e => e.JobRunStepId);
            b.HasQueryFilter(e => e.JobRunStep.ExecutableVersion.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<Executable>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(e => e.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(e => e.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<ExecutableVersion>(b =>
        {
            b.HasKey(ev => ev.Id);
            b.HasOne(ev => ev.Executable).WithMany(e => e.Versions).HasForeignKey(ev => ev.ExecutableId);
            b.HasOne<User>().WithMany().HasForeignKey(ev => ev.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(ev => ev.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(ev => ev.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<ExecutableParameter>(b =>
        {
            b.HasKey(e => e.Id);
            b.HasOne(e => e.Version).WithMany(x => x.Parameters).HasForeignKey(e => e.ExecutableVersionId);
            b.HasQueryFilter(e => e.Version.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<Schedule>(b =>
        {
            b.HasKey(s => s.Id);
            b.HasOne<User>().WithMany().HasForeignKey(s => s.CreatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<User>().WithMany().HasForeignKey(s => s.DeletedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasQueryFilter(s => s.RecStatus != RecStatus.Deleted);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshToken");
            b.HasKey(rt => rt.Id);
            b.HasOne<User>().WithMany().HasForeignKey(rt => rt.UserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("User");
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.Sid).IsUnique();
            b.HasMany(u => u.AuditEntries).WithOne(a => a.User).HasForeignKey(a => a.UserId).IsRequired(false);
        });

        modelBuilder.Entity<AuditEntry>(b =>
        {
            b.ToTable("AuditEntry");
            b.HasKey(a => a.Id);
            b.HasOne(a => a.User).WithMany(u => u.AuditEntries).HasForeignKey(a => a.UserId).IsRequired(false);
            b.HasIndex(a => a.UserId);
            b.HasIndex(a => new { a.EntityName, a.EntityId });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Convert hard-deletes to soft-deletes for ISoftDeletable entities.
        // Must run before audit snapshot so the snapshot sees Modified state.
        var softDeletedEntities = new HashSet<object>();
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted).ToList())
        {
            softDeletedEntities.Add(entry.Entity);
            entry.State = EntityState.Modified;
            entry.Entity.RecStatus = RecStatus.Deleted;
            entry.Entity.DeletedByUserId = currentUserService.UserId;
            entry.Entity.DeletedDate = DateTime.UtcNow;
        }

        // Snapshot IAuditable entries and their states before save (IDs assigned after INSERT).
        // Capture OldValues/NewValues now while OriginalValues/CurrentValues are still intact.
        var auditableEntries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable &&
                        e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => (
                Entry: e,
                State: e.State,
                Entity: (IAuditable)e.Entity,
                OldValues: e.State == EntityState.Deleted
                    ? SerializePropertyValues(e.OriginalValues)
                    : e.State == EntityState.Modified
                        ? SerializeChangedOldValues(e)
                        : null,
                NewValues: e.State == EntityState.Modified
                    ? SerializeChangedNewValues(e)
                    : null   // Added: captured after save; Deleted: null
            ))
            .ToList();

        // Resolve the current user ID once (null for background/system actions)
        var userId = auditableEntries.Count > 0 ? currentUserService.UserId : null;

        if (auditableEntries.Count > 0)
        {
            // Set CreatedByUserId on new entities before the primary save
            foreach (var (_, state, entity, _, _) in auditableEntries)
            {
                if (state == EntityState.Added)
                    entity.CreatedByUserId = userId;
            }
        }

        // Primary save — DB assigns IDs to new entities
        var result = await base.SaveChangesAsync(cancellationToken);

        // Write audit entries using IDs now populated on entity objects
        if (auditableEntries.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var (entry, state, _, oldValues, newValues) in auditableEntries)
            {
                var entityId = (long)entry.Property("Id").CurrentValue!;
                var action = softDeletedEntities.Contains(entry.Entity)
                    ? "SoftDeleted"
                    : state switch
                    {
                        EntityState.Added => "Created",
                        EntityState.Modified => "Updated",
                        EntityState.Deleted => "Deleted",
                        _ => "Unknown"
                    };

                AuditEntries.Add(new AuditEntry
                {
                    UserId = userId,
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entityId,
                    Action = action,
                    OccurredAt = now,
                    OldValues = oldValues,
                    NewValues = state == EntityState.Added
                        ? SerializePropertyValues(entry.CurrentValues)
                        : newValues
                });
            }

            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private static string SerializePropertyValues(PropertyValues values) =>
        JsonSerializer.Serialize(
            values.Properties.ToDictionary(p => p.Name, p => values[p])
        );

    private static string SerializeChangedOldValues(EntityEntry entry) =>
        JsonSerializer.Serialize(
            entry.Properties
                .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue)
        );

    private static string SerializeChangedNewValues(EntityEntry entry) =>
        JsonSerializer.Serialize(
            entry.Properties
                .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)
        );
}
