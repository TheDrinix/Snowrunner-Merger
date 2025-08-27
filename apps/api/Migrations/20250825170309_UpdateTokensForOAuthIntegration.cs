using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowrunnerMerger.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokensForOAuthIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "UserTokens",
                newName: "ProviderAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountCompletionToken_GoogleId",
                table: "UserTokens",
                newName: "Provider");

            migrationBuilder.AddColumn<string>(
                name: "AccountCompletionToken_Provider",
                table: "UserTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountCompletionToken_ProviderAccountId",
                table: "UserTokens",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OAuthConnections",
                columns: table => new
                {
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ProviderAccountId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthConnections", x => new { x.Provider, x.ProviderAccountId });
                    table.ForeignKey(
                        name: "FK_OAuthConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthConnections_UserId_Provider",
                table: "OAuthConnections",
                columns: new[] { "UserId", "Provider" },
                unique: true);
            
            // Transfer existing google ids from table 'Users' to 'OAuthConnections'
            migrationBuilder.Sql(@"
                INSERT INTO ""OAuthConnections"" (""Provider"", ""ProviderAccountId"", ""UserId"")
                SELECT 
                    'Google' AS ""Provider"", 
                    ""GoogleId"" AS ""ProviderAccountId"",
                    ""Id"" AS ""UserId""
                FROM ""Users"" 
                WHERE ""googleId"" IS NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthConnections");

            migrationBuilder.DropColumn(
                name: "AccountCompletionToken_Provider",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "AccountCompletionToken_ProviderAccountId",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "ProviderAccountId",
                table: "UserTokens",
                newName: "GoogleId");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "UserTokens",
                newName: "AccountCompletionToken_GoogleId");
        }
    }
}
