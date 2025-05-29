using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class TablePriceIdNullableInTablePriceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems");

            migrationBuilder.AlterColumn<int>(
                name: "TablePriceId",
                table: "TablePriceItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems");

            migrationBuilder.AlterColumn<int>(
                name: "TablePriceId",
                table: "TablePriceItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
