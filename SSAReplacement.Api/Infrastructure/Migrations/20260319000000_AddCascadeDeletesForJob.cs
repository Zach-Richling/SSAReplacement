using FluentMigrator;
using System.Data;

namespace SSAReplacement.Api.Infrastructure.Migrations;

//[Migration(20260319000000)]
public class AddCascadeDeletesForJob : Migration
{
    public override void Up()
    {
        Create.ForeignKey("FK_JobVariable_Job")
            .FromTable("JobVariable").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobRun_Job")
            .FromTable("JobRun").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobSchedule_Job")
            .FromTable("JobSchedule").ForeignColumn("JobId")
            .ToTable("Job").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobLog_JobRun")
            .FromTable("JobLog").ForeignColumn("JobRunId")
            .ToTable("JobRun").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_JobVariable_Job");
        Delete.ForeignKey("FK_JobRun_Job");
        Delete.ForeignKey("FK_JobSchedule_Job");
        Delete.ForeignKey("FK_JobLog_JobRun");
    }
}
