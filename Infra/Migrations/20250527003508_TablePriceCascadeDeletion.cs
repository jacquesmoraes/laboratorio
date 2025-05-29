using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class TablePriceCascadeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_TablePrices_TablePriceId",
                table: "TablePriceItems",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id");
        }
    }
}
