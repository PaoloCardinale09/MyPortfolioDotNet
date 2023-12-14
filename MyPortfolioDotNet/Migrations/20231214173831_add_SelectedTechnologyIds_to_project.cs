using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioDotNet.Migrations
{
    /// <inheritdoc />
    public partial class add_SelectedTechnologyIds_to_project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedTechnologyIds",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedTechnologyIds",
                table: "Project");
        }
    }
}
