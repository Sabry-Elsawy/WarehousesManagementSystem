using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOutboundCycleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SO_Items");

            migrationBuilder.RenameColumn(
                name: "CustomerRef",
                table: "SalesOrders",
                newName: "SO_Number");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "Pickings",
                newName: "QuantityToPick");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "SO_Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Pickings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "QuantityPicked",
                table: "Pickings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "QuantityPicked",
                table: "Pickings");

            migrationBuilder.RenameColumn(
                name: "SO_Number",
                table: "SalesOrders",
                newName: "CustomerRef");

            migrationBuilder.RenameColumn(
                name: "QuantityToPick",
                table: "Pickings",
                newName: "Qty");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SO_Items",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SalesOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Pickings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
