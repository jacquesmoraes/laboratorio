using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems");

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
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePriceItems_WorkTypes_WorkTypeId",
                table: "TablePriceItems",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
