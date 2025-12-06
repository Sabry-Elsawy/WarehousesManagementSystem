using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosureAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Zones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Zones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Zones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Zones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Warehouses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Vendors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Vendors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "TransferOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "TransferOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "TransferOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "TransferOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "TransferOrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "TransferOrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "TransferOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "TransferOrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "SO_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "SO_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "SO_Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "SO_Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Receipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Receipts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "ReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "ReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "ReceiptItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "ReceiptItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Racks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Racks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Racks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Racks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Putaways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Putaways",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Putaway_Bins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Putaway_Bins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Putaway_Bins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Putaway_Bins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "POs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "POs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "POs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "POs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "PO_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "PO_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "PO_Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "PO_Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Pickings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Pickings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Pickings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Pickings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "InventoryTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "InventoryTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Inventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "CycleCounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "CycleCounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "CycleCounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "CycleCounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "CycleCountItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "CycleCountItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "CycleCountItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "CycleCountItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "BinTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "BinTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "BinTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "BinTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Bins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Bins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Bins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Bins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "ASNs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "ASNs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "ASNs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "ASNs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "ASN_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "ASN_Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "ASN_Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "ASN_Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Aisles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Aisles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Aisles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoClosed",
                table: "Aisles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "TransferOrders");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "TransferOrders");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "TransferOrders");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "TransferOrders");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "TransferOrderItems");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "TransferOrderItems");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "TransferOrderItems");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "TransferOrderItems");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "SO_Items");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Racks");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Racks");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Racks");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Racks");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Putaways");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Putaways");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Putaway_Bins");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Putaway_Bins");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Putaway_Bins");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Putaway_Bins");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Pickings");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "CycleCounts");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "CycleCounts");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "CycleCounts");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "CycleCounts");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "CycleCountItems");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "CycleCountItems");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "CycleCountItems");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "CycleCountItems");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "BinTypes");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "BinTypes");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "BinTypes");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "BinTypes");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "ASNs");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "ASNs");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "ASNs");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "ASNs");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "ASN_Items");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "ASN_Items");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "ASN_Items");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "ASN_Items");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Aisles");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Aisles");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Aisles");

            migrationBuilder.DropColumn(
                name: "IsAutoClosed",
                table: "Aisles");
        }
    }
}
