using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class DeletePerOrderPAyment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "OrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "PerOrderPaymentId",
                table: "ServiceOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerOrderPaymentId",
                table: "ServiceOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId",
                principalTable: "OrderPayments",
                principalColumn: "Id");
        }
    }
}
