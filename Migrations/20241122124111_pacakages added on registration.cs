using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class pacakagesaddedonregistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_Packages_PackageId",
                table: "EventRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_PackageId",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "PackageDescription",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "EventRegistrations",
                newName: "NumberOfParticipant");

            migrationBuilder.AddColumn<string>(
                name: "FristName",
                table: "EventRegistrations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "EventRegistrations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PacakageName",
                table: "EventRegistrations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "EventRegistrations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FristName",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "PacakageName",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "EventRegistrations");

            migrationBuilder.RenameColumn(
                name: "NumberOfParticipant",
                table: "EventRegistrations",
                newName: "PackageId");

            migrationBuilder.AddColumn<string>(
                name: "PackageDescription",
                table: "Packages",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_PackageId",
                table: "EventRegistrations",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_Packages_PackageId",
                table: "EventRegistrations",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
