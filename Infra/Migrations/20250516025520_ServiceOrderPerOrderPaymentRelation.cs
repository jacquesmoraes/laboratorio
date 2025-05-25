using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class ServiceOrderPerOrderPaymentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPaymentServiceOrder");

            migrationBuilder.AddColumn<int>(
                name: "PerOrderPaymentId",
                table: "ServiceOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId",
                principalTable: "OrderPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.CreateTable(
                name: "OrderPaymentServiceOrder",
                columns: table => new
                {
                    PaymentsId = table.Column<int>(type: "integer", nullable: false),
                    ServiceOrdersServiceOrderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentServiceOrder", x => new { x.PaymentsId, x.ServiceOrdersServiceOrderId });
                    table.ForeignKey(
                        name: "FK_OrderPaymentServiceOrder_OrderPayments_PaymentsId",
                        column: x => x.PaymentsId,
                        principalTable: "OrderPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaymentServiceOrder_ServiceOrders_ServiceOrdersService~",
                        column: x => x.ServiceOrdersServiceOrderId,
                        principalTable: "ServiceOrders",
                        principalColumn: "ServiceOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentServiceOrder_ServiceOrdersServiceOrderId",
                table: "OrderPaymentServiceOrder",
                column: "ServiceOrdersServiceOrderId");
        }
    }
}
