using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class OptionalTablePriceInClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TablePrices_TablePriceId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "TablePriceId",
                table: "Clients",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TablePrices_TablePriceId",
                table: "Clients",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TablePrices_TablePriceId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "TablePriceId",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TablePrices_TablePriceId",
                table: "Clients",
                column: "TablePriceId",
                principalTable: "TablePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
