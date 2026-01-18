using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowrunnerMerger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupInviteCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "SaveGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql(
                "UPDATE \"SaveGroups\" SET \"InviteCode\" = UPPER(REPLACE(\"Id\"::text, '-', ''))"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SaveGroups_InviteCode",
                table: "SaveGroups",
                column: "InviteCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaveGroups_InviteCode",
                table: "SaveGroups");

            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "SaveGroups");
        }
    }
}
