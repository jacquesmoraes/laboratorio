using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class SectorsIncluded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tablePriceItems_TablePrices_TablePriceId",
                table: "tablePriceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_tablePriceItems_WorkTypes_WorkTypeId",
                table: "tablePriceItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tablePriceItems",
                table: "tablePriceItems");

            migrationBuilder.RenameTable(
                name: "tablePriceItems",
                newName: "TablePriceItems");

            migrationBuilder.RenameIndex(
                name: "IX_tablePriceItems_WorkTypeId",
                table: "TablePriceItems",
                newName: "IX_TablePriceItems_WorkTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_tablePriceItems_TablePriceId",
                table: "TablePriceItems",
                newName: "IX_TablePriceItems_TablePriceId");

            migrationBuilder.RenameColumn(
                name: "Step",
                table: "ProductionStages",
                newName: "SectorId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "ProductionStages",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TablePriceItems",
                table: "TablePriceItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    SectorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.SectorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionStages_SectorId",
                table: "ProductionStages",
                column: "SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionStages_Sectors_SectorId",
                table: "ProductionStages",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "SectorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionStages_Sectors_SectorId",
                table: "ProductionStages");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TablePriceItems",
                table: "TablePriceItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductionStages_SectorId",
                table: "ProductionStages");

            migrationBuilder.RenameTable(
                name: "TablePriceItems",
                newName: "tablePriceItems");

            migrationBuilder.RenameIndex(
                name: "IX_TablePriceItems_WorkTypeId",
                table: "tablePriceItems",
                newName: "IX_tablePriceItems_WorkTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TablePriceItems_TablePriceId",
                table: "tablePriceItems",
                newName: "IX_tablePriceItems_TablePriceId");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                table: "ProductionStages",
                newName: "Step");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "ProductionStages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tablePriceItems",
                table: "tablePriceItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tablePriceItems_TablePrices_TablePriceId",
                table: "tablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tablePriceItems_WorkTypes_WorkTypeId",
                table: "tablePriceItems",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
