using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_FYP_backend.Migrations
{
    /// <inheritdoc />
    public partial class updatedagainNodeProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeProducts_AmazonNodes_ParentNodeId",
                table: "NodeProducts");

            migrationBuilder.DropIndex(
                name: "IX_NodeProducts_ParentNodeId",
                table: "NodeProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NodeProducts_ParentNodeId",
                table: "NodeProducts",
                column: "ParentNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeProducts_AmazonNodes_ParentNodeId",
                table: "NodeProducts",
                column: "ParentNodeId",
                principalTable: "AmazonNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
