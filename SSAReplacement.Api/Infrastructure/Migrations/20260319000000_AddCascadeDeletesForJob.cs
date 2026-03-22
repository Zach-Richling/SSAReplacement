using FluentMigrator;
using System.Data;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20260319000000)]
public class AddCascadeDeletesForJob : Migration
{
    public override void Up()
    {
        Create.ForeignKey("FK_JobStep_Job")
            .FromTable("JobStep").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobStepParameter_JobStep")
            .FromTable("JobStepParameter").ForeignColumn("JobStepId")
            .ToTable("JobStep").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobRun_Job")
            .FromTable("JobRun").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobSchedule_Job")
            .FromTable("JobSchedule").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobRunStep_JobRun")
            .FromTable("JobRunStep").ForeignColumn("JobRunId")
            .ToTable("JobRun").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobLog_JobRunStep")
            .FromTable("JobLog").ForeignColumn("JobRunStepId")
            .ToTable("JobRunStep").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_JobStep_Job");
        Delete.ForeignKey("FK_JobStepParameter_JobStep");
        Delete.ForeignKey("FK_JobRun_Job");
        Delete.ForeignKey("FK_JobSchedule_Job");
        Delete.ForeignKey("FK_JobRunStep_JobRun");
        Delete.ForeignKey("FK_JobLog_JobRunStep");
    }
}
