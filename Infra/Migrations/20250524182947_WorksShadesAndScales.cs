using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class WorksShadesAndScales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScaleId",
                table: "Works",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShadeId",
                table: "Works",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Works_ScaleId",
                table: "Works",
                column: "ScaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_ShadeId",
                table: "Works",
                column: "ShadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Scales_ScaleId",
                table: "Works",
                column: "ScaleId",
                principalTable: "Scales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Shades_ShadeId",
                table: "Works",
                column: "ShadeId",
                principalTable: "Shades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Scales_ScaleId",
                table: "Works");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Shades_ShadeId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_ScaleId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_ShadeId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ScaleId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ShadeId",
                table: "Works");
        }
    }
}
