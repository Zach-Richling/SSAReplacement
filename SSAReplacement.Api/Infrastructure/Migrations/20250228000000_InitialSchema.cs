using FluentMigrator;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20250228000000)]
public sealed class InitialSchema : Migration
{
    public override void Up()
    {
        Create.Table("Schedule")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(256).Nullable()
            .WithColumn("CronExpression").AsString(256).NotNullable()
            .WithColumn("IsEnabled").AsBoolean().NotNullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable();

        Create.Table("Executable")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(256).Nullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable();

        Create.Table("Job")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ExecutableId").AsInt32().NotNullable()
            .WithColumn("Name").AsString(256).NotNullable()
            .WithColumn("IsEnabled").AsBoolean().NotNullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable()
            .WithColumn("WebhookUrl").AsString(2048).Nullable()
            .WithColumn("NotifyEmail").AsString(256).Nullable();

        Create.Table("JobSchedule")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt32().NotNullable()
            .WithColumn("ScheduleId").AsInt32().NotNullable();

        Create.Table("ExecutableVersion")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ExecutableId").AsInt32().NotNullable()
            .WithColumn("Version").AsString(64).NotNullable()
            .WithColumn("Path").AsString(2048).NotNullable()
            .WithColumn("EntryPointDll").AsString(256).NotNullable()
            .WithColumn("UploadedAt").AsDateTime2().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable();

        Create.Table("JobVariable")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt32().NotNullable()
            .WithColumn("Key").AsString(256).NotNullable()
            .WithColumn("Value").AsString(4096).NotNullable();

        Create.Table("JobRun")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt32().NotNullable()
            .WithColumn("ExecutableVersionId").AsInt32().NotNullable()
            .WithColumn("ScheduleId").AsInt32().Nullable()
            .WithColumn("StartedAt").AsDateTime2().NotNullable()
            .WithColumn("FinishedAt").AsDateTime2().Nullable()
            .WithColumn("Status").AsString(32).NotNullable()
            .WithColumn("ExitCode").AsInt32().Nullable()
            .WithColumn("Trigger").AsString(32).Nullable();

        Create.Table("JobLog")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("JobRunId").AsInt32().NotNullable()
            .WithColumn("LogType").AsString(16).NotNullable()
            .WithColumn("Content").AsString().NotNullable();

        Create.Table("ExecutableParameter")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ExecutableVersionId").AsInt32().NotNullable()
            .WithColumn("Name").AsString(256).NotNullable()
            .WithColumn("TypeName").AsString(256).NotNullable()
            .WithColumn("Description").AsString(1024).Nullable()
            .WithColumn("Required").AsBoolean().NotNullable()
            .WithColumn("DefaultValue").AsString(4096).Nullable();
    }

    public override void Down()
    {
        Delete.Table("JobLog");
        Delete.Table("JobRun");
        Delete.Table("JobVariable");
        Delete.Table("ExecutableVersion");
        Delete.Table("JobSchedule");
        Delete.Table("Job");
        Delete.Table("Executable");
        Delete.Table("Schedule");
    }
}
