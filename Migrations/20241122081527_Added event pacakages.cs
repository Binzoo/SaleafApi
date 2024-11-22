using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedeventpacakages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPrice",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Currecny",
                table: "EventRegistrations",
                newName: "Currency");

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "EventRegistrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackageName = table.Column<string>(type: "text", nullable: false),
                    PackagePrice = table.Column<double>(type: "double precision", nullable: false),
                    PackageDescription = table.Column<string>(type: "text", nullable: true),
                    EventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageId);
                    table.ForeignKey(
                        name: "FK_Packages_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_PackageId",
                table: "EventRegistrations",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_EventId",
                table: "Packages",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_Packages_PackageId",
                table: "EventRegistrations",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_Packages_PackageId",
                table: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_PackageId",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "EventRegistrations");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "EventRegistrations",
                newName: "Currecny");

            migrationBuilder.AddColumn<double>(
                name: "EventPrice",
                table: "Events",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
