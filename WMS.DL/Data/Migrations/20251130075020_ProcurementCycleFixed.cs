using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProcurementCycleFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Putaways_ReceiptItems_ReceiptItemId",
                table: "Putaways");

            migrationBuilder.RenameColumn(
                name: "RecievedDate",
                table: "Receipts",
                newName: "ReceivedDate");

            migrationBuilder.RenameColumn(
                name: "ReceivedQty",
                table: "ReceiptItems",
                newName: "QtyReceived");

            migrationBuilder.RenameColumn(
                name: "ExpectedQty",
                table: "ReceiptItems",
                newName: "QtyExpected");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "PO_Items",
                newName: "QtyReceived");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "ASNs",
                newName: "ASN_Number");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "ASN_Items",
                newName: "QtyShipped");

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "Receipts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ASNItemId",
                table: "ReceiptItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscrepancyType",
                table: "ReceiptItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedArrivalDate",
                table: "POs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PO_Number",
                table: "POs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineStatus",
                table: "PO_Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QtyOrdered",
                table: "PO_Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "PO_Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "ASNs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LinkedPOItemId",
                table: "ASN_Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_ASNItemId",
                table: "ReceiptItems",
                column: "ASNItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ASN_Items_LinkedPOItemId",
                table: "ASN_Items",
                column: "LinkedPOItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ASN_Items_PO_Items_LinkedPOItemId",
                table: "ASN_Items",
                column: "LinkedPOItemId",
                principalTable: "PO_Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Putaways_ReceiptItems_ReceiptItemId",
                table: "Putaways",
                column: "ReceiptItemId",
                principalTable: "ReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_ASN_Items_ASNItemId",
                table: "ReceiptItems",
                column: "ASNItemId",
                principalTable: "ASN_Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ASN_Items_PO_Items_LinkedPOItemId",
                table: "ASN_Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Putaways_ReceiptItems_ReceiptItemId",
                table: "Putaways");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_ASN_Items_ASNItemId",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_ASNItemId",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ASN_Items_LinkedPOItemId",
                table: "ASN_Items");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ASNItemId",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "DiscrepancyType",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ExpectedArrivalDate",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "PO_Number",
                table: "POs");

            migrationBuilder.DropColumn(
                name: "LineStatus",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "QtyOrdered",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "PO_Items");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "ASNs");

            migrationBuilder.DropColumn(
                name: "LinkedPOItemId",
                table: "ASN_Items");

            migrationBuilder.RenameColumn(
                name: "ReceivedDate",
                table: "Receipts",
                newName: "RecievedDate");

            migrationBuilder.RenameColumn(
                name: "QtyReceived",
                table: "ReceiptItems",
                newName: "ReceivedQty");

            migrationBuilder.RenameColumn(
                name: "QtyExpected",
                table: "ReceiptItems",
                newName: "ExpectedQty");

            migrationBuilder.RenameColumn(
                name: "QtyReceived",
                table: "PO_Items",
                newName: "Qty");

            migrationBuilder.RenameColumn(
                name: "ASN_Number",
                table: "ASNs",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "QtyShipped",
                table: "ASN_Items",
                newName: "Qty");

            migrationBuilder.AddForeignKey(
                name: "FK_Putaways_ReceiptItems_ReceiptItemId",
                table: "Putaways",
                column: "ReceiptItemId",
                principalTable: "ReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
