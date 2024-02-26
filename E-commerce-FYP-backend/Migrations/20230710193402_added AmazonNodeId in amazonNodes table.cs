using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class addedAmazonNodeIdinamazonNodestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmazonNodeId",
                table: "AmazonNodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmazonNodeId",
                table: "AmazonNodes");
        }
    }
}
