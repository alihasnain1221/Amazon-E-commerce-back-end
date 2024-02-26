using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class addedimagelinkcolumnincart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "CartProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "CartProducts");
        }
    }
}
