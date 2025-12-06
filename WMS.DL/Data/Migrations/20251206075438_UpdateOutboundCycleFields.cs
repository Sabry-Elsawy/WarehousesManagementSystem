using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOutboundCycleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBackorder",
                table: "SO_Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalSOItemId",
                table: "SO_Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOn",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Carrier",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedDate",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalPickingId",
                table: "Pickings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickedBy",
                table: "Pickings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedInventoryId",
                table: "Pickings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShortageQuantity",
                table: "Pickings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortageReason",
                table: "Pickings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedOn",
                table: "Pickings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBackorder",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "OriginalSOItemId",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "CancelledOn",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Carrier",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ShippedDate",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "OriginalPickingId",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "PickedBy",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ReservedInventoryId",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ShortageQuantity",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ShortageReason",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "StartedOn",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "Inventories");
        }
    }
}
