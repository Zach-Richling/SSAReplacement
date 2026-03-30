using FluentMigrator;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20260330000000)]
public class AddAuditEntryValues : Migration
{
    public override void Up()
    {
        Alter.Table("AuditEntry")
            .AddColumn("OldValues").AsString(int.MaxValue).Nullable()
            .AddColumn("NewValues").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Column("OldValues").FromTable("AuditEntry");
        Delete.Column("NewValues").FromTable("AuditEntry");
    }
}
