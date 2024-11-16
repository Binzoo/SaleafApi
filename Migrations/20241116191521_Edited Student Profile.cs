using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeleafAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditedStudentProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Handle cases where Skills is a single string without commas
            // Enclose single string values in curly braces for array conversion
            migrationBuilder.Sql(
                "UPDATE \"StudentProfiles\" SET \"Skills\" = '{\"' || REPLACE(REPLACE(\"Skills\", '\"', ''), '''', '') || '\"}' " +
                "WHERE \"Skills\" IS NOT NULL AND POSITION(',' IN \"Skills\") = 0;");

            // Transform existing data with commas into a valid PostgreSQL array literal
            // This step transforms comma-separated values into a properly formatted array
            migrationBuilder.Sql(
                "UPDATE \"StudentProfiles\" SET \"Skills\" = '{\"' || REPLACE(REPLACE(REPLACE(\"Skills\", '\"', ''), '''', ''), ', ', '\", \"') || '\"}' " +
                "WHERE \"Skills\" IS NOT NULL AND POSITION(',' IN \"Skills\") > 0;");

            // Use raw SQL to alter the Skills column with explicit casting
            migrationBuilder.Sql(
                "ALTER TABLE \"StudentProfiles\" ALTER COLUMN \"Skills\" TYPE text[] USING \"Skills\"::text[];");

            // Add the new column as originally intended
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "StudentProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the ImageUrl column
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "StudentProfiles");

            // Revert the Skills column back to its original type
            migrationBuilder.Sql(
                "ALTER TABLE \"StudentProfiles\" ALTER COLUMN \"Skills\" TYPE text USING array_to_string(\"Skills\", ', ');");
        }
    }
}
