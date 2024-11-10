using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EventPrice",
                table: "Events",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPrice",
                table: "Events");
        }
    }
}
