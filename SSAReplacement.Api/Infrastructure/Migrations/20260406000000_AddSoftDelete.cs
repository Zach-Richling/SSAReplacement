using FluentMigrator;
using System.Data;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20260406000000)]
public class AddSoftDelete : Migration
{
    private static readonly string[] Tables =
    [
        "Executable", "ExecutableVersion",
        "Job", "JobStep", "JobStepParameter", "JobSchedule",
        "JobRun", "Schedule"
    ];

    public override void Up()
    {
        foreach (var table in Tables)
        {
            Alter.Table(table)
                .AddColumn("RecStatus").AsByte().NotNullable().WithDefaultValue(1)
                .AddColumn("DeletedByUserId").AsInt64().Nullable()
                .AddColumn("DeletedDate").AsDateTime2().Nullable();

            Create.ForeignKey($"FK_{table}_DeletedByUser")
                .FromTable(table).ForeignColumn("DeletedByUserId")
                .ToTable("User").PrimaryColumn("Id")
                .OnDelete(Rule.None);
        }
    }

    public override void Down()
    {
        foreach (var table in Tables)
        {
            Delete.ForeignKey($"FK_{table}_DeletedByUser").OnTable(table);
            Delete.Column("RecStatus").FromTable(table);
            Delete.Column("DeletedByUserId").FromTable(table);
            Delete.Column("DeletedDate").FromTable(table);
        }
    }
}
