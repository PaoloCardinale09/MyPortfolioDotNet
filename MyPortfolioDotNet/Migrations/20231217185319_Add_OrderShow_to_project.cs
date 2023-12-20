using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioDotNet.Migrations
{
    /// <inheritdoc />
    public partial class Add_OrderShow_to_project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderShow",
                table: "Project",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderShow",
                table: "Project");
        }
    }
}
