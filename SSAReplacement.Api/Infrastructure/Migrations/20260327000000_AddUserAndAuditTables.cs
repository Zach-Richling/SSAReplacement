using FluentMigrator;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20260327000000)]
public class AddUserAndAuditTables : Migration
{
    public override void Up()
    {
        Create.Table("User")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Sid").AsString(128).NotNullable()
            .WithColumn("Username").AsString(256).NotNullable()
            .WithColumn("FirstSeenAt").AsDateTime2().NotNullable()
            .WithColumn("LastSeenAt").AsDateTime2().NotNullable();

        Create.Index("IX_User_Sid")
            .OnTable("User")
            .OnColumn("Sid")
            .Unique();

        Create.Table("AuditEntry")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("UserId").AsInt64().Nullable()
            .WithColumn("EntityName").AsString(64).NotNullable()
            .WithColumn("EntityId").AsInt64().NotNullable()
            .WithColumn("Action").AsString(16).NotNullable()
            .WithColumn("OccurredAt").AsDateTime2().NotNullable();

        Create.ForeignKey("FK_AuditEntry_User")
            .FromTable("AuditEntry").ForeignColumn("UserId")
            .ToTable("User").PrimaryColumn("Id");

        Create.Index("IX_AuditEntry_UserId")
            .OnTable("AuditEntry")
            .OnColumn("UserId");

        Create.Index("IX_AuditEntry_EntityName_EntityId")
            .OnTable("AuditEntry")
            .OnColumn("EntityName").Ascending()
            .OnColumn("EntityId").Ascending();

        // Add UserId FK to RefreshToken
        Alter.Table("RefreshToken")
            .AddColumn("UserId").AsInt64().Nullable();

        Create.ForeignKey("FK_RefreshToken_User")
            .FromTable("RefreshToken").ForeignColumn("UserId")
            .ToTable("User").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.None);

        // Add CreatedByUserId to all IAuditable entity tables
        var auditableTables = new[]
        {
            "Executable", "ExecutableVersion",
            "Job", "JobStep", "JobStepParameter", "JobSchedule",
            "JobRun",
            "Schedule"
        };

        foreach (var table in auditableTables)
        {
            Alter.Table(table)
                .AddColumn("CreatedByUserId").AsInt64().Nullable();

            Create.ForeignKey($"FK_{table}_CreatedByUser")
                .FromTable(table).ForeignColumn("CreatedByUserId")
                .ToTable("User").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            Create.Index($"IX_{table}_CreatedByUserId")
                .OnTable(table)
                .OnColumn("CreatedByUserId");
        }
    }

    public override void Down()
    {
        var auditableTables = new[]
        {
            "Executable", "ExecutableVersion",
            "Job", "JobStep", "JobStepParameter", "JobSchedule",
            "JobRun",
            "Schedule"
        };

        foreach (var table in auditableTables)
        {
            Delete.Index($"IX_{table}_CreatedByUserId").OnTable(table);
            Delete.ForeignKey($"FK_{table}_CreatedByUser").OnTable(table);
            Delete.Column("CreatedByUserId").FromTable(table);
        }

        Delete.ForeignKey("FK_RefreshToken_User").OnTable("RefreshToken");
        Delete.Column("UserId").FromTable("RefreshToken");

        Delete.ForeignKey("FK_AuditEntry_User").OnTable("AuditEntry");
        Delete.Table("AuditEntry");
        Delete.Table("User");
    }
}
