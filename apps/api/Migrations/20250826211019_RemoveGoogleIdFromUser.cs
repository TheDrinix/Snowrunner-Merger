using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowrunnerMerger.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGoogleIdFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_GoogleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true);

            // Update existing users to have google ids if they exist in OAuthConnections
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""GoogleId"" = oc.""ProviderAccountId""
                FROM ""OAuthConnections"" oc
                WHERE oc.""UserId"" = ""Users"".""Id"" AND oc.""Provider"" = 'google';
            ");
        }
    }
}
