using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseOrderProductRelationRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_POs_Products_ProductId",
                table: "POs");

            migrationBuilder.DropIndex(
                name: "IX_POs_ProductId",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "POs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "POs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_POs_ProductId",
                table: "POs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_POs_Products_ProductId",
                table: "POs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
