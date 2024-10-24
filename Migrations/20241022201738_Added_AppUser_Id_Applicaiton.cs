using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class Added_AppUser_Id_Applicaiton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "BursaryApplications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_AppUserId",
                table: "BursaryApplications",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BursaryApplications_AspNetUsers_AppUserId",
                table: "BursaryApplications",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BursaryApplications_AspNetUsers_AppUserId",
                table: "BursaryApplications");

            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_AppUserId",
                table: "BursaryApplications");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "BursaryApplications");
        }
    }
}
