using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnowrunnerMerger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMapsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapStoredSaveInfo",
                columns: table => new
                {
                    DiscoveredMapsId = table.Column<string>(type: "text", nullable: false),
                    StoredSaveInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapStoredSaveInfo", x => new { x.DiscoveredMapsId, x.StoredSaveInfoId });
                    table.ForeignKey(
                        name: "FK_MapStoredSaveInfo_Maps_DiscoveredMapsId",
                        column: x => x.DiscoveredMapsId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapStoredSaveInfo_StoredSaves_StoredSaveInfoId",
                        column: x => x.StoredSaveInfoId,
                        principalTable: "StoredSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Maps",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "RU_02", "Taymyr" },
                    { "RU_03", "Kola Peninsula" },
                    { "RU_04", "Amur" },
                    { "RU_05", "Don" },
                    { "RU_08", "Belozersk Glades" },
                    { "RU_13", "Almaty Region" },
                    { "US_01", "Michigan" },
                    { "US_02", "Alaska" },
                    { "US_03", "Wisconsin" },
                    { "US_04", "Yukon" },
                    { "US_06", "Maine" },
                    { "US_07", "Tennessee" },
                    { "US_09", "Ontario" },
                    { "US_10", "British Columbia" },
                    { "US_11", "Scandinavia" },
                    { "US_12", "North Carolina" },
                    { "US_14", "Austria" },
                    { "US_15", "Quebec" },
                    { "US_16", "Washington" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapStoredSaveInfo_StoredSaveInfoId",
                table: "MapStoredSaveInfo",
                column: "StoredSaveInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapStoredSaveInfo");

            migrationBuilder.DropTable(
                name: "Maps");
        }
    }
}
