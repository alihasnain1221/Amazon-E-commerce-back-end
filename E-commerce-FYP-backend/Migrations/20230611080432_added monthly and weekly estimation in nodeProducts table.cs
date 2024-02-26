using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class addedmonthlyandweeklyestimationinnodeProductstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlySalesEstimation",
                table: "NodeProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeeklySalesEstimation",
                table: "NodeProducts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlySalesEstimation",
                table: "NodeProducts");

            migrationBuilder.DropColumn(
                name: "WeeklySalesEstimation",
                table: "NodeProducts");
        }
    }
}
