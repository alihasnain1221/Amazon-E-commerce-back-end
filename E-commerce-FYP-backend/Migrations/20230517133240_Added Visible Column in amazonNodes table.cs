using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedVisibleColumninamazonNodestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "AmazonNodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "AmazonNodes");
        }
    }
}
