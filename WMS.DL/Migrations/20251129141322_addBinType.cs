using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addBinType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BinTypeId",
                table: "Bins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BinType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bins_BinTypeId",
                table: "Bins",
                column: "BinTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bins_BinType_BinTypeId",
                table: "Bins",
                column: "BinTypeId",
                principalTable: "BinType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bins_BinType_BinTypeId",
                table: "Bins");

            migrationBuilder.DropTable(
                name: "BinType");

            migrationBuilder.DropIndex(
                name: "IX_Bins_BinTypeId",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "BinTypeId",
                table: "Bins");
        }
    }
}
