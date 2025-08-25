using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowrunnerMerger.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTokensPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "user_token_pkey",
                table: "UserTokens");

            migrationBuilder.AddPrimaryKey(
                name: "user_token_pkey",
                table: "UserTokens",
                columns: new[] { "UserId", "Token", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "user_token_pkey",
                table: "UserTokens");

            migrationBuilder.AddPrimaryKey(
                name: "user_token_pkey",
                table: "UserTokens",
                columns: new[] { "UserId", "Token" });
        }
    }
}
