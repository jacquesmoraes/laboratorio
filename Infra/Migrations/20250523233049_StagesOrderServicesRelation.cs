using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class StagesOrderServicesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionStages_ServiceOrders_ServiceOrderId",
                table: "ProductionStages");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceOrderId",
                table: "ProductionStages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionStages_ServiceOrders_ServiceOrderId",
                table: "ProductionStages",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "ServiceOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionStages_ServiceOrders_ServiceOrderId",
                table: "ProductionStages");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceOrderId",
                table: "ProductionStages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionStages_ServiceOrders_ServiceOrderId",
                table: "ProductionStages",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "ServiceOrderId");
        }
    }
}
