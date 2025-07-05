using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class WorktypeTablePriceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "TablePriceItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TablePriceItems",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "WorkTypeId",
                table: "TablePriceItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TablePriceItems_WorkTypeId",
                table: "TablePriceItems",
                column: "WorkTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems");

            migrationBuilder.DropIndex(
                name: "IX_TablePriceItems_WorkTypeId",
                table: "TablePriceItems");

            migrationBuilder.DropColumn(
                name: "WorkTypeId",
                table: "TablePriceItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TablePriceItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "TablePriceItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
