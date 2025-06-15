using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class TablePriceItem_worktype_removedRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TablePriceItems",
                newName: "TablePriceItemId");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "TablePriceItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "TablePriceItems");

            migrationBuilder.RenameColumn(
                name: "TablePriceItemId",
                table: "TablePriceItems",
                newName: "Id");

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
