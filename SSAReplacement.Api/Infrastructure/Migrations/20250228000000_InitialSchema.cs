using FluentMigrator;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20250228000000)]
public sealed class InitialSchema : Migration
{
    public override void Up()
    {
        Create.Table("Job")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString(256).NotNullable()
            .WithColumn("IsEnabled").AsBoolean().NotNullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable()
            .WithColumn("NotifyEmail").AsString(256).Nullable();

        Create.Table("JobStep")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt64().NotNullable()
            .WithColumn("ExecutableId").AsInt64().NotNullable()
            .WithColumn("StepNumber").AsInt32().NotNullable()
            .WithColumn("Name").AsString(256).NotNullable();

        Create.Table("JobStepParameter")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobStepId").AsInt64().NotNullable()
            .WithColumn("Key").AsString(256).NotNullable()
            .WithColumn("Value").AsString(4096).NotNullable();

        Create.Table("JobRun")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt64().NotNullable()
            .WithColumn("ScheduleId").AsInt64().Nullable()
            .WithColumn("CurrentStep").AsInt32().Nullable()
            .WithColumn("StartedAt").AsDateTime2().NotNullable()
            .WithColumn("FinishedAt").AsDateTime2().Nullable()
            .WithColumn("Status").AsString(32).NotNullable()
            .WithColumn("ExitCode").AsInt32().Nullable()
            .WithColumn("Trigger").AsString(32).Nullable();

        Create.Table("JobRunStep")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobRunId").AsInt64().NotNullable()
            .WithColumn("ExecutableVersionId").AsInt64().NotNullable()
            .WithColumn("StepNumber").AsInt32().NotNullable()
            .WithColumn("StepName").AsString(256).NotNullable()
            .WithColumn("StartedAt").AsDateTime2().NotNullable()
            .WithColumn("FinishedAt").AsDateTime2().Nullable()
            .WithColumn("Status").AsString(32).NotNullable()
            .WithColumn("ExitCode").AsInt32().Nullable();

        Create.Table("JobLog")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobRunStepId").AsInt64().NotNullable()
            .WithColumn("LogDate").AsDateTime2().NotNullable()
            .WithColumn("LogType").AsString(16).NotNullable()
            .WithColumn("Content").AsString().NotNullable();

        Create.Table("JobSchedule")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("JobId").AsInt64().NotNullable()
            .WithColumn("ScheduleId").AsInt64().NotNullable();

        Create.Table("Schedule")
           .WithColumn("Id").AsInt64().PrimaryKey().Identity()
           .WithColumn("Name").AsString(256).NotNullable()
           .WithColumn("CronExpression").AsString(256).NotNullable()
           .WithColumn("IsEnabled").AsBoolean().NotNullable()
           .WithColumn("CreatedAt").AsDateTime2().NotNullable();

        Create.Table("Executable")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString(256).NotNullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable();

        Create.Table("ExecutableVersion")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("ExecutableId").AsInt64().NotNullable()
            .WithColumn("Version").AsInt32().NotNullable()
            .WithColumn("EntryPointExe").AsString(256).NotNullable()
            .WithColumn("UploadedAt").AsDateTime2().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable();

        Create.Table("ExecutableParameter")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("ExecutableVersionId").AsInt64().NotNullable()
            .WithColumn("Name").AsString(256).NotNullable()
            .WithColumn("TypeName").AsString(256).NotNullable()
            .WithColumn("Description").AsString(1024).Nullable()
            .WithColumn("Required").AsBoolean().NotNullable()
            .WithColumn("DefaultValue").AsString(4096).Nullable();
    }

    public override void Down()
    {
        Delete.Table("JobLog");
        Delete.Table("JobRunStep");
        Delete.Table("JobRun");
        Delete.Table("JobStepParameter");
        Delete.Table("JobStep");
        Delete.Table("ExecutableVersion");
        Delete.Table("JobSchedule");
        Delete.Table("Job");
        Delete.Table("Executable");
        Delete.Table("Schedule");
        Delete.Table("ExecutableParameter");
    }
}
