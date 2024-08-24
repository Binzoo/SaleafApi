using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAnonymous",
                table: "Donations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAnonymous",
                table: "Donations");
        }
    }
}
