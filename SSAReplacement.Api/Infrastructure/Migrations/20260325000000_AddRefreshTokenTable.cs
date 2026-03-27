using FluentMigrator;

namespace SSAReplacement.Api.Infrastructure.Migrations;

[Migration(20260325000000)]
public class AddRefreshTokenTable : Migration
{
    public override void Up()
    {
        Create.Table("RefreshToken")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Username").AsString(256).NotNullable()
            .WithColumn("Token").AsString(512).NotNullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable()
            .WithColumn("ExpiresAt").AsDateTime2().NotNullable()
            .WithColumn("IsRevoked").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("ReplacedByToken").AsString(512).Nullable();

        Create.Index("IX_RefreshToken_Token")
            .OnTable("RefreshToken")
            .OnColumn("Token");
    }

    public override void Down()
    {
        Delete.Table("RefreshToken");
    }
}
