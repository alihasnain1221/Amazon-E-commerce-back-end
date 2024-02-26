using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class removedprofitPercentagecolumnfromamazonNodestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfitPercentage",
                table: "AmazonNodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProfitPercentage",
                table: "AmazonNodes",
                type: "decimal(3,3)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
